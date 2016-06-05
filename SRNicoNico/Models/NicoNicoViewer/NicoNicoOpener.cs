using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using SRNicoNico.ViewModels;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class NicoNicoOpener : NotificationObject {

        public static TabItemViewModel Open(Uri uri) {

            return Open(uri.OriginalString);
        }


        //URLから適当なViewを開く
        public static TabItemViewModel Replace(TabItemViewModel old, string url) {

            if(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {

                System.Diagnostics.Process.Start(url);
                return null;
            }

            if(url.StartsWith("http://www.nicovideo.jp/watch/")) {

                var vm = new VideoViewModel(url);
                App.ViewModelRoot.ReplaceTabAndSetCurrent(old, vm);
                vm.Initialize();
                return vm;
            } else if(url.StartsWith("http://www.nicovideo.jp/user/")) {

                var vm = new UserViewModel(url);
                App.ViewModelRoot.ReplaceTabAndSetCurrent(old, vm);
                return vm;
            } else if(url.StartsWith("http://www.nicovideo.jp/mylist/")) {

                var vm = new PublicMylistViewModel(url);
                App.ViewModelRoot.ReplaceTabAndSetCurrent(old, vm);
                return vm;
            } else if(url.StartsWith("http://com.nicovideo.jp/community/")) {

                var vm = new CommunityViewModel(url);
                App.ViewModelRoot.ReplaceTabAndSetCurrent(old, vm);
                return vm;
            } else if(url.StartsWith("http://live.nicovideo.jp/watch/")) {

                var vm = new LiveViewModel(url);
                App.ViewModelRoot.ReplaceTabAndSetCurrent(old, vm);
                return vm;
            } else {

                System.Diagnostics.Process.Start(url);
                return null;
            }
        }

        //URLから適当なViewを開く
        public static TabItemViewModel Open(string url) {

            if(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {

                System.Diagnostics.Process.Start(url);
                return null;
            }

            if(url.StartsWith("http://www.nicovideo.jp/watch/")) {

                var vm = new VideoViewModel(url);
                App.ViewModelRoot.AddTabAndSetCurrent(vm);
                vm.Initialize();
                return vm;
            } else if(url.StartsWith("http://www.nicovideo.jp/user/")) {

                var vm = new UserViewModel(url);
                App.ViewModelRoot.AddTabAndSetCurrent(vm);
                return vm;
            } else if(url.StartsWith("http://www.nicovideo.jp/mylist/")) {

                var vm = new PublicMylistViewModel(url);
                App.ViewModelRoot.AddTabAndSetCurrent(vm);
                return vm;
            } else if(url.StartsWith("http://com.nicovideo.jp/community/")) {

                var vm = new CommunityViewModel(url);
                App.ViewModelRoot.AddTabAndSetCurrent(vm);
                return vm;
            } else if(url.StartsWith("http://live.nicovideo.jp/watch/")) {

                var vm = new LiveViewModel(url);
                App.ViewModelRoot.AddTabAndSetCurrent(vm);
                return vm;
            } else {

                System.Diagnostics.Process.Start(url);
                return null;
            }
        }

        //ニコニコのURLが何を指しているかを返す
        public static NicoNicoUrlType GetType(string url) {

            if(url.StartsWith("http://www.nicovideo.jp/watch/")) {

                return NicoNicoUrlType.Video;
            }
            if(url.StartsWith("http://www.nicovideo.jp/user/")) {

                return NicoNicoUrlType.User;
            }
            if(url.StartsWith("http://www.nicovideo.jp/mylist/")) {

                return NicoNicoUrlType.Mylist;
            }
            if(url.StartsWith("http://com.nicovideo.jp/community/")) {

                return NicoNicoUrlType.Community;
            }
            if(url.StartsWith("http://live.nicovideo.jp/watch/")) {

                return NicoNicoUrlType.Live;
            }

            return NicoNicoUrlType.Other;
        }

    }
    
    public enum NicoNicoUrlType {

        Video,
        Live,
        Mylist,
        User,
        Community,
        Other

    }
}
