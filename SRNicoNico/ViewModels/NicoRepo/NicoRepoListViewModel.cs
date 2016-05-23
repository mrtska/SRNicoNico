using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class NicoRepoListViewModel : TabItemViewModel {


        private readonly string Id;

        private IList<NicoNicoNicoRepoDataEntry> RawData;

        #region Result変更通知プロパティ
        private NicoRepoResultViewModel _Result;

        public NicoRepoResultViewModel Result {
            get { return _Result; }
            set { 
                if(_Result == value)
                    return;
                _Result = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Filter変更通知プロパティ
        private string _Filter = "すべて";

        public string Filter {
            get { return _Filter; }
            set { 
                if(_Filter == value)
                    return;
                _Filter = value;
                RaisePropertyChanged();
                FilterNicoRepo();
            }
        }
        #endregion

        private NicoNicoNicoRepo NicoRepo;

        private NicoRepoViewModel NicoRepoVM;

        public NicoRepoListViewModel(NicoRepoViewModel vm, string title, string id) : base(title) {

            NicoRepoVM = vm;
            Id = id;
            Result = new NicoRepoResultViewModel(title);
        }
        /*
         * <ComboBoxItem Content="すべて" />
            <ComboBoxItem Content="動画投稿のみ" />
            <ComboBoxItem Content="生放送開始のみ" />
            <ComboBoxItem Content="マイリスト登録のみ" />
         */
        public void FilterNicoRepo() {

            switch(Filter) {
                case "すべて":
                    Result.NicoRepo.Clear();
                    foreach(var raw in RawData) {

                        Result.NicoRepo.Add(new NicoRepoResultEntryViewModel(raw, this));
                    }
                    break;
                case "動画投稿のみ":

                    Result.NicoRepo.Clear();
                    var video = RawData.Where(e => e.Title.Contains("動画を投稿しました。"));

                    foreach(var raw in video) {

                        Result.NicoRepo.Add(new NicoRepoResultEntryViewModel(raw, this));
                    }
                    break;
                case "生放送予約/開始のみ":

                    Result.NicoRepo.Clear();
                    var live = RawData.Where(e => e.LogId.Contains("log-target-live-program"));
                    foreach(var raw in live) {

                        Result.NicoRepo.Add(new NicoRepoResultEntryViewModel(raw, this));
                    }
                    break;
                case "マイリスト登録のみ":

                    Result.NicoRepo.Clear();
                    var mylist = RawData.Where(e => e.Title.Contains("マイリスト登録しました。"));

                    foreach(var raw in mylist) {

                        Result.NicoRepo.Add(new NicoRepoResultEntryViewModel(raw, this));
                    }
                    break;
            }
        }


        //ニコレポリストを開く UIスレッドで呼んではいけない
        public void OpenNicoRepoList() {

            Result.IsActive = true;
           
            NicoRepoVM.Status = "ニコレポ取得中(" + Name + ")";


            Result.OwnerViewModel = this;
            Result.NicoRepo.Clear();


            NicoRepo = new NicoNicoNicoRepo(Id);

            RawData = NicoRepo.GetNicoRepo();

            if(RawData == null) {

                NicoRepoVM.Status = "ニコレポ(" + Name + ") の取得に失敗しました";
                Result.IsActive = false;
                return;
            }

            foreach(var entry in RawData) {

                Result.NicoRepo.Add(new NicoRepoResultEntryViewModel(entry, this));
            }

            Result.IsActive = false;

        }

        public void NextNicoRepoList() {

            Result.IsActive = true;

            Task.Run(() => {

                var data = NicoRepo.NextNicoRepo();

                if(data == null) {

                    Result.IsActive = false;
                    return;
                }

                foreach(var entry in data) {

                    RawData.Add(entry);

                    switch(Filter) {
                        case "すべて":
                            Result.NicoRepo.Add(new NicoRepoResultEntryViewModel(entry, this));
                            break;
                        case "動画投稿のみ":
                            if(entry.Title.Contains("動画を投稿しました。")) {
                                Result.NicoRepo.Add(new NicoRepoResultEntryViewModel(entry, this));
                            }
                            break;
                        case "生放送開始のみ":
                            if(entry.LogId.Contains("log-target-live-program")) {
                                Result.NicoRepo.Add(new NicoRepoResultEntryViewModel(entry, this));
                            }
                            break;
                        case "マイリスト登録のみ":
                            if(entry.Title.Contains("マイリスト登録しました。")) {
                                Result.NicoRepo.Add(new NicoRepoResultEntryViewModel(entry, this));
                            }
                            break;
                    }
                    
                }

                Result.IsActive = false;
            });
        }

        public void Refresh() {

            Task.Run(() => {

                Filter = "すべて";
                OpenNicoRepoList();
                NicoRepoVM.Status = "";
            });


        }
    }
}
