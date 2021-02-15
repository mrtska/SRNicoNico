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

            TestSessionService = new NicoNicoSessionService(this, this);
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
