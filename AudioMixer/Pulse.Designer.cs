namespace AudioMixer
{
    partial class Pulse
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
            this.groupPanel = new System.Windows.Forms.Panel();
            this.beat = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.ppm = new System.Windows.Forms.Label();
            this.editValue = new System.Windows.Forms.TextBox();
            this.groupPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupPanel
            // 
            this.groupPanel.AutoSize = true;
            this.groupPanel.Controls.Add(this.beat);
            this.groupPanel.Controls.Add(this.label);
            this.groupPanel.Controls.Add(this.ppm);
            this.groupPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupPanel.Location = new System.Drawing.Point(0, 0);
            this.groupPanel.Margin = new System.Windows.Forms.Padding(0);
            this.groupPanel.Name = "groupPanel";
            this.groupPanel.Size = new System.Drawing.Size(200, 46);
            this.groupPanel.TabIndex = 0;
            // 
            // beat
            // 
            this.beat.AutoSize = true;
            this.beat.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.beat.ForeColor = System.Drawing.Color.Snow;
            this.beat.Location = new System.Drawing.Point(120, 0);
            this.beat.Margin = new System.Windows.Forms.Padding(0);
            this.beat.Name = "beat";
            this.beat.Size = new System.Drawing.Size(38, 24);
            this.beat.TabIndex = 4;
            this.beat.Text = "4/4";
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.ForeColor = System.Drawing.Color.Snow;
            this.label.Location = new System.Drawing.Point(2, 0);
            this.label.Margin = new System.Windows.Forms.Padding(0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(53, 24);
            this.label.TabIndex = 3;
            this.label.Text = "BPM";
            // 
            // ppm
            // 
            this.ppm.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.ppm.ForeColor = System.Drawing.Color.Snow;
            this.ppm.Location = new System.Drawing.Point(64, 0);
            this.ppm.Margin = new System.Windows.Forms.Padding(0);
            this.ppm.Name = "ppm";
            this.ppm.Size = new System.Drawing.Size(46, 28);
            this.ppm.TabIndex = 2;
            this.ppm.Text = "120";
            this.ppm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // editValue
            // 
            this.editValue.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.editValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.editValue.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.editValue.Location = new System.Drawing.Point(80, 7);
            this.editValue.Margin = new System.Windows.Forms.Padding(0);
            this.editValue.Name = "editValue";
            this.editValue.Size = new System.Drawing.Size(40, 17);
            this.editValue.TabIndex = 1;
            this.editValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Pulse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.editValue);
            this.Controls.Add(this.groupPanel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "Pulse";
            this.Size = new System.Drawing.Size(200, 46);
            this.groupPanel.ResumeLayout(false);
            this.groupPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel groupPanel;
        private System.Windows.Forms.Label ppm;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Label beat;
        private System.Windows.Forms.TextBox editValue;
    }
}
