using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.IO;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Views.Contents.Search;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows;
using System.Collections.ObjectModel;
using SRNicoNico.Views.Contents.SignIn;
using System.Windows.Input;
using System.Windows.Media;
using SRNicoNico.Models.NicoNicoViewer;
using System.Reflection;
using SRNicoNico.Views.Contents.Misc;
using SRNicoNico.Views;
using System.Windows.Controls;
using SRNicoNico.Views.Contents.Interface;

namespace SRNicoNico.ViewModels {
	public class ContentContainerViewModel : ViewModel {

        
		#region Title変更通知プロパティ
		private string _Title = "NicoNicoViewer ";

		public string Title {
			get { return _Title; }
			set { 
				if(_Title == value)
					return;
				_Title = value;
				RaisePropertyChanged();
			}
		}
        #endregion

        #region WindowState変更通知プロパティ
        private WindowState _WindowState;

        public WindowState WindowState {
            get { return _WindowState; }
            set { 
                if(_WindowState == value)
                    return;
                _WindowState = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        

        #region Visibility変更通知プロパティ
        private Visibility _Visibility;

        public Visibility Visibility {
            get { return _Visibility; }
            set { 
                if(_Visibility == value)
                    return;
                _Visibility = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        #region ContentViewModel変更通知プロパティ
        private TabItemViewModel _ContentViewModel;

        public TabItemViewModel ContentViewModel {
            get { return _ContentViewModel; }
            set { 
                if(_ContentViewModel == value)
                    return;
                _ContentViewModel = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Control変更通知プロパティ
        private Control _Control;

        public Control Control {
            get { return _Control; }
            set { 
                if(_Control == value)
                    return;
                _Control = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        public ContentContainerViewModel(TabItemViewModel vm) {

            ContentViewModel = vm;

            if(vm is IExternalizable) {

                var ext = (IExternalizable)vm;
                Control = ext.View;
            } else {

                throw new ArgumentException("IExternalizableを実装していません。" + vm);
            }

            Initialize();
		}

		public async void Initialize() {

            await App.ViewModelRoot.Messenger.RaiseAsync(new TransitionMessage(typeof(ContentContainer), this, TransitionMode.Normal));
		}


        public void KeyDown(KeyEventArgs e) {

            ContentViewModel?.KeyDown(e);
        }

    }
}
