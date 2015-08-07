using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models;

using SRNicoNico.Models.NicoNicoWrapper;


namespace SRNicoNico.ViewModels {

	public class SearchResultEntryViewModel : ViewModel {
	
		public NicoNicoSearchResultNode Node { get; set; }

		public void OpenVideo() {

			//UIスレッドは使わない
			Task.Run(new Action(() => {

				App.ViewModelRoot.Video.Cmsid = Node.cmsid;
				//ViewをVideoに変える
				App.ViewModelRoot.Content = App.ViewModelRoot.Video;


                NicoNicoGetFlv.AccessVideoPage(Node.cmsid);


                //GetFlvAPIを叩いたてサーバーを取得
                NicoNicoGetFlvData data = NicoNicoGetFlv.GetFlv(Node.cmsid);

				//cacheディレクトリを無ければ作成
				Directory.CreateDirectory(NicoNicoUtil.CurrentDirectory.DirectoryName + @"\cache");

				//キャッシュパス
				string path = NicoNicoUtil.CurrentDirectory.DirectoryName + @"\cache\" + Node.cmsid;
				FileInfo cache = new FileInfo(path);

				//キャッシュが存在したら
				if(cache.Exists) {

					long length = cache.Length;

					App.ViewModelRoot.Video.CacheStream = new FileStream(path, FileMode.Open, FileAccess.Write);
					App.ViewModelRoot.Video.VideoStream = NicoNicoGetFlv.GetFlvStreamRange(Node.cmsid, data.VideoUrl, length);
					App.ViewModelRoot.Video.CacheExists = true;
					
					

				} else {

					App.ViewModelRoot.Video.CacheStream = new FileStream(path, FileMode.Create, FileAccess.Write);
					App.ViewModelRoot.Video.VideoStream = NicoNicoGetFlv.GetFlvStream(Node.cmsid, data.VideoUrl);
					App.ViewModelRoot.Video.CacheExists = false;
				}

				App.ViewModelRoot.Video.Path = path;
				App.ViewModelRoot.Video.StartStreaming();



			}));


			

		}

	}
}
