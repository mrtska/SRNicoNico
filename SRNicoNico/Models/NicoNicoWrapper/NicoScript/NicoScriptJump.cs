using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoScriptJump : NicoScriptBase {

        private static readonly Regex SeekPattern = new Regex(@"#(\d+:\d+)");

        public NicoScriptJump(VideoCommentViewModel vm, NicoNicoCommentEntry entry) : base(vm, entry, false) {
        }

        public override void Execute(NicoNicoCommentEntry target) {

            if(!Settings.Instance.DisableJumpCommand) {

                var seek = SeekPattern.Match(Entry.Content);

                if(seek.Success) {

                    Owner.Owner.Handler.Seek(NicoNicoUtil.ConvertTime(seek.Groups[1].Value));
                    return;
                }
                var split = Entry.Content.Split(' ');

                Owner.Owner.VideoUrl = "http://www.nicovideo.jp/watch/" + split[1];
                Owner.Owner.Initialize();
            }
        }
    }
}
