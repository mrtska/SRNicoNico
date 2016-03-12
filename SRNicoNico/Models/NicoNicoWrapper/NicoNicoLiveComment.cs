using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Livet;

using SRNicoNico.Models.NicoNicoViewer;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoLiveComment : NotificationObject {

        private const string GetWayBackKeyApiUrl = "http://live.nicovideo.jp/api/getwaybackkey";

        private XmlSocket XmlSocket;

        private NicoNicoGetPlayerStatus Status;

        private int LastRequestTime = 0;

        public NicoNicoLiveComment(string host, int port, NicoNicoGetPlayerStatus status) {

            Status = status;

            XmlSocket = new XmlSocket(host, port);
            XmlSocket.XmlReceive += OnReceive;
            XmlSocket.OnConnect += OnConnect;
            XmlSocket.Connect();
        }


        private void OnConnect(object sender, SocketAsyncEventArgs e) {

            if(e.SocketError == SocketError.Success) {

                //タイムシフト
                if(Status.Archive) {

                    try {

                        var query = new GetRequestQuery(GetWayBackKeyApiUrl);
                        query.AddQuery("thread", Status.ThreadId);

                        var a = NicoNicoWrapperMain.Session.GetAsync(query.TargetUrl).Result;
                        var key = a.Split('=')[1];

                        var xml = BuildThread(-1000, LastRequestTime, key);

                        XmlSocket.Send(xml);

                        Task.Run(() => {

                            XmlSocket.RecursiveReceive(new StringBuilder());
                        });


                    } catch(RequestTimeout) {


                    }




                } else {

                    var xml = BuildThread();
                }

    
            }
        }

        private string BuildThread(int res = -50, int when = -1, string waybackkey = null) {


            var xml = new XmlDocument();
            var thread = xml.CreateElement("thread");
            thread.SetAttribute("thread", Status.ThreadId);
            thread.SetAttribute("res_from", res.ToString());
            thread.SetAttribute("version", "20061206");
            thread.SetAttribute("scores", "1");
            if(when != -1) {

                thread.SetAttribute("user_id", Status.UserId);
                thread.SetAttribute("when", when.ToString());
                thread.SetAttribute("waybackkey", waybackkey);
            }



            xml.AppendChild(thread);

            return xml.InnerXml;
        }


        private void OnReceive(object sender, XmlSocketReceivedEventArgs e) {

            ;
        }


        private void CallBack(object b) {


        }

    }
}
