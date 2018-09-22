/// <reference path="jquery.d.ts" />
var SWFVideoViewModelImpl = /** @class */ (function () {
    function SWFVideoViewModelImpl() {
        this.WIDTH = 640;
        this.HEIGHT = 360;
    }
    SWFVideoViewModelImpl.prototype.initialize = function (src, initialPos, autoplay) {
        var _this = this;
        //レイヤーを取得
        this.Video = document.getElementById("player");
        //動画の高さをウィンドウの高さに合わせる
        this.Video.style.height = window.innerHeight + "px";
        this.Video.paused = false;
        this.Video.seeking = false;
        this.Video.ended = false;
        this.Video.AsOpenVideo(src, initialPos, autoplay);
        //ウィンドウサイズが変わったら動画の高さやコメントのサイズを計算しなおす
        window.onresize = function (e) {
            //動画の高さを現在のウィンドウの高さに
            _this.Video.style.height = window.innerHeight + "px";
            GetComment().calcCommentSize(window.innerWidth, window.innerHeight);
        };
        //クリックして一時停止をハンドリングするために
        window.onmousedown = function (e) {
            //左クリックのみ
            if (e.button == 0) {
                InvokeHost("click");
            }
        };
        window.onmousewheel = function (e) {
            InvokeHost("mousewheel", e.wheelDelta);
        };
    };
    SWFVideoViewModelImpl.prototype.videoloop = function (time, vpos, buffer) {
        //C#側に渡すパラメータ
        var obj = {};
        //コメント描画タイミングのvposや現在の再生時間をobjに詰め込む
        obj.vpos = vpos;
        obj.time = time;
        //動画のバッファされた範囲をobjに突っ込む
        var bufferrange = [];
        var brange = {
            start: 0,
            end: buffer
        };
        bufferrange.push(brange);
        obj.buffered = bufferrange;
        //再生した部分の範囲をobjに突っ込む
        var played = [];
        var prange = {
            start: 0,
            end: time
        };
        played.push(prange);
        obj.played = played;
        //objをJsonに変換してC#側に渡す
        var json = JSON.stringify(obj);
        InvokeHost("currenttime", json);
    };
    //動画シーク
    SWFVideoViewModelImpl.prototype.seek = function (pos) {
        if (this.Video) {
            this.Video.AsSeek(pos);
        }
    };
    //一時停止
    SWFVideoViewModelImpl.prototype.pause = function () {
        if (this.Video) {
            this.Video.paused = true;
            this.Video.AsPause();
        }
    };
    //再開
    SWFVideoViewModelImpl.prototype.play = function () {
        if (this.Video) {
            this.Video.paused = false;
            this.Video.ended = false;
            this.Video.AsPlay();
        }
    };
    //音量調整
    SWFVideoViewModelImpl.prototype.setVolume = function (volume) {
        if (this.Video) {
            this.Video.AsSetVolume(volume);
        }
    };
    //現在のVposを取得
    SWFVideoViewModelImpl.prototype.getVpos = function () {
        if (this.Video) {
            return this.Video.AsCurrentTime() * 100;
        }
        return 0;
    };
    //動画の横幅を返す 動画データではなくobjectタグの横幅
    SWFVideoViewModelImpl.prototype.getVideoWidth = function () {
        return this.Video.clientWidth;
    };
    return SWFVideoViewModelImpl;
}());
//# sourceMappingURL=videoflashplayer.js.map