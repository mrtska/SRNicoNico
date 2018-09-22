using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoCommentList {

        private static readonly Dictionary<CommentType, string> TypeString = new Dictionary<CommentType, string>() {

            { CommentType.Normal, "通常コメント" },
            { CommentType.Uploader, "投稿者コメント" },
            { CommentType.Community, "コミュニティコメント" },
            { CommentType.Channel, "チャンネルコメント" },
            { CommentType.ExtCommunity, "引用コメント" },
            { CommentType.NicoScript, "ニコスクリプト" }
        };

        
        public string ListName { get; private set; }

        public List<NicoNicoCommentEntry> CommentList { get; private set; }

        public CommentComposite Composite { get; private set; }

        public CommentType CommentType { get; private set; }

        //コメントチケット？
        public string Ticket { get; set; }

        //最後のコメント
        public int LastRes { get; set; }

        //ThreadKey ある場合
        public string ThreadKey { get; set; }

        //ある場合 0か1
        public string Force184 { get; set; }


        public NicoNicoCommentList(CommentType type, CommentComposite composite) {

            CommentType = type;
            ListName = TypeString[type];
            Composite = composite;
            CommentList = new List<NicoNicoCommentEntry>();
        }

        public void Add(NicoNicoCommentEntry entry) {

            CommentList.Add(entry);
        }

        public void Sort() {

            CommentList.Sort();
        }
    }
}
