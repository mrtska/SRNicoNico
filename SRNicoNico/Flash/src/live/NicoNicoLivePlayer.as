package live {
	import flash.events.AsyncErrorEvent;
	import flash.events.Event;
	import flash.events.NetStatusEvent;
	import flash.events.TimerEvent;
	import flash.external.ExternalInterface;
	import flash.geom.Rectangle;
	import flash.media.StageVideo;
	import flash.media.Video;
	import flash.net.NetConnection;
	import flash.net.NetStream;
	import flash.net.ObjectEncoding;
	import flash.utils.Timer;
	/**
	 * ...
	 * @author mrtska
	 */
	public class NicoNicoLivePlayer extends NicoNicoPlayerBase {
		
		//放送URL rtmp
		private var videoUrl:String;
		
		//ストリーム
		private var stream:NetStream;
		
		//こねくしょん
		private var connection:NetConnection;
		
		//API結果
		private var getPlayerStatus:Object;
		
		//vpos最大最小値
		private var maxVpos:int;
		private var minVpos:int;
		
		private var baseTime:Number;
		
		private var publishUrl:String;
		private var publishVpos:int;
		
		private var offset:int;
		private var vpos:int;
		
		private var vposTimer:Timer;
		
		//動画メタデータ
		private var metadata:Object;
		
		public function NicoNicoLivePlayer() {
			
			if (ExternalInterface.available) {
				
				ExternalInterface.addCallback("AsCommandExcute", CommandExcute);
			}
			
			//OpenVideo("rtmp://nlaoe115.live.nicovideo.jp:1935/fileorigin/04", "{\"Ticket\": \"23425727:lv254486518:0:1457695076:725986265f52e95e\"}");
		}
		
		
		
		public override function OpenVideo(videoUrl:String, config:String):void {
			
			this.videoUrl = videoUrl;
			var json:Object = JSON.parse(config);
			getPlayerStatus = json;
			
			connection = new NetConnection();
			connection.objectEncoding = ObjectEncoding.AMF3;
			connection.addEventListener(NetStatusEvent.NET_STATUS, onConnect);
			connection.connect(videoUrl, json.Ticket);
			
		}
		
		
		private function ConnectStream():void {
			
			ExternalInterface.call("success");
			
			stream = new NetStream(connection);
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
					ExternalInterface.call(propName + "=" + param[propName]);
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
			}
			stream.client = obj;
			
			this.maxVpos = (int(getPlayerStatus.EndTime) - int(getPlayerStatus.StartTime)) * 100;
			this.minVpos = 0;
			this.offset = int(getPlayerStatus.StartTime) - int(getPlayerStatus.BaseTime);
			this.baseTime = new Date().getTime();
			

			this.vposTimer = new Timer(25);
			this.vposTimer.addEventListener(TimerEvent.TIMER, onTick);
			this.vposTimer.start();
			
		}
		
		private function CommandExcute(cmd:String, vposs:String, arg:String):void {
			
			var args:Array = arg.split(" ");
			var vpos:int = int(vposs);
			
			switch(cmd) {
				case "/publish":	//URLを指定
					var id:String = args[0];
					var url:String = args[1];
					
					if (url.match(/(.*)\.(f4v|mp4)$/i)) {
						
						publishUrl = "mp4:" + url;
						publishVpos = vpos;
					}
					
					break;
				case "/play":	//以前またはその後にpublishコマンドで指定されたURLを再生する
					
					var offset:int = (this.vpos - publishVpos) / 100;
					if (offset < 0) {
						
						offset = 0;
					}
7
					stream.play(publishUrl, this.offset + offset);
					break;
				
			}
			
		}
		
		public override function Pause():void {
			
			
			vposTimer.stop();
		}
		public override function Resume():void {
			
			vposTimer.start();
			
		}
		
		
		private var prevTime:int = 0;
		private var prevLoaded:uint = 0;
		
		private function onTick(e:TimerEvent):void {
			
			
			/*コメントのアレ
			var vpos:Number = Math.floor(value * 100);

			prevLoaded = stream.bytesLoaded;
			prevTime = (int) (value);
			
			
			*///タイムシフト
			
			var now:Date = new Date();
			vpos = (now.getTime() - this.baseTime) / 10;
			ExternalInterface.call("CsFrame", int(stream.time * 100 + offset * 100).toString());

			
			//rasterizer.render(vpos);
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
				
				break;
			}
			
		}
		
		private function onAsyncError(e:AsyncErrorEvent):void {
			
			ExternalInterface.call("AsyncError");
			trace("onAsyncError");
			
		}
		
		private function onConnect(e:NetStatusEvent):void {
			
			ExternalInterface.call(e.info.code);
			switch(e.info.code) {
			case "NetConnection.Connect.Success":
				ConnectStream();
				break;
			// その他
			default:
			}
		}
		
		
	}
}