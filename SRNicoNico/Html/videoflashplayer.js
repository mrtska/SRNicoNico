var SWFVideoViewModelImpl = (function () {
    function SWFVideoViewModelImpl() {
        this.WIDTH = 640;
        this.HEIGHT = 360;
    }
    SWFVideoViewModelImpl.prototype.initialize = function (src, initialPos, autoplay) {
        var _this = this;
        this.Video = document.getElementById("player");
        this.Video.style.height = window.innerHeight + "px";
        this.Video.paused = false;
        this.Video.seeking = false;
        this.Video.ended = false;
        this.Video.AsOpenVideo(src, initialPos, autoplay);
        window.onresize = function (e) {
            _this.Video.style.height = window.innerHeight + "px";
            CommentViewModel.calcCommentSize(window.innerWidth, window.innerHeight);
        };
        window.onmousedown = function (e) {
            if (e.button == 0) {
                invoke_host("click");
            }
        };
        window.onmousewheel = function (e) {
            invoke_host("mousewheel", e.wheelDelta);
        };
    };
    SWFVideoViewModelImpl.prototype.videoloop = function (time, vpos, buffer) {
        var obj = {};
        obj.vpos = vpos;
        obj.time = time;
        var bufferrange = [];
        var brange = {
            start: 0,
            end: buffer
        };
        bufferrange.push(brange);
        obj.buffered = bufferrange;
        var played = [];
        var prange = {
            start: 0,
            end: time
        };
        played.push(prange);
        obj.played = played;
        var json = JSON.stringify(obj);
        invoke_host("currenttime", json);
    };
    SWFVideoViewModelImpl.prototype.seek = function (pos) {
        if (this.Video) {
            this.Video.AsSeek(pos);
        }
    };
    SWFVideoViewModelImpl.prototype.pause = function () {
        if (this.Video) {
            this.Video.paused = true;
            this.Video.AsPause();
        }
    };
    SWFVideoViewModelImpl.prototype.play = function () {
        if (this.Video) {
            this.Video.paused = false;
            this.Video.ended = false;
            this.Video.AsPlay();
        }
    };
    SWFVideoViewModelImpl.prototype.setVolume = function (volume) {
        if (this.Video) {
            this.Video.AsSetVolume(volume);
        }
    };
    SWFVideoViewModelImpl.prototype.getVpos = function () {
        if (this.Video) {
            return this.Video.AsCurrentTime() * 100;
        }
        return 0;
    };
    SWFVideoViewModelImpl.prototype.getVideoWidth = function () {
        return this.Video.clientWidth;
    };
    return SWFVideoViewModelImpl;
}());
VideoViewModel = new SWFVideoViewModelImpl();
