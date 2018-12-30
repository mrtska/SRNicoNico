using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoViewer {
    public static class WebSocketExtension {

        public static async Task SendAsync(this ClientWebSocket client, string data, Encoding encoding) {

            var array = new ArraySegment<byte>(encoding.GetBytes(data));
            await client.SendAsync(array, WebSocketMessageType.Text, true, default(CancellationToken));
        }


    }
}
