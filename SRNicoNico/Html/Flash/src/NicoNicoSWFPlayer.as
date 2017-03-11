package 
{
	
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
	
	[SWF(width="512", height="384")]
	public class NicoNicoSWFPlayer extends NicoNicoPlayerBase {
		
		//ストリーミングURL そのまんま
		private var videoUrl:String;
		
		
		private var loader:Loader = new Loader();

		//SWF動画インスタンス
		private var movie:MovieClip;
		
		private var initialPos:Number;
		
		private var autoPlay:Boolean;
		
		public function NicoNicoSWFPlayer() {

			
		}
		
		
		
		//指定したURLをストリーミング再生する
		public override function OpenVideo(videoUrl:String, initialPos:Number, autoplay:*):void {
			
			this.initialPos = initialPos;
			//なぜか直接代入するとfalseになる
			if (autoplay) {
				
				this.autoPlay = true;
			} else {
				
				this.autoPlay = false;
			}
			
			var req:URLRequest = new URLRequest(videoUrl);
			loader.contentLoaderInfo.addEventListener(Event.INIT, onInit);
			var fLoader:ForcibleLoader = new ForcibleLoader(loader);
			fLoader.load(req);
			
		}
		
		public override function GetCurrentTime():Number {
			
			if (movie) {
				
				var frame:int = movie.currentFrame;
				return (frame / movie.loaderInfo.frameRate);
			} else {
				
				return 0;
			}
		}
		
		//そのまんま
		public override function Pause():void {
			
			if(movie) {
				
				ExternalInterface.call("invoke_host", "playstate", false);
				ExternalInterface.call("CommentViewModel.pauseComment()");
				movie.stop();
			}
		}
		public override function Resume():void {
			
			if(movie) {
				
				ExternalInterface.call("eval", "VideoViewModel.video.ended = false");
				ExternalInterface.call("invoke_host", "playstate", true);
				ExternalInterface.call("CommentViewModel.resumeComment()");
				
				if (!hasEventListener(Event.ENTER_FRAME)) {
					
					addEventListener(Event.ENTER_FRAME, onFrame2);
				}
				
				movie.play();
			}
		}
		public override function Seek(pos:Number):void {
			
			pos = Math.floor(pos);
			if(movie) {
				
				ExternalInterface.call("CommentViewModel.purgeComment()");
				if (movie.isPlaying) {
					
					ExternalInterface.call("CommentViewModel.resumeComment()");
					movie.gotoAndPlay(pos * movie.loaderInfo.frameRate);
				} else {
					
					ExternalInterface.call("CommentViewModel.pauseComment()");
					movie.gotoAndStop(pos * movie.loaderInfo.frameRate);
				}
				
			}
		}
		
		private var volume:Number = 0;
		
		public override function ChangeVolume(vol:Number):void {
			
			if(movie) {
				
				movie.soundTransform = new SoundTransform(vol);
			} else {
				
				this.volume = vol;
			}
		}
		
		private var videoW:int;
		private var videoH:int;
		
		private function onInit(e:Event):void {
			
			addChild(loader);
			
			movie = loader.content as MovieClip;
			movie.soundTransform = new SoundTransform(this.volume);
			var stageW:int = stage.stageWidth;
			var stageH:int = stage.stageHeight;

			videoW = loader.contentLoaderInfo.width;
			videoH = loader.contentLoaderInfo.height;
			
			var movieW:int = movie.width;
			var movieH:int = movie.height;			
			ExternalInterface.call("invoke_host", "widtheight", movieW + "×" + movieH);

			//動画アスペクト比
			var AR:Number = videoW / videoH;
			var aspectW:Number = stageH * AR;
			
			//アスペクト比を考慮したときに両端に出る黒い部分の位置　両端からのオフセット
			var x:* = (stageW - videoW) / 2;
			
			
			resize(stageW, stageH);
			
			addEventListener(Event.ENTER_FRAME, onFrame2);
			
			//this.renderTick.start();
						ExternalInterface.call("invoke_host", "log" + this.autoPlay);

			if(this.autoPlay) {
				
				movie.play();
				ExternalInterface.call("invoke_host", "playstate", true);
			} else {
				
				movie.stop();
			}
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

		public function onFrame2(e:Event):void {
			
			// 再生時間を取得
			var frame:int = movie.currentFrame;
			var time:Number = (frame / movie.loaderInfo.frameRate);
			var vpos:Number = Math.floor(time * 100);
			
			
			// バッファの計算
			var buffer:Number = (loader.contentLoaderInfo.bytesLoaded) / (loader.contentLoaderInfo.bytesTotal);
			
			ExternalInterface.call("VideoViewModel.videoloop", time, vpos, buffer);
			
			if (frame >= movie.totalFrames) {
				
				ExternalInterface.call("invoke_host", "playstate", false);
				movie.stop();
				ExternalInterface.call("CommentViewModel.pauseComment()");
				ExternalInterface.call("eval", "VideoViewModel.video.ended = true");
				
				ExternalInterface.call("invoke_host", "ended");
				
				if (hasEventListener(Event.ENTER_FRAME)) {
					
					removeEventListener(Event.ENTER_FRAME, onFrame2);
				}
				
				
			}
		}
	
	}

}