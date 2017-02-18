package 
{
	

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
		
		private var autoPlay:Boolean;
		//コネクション 
		private static var connection:NetConnection;
		
		private var secureNetConnection:SecureNetConnection = new SecureNetConnection("http://res.nimg.jp/swf/player/secure_nccreator.swf?t=201111091500");
		
		public function NicoNicoRTMPPlayer() {
			
			super();
			Security.allowDomain("*");
		}
		
		
		
		//指定したURLをストリーミング再生する
		public override function OpenVideo(videoUrl:String, initialPos:Number, autoplay:*):void {
			
			//なぜか直接代入するとfalseになる
			if (autoplay) {
				
				this.autoPlay = true;
			} else {
				
				this.autoPlay = false;
			}
			
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
			
		}
		
		public override function GetCurrentTime():Number {
			
			if (stream) {
				
				return stream.time;
			} else {
				
				return 0;
			}
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
				
				stream.seek(pos);
			}
		}
		
		private var volume:Number = 0;
		
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
			
			if (this.autoPlay) {
				
				ExternalInterface.call("invoke_host", "playstate", true);
				stream.resume();
			} else {
				
				stream.pause();
			}
		}
		
		
		private function onSecurityError(e:SecurityErrorEvent):void {
			
			ExternalInterface.call("invoke_host", "log"+ e.toString());
		}
		
		private function onAsyncError(e:AsyncErrorEvent):void {
			
			ExternalInterface.call("invoke_host", "log"+"onAsyncError2", e.text);
		}
		
		public override function onNetStatus(e:NetStatusEvent):void {
			
			ExternalInterface.call("invoke_host", "log" + e.info.code);
			switch(e.info.code) {
			case "NetStream.Play.Start":
				break;
			case "NetStream.Buffer.Full":
				
				break;
				
			case "NetStream.Pause.Notify":
				ExternalInterface.call("invoke_host", "playstate", false);
				ExternalInterface.call("CommentViewModel.pauseComment()");
				break;
			case "NetStream.Unpause.Notify":
				ExternalInterface.call("invoke_host", "playstate", true);
				ExternalInterface.call("CommentViewModel.resumeComment()");
				break;
			case "NetStream.SeekStart.Notify":
				ExternalInterface.call("CommentViewModel.pauseComment()");
				ExternalInterface.call("eval", "VideoViewModel.video.seeking = true");
				break;
			case "NetStream.Seek.Complete":
				ExternalInterface.call("CommentViewModel.purgeComment()");
				ExternalInterface.call("CommentViewModel.resumeComment()");
				ExternalInterface.call("eval", "VideoViewModel.video.seeking = false");
				break;
			case "NetStream.Play.Stop":
				
				ExternalInterface.call("CommentViewModel.pauseComment()");
				ExternalInterface.call("invoke_host", "playstate", false);
				ExternalInterface.call("eval", "VideoViewModel.video.ended = true");
				break;
			default:
				break;
			}
			
		}
		
		
		private function onConnect(e:NetStatusEvent):void {
			
			ExternalInterface.call("invoke_host", "log" + e.info.code);
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

		
		public override function onFrame(e:TimerEvent):void {
			
			// 再生時間を取得
			var time:Number = stream.time;
			
			// バッファの計算
			var buffer:Number = 0;
			var vpos:Number = Math.floor(time * 100);
			
			ExternalInterface.call("VideoViewModel.videoloop", time, vpos, buffer);
			
		}
	
	}

}