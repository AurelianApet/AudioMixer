using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioMixer
{
    public partial class PlayerController : UserControl
    {
        public static PlayerController it;
        public PlayerController()
        {
            it = this;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            InitializeComponent();
            Init();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Control a = (Control)this;
            g.DrawPath(new Pen(MainForm.borderColor, 2.3f), Video.Controls.ColorSlider.CreateRoundRectPath(new Rectangle(1, 1, a.Width - 3, a.Height - 3), new Size(7, 7)));
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            stopBtn.Location = new Point(Width / 2 - stopBtn.Width / 2, Height / 2 - stopBtn.Height / 2 - 1);
            playBtn.Location = new Point(Width * 2 / 9 - playBtn.Width / 2, Height / 2 - playBtn.Height / 2 - 1);
            pauseBtn.Location = new Point(Width * 7 / 9 - pauseBtn.Width / 2, Height / 2 - pauseBtn.Height / 2 - 1);
        }
        public void Init()
        {
            playBtn.MouseHover += (sender, e) => Cursor = Cursors.Hand;
            playBtn.MouseLeave += (sender, e) => Cursor = Cursors.Default;
            playBtn.MouseClick += (sender, e) =>
            {
                if (TimeLineContent.GetInstance() != null)
                {
                    TimeLineContent.GetInstance().PlayOrStop(2);
                }
            };

            stopBtn.MouseHover += (sender, e) => Cursor = Cursors.Hand;
            stopBtn.MouseLeave += (sender, e) => Cursor = Cursors.Default;
            stopBtn.MouseClick += (sender, e) =>
            {
                if (TimeLineContent.GetInstance() != null)
                {
                    TimeLineContent.GetInstance().PlayOrStop(1);
                }
            };

            pauseBtn.MouseHover += (sender, e) => Cursor = Cursors.Hand;
            pauseBtn.MouseLeave += (sender, e) => Cursor = Cursors.Default;
            pauseBtn.MouseClick += (sender, e) =>
            {
                if (TimeLineContent.GetInstance() != null)
                {
                    TimeLineContent.GetInstance().PlayOrStop(1);
                }
            };
            playBtn.Size = pauseBtn.Size = stopBtn.Size = new Size((int)MainForm.F(28), (int)MainForm.F(28));
        }
    }
}
