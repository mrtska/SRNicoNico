using System;
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

            Assert.NotNull(result.Viewer.Id);
            Assert.NotNull(result.Viewer.Nickname);
        }

        /// <summary>
        /// DMCセッションを作成してハートビートを送る事が出来ることのテスト
        /// </summary>
        [Fact]
        public async Task HeartbeatUnitTest() {

            var result = await VideoService.WatchAsync("sm9", true);

            // セッションを作成
            var session = await VideoService.CreateSessionAsync(result.Media.Movie.Session);
            Assert.NotNull(session);

            // 1秒待つ
            await Task.Delay(1000);

            // ハートビート
            session = await VideoService.HeartbeatAsync(session);
            Assert.NotNull(session);

            // 1秒待つ
            await Task.Delay(1000);

            // セッション削除
            await VideoService.DeleteSessionAsync(session);
        }

        /// <summary>
        /// コメントを取得することが出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetCommentUnitTest() {

            var result = await VideoService.WatchAsync("sm9", true);

            var comments = await VideoService.GetCommentAsync(result.Comment);

            Assert.NotEmpty(comments);
        }

        /// <summary>
        /// ストーリーボードを取得することが出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetStoryBoardUnitTest() {

            var result = await VideoService.WatchAsync("sm9", true);

            var sb = await VideoService.GetStoryBoardAsync(result.Media.StoryBoard.Session);

            Assert.NotEqual(0, sb.Columns);
            Assert.NotEqual(0, sb.Interval);
            Assert.NotEqual(0, sb.Quality);
            Assert.NotEqual(0, sb.Rows);
            Assert.NotEqual(0, sb.ThumbnailHeight);
            Assert.NotEqual(0, sb.ThumbnailWidth);

            Assert.NotEmpty(sb.BitmapMap);
        }

        /// <summary>
        /// 動画再生位置を保存することが出来ることのテスト
        /// </summary>
        [Fact]
        public async Task SavePlaybackPositionUnitTest() {

            var result = await VideoService.SavePlaybackPositionAsync("sm8628149", 5);
            Assert.True(result);
        }



    }
}
