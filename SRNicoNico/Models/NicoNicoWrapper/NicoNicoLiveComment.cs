using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Livet;

using SRNicoNico.ViewModels;
using SRNicoNico.Models.NicoNicoViewer;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoLiveComment : NotificationObject, IDisposable {

        private const string GetWayBackKeyApiUrl = "http://live.nicovideo.jp/api/getwaybackkey";

        private XmlSocket XmlSocket;

        private NicoNicoGetPlayerStatus Status;

        private int LastRequestTime = 0;


        private int _Vpos;

        public int Vpos {
            get { return _Vpos; }
            set { 
                if(_Vpos == value)
                    return;
                _Vpos = value;


                if(Status.Archive) {

                    if(LastRequestTime + 30 <= int.Parse(Status.BaseTime) + value / 100) {

                        LastRequestTime = int.Parse(Status.BaseTime) + value / 100;

                        Task.Run(() => {

                            XmlSocket.Connect();
                            RequestComment();

                        });
                    }
                }


            }
        }


        private string Ticket;
        private int LastRes = -50;
        private int ResFrom = -50;

        private LiveWatchViewModel Owner;
        private LiveFlashHandler Handler;

        public NicoNicoLiveComment(string host, int port, LiveWatchViewModel vm) {

            Owner = vm;
            Status = vm.Content.GetPlayerStatus;
            Handler = vm.Handler;

            XmlSocket = new XmlSocket(host, port);
            XmlSocket.XmlReceive += OnReceive;

            if(Status.Archive) {

                ResFrom = -1000;
            } else {

                XmlSocket.Connect();
                RequestComment();
            }
        }

        private void RequestComment() {

            //タイムシフト
            if(Status.Archive) {

                try {

                    Console.WriteLine("Request:" + Vpos + " " + LastRequestTime);
                    var query = new GetRequestQuery(GetWayBackKeyApiUrl);
                    query.AddQuery("thread", Status.ThreadId);

                    var a = NicoNicoWrapperMain.Session.GetAsync(query.TargetUrl).Result;
                    var key = a.Split('=')[1];

                    var xml = BuildThread(ResFrom, LastRequestTime + 60, key);

                    Console.WriteLine(xml);

                    XmlSocket.Send(xml);

                    Task.Run(() => {

                        XmlSocket.RecursiveReceive(new MemoryStream());
                    });

                } catch(RequestTimeout) {


                }




            } else {

                var xml = BuildThread();

                XmlSocket.Send(xml);
                Task.Run(() => {

                    XmlSocket.RecursiveReceive(new MemoryStream());
                });
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

                thread.SetAttribute("when", when.ToString());
                thread.SetAttribute("waybackkey", waybackkey);
                thread.SetAttribute("user_id", Status.UserId);
            }



            xml.AppendChild(thread);

            return xml.InnerXml;
        }

        private int CommentNum = 0;

        private void OnReceive(object sender, XmlSocketReceivedEventArgs e) {


            var xml = new XmlDocument();

            try {

                xml.LoadXml(e.Xml);

                Console.WriteLine(e.Xml);

                var thread = xml.SelectSingleNode("/thread");
                if(thread != null) {

                    Ticket = thread.Attributes["ticket"].Value;
                    if(thread.Attributes["last_res"] != null) {

                        LastRes = int.Parse(thread.Attributes["last_res"].Value);
                    }
                    ResFrom = LastRes + 1;

                    return;
                }

                var chat = xml.SelectSingleNode("/chat");
                if(chat != null) {

                    var comment = new NicoNicoCommentEntry();
                    comment.No = (++CommentNum).ToString();
                    comment.Vpos = chat.Attributes["vpos"].Value;
                    comment.Mail = chat.Attributes["mail"] != null ? chat.Attributes["mail"].Value : "";
                    comment.Score = chat.Attributes["score"] != null ? int.Parse(chat.Attributes["score"].Value) : 0;
                    comment.UserId = chat.Attributes["user_id"].Value;
                    comment.Date = NicoNicoUtil.GetTimeFromVpos(Status, comment.Vpos);
                    comment.Content = chat.InnerText;

                    Owner.CommentList.Add(comment);

                    //コマンドは弾く
                    if(chat.InnerText.StartsWith("/")) {

                        return;
                    }
                    Handler.InvokeScript("AsInjectOneComment", comment.ToJson());

                }

            } catch(XmlException) {

                return;
            }
        }

        public void Dispose() {

            XmlSocket.Disconnect();
        }
    }
}
