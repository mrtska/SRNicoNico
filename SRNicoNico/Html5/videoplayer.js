

function invoke_host(cmd, args) {

    window.external.InvokeFromJavaScript(cmd, args);
}


window.onresize = function (e) {

    
    var video = document.getElementById("player");

    video.style.height = window.innerHeight + "px";
}



function init(src) {

    var video = document.getElementById("player");

    video.style.height = window.innerHeight + "px";

    video.src = src;


    video.load();


    video.play();




}

function seek(pos) {

    var video = document.getElementById("player");

    video.currentTime = pos;



}

















