using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoMylist {

        private const string MylistTop = "http://www.nicovideo.jp/my/mylist";

        //マイリストグループ関連の処理
        public NicoNicoMylistGroup Group { get; private set; }

        //マイリスト自体への処理
        public NicoNicoMylistItem Item { get; private set; }

        private readonly TabItemViewModel Owner;

        public NicoNicoMylist(TabItemViewModel vm) {

            Group = new NicoNicoMylistGroup(vm);
            Item = new NicoNicoMylistItem(vm);
            Owner = vm;
        }

        //マイリストトークンを取得 書き込み系の処理につかう CSRF防止だろうね
        public async Task<string> GetMylistTokenAsync() {

            try {

                Owner.Status = "マイリストトークン取得中";

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(MylistTop);
                return Regex.Match(a, @"NicoAPI.token.+?""(.+?)""").Groups[1].Value;
            } catch(RequestFailed) {

                Owner.Status = "マイリストトークンの取得に失敗しました";
                return null;
            }
        }
    }
}
