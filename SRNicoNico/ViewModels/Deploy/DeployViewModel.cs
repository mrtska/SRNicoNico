using Microsoft.Win32;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {

    public class DeployViewModel : TabItemViewModel {

#if DEBUG
        public DeployModel Model { get; set; }
#endif
        public DeployViewModel() : base("デプロイ") {
#if DEBUG
            Model = new DeployModel();
#endif
        }

        public void Initialize() {


        }
#if DEBUG
        public void FileSelect() {

            var dialog = new OpenFileDialog();
            dialog.ShowDialog();

            Model.FilePath = dialog.FileName;
        }

    public async void Upload() {
            Status = "アップロード中";
            Status = await Model.UploadAsync();
        }
#endif
    }
}
