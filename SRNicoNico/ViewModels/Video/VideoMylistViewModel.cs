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


        public VideoMylistViewModel(VideoViewModel vm) {

            Video = vm;
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





        



    }
}
