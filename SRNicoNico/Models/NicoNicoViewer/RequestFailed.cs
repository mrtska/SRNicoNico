using System;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class RequestFailed : Exception {

        public Exception ExceptionObject { get; private set; }
        public FailedType FailedType { get; private set; }

        public RequestFailed(Exception e, FailedType type) {

            ExceptionObject = e;
            FailedType = type;
        }

    }

    public enum FailedType {

        TimeOut,
        Failed
    }

}
