
var CommentViewModelImpl = function() {};

CommentViewModelImpl.prototype = {

    //基準となる横縦幅
    WIDTH: 640,
    HEIGHT: 360,


    //ニコニコビルトインカラーのマップ
    niconico_color_map: {},

    //視聴者コメントのリスト
    listener_comment: [],

    //描画中の視聴者コメントのリスト
    rendering_listener_comment: [],

    //投稿者コメントのリスト
    owner_comment: [],

    //描画中の投稿者コメントのリスト
    rendering_owner_comment: [],

    //commentLayer div要素
    layer: {},

    //現在のコメント描画地点
    current_vpos: 0,

    //ニコニコで定義されているコメントカラーをJSで定義する
    comment_color_init: function () {

        //---全会員共通---
        this.niconico_color_map["white"] = "#FFFFFF";
        this.niconico_color_map["red"] = "#FF0000";
        this.niconico_color_map["pink"] = "#FF8080";
        this.niconico_color_map["orange"] = "#FFC000";
        this.niconico_color_map["yellow"] = "#FFFF00";
        this.niconico_color_map["green"] = "#00FF00";
        this.niconico_color_map["cyan"] = "#00FFFF";
        this.niconico_color_map["blue"] = "#0000FF";
        this.niconico_color_map["purple"] = "#C000FF";
        this.niconico_color_map["black"] = "#000000";
        //------

        //---プレミアム会員のみ---
        this.niconico_color_map["white2"] = "#CCCC99";
        this.niconico_color_map["niconicowhite"] = this.niconico_color_map["white2"];

        this.niconico_color_map["red2"] = "#CC0033";
        this.niconico_color_map["truered"] = this.niconico_color_map["red2"];

        this.niconico_color_map["pink2"] = "#FF33CC";

        this.niconico_color_map["orange2"] = "#FF6600";
        this.niconico_color_map["passionorange"] = this.niconico_color_map["orange2"];

        this.niconico_color_map["yellow2"] = "#999900";
        this.niconico_color_map["madyellow"] = this.niconico_color_map["yellow2"];

        this.niconico_color_map["green2"] = "#00CC66";
        this.niconico_color_map["elementalgreen"] = this.niconico_color_map["green2"];

        this.niconico_color_map["cyan2"] = "#00CCCC";

        this.niconico_color_map["blue2"] = "#3399FF";
        this.niconico_color_map["marineblue"] = this.niconico_color_map["blue2"];

        this.niconico_color_map["purple2"] = "#6633CC";
        this.niconico_color_map["nobleviolet"] = this.niconico_color_map["purple2"];

        this.niconico_color_map["black2"] = "#666666";

    },

    //このメソッドはC#から呼ばれる jsonにはC#が取得したコメントがJsonになって入っている
    initialize: function (json) {

        //ニコニコビルトインカラー初期化
        this.comment_color_init();

        //コメントレイヤーのdivを取得
        this.layer = document.getElementById("commentlayer");

        //C#で取得したコメントをパース
        var obj = JSON.parse(json);

        //パースしたjsonのコメントリストをfor each
        for (var index in obj.array) {

            var val = obj.array[index];
            //コメント一つ一つをp要素で描画するのでp要素を作成
            var element = document.createElement("span");
            element.innerText = val.Content;

            //p要素にvposを突っ込む C#やJavaばかりやっていた私には驚き
            //val.VposはstringなのでparseIntしないとあとあと 100 + 400 = 100400 になったりする
            element.vpos = parseInt(val.Vpos);
            element.no = val.No;    //コメントナンバー 描画位置計算時に使う

            //コメントの装飾 なんでmailなんだ
            var mail = String(val.Mail);

            //上コメント 流れるコメント以外は基本３秒表示
            if (mail.contains("ue")) {

                element.vend = element.vpos + 300;
                element.duration = 3000;
                element.pos = "ue";

                //left50%にtransformするとpostionがfixedでも中央に描画される
                $(element).css("left", "50%");
                $(element).css("transform", "translate(-50%, 0%)");


            } else if (mail.contains("shita")) {    //下コメント


                element.vend = element.vpos + 300;
                element.duration = 3000;
                element.pos = "shita";

                //上と同じ
                $(element).css("bottom", "0");
                $(element).css("left", "50%");
                $(element).css("transform", "translate(-50%, 0%)");


            } else {    //流れるコメント

                //流れるコメントは4秒表示
                element.vend = element.vpos + 400;
                element.duration = 4000;
                element.pos = "naka";

                //流れるコメントの初期値は一番右
                $(element).css("left", "100%");
            }

            //デフォルトカラーは白
            $(element).css("color", "#FFF");

            //コメントのmailにニコニコのビルトインカラーが含まれていたらその色を適用する
            for (var key in this.niconico_color_map) {

                if (mail.contains(key)) {

                    $(element).css("color", this.niconico_color_map[key]);
                }
            }

            //ニコニコはコメントカラーを16進トリプレット表記でも指定できる
            if (mail.contains("#")) {


            }

            //ここでコメントサイズの初期値をcssで指定する
            //ウィンドウのheightが変わるとfont-sizeもそれに応じて乗算される
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

            //いろいろと設定をしたp要素をリストに入れておく
            this.listener_comment.push(element);
        }
    },

    //ウィンドウサイズ変更はフルスクリーンによってコメントサイズの再計算が必要になった時に呼ばれる
    calc_comment_size: function (width, height) {

        //現在の高さを基準の高さで割って係数をだす    
        var mul = height / this.HEIGHT;

        
        //全てのコメントに係数を掛けて反映させる
        for(var index in this.listener_comment) {

            var target = this.listener_comment[index];

            
            if (target.size == "big") {

                $(target).css("font-size", mul * 39);

            } else if (target.size == "small") {

                $(target).css("font-size", mul * 15);
            } else {

                $(target).css("font-size", mul * 24);
            }

        }
    
        //ホントは描画中のコメントに係数をかけたりして座標を再計算しないとなんだけど複雑すぎて分からんし
        //こうしてもう一度描画を最初からさせても大して重くなかったのでこの方法を採用
        this.rendering_listener_comment.length = 0;

    },
    //描画中のコメントを全て一時停止する
    pause_comment: function () {

        this.rendering_listener_comment.forEach(function (target) {

            $(target).css("animation-play-state", "paused");
        });
    },
    //一時停止中のコメントを全て再開させる
    resume_comment: function () {

        this.rendering_listener_comment.forEach(function (target) {

            $(target).css("animation-play-state", "running");
        });
    },

    //コメントのp要素から現在のy座標(top)を取得する
    getTop: function (val) {

        return parseInt($(val).css("top"));
    },

    //現在のvposでtarget(p要素)がどのX座標(left)に居るべきかを取得する
    getX: function (target, vpos) {

        //流れないコメントだったらX座標は一定
        if (target.pos != "naka") {

            return (window.innerWidth - target.clientWidth) / 2;
        }

        //現在のvposとコメント表示開始時のvposのオフセットを取得する
        var offset = vpos - target.vpos;


        //vpos当たりの横幅を計算してoffsetを掛けて出てきた値を現在のウィンドウの横幅から引く
        var ret = window.innerWidth - offset * ((window.innerWidth + target.clientWidth) / (target.vend - target.vpos))

        return ret;
    },

    //現在描画中のコメントを考慮してtarget(p要素)がどのY座標で描画すれば良いか計算する
    getY: function (target) {


        var offsetY = 0;

        //下コメだったらオフセットは下から上に行く
        if (target.pos == "shita") {

            offsetY = window.innerHeight - target.clientHeight;

            //invoke_host("log", offsetY);

        }

        var flag = false;
        do {


            flag = false;

            //描画中のコメントの位置を考慮して返す座標を決める
            for (var i = 0; i < this.rendering_listener_comment.length; i++) {

                var entry = this.rendering_listener_comment[i];

                //同じコメントナンバーだったらやり直し
                if (target.no == entry.no) {

                    continue;
                }

                //同じ描画位置同士でしか計算はしない
                if (target.pos == entry.pos) {

                    //描画中のコメントの位置よりサジェストされたY座標が小さかったら
                    if (this.getTop(entry) + entry.clientHeight > offsetY) {

                        //描画中のコメントの位置よりもサジェストされたY座標＋位置を決めたいコメントの高さが大きかったら
                        //入れる場所がないので流れるコメントは描画中のコメントのX座標も考慮して計算する
                        //上下コメントはどう頑張っても入らないのでMath.randomでテキトーな位置に描画する
                        if (offsetY + target.clientHeight > this.getTop(entry)) {


                            if (target.pos == "shita") {

                                offsetY = this.getTop(entry) - target.clientHeight - 1;
                                if (offsetY < 0) {

                                    offsetY = Math.random() * (window.innerHeight - target.clientHeight);
                                    break;
                                }
                                flag = true;
                                break;
                            }

                            if (target.pos == "ue") {

                                offsetY = this.getTop(entry) + target.clientHeight + 1;

                                if (offsetY + target.clientHeight > window.innerHeight) {

                                    offsetY = Math.random() * (window.innerHeight - target.clientHeight);
                                    break;
                                }
                                flag = true;
                                break;
                            }

                            var max = Math.max(target.vpos, entry.vpos);
                            var min = Math.min(target.vend, entry.vend);
                            var x1 = this.getX(target, max);
                            var x2 = this.getX(target, min);
                            var x3 = this.getX(entry, max);
                            var x4 = this.getX(entry, min);


                            if (x1 <= x3 + entry.clientWidth && x3 <= x1 + target.clientWidth || x2 <= x4 + entry.clientWidth && x4 <= x2 + target.clientWidth) {

                                offsetY = this.getTop(entry) + entry.clientHeight + 1;

                                if (offsetY + target.clientHeight > window.innerHeight) {

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

        } while (flag);

        
        target.offsetYP = offsetY / window.innerHeight;
        //ピクセルで出力する
        return offsetY + "px";
    },

    //vposからあと何ミリ秒描画すればいいかを返す
    getDuration: function(target, vpos) {

        var offset = 1 - (vpos - target.vpos) / (target.vend - target.vpos);
        
        return target.duration * offset;
    },

    //VideoViewModelから1デシ秒に一回くらい呼ばれる
    comment_tick: function (vpos) {

        this.current_vpos = vpos;
        //コメントリストから該当の時間になったコメントをコメントレイヤーに追加していく
        for(var index in this.listener_comment) {

            var target = this.listener_comment[index];

            //該当時間かどうか
            if (vpos >= target.vpos && vpos < target.vend) {

                //既に追加してあったらスキップ    
                if (this.rendering_listener_comment.indexOf(target) >= 0) {

                } else {

                    //コメントレイヤーに追加
                    //先に追加しないとclientWidthなどが正しく取得できない
                    this.layer.appendChild(target);

                    //描画中リストに追加
                    this.rendering_listener_comment.push(target);

                    //流れるコメントはアニメーションを追加
                    if (target.pos == "naka") {

                        $(target).keyframes({
                            "0%": {

                                //初期値をgetXで取得する しないとシークした時に全てのコメントが右から始まってしまって気持ち悪い
                                left: this.getX(target, vpos) + "px"
                            },
                            "100%": {

                                //終わりはコメントの横幅を-にしたもの つまりコメントが見えなくなるまで
                                left: -target.clientWidth + "px"

                            },

                        }, {

                            duration: this.getDuration(target, vpos),  //コメント表示時間 普通は4秒
                            easing: "linear",
                            count: 1    //ループされても困る
                        });
                    }

                    //一時停止したタイミングでコメント描画が始まるとコメントが動いてしまうので一時停止させる
                    if (VideoViewModel.video.paused || VideoViewModel.video.seeking) {

                        $(target).css("animation-play-state", "paused");
                    }

                    //Y座標を設定する
                    $(target).css("top", this.getY(target));
                }
            }
        }

        //描画中リストに描画時間が過ぎたコメントがあったら排除する 重くなるからね   
        for (var index in this.rendering_listener_comment) {

            var target = this.rendering_listener_comment[index];

            if (vpos < target.vpos || vpos > target.vend) {

                this.layer.removeChild(target);
                this.rendering_listener_comment.splice(this.rendering_listener_comment.indexOf(target), 1);
                continue;
            }

        }
    },
    show_comment: function() {

        $(this.layer).css("visibility", "visible");
    },

    hide_comment: function () {

        $(this.layer).css("visibility", "hidden");
    }
};
var CommentViewModel = new CommentViewModelImpl();

function CommentViewModel$initialize(json) {

    CommentViewModel.initialize(json);
}
function CommentViewModel$hide_comment() {

    CommentViewModel.hide_comment();
}
function CommentViewModel$show_comment() {

    CommentViewModel.show_comment();
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

        for (var key in this.niconico_color_map) {

            if (mail.contains(key)) {

                $(element).css("color", this.niconico_color_map[key]);
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
        this.listener_comment.push(element);

    });
}




