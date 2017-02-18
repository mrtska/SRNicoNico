using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoScriptDefault : NicoScriptBase {

        private readonly Regex ScriptPattern = new Regex(@"(\@.+?)($| )");

        private static readonly string CommentColorPattern;

        static NicoScriptDefault() {

            CommentColorPattern = "";
            foreach(var key in NicoNicoCommentEntry.NicoNicoOfficialCommentColor.Keys) {

                CommentColorPattern += key + "|";
            }

            CommentColorPattern = CommentColorPattern.Substring(0, CommentColorPattern.Length - 1);
        }

        public NicoScriptDefault(VideoCommentViewModel vm, NicoNicoCommentEntry entry) : base(vm, entry, true) {

        }

        public override void Execute(NicoNicoCommentEntry target) {

            //適用するコメントのMailにコマンドが指定してなかったら＠デフォルトコマンドの値を入れる
            if(!Regex.IsMatch(target.Mail, @"ue|shita|naka")) {

                if(Entry.Mail.Contains("ue")) {

                    target.Position = "ue";
                } else if(Entry.Mail.Contains("shita")) {

                    target.Position = "shita";
                } else {

                    target.Position = "naka";
                }
            }

            //適用するコメントのMailに色関連のコマンドがなければデフォルトコマンドの値を
            if(!Regex.IsMatch(target.Mail, CommentColorPattern) && !NicoNicoCommentEntry.TripletColor.IsMatch(target.Mail)) {

                //色を反映させる
                foreach(var key in NicoNicoCommentEntry.NicoNicoOfficialCommentColor.Keys) {

                    if(Entry.Mail.Contains(key)) {

                        target.CommentColor = NicoNicoCommentEntry.NicoNicoOfficialCommentColor[key];
                        break;
                    }
                }

                //#xxxxxxで指定された色を取得する
                var result = NicoNicoCommentEntry.TripletColor.Match(Entry.Mail);
                if(result.Success) {

                    target.CommentColor = result.Value;
                }
            }

            //適用するコメントのMail（ry
            if(!Regex.IsMatch(target.Mail, "big|small|medium")) {

                //コメントサイズ
                if(Entry.Mail.Contains("big")) {

                    target.CommentSize = "big";
                } else if(Entry.Mail.Contains("small")) {

                    target.CommentSize = "small";
                } else {

                    target.CommentSize = "regular";
                }
            }

            //適（ry
            if(!Regex.IsMatch(target.Mail, "defont|gothic|mincho")) {

                //フォントを設定
                if(Entry.Mail.Contains("gothic")) {

                    target.Font = "gothic";
                } else if(Entry.Mail.Contains("mincho")) {

                    target.Font = "mincho";
                } else {

                    target.Font = "defont";
                }
            }
        }
    }
}
