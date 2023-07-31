namespace AudioMixer
{
    partial class FindForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mSearchName = new System.Windows.Forms.TextBox();
            this.mSearchResults = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // mSearchName
            // 
            this.mSearchName.Dock = System.Windows.Forms.DockStyle.Top;
            this.mSearchName.Location = new System.Drawing.Point(0, 0);
            this.mSearchName.Name = "mSearchName";
            this.mSearchName.Size = new System.Drawing.Size(244, 20);
            this.mSearchName.TabIndex = 0;
            this.mSearchName.TextChanged += new System.EventHandler(this.mSearchName_TextChanged);
            this.mSearchName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mSearchName_KeyDown);
            // 
            // mSearchResults
            // 
            this.mSearchResults.AutoScroll = true;
            this.mSearchResults.AutoSize = true;
            this.mSearchResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mSearchResults.Location = new System.Drawing.Point(0, 20);
            this.mSearchResults.Name = "mSearchResults";
            this.mSearchResults.Size = new System.Drawing.Size(244, 284);
            this.mSearchResults.TabIndex = 1;
            // 
            // FindForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(244, 304);
            this.Controls.Add(this.mSearchResults);
            this.Controls.Add(this.mSearchName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(260, 320);
            this.Name = "FindForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Find";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mSearchName;
        private System.Windows.Forms.Panel mSearchResults;
    }
}