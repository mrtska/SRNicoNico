package  {
	
	
	import flash.display.Loader;
	import flash.display.LoaderInfo;
	import flash.display.MovieClip;
	import flash.display.StageAlign;
	import flash.display.StageScaleMode;
	import flash.events.Event;
	import flash.events.TimerEvent;
	import flash.media.SoundTransform;
	import flash.net.URLRequest;
	import flash.external.ExternalInterface;
	import flash.system.Security;
	import org.libspark.utils.ForcibleLoader;
	
	[SWF(width="640", height="360")]
	public class NicoNicoNMPlayer extends NicoNicoPlayerBase {
		
		//ストリーミングURL そのまんま
		private var videoUrl:String;
		
		
		private var loader:Loader = new Loader();

		private var videoW:int;
		private	var videoH:int;
		private var movie:MovieClip;
		
		//コンストラクタ
		public function NicoNicoNMPlayer() {
			
			super();
			stage.align = StageAlign.TOP_LEFT;
			stage.scaleMode = StageScaleMode.NO_SCALE;
			
			stage.addEventListener(Event.RESIZE, function(e:Event):void {
				
				trace(stage.stageWidth);
				resize(stage.stageWidth, stage.stageHeight);
				if (rasterizer) {
					
					rasterizer.updateBounds(stage.stageWidth, stage.stageHeight);
					
				}
			});
			
			//("Z:/smile.swf");
			//OpenVideo("Z:/smile.swf", "");
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
		public override function OpenVideo(videoUrl:String, config:String):void {
			
			
			
			var req:URLRequest = new URLRequest(videoUrl);
			
			loader.contentLoaderInfo.addEventListener(Event.INIT, onInit);
			
			var fLoader:ForcibleLoader = new ForcibleLoader(loader);
			fLoader.load(req);
			this.ApplyChanges(config);
			
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
				
				if (movie.isPlaying) {
					
					movie.gotoAndPlay(pos * movie.loaderInfo.frameRate);
				} else {
					
					movie.gotoAndStop(pos * movie.loaderInfo.frameRate);
				}
			}
		}
		
		public override function ChangeVolume(vol:Number):void {
			
			if(movie) {
				
				movie.soundTransform = new SoundTransform(vol);
			}
		}
		
		private function onInit(e:Event):void {
			
			addChild(loader);
			
			loader.width = 512;
			loader.height = 384;
			movie = loader.content as MovieClip;
			
			var stageW:int = stage.stageWidth;
			var stageH:int = stage.stageHeight;

			videoW = loader.contentLoaderInfo.width;
			videoH = loader.contentLoaderInfo.height;
			
			
			trace("stageWidth:" + stageW + " stageHeight:" + stageH);
			trace("videoWidth:" + videoW + " videoHeight:" + videoH);

			//動画アスペクト比
			var AR:Number = videoW / videoH;
			
			trace(AR);
			
			var aspectW:Number = stageH * AR;
			
			trace("apectWidth:" + movie.width);
			
			//アスペクト比を考慮したときに両端に出る黒い部分の位置　両端からのオフセット
			var x:* = (stageW - videoW) / 2;
			
			trace(x);
			
			var _loc3_:Number = 1;
			//movie.x = x;

			resize(stageW, stageH);
			
			rasterizer.updateBounds(stage.stageWidth, stage.stageHeight);
			addChild(rasterizer);
			
			renderTick.addEventListener(TimerEvent.TIMER, onFrame);
			
			CallCSharp("Initialized");
			this.renderTick.start();
			
			var width:String = videoW.toString();
			var height:String = videoH.toString();
			ExternalInterface.call("WidthHeight", width + "×" + height);

			var framerate:String = loader.contentLoaderInfo.frameRate.toString();
			ExternalInterface.call("Framerate", framerate);
			var filesize:String = movie.loaderInfo.bytesTotal.toString();
			
			ExternalInterface.call("FileSize", filesize);

		}
		
		
		public function resize(width:Number, height:Number) : void {
			
			var scale:Number = 1;
			if (this.videoH / this.videoW < height / width) {
				
				scale = width / this.videoW;
				this.loader.scaleX = this.loader.scaleY = scale;
				this.loader.x = 0;
				this.loader.y = (height - this.videoH * scale) / 2;
				this.rasterizer.scaleY = scale;
				
			} else {
				
				scale = height / this.videoH;
				this.loader.scaleX = this.loader.scaleY = scale;
				this.loader.x = (width - this.videoW * scale) / 2;
				this.loader.y = 0;
				this.rasterizer.scaleY = scale;
				
			}
      }

		private var prevTime:int = 0;
		private var prevLoaded:uint = 0;
		
		public override function onFrame(e:TimerEvent):void {
			
			// 再生時間を取得
			var frame:int = movie.currentFrame;
			var value:Number = (frame / movie.loaderInfo.frameRate);
			var vpos:Number = Math.floor(value * 100);
			
			
			// バッファの計算
			var buffer:Number = (loader.contentLoaderInfo.bytesLoaded) / (loader.contentLoaderInfo.bytesTotal);
			
			
			if (ExternalInterface.available) {

				ExternalInterface.call("CsFrame", value.toString(), buffer.toString(), (loader.contentLoaderInfo.bytesLoaded - prevLoaded).toString(), vpos.toString());
			}
			prevLoaded = loader.contentLoaderInfo.bytesLoaded;
			prevTime = (int) (value);
			
			rasterizer.render(vpos);
			
			if (frame == movie.totalFrames) {
				
				Pause();
				CallCSharp("Stop");
			}
		}
		
		
	}
}