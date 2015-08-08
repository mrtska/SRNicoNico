using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;

using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace SRNicoNico.Views.Extentions {
	public static class ButtonExtentions {

		public static void PerformClick(this Button button) {


			if(button == null)
				throw new ArgumentNullException("button");

			var provider = new ButtonAutomationPeer(button) as IInvokeProvider;
			provider.Invoke();
		}
	}
}
