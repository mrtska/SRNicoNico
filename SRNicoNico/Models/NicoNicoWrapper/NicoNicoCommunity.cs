using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using Livet;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoCommunity : NotificationObject {


        public string CommunityUrl;

        public NicoNicoCommunity(string url) {

            CommunityUrl = url;
        }

        public void GetCommunity() {

            try {

                var a = NicoNicoWrapperMain.Session.GetAsync(CommunityUrl).Result;

                var doc = new HtmlDocument();
                doc.LoadHtml2(a);


                var community_main = doc.DocumentNode.SelectSingleNode("//div[@id='community_main']");

                ;

            } catch(RequestTimeout) {

                return;
            }
        }




    }

    public class NicoNicoCommunityContent : NotificationObject {

        //コミュニティURL
        public string CommunityUrl { get; set; }

        //コミュニティサムネイルURL
        public string ThumbnailUrl { get; set; }

        //オーナーURL
        public string OwnerUrl { get; set; }
        
        //オーナー名
        public string OwnerName { get; set; }

        //コミュニティ名
        public string CommunityTitle { get; set; }

        //開設日
        public string OpeningDate { get; set; }

        //お知らせ
        public IList<NicoNicoCommunityNews> CommunityNews { get; set; }

        //コミュニティレベル
        public string CommunityLevel { get; set; }

        //コミュニティメンバー
        public string CommunityMember { get; set; }

        //コミュニティ登録タグ
        public IList<string> CommunityTags { get; set; }

        //プロフィール
        public string CommunityProfile { get; set; }

        //特権
        public string Privilege { get; set; }

        //累計来場者数
        public string TotalVisitors { get; set; }

        //投稿動画数
        public string Videos { get; set; }
    }

    public class NicoNicoCommunityNews {

        //お知らせタイトル
        public string Title { get; set; }

        //内容
        public string Description { get; set; }

        //日付
        public string Date { get; set; }
    }
    }




}
