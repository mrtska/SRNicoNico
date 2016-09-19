


var video;

function invoke_host(cmd, args) {

    window.external.InvokeFromJavaScript(cmd, args);
}

function invoke_host4(cmd, args0, args1, args2, args3) {

    window.external.InvokeFromJavaScript(cmd, args0, args1, args2, args3);
}

var WIDTH = 640;
var HEIGHT = 360;


window.onresize = function (e) {

    
    video.style.height = window.innerHeight + "px";

    var actx = window.innerWidth / WIDTH;
    var acty = window.innerHeight / HEIGHT;

    calc_comment_size(window.innerWidth, window.innerHeight);
    

  //  invoke_host("log", "actx :" + actx + " acty:" + acty);

    //document.body.style["transform-origin"] = "";
    //document.body.style["transform"] = "scale(" + actx + "," + acty + ")";
    //document.body.style["zoom"] = (acty + actx) / 2;
}



function init(src) {

    if (!('contains' in String.prototype)) {
        String.prototype.contains = function (str, startIndex) {
            "use strict";
            return -1 !== String.prototype.indexOf.call(this, str, startIndex);
        };
    }

    video = document.getElementById("player");

    video.style.height = window.innerHeight + "px";

    video.src = src;

    video.addEventListener("loadedmetadata", function () {

        invoke_host("duration", video.duration);
        invoke_host("widtheight", video.videoWidth + "×" + video.videoHeight);

        setInterval(function () {



            var obj = {};
            
            obj.vpos = Math.floor(video.currentTime * 100);
            obj.time = video.currentTime;

            var buffer = [];

            for (var i = 0; i < video.buffered.length; i++) {

                var range = {
                    start: video.buffered.start(i),
                    end: video.buffered.end(i)
                };

                buffer.push(range);

            }

            obj.buffered = buffer;


            var played = [];

            for (var i = 0; i < video.played.length; i++) {

                var range = {
                    start: video.played.start(i),
                    end: video.played.end(i)
                };

                played.push(range);
            }
            
            comment_tick(obj.vpos);

            obj.played = played;

            var json = JSON.stringify(obj);

            invoke_host("currenttime", json);

        }, 10);
    });

    video.addEventListener("play", function () {

        resume_comment();
        invoke_host("playstate", true);
    });
    video.addEventListener("pause", function () {

        pause_comment();
        invoke_host("playstate", false);
    });


    video.load();


    video.play();



}

function seek(pos) {

    video.currentTime = pos;
}

function pause() {

    video.pause();
}

function resume() {

    video.play();
}

function setvolume(vol) {

    video.volume = vol;
    
}















