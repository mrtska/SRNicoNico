
function invoke_host(cmd, args) {

    window.external.InvokeFromJavaScript(cmd, args);
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
        this.video = document.getElementById("player");

        //動画の高さをウィンドウの高さに合わせる
        this.video.style.height = window.innerHeight + "px";

        //動画のURLを設定
        this.video.src = src;

        //動画のメタデータロード後
        this.video.addEventListener("loadedmetadata", function (e) {

            //C#側に動画の長さを伝える
            //invoke_host("duration", e.target.duration);

            //C#側に動画の解像度を伝える
            invoke_host("widtheight", e.target.videoWidth + "×" + e.target.videoHeight);

            //コメント描画や動画のバッファした部分をC#に送るメインループ
            setInterval(function () {

                //C#側に渡すパラメータ
                var obj = {};

                //コメント描画タイミングのvposや現在の再生時間をobjに詰め込む
                obj.vpos = Math.floor(e.target.currentTime * 100);
                obj.time = e.target.currentTime;

                //動画のバッファされた範囲をobjに突っ込む
                var buffer = [];
                for (var i = 0; i < e.target.buffered.length; i++) {

                    var range = {
                        start: e.target.buffered.start(i),
                        end: e.target.buffered.end(i)
                    };
                    buffer.push(range);
                }
                obj.buffered = buffer;


                //再生した部分の範囲をobjに突っ込む
                var played = [];
                for (var i = 0; i < e.target.played.length; i++) {

                    var range = {
                        start: e.target.played.start(i),
                        end: e.target.played.end(i)
                    };
                    played.push(range);
                }
                obj.played = played;

                //コメントを描画する
                CommentViewModel.comment_tick(obj.vpos);

                //objをJsonに変換してC#側に渡す
                var json = JSON.stringify(obj);
                invoke_host("currenttime", json);

            }, 10); //デシ秒毎に実行する
        });

        //再生されたらコメントの一時停止を解除してC#側に伝える
        this.video.addEventListener("play", function () {

            CommentViewModel.resume_comment();
            invoke_host("playstate", true);
        });

        //一時停止されたらコメントも一時停止してC#側に伝える
        this.video.addEventListener("pause", function () {

            CommentViewModel.pause_comment();
            invoke_host("playstate", false);
        });

        this.video.addEventListener("ended", function () {

            CommentViewModel.pause_comment();
            invoke_host("playstate", false);
            invoke_host("ended");
        });

        this.video.addEventListener("playing", function () {

            CommentViewModel.resume_comment();
            invoke_host("playstate", true);
            invoke_host("playing");
        });

        this.video.addEventListener("waiting", function () {

            CommentViewModel.pause_comment();
            invoke_host("waiting");
        });

        this.video.addEventListener("seeking", function () {

            CommentViewModel.pause_comment();
            invoke_host("seeking");
        });
        this.video.addEventListener("seeked", function (e) {

            if (!e.target.paused) {

                CommentViewModel.resume_comment();
                invoke_host("playstate", true);
            }

            invoke_host("seeked");
        });


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

        //ロードして再生 勝手に再生しないようにとかするならここかな
        this.video.load();
        this.video.play();
    },
    seek: function (pos) {

        this.video.currentTime = pos;
    },
    pause: function () {

        this.video.pause();
    },
    resume: function () {

        this.video.play();
    },
    setvolume: function (vol) {
        
        this.video.volume = vol;
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