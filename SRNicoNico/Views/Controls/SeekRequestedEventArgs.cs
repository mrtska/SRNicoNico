using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;

namespace SRNicoNico.Views.Controls {
    public class SeekRequestedEventArgs : EventArgs {


        public double Position;

        public SeekRequestedEventArgs(double pos) {

            Position = pos;
        }
    }

    public delegate void SeekRequestedHandler(object sender, SeekRequestedEventArgs e);
}
