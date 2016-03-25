using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Reflection;
using System.IO;

using Newtonsoft.Json;


namespace SRNicoNico.Models.NicoNicoWrapper {
	public class NicoNicoUtil {



		//XMLをJsonに変換
		public static string XmlToJson(string xml) {

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			return JsonConvert.SerializeXmlNode(doc);
		}

        //カレントディレクトリ取得
		private static FileInfo _CurrentDirectory;
		public static FileInfo CurrentDirectory {
			get {
				if(_CurrentDirectory == null) {

					var currentAssembly = Assembly.GetEntryAssembly();
					_CurrentDirectory = new FileInfo(currentAssembly.Location);
				}
				return _CurrentDirectory;
			}
		}

        //m:sをsに変換
        public static long ConvertTime(string s) {

            string[] strings = s.Split(':');
            string minutes = strings[0];
            string seconds = strings[1];

            return (long.Parse(minutes) * 60) + long.Parse(seconds);
        }

		//sをm:sに変換
		public static string ConvertTime(long time) {

			long munites = time / 60;
			long seconds = time % 60;

			if(seconds < 10) {

				return munites + ":0" + seconds;
			}

			return munites + ":" + seconds;
		}


        public static string GetTimeFromVpos(NicoNicoGetPlayerStatus status, string vpos) {

            return GetTimeFromVpos(status, int.Parse(vpos));
        }


        public static string GetTimeFromVpos(NicoNicoGetPlayerStatus status, int vpos) {

            //秒に直す
            vpos /= 100;

            //マイナス部分
            var basetime = int.Parse(status.StartTime) - int.Parse(status.BaseTime);

            //マイナス部分を引く
            vpos -= basetime;

            var time = new TimeSpan(0, 0, vpos).ToString();

            if(time.StartsWith("00:")) {

                time = time.Substring(3);
            }

            return time;
        }

        public static string DateFromVitaFormatDate(string date) {
            return Regex.Replace(date, @"(\d\d\d\d-\d\d-\d\d).(\d\d:\d\d:\d\d).\d\d:\d\d", "$1 $2");
        }


	}
}
