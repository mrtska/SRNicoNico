using Codeplex.Data;
using Livet;
using Livet.Messaging;
using Livet.Messaging.Windows;
using SRNicoNico.Models.NicoNicoWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SRNicoNico.ViewModels {


    public class SignInViewModel : ViewModel {

        private const string UserSessionName = "user.session";


        #region SignInViewTitle変更通知プロパティ
        private string _SignInViewTitle = "サインイン";

        public string SignInViewTitle {
            get { return _SignInViewTitle; }
            set { 
                if(_SignInViewTitle == value)
                    return;
                _SignInViewTitle = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        public SignInViewModel() {

        }


        public async Task<List<NicoNicoSessionUser>> AutoSignIn() {

            //ユーザーのセッションファイルがあったらオートログインを試みる
            if(File.Exists(NicoNicoUtil.OptionDirectory + UserSessionName)) {

                try {

                    //ファイルからセッションデータをロードする
                    using(var file = File.OpenRead(NicoNicoUtil.OptionDirectory + UserSessionName)) {
                        using(var reader = new StreamReader(file)) {


                            dynamic json = DynamicJson.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd())));

                            var list = new List<NicoNicoSessionUser>();

                            foreach(var user in json.list) {

                                var session = new NicoNicoSession(user.session.Key);

                                //カレントユーザーだけサインインする
                                if(user.session.UserId != json.CurrentUser) {

                                    list.Add(new NicoNicoSessionUser(session));
                                    continue;
                                }

                                App.SetCookie(new Uri("https://nicovideo.jp/"), "user_session=" + session.Key);
                                
                                var status = await session.VerifySignInAsync();
                                if(status == SigninStatus.Success) {

                                    list.Add(new NicoNicoSessionUser(session));
                                } else if(status == SigninStatus.Failed) {

                                    SignInViewTitle = "自動サインイン失敗";
                                    list.Add(await SignInAsync());
                                } else {

                                    list.Add(await SignInAsync());
                                }

                            }

                            return list;//  new NicoNicoUser(new NicoNicoSession());
                        }
                    }

                    //例外吐いて読み込めなかったらファイルを消して作り直す
                } catch(Exception) {

                    if(Directory.Exists(NicoNicoUtil.OptionDirectory + UserSessionName)) {

                        Directory.Delete(NicoNicoUtil.OptionDirectory + UserSessionName, true);
                    }

                    if(File.Exists(NicoNicoUtil.OptionDirectory + UserSessionName)) {

                        File.Delete(NicoNicoUtil.OptionDirectory + UserSessionName);
                    }

                    //ダイアログを使ってサインイン
                    return new List<NicoNicoSessionUser>() { await SignInAsync() };
                }
            } else {

                return new List<NicoNicoSessionUser>() { await SignInAsync() };
            }
        }

        public async Task<NicoNicoSessionUser> SignInAsync() {

            {
                //前のCookieを削除
                var expiration = DateTime.UtcNow - TimeSpan.FromDays(1);
                string str = string.Format("{0}=; expires={1}; path=/; domain=.nicovideo.jp", "user_session", expiration.ToString("R"));
                App.SetCookie(new Uri("https://nicovideo.jp/"), str);
            }
            
            var userSession = "";

            //ブラウザのCookieをポーリングしてログインしてるか確認
            var pollingTimer = new System.Timers.Timer(1000);

            pollingTimer.Elapsed += async (o, e) => {

                //Cookieをブラウザから取得
                var cookie = App.GetCookie(new Uri("https://nicovideo.jp/"));

                foreach(var entry in cookie.Split(';')) {

                    var keyvalue = entry.Split('=');
                    var key = keyvalue[0];

                    //セッションが見つかったらポップアップを閉じて終了
                    if(key.Contains("user_session")) {

                        userSession = keyvalue[1];


                        var session = new NicoNicoSession(userSession);

                        var status = await session.VerifySignInAsync();

                        if(status == SigninStatus.ServiceUnavailable) {

                            MessageBox.Show("メンテナンス中か、サーバーが落ちています。");
                            Environment.Exit(0);
                        }

                        //失敗した時に前のセッションを消す
                        if(status == SigninStatus.Failed) {

                            //前のCookieを削除
                            var expiration = DateTime.UtcNow - TimeSpan.FromDays(1);
                            string str = string.Format("{0}=; expires={1}; path=/; domain=.nicovideo.jp", "user_session", expiration.ToString("R"));
                            App.SetCookie(new Uri("https://nicovideo.jp/"), str);
                        }

                        if(status == SigninStatus.Success) {

                            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "SignIn"));
                        }
                        break;
                    }
                }
            };

            pollingTimer.Enabled = true;

            while(true) {

                //Modalで開くと下のDisposeはウィンドウを閉じるまで呼ばれない
                App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.SignInView), this, TransitionMode.Modal));

                var session = new NicoNicoSession(userSession);

                var status = await session.VerifySignInAsync();

                //メンテナンス中なら落とす
                if(status == SigninStatus.ServiceUnavailable) {

#pragma warning disable CS4014 // この呼び出しを待たないため、現在のメソッドの実行は、呼び出しが完了する前に続行します
                    Task.Run(() => {

                        Thread.Sleep(8000);
                        Environment.Exit(0);
                    });
#pragma warning restore CS4014 // この呼び出しを待たないため、現在のメソッドの実行は、呼び出しが完了する前に続行します
                    MessageBox.Show("メンテナンス中か、サーバーが落ちています。");
                }

                //失敗した時に前のセッションを消す
                if(status == SigninStatus.Failed) {

                    //前のCookieを削除
                    var expiration = DateTime.UtcNow - TimeSpan.FromDays(1);
                    string str = string.Format("{0}=; expires={1}; path=/; domain=.nicovideo.jp", "user_session", expiration.ToString("R"));
                    App.SetCookie(new Uri("http://nicovideo.jp/"), str);
                }

                if(status == SigninStatus.Success) {

                    //タイマーを終了
                    pollingTimer.Dispose();
                    return new NicoNicoSessionUser(session);
                }
            }
        }
        
        //Jsonにしてセッションを保存する
        public void SaveSession(List<NicoNicoSessionUser> users, NicoNicoSessionUser current) {

            dynamic json = new DynamicJson();

            var list = new List<dynamic>();
            foreach(var user in users) {

                list.Add(new { session = user.Session });
            }

            json.list = (object[]) list.ToArray();

            json.CurrentUser = current.Session.UserId;

            var str = json.ToString();

            using (var writer = new StreamWriter(NicoNicoUtil.OptionDirectory + UserSessionName)) {

                writer.WriteLine(Convert.ToBase64String(Encoding.UTF8.GetBytes(str)));
            }
        }
        public void ExitButtonDown() {

            Application.Current.Shutdown();
        }
    }
}
