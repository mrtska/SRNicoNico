using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SRNicoNico.Views.Service.Theme {
    public class Blue : IThemeColor {

        private static readonly SolidColorBrush Accent = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC"));
        private static readonly SolidColorBrush AccentActive = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF005AAA"));
        private static readonly SolidColorBrush AccentAlphaHighlight = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A0007ACC"));
        private static readonly SolidColorBrush AccentForeground = new SolidColorBrush(Colors.White);
        private static readonly SolidColorBrush AccentHighlight = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF28A0F0"));

        public Brush AccentActiveBrush => AccentActive;

        public Brush AccentAlphaHighlightBrush => AccentAlphaHighlight;

        public Brush AccentBrush => Accent;

        public Brush AccentForegroundBrush => AccentForeground;

        public Brush AccentHighlightBrush => AccentHighlight;

    }
}
