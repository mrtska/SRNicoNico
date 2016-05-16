using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class RequestFailed : Exception {


        public FailedType FailedType { get; private set; }

        public RequestFailed(FailedType type) {

            FailedType = type;
        }

    }

    public enum FailedType {

        TimeOut,
        Failed
    }

}
