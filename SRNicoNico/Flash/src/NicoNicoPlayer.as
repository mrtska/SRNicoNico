package  {
	
	import flash.events.AsyncErrorEvent;
	import flash.events.Event;
	import flash.events.KeyboardEvent;
	import flash.events.MouseEvent;
	import flash.events.NetStatusEvent;
	import flash.events.TimerEvent;
	import flash.geom.Rectangle;
	import flash.media.SoundTransform;
	import flash.media.StageVideo;
	import flash.media.Video;
	import flash.net.NetConnection;
	import flash.net.NetStream;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.ui.Keyboard;
	import flash.external.ExternalInterface;
	import flash.ui.Mouse;
	
	[SWF(width="640", height="360")]
	public class NicoNicoPlayer extends NicoNicoPlayerBase {
		
		//ストリーミングURL そのまんま
		private var videoUrl:String;
		
		//ストリーミングするためのやつ こんな感じのやつがC#とかでも使えればいいのにな
		private var stream:NetStream;
		
		//コネクション 正直何に使ってるのかわからない
		private var connection:NetConnection;
		
		//動画メタデータ
		private var metadata:Object;
		
		
		//コンストラクタ
		public function NicoNicoPlayer() {
			
			super();
		}
		
		//指定したURLをストリーミング再生する
		public override function OpenVideo(videoUrl:String, config:String):void {
			
			this.videoUrl = videoUrl;
			connection = new NetConnection();
			connection.addEventListener(NetStatusEvent.NET_STATUS, onConnect);
			connection.connect(null);
		}
		
		//そのまんま
		public override function Pause():void {
			
			stream.pause();
		}
		public override function Resume():void {
			
			stream.resume();
			
		}
		
		
		
		public override function Seek(pos:Number):void {
			
			stream.seek(pos);
		}
		public override function ChangeVolume(vol:Number):void {
			
			stream.soundTransform = new SoundTransform(vol);
		}

		
		//ビデオコントロールにストリームを繋ぐ
		private function ConnectStream():void {
			
			//インスタンス作成
			stream = new NetStream(connection);

			//イベントリスナ登録
			stream.addEventListener(NetStatusEvent.NET_STATUS, onNetStatus);
			stream.addEventListener(AsyncErrorEvent.ASYNC_ERROR, onAsyncError);
			
			
			stream.inBufferSeek = true;
			//ハードウェアデコーダーは使う
			stream.useHardwareDecoder = true;
			stream.bufferTime = 1;
			
			var obj:Object = new Object();
			obj.onMetaData = function(param:Object):void {
				
				ExternalInterface.call("invoke_host", "log", "onmetadata");
				metadata = param;
				var width:String = param["width"];
				var height:String = param["height"];
				
				ExternalInterface.call("invoke_host", "widtheight", width + "×" + height);
				
				
				var stageW:int = stage.stageWidth;
				var stageH:int = stage.stageHeight;
				var videoW:int = param.width;
				var videoH:int = param.height;
				
				//動画アスペクト比
				var AR:* = videoW / videoH;
				
				var aspectW:int = stageH * AR;
				
				//アスペクト比を考慮したときに両端に出る黒い部分の位置　両端からのオフセット
				var x:* = (stageW - aspectW) / 2;
				
				
				var vec:Vector.<StageVideo> = stage.stageVideos;
				if(vec.length >= 1) {
					;
					var stageVideo:StageVideo = vec[0];
					
					stageVideo.viewPort = new Rectangle(x, 0, aspectW, stageH);

					
					stageVideo.attachNetStream(stream);
					
				} else {
					
					var video:Video = new Video(aspectW, stageH);
					video.smoothing = true;
					video.x = x;
					
					
					video.attachNetStream(stream);
					addChild(video);

				}
				renderTick.addEventListener(TimerEvent.TIMER, onFrame);
				renderTick.start();
			}
			stream.client = obj;
			stream.play(videoUrl);
		}
		
		
		
		public override function onFrame(e:TimerEvent):void {
			
			// 再生時間を取得
			var time:Number = stream.time;
			
			// バッファの計算
			var buffer:Number = (stream.bytesLoaded) / (stream.bytesTotal);
			
			//コメントのアレ
			var vpos:Number = Math.floor(time * 100);
			
			ExternalInterface.call("VideoViewModel.video_tick", time, vpos, buffer);
			
		}
		
		private function onAsyncError(e:AsyncErrorEvent):void {
			
			ExternalInterface.call("AsyncError");
			trace("onAsyncError");
			
			trace(e.text);
		}
		
		public override function onNetStatus(e:NetStatusEvent):void {
			
			ExternalInterface.call("invoke_host", "log", e.info.code);
			switch(e.info.code) {
			case "NetStream.Play.Start":

				break;
			case "NetStream.Pause.Notify":
				ExternalInterface.call("invoke_host", "playstate", false);
				break;
			case "NetStream.Unpause.Notify":
				ExternalInterface.call("invoke_host", "playstate", true);
				break;
			case "NetStream.SeekStart.Notify":
				ExternalInterface.call("CommentViewModel.pause_comment()");
				ExternalInterface.call("eval", "VideoViewModel.video.seeking = true");
				break;
			case "NetStream.Seek.Complete":
				ExternalInterface.call("CommentViewModel.resume_comment()");
				ExternalInterface.call("eval", "VideoViewModel.video.seeking = false");
				break;
			case "NetStream.Play.Stop":
				
				ExternalInterface.call("CommentViewModel.pause_comment()");
				ExternalInterface.call("invoke_host", "playstate", false);
				ExternalInterface.call("eval", "VideoViewModel.video.ended = true");
				break;
			default:
				
				trace("default:" + e.info.code);
				break;
			}
			
		}
		
		
		private function onConnect(e:NetStatusEvent):void {
			
			ExternalInterface.call("invoke_host", "log", e.info.code);
			switch(e.info.code) {
			case "NetConnection.Connect.Success":
				ConnectStream();
				break;
			// その他
			default:
				trace(e.info.code);
			}
		}
	}
}