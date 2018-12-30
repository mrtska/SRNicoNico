

namespace SRNicoNico.Live {
    export class LiveViewModel {

        WIDTH: number = 640;
        HEIGHT: number = 360;

        public Video: HTMLVideoElement;

        private loaded: boolean = false;

        private loop: number = -1;

        public initialize(src: string, initialPos: number, autoplay: boolean) {

            //レイヤーを取得
            this.Video = document.getElementById("player") as HTMLVideoElement;

            //動画の高さをウィンドウの高さに合わせる
            this.Video.style.height = window.innerHeight + "px";

            //動画のURLを設定
            this.Video.src = src;

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

                InvokeHost("canplaythrough");
            });
            this.Video.addEventListener("progress", function (e) {

                InvokeHost("progress");
            });
            
            this.Video.addEventListener("onerror", function (e: ErrorEvent) {

                InvokeHost(e.error);
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
            window.onmousewheel = function (e) {

                InvokeHost("mousewheel", e.wheelDelta);
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
    export var ViewModel: LiveViewModel = new LiveViewModel();

}

function GetLive(): SRNicoNico.Live.LiveViewModel {

    return SRNicoNico.Live.ViewModel;
}

function Live$Initialize(src: string, initialPos: number, autoplay: boolean) {

    SRNicoNico.Live.ViewModel.initialize(src, initialPos, autoplay);
}
function Live$Pause() {

    SRNicoNico.Live.ViewModel.pause();
}
function Live$Resume() {

    SRNicoNico.Live.ViewModel.play();
}
function Live$Seek(pos: number) {

    SRNicoNico.Live.ViewModel.seek(pos);
}
function Live$SetVolume(vol: number) {

    SRNicoNico.Live.ViewModel.setVolume(vol);
}
function Live$SetRate(rate: number) {

    SRNicoNico.Live.ViewModel.setRate(rate);
}

window.addEventListener("load", function () {

    InvokeHost("ready");
});
