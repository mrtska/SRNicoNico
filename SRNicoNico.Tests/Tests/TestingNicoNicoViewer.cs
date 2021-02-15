using System;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Services;

namespace SRNicoNico.Tests {
    /// <summary>
    /// テスト用クラス
    /// </summary>
    public class TestingNicoNicoViewer : INicoNicoViewer, ISettings {

        public static readonly TestingNicoNicoViewer Instance = new TestingNicoNicoViewer();

        public NicoNicoSessionService TestSessionService;

        public TestingNicoNicoViewer() {

            TestSessionService = new NicoNicoSessionService(this, this) {
                // SessionServiceがサインインダイアログを表示させようとしているということは認証に失敗しているということ
                SignInDialogHandler = () => throw new InvalidOperationException("認証に失敗しました")
            };
            TestSessionService.StoreSession(UserSession);
        }
        
        public double CurrentVersion => 2;

        /// <inheritdoc />
        public string DefaultWebViewPageUrl { get; set; } = "https://www.nicovideo.jp/";

        /// <summary>
        /// テストで使うセッション
        /// </summary>
        public string UserSession { get; set; } = "";
    }
}
