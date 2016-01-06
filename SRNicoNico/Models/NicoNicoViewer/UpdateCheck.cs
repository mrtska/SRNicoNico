using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using Codeplex.Data;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class UpdateCheck : NotificationObject {

        private static string CheckUrl = "https://mrtska.net/niconicowrapper/update";
        
        public static bool IsUpdateAvailable(double cur, ref string url) {


            try {

                var a = NicoNicoWrapperMain.Session.GetAsync(CheckUrl).Result;

                var json = DynamicJson.Parse(a);

                if(cur < json.version) {

                    url = json.url;
                    return true;
                }


                return false;
            } catch(RequestTimeout) {

                return false;
            }
        }
    }
}
