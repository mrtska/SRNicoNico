using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoWrapper;
using Codeplex.Data;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class VIdeoCommentConfig {

        public bool Hide3DSComment { get; set; }

        public bool HideWiiUComment { get; set; }


        public string ToJson() {

            return DynamicJson.Serialize(this);
        }



    }
}
