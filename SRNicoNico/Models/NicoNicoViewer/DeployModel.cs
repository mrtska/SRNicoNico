using Livet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoViewer {

#if DEBUG
    public class DeployModel : NotificationObject {

        private const string DeployUrl = "url";

        private const string SecretKey = "long long key";


        #region FilePath変更通知プロパティ
        private string _FilePath;

        public string FilePath {
            get { return _FilePath; }
            set { 
                if (_FilePath == value)
                    return;
                _FilePath = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public async Task<string> UploadAsync() {

            if(string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath)) {

                return "ファイルパスが指定されていないか、ファイルが存在しません";
            }

            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, DeployUrl);
            request.Headers.Add("token", SecretKey);
            request.Headers.Add("version", App.ViewModelRoot.CurrentVersion.ToString());

            request.Content = new StreamContent(File.Open(FilePath, FileMode.Open));

            var res = await client.SendAsync(request);
            var str = await res.Content.ReadAsStringAsync();

            return str;
        }



    }
#endif
}
