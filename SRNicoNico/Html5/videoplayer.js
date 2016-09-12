

var mediaSource = new MediaSource();

function init() {

    var video = document.getElementById("player");

    video.src = window.URL.createObjectURL(mediaSource);


    mediaSource.addEventListener("sourceopen", function () {

        var source = mediaSource.addSourceBuffer("video/mp4");

    }, false);


    video.load();  

    //video.play();




}





















