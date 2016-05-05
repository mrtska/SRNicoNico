using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {

	public class CommentEntryViewModel {

		public NicoNicoCommentEntry Entry { get; private set; }

        public VideoViewModel Owner;

		public CommentEntryViewModel(NicoNicoCommentEntry entry, VideoViewModel vm) {

			Entry = entry;
            Owner = vm;
        }
	}
}
