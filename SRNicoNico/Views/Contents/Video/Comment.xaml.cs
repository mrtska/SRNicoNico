using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SRNicoNico.Views.Contents.Video {
	/// <summary>
	/// Comment.xaml の相互作用ロジック
	/// </summary>
	public partial class Comment : UserControl {
		public Comment() {
			InitializeComponent();
		}
	}

	public class FollowablePopup : Popup
    {
        static FollowablePopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FollowablePopup), new FrameworkPropertyMetadata(typeof(FollowablePopup)));

            // PopupのIsOpenプロパティ更新のイベントハンドラを設定する。
            FollowablePopup.IsOpenProperty.OverrideMetadata(typeof(FollowablePopup), new FrameworkPropertyMetadata(IsOpenChanged));
        }

        private static void IsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as FollowablePopup;
            if (ctrl == null)
                return;

            var target = ctrl.PlacementTarget;
            if (target == null)
                return;

            var win = Window.GetWindow(target);
            // ポップアップの親要素にいるScrollViewer要素があれば取得する。
            var scrollViewer = ctrl.GetDependencyObjectFromVisualTree(ctrl, typeof(ScrollViewer)) as ScrollViewer;

            // 更新前のIsOpenプロパティがtrueだったので、
            // 登録済みのイベントハンドラを解除する。
            if (e.OldValue != null && (bool)e.OldValue == true)
            {
                if (win != null)
                {
                    // ウィンドウの移動/リサイズ時の処理を設定
                    win.LocationChanged -= ctrl.OnFollowWindowChanged;
                    win.SizeChanged -= ctrl.OnFollowWindowChanged;
                }

                if (scrollViewer != null)
                {
                    // ListBoxなどのようなScrollViewerを持った要素内に設定された場合の動作
                    scrollViewer.ScrollChanged -= ctrl.OnScrollChanged;
                }
            }

            // IsOpenプロパティをtrueに変更したので、
            // 各種イベントハンドラを登録する。
            if (e.NewValue != null && (bool)e.NewValue == true)
            {
                if (win != null)
                {
                    // ウィンドウの移動/リサイズ時の処理を設定
                    win.LocationChanged += ctrl.OnFollowWindowChanged;
                    win.SizeChanged += ctrl.OnFollowWindowChanged;
                }

                if (scrollViewer != null)
                {
                    // ListBoxなどのようなScrollViewerを持った要素内に設定された場合の動作
                    scrollViewer.ScrollChanged += ctrl.OnScrollChanged;
                }
            }
        }

        private void OnFollowWindowChanged(object sender, EventArgs e)
        {
            var offset = this.HorizontalOffset;
            // HorizontalOffsetなどのプロパティを一度変更しないと、ポップアップの位置が更新されないため、
            // 同一プロパティに2回値をセットしている。
            this.HorizontalOffset = offset + 1;
            this.HorizontalOffset = offset;
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.IsOpen = false;
        }

        private DependencyObject GetDependencyObjectFromVisualTree(DependencyObject startObject, Type type)
        {
            var parent = startObject;
            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                    break;
                else
                    parent = VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }
    }
}
