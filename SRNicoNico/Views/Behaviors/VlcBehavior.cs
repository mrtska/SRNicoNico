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
using Livet;

namespace SRNicoNico.Views.Behaviors {

	public class VlcBehavior : Behavior<VlcPlayer> {


		protected override void OnAttached() {
			base.OnAttached();

			this.AssociatedObject.Loaded += Loaded;

			
		}

	
		protected override void OnDetaching() {
			base.OnDetaching();



			this.AssociatedObject.Loaded -= Loaded;
		}


		private void Loaded(object sender, EventArgs e) {

			App.ViewModelRoot.Video.player = this.AssociatedObject;

			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {


				this.AssociatedObject.LoadMedia(App.ViewModelRoot.Video.path);
				Thread.Sleep(1000);
				this.AssociatedObject.Play();
				
			}));

		

		}



	}
}
