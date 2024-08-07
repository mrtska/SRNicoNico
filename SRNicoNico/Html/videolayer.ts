import './style.scss';

import { CommentHandler } from './commentlayer';
import Hls from 'hls.js';

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
    private hls: Hls | null;

    constructor(vm: VideoViewModel) {

        this.vm = vm;
        this.video = document.getElementById('video') as HTMLVideoElement;
        this.hls = null;
    }

    public initialize(contentUri: string, volume: number, autoplay: boolean) {

        this.video.autoplay = autoplay;

        window.addEventListener('click', e => {
            // 動画部分がクリックされたら.NET側に通知する
            postMessage({
                type: 'clicked'
            });
        });
        window.addEventListener('dblclick', e => {
            // ダブルクリックでフルスクリーン切り替え
            postMessage({
                type: 'doubleclick'
            });
        });
        window.addEventListener('wheel', async e => {

            const volume = await this.vm.Volume;
            if (e.deltaY >= 0) {
                this.vm.Volume = volume - 0.02;
            } else {
                this.vm.Volume = volume + 0.02;
            }
        });
        window.addEventListener('keydown', e => {
            postMessage({
                type: e.code
            });
            e.preventDefault();
        });
        document.addEventListener('mousemove', e => {
            postMessage({
                type: 'mousemove'
            });
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
        this.video.addEventListener('ended', e => {
            postMessage({
                type: 'ended'
            });
        });

        this.createHls(contentUri);
        this.video.volume = volume;
    }

    private createHls(contentUri: string): void {

        if (!Hls.isSupported()) {
            alert("HLSをサポートしていません");
            return;
        }

        this.hls = new Hls({
            xhrSetup: (xhr, url) => {
                xhr.withCredentials = true;
            }
        });
        this.hls.config.enableWorker = true;

        this.hls.on(Hls.Events.MEDIA_ATTACHED, () => {

            this.hls!.loadSource(contentUri);
        });

        this.hls.on(Hls.Events.MANIFEST_PARSED, () => {

        });
        this.hls.on(Hls.Events.KEY_LOADED, () => {
        });
        this.hls.on(Hls.Events.ERROR, (data, error) => {

            console.error(data);
            console.error(error);
        });
        this.hls.attachMedia(this.video);
    }

    public setSrc(contentUri: string, resume: boolean): void {

        const currentTime = this.video.currentTime;
        const paused = this.video.paused;

        if (contentUri.includes(".m3u8")) {

            if (this.hls !== null) {
                this.hls.destroy();
                this.hls.detachMedia();
            }

            this.createHls(contentUri);
        } else {

            this.video.src = contentUri;
        }

        if (resume) {
            this.video.currentTime = currentTime;
            if (paused) {
                this.video.pause();
            } else {
                this.video.play();
            }
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

    public setRate(value: number): void {
        this.video.playbackRate = value;
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
    private comment: CommentHandler;

    constructor() {
        this.vm = window.chrome.webview?.hostObjects?.vm;
        this.video = new VideoHandler(this.vm);

        this.comment = new CommentHandler();

        if (window.chrome.webview) {
            setInterval(() => this.playerloop(), 1000 / 60);
        }
        setInterval(() => this.renderloop(), 1000 / 60);
    }

    public playerloop(): void {

        const currentTime = this.video.getCurrentTime();
        if (window.chrome.webview) {
            this.vm.CurrentTime = currentTime;
        }

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

    public renderloop(): void {

        const currentTime = this.video.getCurrentTime();
        this.comment.render(currentTime);
    }

    public messageReceived(message: any): void {
        
        const type = message.type as string;
        const value = message.value;
        switch (type) {
            case 'setContent':
                this.video.initialize(value.contentUri, value.volume, value.autoplay);
                break;
            case 'setSrc':
                this.video.setSrc(value.contentUri, !value.clearComment);
                if (value.clearComment) {
                    this.comment.clearComment();
                }
                break;
            case 'seek':
                this.video.seek(value);
                break;
            case 'setVolume':
                this.video.setVolume(value);
                break;
            case 'setRate':
                this.video.setRate(value);
                break;
            case 'togglePlay':
                this.video.togglePlay();
                break;
            case 'dispatchComment':
                this.comment.initialize(value);
                break;
            case 'postComment':
                this.comment.addPostComment(value.threadId, value.fork, value.number);
                break;
            case 'setVisibility':
                this.comment.setVisibility(value);
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
