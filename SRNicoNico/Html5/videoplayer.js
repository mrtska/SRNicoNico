
function invoke_host(cmd, args) {

    window.external.InvokeFromJavaScript(cmd, args);
}


//containsを追加する 無いと困る
(function() {
    if (!("contains" in String.prototype)) {
        String.prototype.contains = function (str, startIndex) {
            "use strict";
            return -1 !== String.prototype.indexOf.call(this, str, startIndex);
        };
    }    
})();

var VideoViewModel = {};

VideoViewModel.prototype = {
    
    WIDTH: 640,
    HEIGHT: 360,
    video: {},
  
    initialize: function(src) {
        
        //html5のvideo要素を取得
        video = document.getElementById("player");

        //動画の高さをウィンドウの高さに合わせる
        video.style.height = window.innerHeight + "px";

        //動画のURLを設定
        video.src = src;

        //動画のメタデータロード後
        video.addEventListener("loadedmetadata", function () {

            //C#側に動画の長さを伝える
            invoke_host("duration", video.duration);
            
            //C#側に動画の解像度を伝える
            invoke_host("widtheight", video.videoWidth + "×" + video.videoHeight);

            //コメント描画や動画のバッファした部分をC#に送るメインループ
            setInterval(function () {

                //C#側に渡すパラメータ
                var obj = {};

                //コメント描画タイミングのvposや現在の再生時間をobjに詰め込む
                obj.vpos = Math.floor(video.currentTime * 100);
                obj.time = video.currentTime;

                //動画のバッファされた範囲をobjに突っ込む
                var buffer = [];
                for (var i = 0; i < video.buffered.length; i++) {

                    var range = {
                        start: video.buffered.start(i),
                        end: video.buffered.end(i)
                    };
                    buffer.push(range);
                }
                obj.buffered = buffer;


                //再生した部分の範囲をobjに突っ込む
                var played = [];
                for (var i = 0; i < video.played.length; i++) {

                    var range = {
                        start: video.played.start(i),
                        end: video.played.end(i)
                    };
                    played.push(range);
                }
                obj.played = played;

                //コメントを描画する
                comment_tick(obj.vpos);

                //objをJsonに変換してC#側に渡す
                var json = JSON.stringify(obj);
                invoke_host("currenttime", json);

            }, 10); //デシ秒毎に実行する
        });

        //再生されたらコメントの一時停止を解除してC#側に伝える
        video.addEventListener("play", function () {

            resume_comment();
            invoke_host("playstate", true);
        });
        
        //一時停止されたらコメントも一時停止してC#側に伝える
        video.addEventListener("pause", function () {

            pause_comment();
            invoke_host("playstate", false);
        });
        
        
        //ウィンドウサイズが変わったら動画の高さやコメントのサイズを計算しなおす
        window.addEventListener("onresize", function (e) {

            //動画の高さを現在のウィンドウの高さに
            video.style.height = window.innerHeight + "px";

            var actx = window.innerWidth / WIDTH;
            var acty = window.innerHeight / HEIGHT;

            calc_comment_size(window.innerWidth, window.innerHeight);


          //  invoke_host("log", "actx :" + actx + " acty:" + acty);

            //document.body.style["transform-origin"] = "";
            //document.body.style["transform"] = "scale(" + actx + "," + acty + ")";
            //document.body.style["zoom"] = (acty + actx) / 2;
        });

        
        //ロードして再生 勝手に再生しないようにとかするならここかな
        video.load();
        video.play();
    },
    seek: function(pos) {
        
        video.currentTime = pos;   
    },
    pause: function() {
        
        video.pause();
    },
    resume: function() {
        
        video.play();
    },
    setvolume: function(vol) {
        
        video.volume = vol;
    }
    
};
















