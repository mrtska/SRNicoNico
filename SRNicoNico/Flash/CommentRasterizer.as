package  {
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.events.HTTPStatusEvent;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	
	
	
	
	
	public class CommentRasterizer extends Sprite {
		
		
		
		private var comment:XML;
		
		public function CommentRasterizer() {
			
			var loader:URLLoader = new URLLoader();
			
			var request:URLRequest = new URLRequest("Z:/msg.txt");
			
			loader.addEventListener(Event.COMPLETE, onComplete);
			
			loader.load(request);

		}
		
		public function onComplete(e:Event):void {
			
			
			var loader:URLLoader = e.currentTarget as URLLoader;
			
			comment = new XML(loader.data);
			
			trace(comment.children().children());
			
		}
	}
}