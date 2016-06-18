using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Collections.Specialized;

namespace SRNicoNico.ViewModels {
    public class DownloadHandler : IDownloadHandler {

        void IDownloadHandler.OnBeforeDownload(IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback) {

            ;
        }

        void IDownloadHandler.OnDownloadUpdated(IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback) {

            ;
        }
    }
}
