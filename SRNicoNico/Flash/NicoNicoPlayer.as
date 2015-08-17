package {
	
	
	import flash.display.Sprite;
	import flash.events.HTTPStatusEvent;
	import flash.external.ExternalInterface;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.net.URLRequestMethod;
	import flash.net.URLRequestHeader;
	import flash.text.TextField;
	import flash.events.Event;
	import flash.text.TextFieldAutoSize;
	import flash.display.StageScaleMode;
	import flash.net.NetStream;
	import flash.net.NetConnection;
	import flash.events.NetStatusEvent;
	import flash.media.Video;
	import flash.text.TextFormat;
	import flash.media.StageVideo;
	 import flash.geom.Rectangle;
	 import flash.net.NetStreamPlayOptions;
	
	
	public class NicoNicoPlayer extends Sprite {
		
		
		
		
		
		private var videoUrl:String;
		
		private var stream:NetStream;
		private var connection:NetConnection;
		
		private var debugText:TextField = new TextField();
		
		public function NicoNicoPlayer() {
			

			stage.scaleMode = StageScaleMode.EXACT_FIT;
			
			var format:TextFormat = new TextFormat();
			format.color = 0xFFFFFF;
			debugText.defaultTextFormat = format;
			debugText.text = "スタート";
			
			//addChild(debugText);
			
			if(ExternalInterface.available) {
				
				//コールバック登録
				ExternalInterface.addCallback("CsOpenVideo", OpenVideo);
				
				
				

			}
			
			
			//OpenVideo("http://mrtska.net/SRNicoNico/sm9");
			OpenVideo("file:///Z:/smile.mp4");
						
		}
		
		private function OpenVideo(videoUrl:String):void {
			
			this.videoUrl = videoUrl;
			connection = new NetConnection();
			connection.addEventListener(NetStatusEvent.NET_STATUS, onConnect);
			connection.connect(null);
			
		}
		
		private function ConnectStream() {
			
	
			
			debugText.text = "動画再生:" + videoUrl;
			stream = new NetStream(connection);
			stream.addEventListener(NetStatusEvent.NET_STATUS, onNetStatus);
			stream.useHardwareDecoder = true;
			var obj:Object = new Object();
			obj.onMetaData = function(param:Object):void {
				
				
				
				for(var propName:String in param){
					trace(propName + "=" + param[propName]);
				}
				
			
			}
			stream.client = obj;
			
			
			
				
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
			
			
			
				
			
			
			
			
			stream.play(videoUrl);
		}
		
		private function onNetStatus(e:NetStatusEvent):void {
			
			
			trace(e.info.code);
		}
		
		
		private function onConnect(e:NetStatusEvent):void {
			
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