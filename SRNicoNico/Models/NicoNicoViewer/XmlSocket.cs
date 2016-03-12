using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class XmlSocket : IDisposable {

        private Socket Socket;

        public event EventHandler<SocketAsyncEventArgs> OnConnect;
        public event EventHandler<XmlSocketReceivedEventArgs> XmlReceive;



        private string Host;
        private int Port;

        public XmlSocket(string host, int port) {

            Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Host = host;
            Port = port;
        }

        public void Connect() {

            var args = new SocketAsyncEventArgs();
            
            args.Completed += OnConnect;
            args.RemoteEndPoint = new DnsEndPoint(Host, Port);

            Socket.ConnectAsync(args);
        }



        public void Send(string xml) {

            var args = new SocketAsyncEventArgs();
            var data = Encoding.UTF8.GetBytes(xml + "\0");

            args.SetBuffer(data, 0, data.Length);

            Socket.SendAsync(args);

        }

        private byte[] Buffer = new byte[1024];

        public void RecursiveReceive(StringBuilder sb) {

            var args = new SocketAsyncEventArgs();

            args.UserToken = sb;
            args.SetBuffer(Buffer, 0, Buffer.Length);
            args.Completed += OnReceive;

            Socket.ReceiveAsync(args);
        }

        private void OnReceive(object sender, SocketAsyncEventArgs e) {

            var sb = e.UserToken as StringBuilder;

            for(int i = 0; i < e.BytesTransferred; i++) {

                char c = (char)e.Buffer[i];
                if(c != '\0') {

                    sb.Append(c);
                } else {

                    if(XmlReceive != null) {

                        XmlReceive(this, new XmlSocketReceivedEventArgs(sb.ToString()));
                    }
                    sb.Clear();
                }
            }
            RecursiveReceive(sb);
        }




        void IDisposable.Dispose() {
            Socket.Dispose();
        }
    }

    public class XmlSocketReceivedEventArgs : EventArgs {

        public string Xml { get; set; }

        public XmlSocketReceivedEventArgs(string xml) {

            Xml = xml;
        }

    }
}
