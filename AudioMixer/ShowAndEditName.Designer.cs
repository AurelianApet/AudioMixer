namespace AudioMixer
{
    partial class ShowAndEditName
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
            this.TrackName = new System.Windows.Forms.Label();
            this.editName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TrackName
            // 
            this.TrackName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrackName.Location = new System.Drawing.Point(0, 0);
            this.TrackName.Name = "TrackName";
            this.TrackName.Size = new System.Drawing.Size(124, 40);
            this.TrackName.TabIndex = 0;
            this.TrackName.Text = "Drum";
            this.TrackName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.TrackName.DoubleClick += new System.EventHandler(this.TrackName_DoubleClick);
            // 
            // editName
            // 
            this.editName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editName.Enabled = false;
            this.editName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editName.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.editName.Location = new System.Drawing.Point(0, 0);
            this.editName.Margin = new System.Windows.Forms.Padding(0);
            this.editName.MinimumSize = new System.Drawing.Size(4, 25);
            this.editName.Name = "editName";
            this.editName.Size = new System.Drawing.Size(127, 23);
            this.editName.TabIndex = 1;
            this.editName.Visible = false;
            this.editName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.editName_KeyDown);
            this.editName.Leave += new System.EventHandler(this.editName_Leave);
            // 
            // ShowAndEditName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.editName);
            this.Controls.Add(this.TrackName);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(0, 20);
            this.Name = "ShowAndEditName";
            this.Size = new System.Drawing.Size(127, 55);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label TrackName;
        public System.Windows.Forms.TextBox editName;
    }
}
