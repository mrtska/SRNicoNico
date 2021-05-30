﻿import './style.scss';

function postMessage(message: Message) {

    if (window.chrome.webview) {
        window.chrome.webview.postMessage(message);
    }
}

/**
 * 動画を制御するクラス
 * */
class VideoHandler {

    private vm: VideoViewModel;
    private video: HTMLVideoElement;

    constructor(vm: VideoViewModel) {

        this.vm = vm;
        this.video = document.getElementById('video') as HTMLVideoElement;
    }

    public initialize(contentUri: string, volume: number, autoplay: boolean) {

        this.video.addEventListener('click', e => {
            // 動画部分がクリックされたら.NET側に通知する
            this.vm.TogglePlay();
        });
        this.video.addEventListener('wheel', async e => {

            const volume = await this.vm.Volume;
            if (e.deltaY >= 0) {
                this.vm.Volume = volume - 0.02;
            } else {
                this.vm.Volume = volume + 0.02;
            }
        });

        this.video.addEventListener('pause', e => {
            this.vm.PlayState = false;
        });
        this.video.addEventListener('play', e => {
            this.vm.PlayState = true;
        });

        this.video.addEventListener('loadedmetadata', e => {

            postMessage({
                type: 'info',
                value: {
                    width: this.video.videoWidth,
                    height: this.video.videoHeight,
                    duration: this.video.duration
                }
            });
        });

        this.video.src = contentUri;
        this.video.volume = volume;
        if (autoplay) {
            this.video.play();
        }
    }

    public getCurrentTime(): number {
        return this.video.currentTime;
    }

    public seek(position: number): void {
        this.video.currentTime = position;
    }

    public setVolume(volume: number): void {
        this.video.volume = volume;
    }

    public togglePlay(): void {

        if (this.video.paused) {
            this.video.play();
        } else {
            this.video.pause();
        }
    }

    public getPlayedTimeRanges(): TimeRanges {
        return this.video.played;
    }

    public getBufferedTimeRanges(): TimeRanges {
        return this.video.buffered;
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
        this.video = new VideoHandler(this.vm);

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
            case 'togglePlay':
                this.video.togglePlay();
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
