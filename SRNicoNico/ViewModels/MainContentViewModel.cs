using System;
using System.Collections.Generic;
using System.Text;
using Livet;
using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// MainContentクラスのDataContext
    /// </summary>
    public class MainContentViewModel : ViewModel {

        /// <summary>
        /// システムタブのリスト
        /// WebViewや視聴履歴など
        /// </summary>
        public ObservableSynchronizedCollection<TabItemViewModel> SystemItems { get; private set; }

        /// <summary>
        /// ユーザータブのリスト
        /// 動画やユーザーページなど
        /// </summary>
        public ObservableSynchronizedCollection<TabItemViewModel> UserItems { get; private set; }


        public MainContentViewModel(IUnityContainer container) {

            SystemItems = new ObservableSynchronizedCollection<TabItemViewModel>();
            UserItems = new ObservableSynchronizedCollection<TabItemViewModel>();
            Initialize(container);
        }

        private void Initialize(IUnityContainer container) {

            SystemItems.Add(container.Resolve<StartViewModel>());
        }

    }
}
