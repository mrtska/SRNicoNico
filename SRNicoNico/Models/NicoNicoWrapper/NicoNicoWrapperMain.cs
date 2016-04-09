using System;

using Livet;


namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoWrapperMain : NotificationObject {


		public static NicoNicoWrapperMain Instance;

		private static NicoNicoSession _Session { get; set; }

        public static NicoNicoSession Session {

            get {

                return _Session;
            }
        }
        

        public NicoNicoWrapperMain(NicoNicoSession session) {

            _Session = session;
        }
	}
}
