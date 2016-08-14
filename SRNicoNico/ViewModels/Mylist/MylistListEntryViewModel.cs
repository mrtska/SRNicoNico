using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows;

namespace SRNicoNico.ViewModels {
    public class MylistListEntryViewModel : ViewModel {

        //エントリ
        public NicoNicoMylistData Entry { get; set; }

        //リスト
        public MylistListViewModel Owner { get; set; }

        //編集モード時
        #region IsChecked変更通知プロパティ
        private bool _IsChecked;

        public bool IsChecked {
            get { return _IsChecked; }
            set {
                if(_IsChecked == value)
                    return;
                _IsChecked = value;
                Owner.EditModeViewModel.IsAnyoneChecked = value;

                int count = 0;
                foreach(MylistListEntryViewModel entry in Owner.Mylist) {

                    if(entry.IsChecked) {

                        count++;
                    }
                }
                if(count != 0) {

                    Owner.EditModeViewModel.Status = count + "個選択中";
                } else {

                    Owner.EditModeViewModel.Status = "";
                }
                RaisePropertyChanged();
            }
        }
        #endregion

        public MylistListEntryViewModel(MylistListViewModel vm, NicoNicoMylistData data) {

            Owner = vm;
            Entry = data;
        }

        public MylistListEntryViewModel(NicoNicoMylistData data) {

            Entry = data;
        }


        public void OpenWebView() {

            if(Entry.Type == 0) {

                App.ViewModelRoot.AddWebViewTab("http://www.nicovideo.jp/watch/" + Entry.Id, true);
            } else if(Entry.Type == 5) {

                App.ViewModelRoot.AddWebViewTab("http://seiga.nicovideo.jp/watch/mg" + Entry.Id, true);
            } else if(Entry.Type == 6) {

                App.ViewModelRoot.AddWebViewTab("http://seiga.nicovideo.jp/watch/bk" + Entry.Id, true);
            } else if(Entry.Type == 11) {

                //System.Diagnostics.Process.Start("http://ch.nicovideo.jp/" + SelectedItem.Entry);
            }
        }

        public void OpenBrowser() {


            if(Entry.Type == 0) {

                System.Diagnostics.Process.Start("http://www.nicovideo.jp/watch/" + Entry.Id);
            } else if(Entry.Type == 5) {

                System.Diagnostics.Process.Start("http://seiga.nicovideo.jp/watch/mg" + Entry.Id);
            } else if(Entry.Type == 6) {

                System.Diagnostics.Process.Start("http://seiga.nicovideo.jp/watch/bk" + Entry.Id);
            } else if(Entry.Type == 11) {

                //System.Diagnostics.Process.Start("http://ch.nicovideo.jp/" + SelectedItem.Entry);
            }
        }

        public void CopyUrl() {


            if(Entry.Type == 0) {

                Clipboard.SetText("http://www.nicovideo.jp/watch/" + Entry.Id);
            } else if(Entry.Type == 5) {

                Clipboard.SetText("http://seiga.nicovideo.jp/watch/mg" + Entry.Id);
            } else if(Entry.Type == 6) {

                Clipboard.SetText("http://seiga.nicovideo.jp/watch/bk" + Entry.Id);
            } else if(Entry.Type == 11) {

                //System.Diagnostics.Process.Start("http://ch.nicovideo.jp/" + SelectedItem.Entry);
            }
        }

    }
}
