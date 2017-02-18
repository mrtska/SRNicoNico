using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {

    //@置換 ニコスクリプト 本家仕様が破綻しているので見送り
#if false
    public class NicoScriptReplace : NicoScriptBase {

        //比較対象
        private string Compare;

        //置換後の文字列
        private string ReplaceWith;

        //置換範囲
        private string Range = "単";

        //対象
        private string Target = "コメ";

        //条件
        private string Condition = "部分一致";

        //イカれたスクリプト対策
        private bool Ready = true;
        
        public NicoScriptReplace(NicoNicoCommentEntry entry) : base(entry) {

            try {
                var splited = entry.Content.Split(' ');

                Compare = splited[1];

                for(int i = 2; i < splited.Length; i++) {

                    var str = splited[i];

                    if(str == "全") {

                        Range = "全";
                    }

                }

            } catch(Exception) {

                Ready = false;
            }
        }

        public override void Execute(NicoNicoCommentEntry target) {

            if(Ready) {


            }
        }
    }
#endif
}
