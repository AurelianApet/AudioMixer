namespace AudioMixer
{
    partial class ScrollContent
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
            this.hScrollBar = new Video.Controls.ColorSlider();
            this.vScrollBar = new Video.Controls.ColorSlider();
            this.mTimeLineContent = new AudioMixer.TimeLineContent();
            this.SuspendLayout();
            // 
            // hScrollBar
            // 
            this.hScrollBar.BackColor = System.Drawing.Color.Transparent;
            this.hScrollBar.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.hScrollBar.IsPulse = false;
            this.hScrollBar.LargeChange = ((uint)(5u));
            this.hScrollBar.Location = new System.Drawing.Point(0, 263);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(671, 7);
            this.hScrollBar.SmallChange = ((uint)(1u));
            this.hScrollBar.TabIndex = 4;
            this.hScrollBar.Text = "colorSlider1";
            this.hScrollBar.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.hScrollBar.ThumbSize = 50;
            this.hScrollBar.Value = 0;
            // 
            // vScrollBar
            // 
            this.vScrollBar.BackColor = System.Drawing.Color.Transparent;
            this.vScrollBar.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.vScrollBar.IsPulse = false;
            this.vScrollBar.LargeChange = ((uint)(5u));
            this.vScrollBar.Location = new System.Drawing.Point(664, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.vScrollBar.Size = new System.Drawing.Size(7, 267);
            this.vScrollBar.SmallChange = ((uint)(1u));
            this.vScrollBar.TabIndex = 5;
            this.vScrollBar.Text = "colorSlider2";
            this.vScrollBar.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.vScrollBar.ThumbSize = 50;
            this.vScrollBar.Value = 0;
            this.vScrollBar.ValueChanged += new System.EventHandler(this.vScrollBar_ValueChanged);
            // 
            // mTimeLineContent
            // 
            this.mTimeLineContent.AllowDrop = true;
            this.mTimeLineContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(38)))), ((int)(((byte)(39)))), ((int)(((byte)(42)))));
            this.mTimeLineContent.Beat = 4;
            this.mTimeLineContent.CurPlayStartTime = ((long)(0));
            this.mTimeLineContent.CurPlayTime = ((long)(0));
            this.mTimeLineContent.Location = new System.Drawing.Point(0, 0);
            this.mTimeLineContent.LTime = ((long)(0));
            this.mTimeLineContent.Name = "mTimeLineContent";
            this.mTimeLineContent.OffsetY = 0;
            this.mTimeLineContent.RTime = ((long)(600000000));
            this.mTimeLineContent.Size = new System.Drawing.Size(675, 350);
            this.mTimeLineContent.TabIndex = 6;
            this.mTimeLineContent.TotalTime = ((long)(600000000));
            this.mTimeLineContent.Resize += new System.EventHandler(this.TimeLineContent_Resize);
            // 
            // ScrollContent
            // 
            this.AllowDrop = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.hScrollBar);
            this.Controls.Add(this.vScrollBar);
            this.Controls.Add(this.mTimeLineContent);
            this.Name = "ScrollContent";
            this.Size = new System.Drawing.Size(671, 270);
            this.ResumeLayout(false);

        }

        #endregion
        private Video.Controls.ColorSlider hScrollBar;
        private Video.Controls.ColorSlider vScrollBar;
        private TimeLineContent mTimeLineContent;
    }
}
