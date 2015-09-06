package  {
	
	
	import flash.display.Loader;
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.display.StageAlign;
	import flash.display.StageScaleMode;
	import flash.events.AsyncErrorEvent;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.events.NetStatusEvent;
	import flash.external.ExternalInterface;
	import flash.geom.Rectangle;
	import flash.media.StageVideo;
	import flash.media.Video;
	import flash.net.NetConnection;
	import flash.net.NetStream;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.system.fscommand;
	import flash.text.TextFormat;
	
	import org.libspark.utils.ForcibleLoader;
	
	[SWF(width="672", height="384")]
	public class NicoNicoNMPlayer extends Sprite {
		
		//ストリーミングURL そのまんま
		private var videoUrl:String;
		
		//ストリーミングするためのやつ こんな感じのやつがC#とかでも使えればいいのにな
		

		
		private var loader:Loader = new Loader();

		private var movie:MovieClip;
		
		//コメントラスタライザ
		private var rastarizer:CommentRasterizer;
		
		//コンストラクタ
		public function NicoNicoNMPlayer() {
			
			stage.color = 0x000000;
			//枠に合わせてスケールする
			stage.scaleMode = StageScaleMode.SHOW_ALL;
			stage.align = StageAlign.TOP;
			
			//JSコールバック登録
			if(ExternalInterface.available) {
				
				//コールバック登録
				ExternalInterface.addCallback("AsOpenVideo", OpenVideo);
				ExternalInterface.addCallback("AsPause", Pause);
				ExternalInterface.addCallback("AsResume", Resume);
				ExternalInterface.addCallback("AsSeek", Seek);
				ExternalInterface.addCallback("AsInjectComment", InjectComment);
			}
			
			rastarizer = new CommentRasterizer();
			
			//OpenVideo("Z:/smile.swf");
			//OpenVideo("Z:/smile.mp4");
			//OpenVideo("Z:/smile (1).mp4");
			//var now:Date = new Date();
			//OpenVideo("http://mrtska.net/SRNicoNico/sm9?"+ now.time.toString());
			//OpenVideo("http://mrtska.net/SRNicoNico/sm8628149");
			//OpenVideo("http://mrtska.net/SRNicoNico/sm9");
			/*var loader:URLLoader = new URLLoader();
			var req:URLRequest = new URLRequest("Z:/msg.txt");
			
			loader.addEventListener(Event.COMPLETE, function(e:Event):void {
				
				InjectComment(loader.data);
			});
			loader.load(req);*/
			
			
		}
		
		//指定したURLをストリーミング再生する
		private function OpenVideo(videoUrl:String):void {
			
			var req:URLRequest = new URLRequest(videoUrl);
			
			loader.contentLoaderInfo.addEventListener(Event.INIT, onInit);
			var fLoader:ForcibleLoader = new ForcibleLoader(loader);
			fLoader.load(req);
			
		}
		
		//そのまんま
		private function Pause():void {
			
			movie.stop();
		}
		private function Resume():void {
			
			movie.play();
		}
		private function Seek(pos:Number):void {
			
			movie.gotoAndPlay(pos);
		}
		private function InjectComment(json:String):void {
			
			rastarizer.load(json);
		}

		
		private function onInit(e:Event):void {
			
			trace(e);
			addChild(loader);
			
			movie = loader.content as MovieClip;
			rastarizer.updateBounds(stage.stageWidth, stage.stageHeight);
			addChild(rastarizer);
			addEventListener(Event.ENTER_FRAME, onFrame);
		}
		
		
		private var prevTime:int = 0;
		private var prevLoaded:uint = 0;
		
		private function onFrame(e:Event):void {
			
			// 再生時間を取得
			var frame:int = movie.currentFrame;
			var value:Number = (frame / movie.loaderInfo.frameRate);
			var vpos:Number = Math.floor(value * 100);
			
			var time:int = value;
			
			// バッファの計算
			var buffer:Number = (loader.contentLoaderInfo.bytesLoaded) / (loader.contentLoaderInfo.bytesTotal);
			
			trace("Time:" + time + " vpos:" + vpos + " buffer:" + buffer);
			
			if(prevTime != (int)(value)) {
				
				fscommand("CsFrame", time + ":" + buffer.toString() + ":" + (loader.contentLoaderInfo.bytesLoaded - prevLoaded).toString());
				prevLoaded = loader.contentLoaderInfo.bytesLoaded;
			}
			prevTime = (int) (value);
			
			
			
			rastarizer.render(vpos);
			//trace("value:" + value + " diff:" + this.diff);
		}
		
		
	}
}