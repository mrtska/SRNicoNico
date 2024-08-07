using System.Windows;
using Microsoft.Xaml.Behaviors;
using SRNicoNico.Entities;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;
using SRNicoNico.ViewModels;
using Unity;

namespace SRNicoNico.Views.Actions {
    /// <summary>
    /// 対象の動画を投稿したアカウントをミュートするトリガーアクション
    /// </summary>
    public class MuteAccountAction : TriggerAction<DependencyObject> {

        /// <summary>
        /// VideoItem本体
        /// </summary>
        public VideoItem Item {
            get { return (VideoItem)GetValue(WatchIdProperty); }
            set { SetValue(WatchIdProperty, value); }
        }
        public static readonly DependencyProperty WatchIdProperty =
            DependencyProperty.Register(nameof(Item), typeof(VideoItem), typeof(MuteAccountAction), new FrameworkPropertyMetadata(null));

        protected async override void Invoke(object parameter) {

            if (Item == null || Item.OwnerType == null) {
                return;
            }

            var accountService = App.UnityContainer!.Resolve<IAccountService>();
            var vm = App.UnityContainer!.Resolve<MainWindowViewModel>();

            if (Item.OwnerType == "user") {

                await accountService.AddMutedAccountAsync(AccountType.User, Item.OwnerId!);
            } else if (Item.OwnerType == "channel") {

                await accountService.AddMutedAccountAsync(AccountType.Channel, Item.OwnerId!);
            } else if (Item.OwnerType == "community") {

                await accountService.AddMutedAccountAsync(AccountType.Community, Item.OwnerId!);
            } else {
                return;
            }

            Item.IsMuted = true;
            vm.Status = "ミュート設定に登録しました";
        }
    }
}
