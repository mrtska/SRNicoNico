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


            if(request.Url.StartsWith("http://localbridge")) {

                return this;
            }
            return null;
        }

        void IDisposable.Dispose() {

        }

        public string MimeType { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }

        void IResourceHandler.GetResponseHeaders(IResponse response, out long responseLength, out string redirectUrl) {
            responseLength = Size;
            redirectUrl = null;

            response.StatusCode = 200;
            response.StatusText = "OK";
            response.MimeType = MimeType;
        }

        bool IResourceHandler.ProcessRequest(IRequest request, ICallback callback) {

            var uri = new Uri(request.Url);
            var name = uri.AbsolutePath;
            MimeType = ResourceHandler.GetMimeType(System.IO.Path.GetExtension(name));
            Path = name;


            var file = Environment.CurrentDirectory + "/Flash" + Path;
            var info = new FileInfo(file);

            Size = info.Length;
            callback.Continue();
            return true;
        }

        bool IResourceHandler.ReadResponse(Stream dataOut, out int bytesRead, ICallback callback) {

            callback.Dispose();

            var file = Environment.CurrentDirectory + "/Flash" + Path;

            var stream = new FileStream(file, FileMode.Open);
            var buffer = new byte[dataOut.Length];
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            dataOut.Write(buffer, 0, buffer.Length);

            return true;

        }
    }
}
