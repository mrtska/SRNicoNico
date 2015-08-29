using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using System.Timers;

using SRNicoNico.ViewModels;

using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Views.Controls;
using System.Windows.Media;

namespace SRNicoNico.Models.NicoNicoViewer {

    //コメント描画位置など
    public class CommentRasterizer : NotificationObject {


        private static Dictionary<string, Brush> NicoNicoColorMap = new Dictionary<string, Brush>();

        
        static CommentRasterizer() {

            //---全会員共通---
            NicoNicoColorMap["white"] = new SolidColorBrush(Colors.White);
            NicoNicoColorMap["red"] = new SolidColorBrush(Colors.Red);
            NicoNicoColorMap["pink"] = new SolidColorBrush(Color.FromRgb(0xFF, 0x80, 0x80));
            NicoNicoColorMap["orange"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xC0, 0x00));
            NicoNicoColorMap["yellow"] = new SolidColorBrush(Colors.Yellow);
            NicoNicoColorMap["green"] = new SolidColorBrush(Color.FromRgb(0x00, 0xFF, 0x00));
            NicoNicoColorMap["cyan"] = new SolidColorBrush(Colors.Cyan);
            NicoNicoColorMap["blue"] = new SolidColorBrush(Colors.Blue);
            NicoNicoColorMap["purple"] = new SolidColorBrush(Color.FromRgb(0xC0, 0x00, 0xFF));
            NicoNicoColorMap["black"] = new SolidColorBrush(Colors.Black);
            //------

            //---プレミアム会員のみ---
            NicoNicoColorMap["white2"] = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC,0x99));
            NicoNicoColorMap["niconicowhite"] = NicoNicoColorMap["white2"];

            NicoNicoColorMap["red2"] = new SolidColorBrush(Color.FromRgb(0xCC, 0x00, 0x33));
            NicoNicoColorMap["truered"] = NicoNicoColorMap["red2"];

            NicoNicoColorMap["pink2"] = new SolidColorBrush(Color.FromRgb(0xFF, 0x33, 0xCC));

            NicoNicoColorMap["orange2"] = new SolidColorBrush(Color.FromRgb(0xFF, 0x66, 0x00));
            NicoNicoColorMap["passionorange"] = NicoNicoColorMap["orange2"];

            NicoNicoColorMap["yellow2"] = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x00));
            NicoNicoColorMap["madyellow"] = NicoNicoColorMap["yellow2"];

            NicoNicoColorMap["green2"] = new SolidColorBrush(Color.FromRgb(0x00, 0xCC, 0x66));
            NicoNicoColorMap["elementalgreen"] = NicoNicoColorMap["green2"];

            NicoNicoColorMap["cyan2"] = new SolidColorBrush(Color.FromRgb(0x00, 0xCC, 0xCC));

            NicoNicoColorMap["blue2"] = new SolidColorBrush(Color.FromRgb(0x33, 0x99, 0xFF));
            NicoNicoColorMap["marineblue"] = NicoNicoColorMap["blue2"];

            NicoNicoColorMap["purple2"] = new SolidColorBrush(Color.FromRgb(0x66, 0x33, 0xCC));
            NicoNicoColorMap["nobleviolet"] = NicoNicoColorMap["purple2"];

            NicoNicoColorMap["black2"] = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));
            //------

        }






        //タイマー これでコメント描画タイミングを取得する
        private Timer Timer;

        //現在の時間 10ミリ秒 単位
        private int CurrentTime;

        private DispatcherCollection<CommentEntryViewModel> CommentList;

        private readonly CommentView View;

    

        public CommentRasterizer(CommentView view, DispatcherCollection<CommentEntryViewModel> list) {

            View = view;
            CommentList = list;

            Timer = new Timer(10);

            Timer.Elapsed += Timer_Tick;
            Timer.Start();
        }


        //Tick毎の処理
        private void Timer_Tick(object sender, ElapsedEventArgs e) {




            foreach(CommentEntryViewModel vm in CommentList) {

                NicoNicoCommentEntry entry = vm.Entry;

                if(entry.Vpos == CurrentTime) {

                    TriggerComment(entry);
                }
            }


            






            //時間計測
            CurrentTime++;
        }

        //コメントを描画する
        public void TriggerComment(NicoNicoCommentEntry entry) {

            Console.WriteLine("たいまーてすと " + "No:" + entry.No + " Mail:" + entry.Mail + " Time:" + CurrentTime + "テキスト:" + entry.Content);


            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                //コメントカラー
                Brush color = new SolidColorBrush(Colors.White);
                foreach(KeyValuePair<string, Brush> pair in NicoNicoColorMap) {

                    if(entry.Mail.Contains(pair.Key)) {

                        color = pair.Value;
                        break;
                    }
                }

                //コメントサイズ
                double size = 20;
                if(entry.Mail.Contains("big")) {

                    //複数行コメントはbigが指定してあっても大きくしない
                    if(!entry.Content.Contains("\n")) {

                        size = 40;
                    }

                } else if(entry.Mail.Contains("small")) {

                    size = 10;
                }

                double length = 0;
                //複数行コメントだったら
                if(entry.Content.Contains("\n")) {

                    string[] contents = entry.Content.Split('\n');

                    foreach(string text in contents) {

                        length = Math.Max(length, text.Length * size);
                    }
                } else {

                    length = entry.Content.Length * size;
                }


                CommentPosition pos = CommentPosition.Naka;
                if(entry.Mail.Contains("ue")) {

                    pos = CommentPosition.Ue;
                } else if(entry.Mail.Contains("shita")) {

                    pos = CommentPosition.Shita;
                } else {

                    length = -length;
                }


                View.DrawComment(entry.No, entry.Content, color, size, length, pos);
            }));
        }





        //シーク 単位は同じ
        public void Seek(int millis) {

            CurrentTime = millis;
        }
        public void Pause() {
            
            Timer.Enabled = false;
        }
        public void Resume() {

            Timer.Enabled = true;
        }



    }
}
