package comment {
	import flash.display.Sprite;
	
	public class CommentRasterizer extends Sprite {
		
		//コメントの位置を決定する
		private var positioner:CommentPositioner;
		
		//コメントリスト
		private var commentList:Vector.<CommentEntry>;
		
		//描画中のコメントリスト
		private var drawingList:Vector.<CommentEntry>;
		
		//次に見るコメントの位置を覚えておく
		private var nextIndex:uint = 0;
		
		public function CommentRasterizer() {
			
			commentList = new Vector.<CommentEntry>();
			drawingList = new Vector.<CommentEntry>();
		}
		
		//フィールド初期化
		public function updateBounds(width:int, height:int):void {
			
			positioner = new CommentPositioner(width, height, drawingList);
		}
		
		
		
		//コメントリストをC#からもらう
		public function load(string:String):void {
			
			var json:Object = JSON.parse(string);
			
			for each(var obj:Object in json.array) {
				
				commentList.push(new CommentEntry(obj.No, obj.Vpos, obj.Mail, obj.Content));
			}
		}

		public function render(vpos:Number):void {
			
			for(var i:int = nextIndex; i < commentList.length; i++) {
				
				var target:CommentEntry = commentList[i];
				
				//描画時間になったら
				if(vpos >= target.vpos && vpos < target.vend) {
					
					target.x = positioner.getX(target, vpos);
					
					if(drawingList.indexOf(target) >= 0) {
						
						continue;
					}
					
					target.y = positioner.getY(target);
					drawingList.push(target);
					
					addChild(target);
				}
			}
			
			//描画中のリストを走査
			for each(var entry:CommentEntry in drawingList) {
				
				//コメントの描画時間が終わったら
				if(vpos < entry.vpos || vpos > entry.vend) {
					
					drawingList.splice(drawingList.indexOf(entry), 1);
					removeChild(entry);
				}
			}
		}
	}
}