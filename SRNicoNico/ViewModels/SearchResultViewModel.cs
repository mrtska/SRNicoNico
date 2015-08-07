using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
	public class SearchResultViewModel : ViewModel {
	

		//検索結果の総数
		#region Total変更通知プロパティ
		private string _Total;

		public string Total {
			get { return _Total; }
			set { 
				if(_Total == value)
					return;
				_Total = value;
				RaisePropertyChanged();
			}
		}
		#endregion
		
		
		#region searchResult変更通知プロパティ
		private ObservableCollection<SearchResultEntryViewModel> _List = new ObservableCollection<SearchResultEntryViewModel>();

		public ObservableCollection<SearchResultEntryViewModel> List {
			get { return _List; }
			set { 
				if(_List == value)
					return;
				_List = value;
				RaisePropertyChanged();
				
			}
		}
		#endregion



	}
}
