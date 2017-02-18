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
    public class TabItemViewModel : ViewModel, ITabItem {

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

        public virtual string Status {
            get { return _Status; }
            set {
                if(_Status == value)
                    return;
                _Status = value;
                if(App.ViewModelRoot.MainContent.SelectedTab == this) {

                    App.ViewModelRoot.Status = value;
                }
                RaisePropertyChanged();
            }
        }
        #endregion


        #region IsActive変更通知プロパティ
        private bool _IsActive;

        public virtual bool IsActive {
            get { return _IsActive; }
            set {
                _IsActive = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public TabItemViewModel(string name = "") {

            Name = name;
        }

        //キーイベントをハンドリングする時はオーバーライド
        public virtual void KeyDown(KeyEventArgs e) { }
        public virtual void KeyUp(KeyEventArgs e) { }


        //マウスイベントをハンドリングする時はオーバーライド
        public virtual void MouseDown(MouseButtonEventArgs e) { }

        public virtual bool CanShowHelp() {

            return false;
        }
        public virtual void ShowHelpView(InteractionMessenger Messenger) { }
    }
}
