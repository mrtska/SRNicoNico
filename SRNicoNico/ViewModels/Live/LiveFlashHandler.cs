using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Controls;
using SRNicoNico.Views.Contents.Live;
using AxShockwaveFlashObjects;
using Flash.External;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class LiveFlashHandler : ViewModel {


        public AxShockwaveFlash ShockwaveFlash;

        //Flashの関数を呼ぶためのもの
        public ExternalInterfaceProxy Proxy;

        private LiveWatchViewModel Owner;

        public bool IsInitialized;

        public LiveFlashHandler(LiveWatchViewModel vm, AxShockwaveFlash flash)  {

            Owner = vm;

            ShockwaveFlash = flash;
            Proxy = new ExternalInterfaceProxy(ShockwaveFlash);
            
        }

        public void LoadMovie() {
            
            ShockwaveFlash.LoadMovie(0, Directory.GetCurrentDirectory() + "./Flash/NicoNicoLivePlayer.swf");
            Proxy.ExternalInterfaceCall += new ExternalInterfaceCallEventHandler(ExternalInterfaceHandler);
        }

        public void DisposeHandler() {

            ShockwaveFlash.Dispose();
        }
        
        private object ExternalInterfaceHandler(object sender, ExternalInterfaceCallEventArgs e) {

            InvokeFromActionScript(e.FunctionCall.FunctionName, e.FunctionCall.Arguments);
            return false;
        }
        //ExternalIntarface.callでActionscriptから呼ばれる
        public void InvokeFromActionScript(string func, params string[] args) {

            switch(func) {
                case "CsFrame": //毎フレーム呼ばれる
                    CsFrame(args[0]);
                    break;
                case "NetConnection.Connect.Closed":    //RTMP動画再生時にタイムアウトになったら
                    break;
                case "NetStream.Play.Start":    //再生が開始された

                    Owner.IsPlaying = true;
                    break;
                case "ShowController":

                    break;
                case "HideController":

                    break;
                default:
                    Console.WriteLine("Invoked From Actionscript:" + func);
                    break;
            }
        }
        

        public void CsFrame(string vposs) {
            
            var vpos = long.Parse(vposs);
            
            Owner.LiveCommentInstance.Vpos = (int)vpos;
            Owner.Status = vposs + " " + GetTimeFromVpos((int)vpos) + " " + (int.Parse(Owner.Content.GetPlayerStatus.BaseTime) + vpos / 100);
            var unix = UnixTime.ToUnixTime(DateTime.Now);
            Owner.Time.CurrentTimeString = GetTimeFromVpos((int)(unix - long.Parse(Owner.Content.GetPlayerStatus.BaseTime)) * 100);
            Owner.Time.CurrentTime = vpos / 100;

            if(Owner.Content.Type == LivePageType.TimeShift) {

                foreach(var que in Owner.Content.GetPlayerStatus.QueSheet) {
                    
                    //終わってたら次へ
                    if(que.Done) {

                        continue;
                    }
                    
                    var quepos = int.Parse(que.Vpos);
                    
                    if(quepos <= vpos) {
                        
                        var list = que.Content.Split(new char[] { ' ' }, 2);
                        var command = list[0];
                        var args = list.Length == 2 ? list[1] : "";
                        
                        que.Done = true;
                        switch(command) {
                            case "/perm":
                                Owner.PermAria = args;
                                break;
                            case "/cls":
                            case "/clear":
                                Owner.PermAria = "ニコニコ生放送";
                                break;

                            default:
                                InvokeScript("AsCommandExcute", command, que.Vpos, args);
                                break;
                        }
                        Console.WriteLine("Command:" + que.Content);

                    }
                }
            }

            //Console.WriteLine("hogehoge frame: " + vposs);
        }


        private string GetTimeFromVpos(int vpos) {

            //秒に直す
            vpos /= 100;

            //マイナス部分
            var basetime = int.Parse(Owner.Content.GetPlayerStatus.StartTime) - int.Parse(Owner.Content.GetPlayerStatus.BaseTime);
            
            //マイナス部分を引く
            vpos -= basetime;

            var time = new TimeSpan(0, 0, vpos);
            return time.ToString();
        }
        
        public void Pause() {
            
            Owner.IsPlaying = false;
            InvokeScript("AsPause");
        }

        public void Resume() {

            Owner.IsPlaying = true;
            InvokeScript("AsResume");
        }


        public void Seek(int vpos) {

            InvokeScript("AsSeek", vpos.ToString());
        }


        //ASを呼ぶ
        public void InvokeScript(string func, params object[] args) {

            //読み込み前にボタンを押しても大丈夫なように メモリ解放されたあとに呼ばれないように
            if(ShockwaveFlash != null && !ShockwaveFlash.IsDisposed) {
                
                try {

                    Proxy.Call(func, args);
                } catch(COMException) {

                    Console.WriteLine("COMException：" + func);
                }
            }
        }   
        


    }
}
