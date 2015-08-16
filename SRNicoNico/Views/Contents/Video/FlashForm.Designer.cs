namespace SRNicoNico.Views.Contents.Video {
    partial class FlashForm {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlashForm));
            this.axShockwaveFlash1 = new AxShockwaveFlashObjects.AxShockwaveFlash();
            this.shockWaveFlash = new AxShockwaveFlashObjects.AxShockwaveFlash();
            ((System.ComponentModel.ISupportInitialize)(this.axShockwaveFlash1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.shockWaveFlash)).BeginInit();
            this.SuspendLayout();
            // 
            // axShockwaveFlash1
            // 
            this.axShockwaveFlash1.Enabled = true;
            this.axShockwaveFlash1.Location = new System.Drawing.Point(0, 0);
            this.axShockwaveFlash1.Name = "axShockwaveFlash1";
            this.axShockwaveFlash1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axShockwaveFlash1.OcxState")));
            this.axShockwaveFlash1.Size = new System.Drawing.Size(674, 407);
            this.axShockwaveFlash1.TabIndex = 0;
            // 
            // shockWaveFlash
            // 
            this.shockWaveFlash.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shockWaveFlash.Enabled = true;
            this.shockWaveFlash.Location = new System.Drawing.Point(0, 0);
            this.shockWaveFlash.Name = "shockWaveFlash";
            this.shockWaveFlash.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("shockWaveFlash.OcxState")));
            this.shockWaveFlash.Size = new System.Drawing.Size(674, 407);
            this.shockWaveFlash.TabIndex = 0;
            // 
            // FlashForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.shockWaveFlash);
            this.Name = "FlashForm";
            this.Size = new System.Drawing.Size(674, 407);
            this.Load += new System.EventHandler(this.FlashForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axShockwaveFlash1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shockWaveFlash)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxShockwaveFlashObjects.AxShockwaveFlash axShockwaveFlash1;
        private AxShockwaveFlashObjects.AxShockwaveFlash shockWaveFlash;
    }
}
