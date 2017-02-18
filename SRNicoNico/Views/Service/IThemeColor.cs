using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SRNicoNico.Views.Service {
    public interface IThemeColor {

        Brush AccentBrush { get; }

        Brush AccentHighlightBrush { get; }

        Brush AccentAlphaHighlightBrush { get; }

        Brush AccentActiveBrush { get; }

        Brush AccentForegroundBrush { get; }

    }
}
