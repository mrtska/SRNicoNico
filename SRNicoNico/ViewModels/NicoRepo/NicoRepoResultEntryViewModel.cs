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
using SRNicoNico.Models.NicoNicoViewer;
using System.Windows;

namespace SRNicoNico.ViewModels {
    public class NicoRepoResultEntryViewModel : ViewModel {

        //エントリ
        public NicoNicoNicoRepoDataEntry Entry { get; private set; }

        public NicoRepoListViewModel Owner;

        //自分のニコレポには削除ボタンを表示する
        public bool ShowDeleteButton { get; set; }



        #region NicoRepoType変更通知プロパティ
        private NicoNicoUrlType _NicoRepoType;

        public NicoNicoUrlType NicoRepoType {
            get { return _NicoRepoType; }
            set { 
                if(_NicoRepoType == value)
                    return;
                _NicoRepoType = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region ToolTip変更通知プロパティ
        private ViewModel _ToolTip;

        public ViewModel ToolTip {
            get { return _ToolTip; }
            set { 
                if(_ToolTip == value)
                    return;
                _ToolTip = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region ToolTipBisibility変更通知プロパティ
        private Visibility _ToolTipVisibility = Visibility.Hidden;

        public Visibility ToolTipVisibility {
            get { return _ToolTipVisibility; }
            set { 
                if(_ToolTipVisibility == value)
                    return;
                _ToolTipVisibility = value;
                RaisePropertyChanged();
            }
        }
        #endregion


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

        public void ShowInfomation() {

            if(ToolTip != null || Entry.VideoUrl == null) {

                return;
            }

            NicoRepoType = NicoNicoOpener.GetType(Entry.VideoUrl);

            if(NicoRepoType == NicoNicoUrlType.Video) {

                ToolTip = new VideoDataViewModel(Entry.VideoUrl.Substring(30));
                ToolTipVisibility = Visibility.Visible;
            }
            
        }

    }
}
