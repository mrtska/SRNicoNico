using Livet;

namespace SRNicoNico.ViewModels {
    public class OtherViewModel : TabItemViewModel {

        #region OtherList変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _OtherList = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);
        
        public DispatcherCollection<TabItemViewModel> OtherList {
            get { return _OtherList; }
            set { 
                if(_OtherList == value)
                    return;
                _OtherList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public OtherViewModel() : base("その他") {

            OtherList.Add(new OverViewViewModel(this));
            OtherList.Add(new OSSViewModel());
            OtherList.Add(new PrivacyPolicyViewModel());
        }
    }
}
