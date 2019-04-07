


interface External {

    InvokeFromJavaScript(cmd: string, args: any): void;
}

function InvokeHost(cmd: string, args: any = ""): void {

    if (window.external != null && cmd != null) {

        window.external.InvokeFromJavaScript(cmd, args);
    }
}
eval("window.InvokeHost = InvokeHost");