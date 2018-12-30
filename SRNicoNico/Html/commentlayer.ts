

declare class VideoViewModel {

    public Video: HTMLVideoElement;

    public getVpos(): number;
    public getVideoWidth(): number;
}

interface External {

    InvokeFromJavaScript(cmd: string, args: string | number): void;
}

function InvokeHost(cmd: string, args: any = ""): void {

    if (window.external != null && cmd != null) {

        window.external.InvokeFromJavaScript(cmd, args);
    }
}

namespace SRNicoNico.Comment {

    class CommentEntry {

        //コメント番号
        public Number: number;

        //投稿者コメントか
        public IsUploader: boolean;

        //コメント再生位置
        public Vpos: number;
        public Vend: number;

        //コメントの描画位置
        public Position: string;

        //コメントの色
        public CommentColor: string;

        //コメントの大きさ
        public CommentSize: string;

        //描画時間
        public Duration: number;

        //拡大率
        public Scale: number;

        //コメント
        public Content: string;

        //透明度
        public Opacity: number;

        public Reverse: boolean;

        //投稿直後か
        public JustPosted: boolean;

        //要素
        public Element: HTMLSpanElement;


        constructor(json: any) {

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

            this.Element = document.createElement("span");
            this.Element.innerText = this.Content;

            if (this.JustPosted) {

                $(this.Element).css("border", "solid 1px #FFFF00");
            }

            if (json.Font === "mincho") {

                $(this.Element).css("font-family", '"游明朝体", "游明朝", "Yu Mincho", YuMincho, yumincho, YuMin-Medium');
            } else {

                $(this.Element).css("font-family", '"游ゴシック体", "游ゴシック", "Yu Gothic", YuGothic, yugothic, YuGo-Medium');
            }
        }

        public getTop(): number {

            return parseInt($(this.Element).css("top"));
        }
    }
    export class CommentViewModel {

        WIDTH: number = 640;
        HEIGHT: number = 360;

        SmallCommentSize: number = 15;
        RegularCommentSize: number = 24;
        BigCommentSize: number = 39;

        Layer: HTMLDivElement;

        RenderingCommentList: Array<CommentEntry> = new Array<CommentEntry>();


        constructor() {

            //レイヤーを取得
            this.Layer = document.getElementById("commentlayer") as HTMLDivElement;

            let isRollOver = true;

            //カーソルを非表示にする処理 ここはFlashのほうが簡単だったかもね
            var hideFunc = function () {

                if (isRollOver) {

                    document.body.style["cursor"] = "none";
                    InvokeHost("hidecontroller");
                }
            }

            //
            let id = -1;

            $(document).mouseleave(function () {

                isRollOver = false;
                clearInterval(id);
                setTimeout(() => {
                    InvokeHost("hidecontroller");
                }, 3000);
            });

            var x = 0;
            var y = 0;

            $(document).mousemove(function (e) {

                //マウスカーソルの位置が変わっていなかったら
                if (x == e.pageX && y == e.pageY) {

                    return;
                }

                x = e.pageX;
                y = e.pageY;
                document.body.style["cursor"] = "default";

                clearInterval(id);
                id = setInterval(hideFunc, 1600);

                isRollOver = true;
                InvokeHost("showcontroller");
            });
        }

        public dispatchComment(jsonStr: string): void {

            let entry: CommentEntry = new CommentEntry(JSON.parse(jsonStr));

            //上コメント 流れるコメント以外は基本３秒表示
            if (entry.Position == "ue") {

                //left50%にtransformするとpostionがfixedでも中央に描画される
                $(entry.Element).css("left", "50%");
                $(entry.Element).css("transform", "translate(-50%, 0%)");

            } else if (entry.Position == "shita") {    //下コメント

                //上と同じ
                $(entry.Element).css("bottom", "0");
                $(entry.Element).css("left", "50%");
                $(entry.Element).css("transform", "translate(-50%, 0%)");
            } else {    //流れるコメント

                //流れるコメントの初期値は一番右
                $(entry.Element).css("left", "100%");
                $(entry.Element).css("transform", "scale(" + entry.Scale + ", " + entry.Scale + ")");
            }

            //デフォルトカラーは白
            $(entry.Element).css("color", entry.CommentColor);

            //現在の高さを基準の高さで割って係数をだす
            var mul = window.innerHeight / this.HEIGHT;

            //ここでコメントサイズの初期値をcssで指定する
            //ウィンドウのheightが変わるとfont-sizeもそれに応じて乗算される
            if (entry.CommentSize == "big") {

                $(entry.Element).css("font-size", mul * this.BigCommentSize + "px");
            } else if (entry.CommentSize == "small") {

                $(entry.Element).css("font-size", mul * this.SmallCommentSize + "px");
            } else {

                $(entry.Element).css("font-size", mul * this.RegularCommentSize + "px");
            }
            $(entry.Element).css("opacity", entry.Opacity);


            //コメントレイヤーに追加
            //先に追加しないとclientWidthなどが正しく取得できない そりゃそうだ
            entry.Element = this.Layer.appendChild(entry.Element) as HTMLSpanElement;

            //描画中リストに追加
            this.RenderingCommentList.push(entry);

            //はみ出る位置固定コメントは縮小させる
            if (entry.Position != "naka") {

                var width = GetVideo().getVideoWidth();
                if (entry.Element.clientWidth > width) {

                    var scale = width / entry.Element.clientWidth;
                    //拡大率を設定
                    $(entry.Element).css("transform", "translateX(-50%) scale(" + scale + ", " + scale + ")");
                }
            }

            //流れるコメントはアニメーションを追加
            if (entry.Position == "naka") {

                if (entry.Reverse) {
                    $(entry.Element).keyframes({
                        "0%": {

                            //初期値をgetXで取得する しないとシークした時に全てのコメントが右から始まってしまって気持ち悪い
                            left: (this.getX(entry, GetVideo().getVpos())) + "px"
                        },
                        "100%": {

                            //終わりはコメントの横幅を-にしたもの つまりコメントが見えなくなるまで
                            left: (window.innerWidth) + "px"
                        },
                    }, {

                            duration: this.getDuration(entry, GetVideo().getVpos()),  //コメント表示時間 普通は4秒
                            easing: "linear",
                            count: 1    //ループされても困る
                        }, () => {

                            this.RenderingCommentList.splice(this.RenderingCommentList.indexOf(entry), 1);
                            this.Layer.removeChild(entry.Element);
                        });
                } else {

                    $(entry.Element).keyframes({
                        "0%": {

                            //初期値をgetXで取得する しないとシークした時に全てのコメントが右から始まってしまって気持ち悪い
                            left: this.getX(entry, GetVideo().getVpos()) + "px"
                        },
                        "100%": {

                            //終わりはコメントの横幅を-にしたもの つまりコメントが見えなくなるまで
                            left: (-entry.Element.clientWidth) + "px"
                        },
                    }, {

                            duration: this.getDuration(entry, GetVideo().getVpos()),  //コメント表示時間 普通は4秒
                            easing: "linear",
                            count: 1    //ループされても困る
                        }, () => {

                            this.RenderingCommentList.splice(this.RenderingCommentList.indexOf(entry), 1);
                            this.Layer.removeChild(entry.Element);
                        });
                }
            } else {

                $(entry.Element).keyframes({
                    "0%": {

                        opacity: GetVideo().Video.paused ? entry.Opacity : 0
                    },
                    "2%, 98%": {

                        opacity: entry.Opacity
                    },
                    "100%": {

                        opacity: 0
                    },
                }, {

                        duration: this.getDuration(entry, GetVideo().getVpos()) - 300,  //コメント表示時間
                        easing: "linear",
                        count: 1    //ループされても困る
                    }, () => {

                        this.RenderingCommentList.splice(this.RenderingCommentList.indexOf(entry), 1);
                        this.Layer.removeChild(entry.Element);
                    });
            }

            //一時停止したタイミングでコメント描画が始まるとコメントが動いてしまうので一時停止させる
            if (GetVideo().Video.paused || GetVideo().Video.seeking || GetVideo().Video.ended) {

                $(entry.Element).css("animation-play-state", "paused");
            }

            //Y座標を設定する
            $(entry.Element).css("top", this.getY(entry));
        }

        //現在のvposでtarget(p要素)がどのX座標(left)に居るべきかを取得する
        public getX(target: CommentEntry, vpos: number): number {

            //流れないコメントだったらX座標は一定
            if (target.Position != "naka") {

                return (window.innerWidth - target.Element.clientWidth) / 2;
            }

            //現在のvposとコメント表示開始時のvposのオフセットを取得する
            var offset = vpos - target.Vpos;

            //@逆 あたり
            if (target.Reverse) {

                //
                return -target.Element.clientWidth - offset * ((window.innerWidth - target.Element.clientWidth) / (target.Vend - target.Vpos));
            } else {

                //vpos当たりの横幅を計算してoffsetを掛けて出てきた値を現在のウィンドウの横幅から引く
                return window.innerWidth - offset * ((window.innerWidth + target.Element.clientWidth) / (target.Vend - target.Vpos));
            }
        }

        //現在描画中のコメントを考慮してtarget(p要素)がどのY座標で描画すれば良いか計算する
        public getY(target: CommentEntry): number {

            let offsetY: number = 0;

            //下コメだったらオフセットは下から上に行く
            if (target.Position == "shita") {

                offsetY = window.innerHeight - target.Element.clientHeight;
            }

            var flag = false;
            do {
                flag = false;

                //描画中のコメントの位置を考慮して返す座標を決める
                for (var entry of this.RenderingCommentList) {


                    //同じコメントナンバーだったらやり直し
                    if (target.Number == entry.Number && target.IsUploader == entry.IsUploader) {

                        continue;
                    }

                    //同じ描画位置同士でしか計算はしない
                    if (target.Position == entry.Position) {

                        //描画中のコメントの位置よりサジェストされたY座標が小さかったら
                        if (entry.getTop() + entry.Element.clientHeight > offsetY) {

                            //描画中のコメントの位置よりもサジェストされたY座標＋位置を決めたいコメントの高さが大きかったら
                            //入れる場所がないので流れるコメントは描画中のコメントのX座標も考慮して計算する
                            //上下コメントはどう頑張っても入らないのでMath.randomでテキトーな位置に描画する
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

            //ピクセルで出力する
            return offsetY;
        }

        public getDuration(target: CommentEntry, vpos: number): number {

            var offset = 1 - (vpos - target.Vpos) / (target.Vend - target.Vpos);
            return target.Duration * offset;
        }

        //コメント描画を一時停止
        public pauseComment(): void {

            for (let entry of this.RenderingCommentList) {

                $(entry.Element).css("animation-play-state", "paused");
            }
        }

        //コメント描画を再開
        public resumeComment(): void {

            for (let entry of this.RenderingCommentList) {

                $(entry.Element).css("animation-play-state", "running");
            }
        }

        //描画されているコメントを全て消す
        public purgeComment(): void {

            this.RenderingCommentList.length = 0;
            $(this.Layer).empty();
        }

        //コメントレイヤーを表示
        public showComment(): void {

            $(this.Layer).css("visibility", "visible");
        }

        //コメントレイヤーを非表示
        public hideComment(): void {

            $(this.Layer).css("visibility", "hidden");
        }

        public calcCommentSize(width: number, height: number): void {

            var source: Array<CommentEntry> = this.RenderingCommentList.concat();

            $(this.Layer).empty();
            this.RenderingCommentList.length = 0;

            source.forEach((entry) => {

                this.dispatchComment(JSON.stringify(entry));
            });

        }

        //透明度を設定
        public setOpacity(opacity: number): void {

            for (let entry of this.RenderingCommentList) {

                entry.Opacity = opacity;
                $(entry.Element).css("opacity", opacity);
            }
        }

        //コメントのデフォルトサイズを設定
        public setBaseSize(str: string): void {

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

        }
    }

    export var ViewModel: CommentViewModel
}

function Comment$Initialize() {

    SRNicoNico.Comment.ViewModel = new SRNicoNico.Comment.CommentViewModel();
}
eval("window.Comment$Initialize = Comment$Initialize;");


function Comment$Hide() {

    SRNicoNico.Comment.ViewModel.hideComment();
}
eval("window.Comment$Hide = Comment$Hide;");

function Comment$Show() {

    SRNicoNico.Comment.ViewModel.showComment();
}
eval("window.Comment$Show = Comment$Show;");

function Comment$Dispatch(json: any) {

    SRNicoNico.Comment.ViewModel.dispatchComment(json);
}
eval("window.Comment$Dispatch = Comment$Dispatch;");

function Comment$SetOpacity(op: number) {

    SRNicoNico.Comment.ViewModel.setOpacity(op);
}
eval("window.Comment$SetOpacity = Comment$SetOpacity;");

function Comment$SetBaseSize(str: string) {

    SRNicoNico.Comment.ViewModel.setBaseSize(str);
}
eval("window.Comment$SetBaseSize = Comment$SetBaseSize;");

function GetComment(): SRNicoNico.Comment.CommentViewModel {

    return SRNicoNico.Comment.ViewModel;
}
eval("window.GetComment = GetComment;");
