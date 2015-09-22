package comment {
	import flash.utils.Dictionary;

	public class CommentCommand {
		
		//コメント位置
		public static const PLACE_UE:String = "ue";
		public static const PLACE_NAKA:String = "naka";
		public static const PLACE_SHITA:String = "shita";
		
		//フォントサイズ
		public static const BIG_FONT_SIZE:Number = 39;
		public static const MEDIUM_FONT_SIZE:Number = 24;
		public static const SMALL_FONT_SIZE:Number = 15;
		
		
		//ニコニコ上で使える一般的なカラーマップ（#指定除く）
		private static const NicoNicoColorMap:Dictionary = new Dictionary();
		
		
		{
			//---全会員共通---
			NicoNicoColorMap["white"] = 0xFFFFFF;
			NicoNicoColorMap["red"] = 0xFF0000;
			NicoNicoColorMap["pink"] = 0xFF8080;
			NicoNicoColorMap["orange"] = 0xFFC000;
			NicoNicoColorMap["yellow"] = 0xFFFF00;
			NicoNicoColorMap["green"] = 0x00FF00;
			NicoNicoColorMap["cyan"] = 0x00FFFF;
			NicoNicoColorMap["blue"] = 0x0000FF;
			NicoNicoColorMap["purple"] = 0xC000FF;
			NicoNicoColorMap["black"] = 0x000000;
			//------
			
			//---プレミアム会員のみ---
			NicoNicoColorMap["white2"] = 0xCCCC99;
			NicoNicoColorMap["niconicowhite"] = NicoNicoColorMap["white2"];
			
			NicoNicoColorMap["red2"] = 0xCC0033;
			NicoNicoColorMap["truered"] = NicoNicoColorMap["red2"];
			
			NicoNicoColorMap["pink2"] = 0xFF33CC;
			
			NicoNicoColorMap["orange2"] = 0xFF6600;
			NicoNicoColorMap["passionorange"] = NicoNicoColorMap["orange2"];
			
			NicoNicoColorMap["yellow2"] = 0x999900;
			NicoNicoColorMap["madyellow"] = NicoNicoColorMap["yellow2"];
			
			NicoNicoColorMap["green2"] = 0x00CC66;
			NicoNicoColorMap["elementalgreen"] = NicoNicoColorMap["green2"];
			
			NicoNicoColorMap["cyan2"] = 0x00CCCC;
			
			NicoNicoColorMap["blue2"] = 0x3399FF;
			NicoNicoColorMap["marineblue"] = NicoNicoColorMap["blue2"];
			
			NicoNicoColorMap["purple2"] = 0x6633CC;
			NicoNicoColorMap["nobleviolet"] = NicoNicoColorMap["purple2"];
			
			NicoNicoColorMap["black2"] = 0x666666;
			//------
			
			
		}
		
		//コメントからサイズを取得して返す
		private static function getFontSize(mail:String):uint {
			
			if(mail.indexOf("big") >= 0) {
				
				return BIG_FONT_SIZE;
			} else if(mail.indexOf("small") >= 0) {
				
				return SMALL_FONT_SIZE;
			} else {
				
				return MEDIUM_FONT_SIZE;		
			}
		}
		
		//コメントカラー
		private var _color:uint = 0xFFFFFF;
		
		//コメントサイズ
		private var _size:Number;
		
		//コメントの位置
		private var _pos:String;
		
		//コメント表示時間
		private var _duration:uint;
		
		//
		private var _full:Boolean = false;
		
		public function CommentCommand(mail:String) {
		
			
			//カラーコードが直で書かれていたら
			if(mail.indexOf("#") >= 0) {
				
				var index:int = mail.indexOf("#");
				trace("カラーコード:" + mail.substring(index, 6));
			}
			
			//カラーが書かれていたら
			for(var key:String in NicoNicoColorMap) {
				
				if(mail.indexOf(key) >= 0) {
					
					//trace(key + ":0x" + NicoNicoColorMap[key].toString(16).toUpperCase());
					_color = NicoNicoColorMap[key];
					break;
				}
			}
			
			//フォントサイズを取得
			_size = getFontSize(mail);
			
			//コメントの位置を取得、設定
			if(mail.indexOf("ue") >= 0) {
				
				_pos = "ue";
				_duration = 300;
			} else if(mail.indexOf("shita") >= 0) {
				
				_pos = "shita";
				_duration = 300;
			} else {
				
				_pos = "naka";
				_duration = 400;
			}
			
			if(mail.indexOf("full") >= 0) {
				
				_full = true;
			}
		}
		
		public function get color():uint {
			
			return _color;
		}

		public function get size():Number {
			
			return _size;
		}
		
		public function get pos():String {
			
			return _pos;
		}
		
		public function get duration():uint {
			
			return _duration;
		}
		
		public function get full():Boolean {
			
			return _full;
		}
	}
}