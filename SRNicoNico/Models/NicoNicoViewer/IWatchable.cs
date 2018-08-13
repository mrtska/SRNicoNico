namespace SRNicoNico.Models.NicoNicoViewer {
    public interface IWatchable {

        bool IsWatched { get; set; }

        string ContentUrl { get; set; }
    }
}
