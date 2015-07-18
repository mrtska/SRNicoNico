using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Reflection;
using System.IO;

using Newtonsoft.Json;


namespace SRNicoNico.Models.NicoNicoWrapper {
	public class NicoNicoUtil {



		//XMLをJsonに変換
		public static string xmlToJson(string xml) {

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			return JsonConvert.SerializeXmlNode(doc);
		}



		private static FileInfo _currentDirectory;

		public static FileInfo currentDirectory {
			get {

				if(_currentDirectory == null) {

					var currentAssembly = Assembly.GetEntryAssembly();
					_currentDirectory = new FileInfo(currentAssembly.Location);
				}

				return _currentDirectory;
			}
		}




	}
}
