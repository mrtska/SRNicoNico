using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using FirstFloor.ModernUI.Windows.Controls;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class NicoRepoResultEntryViewModel : ViewModel {

        //エントリ
        public NicoNicoNicoRepoDataEntry Entry { get; private set; }

        public NicoRepoListViewModel Owner;

        //自分のニコレポには削除ボタンを表示する
        public bool ShowDeleteButton { get; set; }

        //削除ボタンを利用するならこっち
        public NicoRepoResultEntryViewModel(NicoNicoNicoRepoDataEntry entry, NicoRepoListViewModel owner) {

            Owner = owner;
            Entry = entry;
            if(entry.IsMyNicoRepo) {

                ShowDeleteButton = true;
            }
        }

        public NicoRepoResultEntryViewModel(NicoNicoNicoRepoDataEntry entry) {

            Entry = entry;
        }


        public void OpenHyperLink(string uri) {


            if(uri.Contains("user")) {


                Task.Run(() => {

                    new UserViewModel(uri);
                });
            }



        }

        //ニコレポ削除
        public void DeleteNicoRepo() {

            if(ShowDeleteButton) {

                ShowDeleteButton = false;
                
                App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.Contents.NicoRepo.NicoRepoDeleteDialog), this, TransitionMode.Modal));
            }
        }

        //ニコレポ削除処理
        public void Delete() {

            Task.Run(() => {

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://www.nicovideo.jp/api/nicorepo/delete_log");

                Dictionary<string, string> form = new Dictionary<string, string>();
                form["log_id"] = Entry.LogId;
                form["type"] = Entry.Type;
                form["time"] = Entry.DeleteTime;
                form["token"] = Entry.Token;

                request.Content = new FormUrlEncodedContent(form);

                var response = NicoNicoWrapperMain.GetSession().GetAsync(request).Result;

                Close();


                Owner?.Reflesh();
            });
        }

        //ダイアログクローズ
        public void Close() {

            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "WindowAction"));
        }
    }
}
