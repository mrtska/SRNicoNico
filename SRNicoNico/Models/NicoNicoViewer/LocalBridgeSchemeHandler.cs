using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace SRNicoNico.Models.NicoNicoViewer {
    class LocalBridgeSchemeHandler : ISchemeHandlerFactory, IResourceHandler {
        void IResourceHandler.Cancel() {
        }

        bool IResourceHandler.CanGetCookie(Cookie cookie) {
            
            return false;
        }
    

          bool IResourceHandler.CanSetCookie(Cookie cookie) {

            return false;
        }

        IResourceHandler ISchemeHandlerFactory.Create(IBrowser browser, IFrame frame, string schemeName, IRequest request) {


            if(request.Url.StartsWith("http://localbridge") || schemeName == "https") {

                return this;
            }

            if(request.Url.StartsWith("http://res.nimg.jp") && !request.Url.StartsWith("http://res.nimg.jp/swf/player/secure_nccreator.swf")) {
                return this;
            }
            
            return null;
        }

        void IDisposable.Dispose() {

        }

        public FileStream Stream { get; set; }
        public string MimeType { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }

        void IResourceHandler.GetResponseHeaders(IResponse response, out long responseLength, out string redirectUrl) {
            responseLength = Size;
            redirectUrl = null;
            var header = new System.Collections.Specialized.NameValueCollection();

            header["Content-Length"] = Size.ToString();

            response.ResponseHeaders = header;

            response.StatusCode = 200;
            response.StatusText = "OK";
            
        }

        bool IResourceHandler.ProcessRequest(IRequest request, ICallback callback) {

            var uri = new Uri(request.Url);
            var name = uri.AbsolutePath;
            MimeType = ResourceHandler.GetMimeType(System.IO.Path.GetExtension(name));

            if(request.Url == "http://res.nimg.jp/swf/player/secure_nccreator.swf?t=201111091500") {

                Path = "/secure_nccreator.swf";
            } else {

                Path = name;
            }



            var file = Environment.CurrentDirectory + "/Flash" + Path;
            Stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            Size = Stream.Length;

            callback.Continue();
            return true;
        }

        bool IResourceHandler.ReadResponse(Stream dataOut, out int bytesRead, ICallback callback) {

            callback.Dispose();

            var file = Environment.CurrentDirectory + "/Flash" + Path;

            
            var buffer = new byte[dataOut.Length];
            bytesRead = Stream.Read(buffer, 0, buffer.Length);


            dataOut.Write(buffer, 0, buffer.Length);
        

            return bytesRead > 0;

        }
    }
}
