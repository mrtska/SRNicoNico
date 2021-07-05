
declare namespace chrome {
    export var webview: WebView;
}

interface Message {
    type: string;
    value?: any;
}

interface WebView {
    hostObjects: HostObjects;
    addEventListener(eventName: string, listener: (this: WebView, event: any) => any): void;
    postMessage(message: Message): void;
}

interface HostObjects {
    vm: VideoViewModel;
    sync: SyncProxy; // 同期プロキシ / sync proxy
}

interface SyncProxy {
    vm: VideoViewModel;
}

interface VideoViewModel {
    /**
     * 現在の再生時間
     */
    CurrentTime: number;
    /**
     * 音量
     */
    Volume: number;
    /**
     * 再生状態
     */
    PlayState: boolean;
    /**
     * 再生切り替え
     */
    TogglePlay(): void;
    /**
     * フルスクリーン切り替え
     */
    ToggleFullScreen(): void;
}

interface CommentLayer {
    /**
     * コメントレイヤーのZインデックス
     */
    index: number;
    /**
     * コメントリスト
     */
    entries: CommentEntry[];

    /**
     * 表示中のコメントのリスト
     */
    activeComments: CommentEntry[];
}

type CommentVisibility = 'visible' | 'hidden' | 'onlyAuthor';

type FontFamily = 'defont' | 'mincho' | 'gothic';
type FontSize = 'medium' | 'small' | 'big';
type Position = 'ue' | 'naka' | 'shita';

interface CommentEntry {
    /**
     * コメント本文
     */
    content: string;
    /**
     * コメント番号
     */
    no: number;
    /**
     * コメント再生位置
     */
    vpos: number;
    /**
     * コメント装飾
     */
    mail: string;
    /**
     * フォントサイズ
     */
    fontSize: FontSize;
    /**
     * フォントの種類
     */
    fontFamily: FontFamily;
    /**
     * コメント位置
     */
    position: Position;
    /**
     * 文字色
     */
    color: string;
    /**
     * コメント表示時間
     * 上コメか下コメの時のみ
     */
    duration: number;
    /**
     * 文字数が長い固定コメントにおいて通常4:3サイズの範囲で縮小されるところを16:9まで引き伸ばすかどうか
     */
    full: boolean;
    /**
     * 改行を含むコメントの自動縮小を無効化するかどうか
     */
    ender: boolean;
    /**
     * 半透明にする隠しコマンド
     */
    live: boolean;
    /**
     * 行数
     * 改行の数+1とも言う
     */
    lineCount: number;

    /**
     * デフォルトウィンドウサイズ(1366 * 768)におけるフォントサイズ
     */
    baseFontSize: number;
    /**
     * デフォルトウィンドウサイズにおける1行あたりの高さ
     */
    baseLineHeight: number;
    /**
     * 現在描画されているコメントのX座標
     * 画面のスケールを考慮した実際の位置
     */
    currentX: number;
    /**
     * 現在描画されているコメントのY座標
     * 画面のスケールは考慮せずデフォルトサイズでの位置
     */
    currentY: number;
    /**
     * 入り切らずにランダム位置に配置されたコメントや高さ固定かどうか
     */
    isFreedomYcoord: boolean;
    /**
     * 描画時の実際の横幅
     * 画面のスケールも考慮された値
     */
    actualWidth: number;
    /**
     * 描画時の実際の高さ
     * 画面のスケールも考慮された値
     */
    actualHeight: number;
    /**
     * デフォルトウィンドウサイズにおけるフォントプロパティの値
     * フォントサイズはem指定
     */
    baseFontProperty: string;
    /**
     * フォントによって表示に最適なオフセットがある
     */
    adjustYcoord: number;
    /**
     * 画面スケールは考慮されないパディングやオーバーフロースケールが考慮される横幅
     */
    virtualWidth: number;
    /**
     * 画面スケールは考慮されないパディングやオーバーフロースケールが考慮される高さ
     */
    virtualHeight: number;
}
