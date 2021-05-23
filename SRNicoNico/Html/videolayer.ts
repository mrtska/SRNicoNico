declare namespace chrome {
    export var webview: WebView;
};

interface WebView {
    hostObjects: HostObjects;
    addEventListener(eventName, listener: (this: WebView, event: any) => any);
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

/**
 * 動画を制御するクラス
 * */
class VideoHandler {




};


/**
 * 動画プレイヤーを制御するクラス
 * */
class PlayerHandler {

    private vm: VideoViewModel;
    private video: VideoHandler;

    constructor() {
        this.vm = window.chrome.webview.hostObjects.sync.vm;
        this.video = new VideoHandler();
    }

    public playerloop(): void {


    }

    public messageReceived(message: string): void {

        alert(message);
    }

};

const player = new PlayerHandler();

window.chrome.webview.addEventListener('message', event => {

    player.messageReceived(event.data);
});


