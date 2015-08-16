using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AxShockwaveFlashObjects;
using Flash.External;

namespace SRNicoNico.Views.Contents.Video {
    public partial class FlashForm : UserControl {


        private ExternalInterfaceProxy proxy;



        public FlashForm() {
            InitializeComponent();
            
   



        }

        private void FlashForm_Load(object sender, EventArgs e) {

            proxy = new ExternalInterfaceProxy(shockWaveFlash);
            shockWaveFlash.LoadMovie(0, @"E:\Development\MVVM\WPF\SRNicoNico\SRNicoNico\Flash\NicoNicoPlayer.swf");
            shockWaveFlash.Play();
            shockWaveFlash.ScaleMode = 2;




        }
    }
}
