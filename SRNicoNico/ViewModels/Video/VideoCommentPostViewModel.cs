using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using System;
using System.Linq;

namespace SRNicoNico.ViewModels {
    public class VideoCommentPostViewModel : ViewModel {


        #region Position変更通知プロパティ
        private string _Position = "";

        public string Position {
            get { return _Position; }
            set {
                if (_Position == value)
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
                if (_Size == value)
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
                if (_Color == value)
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
                if (_IsPremium == value)
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
                if (_Text == value)
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
                if (_Mail == value)
                    return;
                _Mail = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Use184変更通知プロパティ
        private bool _Use184;

        public bool Use184 {
            get { return _Use184; }
            set { 
                if (_Use184 == value)
                    return;
                _Use184 = value;
                Settings.Instance.Use184 = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region PostPending変更通知プロパティ
        private bool _PostPending = false;

        public bool PostPending {
            get { return _PostPending; }
            set { 
                if (_PostPending == value)
                    return;
                _PostPending = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region IsFocused変更通知プロパティ
        private bool _IsFocused = false;

        public bool IsFocused {
            get { return _IsFocused; }
            set { 
                if (_IsFocused == value)
                    return;
                _IsFocused = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        private readonly VideoCommentViewModel Owner;

        private readonly NicoNicoWatchApi ApiData;

        public VideoCommentPostViewModel(VideoCommentViewModel vm, NicoNicoWatchApi apiData) {

            Owner = vm;
            Use184 = true;

            ApiData = apiData;

            Initialize();
        }

        private void UpdateMail() {

            Mail = Size.ToString().ToLower() + " " + Position.ToString().ToLower();
            if (Color != "white") {

                Mail += " " + Color;
            }

            //省略できるやつはする
            Mail = Mail.Replace("medium", "").Replace("naka", "");
        }


        public void Initialize() {

            IsPremium = ApiData.ViewerInfo.IsPremium;
        }

        // コメント投下
        public async void Post() {

            // 連打して2回ポストされないように
            if (PostPending || Text.TrimEnd().Length == 0) {

                return;
            }
            PostPending = true;
            var vpos = Owner.CurrentVpos;

            var list = Owner.CommentList.Single(e => e.ListInstance.Composite.IsDefaultPostTarget);

            //コメントを投稿してコメントナンバーを取得する
            var number = await Owner.Model.PostAsync(list.ListInstance, vpos, Mail, Text);

            //APIを叩いて返ってきたコメント番号をもとにhtml5で描画してみる
            //APIの応答が遅いと表示される前に描画タイミングが過ぎてしまうかもね
            if (number != -1) {

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
                list.CommentList.Add(new VideoCommentEntryViewModel(entry, Owner.Handler));

                //TextBoxを空にする
                Text = "";
            }

            //処理終わり
            PostPending = false;
        }
    }
}
