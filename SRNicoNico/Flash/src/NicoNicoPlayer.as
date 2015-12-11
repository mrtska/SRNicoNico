package  {
	
	import flash.events.AsyncErrorEvent;
	import flash.events.Event;
	import flash.events.KeyboardEvent;
	import flash.events.NetStatusEvent;
	import flash.geom.Rectangle;
	import flash.media.SoundTransform;
	import flash.media.StageVideo;
	import flash.media.Video;
	import flash.net.NetConnection;
	import flash.net.NetStream;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.system.fscommand;
	import flash.ui.Keyboard;
	import flash.ui.Mouse;
	
	[SWF(width="672", height="384")]
	public class NicoNicoPlayer extends NicoNicoPlayerBase {
		
		//ストリーミングURL そのまんま
		private var videoUrl:String;
		
		//ストリーミングするためのやつ こんな感じのやつがC#とかでも使えればいいのにな
		private var stream:NetStream;
		
		//コネクション 正直何に使ってるのかわからない
		private var connection:NetConnection;
		
		//指定したシークポジション
		private var wantSeekPos:Number;
		
		private var diff:Number = 0;
		
		//動画メタデータ
		private var metadata:Object;
		
		
		//コンストラクタ
		public function NicoNicoPlayer() {
			
			super();
			//OpenVideo("Z:/issue2.mp4");
			//OpenVideo("Z:/smile.flv");
			/*OpenVideo("Z:/smile.swf");
			//
			//OpenVideo("Z:/smile (1).mp4");
			//var now:Date = new Date();
			//OpenVideo("http://mrtska.net/SRNicoNico/sm9?"+ now.time.toString());
			//OpenVideo("http://mrtska.net/SRNicoNico/sm8628149");
			//OpenVideo("http://mrtska.net/SRNicoNico/sm9");
			var loader:URLLoader = new URLLoader();
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
		public override function OpenVideo(videoUrl:String):void {
			
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
			
			wantSeekPos = pos;
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
				
				metadata = param;
				for(var propName:String in param){
					trace(propName + "=" + param[propName]);
				}

				var vec:Vector.<StageVideo> = stage.stageVideos;
				if(vec.length >= 1) {
					;
					var stageVideo:StageVideo = vec[0];
					
					//アスペクト比が4:3だったら
					if ((param.height / param.width) == 0.75) {
						
						var aspect:* = (stage.stageWidth - 512) / 2;
					
						if (aspect < 0) {
							
							aspect = 0;

						}
						stageVideo.viewPort = new  Rectangle(aspect, 0, 512, stage.stageHeight);

					} else {
						
						stageVideo.viewPort = new  Rectangle(0, 0, stage.stageWidth, stage.stageHeight);
					}
					
					stageVideo.attachNetStream(stream);
					
				} else {
					
					var video:Video = new Video(stage.stageWidth, param.height);
					video.smoothing = true;
					
					video.attachNetStream(stream);
					addChild(video);
				}
				
				addEventListener(Event.ENTER_FRAME, onFrame);
			}
			stream.client = obj;
			
			stream.play(videoUrl);
		}
		
		
		private var prevTime:int = 0;
		private var prevLoaded:uint = 0;
		
		public override function onFrame(e:Event):void {
			
			// 再生時間を取得
			var value:Number = stream.time;
			
			// バッファの計算
			var buffer:Number = (stream.bytesLoaded) / (stream.bytesTotal);
			
			//trace("Time:" + stream.time);
			
			//if(prevTime != (int)(value)) {
				
				fscommand("CsFrame", value + ":" + buffer.toString() + ":" + (stream.bytesLoaded - prevLoaded).toString());
				prevLoaded = stream.bytesLoaded;
			//}
			prevTime = (int) (value);
			
			var vpos:Number = Math.floor(value * 100);
			
			rastarizer.render(vpos);
			//trace("value:" + value + " diff:" + this.diff);
		}
		
		private function onAsyncError(e:AsyncErrorEvent):void {
			
			fscommand("AsyncError");
			trace("onAsyncError");
			
			trace(e.text);
		}
		
		public override function onNetStatus(e:NetStatusEvent):void {
			
			trace("onNetStatus");
			super.onNetStatus(e);
			switch(e.info.code) {
			case "NetStream.Play.Start":

				break;
			
			case "NetStream.Seek.Notify":				
				
				trace("Seek.Notify");
				
				var diff:Number;
				
				if(stream.time < wantSeekPos) {
					
					diff = stream.time - wantSeekPos;
				} else {
					
					diff = wantSeekPos - stream.time;
				}
				this.diff = diff;
				
				trace("Seek:" + stream.time);
				
				break;
			case "NetStream.Seek.Complete":
				
				trace("Seek.Complete");
				
				//stream.step(diff * stage.frameRate);
				
				
				
				break;
			case "NetStream.Buffer.Full":
				
				trace("Buffer.Full:");
				break;
			default:
				
				trace("default:" + e.info.code);
				break;
			}
			
		}
		
		
		private function onConnect(e:NetStatusEvent):void {
			
			fscommand(e.info.code);
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