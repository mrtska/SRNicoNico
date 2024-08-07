﻿using SRNicoNico.Services;
using SRNicoNico.ViewModels;
using SRNicoNico.Views.Controls;

namespace SRNicoNico.Models {
    /// <summary>
    /// 設定項目
    /// </summary>
    public interface ISettings {

        /// <summary>
        /// セッション文字列
        /// </summary>
        string? UserSession { get; set; }

        /// <summary>
        /// WebViewで開くデフォルトページ
        /// </summary>
        string DefaultWebViewPageUrl { get; set; }

        /// <summary>
        /// アクセントカラー
        /// </summary>
        string AccentColor { get; set; }

        /// <summary>
        /// フォントファミリ
        /// </summary>
        string FontFamily { get; set; }

        /// <summary>
        /// 終了時に確認ダイアログを出すかどうか
        /// </summary>
        bool ShowExitConfirmDialog { get; set; }

        /// <summary>
        /// 現在の検索設定
        /// </summary>
        SearchSortKey SelectedSortKey { get; set; }

        /// <summary>
        /// 現在のコメント表示設定
        /// </summary>
        CommentVisibility CurrentCommentVisibility { get; set; }

        /// <summary>
        /// 現在のリピート動作設定
        /// </summary>
        RepeatBehavior CurrentRepeatBehavior { get; set; }

        /// <summary>
        /// 現在の音量
        /// </summary>
        float CurrentVolume { get; set; }

        /// <summary>
        /// 現在のミュート設定
        /// </summary>
        bool CurrentIsMute { get; set; }

        /// <summary>
        /// ABリピートを無効にするかどうか
        /// </summary>
        bool DisableABRepeat { get; set; }

        /// <summary>
        /// 動画を自動再生するかどうか
        /// </summary>
        bool AutomaticPlay { get; set; }

        /// <summary>
        /// フルスクリーン時にシークバーを常に表示するかどうか
        /// </summary>
        bool AlwaysShowSeekBar { get; set; }

        /// <summary>
        /// 動画をクリックした時に一時停止するかどうか
        /// </summary>
        bool ClickOnPause { get; set; }

        /// <summary>
        /// 動画をダブルクリックした時にフルスクリーン切り替えするかどうか
        /// </summary>
        bool DoubleClickToggleFullScreen { get; set; }

        /// <summary>
        /// ジャンプコマンドを無効にするかどうか
        /// </summary>
        bool DisableJumpCommand { get; set; }

        /// <summary>
        /// レジューム再生を有効にするかどうか
        /// </summary>
        bool UseResumePlay { get; set; }

        /// <summary>
        /// カーソルキーでシークした際にシークする秒数
        /// </summary>
        int VideoSeekAmount { get; set; }

        /// <summary>
        /// フルスクリーン時にシークバーのポップアップをどこに表示するか
        /// </summary>
        PopupPlacement FullScreenPopupPlacement { get; set; }

        /// <summary>
        /// 動画をロードする際にHLSを使うかどうか
        /// </summary>
        bool DisableHls { get; set; }

        /// <summary>
        /// かんたんコメントを無効にするかどうか
        /// </summary>
        bool DisableEasyComment { get; set; }

        /// <summary>
        /// ミュートされた投稿者の動画を一覧に表示しない
        /// </summary>
        bool HideMutedVideo { get; set; }

        /// <summary>
        /// 生放送更新通知更新間隔
        /// </summary>
        int LiveNotifyRefreshInterval { get; set; }

        /// <summary>
        /// 現在の設定でアクセントを変更する
        /// </summary>
        void ChangeAccent();

        /// <summary>
        /// 現在の設定でフォントを変更する
        /// </summary>
        void ChangeFontFamily();

        /// <summary>
        /// ミュート設定を変更する
        /// </summary>
        void ChangeMutedAccount();
    }
}
