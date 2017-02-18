using SRNicoNico.Views.Service.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace SRNicoNico.Views.Service {

    public enum ThemeColors {

        Blue,
        Orange,
        Purple

    }




    public class ThemeSelectorService {

        //テーマ本体とenumのマップ
        private static readonly Dictionary<ThemeColors, IThemeColor> ColorMap = new Dictionary<ThemeColors, IThemeColor>();

        private static readonly string[] AccentTarget = { "AccentBrushKey", "WindowBorderActive", "Accent", "SliderThumbBorderDragging" };
        private static readonly string[] AccentActiveTarget = { "AccentActiveBrushKey" };
        private static readonly string[] AccentAlphaHighlightTarget = { "AccentAlphaHighlightBrushKey" };
        private static readonly string[] AccentForegroundTarget = { "AccentForegroundBrushKey" };
        private static readonly string[] AccentHighlightTarget = { "AccentHighlightBrushKey", "DataGridCellBackgroundHover", "DataGridCellBackgroundSelected", "ModernButtonTextHover", "ModernButtonIconBackgroundPressed", "SliderThumbBackgroundDragging" };

        static ThemeSelectorService() {

            ColorMap[ThemeColors.Orange] = new Orange();
            ColorMap[ThemeColors.Blue] = new Blue();
            ColorMap[ThemeColors.Purple] = new Purple();
        }

        public void ChangeTheme(ThemeColors color) {

            var theme = ColorMap[color];

            //テーマを反映
            foreach(var name in AccentTarget) {

                App.Current.Resources[name] = theme.AccentBrush;
            }

            foreach (var name in AccentActiveTarget) {

                App.Current.Resources[name] = theme.AccentActiveBrush;
            }
            foreach (var name in AccentAlphaHighlightTarget) {

                App.Current.Resources[name] = theme.AccentAlphaHighlightBrush;
            }

            foreach(var name in AccentForegroundTarget) {

                App.Current.Resources[name] = theme.AccentForegroundBrush;
            }
            foreach (var name in AccentHighlightTarget) {

                App.Current.Resources[name] = theme.AccentHighlightBrush;
            }
        }



    }
}
