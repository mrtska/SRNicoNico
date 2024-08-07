using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Livet;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// クラッシュレポート画面のViewModel
    /// </summary>
    public class CrashReportViewModel : ViewModel {

        /// <summary>
        /// 吐いた例外
        /// </summary>
        public Exception Exception { get; }

        private string _WillSendText = string.Empty;
        /// <summary>
        /// サーバに送られるテキスト
        /// </summary>
        public string WillSendText {
            get { return _WillSendText; }
            set { 
                if (_WillSendText == value)
                    return;
                _WillSendText = value;
                RaisePropertyChanged();
            }
        }

        private string _AdditionalMessage = string.Empty;
        /// <summary>
        /// 特筆事項
        /// </summary>
        public string AdditionalMessage {
            get { return _AdditionalMessage; }
            set { 
                if (_AdditionalMessage == value)
                    return;
                _AdditionalMessage = value;
                RaisePropertyChanged();
            }
        }


        private string _Status = "レポートを送信";
        /// <summary>
        /// 送信ステータス
        /// </summary>
        public string Status {
            get { return _Status; }
            set { 
                if (_Status == value)
                    return;
                _Status = value;
                RaisePropertyChanged();
            }
        }


        private bool _IsSending;
        /// <summary>
        /// 送信中
        /// </summary>
        public bool IsSending {
            get { return _IsSending; }
            set { 
                if (_IsSending == value)
                    return;
                _IsSending = value;
                RaisePropertyChanged();
            }
        }


        public CrashReportViewModel(Exception e) {

            Exception = e;

            WillSendText = e.ToString();
        }

        public async void SendReport() {

            var client = new HttpClient();

            var formData = new Dictionary<string, string> {
                ["stackTrace"] = WillSendText,
                ["additionalMessage"] = AdditionalMessage
            };

            IsSending = true;

            Status = "送信中";

            try {
                // mrtska.netにスタックトレースを送信
                using var result = await client.PostAsync("https://mrtska.net/niconicoviewer/report", new FormUrlEncodedContent(formData));

                if (result.IsSuccessStatusCode) {

                    Status = "ご協力ありがとうございました。";
                } else {

                    IsSending = false;
                    Status = "送信出来ませんでした。";
                }
            } catch {

                IsSending = false;
                Status = "送信出来ませんでした。";
            }
        }
    }
}
