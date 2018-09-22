using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using System;
using System.Linq;

namespace SRNicoNico.ViewModels {
    public class VideoCommentViewModel : ViewModel {

        #region IsActive変更通知プロパティ
        private bool _IsActive;

        public bool IsActive {
            get { return _IsActive; }
            set { 
                if (_IsActive == value)
                    return;
                _IsActive = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CommentVisibility変更通知プロパティ
        private bool _CommentVisibility = true;

        public bool CommentVisibility {
            get { return _CommentVisibility; }
            set { 
                if (_CommentVisibility == value)
                    return;
                _CommentVisibility = value;
                Settings.Instance.CommentVisibility = value;
                if (value) {

                    Handler?.WebBrowser?.InvokeScript("Comment$Show");
                } else {

                    Handler?.WebBrowser?.InvokeScript("Comment$Hide");
                }
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CommentList変更通知プロパティ
        private ObservableSynchronizedCollection<VideoCommentListViewModel> _CommentList;

        public ObservableSynchronizedCollection<VideoCommentListViewModel> CommentList {
            get { return _CommentList; }
            set { 
                if (_CommentList == value)
                    return;
                _CommentList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedList変更通知プロパティ
        private VideoCommentListViewModel _SelectedList;

        public VideoCommentListViewModel SelectedList {
            get { return _SelectedList; }
            set { 
                if (_SelectedList == value)
                    return;
                _SelectedList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Post変更通知プロパティ
        private VideoCommentPostViewModel _Post;

        public VideoCommentPostViewModel Post {
            get { return _Post; }
            set { 
                if (_Post == value)
                    return;
                _Post = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region AutoScrollEnabled変更通知プロパティ
        private bool _AutoScrollEnabled = true;

        public bool AutoScrollEnabled {
            get { return _AutoScrollEnabled; }
            set { 
                if (_AutoScrollEnabled == value)
                    return;
                _AutoScrollEnabled = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CurrentVpos変更通知プロパティ
        private int _CurrentVpos;

        public int CurrentVpos {
            get { return _CurrentVpos; }
            set { 
                if (_CurrentVpos == value)
                    return;
                _CurrentVpos = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        internal readonly NicoNicoComment Model;

        internal readonly VideoHtml5Handler Handler;

        public VideoCommentViewModel(NicoNicoWatchApi apiData, VideoHtml5Handler handler) {

            IsActive = true;
            CommentVisibility = Settings.Instance.CommentVisibility;
            Model = new NicoNicoComment(apiData);
            Post = new VideoCommentPostViewModel(this, apiData);
            Handler = handler;
            Initialize();
        }

        public async void Initialize() {

            CommentList = new ObservableSynchronizedCollection<VideoCommentListViewModel>();
            var a = await Model.GetAllCommentAsync();
            foreach(var list in a) {

                list.Sort();
                CommentList.Add(new VideoCommentListViewModel(list, Handler));
            }

            SelectedList = CommentList.SingleOrDefault(e => e.ListInstance.Composite.IsDefaultPostTarget);

            Handler.WebBrowser?.InvokeScript("Comment$Initialize");
            ApplyCommentConfiguration();
            if (CommentVisibility) {

                Handler.WebBrowser?.InvokeScript("Comment$Show");
            } else {

                Handler.WebBrowser?.InvokeScript("Comment$Hide");
            }
            Handler.AttachComment(this);
            IsActive = false;
        }

        public void ApplyCommentConfiguration() {

            ;
            Handler.WebBrowser?.InvokeScript("Comment$SetBaseSize", Settings.Instance.CommentSize);
            Handler.WebBrowser?.InvokeScript("Comment$SetOpacity", Settings.Instance.CommentAlpha);

        }

        public void ToggleAutoScroll() {

            AutoScrollEnabled ^= true;
        }
        public async void Refresh() {

            var ret = await Model.RefreshCommentAsync(SelectedList.ListInstance);
            SelectedList.CommentList.Clear();
            ret.Sort();
            foreach (var entry in ret.CommentList) {

                //コメントをふるいにかける
                App.ViewModelRoot.Setting.NGFilter.Filter(entry);

                if (entry.Rejected) {

                    continue;
                }
                SelectedList.CommentList.Add(new VideoCommentEntryViewModel(entry, Handler));
            }
        }

        public void CommentTick(int vpos) {

            CurrentVpos = vpos;

            foreach (var list in CommentList) {

                if(list.ListInstance.CommentType == CommentType.NicoScript) {

                    continue;
                }

                foreach (var entry in list.CommentList) {

                    var item = entry.Item;

                    //描画が終わる時間よりも現在時間のほうが大きかったらまだまだ
                    if (item.Vend < vpos) {

                        item.IsRendering = false;
                        continue;
                    }

                    //コメントリストは描画順でソートされているのでvposより大きいやつが来たらそれ以降は無視
                    if (item.Vpos > vpos) {

                        item.IsRendering = false;
                        continue;
                    }

                    //投稿者コメントでかつニコスクリプトだったら
                    if (item.IsUploader/* && item.NicoScript != null && item.NicoScript.AffectOtherComments*/) {

                        continue;
                    }

                    if (!item.IsRendering && vpos >= item.Vpos && vpos <= item.Vend) {

                        item.IsRendering = true;


                        //ニコスクリプトが存在したら対象の
                        //if (NicoScript != null) {

                        //    foreach (var script in NicoScript) {

                        //        if (script.AffectOtherComments) {

                        //            script.ExecuteIfValidTime(item);
                        //        }
                        //    }
                        //}

                        //if (item.NicoScript != null && !item.NicoScript.AffectOtherComments) {

                        //    item.NicoScript.Execute(null);
                        //    continue;
                        //}
                        item.DefaultCommentSize = Settings.Instance.CommentSize;
                        item.Opacity = Settings.Instance.CommentAlpha / 100.0;
                        Handler.WebBrowser?.InvokeScript("Comment$Dispatch", item.ToJson());
                    }
                }
            }


        }

    }
}
