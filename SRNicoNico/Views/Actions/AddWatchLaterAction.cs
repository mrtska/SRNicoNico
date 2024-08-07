using System.Windows;
using Microsoft.Xaml.Behaviors;
using SRNicoNico.Models;
using SRNicoNico.Services;
using SRNicoNico.ViewModels;
using Unity;

namespace SRNicoNico.Views.Actions {
    /// <summary>
    /// Viewからあとで見るに登録を行うトリガーアクション
    /// </summary>
    public class AddWatchLaterAction : TriggerAction<DependencyObject> {

        /// <summary>
        /// URL本体
        /// </summary>
        public string WatchId {
            get { return (string)GetValue(WatchIdProperty); }
            set { SetValue(WatchIdProperty, value); }
        }
        public static readonly DependencyProperty WatchIdProperty =
            DependencyProperty.Register(nameof(WatchId), typeof(string), typeof(AddWatchLaterAction), new PropertyMetadata(""));

        protected async override void Invoke(object parameter) {

            if (string.IsNullOrEmpty(WatchId)) {

                return;
            }

            var mylistService = App.UnityContainer!.Resolve<IMylistService>();
            var vm = App.UnityContainer!.Resolve<MainWindowViewModel>();

            vm.Status = "あとで見るに登録中";
            try {

                var result = await mylistService.AddWatchLaterAsync(WatchId, null);
                if (result) {
                    vm.Status = "あとで見るに登録しました";
                } else {
                    vm.Status = "すでに登録済みです";
                }

            } catch (StatusErrorException e) {
                vm.Status = $"あとで見るに動画を登録出来ませんでした。 ステータスコード: {e.StatusCode}";
                return;
            }
        }
    }
}
