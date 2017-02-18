using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoScriptReverse : NicoScriptBase {

        private string Condition = "全";

        public NicoScriptReverse(VideoCommentViewModel vm, NicoNicoCommentEntry entry) : base(vm, entry, true) {

            foreach(var str in entry.Content.Split(' ')) {

                if(str == "コメ" || str == "投コメ") {

                    Condition = str;
                }
            }
        }

        public override void Execute(NicoNicoCommentEntry target) {

            switch(Condition) {
                case "全":
                    target.Reverse = true;
                    break;
                case "コメ":
                    if(!target.IsUploader) {

                        target.Reverse = true;
                    }
                    break;
                case "投コメ":
                    if(target.IsUploader) {

                        target.Reverse = true;
                    }
                    break;
            }
        }
    }
}
