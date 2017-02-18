using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoNicoScriptInterpreter {

        private static readonly Regex ScriptPattern = new Regex(@"(\@.+?)($| )");

        

        public static NicoScriptBase GetScriptInstance(VideoCommentViewModel vm, NicoNicoCommentEntry entry) {

            var match = ScriptPattern.Match(entry.Content);

            if(match.Success) {

                switch(match.Groups[1].Value) {
                    case "@デフォルト":

                        return new NicoScriptDefault(vm, entry);
                    case "@逆":

                        return new NicoScriptReverse(vm, entry);
                    case "@ジャンプ":

                        return new NicoScriptJump(vm, entry);
                    case "@置換":

                        //return new NicoScriptReplace(entry);
                    default:
                        return null;
                }
            } else {

                return null;
            }
        }

    }
}
