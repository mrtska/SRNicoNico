using System;
using System.Windows.Interactivity;

using xZune.Vlc.Wpf;

namespace SRNicoNico.Views.Behaviors {

	//VlcMediaPlayerを取得するBehavior
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

			App.ViewModelRoot.Video.Player = this.AssociatedObject;
			App.ViewModelRoot.Video.Initialize();
		}
	}
}
