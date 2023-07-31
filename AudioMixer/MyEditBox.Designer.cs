namespace AudioMixer
{
    partial class MyEditBox
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
            this.editValue = new System.Windows.Forms.TextBox();
            this.showValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // editValue
            // 
            this.editValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.editValue.Location = new System.Drawing.Point(3, 3);
            this.editValue.Name = "editValue";
            this.editValue.Size = new System.Drawing.Size(73, 21);
            this.editValue.TabIndex = 0;
            this.editValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // showValue
            // 
            this.showValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.showValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.showValue.Location = new System.Drawing.Point(3, 3);
            this.showValue.Name = "showValue";
            this.showValue.Size = new System.Drawing.Size(73, 21);
            this.showValue.TabIndex = 1;
            this.showValue.Text = "label1";
            this.showValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MyEditBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.showValue);
            this.Controls.Add(this.editValue);
            this.Name = "MyEditBox";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(79, 27);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox editValue;
        private System.Windows.Forms.Label showValue;
    }
}
