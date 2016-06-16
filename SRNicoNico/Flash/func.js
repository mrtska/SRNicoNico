
document.addEventListener('DOMContentLoaded', function () {


    window.AsScriptHandler = function (command) {

        external.invokeFromActionScript(String(command));
    }
    window.AsScriptHandler2 = function (command, arg0) {

        external.invokeFromActionScript(String(command), arg0);
    }
    window.AsScriptHandler3 = function (command, arg0, arg1) {

        external.invokeFromActionScript(String(command), arg0, arg1);
    }
    window.AsScriptHandler4 = function (command, arg0, arg1, arg2) {

        external.invokeFromActionScript(String(command), arg0, arg1, arg2);
    }
    window.AsScriptHandler5 = function (command, arg0, arg1, arg2, arg3) {

        external.invokeFromActionScript(String(command), arg0, arg1, arg2, arg3);
    }


    var flash = document.NicoNicoPlayer;

    window.AsExecuteInstruction = function AsExecuteInstruction(methodName) {

        flash[methodName]();
    }
    window.AsExecuteInstruction1 = function AsExecuteInstruction1(methodName, args) {

        flash[methodName](args);
    }

    window.AsExecuteInstruction2 = function AsExecuteInstruction2(methodName, args0, args1) {

        flash[methodName](args0, args1);
    }

    window.AsExecuteInstruction3 = function AsExecuteInstruction3(methodName, args0, args1, args2) {

        flash[methodName](args0, args1, args2);
    }

});