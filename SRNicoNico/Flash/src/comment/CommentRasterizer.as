package comment {
	import flash.display.Sprite;
	import flash.external.ExternalInterface;
	import flash.text.TextFormat;
	
	public class CommentRasterizer extends Sprite {
		
		//コメントの位置を決定する
		private var positioner:CommentPositioner;
		
		//コメントリスト
		private var commentList:Vector.<CommentEntry>;
		
		//描画中のコメントリスト
		private var drawingList:Vector.<CommentEntry>;
		
		//次に見るコメントの位置を覚えておく
		private var nextIndex:uint = 0;
		
		//コメント設定
		private var Hide3DS:Boolean;
		private var HideWiiU:Boolean;
		private var NGLevel:String;
		
		
		public function CommentRasterizer() {
			
			commentList = new Vector.<CommentEntry>();
			drawingList = new Vector.<CommentEntry>();
		}
		
		//フィールド初期化
		public function updateBounds(width:int, height:int):void {
			
			positioner = new CommentPositioner(width, height, drawingList);
		}
		
		public function loadMyComment(string:String):void {
			
			var obj:Object = JSON.parse(string);
			commentList.push(new CommentEntry(obj.No, obj.Vpos, obj.Mail, obj.Content, true, obj.Score));
			
		}
		
		//コメントリストをC#からもらう
		public function loadComment(string:String):void {
			
			var json:Object = JSON.parse(string);
			
			for each(var obj:Object in json.array) {
				
				commentList.push(new CommentEntry(obj.No, obj.Vpos, obj.Mail, obj.Content, false, obj.Score));
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
					
					//投稿者コメントだったら
					if (target.fork) {
						
						addChild(target);
					} else {
						
						addChild(target);
					}
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
		
		public function loadUploaderComment(string:String):void {
			
			
			var json:Object = JSON.parse(string);
			
			for each(var obj:Object in json.array) {
				
				var entry:CommentEntry = new CommentEntry(obj.No, obj.Vpos, obj.Mail, obj.Content, false, obj.Score);
				entry.fork = true;	//投稿者コメントなので
				
				
				commentList.push(entry);
			}
		}
		public function applyChanges(json:String):void {
			
			var obj:Object = JSON.parse(json);
			
			Hide3DS = obj.Hide3DSComment;
			HideWiiU = obj.HideWiiUComment;
			NGLevel = obj.NGSharedLevel;
			
			CommentCommand.UpdateFontSize(obj.DefaultCommentSize);
			//コメントリストを走査して設定を反映させる 実装がキモいので後で直すかも
			for each(var entry:CommentEntry in commentList) {
				
				entry.alpha = obj.CommentAlpha;
				entry.command = new CommentCommand(entry.mail);
				entry.setTextFormat(new TextFormat("Arial", entry.command.size, entry.command.color, true));
				
			}
		}
	}
}