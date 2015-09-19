package {
	import flash.display.Sprite;
	import flash.display.StageScaleMode;
	import flash.external.ExternalInterface;
	
	//抽象クラスのように使う
	public class NicoNicoPlayerBase extends Sprite {
		
		
		
		//コメントラスタライザ
		public var rastarizer:CommentRasterizer;
		
		public function NicoNicoPlayerBase() {
			
			stage.color = 0x000000;
			//枠に合わせてスケールする
			stage.scaleMode = StageScaleMode.SHOW_ALL;
			
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
		}
		
		//指定したURLをストリーミング再生する オーバーライドして使う
		public function OpenVideo(videoUrl:String):void {}
		public function Pause():void {}
		public function Resume():void {}
		public function Seek(pos:Number):void {}
		public function InjectComment(json:String):void {
		
			rastarizer.load(json);
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