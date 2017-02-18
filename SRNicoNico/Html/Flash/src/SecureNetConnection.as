package  {
	
	import flash.display.Loader;
	import flash.display.LoaderInfo;
	import flash.events.Event;
	import flash.net.NetConnection;
	import flash.net.URLRequest;


	public class SecureNetConnection extends Object {
		
		
		private var swfUrl:String;
		
		private var info:LoaderInfo;
		
		private var verifier:*;
		
		public function SecureNetConnection(swfUrl:String) {
			
			this.swfUrl = swfUrl;
		}

		public function get version() : String {
			return "201111091500";
		}
		
		public function getSecureNetConnection(func:Function):NetConnection {
			
			var ret:NetConnection;
			
			loadSWF(function(o:*):void {

				this.info = o;
				var obj:* = this.info.content;
				
				var creatorClass:Class = this.info.applicationDomain.getDefinition("jp.nicovideo.util.stream.secure::ISecureNetConnectionCreatable");
				var verifierClass:Class = this.info.applicationDomain.getDefinition("jp.nicovideo.util.stream.secure::SecureNetConnectionVerifiar");
				verifier = new verifierClass();
				
				var a:* = obj as creatorClass;
				func(a.create(verifier));
			});
			return ret;
		}
		
		
		
		
		public function loadSWF(process:Function) : void {
			
			
			var request:URLRequest = new URLRequest(swfUrl); 
			var loader:Loader = new Loader();
			var info:LoaderInfo = loader.contentLoaderInfo;
			info.addEventListener(Event.INIT, function(e:Event):void {
				
				

				
				process(info);
			});
			
			loader.load(request);
			
		}
	}
	
}