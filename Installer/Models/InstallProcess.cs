using Installer.ViewModels;
using Microsoft.Win32;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace Installer.Models {
    public class InstallProcess {

        private readonly MainWindowViewModel Owner;

        public InstallProcess(MainWindowViewModel vm) {

            Owner = vm;
        }

        public async Task InstallAsync() {

            Owner.Status = "レジストリ登録中";
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", "SRNicoNico.exe", 0x00002AF8, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_GPU_RENDERING", "SRNicoNico.exe", 0x00000001, RegistryValueKind.DWord);

            var location = Owner.InstallLocation;

            var settings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SRNicoNico");

            if (Directory.Exists(location)) {

                Owner.Status = "元ファイルバックアップ中";
                if (File.Exists(Path.Combine(settings, "backup.zip"))) {

                    Owner.Status = "古いバックアップファイルを削除中";
                    File.Delete(Path.Combine(settings, "backup.zip"));
                }

                ZipFile.CreateFromDirectory(location, Path.Combine(settings, "backup.zip"), CompressionLevel.Optimal, true);
            }

            Owner.Status = "ディレクトリ作成中";
            Directory.CreateDirectory(location);
            Directory.CreateDirectory(Path.Combine(location, "tmp"));
            Directory.CreateDirectory(settings);

            using (var wc = new WebClient()) {

                Owner.Status = "本体ダウンロード中";
                await wc.DownloadFileTaskAsync(await UpdateChecker.GetLatestBinaryUrl(wc), Path.Combine(location, @"tmp\data"));
            }

            Owner.Status = "解凍中";
            using (var archive = ZipFile.OpenRead(Path.Combine(location, @"tmp\data"))) {

                foreach(var entry in archive.Entries) {

                    var fullname = Path.Combine(location, entry.FullName);
                    var dir = Path.GetDirectoryName(fullname);
                    if (!Directory.Exists(fullname)) {

                        Directory.CreateDirectory(dir);
                    }
                    if(!string.IsNullOrEmpty(Path.GetFileName(fullname))) {

                        entry.ExtractToFile(fullname, true);
                    }
                }
            }
            Owner.Status = "OKOK";
        }
    }
}
