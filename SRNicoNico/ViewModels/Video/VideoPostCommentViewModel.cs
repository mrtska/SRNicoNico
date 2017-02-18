using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class VideoPostCommentViewModel : ViewModel {


        #region Position変更通知プロパティ
        private string _Position = "";

        public string Position {
            get { return _Position; }
            set { 
                if(_Position == value)
                    return;
                _Position = value;
                UpdateMail();
                RaisePropertyChanged();
            }
        }
        #endregion
        #region Size変更通知プロパティ
        private string _Size = "";

        public string Size {
            get { return _Size; }
            set { 
                if(_Size == value)
                    return;
                _Size = value;
                UpdateMail();
                RaisePropertyChanged();
            }
        }
        #endregion
        #region Color変更通知プロパティ
        private string _Color = "";

        public string Color {
            get { return _Color; }
            set { 
                if(_Color == value)
                    return;
                _Color = value;
                UpdateMail();
                RaisePropertyChanged();
            }
        }
        #endregion



        #region IsPremium変更通知プロパティ
        private bool _IsPremium = false;

        public bool IsPremium {
            get { return _IsPremium; }
            set { 
                if(_IsPremium == value)
                    return;
                _IsPremium = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Text変更通知プロパティ
        private string _Text = "";

        public string Text {
            get { return _Text; }
            set { 
                if(_Text == value)
                    return;
                _Text = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Mail変更通知プロパティ
        private string _Mail = "";

        public string Mail {
            get { return _Mail; }
            set {
                if(_Mail == value)
                    return;
                _Mail = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsCommentPopupOpen変更通知プロパティ
        private bool _IsCommentPopupOpen;

        public bool IsCommentPopupOpen {
            get { return _IsCommentPopupOpen; }
            set { 
                if(_IsCommentPopupOpen == value)
                    return;
                _IsCommentPopupOpen = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Use184変更通知プロパティ

        public bool Use184 {
            get { return Settings.Instance.Use184; }
            set { 
                if(Settings.Instance.Use184 == value)
                    return;
                Settings.Instance.Use184 = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        public int CurrentVpos {
            get {
                return Owner.CurrentVpos;
            }
        }

        private bool PostPending = false;

        private VideoCommentViewModel Owner;

        public VideoPostCommentViewModel(VideoCommentViewModel vm) {
            
            Owner = vm;
            Initialize();
        }

        private void UpdateMail() {

            Mail = Size.ToString().ToLower() + " " + Position.ToString().ToLower();
            if(Color != "white") {

                Mail += " " + Color;
            }

            //省略できるやつはする
            Mail = Mail.Replace("medium", "").Replace("naka", "");
        }
        

        public void Initialize() {

            IsPremium = Owner.Owner.ApiData.Viewer.IsPremium;
        }

        //コメント投下
        public async void Post() {

            //連打して2回ポストされないように
            //割りと重要かなって
            if(PostPending || Text.TrimEnd().Length == 0) {

                return;
            }
            PostPending = true;
            var vpos = CurrentVpos;


            VideoCommentListViewModel target = null;
            if (Owner.Owner.ApiData.IsChannelVideo) {

                target = Owner.CommentListList.Where(e => e.CommentListInstance.ListName == "チャンネルコメント").First();
            } else {

                target = Owner.CommentListList.Where(e => e.CommentListInstance.ListName == "通常コメント").First();
            }
            //コメントを投稿してコメントナンバーを取得する
            var number = await Owner.CommentInstance.PostAsync(target.CommentListInstance, vpos, Mail, Text);

            //APIを叩いて返ってきたコメント番号をもとにhtml5で描画してみる
            //APIの応答が遅いと表示される前に描画タイミングが過ぎてしまうかもね
            if(number != -1) {

                var entry = new NicoNicoCommentEntry() {
                    Number = number,
                    Vpos = vpos,
                    PostedAt = UnixTime.ToUnixTime(DateTime.Now),
                    Anonymity = Settings.Instance.Use184,
                    UserId = App.ViewModelRoot.CurrentUser.UserId,
                    Mail = Mail,
                    Content = Text,
                    JustPosted = true
                };
                entry.DisassembleMail();
                target.CommentList.Add(new VideoCommentEntryViewModel(Owner, entry));

                //TextBoxを空にする
                Text = "";
            }

            //処理終わり
            PostPending = false;
        }

    }
}
