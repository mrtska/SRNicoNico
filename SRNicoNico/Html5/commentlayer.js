


function invoke_host(cmd, args) {

    window.external.InvokeFromJavaScript(cmd, args);
}


var layer;

var listener_comment = [];

var rendering_listener_comment = [];


var niconico_color_map = {};
function comment_color_init() {

    //---全会員共通---
    niconico_color_map["white"] = "#FFFFFF";
    niconico_color_map["red"] = "#FF0000";
    niconico_color_map["pink"] = "#FF8080";
    niconico_color_map["orange"] = "#FFC000";
    niconico_color_map["yellow"] = "#FFFF00";
    niconico_color_map["green"] = "#00FF00";
    niconico_color_map["cyan"] = "#00FFFF";
    niconico_color_map["blue"] = "#0000FF";
    niconico_color_map["purple"] = "#C000FF";
    niconico_color_map["black"] = "#000000";
    //------

    //---プレミアム会員のみ---
    niconico_color_map["white2"] = "#CCCC99";
    niconico_color_map["niconicowhite"] = niconico_color_map["white2"];

    niconico_color_map["red2"] = "#CC0033";
    niconico_color_map["truered"] = niconico_color_map["red2"];

    niconico_color_map["pink2"] = "#FF33CC";

    niconico_color_map["orange2"] = "#FF6600";
    niconico_color_map["passionorange"] = niconico_color_map["orange2"];

    niconico_color_map["yellow2"] = "#999900";
    niconico_color_map["madyellow"] = niconico_color_map["yellow2"];

    niconico_color_map["green2"] = "#00CC66";
    niconico_color_map["elementalgreen"] = niconico_color_map["green2"];

    niconico_color_map["cyan2"] = "#00CCCC";

    niconico_color_map["blue2"] = "#3399FF";
    niconico_color_map["marineblue"] = niconico_color_map["blue2"];

    niconico_color_map["purple2"] = "#6633CC";
    niconico_color_map["nobleviolet"] = niconico_color_map["purple2"];

    niconico_color_map["black2"] = "#666666";

}

function calc_comment_size(width, height) {


    var mul = height / 360;
    
    listener_comment.forEach(function (target) {


        if(target.size == "big") {



            $(target).css("font-size", mul * 39);

        } else if(target.size == "small") {


            $(target).css("font-size", mul * 15);
        } else {

        $(target).css("font-size", mul * 24);
        }
    });
}

function resume_comment() {

    rendering_listener_comment.forEach(function (target) {

        $(target).css("animation-play-state", "running");
    });
}

function pause_comment() {


    rendering_listener_comment.forEach(function (target) {

        $(target).css("animation-play-state", "paused");


    });

}



function comment_init(json) {

    comment_color_init();
    
    layer = document.getElementById("commentlayer");

    var obj = JSON.parse(json);




    obj.array.forEach(function (val) {

        var element = document.createElement("p");
        element.innerText = val.Content;
        element.vpos = parseInt(val.Vpos);
        element.no = val.No;
        
        var mail = String(val.Mail);

        if (mail.contains("ue")) {

            element.vend = element.vpos + 300;
            element.duration = 300;
            element.pos = "ue";
            element.y = 0;
            $(element).css("text-align", "center");
            $(element).css("left", "50%");
            $(element).css("transform", "translate(-50%, 0%)");


        } else if (mail.contains("shita")) {


            element.vend = element.vpos + 300;
            element.duration = 300;
            element.pos = "shita";
            element.y = 92;
            $(element).css("text-align", "center");
            $(element).css("bottom", "0");
            $(element).css("left", "50%");
            $(element).css("transform", "translate(-50%, 0%)");


        } else {

            element.vend = element.vpos + 400;
            element.duration = 400;
            element.y = 0;
            
            element.pos = "naka";

            $(element).css("position", "fixed");
            $(element).css("left", "100%");

        }
        

        $(element).css("color", "#FFF");

        for (var key in niconico_color_map) {

            if (mail.contains(key)) {

                $(element).css("color", niconico_color_map[key]);
            }
        }

        if (mail.contains("#")) {

                
        }

        if (mail.contains("big")) {

            element.size = "big";

            $(element).css("font-size", "39px");
        } else if (mail.contains("small")) {

            element.size = "small";
            $(element).css("font-size", "14px");
        } else {

            element.size = "medium";
            $(element).css("font-size", "24px");

        }



        //$(element).css("transform   ", "translate(-50%, -50%)");
        listener_comment.push(element);

    });

}

function uploader_comment_init(json) {

    
    var obj = JSON.parse(json);


    obj.array.forEach(function (val) {

        var element = document.createElement("p");
        element.innerText = val.Content;
        element.vpos = parseInt(val.Vpos);
        element.no = val.No;
        
        var mail = String(val.Mail);

        if (mail.contains("ue")) {

            element.vend = element.vpos + 300;
            element.duration = 300;
            element.pos = "ue";
            element.y = 0;
            $(element).css("text-align", "center");
            $(element).css("left", "50%");
            $(element).css("transform", "translate(-50%, 0%)");


        } else if (mail.contains("shita")) {


            element.vend = element.vpos + 300;
            element.duration = 300;
            element.pos = "shita";
            element.y = 92;
            $(element).css("text-align", "center");
            $(element).css("bottom", "0");
            $(element).css("left", "50%");
            $(element).css("transform", "translate(-50%, 0%)");


        } else {

            element.vend = element.vpos + 400;
            element.duration = 400;
            element.y = 0;
            
            element.pos = "naka";

            $(element).css("position", "fixed");
            $(element).css("left", "100%");

        }
        

        $(element).css("color", "#FFF");

        for (var key in niconico_color_map) {

            if (mail.contains(key)) {

                $(element).css("color", niconico_color_map[key]);
            }
        }

        if (mail.contains("#")) {

                
        }

        if (mail.contains("big")) {

            element.size = "big";

            $(element).css("font-size", "39px");
        } else if (mail.contains("small")) {

            element.size = "small";
            $(element).css("font-size", "14px");
        } else {

            element.size = "medium";
            $(element).css("font-size", "24px");

        }



        //$(element).css("transform   ", "translate(-50%, -50%)");
        listener_comment.push(element);

    });
}


function getTop(val) {

    return parseInt($(val).css("top"));
}

//流れる系のコメント用
function getX(target, vpos) {

    if(target.pos != "naka") {

        return (window.innerWidth - target.clientWidth) / 2;
    }

    var sub = vpos - target.vpos;
    var ret = (window.innerWidth + target.clientWidth) / (target.vend - target.vpos);

    //invoke_host("log", "hage" + window.innerWidth - sub * ret);
    return window.innerWidth - sub * ret;
}

function getY(target) {


    var offsetY = 0;

    //下コメだったら
    if(target.pos == "shita") {

        offsetY = window.innerHeight - target.clientHeight;

        //invoke_host("log", offsetY);

    }

    var flag = false;
    do {


        flag = false;
        var count = 0;
            
        for (var i = 0; i < rendering_listener_comment.length; i++) {

            var entry = rendering_listener_comment[i];

            if(target.no == entry.no) {

                continue;
            }

            //同じ描画位置
            if(target.pos == entry.pos) {

                if(getTop(entry) + entry.clientHeight > offsetY) {

            //invoke_host("log", "unko" + target.clientHeight);
                    if(offsetY + target.clientHeight > getTop(entry)) {


                        if(target.pos == "shita") {

                            offsetY = getTop(entry) - target.clientHeight - 1;
                            if(offsetY < 0) {

                                offsetY = Math.random() * (window.innerHeight - target.clientHeight);
                                break;
                            }
                            flag = true;
                            break;
                        }
                        
                        if(target.pos == "shita") {

                            offsetY = getTop(entry) - target.clientHeight + 1;

                            if(offsetY + target.clientHeight > window.innerHeight) {

                                offsetY = Math.random() * (window.innerHeight - target.clientHeight);
                                break;
                            }
                            flag = true;
                            break;
                        }

                        var max = Math.max(target.vpos, entry.vpos);
                        var min = Math.min(target.vend, entry.vend);
                        var x1 = getX(target, max);
                        var x2 = getX(target, min);
                        var x3 = getX(entry, max);
                        var x4 = getX(entry, min);


                        if(x1 <= x3 + entry.clientWidth && x3 <= x1 + target.clientWidth || x2 <= x4 + entry.clientWidth && x4 <= x2 + target.clientWidth) {
                            
                            offsetY = getTop(entry) + entry.clientHeight + 1;
                          
                            if(offsetY + target.clientHeight > window.innerHeight) {
                                
                                offsetY = Math.random() * window.innerHeight - target.clientHeight;
                                break;
                            }

                            flag = true;
                            break;
                        }

                    }

                }

            }
        } 
        
    } while(flag);



    //invoke_host("log", "result:" + offsetY);
    return offsetY + "px";
}

function comment_tick(vpos) {


   // invoke_host("log", vpos);
    listener_comment.forEach(function (target) {

        if (vpos >= target.vpos && vpos < target.vend) {

            

            if (rendering_listener_comment.indexOf(target) >= 0) {

            } else {

                layer.appendChild(target);

                rendering_listener_comment.push(target);

                if (target.pos == "naka") {

                    $(target).keyframes({
                        "0%": {

                            left: getX(target, vpos) + "px"
                        },
                        "100%": {

                            left: -target.clientWidth + "px"

                        }, 

                    }, {

                        duration: target.duration * 10,
                        easing: "linear",
                        count: 1
                    });
                }


                $(target).css("top", getY(target));

            }
        }
      
    });

    rendering_listener_comment.forEach(function (target) {
       // invoke_host("log", target.vend);
        if (vpos < target.vpos || vpos > target.vend) {

            if (rendering_listener_comment.indexOf(target) >= 0) {

                layer.removeChild(target);
                rendering_listener_comment.splice(rendering_listener_comment.indexOf(target), 1);
            }
        }
    });


}




















