
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
     * */
    CurrentTime: number;
    /**
     * 音量
     * */
    Volume: number;
    /**
     * 再生状態
     * */
    PlayState: boolean;
    /**
     * 再生切り替え
     * */
    TogglePlay(): void;
}

interface CommentLayer {
    /**
     * コメントレイヤーのZインデックス
     **/
    index: number;
    /**
     * コメントリスト
     **/
    entries: CommentEntry[];

    /**
     * 表示中のコメントのリスト
     **/
    activeComments: CommentEntry[];
}

type FontSize = 'middle' | 'small' | 'big';
type Position = 'ue' | 'naka' | 'shita';

interface CommentEntry {
    /**
     * コメント本文
     * */
    content: string;
    /**
     * コメント番号
     **/
    no: number;
    /**
     * コメント再生位置
     **/
    vpos: number;
    /**
     * コメント装飾
     **/
    mail: string;
    /**
     * フォントサイズ
     **/
    fontSize: FontSize;
    /**
     * コメント位置
     **/
    position: Position;
    /**
     * 文字色
     */
    color: string;

    /**
     * 現在描画されているコメントのX座標
     */
    currentX: number;
    /**
     * 現在描画されているコメントのY座標
     */
    currentY: number;
    /**
     * 描画時の実際の横幅
     */
    actualWidth: number;
    /**
     * 描画時の実際の高さ
     */
    actualHeight: number;
}
