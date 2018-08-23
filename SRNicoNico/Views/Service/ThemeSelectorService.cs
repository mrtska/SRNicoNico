using SRNicoNico.Views.Service.Theme;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SRNicoNico.Views.Service {

    public enum EnumAccents {

        Blue,
        Orange,
        Purple
    }
    public enum EnumThemes {

        Dark,
        Light
    }

    public class ThemeSelectorService {

        //テーマ本体とenumのマップ
        private static readonly Dictionary<EnumAccents, IAccent> AccentMap = new Dictionary<EnumAccents, IAccent>();
        private static readonly Dictionary<EnumThemes, ITheme> ThemeMap = new Dictionary<EnumThemes, ITheme>();

        static ThemeSelectorService() {

            AccentMap[EnumAccents.Orange] = new Orange();
            AccentMap[EnumAccents.Blue] = new Blue();
            AccentMap[EnumAccents.Purple] = new Purple();

            ThemeMap[EnumThemes.Dark] = new Dark();
            ThemeMap[EnumThemes.Light] = new Light();
        }

        public void ChangeAccent(EnumAccents accent) {

            var theme = AccentMap[accent];

            //アクセントを反映
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source =  new Uri(theme.ResourcePath, UriKind.RelativeOrAbsolute)});
        }

        public void ChangeTheme(EnumThemes theme) {

            var uri = ThemeMap[theme];

            //テーマを反映
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(uri.ResourcePath, UriKind.RelativeOrAbsolute) });
        }
    }
}
