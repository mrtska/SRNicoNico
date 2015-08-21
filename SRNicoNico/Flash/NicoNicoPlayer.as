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
	 import flash.system.fscommand;
	 import flash.events.AsyncErrorEvent;
	 import flash.events.MouseEvent;
	
	
	public class NicoNicoPlayer extends Sprite {
		
		//ストリーミングURL そのまんま
		private var videoUrl:String;
		
		//ストリーミングするためのやつ こんな感じのやつがC#とかでも使えればいいのにな
		private var stream:NetStream;
		
		//コネクション 正直何に使ってるのかわからない
		private var connection:NetConnection;
		
		//指定したシークポジション
		private var wantSeekPos:Number;
		
		private var diff:Number = 0;
		
		private var debugText:TextField = new TextField();
		
		
		private function onClick(e:MouseEvent):void {
			
			
			Seek(30);
			trace("onClick");
		}
		
		
		//コンストラクタ
		public function NicoNicoPlayer() {
			
			
			//stage.addEventListener(MouseEvent.CLICK, onClick);
			
			
			//枠に合わせてスケールする
			stage.scaleMode = StageScaleMode.EXACT_FIT;
			
			
			var format:TextFormat = new TextFormat();
			format.color = 0xFFFFFF;
			debugText.defaultTextFormat = format;
			debugText.text = "スタート";
			
			//addChild(debugText);
				
			//JSコールバック登録
			if(ExternalInterface.available) {
				
				//コールバック登録
				ExternalInterface.addCallback("AsOpenVideo", OpenVideo);
				ExternalInterface.addCallback("AsPause", Pause);
				ExternalInterface.addCallback("AsResume", Resume);
				ExternalInterface.addCallback("AsSeek", Seek);
				
			}
			
			//OpenVideo("Z:/smile.mp4");
			//var now:Date = new Date();
			//OpenVideo("http://mrtska.net/SRNicoNico/sm9?"+ now.time.toString());
			//OpenVideo("http://mrtska.net/SRNicoNico/sm8628149");
			//OpenVideo("http://mrtska.net/SRNicoNico/sm9");
		}
		
		//指定したURLをストリーミング再生する
		private function OpenVideo(videoUrl:String):void {
			
			this.videoUrl = videoUrl;
			connection = new NetConnection();
			connection.addEventListener(NetStatusEvent.NET_STATUS, onConnect);
			connection.connect(null);
			
		}
		
		//そのまんま
		private function Pause():void {
			
			stream.pause();
		}
		private function Resume():void {
			
			stream.resume();
		}
		private function Seek(pos:Number):void {
			
			wantSeekPos = pos;
			stream.seek(pos);
		}
		
		
		
		
		
		
		//ビデオコントロールにストリームを繋ぐ
		private function ConnectStream() {
			
	
			
			debugText.text = "動画再生:" + videoUrl;

			//インスタンス作成
			stream = new NetStream(connection);
			
			
			//イベントリスナ登録
			stream.addEventListener(NetStatusEvent.NET_STATUS, onNetStatus);
			stream.addEventListener(AsyncErrorEvent.ASYNC_ERROR, onAsyncError);
			stream.inBufferSeek = true;
			//ハードウェアデコーダーは使う
			stream.useHardwareDecoder = true;
			var obj:Object = new Object();
			obj.onMetaData = function(param:Object):void {
				
				
				
				for(var propName:String in param){
					trace(propName + "=" + param[propName]);
				}
				
				stage.frameRate = param.videoframerate;
				if(stage.frameRate == 0) {
					
					stage.frameRate = param.framerate;
				}
			}
			
			obj.onCuePoint = function(param:Object):void {
				
				trace("onCuePoint");
				for(var propName:String in param){
					trace(propName + "=" + param[propName]);
				}
				
			}

			obj.onSeekPoint = function(param:Object):void {
				
				trace("onSeekPoint");
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
			
			
			
				
			addEventListener(Event.ENTER_FRAME, onFrame);
			
			
			
			stream.play(videoUrl);
			
		}
		
		private function onFrame(e:Event):void {
				
			// 再生時間を取得
			var value:Number = stream.time;
			  
			// バッファの計算
			var buffer:Number = (stream.bytesLoaded) / (stream.bytesTotal);
			
				
				
			trace("Time:" + stream.time);
			
			
			
			
			fscommand("CsFrame", value + ":" + buffer.toString());
			//trace("value:" + value + " diff:" + this.diff);
			
			if(this.diff != 0) {
				
				stream.step(1000);
				this.diff = 0;
			}
			
		}
		
		private function onAsyncError(e:AsyncErrorEvent):void {
			
			trace("onAsyncError");

			trace(e.text);
		}
		
		private function onNetStatus(e:NetStatusEvent):void {
			
			trace("onNetStatus");
			switch(e.info.code) {
			case "NetStream.Seek.Notify":				
			
				trace("Seek.Notify");
			
				var diff:Number;
			
				if(stream.time < wantSeekPos) {
					
					diff = stream.time - wantSeekPos;
 				} else {
					
					diff = wantSeekPos - stream.time;
				}
				this.diff = diff;
				trace("Diff:" + diff);
			
				break;
			case "NetStream.Seek.Complete":
				
				trace("Seek.Complete");
			
				//stream.step(diff * stage.frameRate);
			
				
				
				break;
			case "NetStream.Buffer.Full":
				
				trace("Buffer.Full");
				break;
			default:
				
				trace("default:" + e.info.code);
				break;
			}
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