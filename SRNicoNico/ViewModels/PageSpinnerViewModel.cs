using System;
using System.Collections.Generic;
using System.Text;
using Livet;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ページネーションを使う画面で使用するViewModel
    /// </summary>
    public abstract class PageSpinnerViewModel : TabItemViewModel {


        private int _CurrentPage;

        public int CurrentPage {
            get { return _CurrentPage; }
            set { 
                if (_CurrentPage == value)
                    return;
                _CurrentPage = value;
                RaisePropertyChanged();
            }
        }

        public PageSpinnerViewModel(string name) : base(name) {

        }

        /// <summary>
        /// ページが切り替えられた時に呼ばれる
        /// </summary>
        /// <param name="page">新しいページ</param>
        public abstract void SpinPage(int page);

    }
}
