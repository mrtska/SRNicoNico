var SRNicoNico;
(function (SRNicoNico) {
    var Video;
    (function (Video) {
        var VideoViewModel = /** @class */ (function () {
            function VideoViewModel() {
                this.WIDTH = 640;
                this.HEIGHT = 360;
                this.loaded = false;
                this.loop = -1;
                this.currentPlayPos = 0;
                this.checkInterval = 50.0;
                this.lastPlayPos = 0;
                this.bufferingDetected = false;
            }
            VideoViewModel.prototype.initialize = function (src, initialPos, autoplay) {
                var _this = this;
                //レイヤーを取得
                this.Video = document.getElementById("player");
                //動画の高さをウィンドウの高さに合わせる
                this.Video.style.height = window.innerHeight + "px";
                //動画のURLを設定
                this.Video.src = src;
                //動画のメタデータロード後
                this.Video.addEventListener("loadedmetadata", function (e) {
                    _this.loaded = true;
                    _this.Video.currentTime = initialPos;
                    InvokeHost("widtheight", _this.Video.videoWidth + "×" + _this.Video.videoHeight);
                    if (_this.loop == -1) {
                        //動画のバッファした部分とか現在時間などをC#に送るメインループ
                        _this.loop = setInterval(function () { return _this.videoloop(); }, 50); //50ms毎に実行すれば多分大丈夫かなと
                    }
                });
                //再生されたらコメントの一時停止を解除してC#側に伝える
                this.Video.addEventListener("play", function () {
                    //動画バッファリング中に再生ボタンを押されてもコメントが動き出さないようにね
                    if (!_this.bufferingDetected) {
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
                    if (!e.target.paused) {
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
                window.onresize = function (e) {
                    //動画の高さを現在のウィンドウの高さに
                    _this.Video.style.height = window.innerHeight + "px";
                    GetComment().calcCommentSize(window.innerWidth, window.innerHeight);
                };
                //クリックして一時停止をハンドリングするために
                window.onclick = function () {
                    InvokeHost("click");
                };
                window.onmousewheel = function (e) {
                    InvokeHost("mousewheel", e.wheelDelta);
                };
                if (autoplay) {
                    this.Video.play();
                    InvokeHost("playstate", true);
                }
                else {
                    InvokeHost("playstate", false);
                }
            };
            VideoViewModel.prototype.videoloop = function () {
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
                var obj = {};
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
            };
            //動画シーク
            VideoViewModel.prototype.seek = function (pos) {
                if (this.Video && this.loaded) {
                    this.Video.currentTime = pos;
                }
            };
            //一時停止
            VideoViewModel.prototype.pause = function () {
                if (this.Video) {
                    this.Video.pause();
                }
            };
            //再開
            VideoViewModel.prototype.play = function () {
                if (this.Video) {
                    this.Video.play();
                }
            };
            //音量調整
            VideoViewModel.prototype.setVolume = function (volume) {
                if (this.Video) {
                    this.Video.volume = volume;
                }
            };
            //再生倍率
            VideoViewModel.prototype.setRate = function (rate) {
                if (this.Video) {
                    this.Video.defaultPlaybackRate = rate;
                    this.Video.playbackRate = rate;
                }
            };
            //現在のVposを取得
            VideoViewModel.prototype.getVpos = function () {
                if (this.Video) {
                    return this.Video.currentTime * 100;
                }
                return 0;
            };
            //動画の横幅を返す 動画データではなくvideoタグの横幅
            VideoViewModel.prototype.getVideoWidth = function () {
                return this.Video.clientWidth;
            };
            return VideoViewModel;
        }());
        Video.VideoViewModel = VideoViewModel;
        Video.ViewModel = new VideoViewModel();
    })(Video = SRNicoNico.Video || (SRNicoNico.Video = {}));
})(SRNicoNico || (SRNicoNico = {}));
function GetVideo() {
    return SRNicoNico.Video.ViewModel;
}
function Video$Initialize(src, initialPos, autoplay) {
    SRNicoNico.Video.ViewModel.initialize(src, initialPos, autoplay);
}
function Video$Pause() {
    SRNicoNico.Video.ViewModel.pause();
}
function Video$Resume() {
    SRNicoNico.Video.ViewModel.play();
}
function Video$Seek(pos) {
    SRNicoNico.Video.ViewModel.seek(pos);
}
function Video$SetVolume(vol) {
    SRNicoNico.Video.ViewModel.setVolume(vol);
}
function Video$SetRate(rate) {
    SRNicoNico.Video.ViewModel.setRate(rate);
}
window.addEventListener("load", function () {
    InvokeHost("ready");
});
//# sourceMappingURL=videoplayer.js.map