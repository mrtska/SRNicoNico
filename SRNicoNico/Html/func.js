// Flash コマンドですべての FSCommand メッセージを処理する
function NicoNicoPlayer_DoFSCommand(command, args) {
	//
	// コードをここに配置します。
	//
	window.external.InvokeFromJavaScript(command, args);
}

var flash = document.NicoNicoPlayer;


function JsOpenVideo(url) {
			
			
	flash.AsOpenVideo(url);
}

function JsPause() {
	
	flash.AsPause();
}

function JsResume() {
	
	flash.AsResume();
}
function JsSeek(pos) {
	
	flash.AsSeek(pos);
}

function JsInjectComment(xml) {
	
	flash.AsInjectComment(xml);
}
function JsToggleComment() {
	
	flash.AsToggleComment();
}