using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;

using SRNicoNico.ViewModels;
using System.Net;
using System.Xml;
using System.IO;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoStoryBoard {


        private VideoViewModel Owner;

        public NicoNicoStoryBoard(VideoViewModel vm) {

            Owner = vm;
        }

        public async Task<NicoNicoStoryBoardData> GetVideoStoryBoardAsync(string videoUrl) {

            try {

                //プロトコルがhttpじゃない時点で取得不可
                if(videoUrl.StartsWith("rtmp")) {

                    return null;
                }

                var res = await App.ViewModelRoot.CurrentUser.Session.GetResponseAsync(videoUrl + "&sb=1");

                //見せてもらえなかったらnullを返すしか無いね
                if(!res.IsSuccessStatusCode || res.Content.Headers.ContentDisposition.FileName.Contains("smile")) {

                    return null;
                }

                //ニコニコのストーリーボードを返すWebサーバがわけわからん文字エンコーディングを返してくるので
                //仕方なくbyte配列から文字列を復元する
                var a = Encoding.UTF8.GetString(await res.Content.ReadAsByteArrayAsync());

                res.Dispose();


                var xml = new XmlDocument();
                xml.LoadXml(a);

                var storyboard = xml.SelectSingleNode("/smile/storyboard[1]");

                var ret = new NicoNicoStoryBoardData() {

                    Id = storyboard.Attributes["id"] != null ? storyboard.Attributes["id"].Value : "1",
                    Cols = int.Parse(storyboard.SelectSingleNode("board_cols").InnerText),
                    Rows = int.Parse(storyboard.SelectSingleNode("board_rows").InnerText),
                    Count = int.Parse(storyboard.SelectSingleNode("board_number").InnerText),
                    Width = int.Parse(storyboard.SelectSingleNode("thumbnail_width").InnerText),
                    Height = int.Parse(storyboard.SelectSingleNode("thumbnail_height").InnerText),
                    Interval = int.Parse(storyboard.SelectSingleNode("thumbnail_interval").InnerText),
                    Number = int.Parse(storyboard.SelectSingleNode("thumbnail_number").InnerText)
                };

                var imageUrl = videoUrl + "&sb=" + ret.Id + "&board=";

                int bitmapindex = 0;
                for(int i = 1; i <= ret.Count; i++) {

                    var image = await App.ViewModelRoot.CurrentUser.Session.GetResponseAsync(imageUrl + i);

                    var bitmap = new Bitmap(await image.Content.ReadAsStreamAsync());

                    for(int j = 0; j < ret.Cols; j++) {

                        for(int k = 0; k < ret.Rows; k++) {

                            var rect = new Rectangle(ret.Width * k, ret.Height * j, ret.Width, ret.Height);

                            ret.BitmapCollection[bitmapindex] = bitmap.Clone(rect, bitmap.PixelFormat);
                            bitmapindex += ret.Interval;
                        }
                    }
                }

                return ret;
            } catch(Exception) {

                Owner.Status = "ストーリーボードの取得に失敗しました";
                return null;
            }
        }
        



    }


    public class NicoNicoStoryBoardData {


        //ストーリーボードID
        public string Id { get; set; }

        //サムネイル一つの横幅
        public int Width { get; set; }

        //サムネイル一つの縦幅
        public int Height { get; set; }

        //サムネイルの数
        public int Number { get; set; }

        //サムネイルの間隔
        public int Interval { get; set; }

        //縦のサムネイル数
        public int Rows { get; set; }

        //横のサムネイル数
        public int Cols { get; set; }

        //ボード数
        public int Count { get; set; }


        public Dictionary<int, Bitmap> BitmapCollection = new Dictionary<int, Bitmap>();


    }

}
