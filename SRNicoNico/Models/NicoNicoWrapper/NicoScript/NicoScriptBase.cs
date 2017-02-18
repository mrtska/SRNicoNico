using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public abstract class NicoScriptBase {

        public readonly NicoNicoCommentEntry Entry;

        //他のコメントに影響を与える系のニコスクリプトかどうか
        public readonly bool AffectOtherComments;

        protected readonly VideoCommentViewModel Owner;

        public NicoScriptBase(VideoCommentViewModel vm, NicoNicoCommentEntry entry, bool aoc) {

            Owner = vm;
            Entry = entry;
            AffectOtherComments = aoc;
        }

        public bool ShouldExecuteTime(int vpos) {

            if(vpos >= Entry.Vpos && vpos <= Entry.Vend) {

                return true;
            }
            return false;
        }

        //実行されたとき
        public abstract void Execute(NicoNicoCommentEntry target);

        public void ExecuteIfValidTime(NicoNicoCommentEntry target) {

            if(ShouldExecuteTime(target.Vpos)) {

                Execute(target);
            }
        }
    }
}
