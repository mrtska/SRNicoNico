using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoViewer {

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]
    public class ObjectForScripting {

        private IObjectForScriptable Scriptable;

        public ObjectForScripting(IObjectForScriptable scriptable) {

            Scriptable = scriptable;
        }

        public void InvokeFromJavaScript(string func, string args) {

            Scriptable.Invoked(func, args);
        }
    }
}
