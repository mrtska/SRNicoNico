using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Livet;
using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// MainWindowクラスのDataContext
    /// </summary>
    public class MainWindowViewModel : ViewModel {

        /// <summary>
        /// 現在のバージョン
        /// </summary>
        public double CurrentVersion {

            get { return 2.00; }
        }

#if DEBUG
        private string _Title = "NicoNicoViewer Debug Build ";
#else
        private string _Title = "NicoNicoViewer";
#endif
        public string Title {
            get { return _Title; }
            set { 
                if (_Title == value)
                    return;
                _Title = value;
                RaisePropertyChanged();
            }
        }

        public MainContentViewModel? MainContent { get; private set; }


        public MainWindowViewModel() {

        }

        /// <summary>
        /// 各ViewModelを初期化する
        /// </summary>
        /// <param name="container">DIコンテナ</param>
        public void Initialize(IUnityContainer container) {

            MainContent = container.Resolve<MainContentViewModel>();
            CompositeDisposable.Add(MainContent);
        }

        public void KeyDown(KeyEventArgs e) {

            MainContent?.SelectedItem?.KeyDown(e);
        }
        public void KeyUp(KeyEventArgs e) {

            MainContent?.SelectedItem?.KeyUp(e);
        }
        public void MouseDown(MouseButtonEventArgs e) {

            MainContent?.SelectedItem?.MouseDown(e);
        }

    }
}
