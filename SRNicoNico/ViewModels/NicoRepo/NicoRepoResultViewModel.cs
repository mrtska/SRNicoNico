using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class NicoRepoResultViewModel : TabItemViewModel {

        public NicoNicoNicoRepo Model { get; set; }



        #region Filter変更通知プロパティ
        private string _Filter;

        public string Filter {
            get { return _Filter; }
            set { 
                if(_Filter == value)
                    return;
                _Filter = value;
                Model.SetFilter(value);

                //OnewayToSourceなのでRaisePropertyChangedは不要
            }
        }
        #endregion

        private readonly NicoRepoViewModel Owner;

        public NicoRepoResultViewModel(NicoRepoViewModel owner, string title, string api) : base(title) {

            Owner = owner;
            Model = new NicoNicoNicoRepo(api);
        }


        public void Initialize() {

            GetMore();
        }

        public async void GetMore() {

            if(IsActive) {

                return;
            }

            IsActive = true;

            Owner.Status = "ニコレポ取得中：" + Name;
            Owner.Status = await Model.GetNicoRepoAsync();
            IsActive = false;
        }

        public void Refresh() {

            Model.Reset();
            Initialize();
        }

        public override void KeyDown(KeyEventArgs e) {
        
            if(e.Key == Key.F5) {

                Refresh();
            }
            if(e.Key == Key.Space) {

                GetMore();
            }
        }
    }
}
