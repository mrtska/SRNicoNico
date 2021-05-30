
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