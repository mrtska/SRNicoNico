using System;

using Livet;


namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoWrapperMain : NotificationObject {


		public static NicoNicoWrapperMain Instance;

		private NicoNicoSession _Session { get; set; }

        public NicoNicoSession Session {

            get {

                return _Session;
            }
        }

        //現在のセッションを取得
        public static NicoNicoSession GetSession() {

			if(Instance == null) {

				throw new SystemException("NicoNicoWrapperMainインスタンスがnullになりました。");
			}

			return Instance.Session;
		}

        public NicoNicoWrapperMain(NicoNicoSession session) {

            _Session = session;
        }
	}
}
