using Codeplex.Data;
using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoCommentEntry : IComparable<NicoNicoCommentEntry> {

        public static readonly Dictionary<string, string> NicoNicoOfficialCommentColor = new Dictionary<string, string>();

        public static readonly Regex TripletColor = new Regex(@"#[\d|A-F]{6}");
        public static readonly Regex DurationRegex = new Regex(@"@(\d+)");

        static NicoNicoCommentEntry() {

            //---全会員共通---
            NicoNicoOfficialCommentColor["white"] = "#FFFFFF";
            NicoNicoOfficialCommentColor["red"] = "#FF0000";
            NicoNicoOfficialCommentColor["pink"] = "#FF8080";
            NicoNicoOfficialCommentColor["orange"] = "#FFC000";
            NicoNicoOfficialCommentColor["yellow"] = "#FFFF00";
            NicoNicoOfficialCommentColor["green"] = "#00FF00";
            NicoNicoOfficialCommentColor["cyan"] = "#00FFFF";
            NicoNicoOfficialCommentColor["blue"] = "#0000FF";
            NicoNicoOfficialCommentColor["purple"] = "#C000FF";
            NicoNicoOfficialCommentColor["black"] = "#000000";
            //------

            //---プレミアム会員のみ---
            NicoNicoOfficialCommentColor["white2"] = "#CCCC99";
            NicoNicoOfficialCommentColor["niconicowhite"] = NicoNicoOfficialCommentColor["white2"];

            NicoNicoOfficialCommentColor["red2"] = "#CC0033";
            NicoNicoOfficialCommentColor["truered"] = NicoNicoOfficialCommentColor["red2"];

            NicoNicoOfficialCommentColor["pink2"] = "#FF33CC";

            NicoNicoOfficialCommentColor["orange2"] = "#FF6600";
            NicoNicoOfficialCommentColor["passionorange"] = NicoNicoOfficialCommentColor["orange2"];

            NicoNicoOfficialCommentColor["yellow2"] = "#999900";
            NicoNicoOfficialCommentColor["madyellow"] = NicoNicoOfficialCommentColor["yellow2"];

            NicoNicoOfficialCommentColor["green2"] = "#00CC66";
            NicoNicoOfficialCommentColor["elementalgreen"] = NicoNicoOfficialCommentColor["green2"];

            NicoNicoOfficialCommentColor["cyan2"] = "#00CCCC";

            NicoNicoOfficialCommentColor["blue2"] = "#3399FF";
            NicoNicoOfficialCommentColor["marineblue"] = NicoNicoOfficialCommentColor["blue2"];

            NicoNicoOfficialCommentColor["purple2"] = "#6633CC";
            NicoNicoOfficialCommentColor["nobleviolet"] = NicoNicoOfficialCommentColor["purple2"];

            NicoNicoOfficialCommentColor["black2"] = "#666666";
        }

        //コメント番号
        public int Number { get; set; }

        //コメント再生位置 デシ秒
        public int Vpos { get; set; }
        //コメント描画終了位置
        public int Vend { get; set; }

        //コメント投稿時間 Unixタイム
        public long PostedAt { get; set; }

        //削除されたか
        public bool Deleted { get; set; }

        //匿名コメントかどうか
        public bool Anonymity { get; set; }

        //コメント
        public string Content { get; set; }

        //Mail
        public string Mail { get; set; } = "";

        //ユーザーID
        public string UserId { get; set; }

        //NGスコア
        public int Score { get; set; }

        //NGスコアが閾値を超過してたら
        public bool Rejected { get; set; }

        //コメント位置
        public string Position { get; set; }

        //コメント拡大率
        public double Scale { get; set; }

        //コメントカラー
        public string CommentColor { get; set; }

        //コメントサイズ
        public string CommentSize { get; set; }

        //透明度
        public double Opacity { get; set; }

        //コメントフォント
        public string Font { get; set; }
        public bool Full { get; set; }

        //投稿した直後か
        public bool JustPosted { get; set; }

        //投稿者コメントか
        public bool IsUploader { get; set; }

        //コメント表示時間 4秒か3秒なんだけど投コメとかで秒数指定してるやつもあるし
        public int Duration { get; set; }

        public string DefaultCommentSize { get; set; }
        public bool IsRendering { get; internal set; }

        public int CompareTo(NicoNicoCommentEntry other) {

            if (Vpos == other.Vpos) {

                return Number.CompareTo(other.Number);
            }
            return Vpos.CompareTo(other.Vpos);
        }

        //Mailをバラして各パラメータに入れる
        internal void DisassembleMail() {

            if (Mail.Contains("ue")) {

                Position = "ue";
                Duration = 3000;
            } else if (Mail.Contains("shita")) {

                Position = "shita";
                Duration = 3000;
            } else {

                Position = "naka";
                Duration = 4000;
            }
            Vend = Vpos + (Duration / 10);

            CommentColor = "#FFFFFF";

            //色を反映させる
            foreach (var key in NicoNicoOfficialCommentColor.Keys) {

                if (Mail.Contains(key)) {

                    CommentColor = NicoNicoOfficialCommentColor[key];
                    break;
                }
            }

            //#xxxxxxで指定された色を取得する
            var result = TripletColor.Match(Mail);
            if (result.Success) {

                CommentColor = result.Value;
            }

            //コメントサイズ
            if (Mail.Contains("big")) {

                CommentSize = "big";
            } else if (Mail.Contains("small")) {

                CommentSize = "small";
            } else {

                CommentSize = "regular";
            }

            //フォントを設定
            if (Mail.Contains("gothic")) {

                Font = "gothic";
            } else if (Mail.Contains("mincho")) {

                Font = "mincho";
            } else {

                Font = "defont";
            }

            if (Mail.Contains("full")) {

                Full = true;
            }

            //投稿者コメントの秒数指定コメント
            if (IsUploader) {

                //@は半角にしましょうね
                if (Content.StartsWith("＠")) {

                    Content = Content.Replace("＠", "@");
                }
                var dur = DurationRegex.Match(Mail);
                if (dur.Success) {

                    //投コメの時間調整
                    Duration = int.Parse(dur.Groups[1].Value) * 1000;
                    Vend = Vpos + (Duration / 10);
                }
            }

            DefaultCommentSize = Settings.Instance.CommentSize;
            Opacity = Settings.Instance.CommentAlpha / 100.0;

            //改行の数を数えて
            Scale = IsOverflowHeight(Content.ToList().Where(c => c.Equals('\n')).Count() + 1) ? 0.5 : 1.0;
        }

        //高さがオーバーフローしてるか offsetはコメントの行数
        private bool IsOverflowHeight(int offset) {

            if (offset == 1) {

                return false;
            }
            if (Mail.Contains("big")) {

                return offset >= 3;
            } else if (Mail.Contains("small")) {

                return offset >= 7;
            } else {

                return offset >= 5;
            }
        }
        public string ToJson() {

            return DynamicJson.Serialize(this);
        }
    }
}
