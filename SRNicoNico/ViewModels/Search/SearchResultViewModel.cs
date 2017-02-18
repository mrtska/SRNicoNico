using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class SearchResultViewModel : ViewModel {



        //検索結果の総数
        #region Total変更通知プロパティ
        private int _Total = -1;

        public int Total {
            get { return _Total; }
            set {
                if(_Total == value)
                    return;
                _Total = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region MaxPages変更通知プロパティ
        private int _MaxPages = 0;

        public int MaxPages {
            get { return _MaxPages; }
            set {
                if(_MaxPages == value)
                    return;
                if(value > 50) {

                    value = 50;
                }
                _MaxPages = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region CurrentPage変更通知プロパティ
        private int _CurrentPage = 1;

        public int CurrentPage {
            get { return _CurrentPage; }
            set {
                if(_CurrentPage == value)
                    return;
                if(value > MaxPages) {

                    value = MaxPages;
                }
                if(value <= 1) {

                    LeftButtonEnabled = false;
                } else {

                    LeftButtonEnabled = true;
                }
                if(value >= MaxPages) {

                    RightButtonEnabled = false;
                } else {

                    RightButtonEnabled = true;
                }

                _CurrentPage = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region LeftButtonEnabled変更通知プロパティ
        private bool _LeftButtonEnabled = false;

        public bool LeftButtonEnabled {
            get { return _LeftButtonEnabled; }
            set {
                if(_LeftButtonEnabled == value)
                    return;
                _LeftButtonEnabled = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region RightButtonEnabled変更通知プロパティ
        private bool _RightButtonEnabled = true;

        public bool RightButtonEnabled {
            get { return _RightButtonEnabled; }
            set {
                if(_RightButtonEnabled == value)
                    return;
                _RightButtonEnabled = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region IsActive変更通知プロパティ
        private bool _IsActive;

        private bool tmpLeftButton;
        private bool tmpRightButton;

        public bool IsActive {
            get { return _IsActive; }
            set {
                if(_IsActive == value)
                    return;
                _IsActive = value;
                if(value) {

                    tmpLeftButton = LeftButtonEnabled;
                    tmpRightButton = RightButtonEnabled;

                    LeftButtonEnabled = false;
                    RightButtonEnabled = false;
                } else {

                    LeftButtonEnabled = tmpLeftButton;
                    RightButtonEnabled = tmpRightButton;
                }
                RaisePropertyChanged();
            }
        }
        #endregion


        #region List変更通知プロパティ
        private DispatcherCollection<SearchResultEntryViewModel> _List = new DispatcherCollection<SearchResultEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<SearchResultEntryViewModel> List {
            get { return _List; }
            set {
                if(_List == value)
                    return;
                _List = value;
                RaisePropertyChanged();

            }
        }
        #endregion

        public SearchViewModel Owner { get; private set; }

        public SearchResultViewModel(SearchViewModel vm) {

            Owner = vm;
        }

        public void LeftButtonClick() {

            if(LeftButtonEnabled) {

                CurrentPage--;
                Owner.SearchPage(CurrentPage);
            }
        }
        public void RightButtonClick() {

            if(RightButtonEnabled) {

                CurrentPage++;
                Owner.SearchPage(CurrentPage);
            }
        }

        public void MakePlayList() {

            var filteredList = List.Select(e => e.Item);

            if(filteredList.Count() == 0) {

               Owner.Status = "連続再生できる動画がありません";
                return;
            }

            var vm = new PlayListViewModel(Owner.SearchText, filteredList);
            App.ViewModelRoot.MainContent.AddUserTab(vm);
        }
    }
}
