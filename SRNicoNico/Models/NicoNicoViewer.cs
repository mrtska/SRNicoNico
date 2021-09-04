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
        public NicoNicoUrlType DetectUrlType(string url) {

            if (url.StartsWith("https://www.nicovideo.jp/watch/")) {

                return NicoNicoUrlType.Video;
            } else if (url.StartsWith("https://www.nicovideo.jp/user/")) {

                return NicoNicoUrlType.User;
            } else if (url.StartsWith("https://www.nicovideo.jp/mylist/")) {

                return NicoNicoUrlType.Mylist;
            } else if (url.StartsWith("https://www.nicovideo.jp/series/")) {

                return NicoNicoUrlType.Series;
            }

            return NicoNicoUrlType.Other;
        }

        /// <inheritdoc />
        public async void OpenUrl(string url) {

            var mainContent = UnityContainer.Resolve<MainContentViewModel>();

            await Task.Run(() => {

                switch (DetectUrlType(url)) {
                    case NicoNicoUrlType.Video:
                        mainContent.AddVideoTab(UnityContainer.Resolve<VideoViewModel>(new ParameterOverride("videoId", url[31..].Split('?')[0])));
                        break;
                    case NicoNicoUrlType.User:
                        mainContent.AddTab(UnityContainer.Resolve<UserViewModel>(new ParameterOverride("userId", url[30..].Split('?')[0])));
                        break;
                    case NicoNicoUrlType.Mylist:
                        mainContent.AddTab(UnityContainer.Resolve<PublicMylistViewModel>(new ParameterOverride("mylistId", url[32..].Split('?')[0])));
                        break;
                    case NicoNicoUrlType.Series:
                        mainContent.AddTab(UnityContainer.Resolve<SeriesViewModel>(new ParameterOverride("seriesId", url[32..].Split('?')[0])));
                        break;
                    default:
                        App.UIDispatcher!.Invoke(() => {
                            mainContent.WebView!.AddTab(url);
                            // WebViewをActiveにする
                            mainContent.SelectedItem = mainContent.WebView;
                        });
                        break;
                }
            });
        }

        /// <inheritdoc />
        public bool CanOpenUrl(string url) => DetectUrlType(url) != NicoNicoUrlType.Other;

    }
}
