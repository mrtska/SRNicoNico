using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Collections.ObjectModel;

namespace SRNicoNico.ViewModels {


    public class VideoMylistViewModel : ViewModel {


        private VideoViewModel Video;

        #region MylistStatus変更通知プロパティ
        private string _MylistStatus;

        public string MylistStatus {
            get { return _MylistStatus; }
            set { 
                if(_MylistStatus == value)
                    return;
                _MylistStatus = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsDeflistButtonEnabled変更通知プロパティ
        private bool _IsDeflistButtonEnabled;

        public bool IsDeflistButtonEnabled {
            get { return _IsDeflistButtonEnabled; }
            set { 
                if(_IsDeflistButtonEnabled == value)
                    return;
                _IsDeflistButtonEnabled = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Mylists変更通知プロパティ
        private ObservableCollection<NicoNicoMylistGroupData> _Mylists = new ObservableCollection<NicoNicoMylistGroupData>();

        public ObservableCollection<NicoNicoMylistGroupData> Mylists {
            get { return _Mylists; }
            set { 
                if(_Mylists == value)
                    return;
                _Mylists = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedMylist変更通知プロパティ
        private NicoNicoMylistGroupData _SelectedMylist;

        public NicoNicoMylistGroupData SelectedMylist {
            get { return _SelectedMylist; }
            set { 
                if(_SelectedMylist == value)
                    return;
                _SelectedMylist = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public VideoMylistViewModel(VideoViewModel vm) {

            Video = vm;
            
            Task.Run(() => Mylists = new ObservableCollection<NicoNicoMylistGroupData>(MylistViewModel.MylistGroupInstance.GetMylistGroup()));
        }

        public void EnableButtons() {
            

            IsDeflistButtonEnabled = true;
        }

        //とりあえずマイリストに登録
        public void AddDeflist() {

            Task.Run(() => {

                IsDeflistButtonEnabled = false;
                Video.Status = "とりあえずマイリストに登録中";

                MylistResult result = MylistViewModel.MylistInstance.AddDefMylist(Video.VideoData.ApiData.Cmsid, Video.VideoData.ApiData.Token);

                if(result == MylistResult.EXIST) {

                    Video.Status = "既にとりあえずマイリストに登録済みです";
                } else if(result == MylistResult.FAILED) {

                    Video.Status = "登録に失敗しました";
                } else {

                    Video.Status = "とりあえずマイリストに登録しました";
                }

                IsDeflistButtonEnabled = true;
            });
        }

        public void OpenMylistDialog() {

            Video.Messenger.Raise(new TransitionMessage(typeof(Views.Contents.Video.AddVideoMylistDialog), this, TransitionMode.Modal));
        }

        public void AddMylist() {

            Task.Run(() => {

                Video.Status = SelectedMylist.Name + "に登録中";
                MylistResult result = MylistViewModel.MylistInstance.AddMylist(SelectedMylist, Video.VideoData.ApiData.Cmsid, Video.VideoData.ApiData.Token);

                if(result == MylistResult.EXIST) {

                    Video.Status = "既に登録済みです";
                } else if(result == MylistResult.FAILED) {

                    Video.Status = "登録に失敗しました";
                } else {

                    Video.Status = SelectedMylist.Name + "に登録しました";
                }
            });
        }

        public void CloseDialog() {

            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "WindowAction"));
        }
    }
}
