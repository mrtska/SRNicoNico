package {
	
	
	public class CommentEntry {
		
		//コメントナンバー
		public var no:uint;
		
		//コメント表示時間
		public var vpos:uint;
		
		//コメント装飾
		public var mail:String;
		
		//コメント
		public var content:String;
		
		
		public function CommentEntry(no:uint, vpos:uint, mail:String, content:String) {
		
			this.no = no;
			this.vpos = vpos;
			this.mail = mail;
			this.content = content;	
		}
	}
}