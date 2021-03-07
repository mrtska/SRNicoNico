using System;
using System.Collections.Generic;
using System.Text;
using Livet;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ページネーションを使う画面で使用するViewModel
    /// </summary>
    public abstract class PageSpinnerViewModel : TabItemViewModel {

        private bool _LeftButtonEnabled;
        /// <summary>
        /// スピナーの左のボタンが押せるかどうか
        /// </summary>
        public bool LeftButtonEnabled {
            get { return _LeftButtonEnabled; }
            set { 
                if (_LeftButtonEnabled == value)
                    return;
                _LeftButtonEnabled = value;
                RaisePropertyChanged();
            }
        }

        private bool _RightButtonEnabled;
        /// <summary>
        /// スピナーの右のボタンが押せるかどうか
        /// </summary>
        public bool RightButtonEnabled {
            get { return _RightButtonEnabled; }
            set { 
                if (_RightButtonEnabled == value)
                    return;
                _RightButtonEnabled = value;
                RaisePropertyChanged();
            }
        }

        private int? _Total;
        /// <summary>
        /// 表示出来ていない部分も含むアイテムの総数
        /// デフォルトはnull 新たにnullを設定することは許可しない
        /// </summary>
        public int? Total {
            get { return _Total; }
            set { 
                if (value == null || _Total == value)
                    return;
                _Total = value;
                MaxPages = (int)value / ItemPeriod + 1;
                RefreshArrowButtons();
                RaisePropertyChanged();
            }
        }

        private int _MaxPages;
        /// <summary>
        /// 最大ページ数
        /// </summary>
        public int MaxPages {
            get { return _MaxPages; }
            set { 
                if (_MaxPages == value)
                    return;
                _MaxPages = value;
                RaisePropertyChanged();
            }
        }

        private int _ItemPeriod;
        /// <summary>
        /// アイテムがいくつごと表示されるか
        /// </summary>
        public int ItemPeriod {
            get { return _ItemPeriod; }
            set { 
                if (_ItemPeriod == value)
                    return;
                _ItemPeriod = value;
                RaisePropertyChanged();
            }
        }

        private int _CurrentPage;
        /// <summary>
        /// 現在のページ
        /// </summary>
        public int CurrentPage {
            get { return _CurrentPage; }
            set { 
                if (_CurrentPage == value)
                    return;
                if (value > MaxPages) {
                    value = MaxPages;
                }
                if (value < 1) {
                    value = 1;
                }
                _CurrentPage = value;

                RefreshArrowButtons();
                RaisePropertyChanged();
            }
        }

        public override bool IsActive {
            get { return base.IsActive; }
            set { 
                if (base.IsActive == value)
                    return;
                base.IsActive = value;
                if (value) {

                    LeftButtonEnabled = false;
                    RightButtonEnabled = false;
                } else {

                    RefreshArrowButtons();
                }
            }
        }


        public PageSpinnerViewModel(string name, int itemPeriod) : base(name) {

            ItemPeriod = itemPeriod;
        }

        /// <summary>
        /// 矢印ボタンの有効状態を現在のページ状態に合わせて更新する
        /// </summary>
        private void RefreshArrowButtons() {

            LeftButtonEnabled = MaxPages != 1 && CurrentPage > 1;
            RightButtonEnabled = MaxPages != 1 && CurrentPage < MaxPages;
        }

        /// <summary>
        /// ページが切り替えられた時に呼ばれる
        /// </summary>
        /// <param name="page">新しいページ</param>
        public virtual void SpinPage(int page) {

            CurrentPage = page;
        }


        /// <summary>
        /// スピナーの左のボタンが押された時に呼ばれる処理
        /// </summary>
        public void LeftButtonClick() {

            if (LeftButtonEnabled) {

                SpinPage(--CurrentPage);
            }
        }

        /// <summary>
        /// スピナーの右のボタンが押された時に呼ばれる処理
        /// </summary>
        public void RightButtonClick() {

            if (RightButtonEnabled) {

                SpinPage(++CurrentPage);
            }
        }
    }
}
