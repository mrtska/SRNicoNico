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
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class LiveFlashHandler : ViewModel {



        public void LoadMovie() {
            
        }

        public void DisposeHandler() {

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
            
            var vpos = int.Parse(vposs);
            
        }   
        


    }
}
