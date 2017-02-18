using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoViewer {
    public interface IWatchable {

        bool IsWatched { get; set; }

        string ContentUrl { get; set; }

    }
}
