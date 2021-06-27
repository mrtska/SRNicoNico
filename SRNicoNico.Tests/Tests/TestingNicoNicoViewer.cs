using System;
using SRNicoNico.Models;
using SRNicoNico.Services;
using SRNicoNico.ViewModels;
using SRNicoNico.Views.Controls;

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
        public string UserSession { get; set; } = Constants.UserSession;


        public string AccentColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string FontFamily { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ShowExitConfirmDialog { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AutomaticPlay { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AlwaysShowSeekBar { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ClickOnPause { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DisableJumpCommand { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool UseResumePlay { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int VideoSeekAmount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public CommentVisibility CurrentCommentVisibility { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public RepeatBehavior CurrentRepeatBehavior { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float CurrentVolume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool CurrentIsMute { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DisableABRepeat { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void ChangeAccent() {
            throw new NotImplementedException();
        }

        public void ChangeFontFamily() {
            throw new NotImplementedException();
        }

        public void OpenUrl(string url) {
            throw new NotImplementedException();
        }
    }
}
