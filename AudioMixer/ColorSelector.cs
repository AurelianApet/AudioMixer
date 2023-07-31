using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AudioMixer
{
    public partial class ColorSelector : UserControl
    {
        #region Events
        public event EventHandler ColorChanged;
        #endregion
        public ColorSelector()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            color1.BackColor = Color.FromArgb(0xff, 0x4d, 0x4d);
            color2.BackColor = Color.FromArgb(0x4d, 0x55, 0xff);
            color3.BackColor = Color.FromArgb(0x6c, 0xc2, 0x5b);
            color4.BackColor = Color.FromArgb(0xff, 0xad, 0x43);
            color5.BackColor = Color.FromArgb(0xfd, 0xff, 0x43);
            color6.BackColor = Color.FromArgb(0xd5, 0x43, 0xff);
            color7.BackColor = Color.FromArgb(0x53, 0xe4, 0xf1);
        }

        private void color1_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void color1_MouseDown(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
            if (ColorChanged!=null) ColorChanged(sender, new EventArgs());
            Panel a = (Panel)sender;
            this.Visible = false;
        }

        private Timer hideTimer = null;
        private void ColorSelector_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (hideTimer != null)
                {
                    hideTimer.Stop();
                    hideTimer.Dispose();
                }
                hideTimer = new Timer();
                hideTimer.Interval = 5000;
                hideTimer.Start();
                hideTimer.Tick += hideme;
            }
        }

        private void hideme(object sender, EventArgs e)
        {
            this.Visible = false;
            if (hideTimer != null)
            {
                hideTimer.Stop();
                hideTimer.Dispose();
            }
        }

        private void color3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void color4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void color5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void color6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void color7_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
