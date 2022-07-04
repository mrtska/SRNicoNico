using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Models {
    /// <summary>
    /// コメントパーサー
    /// </summary>
    public static class CommentParser {

        private static Dictionary<string, string> WideIntegerMap = new Dictionary<string, string> {
            ["０"] = "0",
            ["１"] = "1",
            ["２"] = "2",
            ["３"] = "3",
            ["４"] = "4",
            ["５"] = "5",
            ["６"] = "6",
            ["７"] = "7",
            ["８"] = "8",
            ["９"] = "9",
        };

        public static readonly Regex TripletColor = new Regex(@"#[\d|A-F|a-f]{6}");
        /// <summary>
        /// コメントの装飾をパースして使いやすいようにする
        /// </summary>
        /// <param name="entry">コメント</param>
        /// <returns>パースした結果</returns>
        public static object Parse(VideoCommentEntry entry) {

            // Mailが無い時は全てデフォルトの値を使う
            if (entry.Mail == null) {
                return new {
                    threadId = entry.ThreadId,
                    fork = entry.Fork,
                    no = entry.Number,
                    vpos = entry.Vpos,
                    mail = entry.Mail!,
                    content = entry.Content!,
                    fontSize = "medium",
                    fontFamily = "defont",
                    position = "naka",
                    full = false,
                    ender = false,
                    color = "#FFFFFF",
                    duration = 3F,
                    live = false,
                    lineCount = entry.Content!.Split('\n').Length
                };
            }
            var mails = entry.Mail.Split(' ');

            var fontSize = "medium";
            if (mails.Contains("small")) {
                fontSize = "small";
            } else if (mails.Contains("big")) {
                fontSize = "big";
            }

            var fontFamily = "defont";
            if (mails.Contains("mincho")) {
                fontFamily = "mincho";
            } else if (mails.Contains("gothic")) {
                fontFamily = "gothic";
            }

            var position = "naka";
            if (mails.Contains("ue")) {
                position = "ue";
            } else if (mails.Contains("shita")) {
                position = "shita";
            }

            var match = Regex.Match(entry.Mail, @"@([+-]?\d*\.?\d+\.?(?!\d))");
            var duration = 3F;
            if (match.Success) {
                duration = float.Parse(WideIntegerMap.GetValueOrDefault(match.Groups[1].Value, match.Groups[1].Value));
            }

            //#xxxxxxで指定された色を取得する
            var color = "#FFFFFF";
            var result = TripletColor.Match(entry.Mail);
            if (result.Success) {

                color = result.Value;
            }

            if (mails.Contains("white")) {
                color = "#FFFFFF";
            } else if (mails.Contains("red")) {
                color = "#FF0000";
            } else if (mails.Contains("pink")) {
                color = "#FF8080";
            } else if (mails.Contains("orange")) {
                color = "#FFC000";
            } else if (mails.Contains("yellow")) {
                color = "#FFFF00";
            } else if (mails.Contains("green")) {
                color = "#00FF00";
            } else if (mails.Contains("cyan")) {
                color = "#00FFFF";
            } else if (mails.Contains("blue")) {
                color = "#0000FF";
            } else if (mails.Contains("purple")) {
                color = "#C000FF";
            } else if (mails.Contains("black")) {
                color = "#000000";
            } else if (mails.Contains("white2") || mails.Contains("niconicowhite")) {
                color = "#CCCC99";
            } else if (mails.Contains("pink2")) {
                color = "#FF33CC";
            } else if (mails.Contains("red2") || mails.Contains("truered")) {
                color = "#CC0033";
            } else if (mails.Contains("orange2") || mails.Contains("passionorange")) {
                color = "#FF6600";
            } else if (mails.Contains("yellow2") || mails.Contains("madyellow")) {
                color = "#999900";
            } else if (mails.Contains("green2") || mails.Contains("elementalgreen")) {
                color = "#00CC66";
            } else if (mails.Contains("cyan2")) {
                color = "#00CCCC";
            } else if (mails.Contains("blue2") || mails.Contains("marineblue")) {
                color = "#3399FF";
            } else if (mails.Contains("purple2") || mails.Contains("nobleviolet")) {
                color = "#6633CC";
            } else if (mails.Contains("black2")) {
                color = "#666666";
            }

            var live = false;
            if (mails.Contains("_live")) {
                live = true;
                var translated = ColorTranslator.FromHtml(color);
                color = $"rgba({translated.R},{translated.G},{translated.B},0.5)";
            }

            var full = false;
            if (mails.Contains("full")) {
                full = true;
            }

            var ender = false;
            if (mails.Contains("ender")) {
                ender = true;
            }

            return new {
                threadId = entry.ThreadId,
                fork = entry.Fork,
                no = entry.Number,
                vpos = entry.Vpos,
                mail = entry.Mail!,
                content = entry.Content!,
                fontSize,
                fontFamily,
                position,
                full,
                ender,
                color,
                duration,
                live,
                lineCount = entry.Content!.Split('\n').Length
            };
        }
    }
}
