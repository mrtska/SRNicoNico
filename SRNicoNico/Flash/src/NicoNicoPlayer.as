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
			//OpenVideo("Z:/issue2.mp4");
			//OpenVideo("Z:/smile.flv");
			//OpenVideo("Z:/smile.swf");
			//
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
			loader.load(req);
			stage.addEventListener(KeyboardEvent.KEY_DOWN, function(e:KeyboardEvent):void {
				
				if(e.keyCode == Keyboard.NUMBER_0) {
					
					stream.seek(0);
				}
				if(e.keyCode == Keyboard.NUMBER_1) {
					
					stream.seek(30);
				}
				if(e.keyCode == Keyboard.NUMBER_2) {
					
					stream.seek(60);
				}
				if(e.keyCode == Keyboard.SPACE) {
					
					stream.togglePause();
				}
				if(e.keyCode == Keyboard.N) {
					
					trace("step");
					stream.step(1);
				}
			});*/
		}
		
		//指定したURLをストリーミング再生する
		public override function OpenVideo(videoUrl:String, config:String):void {
			
			this.videoUrl = videoUrl;
			connection = new NetConnection();
			connection.addEventListener(NetStatusEvent.NET_STATUS, onConnect);
			connection.connect(null);
			//this.ApplyChanges(config);
		}
		
		//そのまんま
		public override function Pause():void {
			
			ExternalInterface.call("invoke_host", "playstate", false);
			stream.pause();
		}
		public override function Resume():void {
			
			ExternalInterface.call("invoke_host", "playstate", true);
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
			//stream.soundTransform = new SoundTransform(0.1);
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
				//addChild(rasterizer);
				renderTick.addEventListener(TimerEvent.TIMER, onFrame);
				renderTick.start();
			}
			stream.client = obj;
			stream.soundTransform = new SoundTransform(0.1);
			stream.play(videoUrl);
			//ExternalInterface.call("readya");
			
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
			
			ExternalInterface.call(e.info.code);
			switch(e.info.code) {
			case "NetStream.Play.Start":

				break;
			case "NetStream.Buffer.Full":
				
				trace("Buffer.Full:");
				break;
			case "NetStream.Play.Stop":
				CallCSharp("Stop");
				break;
			default:
				
				trace("default:" + e.info.code);
				break;
			}
			
		}
		
		
		private function onConnect(e:NetStatusEvent):void {
			
			ExternalInterface.call(e.info.code);
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