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
using MetroRadiance.Controls;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {

    public class TabItemViewModel : ViewModel,ITabItem {

        #region IsSelected変更通知プロパティ
        private bool _IsSelected;

        public bool IsSelected {
            get { return _IsSelected; }
            set { 
                if(_IsSelected == value)
                    return;
                _IsSelected = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Name変更通知プロパティ
        private string _Name;

        public string Name {
            get { return _Name; }
            set { 
                if(_Name == value)
                    return;
                _Name = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Badge 変更通知プロパティ

        private int? _Badge;

        /// <summary>
        /// バッジを取得します。
        /// </summary>
        public virtual int? Badge {
            get { return _Badge; }
            protected set {
                if(_Badge != value) {
                    _Badge = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region Status変更通知プロパティ
        private string _Status;

        public string Status {
            get { return _Status; }
            set { 
                if(_Status == value)
                    return;
                _Status = value;
                if(App.ViewModelRoot.SelectedTab == this) {

                    App.ViewModelRoot.Status = value;
                }
                RaisePropertyChanged();
            }
        }
        #endregion


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


        public TabItemViewModel(string name = "") {

            Name = name;
        }

        public virtual void KeyDown(KeyEventArgs e) {


        }

        //ダイアログを閉じる
        public void CloseDialog() {

            Messenger.Raise(new WindowActionMessage(Livet.Messaging.Windows.WindowAction.Close, "WindowAction"));
        }
    }
}
