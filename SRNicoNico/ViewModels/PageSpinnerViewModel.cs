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
using MetroRadiance.UI.Controls;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public abstract class PageSpinnerViewModel : TabItemViewModel {

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
                if(value > MaximumPages) {

                    value = MaximumPages;
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

        public override bool IsActive {
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

        private int MaximumPages;

        public PageSpinnerViewModel(string name, int maximum = 50) : base(name) {

            MaximumPages = maximum;
        }

        //ページ切り替え時に呼ばれるのでここでいろいろ
        //ページ取得はCurrentPageを
        public abstract void SpinPage();


        public void LeftButtonClick() {

            if(LeftButtonEnabled) {

                CurrentPage--;
                SpinPage();
            }
        }
        public void RightButtonClick() {

            if(RightButtonEnabled) {

                CurrentPage++;
                SpinPage();
            }
        }

        public override void KeyDown(KeyEventArgs e) {

            switch(e.Key) {
                case Key.Left:
                    LeftButtonClick();
                    e.Handled = true;
                    break;
                case Key.Right:
                    RightButtonClick();
                    e.Handled = true;
                    break;
            }
        }
    }
}
