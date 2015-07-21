using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Controls;
using System.Windows.Input;


namespace SRNicoNico.Views.Behaviors {


	
	public class TextBoxBehavior : Behavior<TextBox> {




		protected override void OnAttached() {
			base.OnAttached();

			this.AssociatedObject.KeyDown += TextBox_KeyDown;

		}

		protected override void OnDetaching() {
			base.OnDetaching();

			this.AssociatedObject.KeyDown -= TextBox_KeyDown;
		}

		//Enterキーで検索できるように
		private void TextBox_KeyDown(object sender, KeyEventArgs e) {

			if(e.Key == Key.Enter) {

				App.ViewModelRoot.Search.SearchText = this.AssociatedObject.Text;
				App.ViewModelRoot.Search.DoSearch();
			}
		}


	}
}
