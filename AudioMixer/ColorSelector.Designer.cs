namespace AudioMixer
{
    partial class ColorSelector
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
            this.color1 = new System.Windows.Forms.Panel();
            this.color2 = new System.Windows.Forms.Panel();
            this.color3 = new System.Windows.Forms.Panel();
            this.color4 = new System.Windows.Forms.Panel();
            this.color5 = new System.Windows.Forms.Panel();
            this.color6 = new System.Windows.Forms.Panel();
            this.color7 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // color1
            // 
            this.color1.BackColor = System.Drawing.Color.Red;
            this.color1.Location = new System.Drawing.Point(1, 3);
            this.color1.Name = "color1";
            this.color1.Size = new System.Drawing.Size(22, 22);
            this.color1.TabIndex = 0;
            this.color1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.color1_MouseDown);
            this.color1.MouseHover += new System.EventHandler(this.color1_MouseHover);
            // 
            // color2
            // 
            this.color2.BackColor = System.Drawing.Color.DarkOrange;
            this.color2.Location = new System.Drawing.Point(26, 3);
            this.color2.Name = "color2";
            this.color2.Size = new System.Drawing.Size(22, 22);
            this.color2.TabIndex = 1;
            this.color2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.color1_MouseDown);
            this.color2.MouseHover += new System.EventHandler(this.color1_MouseHover);
            // 
            // color3
            // 
            this.color3.BackColor = System.Drawing.Color.Yellow;
            this.color3.Location = new System.Drawing.Point(51, 3);
            this.color3.Name = "color3";
            this.color3.Size = new System.Drawing.Size(22, 22);
            this.color3.TabIndex = 1;
            this.color3.Paint += new System.Windows.Forms.PaintEventHandler(this.color3_Paint);
            this.color3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.color1_MouseDown);
            this.color3.MouseHover += new System.EventHandler(this.color1_MouseHover);
            // 
            // color4
            // 
            this.color4.BackColor = System.Drawing.Color.Green;
            this.color4.Location = new System.Drawing.Point(76, 3);
            this.color4.Name = "color4";
            this.color4.Size = new System.Drawing.Size(22, 22);
            this.color4.TabIndex = 1;
            this.color4.Paint += new System.Windows.Forms.PaintEventHandler(this.color4_Paint);
            this.color4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.color1_MouseDown);
            this.color4.MouseHover += new System.EventHandler(this.color1_MouseHover);
            // 
            // color5
            // 
            this.color5.BackColor = System.Drawing.Color.Blue;
            this.color5.Location = new System.Drawing.Point(101, 3);
            this.color5.Name = "color5";
            this.color5.Size = new System.Drawing.Size(22, 22);
            this.color5.TabIndex = 1;
            this.color5.Paint += new System.Windows.Forms.PaintEventHandler(this.color5_Paint);
            this.color5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.color1_MouseDown);
            this.color5.MouseHover += new System.EventHandler(this.color1_MouseHover);
            // 
            // color6
            // 
            this.color6.BackColor = System.Drawing.Color.DarkBlue;
            this.color6.Location = new System.Drawing.Point(127, 3);
            this.color6.Name = "color6";
            this.color6.Size = new System.Drawing.Size(22, 22);
            this.color6.TabIndex = 1;
            this.color6.Paint += new System.Windows.Forms.PaintEventHandler(this.color6_Paint);
            this.color6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.color1_MouseDown);
            this.color6.MouseHover += new System.EventHandler(this.color1_MouseHover);
            // 
            // color7
            // 
            this.color7.BackColor = System.Drawing.Color.Purple;
            this.color7.Location = new System.Drawing.Point(153, 3);
            this.color7.Name = "color7";
            this.color7.Size = new System.Drawing.Size(22, 22);
            this.color7.TabIndex = 1;
            this.color7.Paint += new System.Windows.Forms.PaintEventHandler(this.color7_Paint);
            this.color7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.color1_MouseDown);
            this.color7.MouseHover += new System.EventHandler(this.color1_MouseHover);
            // 
            // ColorSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.color7);
            this.Controls.Add(this.color6);
            this.Controls.Add(this.color5);
            this.Controls.Add(this.color4);
            this.Controls.Add(this.color3);
            this.Controls.Add(this.color2);
            this.Controls.Add(this.color1);
            this.Name = "ColorSelector";
            this.Size = new System.Drawing.Size(180, 27);
            this.VisibleChanged += new System.EventHandler(this.ColorSelector_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel color1;
        private System.Windows.Forms.Panel color2;
        private System.Windows.Forms.Panel color3;
        private System.Windows.Forms.Panel color4;
        private System.Windows.Forms.Panel color5;
        private System.Windows.Forms.Panel color6;
        private System.Windows.Forms.Panel color7;
    }
}
