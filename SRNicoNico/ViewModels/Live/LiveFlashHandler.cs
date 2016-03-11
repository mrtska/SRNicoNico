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
                case "ShowController":

                    break;
                case "HideController":

                    break;
                default:
                    Console.WriteLine("Invoked From Actionscript:" + func);
                    break;
            }
        }

        public void CsFrame(string vpos) {

            Console.WriteLine("hogehoge frame: " + vpos);
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
