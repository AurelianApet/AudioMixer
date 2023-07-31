namespace AudioMixer
{
    partial class FaderCtrl
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
            this.maxValue = new System.Windows.Forms.Label();
            this.setValue = new System.Windows.Forms.Label();
            this.editSetValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // maxValue
            // 
            this.maxValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.maxValue.Cursor = System.Windows.Forms.Cursors.Hand;
            this.maxValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.maxValue.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.maxValue.Location = new System.Drawing.Point(47, 12);
            this.maxValue.Margin = new System.Windows.Forms.Padding(0);
            this.maxValue.Name = "maxValue";
            this.maxValue.Size = new System.Drawing.Size(40, 20);
            this.maxValue.TabIndex = 1;
            this.maxValue.Text = " -30.00 ";
            this.maxValue.DoubleClick += new System.EventHandler(this.maxValue_DoubleClick);
            // 
            // setValue
            // 
            this.setValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.setValue.Cursor = System.Windows.Forms.Cursors.Hand;
            this.setValue.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.setValue.Location = new System.Drawing.Point(2, 12);
            this.setValue.Margin = new System.Windows.Forms.Padding(3);
            this.setValue.Name = "setValue";
            this.setValue.Size = new System.Drawing.Size(39, 15);
            this.setValue.TabIndex = 0;
            this.setValue.Text = "-30.00";
            this.setValue.DoubleClick += new System.EventHandler(this.setValue_DoubleClick);
            // 
            // editSetValue
            // 
            this.editSetValue.Enabled = false;
            this.editSetValue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.editSetValue.Location = new System.Drawing.Point(2, 10);
            this.editSetValue.Name = "editSetValue";
            this.editSetValue.Size = new System.Drawing.Size(39, 20);
            this.editSetValue.TabIndex = 2;
            this.editSetValue.Visible = false;
            this.editSetValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.editSetValue_KeyDown);
            this.editSetValue.Leave += new System.EventHandler(this.editSetValue_Leave);
            // 
            // FaderCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.editSetValue);
            this.Controls.Add(this.maxValue);
            this.Controls.Add(this.setValue);
            this.Name = "FaderCtrl";
            this.Size = new System.Drawing.Size(114, 97);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label maxValue;
        private System.Windows.Forms.Label setValue;
        private System.Windows.Forms.TextBox editSetValue;
    }
}
