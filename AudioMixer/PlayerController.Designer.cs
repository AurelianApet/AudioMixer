namespace AudioMixer
{
    partial class PlayerController
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.stopBtn = new System.Windows.Forms.PictureBox();
            this.pauseBtn = new System.Windows.Forms.PictureBox();
            this.playBtn = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.stopBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pauseBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playBtn)).BeginInit();
            this.SuspendLayout();
            // 
            // stopBtn
            // 
            this.stopBtn.BackgroundImage = global::AudioMixer.Properties.Resources.Stop1;
            this.stopBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.stopBtn.InitialImage = null;
            this.stopBtn.Location = new System.Drawing.Point(91, 4);
            this.stopBtn.Name = "stopBtn";
            this.stopBtn.Size = new System.Drawing.Size(20, 20);
            this.stopBtn.TabIndex = 2;
            this.stopBtn.TabStop = false;
            // 
            // pauseBtn
            // 
            this.pauseBtn.BackgroundImage = global::AudioMixer.Properties.Resources.Pause1;
            this.pauseBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pauseBtn.InitialImage = null;
            this.pauseBtn.Location = new System.Drawing.Point(141, 4);
            this.pauseBtn.Name = "pauseBtn";
            this.pauseBtn.Size = new System.Drawing.Size(20, 20);
            this.pauseBtn.TabIndex = 1;
            this.pauseBtn.TabStop = false;
            // 
            // playBtn
            // 
            this.playBtn.BackgroundImage = global::AudioMixer.Properties.Resources.play1;
            this.playBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.playBtn.InitialImage = null;
            this.playBtn.Location = new System.Drawing.Point(41, 4);
            this.playBtn.Name = "playBtn";
            this.playBtn.Size = new System.Drawing.Size(20, 20);
            this.playBtn.TabIndex = 0;
            this.playBtn.TabStop = false;
            // 
            // PlayerController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stopBtn);
            this.Controls.Add(this.pauseBtn);
            this.Controls.Add(this.playBtn);
            this.Name = "PlayerController";
            this.Size = new System.Drawing.Size(281, 71);
            ((System.ComponentModel.ISupportInitialize)(this.stopBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pauseBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playBtn)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox playBtn;
        private System.Windows.Forms.PictureBox pauseBtn;
        private System.Windows.Forms.PictureBox stopBtn;
    }
}
