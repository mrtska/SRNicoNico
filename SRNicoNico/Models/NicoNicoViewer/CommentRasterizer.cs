using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using System.Timers;

using SRNicoNico.ViewModels;

using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Views.Controls;

namespace SRNicoNico.Models.NicoNicoViewer {

    //コメント描画位置など
    public class CommentRasterizer : NotificationObject {
        
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

            Console.WriteLine("たいまーてすと " + " Time:" + CurrentTime + "テキスト:" + entry.Content);

            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                View.DrawComment(entry);
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
