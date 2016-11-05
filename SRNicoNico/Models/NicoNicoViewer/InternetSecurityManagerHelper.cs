using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

//codezine.jp 様より
namespace SRNicoNico.Models.NicoNicoViewer {
    public delegate int MapUrlToZoneEventHandler(String pwszUrl, out int pdwZone, int dwFlags);

    public delegate int ProcessUrlActionEventHandler(String pwszUrl, int dwAction, out byte pPolicy, int cbPolicy, byte pContext, int cbContext, int dwFlags, int dwReserved);

    /// <summary>
    /// Form1 の概要の説明です。
    /// </summary>
    public class InternetSecurityManagerHelper : WebBrowserAPI.IServiceProvider, WebBrowserAPI.IInternetSecurityManager {
        public event MapUrlToZoneEventHandler MapUrlToZone;
        public event ProcessUrlActionEventHandler ProcessUrlAction;

        public InternetSecurityManagerHelper() {
        }

        public void Attach(object activeXInstance) {
            // Microsoft Web Browser コントロールの ActiveX コントロール本体を取得
            //object ocx = axWebBrowser.GetOcx();

            // Microsoft Web Browser コントロールから IServiceProvider を取得
            WebBrowserAPI.IServiceProvider ocxServiceProvider = activeXInstance as WebBrowserAPI.IServiceProvider;

            // IServiceProvider.QueryService() を使って IProfferService を取得
            IntPtr profferServicePtr;
            ocxServiceProvider.QueryService(ref WebBrowserAPI.SID_SProfferService, ref WebBrowserAPI.IID_IProfferService, out profferServicePtr);
            WebBrowserAPI.IProfferService profferService = Marshal.GetObjectForIUnknown(profferServicePtr) as WebBrowserAPI.IProfferService;

            // IProfferService.ProfferService() を使って自分を IInternetSecurityManager として提供
            int cookie = 0;
            profferService.ProfferService(ref WebBrowserAPI.IID_IInternetSecurityManager, this, ref cookie);

        }

        #region IServiceProvider メンバ

        int WebBrowserAPI.IServiceProvider.QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject) {
            ppvObject = IntPtr.Zero;
            if(guidService == WebBrowserAPI.IID_IInternetSecurityManager) {
                // 自分から IID_IInternetSecurityManager を QueryInterface して返す
                IntPtr punk = Marshal.GetIUnknownForObject(this);
                return Marshal.QueryInterface(punk, ref riid, out ppvObject);
            }
            return HRESULT.E_NOINTERFACE;
        }

        #endregion

        #region IInternetSecurityManager メンバ

        int WebBrowserAPI.IInternetSecurityManager.SetSecuritySite(WebBrowserAPI.IInternetSecurityMgrSite pSite) {
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        int WebBrowserAPI.IInternetSecurityManager.GetSecuritySite(WebBrowserAPI.IInternetSecurityMgrSite pSite) {
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        int WebBrowserAPI.IInternetSecurityManager.MapUrlToZone(String pwszUrl, out int pdwZone, int dwFlags) {
            pdwZone = 0;
            if(this.MapUrlToZone != null) {
                return this.MapUrlToZone(pwszUrl, out pdwZone, dwFlags);
            }
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        int WebBrowserAPI.IInternetSecurityManager.GetSecurityId(string pwszUrl, byte[] pbSecurityId, ref uint pcbSecurityId, uint dwReserved) {
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        int WebBrowserAPI.IInternetSecurityManager.ProcessUrlAction(String pwszUrl, int dwAction, out byte pPolicy, int cbPolicy, byte pContext, int cbContext, int dwFlags, int dwReserved) {
            pPolicy = 0;
            if(this.ProcessUrlAction != null) {
                return this.ProcessUrlAction(pwszUrl, dwAction, out pPolicy, cbPolicy, pContext, cbContext, dwFlags, dwReserved);
            }
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        int WebBrowserAPI.IInternetSecurityManager.QueryCustomPolicy(String pwszUrl, ref Guid guidKey, byte ppPolicy, int pcbPolicy, byte pContext, int cbContext, int dwReserved) {
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        int WebBrowserAPI.IInternetSecurityManager.SetZoneMapping(int dwZone, String lpszPattern, int dwFlags) {
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        int WebBrowserAPI.IInternetSecurityManager.GetZoneMappings(int dwZone, out IEnumString ppenumString, int dwFlags) {
            ppenumString = null;
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        #endregion
    }
}
