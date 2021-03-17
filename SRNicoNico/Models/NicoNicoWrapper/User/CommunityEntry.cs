using System;
using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// コミュニティ情報
    /// </summary>
    public class CommunityEntry {

        /// <summary>
        /// コミュニティ作成日
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// コミュニティの説明文
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// コミュニティのID coが付く
        /// </summary>
        public string? GlobalId { get; set; }

        /// <summary>
        /// コミュニティのID
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// コミュニティレベル
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// コミュニティ名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// フォローリクエストの自動承認
        /// </summary>
        public bool CommunityAutoAcceptEntry { get; set; }

        /// <summary>
        /// コミュニティのブロマガが有効かどうか
        /// </summary>
        public bool CommunityBlomaga { get; set; }

        /// <summary>
        /// 生放送履歴を表示しないかどうか
        /// </summary>
        public bool CommunityHideLiveArchives { get; set; }

        /// <summary>
        /// コミュニティアイコンなんとか 良く分からん
        /// </summary>
        public string? CommunityIconInspectionMobile { get; set; }

        /// <summary>
        /// 掲示板関係 良く分からん
        /// </summary>
        public bool CommunityInvalidBbs { get; set; }

        /// <summary>
        /// なんだろう 分からん
        /// </summary>
        public bool CommunityPrivLiveBroadcastNew { get; set; }

        /// <summary>
        /// 分からん
        /// </summary>
        public bool CommunityPrivUserAuth { get; set; }

        /// <summary>
        /// 分からん
        /// </summary>
        public bool CommunityPrivVideoPost { get; set; }

        /// <summary>
        /// 全然分からん
        /// </summary>
        public int CommunityShownNewsNum { get; set; }

        /// <summary>
        /// 本当に分からん
        /// </summary>
        public bool CommunityUserInfoRequired { get; set; }

        /// <summary>
        /// 所有者のユーザーID
        /// </summary>
        public string? OwnerId { get; set; }

        /// <summary>
        /// コミュニティのステータス
        /// </summary>
        public CommunityStatus Status { get; set; }

        /// <summary>
        /// コミュニティに登録されているタグ
        /// </summary>
        public IEnumerable<string>? Tags { get; set; }

        /// <summary>
        /// コミュニティに登録されている動画の数
        /// </summary>
        public int ThreadCount { get; set; }

        /// <summary>
        /// コミュニティに登録出来る動画の最大数
        /// </summary>
        public int ThreadMax { get; set; }

        /// <summary>
        /// コミュニティのサムネイルURL
        /// </summary>
        public string? ThumbnailUrl { get; set; }

        /// <summary>
        /// コミュニティに登録しているユーザー数
        /// </summary>
        public int UserCount { get; set; }
    }

    public enum CommunityStatus {
        /// <summary>
        /// オープンコミュニティ
        /// </summary>
        Open,
        /// <summary>
        /// クローズコミュニティ
        /// </summary>
        Closed
    }
}
