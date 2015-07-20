using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

namespace SRNicoNico.Models.NicoNicoWrapper {
	public class NicoNicoWrapperMain : NotificationObject {
		/*
		 * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
		 */


		public static NicoNicoWrapperMain Instance;


		public NicoNicoSession Session { get; set; }

		public NicoNicoUser User { get; internal set; }

		//セッションが確立した後に呼ぶ
		public void init() {

			this.User = new NicoNicoUser(Session.UserId);
			App.ViewModelRoot.Title += "(user:" + this.User.UserName + ")";
		}
			
		//現在のセッションを取得
		public static NicoNicoSession getSession() {

			if(Instance == null) {

				throw new SystemException("NicoNicoWrapperMainインスタンスがnullになりました。");
			}

			return Instance.Session;
		}


			

	}
}
