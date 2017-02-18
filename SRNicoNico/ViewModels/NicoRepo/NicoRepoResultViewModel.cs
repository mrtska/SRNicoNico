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
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class NicoRepoResultViewModel : TabItemViewModel {

        private readonly NicoNicoNicoRepo NicoRepoInstance;

        private readonly string Api;

        private string NextPageTime;


        #region IsEmpty変更通知プロパティ
        private bool _IsEmpty;

        public bool IsEmpty {
            get { return _IsEmpty; }
            set { 
                if(_IsEmpty == value)
                    return;
                _IsEmpty = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        private List<ViewModel> UnFilteredNicoRepoList = new List<ViewModel>();

        #region NicoRepoList変更通知プロパティ
        private DispatcherCollection<ViewModel> _NicoRepoList = new DispatcherCollection<ViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<ViewModel> NicoRepoList {
            get { return _NicoRepoList; }
            set { 
                if(_NicoRepoList == value)
                    return;
                _NicoRepoList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Filter変更通知プロパティ
        private string _Filter;

        public string Filter {
            get { return _Filter; }
            set { 
                if(_Filter == value)
                    return;
                _Filter = value;
                FilterNicoRepo();
            }
        }
        #endregion


        private bool IsEnd;

        public NicoRepoResultViewModel(string title, string api, NicoNicoNicoRepo nicorepo) : base(title) {

            NicoRepoInstance = nicorepo;
            Api = api;
        }


        public void Initialize() {

            IsActive = true;
            IsEmpty = false;
            NextPageTime = null;
            UnFilteredNicoRepoList.Clear();
            NicoRepoList.Clear();
            GetMore();
        }

        public async void GetMore() {

            if(IsEmpty || (NicoRepoList.Count != 0 && !(NicoRepoList.LastOrDefault() is NicoRepoNextButtonEntryViewModel))) {

                return;
            }

            IsActive = true;
            
            //一番最後にあるボタンを消す
            if(NicoRepoList.Count > 0) {

                NicoRepoList.RemoveAt(NicoRepoList.Count - 1);
            }

            var ret = await NicoRepoInstance.GetNicoRepoAsync(Api, NextPageTime);
            
            if(ret != null) {

                foreach(var entry in ret.Items) {

                    UnFilteredNicoRepoList.Add(new NicoRepoResultEntryViewModel(entry));
                }
                NextPageTime = ret.NextPage;
                IsEnd = ret.IsEnd;

                FilterNicoRepo();
            }

            if(UnFilteredNicoRepoList.Count == 0) {

                IsEmpty = true;
            }


            IsActive = false;
        }

        public void FilterNicoRepo() {

            switch(Filter) {
                case "すべて":
                    NicoRepoList.Clear();
                    foreach(var raw in UnFilteredNicoRepoList) {

                        NicoRepoList.Add(raw);
                    }
                    break;
                case "動画投稿のみ":

                    NicoRepoList.Clear();
                    var video = UnFilteredNicoRepoList.Where((e) => ((NicoRepoResultEntryViewModel)e).Item.Title.Contains("動画を投稿しました。"));

                    foreach(var raw in video) {

                        NicoRepoList.Add(raw);
                    }
                    break;
                case "生放送開始のみ":

                    NicoRepoList.Clear();
                    var live = UnFilteredNicoRepoList.Where(e => ((NicoRepoResultEntryViewModel)e).Item.Title.Contains("生放送を開始しました。"));
                    foreach(var raw in live) {

                        NicoRepoList.Add(raw);
                    }
                    break;
                case "マイリスト登録のみ":

                    NicoRepoList.Clear();
                    var mylist = UnFilteredNicoRepoList.Where(e => ((NicoRepoResultEntryViewModel)e).Item.Title.Contains("マイリスト登録しました。"));

                    foreach(var raw in mylist) {

                        NicoRepoList.Add(raw);
                    }
                    break;
            }
            if(!IsEnd) {

                NicoRepoList.Add(new NicoRepoNextButtonEntryViewModel(this));
            }
        }

        public void Refresh() {

            Initialize();
        }

        public override void KeyDown(KeyEventArgs e) {
        
            if(e.Key == Key.F5) {

                Refresh();
            }
            if(e.Key == Key.Space) {

                GetMore();
            }
        }
    }
}
