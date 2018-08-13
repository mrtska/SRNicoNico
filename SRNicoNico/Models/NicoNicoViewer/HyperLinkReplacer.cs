using System.Text.RegularExpressions;

namespace SRNicoNico.Models.NicoNicoViewer {

    //ハイパーリンクを解析してハイパーリンク化する
    public class HyperLinkReplacer {

        private static readonly Regex UrlPattern = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?");

        private static readonly Regex TwitterPattern = new Regex(@"[@|＠][A-z|0-9|_]{1,15}");
        private static readonly Regex Twitter2Pattern = new Regex(@"TwitterID:([A-z|0-9|_]{1,15})");

        private static readonly Regex MailAddressPattern = new Regex(@"[\x00-\x7f]+@[\x00-\x7f]+");

            //URLをハイパーリンク化する
        public static string Replace(string desc) {

            if(Settings.Instance.EnableUrlLink) {

                //普通のURLだったら
                desc = UrlPattern.Replace(desc, new MatchEvaluator((match) => {

                    var matchurl = match.Value;
                    if(Regex.IsMatch(desc, @"<a\s?href=""" + Regex.Escape(matchurl) + @".+?>.+?</a>")) {
                        //aタグが書いてあったらそのまま返す
                        return matchurl;
                    }

                    return "<a href=\"" + matchurl + "\">" + matchurl + "</a>";
                }));
            }

            if(Settings.Instance.EnableTwitterLink) {

                //TwitterのIDだった場合
                desc = TwitterPattern.Replace(desc, new MatchEvaluator((match) => {

                    return "<a href=\"https://twitter.com/" + match.Value.Substring(1) + "\">" + match.Value + "</a>";
                }));
                desc = Twitter2Pattern.Replace(desc, new MatchEvaluator((match) => {

                    return "TwitterID:<a href=\"https://twitter.com/" + match.Groups[1].Value + "\">" + match.Groups[1].Value + "</a>";
                }));
            }
            return desc;
        }
    }
}
