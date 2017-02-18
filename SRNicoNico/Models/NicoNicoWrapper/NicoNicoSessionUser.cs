using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoSessionUser : NotificationObject {

        //このユーザーのセッション
        public NicoNicoSession Session { get; private set; }


        //ユーザーの名前
        #region Name変更通知プロパティ
        private string _Name;

        public string Name {
            get { return _Name; }
            set { 
                if(_Name == value)
                    return;
                _Name = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        //ユーザーID IDは他でも取得できるしこれを使うか微妙なんだよね
        #region UserId変更通知プロパティ
        private string _UserId;

        public string UserId {
            get { return _UserId; }
            set { 
                if(_UserId == value)
                    return;
                _UserId = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public static NicoNicoSessionUser CurrentUser { get; private set; }

        public NicoNicoSessionUser(NicoNicoSession session) {

            Session = session;
        }




    }
}
