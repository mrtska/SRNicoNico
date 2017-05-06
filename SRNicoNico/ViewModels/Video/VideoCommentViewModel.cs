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
using System.Text.RegularExpressions;

namespace SRNicoNico.ViewModels {
    public class VideoCommentViewModel : ViewModel {

        #region IsActive変更通知プロパティ
        private bool _IsActive;

        public bool IsActive {
            get { return _IsActive; }
            set { 
                if(_IsActive == value)
                    return;
                _IsActive = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CommentListList変更通知プロパティ
        private DispatcherCollection<VideoCommentListViewModel> _CommentListList = new DispatcherCollection<VideoCommentListViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<VideoCommentListViewModel> CommentListList {
            get { return _CommentListList; }
            set { 
                if(_CommentListList == value)
                    return;
                _CommentListList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedList変更通知プロパティ
        private VideoCommentListViewModel _SelectedList;

        public VideoCommentListViewModel SelectedList {
            get { return _SelectedList; }
            set { 
                if(_SelectedList == value)
                    return;
                _SelectedList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CommentVisibility変更通知プロパティ
        public bool CommentVisibility {
            get { return Settings.Instance.CommentVisibility; }
            set {
                if(Settings.Instance.CommentVisibility == value)
                    return;
                Settings.Instance.CommentVisibility = value;
                if(value) {


                    Owner?.Handler?.InvokeScript("CommentViewModel$show_comment");
                } else {

                    Owner?.Handler?.InvokeScript("CommentViewModel$hide_comment");
                }
                RaisePropertyChanged();
            }
        }
        #endregion

        #region AutoScrollEnabled変更通知プロパティ
        public bool AutoScrollEnabled {
            get { return Settings.Instance.CommentAutoScroll; }
            set { 
                if(Settings.Instance.CommentAutoScroll == value)
                    return;
                Settings.Instance.CommentAutoScroll = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CanComment変更通知プロパティ
        private bool _CanComment = false;

        public bool CanComment {
            get { return _CanComment; }
            set { 
                if(_CanComment == value)
                    return;
                _CanComment = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CurrentVpos変更通知プロパティ
        private int _CurrentVpos;

        public int CurrentVpos {
            get { return _CurrentVpos; }
            set { 
                if(_CurrentVpos == value)
                    return;
                _CurrentVpos = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Post変更通知プロパティ
        private VideoPostCommentViewModel _Post;

        public VideoPostCommentViewModel Post {
            get { return _Post; }
            set { 
                if(_Post == value)
                    return;
                _Post = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        //ニコスクリプト
        private List<NicoScriptBase> NicoScript;

        internal NicoNicoComment CommentInstance;

        internal VideoViewModel Owner;

        private bool Initialized = false;

        public VideoCommentViewModel(VideoViewModel vm) {

            Owner = vm;
            CommentInstance = new NicoNicoComment(vm);
            Post = new VideoPostCommentViewModel(this);
        }

        

        public async Task Initialize() {

            IsActive = true;

            CommentListList.Clear();

            var lists = await CommentInstance.GetAllCommentAsync();

            if(lists == null) {

                return;
            }

            //コメントを取得できるやつは取得する
            foreach(var list in lists) {

                var listlist = new VideoCommentListViewModel(list);
                
                foreach(var entry in list.CommentList) {
                    
                    //コメントをふるいにかける
                    App.ViewModelRoot.Setting.NGFilter.Filter(entry);

                    if(entry.Rejected) {

                        continue;
                    }

                    //投稿者コメントでかつ@で始まるニコスクリプトだったら
                    if(entry.IsUploader && entry.Content.StartsWith("@")) {

                        var match = Regex.Match(entry.Content, @"(\@.+?)($| )");

                        //ニコスクリプトをパースしてリストに追加
                        if(match.Success) {

                            if(NicoScript == null) {

                                NicoScript = new List<NicoScriptBase>();
                            }

                            var script = NicoNicoNicoScriptInterpreter.GetScriptInstance(this, entry);
                            if(script != null) {

                                NicoScript.Add(script);
                                entry.NicoScript = script;
                            }
                        }
                    }
                    listlist.CommentList.Add(new VideoCommentEntryViewModel(this, entry));
                }

                CommentListList.Add(listlist);
            }

            if(CommentListList.Count > 0) {

                SelectedList = CommentListList.First();
            }

            IsActive = false;
            CanComment = true;
            Owner?.Handler?.InvokeScript("CommentViewModel$initialize");
            ApplyFontSize();
            if(CommentVisibility) {

                Owner?.Handler?.InvokeScript("CommentViewModel$show_comment");
            } else {

                Owner?.Handler?.InvokeScript("CommentViewModel$hide_comment");
            }

            Initialized = true;
        }

        public void CommentTick(int vpos) {
            
            if(!Initialized) {

                return;
            }

            //同じだったら無視
            if(CurrentVpos == vpos) {

                return;
            }


            foreach(var list in CommentListList) {

                foreach(var entry in list.CommentList) {

                    var item = entry.Item;

                    //描画が終わる時間よりも現在時間のほうが大きかったらまだまだ
                    if(item.Vend < vpos) {

                        item.IsRendering = false;
                        continue;
                    }

                    //コメントリストは描画順でソートされているのでvposより大きいやつが来たらそれ以降は無視
                    if(item.Vpos > vpos) {

                        item.IsRendering = false;
                        continue;
                    }

                    //投稿者コメントでかつニコスクリプトだったら
                    if(item.IsUploader && item.NicoScript != null && item.NicoScript.AffectOtherComments) {

                        continue;
                    }

                    if(!item.IsRendering && vpos >= item.Vpos && vpos <= item.Vend) {

                        item.IsRendering = true;


                        //ニコスクリプトが存在したら対象の
                        if(NicoScript != null) {

                            foreach(var script in NicoScript) {

                                if(script.AffectOtherComments) {

                                    script.ExecuteIfValidTime(item);
                                }
                            }
                        }

                        if(item.NicoScript != null && !item.NicoScript.AffectOtherComments) {

                            item.NicoScript.Execute(null);
                            continue;
                        }
                        Owner?.Handler?.InvokeScript("CommentViewModel$dispatch", item.ToJson());
                    }
                }
            }
            CurrentVpos = vpos;

        }

        public async void Refresh() {

            IsActive = true;

            var targetList = CommentListList.Select(e => e.CommentListInstance).ToList();

            CommentListList.Clear();

            //コメントを取得できるやつは取得する
            foreach (var list in await CommentInstance.RefreshCommentAsync(targetList)) {

                var listlist = new VideoCommentListViewModel(list);

                foreach (var entry in list.CommentList) {

                    listlist.CommentList.Add(new VideoCommentEntryViewModel(this, entry));
                }

                CommentListList.Add(listlist);
            }


            if (CommentListList.Count > 0) {

                SelectedList = CommentListList.First();
            }

            IsActive = false;
            CanComment = true;
            Owner?.Handler?.InvokeScript("CommentViewModel$initialize");
            if (CommentVisibility) {

                Owner?.Handler?.InvokeScript("CommentViewModel$show_comment");
            } else {

                Owner?.Handler?.InvokeScript("CommentViewModel$hide_comment");
            }


        }

        public void ApplyFontSize() {

            Owner?.Handler?.InvokeScript("CommentViewModel$setbasesize", Settings.Instance.CommentSize);
        }

        public void ToggleAutoScroll() {

            AutoScrollEnabled ^= true;
        }

    }
}
