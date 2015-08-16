using System;

using Livet;

using SRNicoNico.Models.NicoNicoViewer;


namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoWrapperMain : NotificationObject {
		/*
		 * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
		 */


		public static NicoNicoWrapperMain Instance;


		public NicoNicoSession Session { get; set; }

        public NicoNicoUser User { get; internal set; }

        //セッションが確立した後に呼ぶ
        public void PostInit() {

            
			this.User = new NicoNicoUser(Session.UserId);
			App.ViewModelRoot.Title += "(user:" + this.User.UserName + ")";


            //通信速度監視
            BPSCounter.InitAndStart();

            App.ViewModelRoot.NicoRepo.InitNicoRepo();
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
