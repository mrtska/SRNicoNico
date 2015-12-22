package {
	import flash.display.Sprite;
	import flash.display.StageScaleMode;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.events.NetStatusEvent;
	import flash.events.TimerEvent;
	import flash.external.ExternalInterface;
	import flash.system.fscommand;
	import flash.ui.Mouse;
	import flash.utils.Timer;
	
	import comment.CommentRasterizer;
	
	//抽象クラスのように使う
	public class NicoNicoPlayerBase extends Sprite {
		
		
		
		//コメントラスタライザ
		public var rastarizer:CommentRasterizer;
		
		public function NicoNicoPlayerBase() {
			
			stage.color = 0x000000;
			//枠に合わせてスケールする
			stage.scaleMode = StageScaleMode.SHOW_ALL;
			stage.frameRate = 30;
			
			//JSコールバック登録
			if(ExternalInterface.available) {
				
				//コールバック登録
				ExternalInterface.addCallback("AsOpenVideo", OpenVideo);
				ExternalInterface.addCallback("AsPause", Pause);
				ExternalInterface.addCallback("AsResume", Resume);
				ExternalInterface.addCallback("AsSeek", Seek);
				ExternalInterface.addCallback("AsInjectComment", InjectComment);
				ExternalInterface.addCallback("AsToggleComment", ToggleComment);
				ExternalInterface.addCallback("AsChangeVolume", ChangeVolume);
			}
			
			rastarizer = new CommentRasterizer();
			
			rastarizer.updateBounds(stage.stageWidth, stage.stageHeight);
			addChild(rastarizer);
			
			//OpenVideo("Z:/smile.flv");
			
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
			loader.load(req);*/
			timer = new Timer(1200);
			timer.addEventListener(TimerEvent.TIMER, tick);
			timer.start();
			
			
			stage.addEventListener(MouseEvent.MOUSE_MOVE, move);
		}
		
		private var timer:Timer;
		private var isRollOver:Boolean;
		
		private function move(e:MouseEvent):void {
			
			timer.reset();
			timer.start();
			
			isRollOver = true;
			Mouse.show();
			fscommand("ShowContoller");
		
			if (!stage.hasEventListener(Event.MOUSE_LEAVE)) {

				stage.addEventListener(Event.MOUSE_LEAVE, leave);
			}
		}
		
		private function leave(e:Event):void {
		
			isRollOver = false;
			stage.removeEventListener(Event.MOUSE_LEAVE, leave);
		}
		
		private function tick(e:TimerEvent):void {
			
			if (isRollOver) {
			
				Mouse.hide();
				fscommand("HideContoller");
			}
			
		}
		
		
		
		
		
		
		
		
		//指定したURLをストリーミング再生する オーバーライドして使う
		public function OpenVideo(videoUrl:String):void {}
		public function Pause():void {}
		public function Resume():void {}
		public function Seek(pos:Number):void {}
		public function InjectComment(json:String):void {
		
			rastarizer.load(json);
		}
		
		public function onFrame(e:Event):void {
		
		}
		public function onNetStatus(e:NetStatusEvent):void {
		
			fscommand("NetStatus", e.info.code);
		}
		
		
		public function ToggleComment():void {
			
			trace("とぐる");
			if(rastarizer.visible) {
				
				rastarizer.visible = false;
			} else {
				
				rastarizer.visible = true;
			}
		}
		public function ChangeVolume(vol:Number):void {}		
		
		
	}
}