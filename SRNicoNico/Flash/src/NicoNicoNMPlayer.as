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
				
				resize(stage.stageWidth, stage.stageHeight);
			});
			
		}
		
		//指定したURLをストリーミング再生する
		public override function OpenVideo(videoUrl:String, config:String):void {
			
			var req:URLRequest = new URLRequest(videoUrl);
			loader.contentLoaderInfo.addEventListener(Event.INIT, onInit);
			
			var fLoader:ForcibleLoader = new ForcibleLoader(loader);
			fLoader.load(req);
			
		}
		
		//そのまんま
		public override function Pause():void {
			
			if(movie) {
				
				ExternalInterface.call("invoke_host", "playstate", false);
				movie.stop();
			}
		}
		public override function Resume():void {
			
			if(movie) {
				
				ExternalInterface.call("invoke_host", "playstate", true);
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
			
			
			ExternalInterface.call("invoke_host", "widtheight", videoW + "×" + videoH);

			//動画アスペクト比
			var AR:Number = videoW / videoH;
			var aspectW:Number = stageH * AR;
			
			
			//アスペクト比を考慮したときに両端に出る黒い部分の位置　両端からのオフセット
			var x:* = (stageW - videoW) / 2;
			
			
			var _loc3_:Number = 1;

			resize(stageW, stageH);
			
			renderTick.addEventListener(TimerEvent.TIMER, onFrame);
			
			this.renderTick.start();
			
			var width:String = videoW.toString();
			var height:String = videoH.toString();

		}
		
		
		public function resize(width:Number, height:Number) : void {
			
			var scale:Number = 1;
			if (this.videoH / this.videoW < height / width) {
				
				scale = width / this.videoW;
				this.loader.scaleX = this.loader.scaleY = scale;
				this.loader.x = 0;
				this.loader.y = (height - this.videoH * scale) / 2;
			} else {
				
				scale = height / this.videoH;
				this.loader.scaleX = this.loader.scaleY = scale;
				this.loader.x = (width - this.videoW * scale) / 2;
				this.loader.y = 0;
			}
      }

		public override function onFrame(e:TimerEvent):void {
			
			// 再生時間を取得
			var frame:int = movie.currentFrame;
			var time:Number = (frame / movie.loaderInfo.frameRate);
			var vpos:Number = Math.floor(time * 100);
			
			
			// バッファの計算
			var buffer:Number = (loader.contentLoaderInfo.bytesLoaded) / (loader.contentLoaderInfo.bytesTotal);
			
			ExternalInterface.call("VideoViewModel.video_tick", time, vpos, buffer);
			
			if (frame >= movie.totalFrames) {
				
				ExternalInterface.call("invoke_host", "playstate", false);
				movie.stop();
				ExternalInterface.call("CommentViewModel.pause_comment()");
				ExternalInterface.call("eval", "VideoViewModel.video.ended = true");
			}
		}
		
		
	}
}