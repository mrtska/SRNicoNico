package  {
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.events.HTTPStatusEvent;
	import flash.geom.Point;
	import flash.net.NetStream;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.text.AntiAliasType;
	import flash.text.TextField;
	import flash.text.TextFieldAutoSize;
	import flash.text.engine.ElementFormat;
	import flash.text.engine.FontDescription;
	import flash.text.engine.TextBlock;
	import flash.text.engine.TextElement;
	import flash.text.engine.TextLine;
	
	
	
	
	
	public class CommentRasterizer extends Sprite {
		
		
		
		private var comment:XML;
		
		
		private var stream:NetStream;
		
		private var test:TextBlock = new TextBlock();
		
		private var line:TextLine;
		
		public function CommentRasterizer(stream:NetStream) {
			
			this.stream = stream;
			
			var loader:URLLoader = new URLLoader();
			
			var request:URLRequest = new URLRequest("Z:/msg.txt");
			
			loader.addEventListener(Event.COMPLETE, onComplete);
			
			loader.load(request);
			
			addEventListener(Event.ENTER_FRAME, onFrame);
			var str:String = "The quick brown fox jumps over the lazy dog\nいろはにほへと　ちりぬるを\nわかよたれそ　つねならむ\nうゐのおくやま　けふこえて\nあさきゆめみし　ゑひもせす";
			
			var font_regular:FontDescription = new FontDescription('Meiryo', 'normal');
			var font_bold:FontDescription = new FontDescription('Meiryo', 'bold');
			var fmt:ElementFormat = new ElementFormat();
			fmt.locale = 'ja';
			fmt.fontDescription = font_regular;
			fmt.fontSize = 24;
			fmt.color = 0x333333;
			
			var elem:TextElement = new TextElement(str, fmt);
			var block:TextBlock = new TextBlock();
			block.content = elem;
			var width:int = 320; //px
			var linespace:int = 5; //px
			var offset:Point = new Point(0, 24); //textblockの左上端
			var next:Point = new Point(0, 0);
			
			line = block.createTextLine(null, width);
			
			while (line !== null) {
				line.x = next.x + offset.x;
				line.y = next.y + offset.y;
				next.y += line.ascent + line.descent + linespace; //次の行の位置を指定する  横書きのとき
				addChild(line); //実行しないと画面を更新しない
				
				line = block.createTextLine(line, width);
			}
		}
		
		
		private function onFrame(e:Event):void {
			
			var cur:uint = stream.time * 100;
			//line.x += 1;
			trace(cur);
		}
		
		public function onComplete(e:Event):void {
			
			
			var loader:URLLoader = e.currentTarget as URLLoader;
			
			comment = new XML(loader.data);
			var vec:Vector = new Vector();
		
			trace(comment.children().children());
			
		}
	}
}