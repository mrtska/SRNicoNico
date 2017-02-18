var VideoViewModelImpl = (function () {
    function VideoViewModelImpl() {
        this.WIDTH = 640;
        this.HEIGHT = 360;
        this.currentPlayPos = 0;
        this.checkInterval = 50.0;
        this.lastPlayPos = 0;
        this.bufferingDetected = false;
    }
    VideoViewModelImpl.prototype.initialize = function (src, initialPos, autoplay) {
        var _this = this;
        this.Video = document.getElementById("player");
        this.Video.style.height = window.innerHeight + "px";
        this.Video.src = src;
        this.Video.addEventListener("loadedmetadata", function (e) {
            _this.Video.currentTime = initialPos;
            invoke_host("widtheight", _this.Video.videoWidth + "×" + _this.Video.videoHeight);
            setInterval(function () { return _this.videoloop(); }, 50);
        });
        this.Video.addEventListener("play", function () {
            if (!_this.bufferingDetected) {
                CommentViewModel.resumeComment();
            }
            invoke_host("playstate", true);
        });
        this.Video.addEventListener("pause", function () {
            CommentViewModel.pauseComment();
            invoke_host("playstate", false);
        });
        this.Video.addEventListener("ended", function () {
            CommentViewModel.pauseComment();
            invoke_host("playstate", false);
            invoke_host("ended");
        });
        this.Video.addEventListener("playing", function () {
            CommentViewModel.resumeComment();
            invoke_host("playstate", true);
            invoke_host("playing");
        });
        this.Video.addEventListener("waiting", function () {
            CommentViewModel.pauseComment();
            invoke_host("waiting");
        });
        this.Video.addEventListener("seeking", function () {
            CommentViewModel.pauseComment();
            invoke_host("seeking");
        });
        this.Video.addEventListener("seeked", function (e) {
            CommentViewModel.purgeComment();
            if (!e.target.paused) {
                invoke_host("playstate", true);
            }
            invoke_host("seeked");
        });
        this.Video.addEventListener("canplaythrough", function (e) {
        });
        this.Video.addEventListener("progress", function (e) {
        });
        window.onresize = function (e) {
            _this.Video.style.height = window.innerHeight + "px";
            CommentViewModel.calcCommentSize(window.innerWidth, window.innerHeight);
        };
        window.onclick = function () {
            invoke_host("click");
        };
        window.onmousewheel = function (e) {
            invoke_host("mousewheel", e.wheelDelta);
        };
        this.Video.load();
        if (autoplay) {
            this.Video.play();
        }
    };
    VideoViewModelImpl.prototype.videoloop = function () {
        this.currentPlayPos = this.Video.currentTime;
        var offset = 1 / this.checkInterval;
        if (!this.bufferingDetected && this.currentPlayPos < (this.lastPlayPos + offset) && !this.Video.paused && !this.Video.ended) {
            invoke_host("bufferingstart");
            this.bufferingDetected = true;
            CommentViewModel.pauseComment();
        }
        if (this.bufferingDetected && this.currentPlayPos > (this.lastPlayPos + offset) && !this.Video.paused && !this.Video.ended) {
            invoke_host("bufferingend");
            this.bufferingDetected = false;
            CommentViewModel.resumeComment();
        }
        this.lastPlayPos = this.currentPlayPos;
        var obj = {};
        obj.vpos = Math.floor(this.Video.currentTime * 100);
        obj.time = this.Video.currentTime;
        var buffer = [];
        for (var i = 0; i < this.Video.buffered.length; i++) {
            var range = {
                start: this.Video.buffered.start(i),
                end: this.Video.buffered.end(i)
            };
            buffer.push(range);
        }
        obj.buffered = buffer;
        var played = [];
        for (var i = 0; i < this.Video.played.length; i++) {
            var range = {
                start: this.Video.played.start(i),
                end: this.Video.played.end(i)
            };
            played.push(range);
        }
        obj.played = played;
        var json = JSON.stringify(obj);
        invoke_host("currenttime", json);
        if (this.Video.buffered.length == 1) {
            if (this.Video.buffered.end(0) == this.Video.duration) {
                invoke_host("alldataloaded");
            }
        }
    };
    VideoViewModelImpl.prototype.seek = function (pos) {
        if (this.Video) {
            this.Video.currentTime = pos;
        }
    };
    VideoViewModelImpl.prototype.pause = function () {
        if (this.Video) {
            this.Video.pause();
        }
    };
    VideoViewModelImpl.prototype.play = function () {
        if (this.Video) {
            this.Video.play();
        }
    };
    VideoViewModelImpl.prototype.setVolume = function (volume) {
        if (this.Video) {
            this.Video.volume = volume;
        }
    };
    VideoViewModelImpl.prototype.setRate = function (rate) {
        if (this.Video) {
            this.Video.playbackRate = rate;
        }
    };
    VideoViewModelImpl.prototype.getVpos = function () {
        if (this.Video) {
            return this.Video.currentTime * 100;
        }
        return 0;
    };
    VideoViewModelImpl.prototype.getVideoWidth = function () {
        return this.Video.clientWidth;
    };
    return VideoViewModelImpl;
}());
var VideoViewModel = new VideoViewModelImpl();
