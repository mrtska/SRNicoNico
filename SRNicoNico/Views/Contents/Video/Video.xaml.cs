using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;
using SRNicoNico.Views.Contents.Interface;
using SRNicoNico.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SRNicoNico.Views.Contents.Video {
	/// <summary>
	/// Video.xaml の相互作用ロジック
	/// </summary>
	public partial class Video : ExternalizableUserControl {
		public Video() {
            InitializeComponent();
            if(Settings.Instance.VideoInfoPlacement == "Right") {

                Application.Current.Resources["VideoColumn"] = 0;
                Application.Current.Resources["InfoColumn"] = 1;
                Application.Current.Resources["GridWidth1"] = new GridLength(1.0, GridUnitType.Star);
                Application.Current.Resources["GridWidth2"] = new GridLength(300);
            } else {

                Application.Current.Resources["VideoColumn"] = 1;
                Application.Current.Resources["InfoColumn"] = 0;
                Application.Current.Resources["GridWidth1"] = new GridLength(300);
                Application.Current.Resources["GridWidth2"] = new GridLength(1.0, GridUnitType.Star);

            }
        }
    }
}
