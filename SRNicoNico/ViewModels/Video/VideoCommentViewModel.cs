using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;

using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {

	public class VideoCommentViewModel : ViewModel {


        #region IsPopupOpen変更通知プロパティ
        private bool _IsPopupOpen;

        public bool IsPopupOpen {
            get { return _IsPopupOpen; }
            set { 
                if(_IsPopupOpen == value)
                    return;
                _IsPopupOpen = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region AcceptEnter変更通知プロパティ
        private bool _AcceptEnter;

        public bool AcceptEnter {
            get { return _AcceptEnter; }
            set { 
                if(_AcceptEnter == value)
                    return;
                _AcceptEnter = value;
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


        #region Size変更通知プロパティ
        private CommentSize _Size = CommentSize.Medium;

        public CommentSize Size {
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


        #region Position変更通知プロパティ
        private CommentPosition _Position = CommentPosition.Naka;

        public CommentPosition Position {
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

        private VideoViewModel Owner;

        public VideoCommentViewModel(VideoViewModel vm) {

            Owner = vm;
		}

        public void UpdateMail() {

            Mail = Size.ToString().ToLower() + " " + Position.ToString().ToLower();
            if(Color != "white") {

                Mail += " " + Color;
            }

            //省略できるやつはする
            Mail = Mail.Replace("medium", "").Replace("naka", "");
        }

        public void Post() {

            Owner.CommentInstance.Post(Text, mail, Owner.vpos);

        }


	}
}
