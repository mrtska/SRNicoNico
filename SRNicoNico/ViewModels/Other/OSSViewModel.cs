using Livet;

namespace SRNicoNico.ViewModels {
    public class OSSViewModel : TabItemViewModel {

        #region OSSList変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _OSSList = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<TabItemViewModel> OSSList {
            get { return _OSSList; }
            set {
                if(_OSSList == value)
                    return;
                _OSSList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public OSSViewModel() : base("オープンソースソフトウェア") {

            OSSList.Add(new OSSEntryViewModel("MetroRadiance"));
            OSSList.Add(new OSSEntryViewModel("MetroTrilithon"));
            OSSList.Add(new OSSEntryViewModel("StatefulModel"));
            OSSList.Add(new OSSEntryViewModel("Livet"));
            OSSList.Add(new OSSEntryViewModel("Newtonsoft.Json"));
            OSSList.Add(new OSSEntryViewModel("DynamicJson"));
            OSSList.Add(new OSSEntryViewModel("Material Design Icons"));
            OSSList.Add(new OSSEntryViewModel("HtmlAgilityPack"));
            OSSList.Add(new OSSEntryViewModel("gong-wpf-dragdrop"));
            OSSList.Add(new OSSEntryViewModel("ForcibleLoader"));
        }
    }
}
