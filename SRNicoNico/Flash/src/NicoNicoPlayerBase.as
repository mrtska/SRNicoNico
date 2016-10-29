package {
	import flash.display.Sprite;
	import flash.display.StageScaleMode;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.events.NetStatusEvent;
	import flash.events.TimerEvent;
	import flash.external.ExternalInterface;
	import flash.ui.Mouse;
	import flash.utils.Timer;
	import flash.system.FSCommand;
	
	import comment.CommentRasterizer;
	
	
	//抽象クラスのように使う
	public class NicoNicoPlayerBase extends Sprite {
		
		
		protected var renderTick:Timer;
		
		public function NicoNicoPlayerBase() {
			
			stage.color = 0x000000;
			//枠に合わせてスケールする
			stage.scaleMode = StageScaleMode.SHOW_ALL;
			stage.frameRate = 30;
			
			this.renderTick = new Timer(100);
			
			//rasterizer = new CommentRasterizer();
			stage.showDefaultContextMenu = false;
			
			//JSコールバック登録
			if(ExternalInterface.available) {
				
				//コールバック登録
				ExternalInterface.addCallback("AsOpenVideo", OpenVideo);
				ExternalInterface.addCallback("AsPause", Pause);
				ExternalInterface.addCallback("AsResume", Resume);
				ExternalInterface.addCallback("AsSeek", Seek);
				ExternalInterface.addCallback("AsSetVolume", ChangeVolume);
				
			}
			
			//timer = new Timer(1200);
			//timer.addEventListener(TimerEvent.TIMER, tick);
			//timer.start();
			
			
			//stage.addEventListener(MouseEvent.MOUSE_MOVE, move);
			stage.addEventListener(MouseEvent.MOUSE_WHEEL, wheel);
			stage.addEventListener(MouseEvent.CLICK, click);
		}
		
		private var timer:Timer;
		private var isRollOver:Boolean;
		
		public function move(e:MouseEvent):void {
			
			
			timer.reset();
			timer.start();
			
			isRollOver = true;
			Mouse.show();
		
			if (!stage.hasEventListener(Event.MOUSE_LEAVE)) {

				stage.addEventListener(Event.MOUSE_LEAVE, leave);
			}
				
			CallCSharp("ShowController");

		}
		
		public function wheel(e:MouseEvent):void {
			
			ExternalInterface.call("invoke_host", "mousewheel", e.delta);
		}
		
		private function leave(e:Event):void {
		
			isRollOver = false;
			stage.removeEventListener(Event.MOUSE_LEAVE, leave);
		}
		
		private function tick(e:TimerEvent):void {
			
			if (isRollOver) {
			
				Mouse.hide();
				CallCSharp("HideController");
				
			}
			
		}
		
		//クリックして一時停止をハンドリングするために
		private function click(e:MouseEvent):void {
			
			ExternalInterface.call("invoke_host", "click");
		}
		
		//指定したURLをストリーミング再生する オーバーライドして使う
		public function OpenVideo(videoUrl:String, config:String):void {}
		public function Pause():void {}
		public function Resume():void {}
		public function Seek(pos:Number):void {}
		
		
		public function onFrame(e:TimerEvent):void {
		
		}
		public function onNetStatus(e:NetStatusEvent):void {
		
		}
		
		
		public function ChangeVolume(vol:Number):void { }		
		
		
		public function CallCSharp(func:String):void {
		
			if (ExternalInterface.available) {
				
				ExternalInterface.call(func);
			}
		}
		
		
	}
}