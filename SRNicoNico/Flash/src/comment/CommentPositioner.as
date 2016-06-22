package comment {
	public class CommentPositioner {
		
		//Flashの横幅
		private var width:Number;
		
		//Flashの縦幅
		private var height:Number;
		
		//描画中のコメントリスト
		private var drawingComment:Vector.<CommentEntry>;
		
		public function CommentPositioner(width:Number, height:Number, drawingComment:Vector.<CommentEntry>) {
		
			this.width = width;
			this.height = height;
			this.drawingComment = drawingComment;
		}
		
		public function updateBounds(width:Number, height:Number):void {
			
			this.width = width;
			this.height = height;
		}
		
		
		//現在のvposからそのコメントのX座標を取得する
		public function getX(entry:CommentEntry, vpos:Number):Number {
			
			//流れない系のコメントのX座標は一定なのでこれ
			if(entry.command.pos != CommentCommand.PLACE_NAKA) {
				
				return (this.width - entry.width) / 2;
			}
			
			//差分
			var sub:Number = vpos - entry.vpos;
			var ret:Number = (this.width + entry.width) / (entry.vend - entry.vpos);
			
			return this.width - sub * ret;
		}
		
		public function getY(entry:CommentEntry):Number {
			
			var flag:Boolean = false;
			
			//Y座標
			var offsetY:Number = 0;
			
			//下コメだったら現在の高さからコメントの高さを引けばいいよね
			if(entry.command.pos == CommentCommand.PLACE_SHITA) {
				
				offsetY = this.height - entry.height;
			}
			
			do {
				flag = false;
				var count:int = 0;
				
				//描画中のリストを走査して描画できるY座標を特定する
				for each(var target:CommentEntry in drawingComment) {
					
					//描画したいコメントと同じだったらやり直し
					if(entry.no == target.no) {
						
						continue;
					}
					
					//同じだったら
					if(entry.command.pos == target.command.pos) {
						
						//候補のY座標がすでに使われていたら
						if(target.y + target.height > offsetY) {
							
							//ターゲットよりも下に描画する必要があるからoffsetYを変更する
							if(offsetY + entry.height > target.y) {
								
								//下コメだったら
								if(entry.command.pos == CommentCommand.PLACE_SHITA) {
									
									//すでに描画されているコメントの上に描画する
									offsetY = target.y - entry.height - 1;
									
									//描画出来る位置が無かったら仕方ないのでテキトーに位置に描画する
									if(offsetY < 0) {
										
										offsetY = Math.random() * (this.height - entry.height);
										break;
									}
									
									flag = true;
									break;
								}

								//上コメだったら
								if(entry.command.pos == CommentCommand.PLACE_SHITA) {
									
									//ターゲットより下に描画する
									offsetY = target.y - entry.height + 1;
									
									//描画したい位置が下に突き抜けたら仕方ない
									if(offsetY + entry.height > this.height) {
										
										offsetY = Math.random() * (this.height - entry.height);
										break;
									}
									
									flag = true;
									break;
								}
								
								//中コメ
								var max:* = Math.max(entry.vpos, target.vpos);
								var min:* = Math.min(entry.vend, target.vend);
								var x1:* = getX(entry, max);
								var x2:* = getX(entry, min);
								var x3:* = getX(target, max);
								var x4:* = getX(target, min);
								
								if(x1 <= x3 + target.width && x3 <= x1 + entry.width || x2 <= x4 + target.width && x4 <= x2 + entry.width) {
									
									offsetY = target.y + target.height + 1;
									
									if(offsetY + entry.height > this.height) {
										
										offsetY = Math.random() * this.height - entry.height;
										break;
									}
									flag = true;
									break;
								}
							}
						}
					}
					count++;
				}
			} while(flag);
			
			return offsetY;
		}
	}
}