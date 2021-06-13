using System.Threading.Tasks;
using SRNicoNico.ViewModels;
using Unity;
using Unity.Resolution;

namespace SRNicoNico.Models {
    public class NicoNicoViewer : INicoNicoViewer {
        /// <summary>
        /// 現在のバージョン
        /// </summary>
        public double CurrentVersion {

            get { return 2.00; }
        }

        private readonly IUnityContainer UnityContainer;

        public NicoNicoViewer(IUnityContainer unityContainer) {

            UnityContainer = unityContainer;
        }

        /// <inheritdoc />
        public async void OpenUrl(string url) {

            var mainContent = UnityContainer.Resolve<MainContentViewModel>();

            await Task.Run(() => {

                if (url.StartsWith("https://www.nicovideo.jp/watch/")) {

                    mainContent.AddVideoTab(UnityContainer.Resolve<VideoViewModel>(new ParameterOverride("videoId", url[31..].Split('?')[0])));
                } else if (url.StartsWith("https://www.nicovideo.jp/user/")) {

                    mainContent.AddTab(UnityContainer.Resolve<UserViewModel>(new ParameterOverride("userId", url[30..].Split('?')[0])));
                } else if (url.StartsWith("https://www.nicovideo.jp/mylist/")) {

                    mainContent.AddTab(UnityContainer.Resolve<PublicMylistViewModel>(new ParameterOverride("mylistId", url[32..].Split('?')[0])));
                } else if (url.StartsWith("https://www.nicovideo.jp/series/")) {

                    mainContent.AddTab(UnityContainer.Resolve<SeriesViewModel>(new ParameterOverride("seriesId", url[32..].Split('?')[0])));
                }
            });
        }
    }
}
