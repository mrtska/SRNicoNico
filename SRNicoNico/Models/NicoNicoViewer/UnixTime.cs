using System;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class UnixTime {

        //グリニッジ標準時
        public readonly static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        //DateTimeをUNIX時間に変換する
        public static long ToUnixTime(DateTime dateTime) {
            // 時刻をUTCに変換
            dateTime = dateTime.ToUniversalTime();

            // unix epochからの経過秒数を求める
            return (long)dateTime.Subtract(UnixEpoch).TotalSeconds;
        }

        //UNIX時間からDateTimeに変換するメソッド
        public static DateTime FromUnixTime(long unixTime) {
            // unix epochからunixTime秒だけ経過した時刻を求める
            return UnixEpoch.AddSeconds(unixTime).AddHours(9);
        }
    }
}
