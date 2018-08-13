using Livet;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class PublicMylistEntryViewModel : ViewModel {

        public NicoNicoMylistEntry Item { get; set; }

        internal PublicMylistViewModel Owner;

        public PublicMylistEntryViewModel(PublicMylistViewModel owner, NicoNicoMylistEntry item) {

            Owner = owner;
            Item = item;
        }
    }
}
