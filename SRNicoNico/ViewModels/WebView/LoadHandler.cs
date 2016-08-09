using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CefSharp;

namespace SRNicoNico.ViewModels {


    
    public class LoadHandler : ILoadHandler {


        private WebViewContentViewModel Owner;

        public LoadHandler(WebViewContentViewModel content) {

            Owner = content;
        }

        void ILoadHandler.OnFrameLoadEnd(IWebBrowser browserControl, FrameLoadEndEventArgs frameLoadEndArgs) {

            ;
        }

        void ILoadHandler.OnFrameLoadStart(IWebBrowser browserControl, FrameLoadStartEventArgs frameLoadStartArgs) {

            if(frameLoadStartArgs.Frame.IsMain) {

                Owner.Url = frameLoadStartArgs.Url;
            }

            ;
        }

        void ILoadHandler.OnLoadError(IWebBrowser browserControl, LoadErrorEventArgs loadErrorArgs) {

            ;
        }

        void ILoadHandler.OnLoadingStateChange(IWebBrowser browserControl, LoadingStateChangedEventArgs loadingStateChangedArgs) {

            ;
        }
    }
}
