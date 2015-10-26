using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class RequestTimeout : Exception {

        public Exception OwnerException;

        public RequestTimeout(Exception e) {

            OwnerException = e;
        }

    }
}
