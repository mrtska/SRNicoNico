package {
	public class CommentSizeCalculator {
		

		
		public static function isOverflowheight(entry:CommentEntry, offsetY:int):Boolean {
			
			if(offsetY == 1) {
				
				return false;
			}
			
			switch(entry.command.size) {
			case CommentCommand.BIG_FONT_SIZE:
				return offsetY >= 3;
			case CommentCommand.SMALL_FONT_SIZE:
				return offsetY >= 7;
			default:
				return offsetY >= 5;
			}
		}
		
		
		
		
		public static function calcScale(entry:CommentEntry, offsetY:int):void {
			
			//高さが収まりきらなかったら縮小する
			entry.scaleX = entry.scaleY = isOverflowheight(entry, offsetY) ? 0.5 : 1;
			if(entry.command.pos != CommentCommand.PLACE_NAKA) {
				
				var width:* = entry.command.full ? 672 : 544;
				if(entry.width > width) {
					
					entry.scaleX = entry.scaleY = width / entry.width;
				}
			}
		}
	}
}