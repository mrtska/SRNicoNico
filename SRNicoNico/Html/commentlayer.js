function invoke_host(cmd, args) {
    if (args === void 0) { args = ""; }
    if (window.external != null && cmd != null) {
        window.external.InvokeFromJavaScript(cmd, args);
    }
}
var CommentEntry = (function () {
    function CommentEntry() {
    }
    CommentEntry.prototype.deserialize = function (json) {
        this.Number = json.Number;
        this.Vpos = json.Vpos;
        this.Vend = json.Vend;
        this.Position = json.Position;
        this.CommentColor = json.CommentColor;
        this.CommentSize = json.CommentSize;
        this.Duration = json.Duration;
        this.Scale = json.Scale;
        this.Opacity = json.Opacity;
        this.Reverse = json.Reverse;
        this.IsUploader = json.IsUploader;
        this.JustPosted = json.JustPosted;
        this.Content = json.Content;
        this.Element.innerText = this.Content;
        if (this.JustPosted) {
            $(this.Element).css("border", "solid 1px #FFFF00");
        }
        if (json.Font == "mincho") {
            $(this.Element).css("font-family", "游明朝");
        }
    };
    CommentEntry.prototype.getTop = function () {
        return parseInt($(this.Element).css("top"));
    };
    return CommentEntry;
}());
var CommentViewModelImpl = (function () {
    function CommentViewModelImpl() {
        this.WIDTH = 640;
        this.HEIGHT = 360;
        this.SmallCommentSize = 15;
        this.RegularCommentSize = 24;
        this.BigCommentSize = 39;
        this.RenderingCommentList = new Array();
    }
    CommentViewModelImpl.prototype.initialize = function () {
        this.Layer = document.getElementById("commentlayer");
        var isRollOver = true;
        var hideFunc = function () {
            if (isRollOver) {
                document.body.style["cursor"] = "none";
                invoke_host("hidecontroller");
            }
        };
        var id = -1;
        $(document).mouseleave(function () {
            isRollOver = false;
            clearInterval(id);
        });
        var x = 0;
        var y = 0;
        $(document).mousemove(function (e) {
            if (x == e.pageX && y == e.pageY) {
                return;
            }
            x = e.pageX;
            y = e.pageY;
            document.body.style["cursor"] = "default";
            clearInterval(id);
            id = setInterval(hideFunc, 1600);
            isRollOver = true;
            invoke_host("showcontroller");
        });
    };
    CommentViewModelImpl.prototype.dispatchComment = function (jsonStr) {
        var _this = this;
        var entry = new CommentEntry();
        entry.Element = document.createElement("span");
        entry.deserialize(JSON.parse(jsonStr));
        if (entry.Position == "ue") {
            $(entry.Element).css("left", "50%");
            $(entry.Element).css("transform", "translate(-50%, 0%)");
        }
        else if (entry.Position == "shita") {
            $(entry.Element).css("bottom", "0");
            $(entry.Element).css("left", "50%");
            $(entry.Element).css("transform", "translate(-50%, 0%)");
        }
        else {
            $(entry.Element).css("left", "100%");
            $(entry.Element).css("transform", "scale(" + entry.Scale + ", " + entry.Scale + ")");
        }
        $(entry.Element).css("color", entry.CommentColor);
        var mul = window.innerHeight / this.HEIGHT;
        if (entry.CommentSize == "big") {
            $(entry.Element).css("font-size", mul * this.BigCommentSize + "px");
        }
        else if (entry.CommentSize == "small") {
            $(entry.Element).css("font-size", mul * this.SmallCommentSize + "px");
        }
        else {
            $(entry.Element).css("font-size", mul * this.RegularCommentSize + "px");
        }
        $(entry.Element).css("opacity", entry.Opacity);
        this.Layer.appendChild(entry.Element);
        this.RenderingCommentList.push(entry);
        if (entry.Position != "naka") {
            var width = VideoViewModel.getVideoWidth();
            if (entry.Element.clientWidth > width) {
                var scale = width / entry.Element.clientWidth;
                $(entry.Element).css("transform", "translateX(-50%) scale(" + scale + ", " + scale + ")");
            }
        }
        if (entry.Position == "naka") {
            if (entry.Reverse) {
                $(entry.Element).keyframes({
                    "0%": {
                        left: (this.getX(entry, VideoViewModel.getVpos())) + "px"
                    },
                    "100%": {
                        left: (window.innerWidth) + "px"
                    },
                }, {
                    duration: this.getDuration(entry, VideoViewModel.getVpos()),
                    easing: "linear",
                    count: 1
                }, function () {
                    _this.RenderingCommentList.splice(_this.RenderingCommentList.indexOf(entry), 1);
                    _this.Layer.removeChild(entry.Element);
                });
            }
            else {
                $(entry.Element).keyframes({
                    "0%": {
                        left: this.getX(entry, VideoViewModel.getVpos()) + "px"
                    },
                    "100%": {
                        left: (-entry.Element.clientWidth) + "px"
                    },
                }, {
                    duration: this.getDuration(entry, VideoViewModel.getVpos()),
                    easing: "linear",
                    count: 1
                }, function () {
                    _this.RenderingCommentList.splice(_this.RenderingCommentList.indexOf(entry), 1);
                    _this.Layer.removeChild(entry.Element);
                });
            }
        }
        else {
            $(entry.Element).keyframes({
                "0%": {
                    opacity: 0
                },
                "2%, 98%": {
                    opacity: entry.Opacity
                },
                "100%": {
                    opacity: 0
                },
            }, {
                duration: this.getDuration(entry, VideoViewModel.getVpos()),
                easing: "linear",
                count: 1
            }, function () {
                _this.RenderingCommentList.splice(_this.RenderingCommentList.indexOf(entry), 1);
                _this.Layer.removeChild(entry.Element);
            });
        }
        if (VideoViewModel.Video.paused || VideoViewModel.Video.seeking || VideoViewModel.Video.ended) {
            $(entry.Element).css("animation-play-state", "paused");
        }
        $(entry.Element).css("top", this.getY(entry));
    };
    CommentViewModelImpl.prototype.getX = function (target, vpos) {
        if (target.Position != "naka") {
            return (window.innerWidth - target.Element.clientWidth) / 2;
        }
        var offset = vpos - target.Vpos;
        if (target.Reverse) {
            return -target.Element.clientWidth - offset * ((window.innerWidth - target.Element.clientWidth) / (target.Vend - target.Vpos));
        }
        else {
            return window.innerWidth - offset * ((window.innerWidth + target.Element.clientWidth) / (target.Vend - target.Vpos));
        }
    };
    CommentViewModelImpl.prototype.getY = function (target) {
        var offsetY = 0;
        if (target.Position == "shita") {
            offsetY = window.innerHeight - target.Element.clientHeight;
        }
        var flag = false;
        do {
            flag = false;
            for (var _i = 0, _a = this.RenderingCommentList; _i < _a.length; _i++) {
                var entry = _a[_i];
                if (target.Number == entry.Number || target.IsUploader != entry.IsUploader) {
                    continue;
                }
                if (target.Position == entry.Position) {
                    if (entry.getTop() + entry.Element.clientHeight > offsetY) {
                        if (offsetY + target.Element.clientHeight > entry.getTop()) {
                            if (target.Position == "shita") {
                                offsetY = entry.getTop() - target.Element.clientHeight - 1;
                                if (offsetY < 0) {
                                    offsetY = Math.random() * (window.innerHeight - target.Element.clientHeight);
                                    break;
                                }
                                flag = true;
                                break;
                            }
                            if (target.Position == "ue") {
                                offsetY = entry.getTop() + entry.Element.clientHeight + 1;
                                if (offsetY + target.Element.clientHeight > window.innerHeight) {
                                    offsetY = Math.random() * (window.innerHeight - target.Element.clientHeight);
                                    break;
                                }
                                flag = true;
                                break;
                            }
                            var max = Math.max(target.Vpos, entry.Vpos);
                            var min = Math.min(target.Vend, entry.Vend);
                            var x1 = this.getX(target, max);
                            var x2 = this.getX(target, min);
                            var x3 = this.getX(entry, max);
                            var x4 = this.getX(entry, min);
                            if (x1 <= x3 + entry.Element.clientWidth && x3 <= x1 + target.Element.clientWidth || x2 <= x4 + entry.Element.clientWidth && x4 <= x2 + target.Element.clientWidth) {
                                offsetY = entry.getTop() + entry.Element.clientHeight + 1;
                                if (offsetY + target.Element.clientHeight > window.innerHeight) {
                                    offsetY = Math.random() * window.innerHeight - target.Element.clientHeight;
                                    break;
                                }
                                flag = true;
                                break;
                            }
                        }
                    }
                }
            }
        } while (flag);
        return offsetY;
    };
    CommentViewModelImpl.prototype.getDuration = function (target, vpos) {
        var offset = 1 - (vpos - target.Vpos) / (target.Vend - target.Vpos);
        return target.Duration * offset;
    };
    CommentViewModelImpl.prototype.pauseComment = function () {
        for (var _i = 0, _a = this.RenderingCommentList; _i < _a.length; _i++) {
            var entry = _a[_i];
            $(entry.Element).css("animation-play-state", "paused");
        }
    };
    CommentViewModelImpl.prototype.resumeComment = function () {
        for (var _i = 0, _a = this.RenderingCommentList; _i < _a.length; _i++) {
            var entry = _a[_i];
            $(entry.Element).css("animation-play-state", "running");
        }
    };
    CommentViewModelImpl.prototype.purgeComment = function () {
        this.RenderingCommentList.length = 0;
        $(this.Layer).empty();
    };
    CommentViewModelImpl.prototype.showComment = function () {
        $(this.Layer).css("visibility", "visible");
    };
    CommentViewModelImpl.prototype.hideComment = function () {
        $(this.Layer).css("visibility", "hidden");
    };
    CommentViewModelImpl.prototype.calcCommentSize = function (width, height) {
        var _this = this;
        var source = this.RenderingCommentList.concat();
        $(this.Layer).empty();
        this.RenderingCommentList.length = 0;
        source.forEach(function (entry) {
            _this.dispatchComment(JSON.stringify(entry));
        });
    };
    CommentViewModelImpl.prototype.setOpacity = function (opacity) {
        for (var _i = 0, _a = this.RenderingCommentList; _i < _a.length; _i++) {
            var entry = _a[_i];
            entry.Opacity = opacity;
            $(entry.Element).css("opacity", opacity);
        }
    };
    CommentViewModelImpl.prototype.setBaseSize = function (str) {
        switch (str) {
            case "極小":
                this.RegularCommentSize = 14;
                break;
            case "小":
                this.RegularCommentSize = 18;
                break;
            case "標準":
                this.RegularCommentSize = 24;
                break;
            case "大":
                this.RegularCommentSize = 30;
                break;
        }
        this.BigCommentSize = this.RegularCommentSize + 15;
        this.SmallCommentSize = this.RegularCommentSize - 9;
        this.calcCommentSize(window.innerWidth, window.innerHeight);
    };
    return CommentViewModelImpl;
}());
var CommentViewModel = new CommentViewModelImpl();
function CommentViewModel$initialize() {
    CommentViewModel.initialize();
}
function CommentViewModel$dispatch(json) {
    CommentViewModel.dispatchComment(json);
}
function CommentViewModel$hide_comment() {
    CommentViewModel.hideComment();
}
function CommentViewModel$show_comment() {
    CommentViewModel.showComment();
}
function CommentViewModel$set_opacity(op) {
    CommentViewModel.setOpacity(op);
}
function CommentViewModel$setbasesize(str) {
    CommentViewModel.setBaseSize(str);
}
function VideoViewModel$initialize(src, initialPos, autoplay) {
    VideoViewModel.initialize(src, initialPos, autoplay);
}
function VideoViewModel$seek(pos) {
    VideoViewModel.seek(pos);
}
function VideoViewModel$pause() {
    VideoViewModel.pause();
}
function VideoViewModel$play() {
    VideoViewModel.play();
}
function VideoViewModel$setvolume(vol) {
    VideoViewModel.setVolume(vol);
}
function VideoViewModel$setrate(rate) {
    VideoViewModel.setRate(rate);
}
