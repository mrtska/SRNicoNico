using System;
using System.Collections;
using System.Collections.ObjectModel;
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

using SRNicoNico.ViewModels;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Models.NicoNicoViewer;

using Livet;
using System.Collections.Generic;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.ComponentModel;

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
	///     <MyNamespace:CommentView/>
	///
	/// </summary>
	public class CommentView : Control {

		static CommentView() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CommentView), new FrameworkPropertyMetadata(typeof(CommentView)));
		}
        
		public Grid DrawingGrid {
			get { return (Grid)GetValue(DrawingGridProperty); }
			set { SetValue(DrawingGridProperty, value); }
		}

		// Using a DependencyProperty as the backing store for DrawingGrid.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DrawingGridProperty =
			DependencyProperty.Register("DrawingGrid", typeof(Grid), typeof(CommentView), new FrameworkPropertyMetadata(null));



        

        //描画中のコメント コメントID:TextBlock
        private Dictionary<int, CommentEntry> DrawingComment = new Dictionary<int, CommentEntry>();
        


       



        private CommentRasterizer Builder;


        public CommentView() {


            DataContextChanged += CommentView_DataContextChanged;
            SizeChanged += CommentView_SizeChanged;


        }

        private void CommentView_SizeChanged(object sender, SizeChangedEventArgs e) {

            if(Builder != null) {

                Builder.UpdateSize(ActualWidth, ActualHeight);
            }
        }

        //データコンテキスト(ViewModel)にViewのインスタンスを入れる
        private void CommentView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {


            //ViewModelからViewのメソッドを呼べるようにする これが一番楽だった
            if(DataContext is VideoViewModel) {

                VideoViewModel video = (VideoViewModel) DataContext;
                video.Comment.View = this;
            }
        }

        //ViewModelから呼ばれる
        public void Initialize(ObservableCollection<CommentEntryViewModel> list) {

            Builder = new CommentRasterizer(this, list, DrawingComment);
        }

        //1フレーム置きに呼ばれる
        public void RenderComment(double vpos) {

            //これをしとかないとデザイナがエラー吐くからね しかたないね
#if DEBUG
            if((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)) {
                return;
            }
#endif
            

            //DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => Builder.RenderComment(vpos)));
            if(Builder != null) {

                Builder.RenderComment(vpos);
            }


        }

        //引数に応じた位置、色、時間でコメントを描画する

        //no:コメントナンバー
        //content:コメント
        //color:コメントカラー
        //size:コメントサイズ
        //length:横幅 複数行コメントの時は一番長い行の幅
        //pos:位置
        //dur:表示時間
        public void DrawComment(CommentEntry entry) {

            TranslateTransform trans = new TranslateTransform(entry.Pos.XPos, entry.Pos.YPos);
            DoubleAnimation anim = null;

            
            entry.Text.RenderTransform = trans;

            //アニメーション 最終位置は画面外左
            if(entry.Pos.EnumPos == EnumCommentPosition.Naka) {

                anim = new DoubleAnimation(-entry.Text.Width, entry.Raw.Duration);
            } else {

                anim = new DoubleAnimation(entry.Pos.XPos + 1, entry.Raw.Duration);
            }
                


           // }

            //名前を設定
            var translationName = "myTranslation" + trans.GetHashCode();
            RegisterName(translationName, trans);


            //アニメーションを設定


           
            //一時停止とかのやつ thanks Stackoverflow (http://stackoverflow.com/questions/2841124/wpf-animating-translatetransform-from-code)
            Storyboard story = new Storyboard() { RepeatBehavior = new RepeatBehavior(1) };
            Storyboard.SetTargetName(story, translationName);
            Storyboard.SetTargetProperty(story, new PropertyPath(TranslateTransform.XProperty));

            story.FillBehavior = FillBehavior.Stop;
            var storyboardName = "s" + story.GetHashCode();
            Resources.Add(storyboardName, story);

            
            story.Children.Add(anim);

            //描画が終わったらリソース開放
            story.Completed +=
            (sndr, evtArgs) => {
                Resources.Remove(storyboardName);
                UnregisterName(translationName);
                Anim_Completed(sndr, entry.Text.Name);
            };
            //アニメーション開始
            story.Begin();
            entry.Story = story;
            


            //現在描画中のリストに追加
            //if(!DrawingComment.Keys.Contains(entry.Raw.No)) {

                DrawingComment.Add(entry.Raw.No, entry);
            // }



            //Gridに配置
            DrawingGrid.Children.Add(entry.Text);


        }

        public void Pause() {

            foreach(KeyValuePair<int, CommentEntry> pair in DrawingComment) {

                pair.Value.Story.Pause();
            }
        }

        public void Resume() {

            foreach(KeyValuePair<int, CommentEntry> pair in DrawingComment) {

                pair.Value.Story.Resume();
            }

        }

        public void Seek() {

            foreach(KeyValuePair<int, CommentEntry> pair in DrawingComment) {

                

            }
            Builder.ForceReset = true;
            DrawingComment.Clear();

            for(int i = 1; i < DrawingGrid.Children.Count; i++) {

                DrawingGrid.Children.RemoveAt(i);
            }

            
        }




        //アニメーションが終わったコメント（画面外に出た）
        private void Anim_Completed(object sender, string name) {

            //無いはずだがまあ、ね
            if(name == null) {

                throw new SystemException("Name はnullになりました");
            }

            //キーを取得
            int no = int.Parse(name.Substring(2));


            //役目を終えたコメント リソース開放
            if(DrawingComment.ContainsKey(no)) {

                CommentEntry entry = DrawingComment[no];
                TextBlock text = entry.Text;

                DrawingComment.Remove(no);
                DrawingGrid.Children.Remove(text);
                entry.Story.Children.Remove(entry.Anime);
                
                text.Visibility = Visibility.Collapsed;
                Console.WriteLine("Free:" + entry);
            }
        }
    }



}
