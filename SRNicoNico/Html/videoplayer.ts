
import * as Hls from "hls.js";
import { GetComment } from "./commentlayer";
import "./external";

namespace SRNicoNico.Video {

    export class VideoViewModel {

        WIDTH: number = 640;
        HEIGHT: number = 360;

        public Video: HTMLVideoElement;

        private loaded: boolean = false;

        private loop: number = -1;

        constructor(src: string, initialPos: number, autoplay: boolean) {

            //レイヤーを取得
            this.Video = document.getElementById("player") as HTMLVideoElement;

            //動画の高さをウィンドウの高さに合わせる
            this.Video.style.height = window.innerHeight + "px";

            InvokeHost("src " + src);

            // HLSの場合
            if (src.indexOf("master.m3u8") >= 0) {


                if (Hls.isSupported()) {

                    var hls = new Hls();
                    hls.loadSource(src);
                    hls.attachMedia(this.Video);
                    hls.config.enableSoftwareAES = false;
                    hls.config.enableWorker = true;
                    hls.on(Hls.Events.MANIFEST_PARSED, () => {

                    });
                    hls.on(Hls.Events.KEY_LOADED, () => {


                    });
                    hls.on(Hls.Events.ERROR, (data, error) => {

                        InvokeHost("error" + JSON.stringify(error));
                    });

                } else {

                    alert("お使いのバージョンのOSでは再生できません。");
                    return;
                }
            } else {

                this.Video.src = src;
            }



            //動画のメタデータロード後
            this.Video.addEventListener("loadedmetadata", (e) => {

                this.loaded = true;
                this.Video.currentTime = initialPos;
                InvokeHost("widtheight", this.Video.videoWidth + "×" + this.Video.videoHeight);

                if (this.loop == -1) {
                    //動画のバッファした部分とか現在時間などをC#に送るメインループ
                    this.loop = setInterval(() => this.videoloop(), 50); //50ms毎に実行すれば多分大丈夫かなと
                }
            });

            this.Video.addEventListener("keydown", (e) => {

                e.preventDefault();
            });

            //再生されたらコメントの一時停止を解除してC#側に伝える
            this.Video.addEventListener("play", () => {

                //動画バッファリング中に再生ボタンを押されてもコメントが動き出さないようにね
                if (!this.bufferingDetected) {

                    GetComment().resumeComment();
                }
                InvokeHost("playstate", true);
            });

            //一時停止されたらコメントも一時停止してC#側に伝える
            this.Video.addEventListener("pause", function () {

                GetComment().pauseComment();
                InvokeHost("playstate", false);
            });

            this.Video.addEventListener("ended", function () {

                GetComment().pauseComment();
                InvokeHost("playstate", false);
                InvokeHost("ended");
            });

            this.Video.addEventListener("playing", function () {

                GetComment().resumeComment();
                InvokeHost("playstate", true);
                InvokeHost("playing");
            });

            this.Video.addEventListener("waiting", function () {

                GetComment().pauseComment();
                InvokeHost("waiting");
            });

            this.Video.addEventListener("seeking", function () {

                GetComment().pauseComment();
                InvokeHost("seeking");
            });
            this.Video.addEventListener("seeked", function (e) {

                GetComment().purgeComment();

                if (!(e.target as HTMLVideoElement).paused) {

                    InvokeHost("playstate", true);
                }

                InvokeHost("seeked");
            });

            this.Video.addEventListener("canplaythrough", function (e) {

                // invoke_host("canplaythrough");
            });
            this.Video.addEventListener("progress", function (e) {

                //invoke_host("progress");
            });

            //ウィンドウサイズが変わったら動画の高さやコメントのサイズを計算しなおす
            window.onresize = (e) => {

                //動画の高さを現在のウィンドウの高さに
                this.Video.style.height = window.innerHeight + "px";

                GetComment().calcCommentSize(window.innerWidth, window.innerHeight);

            };

            //クリックして一時停止をハンドリングするために
            window.onclick = function () {

                InvokeHost("click");
            }
            window.addEventListener("wheel", function (e: WheelEvent) {

                InvokeHost("mousewheel", -e.deltaY);
            });

            if (autoplay) {

                this.Video.play();
                InvokeHost("playstate", true);
            } else {

                InvokeHost("playstate", false);
            }
        }

        private currentPlayPos: number = 0;
        private checkInterval: number = 50.0;
        private lastPlayPos: number = 0;
        public bufferingDetected: boolean = false;

        public videoloop() {

            this.currentPlayPos = this.Video.currentTime;

            var offset = 1 / this.checkInterval;

            if (!this.bufferingDetected && this.currentPlayPos < (this.lastPlayPos + offset) && !this.Video.paused && !this.Video.ended) {

                InvokeHost("bufferingstart");
                this.bufferingDetected = true;
                GetComment().pauseComment();
            }
            if (this.bufferingDetected && this.currentPlayPos > (this.lastPlayPos + offset) && !this.Video.paused && !this.Video.ended) {

                InvokeHost("bufferingend");
                this.bufferingDetected = false;
                GetComment().resumeComment();
            }
            this.lastPlayPos = this.currentPlayPos;


            //C#側に渡すパラメータ
            var obj: any = {};

            //コメント描画タイミングのvposや現在の再生時間をobjに詰め込む
            obj.vpos = Math.floor(this.Video.currentTime * 100);
            obj.time = this.Video.currentTime;

            //動画のバッファされた範囲をobjに突っ込む
            var buffer = [];
            for (var i = 0; i < this.Video.buffered.length; i++) {

                var range = {
                    start: this.Video.buffered.start(i),
                    end: this.Video.buffered.end(i)
                };
                buffer.push(range);
            }
            obj.buffered = buffer;


            //再生した部分の範囲をobjに突っ込む
            var played = [];
            for (var i = 0; i < this.Video.played.length; i++) {

                var range = {
                    start: this.Video.played.start(i),
                    end: this.Video.played.end(i)
                };
                played.push(range);
            }
            obj.played = played;

            //objをJsonに変換してC#側に渡す
            var json = JSON.stringify(obj);
            InvokeHost("currenttime", json);

            if (this.Video.buffered.length == 1) {

                if (this.Video.buffered.end(0) == this.Video.duration) {

                    InvokeHost("alldataloaded");
                }
            }
        }
        //動画シーク
        public seek(pos: number): void {

            if (this.Video && this.loaded) {

                this.Video.currentTime = pos;
            }
        }
        //一時停止
        public pause(): void {

            if (this.Video) {

                this.Video.pause();
            }
        }
        //再開
        public play() {

            if (this.Video) {

                this.Video.play();
            }
        }
        //音量調整
        public setVolume(volume: number): void {

            if (this.Video) {

                this.Video.volume = volume;
            }
        }
        //再生倍率
        public setRate(rate: number): void {

            if (this.Video) {

                this.Video.defaultPlaybackRate = rate;
                this.Video.playbackRate = rate;
            }
        }
        //現在のVposを取得
        public getVpos(): number {

            if (this.Video) {

                return this.Video.currentTime * 100;
            }
            return 0;
        }
        //動画の横幅を返す 動画データではなくvideoタグの横幅
        public getVideoWidth(): number {

            return this.Video.clientWidth;
        }
    }
    export var ViewModel: VideoViewModel;
}

export function GetVideo(): SRNicoNico.Video.VideoViewModel {

    return SRNicoNico.Video.ViewModel;
}
eval("window.GetVideo = GetVideo;");

function Video$Initialize(src: string, initialPos: number, autoplay: boolean) {

    SRNicoNico.Video.ViewModel = new SRNicoNico.Video.VideoViewModel(src, initialPos, autoplay);
}
eval("window.Video$Initialize = Video$Initialize;");

function Video$Pause() {

    SRNicoNico.Video.ViewModel.pause();
}
eval("window.Video$Pause = Video$Pause;");

function Video$Resume() {

    SRNicoNico.Video.ViewModel.play();
}
eval("window.Video$Resume = Video$Resume;");

function Video$Seek(pos: number) {

    SRNicoNico.Video.ViewModel.seek(pos);
}
eval("window.Video$Seek = Video$Seek;");

function Video$SetVolume(vol: number) {

    SRNicoNico.Video.ViewModel.setVolume(vol);
}
eval("window.Video$SetVolume = Video$SetVolume;");

function Video$SetRate(rate: number) {

    SRNicoNico.Video.ViewModel.setRate(rate);
}
eval("window.Video$SetRate = Video$SetRate;");

window.addEventListener("load", function () {

    InvokeHost("ready");
});
