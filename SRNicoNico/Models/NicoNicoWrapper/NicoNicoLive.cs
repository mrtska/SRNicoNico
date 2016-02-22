using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using Codeplex.Data;

using Livet;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoLive : NotificationObject {

        private const string LiveUrl = "http://www.nicovideo.jp/my/live";

        private const string NotifyUrl = "http://live.nicovideo.jp/notifybox";


        public NicoNicoLive() {

        }




        public void GetLiveInformation() {

            try {

                var a = NicoNicoWrapperMain.Session.GetAsync(NotifyUrl).Result;


                var doc = new HtmlDocument();


            } catch(RequestTimeout) {


            }




        }




    }
}
