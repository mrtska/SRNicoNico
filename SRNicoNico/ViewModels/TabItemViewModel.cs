using System.Windows.Input;
using Livet;
using MetroRadiance.UI.Controls;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// タブの中に表示されるUIを持つViewModelが継承する
    /// </summary>
    public class TabItemViewModel : ViewModel, ITabItem {

        private string? _Name;
        /// <summary>
        /// タブに表示する名前
        /// </summary>
        public string? Name {
            get { return _Name; }
            set { 
                if (_Name == value)
                    return;
                _Name = value;
                RaisePropertyChanged();
            }
        }

        private int? _Badge;
        /// <summary>
        /// タブに表示するバッジ
        /// </summary>
        public int? Badge {
            get { return _Badge; }
            set { 
                if (_Badge == value)
                    return;
                _Badge = value;
                RaisePropertyChanged();
            }
        }

        private bool _IsSelected;
        /// <summary>
        /// タブが選択されているかどうか
        /// </summary>
        public bool IsSelected {
            get { return _IsSelected; }
            set { 
                if (_IsSelected == value)
                    return;
                _IsSelected = value;
                RaisePropertyChanged();
            }
        }

        public TabItemViewModel(string tabName = "") {

            Name = tabName;
        }

        /// <summary>
        /// このタブを開いている時にキーボードを叩くと呼ばれる
        /// </summary>
        /// <param name="e">キーイベント</param>
        public virtual void KeyDown(KeyEventArgs e) { }
        /// <summary>
        /// このタブを開いている時にキーボードを叩くと呼ばれる
        /// </summary>
        /// <param name="e">キーイベント</param>
        public virtual void KeyUp(KeyEventArgs e) { }

        /// <summary>
        /// このタブを開いている時にマウスのクリックなどをすると呼ばれる
        /// </summary>
        /// <param name="e">マウスイベント</param>
        public virtual void MouseDown(MouseButtonEventArgs e) { }

    }
}
