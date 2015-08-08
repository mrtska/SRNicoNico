﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SRNicoNico.Views.Controls {
	/// <summary>
	/// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
	///
	/// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
	/// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
	/// 追加します:
	///
	///     xmlns:MyNamespace="clr-namespace:SRNicoNico.Views.Controls"
	///
	///
	/// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
	/// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
	/// 追加します:
	///
	///     xmlns:MyNamespace="clr-namespace:SRNicoNico.Views.Controls;assembly=SRNicoNico.Views.Controls"
	///
	/// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
	/// リビルドして、コンパイル エラーを防ぐ必要があります:
	///
	///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
	///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
	///
	///
	/// 手順 2)
	/// コントロールを XAML ファイルで使用します。
	///
	///     <MyNamespace:SeekBar/>
	///
	/// </summary>
	public class SeekBar : Control {


		public long VideoTime {
			get { return (long)GetValue(VideoTimeProperty); }
			set { SetValue(VideoTimeProperty, value); }
		}


		// Using a DependencyProperty as the backing store for VideoTime.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty VideoTimeProperty =
			DependencyProperty.Register("VideoTime", typeof(long), typeof(SeekBar), new FrameworkPropertyMetadata(0L));



		

		public long CurrentTime {
			get { return (long)GetValue(CurrentTimeProperty); }
			set { SetValue(CurrentTimeProperty, value);	}
		}

		// Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CurrentTimeProperty =
			DependencyProperty.Register("CurrentTime", typeof(long), typeof(SeekBar), new FrameworkPropertyMetadata(0L));



		public double CurrentTimeWidth {
			get { return (double)GetValue(CurrentTimeWidthProperty); }
			set { SetValue(CurrentTimeWidthProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CurrentTimeString.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CurrentTimeWidthProperty =
			DependencyProperty.Register("CurrentTimeWidth", typeof(double), typeof(SeekBar), new FrameworkPropertyMetadata(0.0));





		public long BufferedTime {
			get { return (long)GetValue(BufferedTimeProperty); }
			set { SetValue(BufferedTimeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for BufferedTime.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BufferedTimeProperty =
			DependencyProperty.Register("BufferedTime", typeof(long), typeof(SeekBar), new FrameworkPropertyMetadata(0L));



		public double BufferedTimeString {
			get { return (double)GetValue(BufferedTimeStringProperty); }
			set { SetValue(BufferedTimeStringProperty, value); }
		}

		// Using a DependencyProperty as the backing store for BufferedTimeString.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BufferedTimeStringProperty =
			DependencyProperty.Register("BufferedTimeString", typeof(double), typeof(SeekBar), new FrameworkPropertyMetadata(0.0));



		public Thickness SeekCursor {
			get { return (Thickness)GetValue(SeekCursorProperty); }
			set { SetValue(SeekCursorProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SeekCursor.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SeekCursorProperty =
			DependencyProperty.Register("SeekCursor", typeof(Thickness), typeof(SeekBar), new FrameworkPropertyMetadata(null));



		static SeekBar() {

			DefaultStyleKeyProperty.OverrideMetadata(typeof(SeekBar), new FrameworkPropertyMetadata(typeof(SeekBar)));
		}

		public SeekBar() {


			MouseMove += SeekBar_MouseMove;
			MouseDown += SeekBar_MouseDown;
		}

		private void SeekBar_MouseDown(object sender, MouseButtonEventArgs e) {



		}

		private void SeekBar_MouseMove(object sender, MouseEventArgs e) {

			Console.WriteLine("MouseEnter:" + CurrentTime);

		}
	}
}
