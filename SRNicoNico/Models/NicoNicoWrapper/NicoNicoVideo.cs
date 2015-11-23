using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {

    public class NicoNicoVideoInfoEntry {

        //ID sm9みたいな
        public string Cmsid { get; set; }

        //タイトル
        public string Title { get; set; }

        //再生回数
        public int ViewCounter { get; set; }

        //コメント数
        public int CommentCounter { get; set; }

        //マイリスト数
        public int MylistCounter { get; set; }

        //サムネイルURL
        public string ThumbnailUrl { get; set; }

        //再生時間
        public string Length { get; set; }

        //動画投稿時日時
        public string FirstRetrieve { get; set; }


        //コンストラクタ
        public NicoNicoVideoInfoEntry() {

        }
    }

}
