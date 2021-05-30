using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// ストーリーボード情報
    /// </summary>
    public class VideoStoryBoard : IDisposable {

        /// <summary>
        /// ストーリーボードの画像1枚分の高さ
        /// </summary>
        public int ThumbnailHeight { get; set; }

        /// <summary>
        /// ストーリーボードの画像1枚分の横幅
        /// </summary>
        public int ThumbnailWidth { get; set; }

        /// <summary>
        /// 画像の行数
        /// </summary>
        public int Rows { get; set; }
        /// <summary>
        /// 画像の列数
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// 画像間の秒数
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// JPEG画像のクオリティ
        /// </summary>
        public int Quality { get; set; }

        /// <summary>
        /// ストーリーボードの画像のマップ
        /// </summary>
        public IDictionary<int, Bitmap>? BitmapMap { get; set; }

        public void Dispose() {
            BitmapMap?.Values.ToList().ForEach(f => f.Dispose());
        }
    }
}
