using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public static class NicoNicoUtil {


        //設定ファイルがあるディレクトリ
        public static string OptionDirectory {
            get {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SRNicoNico\";
            }
        }

        //カレントディレクトリ取得
        public static string CurrentDirectory {
            get {

                return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }
        }

        //クリップボードにコピー 例外吐いたら握り潰す
        public static void CopyToClipboard(string str) {

            try {

                Clipboard.SetText(str);
            }catch(Exception) {

                //握りつぶす
            }
        }

        //ローカル視聴履歴に指定したIWatchableが存在したらフラグを立てる
        public static void ApplyLocalHistory(IWatchable target) {

            foreach(var entry in App.ViewModelRoot.History.Model.LocalHistries) {

                if(target.ContentUrl.Contains(entry.VideoId)) {

                    target.IsWatched = true;
                    return;
                }
            }

        }

        //m:ssな形式をintに変換する
        public static int ConvertTime(string s) {

            string[] strings = s.Split(':');
            string minutes = strings[0];
            string seconds = strings[1];

            return (int.Parse(minutes) * 60) + int.Parse(seconds);
        }

        //sをm:sに変換
        public static string ConvertTime(int time) {

            int munites = time / 60;
            int seconds = time % 60;

            if(seconds < 10) {

                return munites + ":0" + seconds;
            }

            return munites + ":" + seconds;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        public static void PreventLockWorkstation() {

            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
        }

        public static void ReleasePreventLockworkstation() {

            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }
        public static bool IsValidJson(string strInput) {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || (strInput.StartsWith("[") && strInput.EndsWith("]"))) {
                try {
                    var obj = JToken.Parse(strInput);
                    return true;
                } catch (JsonReaderException) {
                    return false;
                } catch (Exception) {
                    return false;
                }
            } else {
                return false;
            }
        }
    }

    [Flags]
    public enum EXECUTION_STATE : uint {

        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002,
        ES_SYSTEM_REQUIRED = 0x00000001
    }
}
