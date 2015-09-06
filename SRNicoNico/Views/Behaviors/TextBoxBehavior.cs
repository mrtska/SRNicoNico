using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Controls;
using System.Windows.Input;

using Livet;

using SRNicoNico.ViewModels;

namespace SRNicoNico.Views.Behaviors {


	
	public class TextBoxBehavior : Behavior<TextBox> {



        public ViewModel Owner {
            get { return (ViewModel)GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Owner.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register("Owner", typeof(ViewModel), typeof(TextBoxBehavior), new PropertyMetadata(null));




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

                if(Owner is SearchViewModel) {

                    SearchViewModel vm = (SearchViewModel) Owner;

                    vm.SearchText = this.AssociatedObject.Text;
                    vm.DoSearch();
                }
				
			}
		}


	}
}
