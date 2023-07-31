namespace AudioMixer
{
    partial class AudioTrack
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
            this.TrackName = new AudioMixer.ShowAndEditName();
            this.colorselector = new AudioMixer.ColorSelector();
            this.EQSetting = new AudioMixer.MyCheckBox();
            this.Solo = new AudioMixer.MyCheckBox();
            this.Mute = new AudioMixer.MyCheckBox();
            this.mColorShow = new AudioMixer.ColorShow();
            this.SuspendLayout();
            // 
            // TrackName
            // 
            this.TrackName.AutoSize = true;
            this.TrackName.BackColor = System.Drawing.Color.Transparent;
            this.TrackName.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.TrackName.Location = new System.Drawing.Point(99, 11);
            this.TrackName.Margin = new System.Windows.Forms.Padding(0);
            this.TrackName.MinimumSize = new System.Drawing.Size(20, 0);
            this.TrackName.Name = "TrackName";
            this.TrackName.Padding = new System.Windows.Forms.Padding(2);
            this.TrackName.Size = new System.Drawing.Size(82, 30);
            this.TrackName.TabIndex = 11;
            // 
            // colorselector
            // 
            this.colorselector.BackColor = System.Drawing.Color.White;
            this.colorselector.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorselector.Location = new System.Drawing.Point(10, 2);
            this.colorselector.Name = "colorselector";
            this.colorselector.Size = new System.Drawing.Size(180, 30);
            this.colorselector.TabIndex = 4;
            this.colorselector.Visible = false;
            this.colorselector.ColorChanged += new System.EventHandler(this.colorselector_ColorChanged);
            // 
            // EQSetting
            // 
            this.EQSetting.BackColor = System.Drawing.Color.Transparent;
            this.EQSetting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EQSetting.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EQSetting.IsOn = false;
            this.EQSetting.Location = new System.Drawing.Point(84, 3);
            this.EQSetting.Name = "EQSetting";
            this.EQSetting.Size = new System.Drawing.Size(26, 20);
            this.EQSetting.TabIndex = 10;
            // 
            // Solo
            // 
            this.Solo.BackColor = System.Drawing.Color.Transparent;
            this.Solo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Solo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Solo.IsOn = false;
            this.Solo.Location = new System.Drawing.Point(58, 3);
            this.Solo.Name = "Solo";
            this.Solo.Size = new System.Drawing.Size(26, 20);
            this.Solo.TabIndex = 9;
            // 
            // Mute
            // 
            this.Mute.BackColor = System.Drawing.Color.Transparent;
            this.Mute.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Mute.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Mute.IsOn = false;
            this.Mute.Location = new System.Drawing.Point(32, 3);
            this.Mute.Name = "Mute";
            this.Mute.Size = new System.Drawing.Size(26, 20);
            this.Mute.TabIndex = 8;
            // 
            // mColorShow
            // 
            this.mColorShow.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.mColorShow.BackColor = System.Drawing.Color.Transparent;
            this.mColorShow.BColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(77)))), ((int)(((byte)(77)))));
            this.mColorShow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mColorShow.Dock = System.Windows.Forms.DockStyle.Left;
            this.mColorShow.IsFolder = false;
            this.mColorShow.Location = new System.Drawing.Point(2, 3);
            this.mColorShow.Margin = new System.Windows.Forms.Padding(0);
            this.mColorShow.Name = "mColorShow";
            this.mColorShow.Number = 1;
            this.mColorShow.Size = new System.Drawing.Size(25, 74);
            this.mColorShow.TabIndex = 3;
            this.mColorShow.ColorSelectBtnClick += new System.EventHandler(this.numShow1_Click);
            // 
            // AudioTrack
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.colorselector);
            this.Controls.Add(this.EQSetting);
            this.Controls.Add(this.Solo);
            this.Controls.Add(this.Mute);
            this.Controls.Add(this.mColorShow);
            this.Controls.Add(this.TrackName);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MinimumSize = new System.Drawing.Size(278, 30);
            this.Name = "AudioTrack";
            this.Padding = new System.Windows.Forms.Padding(2, 3, 10, 3);
            this.Size = new System.Drawing.Size(278, 80);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ColorShow mColorShow;
        private ColorSelector colorselector;
        private MyCheckBox Mute;
        private MyCheckBox Solo;
        private MyCheckBox EQSetting;
        private ShowAndEditName TrackName;
    }
}
