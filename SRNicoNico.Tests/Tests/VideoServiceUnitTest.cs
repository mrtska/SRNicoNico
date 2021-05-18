﻿using System;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;
using Xunit;

namespace SRNicoNico.Tests {
    /// <summary>
    /// IVideoServiceのユニットテスト
    /// </summary>
    public class VideoServiceUnitTest {

        private readonly ISessionService SessionService;
        private readonly IVideoService VideoService;

        public VideoServiceUnitTest() {

            SessionService = TestingNicoNicoViewer.Instance.TestSessionService;
            VideoService = new NicoNicoVideoService(SessionService);
        }


        /// <summary>
        /// 動画情報を正しく取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetWatchApiDataUnitTest() {

            var result = await VideoService.WatchAsync("sm9", true);

            Assert.NotNull(result.Comment);
            Assert.NotEmpty(result.EasyCommentPhrases);
            Assert.NotNull(result.Genre);
            Assert.NotNull(result.Media);
            Assert.NotNull(result.OkReason);
            Assert.NotNull(result.Owner);
            Assert.NotNull(result.Tag);
            Assert.NotNull(result.Video);

            Assert.NotNull(result.Comment.ServerUrl);
            Assert.NotNull(result.Comment.UserKey);

            Assert.NotEmpty(result.Comment.Layers);
            Assert.NotEmpty(result.Comment.Threads);

            Assert.NotNull(result.Genre.Key);
            Assert.NotNull(result.Genre.Label);

            Assert.NotNull(result.Media.RecipeId);
            Assert.NotNull(result.Media.Movie);
            Assert.NotNull(result.Media.StoryBoard);
            Assert.NotNull(result.Media.TrackingId);

            Assert.NotEmpty(result.Media.Movie.Audios);
            Assert.NotEmpty(result.Media.Movie.Videos);
            Assert.NotNull(result.Media.Movie.Session);

            Assert.NotNull(result.Owner.Id);
            Assert.NotNull(result.Owner.Nickname);
            Assert.NotNull(result.Owner.IconUrl);

            Assert.NotNull(result.Video.Id);
            Assert.NotNull(result.Video.Title);
            Assert.NotEqual(default, result.Video.RegisteredAt);
            Assert.NotNull(result.Video.Description);
            Assert.NotNull(result.Video.ThumbnailUrl);
            Assert.NotNull(result.Video.ThumbnailPlayer);
            Assert.NotNull(result.Video.ThumbnailOgp);
            Assert.NotNull(result.Video.WatchableUserTypeForPayment);
            Assert.NotNull(result.Video.CommentableUserTypeForPayment);
        }
    }
}
