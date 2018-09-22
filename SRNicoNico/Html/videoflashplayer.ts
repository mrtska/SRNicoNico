
/// <reference path="jquery.d.ts" />



  class SWFVideoViewModelImpl {

    WIDTH: number = 640;
    HEIGHT: number = 360;

    public Video: any;

    public initialize(src:string, initialPos:number, autoplay:boolean) {

        //レイヤーを取得
        this.Video = document.getElementById("player") as HTMLObjectElement;

        //動画の高さをウィンドウの高さに合わせる
        this.Video.style.height = window.innerHeight + "px";
        this.Video.paused = false;
        this.Video.seeking = false;
        this.Video.ended = false;
        
        this.Video.AsOpenVideo(src, initialPos, autoplay);

        //ウィンドウサイズが変わったら動画の高さやコメントのサイズを計算しなおす
        window.onresize = (e) => {

            //動画の高さを現在のウィンドウの高さに
            this.Video.style.height = window.innerHeight + "px";

            GetComment().calcCommentSize(window.innerWidth, window.innerHeight);
        };

        //クリックして一時停止をハンドリングするために
        window.onmousedown = function (e) {

            //左クリックのみ
            if (e.button == 0) {

                InvokeHost("click");
            }
        }
        window.onmousewheel = function (e) {

            InvokeHost("mousewheel", e.wheelDelta);
        }
    }
    
    public videoloop(time:number, vpos:number, buffer:number) {

        //C#側に渡すパラメータ
        var obj:any = {};

        //コメント描画タイミングのvposや現在の再生時間をobjに詰め込む
        obj.vpos = vpos;
        obj.time = time;

        //動画のバッファされた範囲をobjに突っ込む
        var bufferrange = [];

        var brange = {
            start: 0,
            end: buffer
        };
        bufferrange.push(brange);
        obj.buffered = bufferrange;


        //再生した部分の範囲をobjに突っ込む
        var played = [];

        var prange = {
            start: 0,
            end: time
        };
        played.push(prange);
        obj.played = played;

        //objをJsonに変換してC#側に渡す
        var json = JSON.stringify(obj);
        InvokeHost("currenttime", json);
        
    }
    //動画シーク
    public seek(pos:number):void {

        if(this.Video) {

            this.Video.AsSeek(pos);
        }
    }
    //一時停止
    public pause():void {

        if(this.Video) {

            this.Video.paused = true;
            this.Video.AsPause();
        }
    }
    //再開
    public play() {

        if(this.Video) {

            this.Video.paused = false;
            this.Video.ended = false;
            this.Video.AsPlay();
        }
    }
    //音量調整
    public setVolume(volume:number):void {

        if(this.Video) {

            this.Video.AsSetVolume(volume);
        }
    }
    //現在のVposを取得
    public getVpos():number {

        if(this.Video) {

            return this.Video.AsCurrentTime() * 100;
        }
        return 0;
    }
    //動画の横幅を返す 動画データではなくobjectタグの横幅
    public getVideoWidth(): number {

        return this.Video.clientWidth;
    }
}











