package {
	import flash.display.Sprite;
	import flash.net.NetStream;
	
	import jp.nicovideo.nicoplayer.views.comment.CommentPositioner;
	import jp.nicovideo.nicoplayer.views.comment.CommentSlotList;

	public class CommentRasterizer extends Sprite {
		
		
		
		private var positioner:CommentPositioner;
		
		private var stream:NetStream;
		
		private var slot:CommentSlotList;
		
		public function CommentRasterizer(width:int, height:int, stream:NetStream) {
			
			this.stream = stream;
			positioner = new CommentPositioner(width, height);
		
			
			slot = new CommentSlotList(positioner, stream.info.metaData.duration);
			
		}
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
	}
}