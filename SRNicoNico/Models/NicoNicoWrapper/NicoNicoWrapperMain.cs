using System;

using Livet;


namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoWrapperMain : NotificationObject {


		public static NicoNicoWrapperMain Instance;

		public NicoNicoSession Session { get; set; }


        //現在のセッションを取得
        public static NicoNicoSession GetSession() {

			if(Instance == null) {

				throw new SystemException("NicoNicoWrapperMainインスタンスがnullになりました。");
			}

			return Instance.Session;
		}
	}
}
