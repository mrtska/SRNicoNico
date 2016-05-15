using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Http;
using Livet;

namespace SRNicoNico.Models.NicoNicoWrapper {

    //ニコニコ本家のNG機能バックエンド
    public class NicoNicoNGComment : NotificationObject {

        private const string NGApi = "http://flapi.nicovideo.jp/api/configurengclient";



        public static void GetNGClient() {

            var pair = new Dictionary<string, string>();
            pair["mode"] = "get";

            try {


                var request = new HttpRequestMessage(HttpMethod.Post, NGApi);

                request.Content = new FormUrlEncodedContent(pair);

                var a = NicoNicoWrapperMain.Session.GetAsync(request).Result;



            } catch(RequestTimeout) {


            }



        }






    }



    public enum NGType {

        Id,
        Word
    }


}
