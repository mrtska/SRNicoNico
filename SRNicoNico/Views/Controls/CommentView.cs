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

using Livet;
using System.Collections.Generic;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;

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


		public DispatcherCollection<CommentEntryViewModel> CommentList {
			get { return (DispatcherCollection<CommentEntryViewModel>)GetValue(CommentListProperty); }
			set { SetValue(CommentListProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CommentList.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CommentListProperty =
			DependencyProperty.Register("CommentList", typeof(DispatcherCollection<CommentEntryViewModel>), typeof(CommentView), new FrameworkPropertyMetadata(null));



		public int CurrentTime {
			get { return (int)GetValue(CurrentTimeProperty); }
			set { SetValue(CurrentTimeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CurrentTimeProperty =
			DependencyProperty.Register("CurrentTime", typeof(int), typeof(CommentView), new FrameworkPropertyMetadata(0));



		public Grid DrawingGrid {
			get { return (Grid)GetValue(DrawingGridProperty); }
			set { SetValue(DrawingGridProperty, value); }
		}

		// Using a DependencyProperty as the backing store for DrawingGrid.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DrawingGridProperty =
			DependencyProperty.Register("DrawingGrid", typeof(Grid), typeof(CommentView), new FrameworkPropertyMetadata(null));




        public bool IsPlaying {
            get { return (bool)GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPause.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(CommentView), new FrameworkPropertyMetadata(false));




        //描画中のコメント コメントID:TextBlock
        private Dictionary<int, Entry> DrawingComment = new Dictionary<int, Entry>();

        //描画出来るコメントの段を管理 
        private Dictionary<int, int> CommentRow = new Dictionary<int, int>();

        private int PrevTime = 0;


        //影をつける
        private  DropShadowEffect effect = new DropShadowEffect();
       
        //4秒かけてアニメーションする
        private Duration duration = new Duration(TimeSpan.FromSeconds(4));

        public CommentView() {

            CompositionTarget.Rendering += CompositionTarget_Rendering;

            //テキストエフェクト いろいろ
            effect.ShadowDepth = 3;
            effect.Direction = 315;
            effect.Color = Colors.BlueViolet;
            effect.Opacity = 0.5;

        }
        

        //1フレーム置きに呼ばれる
        private void CompositionTarget_Rendering(object sender, EventArgs e) {


            //条件に合わなかったらコメントを描画しない
            if(CommentList == null || CommentList.Count == 0 || PrevTime == CurrentTime) {
                
                //一時停止中は上のif文の三つ目の評価がtrueになるのでここに書く
                if(!IsPlaying) {

                    //一番最初に一回呼ばれるが、DrawingCommentの要素数が0なので問題なし
                    foreach(KeyValuePair<int, Entry> pair in DrawingComment) {

                        pair.Value.Story.Pause();
                    }
                } else {

                    foreach(KeyValuePair<int, Entry> pair in DrawingComment) {

                        pair.Value.Story.Resume();
                    }
                }
              
                
                return;
			}
            
            //2回実行されてもCurrentTimeが変わらない時があるので変わらなかったときは上記のとおり新しく描画しない
            PrevTime = CurrentTime;

    
            //Console.WriteLine("Time:" + CurrentTime);

            //コメントリストから持ってくる
            foreach(CommentEntryViewModel vmentry in CommentList) {


				NicoNicoCommentEntry entry = vmentry.Entry;

                //CommentListは再生位置でソートされているので、まだ描画時間になっていないコメントが出てきたらその後は全部無視
                if(entry.Vpos > CurrentTime) {

                    return;
                }

                //コメント描画時間になった
                if(entry.Vpos <= CurrentTime && entry.Vpos + 400 >= CurrentTime && !DrawingComment.ContainsKey(entry.No)) {

                    TextBlock text = new TextBlock();

                    text.Name = "No" + entry.No.ToString();
                    text.Text = entry.Content;
                    text.FontSize = 20;
                    text.Foreground = new SolidColorBrush(Colors.White);

                    text.Effect = effect;

                    
                    //アニメーションする ActualWidthは初期値 つまり右端
                    TranslateTransform trans = new TranslateTransform(ActualWidth, 0);
                    var translationName = "myTranslation" + trans.GetHashCode();
                    RegisterName(translationName, trans);

                    //アニメーションを設定
                    text.RenderTransform = trans;

                    //アニメーション 最終位置は画面外左
                    DoubleAnimation anim = new DoubleAnimation(-(text.Text.Length * text.FontSize), duration);
                    anim.Name = text.Name;

                    //一時停止とかのやつ thanks Stackoverflow (http://stackoverflow.com/questions/2841124/wpf-animating-translatetransform-from-code)
                    Storyboard story = new Storyboard();

                    Storyboard.SetTargetName(story, translationName);
                    Storyboard.SetTargetProperty(story, new PropertyPath(TranslateTransform.XProperty));

                    var storyboardName = "s" + story.GetHashCode();
                    Resources.Add(storyboardName, story);


                    story.Children.Add(anim);

                    //描画が終わったらリソース開放
                    story.Completed +=
                    (sndr, evtArgs) => {
                        Resources.Remove(storyboardName);
                        UnregisterName(translationName);
                        Anim_Completed(sndr, anim.Name);
                    };

                    //アニメーション開始
                    story.Begin();

                    //Gridに配置
                    DrawingGrid.Children.Add(text);
                        
                    //現在描画中のリストに追加
                    DrawingComment.Add(entry.No, new Entry() { Text = text, Anime = anim, Story = story} );


                    
                    Console.WriteLine("Allocate:" + text.Text);
                }
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

                Entry entry = DrawingComment[no];
                TextBlock text = entry.Text;
                DrawingComment.Remove(no);
                DrawingGrid.Children.Remove(text);
                entry.Story.Children.Remove(entry.Anime);
                text.Visibility = Visibility.Collapsed;
                //Console.WriteLine("Free:" + text.Text);
            }
        }
    }

    //便利な奴
    internal class Entry {

        public Storyboard Story { get; set; }
        public TextBlock Text { get; set; }
        public DoubleAnimation Anime { get; set; }
    }
}
