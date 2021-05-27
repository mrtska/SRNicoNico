
declare namespace chrome {
    export var webview: WebView;
};

interface WebView {
    hostObjects: HostObjects;
    addEventListener(eventName, listener: (this: WebView, event: any) => any);
    postMessage(message: string);
};

interface HostObjects {
    vm: VideoViewModel;
    sync: SyncProxy; // 同期プロキシ / sync proxy
};

interface SyncProxy {
    vm: VideoViewModel;
};

interface VideoViewModel {
    GetString(): string;
};