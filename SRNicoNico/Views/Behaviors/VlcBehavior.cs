using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Interactivity;
using System.Reflection;

using xZune.Vlc.Wpf;
using xZune.Vlc;

using Livet;

namespace SRNicoNico.Views.Behaviors {

	public class VlcBehavior : Behavior<VlcPlayer> {

		private VlcMediaPlayer Player;
		

		protected override void OnAttached() {
			base.OnAttached();

			this.AssociatedObject.Loaded += Loaded;

			
		}


		protected override void OnDetaching() {
			base.OnDetaching();

			this.AssociatedObject.Loaded -= Loaded;
			this.Player.Buffering -= Player_Buffering;
			this.Player.EndReached -= Player_EndReached;
		}

		void Player_Buffering(object sender, MediaPlayerBufferingEventArgs e) {


			Console.WriteLine("NewCache:" + e.NewCache);
		}


		void Player_EndReached(object sender, EventArgs e) {

			
			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {


				this.AssociatedObject.Play();

			}));
		}





		private void Loaded(object sender, EventArgs e) {

			App.ViewModelRoot.Video.Player = this.AssociatedObject;


			this.Player = this.AssociatedObject.VlcMediaPlayer;
			this.Player.Buffering += Player_Buffering;
			this.Player.EndReached += Player_EndReached;

			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {


				this.AssociatedObject.LoadMedia(App.ViewModelRoot.Video.Path);
				
				Thread.Sleep(1000);
				this.AssociatedObject.Play();
				
			}));

		

		}


	}
}
