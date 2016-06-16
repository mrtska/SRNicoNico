package {
	import flash.display.Sprite;
	import flash.display.Stage;
	import flash.display.StageScaleMode;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.events.NetStatusEvent;
	import flash.events.TimerEvent;
	import flash.external.ExternalInterface;
	import flash.ui.Mouse;
	import flash.utils.Timer;
	import flash.system.Security;
	import comment.CommentRasterizer;
	import flash.net.URLRequest;
	import flash.errors.IOError;
	import flash.system.FSCommand;
	
	
	//抽象クラスのように使う
	public class NicoNicoPlayerBase extends Sprite {
		
		
		//コメントラスタライザ
		public var rasterizer:CommentRasterizer;
		
		
		protected var renderTick:Timer;
		
		
		public function NicoNicoPlayerBase() {
			
			Security.allowInsecureDomain("*");
			Security.allowDomain("*");
			
			this.renderTick = new Timer(16.6 * 2);
			
			//JSコールバック登録
			if (ExternalInterface.available) {

				//コールバック登録
				ExternalInterface.addCallback("AsOpenVideo", OpenVideo);
				ExternalInterface.addCallback("AsPause", Pause);
				ExternalInterface.addCallback("AsResume", Resume);
				ExternalInterface.addCallback("AsSeek", Seek);
				ExternalInterface.addCallback("AsInjectComment", InjectComment);
				ExternalInterface.addCallback("AsInjectOneComment", InjectOneComment);
				ExternalInterface.addCallback("AsInjectUploaderComment", InjectUploaderComment);
				ExternalInterface.addCallback("AsToggleComment", ToggleComment);
				ExternalInterface.addCallback("AsInjectMyComment", InjectMyComment);
				ExternalInterface.addCallback("AsChangeVolume", ChangeVolume);
				ExternalInterface.addCallback("AsApplyChanges", ApplyChanges);

			}
				
			
			//枠に合わせてスケールする
			stage.scaleMode = StageScaleMode.SHOW_ALL;
			stage.frameRate = 30;
			
			rasterizer = new CommentRasterizer();
			stage.showDefaultContextMenu = false;
			
			
			rasterizer.updateBounds(stage.stageWidth, stage.stageHeight);
			
			
			timer = new Timer(1200);
			timer.addEventListener(TimerEvent.TIMER, tick);
			timer.start();
			
			
			stage.addEventListener(MouseEvent.MOUSE_MOVE, move);
			stage.addEventListener(MouseEvent.CLICK, click);

			CallCSharp("Ready");

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
		
		private function click(e:MouseEvent):void {
			
			CallCSharp("Click");
		}
		
		//指定したURLをストリーミング再生する オーバーライドして使う
		public function OpenVideo(videoUrl:String, config:String):void {}
		public function Pause():void {}
		public function Resume():void {}
		public function Seek(pos:Number):void {}
		public function InjectComment(json:String):void {
		
			
			rasterizer.loadComment(json);
		}
		public function InjectOneComment(json:String):void {
		
				
			rasterizer.loadOneComment(json);
		}
		public function InjectMyComment(json:String):void {
		
				
			rasterizer.loadMyComment(json);
		}
		public function ApplyChanges(json:String):void {
			
			rasterizer.applyChanges(json);
		}
		
		public function InjectUploaderComment(json:String):void {
		
				
			rasterizer.loadUploaderComment(json);
		}
		
		
		public function onFrame(e:TimerEvent):void {
		
		}
		public function onNetStatus(e:NetStatusEvent):void {
		
		}
		
		
		public function ToggleComment():void {
			
			if(rasterizer.visible) {
				
				rasterizer.visible = false;
			} else {
				
				rasterizer.visible = true;
			}
		}
		public function ChangeVolume(vol:Number):void { }		
		
		
		/*public function CallCSharp(func:String):void {
		
			if (ExternalInterface.available) {
				
				ExternalInterface.call("window.AsScriptHandler", func);
			}
		}*/

			
		public function CallCSharp(func:String, ...arg):void {
		
			if (ExternalInterface.available) {
				
				switch(arg.length) {
					case 0:
						ExternalInterface.call.apply(null, ["window.AsScriptHandler", func]);
						break;
					case 1:
						ExternalInterface.call.apply(null, ["window.AsScriptHandler2", func, arg[0]]);
						break;
					case 2:
						ExternalInterface.call.apply(null, ["window.AsScriptHandler3", func, arg[0], arg[1]]);
						break;
					case 3:
						ExternalInterface.call.apply(null, ["window.AsScriptHandler4", func, arg[0], arg[1], arg[2]]);
						break;
					case 4:
						ExternalInterface.call.apply(null, ["window.AsScriptHandler5", func, arg[0], arg[1], arg[2], arg[3]]);
						break;
					
					
					
				}
				
			}
		}
		
	}
}