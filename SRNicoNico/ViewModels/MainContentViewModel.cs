using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;

namespace SRNicoNico.ViewModels;

/// <summary>
/// MainWindowのViewModel
/// すべてを司る
/// </summary>
public class MainContentViewModel : ViewModel {

#if DEBUG
    private string _Title = "NicoNicoViewer Debug Build ";
#else
        private string _Title = "NicoNicoViewer Beta";
#endif
    public string Title {
        get { return _Title; }
        set {
            if (_Title == value)
                return;
            _Title = value;
            RaisePropertyChanged();
        }
    }

    private string _Status = string.Empty;
    /// <summary>
    /// ステータスバーに表示する文字列
    /// </summary>
    public string Status {
        get { return _Status; }
        set {
            if (_Status == value)
                return;
            _Status = value;
            RaisePropertyChanged();
        }
    }


}
