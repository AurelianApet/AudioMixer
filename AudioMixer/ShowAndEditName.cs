using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Text;
using AudioMixer.Properties;
using System.Runtime.InteropServices;

namespace AudioMixer
{
    public partial class ShowAndEditName : UserControl
    {
        private bool centerAlignment = false;
        public void CenterAlignment(bool value)
        {
            centerAlignment = value;
            editName.Font = MainForm.GetFont(10);
            TrackName.ForeColor = Color.FromArgb(0xff, 0xe8, 0xe8, 0xe8);
            if (value)
            {
                Padding = new Padding(1);

                TrackName.TextAlign = ContentAlignment.TopCenter;
                TrackName.Dock = DockStyle.Fill;
                TrackName.Padding = new Padding(1);
                TrackName.Font = MainForm.GetFont(10);
                TrackName.ForeColor = Color.White;

                editName.TextAlign = HorizontalAlignment.Center;
                editName.Dock = DockStyle.Fill;
                editName.Location = new Point(0, 0);
                editName.Font = MainForm.GetFont(10);

//                Height = 30;
            } else
            {
                Padding = new Padding(1);
                TrackName.TextAlign = ContentAlignment.TopCenter;
                TrackName.Padding = new Padding(1);
                TrackName.Dock = DockStyle.Fill;
                TrackName.Font = MainForm.GetFont(10);

                editName.TextAlign = HorizontalAlignment.Center;
                editName.Dock = DockStyle.Fill;
                editName.Font = MainForm.GetFont(10);

//                Height = 30;
            }
        }
        public ShowAndEditName()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            InitializeComponent();
            //System.Drawing.Text.PrivateFontCollection privateFonts = new System.Drawing.Text.PrivateFontCollection();
            //privateFonts.AddFontFile(@"E:\work\13.AudioMixer\AudioMixer(real)\AudioMixer\Resources\NotoSansCJKkr-hinted\NotoSansCJKkr-Bold.otf");
            //System.Drawing.Font font = new Font(privateFonts.Families[0], 12);
            CenterAlignment(false);
            this.editName.LostFocus += EditName_LostFocus;
        }

        private void EditName_LostFocus(object sender, EventArgs e)
        {
            ShowTrackName();
        }

        public void SetText(string str)
        {
            if (this.InvokeRequired)
            {
                Invoke((Action)delegate { TrackName.Text = str; });
            } else
            {
                TrackName.Text = str;
            }
        }

        public string GetName()
        {
            return TrackName.Text;
        }

        private void TrackName_DoubleClick(object sender, EventArgs e)
        {
            TrackName.Visible = false;
            editName.Visible = true;
            editName.Enabled = true;
            editName.Text = TrackName.Text;
            editName.Focus();
            editName.SelectAll();
        }

        public void Edit()
        {
            TrackName_DoubleClick(this, new EventArgs());
        }

        public bool Editing
        {
            get { return editName.Visible; }
        }

        private void EditEnd(EventArgs e)
        {
            ShowTrackName();
        }

        private void editName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString()=="Return")
            {
                ShowTrackName();
            }
        }

        private void editName_Leave(object sender, EventArgs e)
        {
            ShowTrackName();
        }

        private void ShowTrackName()
        {
            this.editName.Enabled = false;
            this.editName.Visible = false;
            TrackName.Visible = true;
            if (TrackView.GetInstance()!= null)
            {
                TrackView.GetInstance().tmp.Enabled = true;
                TrackView.GetInstance().tmp.Focus();
                TrackView.GetInstance().tmp.Enabled = false;
            }
            if (this.editName.Text != "") this.TrackName.Text = this.editName.Text;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            CenterAlignment(centerAlignment);
        }
    }
}
