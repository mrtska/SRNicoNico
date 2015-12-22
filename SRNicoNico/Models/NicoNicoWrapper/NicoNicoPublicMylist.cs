using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoPublicMylist : NotificationObject {

        private string MylistUrl;

        public NicoNicoPublicMylist(string url) {

            MylistUrl = url;
        }

        public void GetMylist() {


            try {

                var a = NicoNicoWrapperMain.Session.GetAsync(MylistUrl).Result;

                ;

            } catch(RequestTimeout) {

                return;
            }



        }


    }
}
