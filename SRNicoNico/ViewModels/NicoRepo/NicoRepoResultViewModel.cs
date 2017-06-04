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

        public NicoRepoResultViewModel(string title, string api, NicoNicoNicoRepo nicorepo) : base(title) {

            NicoRepoInstance = nicorepo;
            Api = api;
        }


        public void Initialize() {

            IsActive = true;
            IsEmpty = false;
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

            var ret = await NicoRepoInstance.GetNicoRepoAsync(Api, UnFilteredNicoRepoList.Count != 0 ? ((NicoRepoResultEntryViewModel) UnFilteredNicoRepoList.Last()).Item.Id : null);
            
            if(ret != null) {

                foreach(var entry in ret.Item1) {

                    var vm = new NicoRepoResultEntryViewModel(entry);
                    UnFilteredNicoRepoList.Add(vm);

                    if (FilterEntry(vm)) {

                        NicoRepoList.Add(vm);
                    }
                }

                if(ret.Item2) {

                    NicoRepoList.Add(new NicoRepoNextButtonEntryViewModel(this));
                }
            }

            if(UnFilteredNicoRepoList.Count == 0) {

                IsEmpty = true;
            }

            IsActive = false;
        }

        public void FilterNicoRepo() {

            if(IsEmpty || UnFilteredNicoRepoList.Count == 0) {

                return;
            }

            bool isnotEnd = NicoRepoList.Count != 0 && NicoRepoList.Last() is NicoRepoNextButtonEntryViewModel;
            NicoRepoList?.Clear();

            foreach (var raw in UnFilteredNicoRepoList) {

                if(raw is NicoRepoResultEntryViewModel item) {

                    if (FilterEntry(item)) {

                        NicoRepoList.Add(raw);
                    }
                }
            }
            if(isnotEnd) {

                NicoRepoList.Add(new NicoRepoNextButtonEntryViewModel(this));
            }
        }

        private bool FilterEntry(NicoRepoResultEntryViewModel item) {

            if (item.Item.Muted) {

                return false;
            }

            switch (Filter) {
                case "すべて":
                    return true;
                case "動画投稿のみ":
                    if(item.Item.Topic.EndsWith("video.upload")) {

                        return true;
                    }
                    return false;
                case "生放送開始のみ":
                    if (item.Item.Topic.EndsWith("program.onairs")) {

                        return true;
                    }
                    return false;
                case "マイリスト登録のみ":
                    if (item.Item.Topic.Contains("mylist.add")) {

                        return true;
                    }
                    return false;
                default:
                    return true;


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
