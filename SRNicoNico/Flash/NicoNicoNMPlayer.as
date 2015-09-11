package  {
	
	
	import flash.display.Loader;
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.media.SoundTransform;
	import flash.net.URLRequest;
	import flash.system.fscommand;
	
	import org.libspark.utils.ForcibleLoader;
	
	[SWF(width="672", height="384")]
	public class NicoNicoNMPlayer extends NicoNicoPlayerBase {
		
		//ストリーミングURL そのまんま
		private var videoUrl:String;
		
		
		private var loader:Loader = new Loader();

		private var movie:MovieClip;
		
		//コンストラクタ
		public function NicoNicoNMPlayer() {
			
			super();
			
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
		public override function OpenVideo(videoUrl:String):void {
			
			var req:URLRequest = new URLRequest(videoUrl);
			
			loader.contentLoaderInfo.addEventListener(Event.INIT, onInit);
			var fLoader:ForcibleLoader = new ForcibleLoader(loader);
			fLoader.load(req);
			
		}
		
		//そのまんま
		public override function Pause():void {
			
			if(movie) {
				
				movie.stop();
			}
		}
		public override function Resume():void {
			
			if(movie) {
				
				movie.play();
			}
		}
		public override function Seek(pos:Number):void {
			
			if(movie) {
				
				movie.gotoAndPlay(pos * movie.loaderInfo.frameRate);
			}
		}
		
		public override function ChangeVolume(vol:Number):void {
			
			if(movie) {
				
				movie.soundTransform = new SoundTransform(vol);
			}
		}
		
		private function onInit(e:Event):void {
			
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