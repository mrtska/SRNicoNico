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

using Vlc.DotNet.Core.Interops;
using Vlc.DotNet;
using Vlc.DotNet.Wpf;
using Vlc.DotNet.Core;
namespace SRNicoNico.Views.Behaviors {

	public class VlcBehavior : Behavior<VlcControl> {


		protected override void OnAttached() {
			base.OnAttached();

			Vlc.DotNet.Forms.VlcControl MediaPlayer = this.AssociatedObject.MediaPlayer;

			this.AssociatedObject.Loaded += Loaded;
			MediaPlayer.VlcLibDirectoryNeeded += OnVlcControlNeedsLibDirectory;
			MediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
			MediaPlayer.Buffering += MediaPlayer_Buffering;
			MediaPlayer.EndReached += MediaPlayer_EndReached;
			MediaPlayer.Stopped += MediaPlayer_Stopped;

			

			
		}

	
		protected override void OnDetaching() {
			base.OnDetaching();

			Vlc.DotNet.Forms.VlcControl MediaPlayer = this.AssociatedObject.MediaPlayer;

			this.AssociatedObject.Loaded -= Loaded;
			MediaPlayer.EncounteredError -= MediaPlayer_EncounteredError;
			MediaPlayer.VlcLibDirectoryNeeded -= OnVlcControlNeedsLibDirectory;
			MediaPlayer.Buffering -= MediaPlayer_Buffering;
			MediaPlayer.Stopped -= MediaPlayer_Stopped;
			MediaPlayer.EndReached -= MediaPlayer_EndReached;
			
			
		
		}


		private void MediaPlayer_EndReached(object sender, VlcMediaPlayerEndReachedEventArgs e) {


			;
		}


		private void MediaPlayer_EncounteredError(object sender, VlcMediaPlayerEncounteredErrorEventArgs e) {
			
			
			;


		}

		private void MediaPlayer_Stopped(object sender, VlcMediaPlayerStoppedEventArgs e) {


			;
		}

		private void MediaPlayer_Buffering(object sender, VlcMediaPlayerBufferingEventArgs e) {


			Console.WriteLine("NewCache:" + e.NewCache);

		}


		private void Loaded(object sender, EventArgs e) {

			App.ViewModelRoot.video.control = this.AssociatedObject;
			Vlc.DotNet.Forms.VlcControl MediaPlayer = this.AssociatedObject.MediaPlayer;

			Thread.Sleep(1000);

			MediaPlayer.Play(new FileInfo(App.ViewModelRoot.video.path));
			
		}



		private void OnVlcControlNeedsLibDirectory(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e) {


			var currentAssembly = Assembly.GetEntryAssembly();
			var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
			e.VlcLibDirectory = new DirectoryInfo(currentDirectory + @"\lib");
		}


	}
}
