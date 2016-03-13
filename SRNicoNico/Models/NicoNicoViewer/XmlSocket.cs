using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class XmlSocket : IDisposable {

        private Socket Socket;

        public event EventHandler<SocketAsyncEventArgs> OnConnect;
        public event EventHandler<XmlSocketReceivedEventArgs> XmlReceive;

        public bool IsConnected {
            get {

                return Socket.Connected;
            }
        }

        private string Host;
        private int Port;

        public XmlSocket(string host, int port) {

            Host = host;
            Port = port;
        }

        public void Connect() {

            if(Socket != null) {

                Socket.Close();
            }
            Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            
            Socket.Connect(Host, Port);
        }



        public void Send(string xml) {

            var data = Encoding.UTF8.GetBytes(xml + '\0');

            Socket.Send(data);
        }

        private byte[] Buffer = new byte[1024];

        public void RecursiveReceive(MemoryStream stream) {

            if(!Socket.Connected) {

                return;
            }

            var args = new SocketAsyncEventArgs();

            args.UserToken = stream;
            args.SetBuffer(Buffer, 0, Buffer.Length);
            args.Completed += OnReceive;

            if(!Socket.ReceiveAsync(args)) {

                ;
            }
        }

        private void OnReceive(object sender, SocketAsyncEventArgs e) {

            var stream = e.UserToken as MemoryStream;

            for(int i = 0; i < e.BytesTransferred; i++) {

                
                var c = e.Buffer[i];
                if(c != 0) {

                    stream.WriteByte(c);
                } else {

                    if(XmlReceive != null) {
                        var text = new string(Encoding.UTF8.GetChars(stream.ToArray()));
                        if(text.StartsWith("<")) {

                            XmlReceive(this, new XmlSocketReceivedEventArgs(text));
                        }
                    }
                    stream.SetLength(0);
                }
                e.Buffer[i] = 0;
            }
            e.Dispose();
            RecursiveReceive(stream);
        }

        public void Disconnect() {

            if(Socket.Connected) {

                Socket.Disconnect(true);
            }
        }


        void IDisposable.Dispose() {

            Socket?.Dispose();
        }
    }

    public class XmlSocketReceivedEventArgs : EventArgs {

        public string Xml { get; set; }

        public XmlSocketReceivedEventArgs(string xml) {

            Xml = xml;
        }

    }
}
