package rtmp  {
	
	
	import flash.events.AsyncErrorEvent;
	import flash.events.Event;
	import flash.events.NetStatusEvent;
	import flash.events.SecurityErrorEvent;
	import flash.events.TimerEvent;
	import flash.geom.Rectangle;
	import flash.media.SoundTransform;
	import flash.media.StageVideo;
	import flash.media.Video;
	import flash.net.NetConnection;
	import flash.net.NetStream;
	import flash.net.ObjectEncoding;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.system.ApplicationDomain;
	import flash.system.Security;
	import flash.system.SecurityDomain;
	import flash.system.fscommand;
	import flash.utils.Timer;
	
	
	[SWF(width="672", height="384")]
	public class NicoNicoRTMPPlayer extends NicoNicoPlayerBase {
		
		//ストリーミングURL そのまんま
		private static var videoUrl:String;
		
		//FMSトークン
		private var fmsToken:String;
		
		//ストリーミングするためのやつ こんな感じのやつがC#とかでも使えればいいのにな
		private var stream:NetStream;
		
		//コネクション 
		private static var connection:NetConnection;
		
		//指定したシークポジション
		private var wantSeekPos:Number;
		
		private var diff:Number = 0;
		
		//動画メタデータ
		private var metadata:*;
		
		private var secureNetConnection:SecureNetConnection = new SecureNetConnection("http://res.nimg.jp/swf/player/secure_nccreator.swf?t=201111091500");
		
		//コンストラクタ
		public function NicoNicoRTMPPlayer() {
			
			super();

			Security.allowDomain("*");
			
			//OpenVideo("rtmpe://smile-chefsf.nicovideo.jp/smile?m=mp4:26673741.16641^1442928807:ca8cce63df646096080443cb50c195c4f73335a4");
			/*//OpenVideo("Z:/smile.swf");
			//OpenVideo("Z:/smile.mp4");
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
			

			
			secureNetConnection.getSecureNetConnection(function(connection:NetConnection):void {
			
				NicoNicoRTMPPlayer.connection = connection;
			
				fmsToken = videoUrl.substr(videoUrl.indexOf("^") + 1);
				
				
				fscommand("OpenVideo", videoUrl);
				NicoNicoRTMPPlayer.videoUrl = videoUrl.substr(0, videoUrl.indexOf("^"));
				
				connection.addEventListener(NetStatusEvent.NET_STATUS, onConnect);
				connection.addEventListener(SecurityErrorEvent.SECURITY_ERROR, onSecurityError);
				
				var tokenTime:String = fmsToken.substr(0,fmsToken.indexOf(":"));
				var tokenHash:String = fmsToken.substr(fmsToken.indexOf(":") + 1);
				connection.connect(NicoNicoRTMPPlayer.videoUrl.substr(0, NicoNicoRTMPPlayer.videoUrl.indexOf("?")), tokenHash, tokenTime, NicoNicoRTMPPlayer.videoUrl.substr(NicoNicoRTMPPlayer.videoUrl.indexOf("=") + 1));
				
				
			});
			//connection.connect("rtmpe://smile-chefsf.nicovideo.jp/smile", "e83c2c042be7e275dfe4c8be786bfa7cea15ee32", "1442904740", "mp4:26673741.16641");
		}
		
		public function Reconnect():void {
			
			
			
		}
		
		//そのまんま
		public override function Pause():void {
			
			if(stream) {
				
				stream.pause();
			}
			
		}
		public override function Resume():void {
			
			if(stream) {
				
				stream.resume();
			}
			
		}
		public override function Seek(pos:Number):void {
			
			if(stream) {
				
				wantSeekPos = pos;
				stream.seek(pos);
			}

		}
		public override function ChangeVolume(vol:Number):void {
			
			if(stream) {
				
				stream.soundTransform = new SoundTransform(vol);
			}
		}
		
		
		//ビデオコントロールにストリームを繋ぐ
		private function ConnectStream():void {
			
			if(connection == null) {
				
				fscommand("ConnectionError");
				return;
			}
			//インスタンス作成
			stream = new NetStream(connection);
			stream.addEventListener(NetStatusEvent.NET_STATUS, onNetStatus);
			stream.addEventListener(AsyncErrorEvent.ASYNC_ERROR, onAsyncError);
			var obj:Object = new Object();
			obj.onMetaData = function(param:Object):void {
				
				metadata = param;
				fscommand("onMetadata", param.toString());
				for(var propName:String in param){
					trace(propName + "=" + param[propName]);
				}
			}
			stream.client = obj;
			
			trace("ConnectStream");
			
			stream.checkPolicyFile = true;
			stream.bufferTime = 1;
			//stream.soundTransform = new SoundTransform(0.1);
			//イベントリスナ登録
			
			var vec:Vector.<StageVideo> = stage.stageVideos;
			if(vec.length >= 1) {
				;
				var stageVideo:StageVideo = vec[0];
				
				stageVideo.viewPort = new  Rectangle(0, 0, stage.stageWidth, stage.stageHeight);
				
				stageVideo.attachNetStream(stream);
				
			} else {
				
				var video:Video = new Video(stage.stageWidth, stage.stageHeight);
				video.smoothing = true;
				
				video.attachNetStream(stream);
				addChild(video);
			}
		
			stream.play(NicoNicoRTMPPlayer.videoUrl.substr(NicoNicoRTMPPlayer.videoUrl.indexOf("=") + 1));
			
			addEventListener(Event.ENTER_FRAME, onFrame);
			
			
		}
		
		private function onSecurityError(e:SecurityErrorEvent):void {
			
			fscommand(e.toString());
			trace(e);
		}
		
		private var prevTime:int = 0;
		private var prevLoaded:uint = 0;
		
		public override function onFrame(e:Event):void {
			
			// 再生時間を取得
			var value:Number = stream.time;
			
			// バッファの計算
			var buffer:Number = (stream.bytesLoaded) / (stream.bytesTotal);
			
			//trace("Time:" + stream.bytesTotal);
			
			if(prevTime != (int)(value)) {
				
				fscommand("CsFrame", value + ":" + buffer.toString() + ":" + (stream.bytesLoaded - prevLoaded).toString());
				prevLoaded = stream.bytesLoaded;
			}
			prevTime = (int) (value);
			
			var vpos:Number = Math.floor(value * 100);
			
			rastarizer.render(vpos);
			//trace("value:" + value + " diff:" + this.diff);
		}
		
		private function onAsyncError(e:AsyncErrorEvent):void {
			
			trace("onAsyncError");
			fscommand("onAsyncError2", e.text);
			trace(e.text);
		}
		
		public override function onNetStatus(e:NetStatusEvent):void {
			
			trace("onNetStatus");
			fscommand("onNetStatus", e.info.code);
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
			
			fscommand(e.info.code, e.info.level);
			trace("onConnect:" + e.info.level);
			trace(e.info.code);
		
			switch(e.info.code) {
			case "NetConnection.Connect.Success":
				ConnectStream();
				break;
			case "NetConnection.Connect.Closed":
				break;
			// その他
			default:
				trace(e.info.code);
			}
		}
	}
		
}