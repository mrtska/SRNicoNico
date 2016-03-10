package live {
	import flash.events.Event;
	import flash.events.NetStatusEvent;
	import flash.events.AsyncErrorEvent;
	import flash.geom.Rectangle;
	import flash.media.StageVideo;
	import flash.media.Video;
	import flash.net.NetConnection;
	import flash.net.NetStream;
	import flash.external.ExternalInterface;
	import flash.net.ObjectEncoding;
	/**
	 * ...
	 * @author mrtska
	 */
	public class NicoNicoLivePlayer extends NicoNicoPlayerBase {
		
		private var videoUrl:String;
		
		private var stream:NetStream;
		
		private var connection:NetConnection;
		
		private var getPlayerStatus:Object;
		
		//動画メタデータ
		private var metadata:Object;
		
		public function NicoNicoLivePlayer() {
			
			
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
	
			addEventListener(Event.ENTER_FRAME, onFrame);
			
			
			
			//タイムシフト
			if (getPlayerStatus.Archive) {
				
				
				stream.play("lv255589164_23425727");
			} else {
				
				
			}
			
			
			


		}
		
		
		private var prevTime:int = 0;
		private var prevLoaded:uint = 0;
		
		public override function onFrame(e:Event):void {
			
			// 再生時間を取得
			var value:Number = stream.time;
			
			// バッファの計算
			var buffer:Number = (stream.bytesLoaded) / (stream.bytesTotal);
			
			//コメントのアレ
			var vpos:Number = Math.floor(value * 100);

			ExternalInterface.call("CsFrame", value.toString(), buffer.toString(), (stream.bytesLoaded - prevLoaded).toString(), vpos.toString());
			prevLoaded = stream.bytesLoaded;
			prevTime = (int) (value);
			
			
			//タイムシフト
			if (getPlayerStatus.Archive) {
				
				for each(var que:Object in getPlayerStatus.QueSheet) {
					
					
				}
				
				
				stream.play("lv255589164_23425727");
			} else {
				
				
			}
			
			
			rasterizer.render(vpos);
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
		
		private function onAsyncError(e:AsyncErrorEvent):void {
			
			ExternalInterface.call("AsyncError");
			trace("onAsyncError");
			
			trace(e.text);
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