namespace SRNicoNico.ViewModels {
    /// <summary>
    /// スタートページクラスのDataContext
    /// </summary>
    public class StartViewModel : TabItemViewModel {

        /// <summary>
        /// 現在のNicoNicoViewerのバージョン
        /// </summary>
        public double CurrentVersion { get; private set; }

        public StartViewModel(MainWindowViewModel vm) : base("スタート") {

            CurrentVersion = vm.CurrentVersion;
        }
    }
}
