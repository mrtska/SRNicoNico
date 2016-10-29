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
		}
		
		//指定したURLをストリーミング再生する
		public override function OpenVideo(videoUrl:String, config:String):void {
			

			
			secureNetConnection.getSecureNetConnection(function(connection:NetConnection):void {
			
				NicoNicoRTMPPlayer.connection = connection;
			
				fmsToken = videoUrl.substr(videoUrl.indexOf("^") + 1);
				
				
				NicoNicoRTMPPlayer.videoUrl = videoUrl.substr(0, videoUrl.indexOf("^"));
				
				connection.addEventListener(NetStatusEvent.NET_STATUS, onConnect);
				connection.addEventListener(SecurityErrorEvent.SECURITY_ERROR, onSecurityError);
				var tokenTime:String = fmsToken.substr(0,fmsToken.indexOf(":"));
				var tokenHash:String = fmsToken.substr(fmsToken.indexOf(":") + 1);
				connection.connect(NicoNicoRTMPPlayer.videoUrl.substr(0, NicoNicoRTMPPlayer.videoUrl.indexOf("?")), tokenHash, tokenTime, NicoNicoRTMPPlayer.videoUrl.substr(NicoNicoRTMPPlayer.videoUrl.indexOf("=") + 1));
				
				
			});
			//connection.connect("rtmpe://smile-chefsf.nicovideo.jp/smile", "e83c2c042be7e275dfe4c8be786bfa7cea15ee32", "1442904740", "mp4:26673741.16641");
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
			
			//インスタンス作成
			stream = new NetStream(connection);
			stream.addEventListener(NetStatusEvent.NET_STATUS, onNetStatus);
			stream.addEventListener(AsyncErrorEvent.ASYNC_ERROR, onAsyncError);
			var obj:Object = new Object();
			obj.onMetaData = function(param:Object):void {
				
				metadata = param;
				ExternalInterface.call("invoke_host", "log", "onMetadata", param.toString());
				var stageW:int = stage.stageWidth;
				var stageH:int = stage.stageHeight;
				var videoW:int = param.width;
				var videoH:int = param.height;
				
				
				ExternalInterface.call("invoke_host", "widtheight", videoW + "×" + videoH);
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
			
			
			stream.checkPolicyFile = true;
			stream.bufferTime = 1;
			//イベントリスナ登録
			
			stream.play(NicoNicoRTMPPlayer.videoUrl.substr(NicoNicoRTMPPlayer.videoUrl.indexOf("=") + 1));
			
		}
		
		private function onSecurityError(e:SecurityErrorEvent):void {
			
			ExternalInterface.cal("invoke_host", "log", e.toString());
		}
		
		
		public override function onFrame(e:TimerEvent):void {
			
			// 再生時間を取得
			var value:Number = stream.time;
			
			// バッファの計算
			var buffer:Number = (stream.bytesLoaded) / (stream.bytesTotal);
			var vpos:Number = Math.floor(value * 100);
			
			ExternalInterface.call("VideoViewModel.video_tick", value, vpos, buffer);
			
			//trace("value:" + value + " diff:" + this.diff);
		}
		
		private function onAsyncError(e:AsyncErrorEvent):void {
			
			ExternalInterface.call("invoke_host", "log", "onAsyncError2", e.text);
		}
		
		public override function onNetStatus(e:NetStatusEvent):void {
			
			ExternalInterface.call("invoke_host", "log", e.info.code);
			switch(e.info.code) {
			case "NetStream.Play.Start":
				
				break;
			case "NetStream.Buffer.Full":
				
				trace("Buffer.Full:");
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
				break;
			}
			
		}
		
		
		private function onConnect(e:NetStatusEvent):void {
			
			ExternalInterface.call("invoke_host", "log", e.info.code);
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