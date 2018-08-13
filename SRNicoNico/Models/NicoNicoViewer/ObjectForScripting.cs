using System.Runtime.InteropServices;
using System.Security.Permissions;

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