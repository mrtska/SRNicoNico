using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using SRNicoNico.ViewModels;

namespace SRNicoNico.Models.NicoNicoViewer {



    //このクラスでJavaScriptからの呼び出しを制御する
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]
    public class ObjectForScriptingHelper {

        
        private readonly VideoViewModel Video;

        public ObjectForScriptingHelper(VideoViewModel video) {
           
            Video = video; 
        } 

        //fscommandでActionscriptから呼ばれる
        public void InvokeFromJavaScript(string func, string args) {

            string[] arg = args.Split(':');

            switch(func) {
                case "CsFrame":
                    
                    Video.CsFrame(float.Parse(arg[0]), float.Parse(arg[1]), long.Parse(arg[2]));
                    break;
                case "NetConnection.Connect.Closed":

                    Video.RTMPTimeOut();
                    break;
                case "ShowContoller":
                    Video.ShowFullScreenPopup();
                    break;
                case "HideContoller":
                    Video.HideFullScreenPopup();
                    break;
                default:
                    Console.Write("Invoked From Actionscript:" + func);
                    Console.WriteLine(" Args:" + args);
                    break;
            }
        }
    }
}
