﻿using System;
using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// マイリストの情報
    /// </summary>
    public class MylistItem {
        /// <summary>
        /// マイリストを作成した日付
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }
        /// <summary>
        /// デフォルトの並び順のキー
        /// </summary>
        public string DefaultSortKey { get; set; } = default!;
        /// <summary>
        /// デフォルトの並び順
        /// ascかdesc
        /// </summary>
        public string DefaultSortOrder { get; set; } = default!;
        /// <summary>
        /// マイリストの説明文
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// マイリストをフォローしている人の数
        /// </summary>
        public int FollowerCount { get; set; }
        /// <summary>
        /// マイリストのID
        /// </summary>
        public string Id { get; set; } = default!;
        /// <summary>
        /// マイリストをフォローしているかどうか
        /// </summary>
        public bool IsFollowing { get; set; }
        /// <summary>
        /// マイリストが公開されているかどうか
        /// </summary>
        public bool IsPublic { get; set; }
        /// <summary>
        /// マイリストに含まれている動画の数
        /// </summary>
        public int ItemsCount { get; set; }
        /// <summary>
        /// マイリスト名
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// 動画投稿者のサムネイルURL
        /// </summary>
        public string? OwnerIconUrl { get; set; }
        /// <summary>
        /// 動画投稿者のID
        /// </summary>
        public string? OwnerId { get; set; }
        /// <summary>
        /// 動画投稿者の名前
        /// </summary>
        public string? OwnerName { get; set; }
        /// <summary>
        /// 動画投稿者の種類
        /// userかchannelかhidden
        /// </summary>
        public string OwnerType { get; set; } = default!;

        /// <summary>
        /// マイリストに含まれている動画のサンプル
        /// </summary>
        public IEnumerable<MylistVideoItem> SampleItems { get; set; } = default!;
    }
}
