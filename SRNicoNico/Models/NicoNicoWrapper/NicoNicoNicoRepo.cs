using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Livet;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoNicoRepo : NotificationObject {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */


        private const string NicoRepoUrl = @"http://www.nicovideo.jp/my/top";

        //二コレポを取得するAPI
        private const string NicoRepoApiUrl = @"http://api.gadget.nicovideo.jp/user/nicorepo";


        public static void Debug() {

            string html = NicoNicoWrapperMain.getSession().HttpClient.GetStringAsync(NicoRepoUrl).Result;

            string token = "dummy";

            foreach(string line in html.Split('\n')) {

                if(line.Contains("Mypage_globals.hash = ")) {

                    //トークンを抜き出す
                    token = line.Split('=')[1];
                    token = token.Replace("'", "");
                    token = token.Replace(";", "");
                    token = token.Substring(1);
                    break;
                }
            }
            
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            
            request.RequestUri = new Uri(NicoRepoApiUrl);
            request.Headers.Add("Cookie2", "$Version=1");
            
            

            var a = NicoNicoWrapperMain.getSession().HttpClient.SendAsync(request).Result;
            Console.WriteLine(a.Content.ReadAsStringAsync().Result);
            ;

        }


    }
}
