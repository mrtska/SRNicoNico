using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;

//http://codezine.jp/ 様より
namespace SRNicoNico.Models.NicoNicoViewer {
    public class HRESULT {
        public const int S_OK = unchecked((int)0x00000000);
        public const int S_FALSE = unchecked((int)0x00000001);
        public const int E_NOTIMPL = unchecked((int)0x80004001);
        public const int E_NOINTERFACE = unchecked((int)0x80004002);
    }

    public class WebBrowserAPI {
        public const int INET_E_DEFAULT_ACTION = unchecked((int)0x800C0011);

        public enum URLZONE {
            URLZONE_LOCAL_MACHINE = 0,
            URLZONE_INTRANET = URLZONE_LOCAL_MACHINE + 1,
            URLZONE_TRUSTED = URLZONE_INTRANET + 1,
            URLZONE_INTERNET = URLZONE_TRUSTED + 1,
            URLZONE_UNTRUSTED = URLZONE_INTERNET + 1,
        }

        public const int URLACTION_MIN = unchecked((int)0x00001000);

        public const int URLACTION_DOWNLOAD_MIN = unchecked((int)0x00001000);
        public const int URLACTION_DOWNLOAD_SIGNED_ACTIVEX = unchecked((int)0x00001001);
        public const int URLACTION_DOWNLOAD_UNSIGNED_ACTIVEX = unchecked((int)0x00001004);
        public const int URLACTION_DOWNLOAD_CURR_MAX = unchecked((int)0x00001004);
        public const int URLACTION_DOWNLOAD_MAX = unchecked((int)0x000011FF);

        public const int URLACTION_ACTIVEX_MIN = unchecked((int)0x00001200);
        public const int URLACTION_ACTIVEX_RUN = unchecked((int)0x00001200);
        public const int URLPOLICY_ACTIVEX_CHECK_LIST = unchecked((int)0x00010000);
        public const int URLACTION_ACTIVEX_OVERRIDE_OBJECT_SAFETY = unchecked((int)0x00001201);
        public const int URLACTION_ACTIVEX_OVERRIDE_DATA_SAFETY = unchecked((int)0x00001202);
        public const int URLACTION_ACTIVEX_OVERRIDE_SCRIPT_SAFETY = unchecked((int)0x00001203);
        public const int URLACTION_SCRIPT_OVERRIDE_SAFETY = unchecked((int)0x00001401);
        public const int URLACTION_ACTIVEX_CONFIRM_NOOBJECTSAFETY = unchecked((int)0x00001204);
        public const int URLACTION_ACTIVEX_TREATASUNTRUSTED = unchecked((int)0x00001205);
        public const int URLACTION_ACTIVEX_NO_WEBOC_SCRIPT = unchecked((int)0x00001206);
        public const int URLACTION_ACTIVEX_CURR_MAX = unchecked((int)0x00001206);
        public const int URLACTION_ACTIVEX_MAX = unchecked((int)0x000013ff);

        public const int URLACTION_SCRIPT_MIN = unchecked((int)0x00001400);
        public const int URLACTION_SCRIPT_RUN = unchecked((int)0x00001400);
        public const int URLACTION_SCRIPT_JAVA_USE = unchecked((int)0x00001402);
        public const int URLACTION_SCRIPT_SAFE_ACTIVEX = unchecked((int)0x00001405);
        public const int URLACTION_CROSS_DOMAIN_DATA = unchecked((int)0x00001406);
        public const int URLACTION_SCRIPT_PASTE = unchecked((int)0x00001407);
        public const int URLACTION_SCRIPT_CURR_MAX = unchecked((int)0x00001407);
        public const int URLACTION_SCRIPT_MAX = unchecked((int)0x000015ff);

        public const int URLACTION_HTML_MIN = unchecked((int)0x00001600);
        public const int URLACTION_HTML_SUBMIT_FORMS = unchecked((int)0x00001601); // aggregate next two
        public const int URLACTION_HTML_SUBMIT_FORMS_FROM = unchecked((int)0x00001602); //
        public const int URLACTION_HTML_SUBMIT_FORMS_TO = unchecked((int)0x00001603); //
        public const int URLACTION_HTML_FONT_DOWNLOAD = unchecked((int)0x00001604);
        public const int URLACTION_HTML_JAVA_RUN = unchecked((int)0x00001605); // derive from Java custom policy
        public const int URLACTION_HTML_USERDATA_SAVE = unchecked((int)0x00001606);
        public const int URLACTION_HTML_SUBFRAME_NAVIGATE = unchecked((int)0x00001607);
        public const int URLACTION_HTML_META_REFRESH = unchecked((int)0x00001608);
        public const int URLACTION_HTML_MIXED_CONTENT = unchecked((int)0x00001609);
        public const int URLACTION_HTML_MAX = unchecked((int)0x000017ff);

        public const int URLACTION_SHELL_MIN = unchecked((int)0x00001800);
        public const int URLACTION_SHELL_INSTALL_DTITEMS = unchecked((int)0x00001800);
        public const int URLACTION_SHELL_MOVE_OR_COPY = unchecked((int)0x00001802);
        public const int URLACTION_SHELL_FILE_DOWNLOAD = unchecked((int)0x00001803);
        public const int URLACTION_SHELL_VERB = unchecked((int)0x00001804);
        public const int URLACTION_SHELL_WEBVIEW_VERB = unchecked((int)0x00001805);
        public const int URLACTION_SHELL_SHELLEXECUTE = unchecked((int)0x00001806);
        public const int URLACTION_SHELL_CURR_MAX = unchecked((int)0x00001806);
        public const int URLACTION_SHELL_MAX = unchecked((int)0x000019ff);

        public const int URLACTION_NETWORK_MIN = unchecked((int)0x00001A00);

        public const int URLACTION_CREDENTIALS_USE = unchecked((int)0x00001A00);
        public const int URLPOLICY_CREDENTIALS_SILENT_LOGON_OK = unchecked((int)0x00000000);
        public const int URLPOLICY_CREDENTIALS_MUST_PROMPT_USER = unchecked((int)0x00010000);
        public const int URLPOLICY_CREDENTIALS_CONDITIONAL_PROMPT = unchecked((int)0x00020000);
        public const int URLPOLICY_CREDENTIALS_ANONYMOUS_ONLY = unchecked((int)0x00030000);

        public const int URLACTION_AUTHENTICATE_CLIENT = unchecked((int)0x00001A01);
        public const int URLPOLICY_AUTHENTICATE_CLEARTEXT_OK = unchecked((int)0x00000000);
        public const int URLPOLICY_AUTHENTICATE_CHALLENGE_RESPONSE = unchecked((int)0x00010000);
        public const int URLPOLICY_AUTHENTICATE_MUTUAL_ONLY = unchecked((int)0x00030000);


        public const int URLACTION_COOKIES = unchecked((int)0x00001A02);
        public const int URLACTION_COOKIES_SESSION = unchecked((int)0x00001A03);

        public const int URLACTION_CLIENT_CERT_PROMPT = unchecked((int)0x00001A04);

        public const int URLACTION_COOKIES_THIRD_PARTY = unchecked((int)0x00001A05);
        public const int URLACTION_COOKIES_SESSION_THIRD_PARTY = unchecked((int)0x00001A06);

        public const int URLACTION_COOKIES_ENABLED = unchecked((int)0x00001A10);

        public const int URLACTION_NETWORK_CURR_MAX = unchecked((int)0x00001A10);
        public const int URLACTION_NETWORK_MAX = unchecked((int)0x00001Bff);


        public const int URLACTION_JAVA_MIN = unchecked((int)0x00001C00);
        public const int URLACTION_JAVA_PERMISSIONS = unchecked((int)0x00001C00);
        public const int URLPOLICY_JAVA_PROHIBIT = unchecked((int)0x00000000);
        public const int URLPOLICY_JAVA_HIGH = unchecked((int)0x00010000);
        public const int URLPOLICY_JAVA_MEDIUM = unchecked((int)0x00020000);
        public const int URLPOLICY_JAVA_LOW = unchecked((int)0x00030000);
        public const int URLPOLICY_JAVA_CUSTOM = unchecked((int)0x00800000);
        public const int URLACTION_JAVA_CURR_MAX = unchecked((int)0x00001C00);
        public const int URLACTION_JAVA_MAX = unchecked((int)0x00001Cff);


        // The following Infodelivery actions should have no default policies
        // in the registry.  They assume that no default policy means fall
        // back to the global restriction.  If an admin sets a policy per
        // zone, then it overrides the global restriction.

        public const int URLACTION_INFODELIVERY_MIN = unchecked((int)0x00001D00);
        public const int URLACTION_INFODELIVERY_NO_ADDING_CHANNELS = unchecked((int)0x00001D00);
        public const int URLACTION_INFODELIVERY_NO_EDITING_CHANNELS = unchecked((int)0x00001D01);
        public const int URLACTION_INFODELIVERY_NO_REMOVING_CHANNELS = unchecked((int)0x00001D02);
        public const int URLACTION_INFODELIVERY_NO_ADDING_SUBSCRIPTIONS = unchecked((int)0x00001D03);
        public const int URLACTION_INFODELIVERY_NO_EDITING_SUBSCRIPTIONS = unchecked((int)0x00001D04);
        public const int URLACTION_INFODELIVERY_NO_REMOVING_SUBSCRIPTIONS = unchecked((int)0x00001D05);
        public const int URLACTION_INFODELIVERY_NO_CHANNEL_LOGGING = unchecked((int)0x00001D06);
        public const int URLACTION_INFODELIVERY_CURR_MAX = unchecked((int)0x00001D06);
        public const int URLACTION_INFODELIVERY_MAX = unchecked((int)0x00001Dff);
        public const int URLACTION_CHANNEL_SOFTDIST_MIN = unchecked((int)0x00001E00);
        public const int URLACTION_CHANNEL_SOFTDIST_PERMISSIONS = unchecked((int)0x00001E05);
        public const int URLPOLICY_CHANNEL_SOFTDIST_PROHIBIT = unchecked((int)0x00010000);
        public const int URLPOLICY_CHANNEL_SOFTDIST_PRECACHE = unchecked((int)0x00020000);
        public const int URLPOLICY_CHANNEL_SOFTDIST_AUTOINSTALL = unchecked((int)0x00030000);
        public const int URLACTION_CHANNEL_SOFTDIST_MAX = unchecked((int)0x00001Eff);

        // For each action specified above the system maintains
        // a set of policies for the action. 
        // The only policies supported currently are permissions (i.e. is something allowed)
        // and logging status. 
        // IMPORTANT: If you are defining your own policies don't overload the meaning of the
        // loword of the policy. You can use the hiword to store any policy bits which are only
        // meaningful to your action.
        // For an example of how to do this look at the URLPOLICY_JAVA above

        // Permissions 
        public const int URLPOLICY_ALLOW = unchecked((int)0x00);
        public const int URLPOLICY_QUERY = unchecked((int)0x01);
        public const int URLPOLICY_DISALLOW = unchecked((int)0x03);

        // Notifications are not done when user already queried.
        public const int URLPOLICY_NOTIFY_ON_ALLOW = unchecked((int)0x10);
        public const int URLPOLICY_NOTIFY_ON_DISALLOW = unchecked((int)0x20);

        // Logging is done regardless of whether user was queried.
        public const int URLPOLICY_LOG_ON_ALLOW = unchecked((int)0x40);
        public const int URLPOLICY_LOG_ON_DISALLOW = unchecked((int)0x80);

        public const int URLPOLICY_MASK_PERMISSIONS = unchecked((int)0x0f);


        public const int URLPOLICY_DONTCHECKDLGBOX = unchecked((int)0x100);


        // ----------------------------------------------------------------------
        // ここ以下は COM Interface の宣言です。
        public static Guid IID_IProfferService = new Guid("cb728b20-f786-11ce-92ad-00aa00a74cd0");
        public static Guid SID_SProfferService = new Guid("cb728b20-f786-11ce-92ad-00aa00a74cd0");
        public static Guid IID_IInternetSecurityManager = new Guid("79eac9ee-baf9-11ce-8c82-00aa004ba90b");

        [ComImport,
            GuidAttribute("6d5140c1-7436-11ce-8034-00aa006009fa"),
            InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown),
            ComVisible(false)]
        public interface IServiceProvider {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject);
        }

        [ComImport,
            GuidAttribute("cb728b20-f786-11ce-92ad-00aa00a74cd0"),
            InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown),
            ComVisible(false)]
        public interface IProfferService {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ProfferService(ref Guid guidService, IServiceProvider psp, ref int cookie);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int RevokeService(int cookie);
        }

        [ComImport,
            GuidAttribute("79eac9ed-baf9-11ce-8c82-00aa004ba90b"),
            InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown),
            ComVisible(false)]
        public interface IInternetSecurityMgrSite {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetWindow(out IntPtr hwnd);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int EnableModeless([In, MarshalAs(UnmanagedType.Bool)] Boolean fEnable);
        }

        [ComImport, GuidAttribute("79eac9ee-baf9-11ce-8c82-00aa004ba90b"),
            InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown),
            ComVisible(false)]
        public interface IInternetSecurityManager {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SetSecuritySite([In] IInternetSecurityMgrSite pSite);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetSecuritySite([Out] IInternetSecurityMgrSite pSite);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int MapUrlToZone([In, MarshalAs(UnmanagedType.LPWStr)] String pwszUrl, out int pdwZone, int dwFlags);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetSecurityId([MarshalAs(UnmanagedType.LPWStr)] string pwszUrl, [MarshalAs(UnmanagedType.LPArray)] byte[] pbSecurityId, ref uint pcbSecurityId, uint dwReserved);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ProcessUrlAction([In, MarshalAs(UnmanagedType.LPWStr)] String pwszUrl, int dwAction, out byte pPolicy, int cbPolicy, byte pContext, int cbContext, int dwFlags, int dwReserved);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int QueryCustomPolicy([In, MarshalAs(UnmanagedType.LPWStr)] String pwszUrl, ref Guid guidKey, byte ppPolicy, int pcbPolicy, byte pContext, int cbContext, int dwReserved);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SetZoneMapping(int dwZone, [In, MarshalAs(UnmanagedType.LPWStr)] String lpszPattern, int dwFlags);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetZoneMappings(int dwZone, out IEnumString ppenumString, int dwFlags);
        }
    }
}
