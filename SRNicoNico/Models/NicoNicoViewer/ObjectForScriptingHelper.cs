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

        public void InvokeFromJavaScript(string func, string args) {

            string[] arg = args.Split(':');

            switch(func) {
                case "CsFrame":
                    
                    Video.CsFrame(int.Parse(arg[0]), float.Parse(arg[1]));
                    break;

                default:
                    Console.WriteLine("Invoked From Javascript:" + func);
                    Console.WriteLine("Args:" + args);
                    break;
            }

        }





    }
}
