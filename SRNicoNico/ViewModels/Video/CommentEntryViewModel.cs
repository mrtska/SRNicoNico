using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {

	public class CommentEntryViewModel {

		public NicoNicoCommentEntry Entry { get; private set; }

		public CommentEntryViewModel(NicoNicoCommentEntry entry) {

			Entry = entry;
		}
	}
}
