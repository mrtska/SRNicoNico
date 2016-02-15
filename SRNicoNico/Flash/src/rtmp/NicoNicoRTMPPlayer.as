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
	import flash.utils.Timer;
	import flash.external.ExternalInterface;
	
	[SWF(width="640", height="360")]
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

			//OpenVideo("rtmpe://smile-chefsf.nicovideo.jp/smile?m=mp4:26673741.16641^1455159335:09a07863b3c274dfc84879d4bb90f51e9bc10036");
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
				
				
				NicoNicoRTMPPlayer.videoUrl = videoUrl.substr(0, videoUrl.indexOf("^"));
				
				connection.addEventListener(NetStatusEvent.NET_STATUS, onConnect);
				connection.addEventListener(SecurityErrorEvent.SECURITY_ERROR, onSecurityError);
				trace(videoUrl);
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
				
				CallCSharp("ConnectionError");
				return;
			}
			//インスタンス作成
			stream = new NetStream(connection);
			stream.addEventListener(NetStatusEvent.NET_STATUS, onNetStatus);
			stream.addEventListener(AsyncErrorEvent.ASYNC_ERROR, onAsyncError);
			var obj:Object = new Object();
			obj.onMetaData = function(param:Object):void {
				
				metadata = param;
				ExternalInterface.call("onMetadata", param.toString());
				for(var propName:String in param){
					trace(propName + "=" + param[propName]);
				}
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
				addChild(rasterizer);
				
				
				addEventListener(Event.ENTER_FRAME, onFrame);
			}
			stream.client = obj;
			
			trace("ConnectStream");
			
			stream.checkPolicyFile = true;
			stream.bufferTime = 1;
			//イベントリスナ登録
			
			stream.play(NicoNicoRTMPPlayer.videoUrl.substr(NicoNicoRTMPPlayer.videoUrl.indexOf("=") + 1));
			CallCSharp("Initialized");
			
		}
		
		private function onSecurityError(e:SecurityErrorEvent):void {
			
			CallCSharp(e.toString());
		}
		
		private var prevTime:int = 0;
		private var prevLoaded:uint = 0;
		
		public override function onFrame(e:Event):void {
			
			// 再生時間を取得
			var value:Number = stream.time;
			
			// バッファの計算
			var buffer:Number = (stream.bytesLoaded) / (stream.bytesTotal);
			var vpos:Number = Math.floor(value * 100);

			ExternalInterface.call("CsFrame", value.toString(), buffer.toString(), (stream.bytesLoaded - prevLoaded).toString(), vpos.toString());
			prevLoaded = stream.bytesLoaded;
			prevTime = (int) (value);
			
			
			rasterizer.render(vpos);
			//trace("value:" + value + " diff:" + this.diff);
		}
		
		private function onAsyncError(e:AsyncErrorEvent):void {
			
			trace("onAsyncError");
			CallCSharp("onAsyncError2", e.text);
			trace(e.text);
		}
		
		public override function onNetStatus(e:NetStatusEvent):void {
			
			trace("onNetStatus");
			CallCSharp(e.info.code);
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
			trace("onConnect:" + e.info.level);
		
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