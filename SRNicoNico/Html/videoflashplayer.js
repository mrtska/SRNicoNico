
function invoke_host(cmd, args) {

    window.external.InvokeFromJavaScript(cmd, args);
}


function readya() {

    invoke_host("log", "ready");
}
//containsを追加する 無いと困る
(function () {
    if (!("contains" in String.prototype)) {
        String.prototype.contains = function (str, startIndex) {
            "use strict";
            return -1 !== String.prototype.indexOf.call(this, str, startIndex);
        };
    }

})();



var VideoViewModelImpl = function() { };
VideoViewModelImpl.prototype = {

    WIDTH: 640,
    HEIGHT: 360,

    video: {},

    initialize: function (src) {
        

        //html5のvideo要素を取得
        this.video = window.player;

        //動画の高さをウィンドウの高さに合わせる
        this.video.style.height = window.innerHeight + "px";

        //動画のURLを設定
        this.video.AsOpenVideo(src, "");


        var v = this.video;
        //ウィンドウサイズが変わったら動画の高さやコメントのサイズを計算しなおす
        window.onresize = function (e) {

            //動画の高さを現在のウィンドウの高さに
            v.style.height = window.innerHeight + "px";

            CommentViewModel.calc_comment_size(window.innerWidth, window.innerHeight);

        };

        //クリックして一時停止をハンドリングするために
        window.onclick = function () {

            invoke_host("click");
        }
        window.onmousewheel = function (e) {

            invoke_host("mousewheel", e.wheelDelta);
        }

    },

    video_tick: function(time, vpos, buffer) {
      
        //C#側に渡すパラメータ
        var obj = {};

        //コメント描画タイミングのvposや現在の再生時間をobjに詰め込む
        obj.time = time;
        obj.vpos = vpos;

        //コメントを描画する
        CommentViewModel.comment_tick(obj.vpos);

        //objをJsonに変換してC#側に渡す
        invoke_host("currenttime", JSON.stringify(obj));
    },


    seek: function (pos) {

        this.video.AsSeek(pos);
    },
    pause: function () {

        CommentViewModel.pause_comment();
        this.video.AsPause();
    },
    resume: function () {

        CommentViewModel.resume_comment();
        this.video.AsResume();
    },
    setvolume: function (vol) {
        
        this.video.AsSetVolume(vol);
    }

};

var VideoViewModel = new VideoViewModelImpl();

function VideoViewModel$initialize(src) {

    VideoViewModel.initialize(src);
}

function VideoViewModel$seek(pos) {

    VideoViewModel.seek(pos);
}

function VideoViewModel$pause() {

    VideoViewModel.pause();
}

function VideoViewModel$resume() {

    VideoViewModel.resume();
}

function VideoViewModel$setvolume(vol) {

    VideoViewModel.setvolume(vol);
}