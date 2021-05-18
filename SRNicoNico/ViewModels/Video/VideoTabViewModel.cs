using System;
using System.Collections.Generic;
using System.Text;
using Livet;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 動画のタブ管理用のViewModel
    /// </summary>
    public class VideoTabViewModel : TabItemViewModel {


        private ObservableSynchronizedCollection<VideoViewModel> _TabItems = new ObservableSynchronizedCollection<VideoViewModel>();

        public ObservableSynchronizedCollection<VideoViewModel> TabItems {
            get { return _TabItems; }
            set { 
                if (_TabItems == value)
                    return;
                _TabItems = value;
                RaisePropertyChanged();
            }
        }


        public VideoTabViewModel(Action collectionClearedAction) : base("動画") {

            // 動画が0になった時にアクションを実行する
            TabItems.CollectionChanged += (o, e) => {
                if (TabItems.Count == 0) {

                    collectionClearedAction();
                }
            };
        }

        public void Loaded() {


        }

        public void Add(VideoViewModel vm) {


        }

    }
}
