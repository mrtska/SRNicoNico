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


		public static NicoNicoWrapperMain instance;


		public NicoNicoSession session { get; set; }

		public NicoNicoUser user { get; internal set; }

		//セッションが確立した後に呼ぶ
		public void init() {

			this.user = new NicoNicoUser(session.UserId);
			App.ViewModelRoot.Title += "(user:" + this.user.UserName + ")";
		}
			
		//現在のセッションを取得
		public static NicoNicoSession getSession() {

			if(instance == null) {

				throw new SystemException("NicoNicoWrapperMainインスタンスがnullになりました。");
			}

			return instance.session;
		}


			

	}
}
