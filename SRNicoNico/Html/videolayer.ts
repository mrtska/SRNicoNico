import './style.scss';

function postMessage(message: Message) {

    if (window.chrome.webview) {
        window.chrome.webview.postMessage(message);
    }
}

/**
 * 動画を制御するクラス
 * */
class VideoHandler {

    private videoElement: HTMLVideoElement;

    constructor() {

        this.videoElement = document.getElementById("video") as HTMLVideoElement;
    }

    public initialize(contentUri: string, volume: number, autoplay: boolean) {

        this.videoElement.addEventListener('click', e => {
            // 動画部分がクリックされたら.NET側に通知する
            postMessage({
                type: 'clicked'
            });
        });
        this.videoElement.addEventListener('loadedmetadata', e => {

            postMessage({
                type: 'info',
                value: {
                    width: this.videoElement.videoWidth,
                    height: this.videoElement.videoHeight,
                    duration: this.videoElement.duration
                }
            });
        });

        this.videoElement.src = contentUri;
        this.videoElement.volume = volume;
        if (autoplay) {
            this.videoElement.play();
        }
    }

    public getCurrentTime(): number {
        return this.videoElement.currentTime;
    }

    public seek(position: number): void {
        this.videoElement.currentTime = position;
    }

    public setVolume(volume: number): void {
        this.videoElement.volume = volume;
    }

    public getPlayedTimeRanges(): TimeRanges {
        return this.videoElement.played;
    }

    public getBufferedTimeRanges(): TimeRanges {
        return this.videoElement.buffered;
    }
};

/**
 * 動画プレイヤーを制御するクラス
 * */
class PlayerHandler {

    private vm: VideoViewModel;
    private video: VideoHandler;

    constructor() {
        this.vm = window.chrome.webview?.hostObjects?.vm;
        this.video = new VideoHandler();

        setInterval(() => this.playerloop(), 1000 / 60);
    }

    public playerloop(): void {

        this.vm.CurrentTime = this.video.getCurrentTime();

        const played = this.video.getPlayedTimeRanges();
        const buffered = this.video.getBufferedTimeRanges();

        const notify = {
            played: Array<any>(),
            buffered: Array<any>()
        };

        for (let i = 0; i < played.length; i++) {
            notify.played.push({
                start: played.start(i),
                end: played.end(i)
            });
        }
        for (let i = 0; i < buffered.length; i++) {
            notify.buffered.push({
                start: buffered.start(i),
                end: buffered.end(i)
            });
        }

        postMessage({
            type: 'loop',
            value: notify
        });
    }

    public messageReceived(message: any): void {
        
        const type = message.type as string;
        const value = message.value;

        switch (type) {
            case 'setContent':
                this.video.initialize(value.contentUri, value.volume, value.autoplay);
                break;
            case 'seek':
                this.video.seek(value);
                break;
            case 'setVolume':
                this.video.setVolume(value);
                break;
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
