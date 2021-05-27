import './style.scss';

function postMessage(message: any) {

    if (window.chrome.webview) {
        window.chrome.webview.postMessage(message);
    }
}

/**
 * 動画を制御するクラス
 * */
class VideoHandler {

    private video: HTMLVideoElement;

    constructor() {

        this.video = document.getElementById("video") as HTMLVideoElement;
    }

    public initialize(contentUri: string) {

        this.video.src = contentUri;
        this.video.play();
    }

};

/**
 * 動画プレイヤーを制御するクラス
 * */
class PlayerHandler {

    private vm: VideoViewModel;
    private video: VideoHandler;

    constructor() {
        this.vm = window.chrome.webview?.hostObjects?.sync.vm;
        this.video = new VideoHandler();
    }

    public playerloop(): void {
        

    }

    public messageReceived(message: any): void {

        const type = message.type as string;
        const value = message.value as string;
        if (type === 'setContent') {
            this.video.initialize(value);
        }
    }

};

const player = new PlayerHandler();

window.chrome.webview?.addEventListener('message', event => {

    player.messageReceived(event.data);
});


// ブラウザ側の初期化が完了したことを.NET側に通知する
postMessage({
    type: 'initialized'
});