using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoViewer {

    //ハイパーリンクを解析してハイパーリンク化する
    public class HyperLinkReplacer {

        //URLをハイパーリンク化する
        public static string Replace(string desc) {

            if(Settings.Instance.EnableUrlLink) {

                //普通のURLだったら
                var url = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?");
                
                desc = url.Replace(desc, new MatchEvaluator((match) => {

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
                var twitter = new Regex(@"[@|＠][A-z|0-9|_]{1,15}");
                desc = twitter.Replace(desc, new MatchEvaluator((match) => {

                    return "<a href=\"https://twitter.com/" + match.Value.Substring(1) + "\">" + match.Value + "</a>";
                }));
                var twitter2 = new Regex(@"TwitterID:([A-z|0-9|_]{1,15})");
                desc = twitter2.Replace(desc, new MatchEvaluator((match) => {

                    return "TwitterID:<a href=\"https://twitter.com/" + match.Groups[1].Value + "\">" + match.Groups[1].Value + "</a>";
                }));

            }

            return desc;
        }
    }
}
