using System;

using Livet;


namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoWrapperMain : NotificationObject {


		public static NicoNicoWrapperMain Instance;

		public NicoNicoSession Session { get; set; }

        public NicoNicoUser User { get; internal set; }

        //セッションが確立した後に呼ぶ
        public void PostInit() {

            User = new NicoNicoUser(Session.UserId);
			App.ViewModelRoot.Title += "(user:" + User.UserName + ")";
        }

        //現在のセッションを取得
        public static NicoNicoSession GetSession() {

			if(Instance == null) {

				throw new SystemException("NicoNicoWrapperMainインスタンスがnullになりました。");
			}

			return Instance.Session;
		}
	}
}
