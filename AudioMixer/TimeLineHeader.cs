using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AudioMixer
{
    public partial class TimeLineHeader : UserControl
    {
        private static TimeLineHeader instance = null;
        public static TimeLineHeader GetInstance()
        {
            return instance;
        }
        public TimeLineHeader()
        {
            instance = this;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint, true);
            InitializeComponent();
            SmallLines = new List<Point>();
            BigLines = new List<Point>();
            StringPos = new List<Point>();
            Strings = new List<string>();
        }

        Rectangle rect, rect1;

        protected override void OnPaint(PaintEventArgs e)
        {
            Font = MainForm.GetFont(7f);
            Graphics gr = e.Graphics;
            Pen bigPen = new Pen(Color.FromArgb(0xff, 0x7f, 0x7f, 0x7f));
            Pen smallPen = new Pen(Color.FromArgb(0xff, 0x58, 0x58, 0x58));
            Color fontColor = Color.FromArgb(0xff, 0xc4, 0xe1, 0xfb);
            for (int i=0; i<SmallLines.Count; i+=2)
            {
                gr.DrawLine(smallPen, SmallLines[i], SmallLines[i + 1]);
            }
            for (int i=0; i<BigLines.Count; i+=2)
            {
                gr.DrawLine(bigPen, BigLines[i], BigLines[i + 1]);
            }
            for (int i=0; i<Strings.Count; i++)
            {
                gr.DrawString(Strings[i], Font, new SolidBrush(fontColor), StringPos[i]);
            }
            gr.FillRectangle(new SolidBrush(Color.FromArgb(0xff, 0x80, 0x80, 0x80)), 0, 0, Width, (int)MainForm.F(25));
            gr.DrawRectangle(new Pen(Color.FromArgb(0xff, 0xc0, 0xc0, 0xc0), 2), 1, 1, Width-3, (int)MainForm.F(22));
//            ControlPaint.DrawBorder(gr, Bounds, Color.DimGray, ButtonBorderStyle.Solid);
            if (TimeLineContent.GetInstance() == null) return;
            long tot = TimeLineContent.GetInstance().TotalTime;
            long lft = TimeLineContent.GetInstance().LTime;
            long rgt = TimeLineContent.GetInstance().RTime;
            int w = Width - 8, lpos = (int)((double)w * lft / tot);
            int rpos = (int)((double)w * rgt / tot), W=5;
            if (rpos - lpos < W*2)
            {
                int t = (rpos + lpos) / 2;
                if (t < W) t = W;
                if (t > w - W) t = w - W;
                lpos = t - W; rpos = t + W;
            }
            rect = new Rectangle(lpos + 3, 3, rpos - lpos + 1, (int)MainForm.F(16));
            rect1 = new Rectangle(1, 3 + (int)MainForm.F(16), Width-10, Height - 3 - (int)MainForm.F(16));
            gr.DrawRectangle(new Pen(Color.FromArgb(0xff, 0x13, 0x13, 0x13), 2), rect);
        }

        bool isIn = false;
        Point pos;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (TimeLineContent.IsInRect(e.Location, rect))
            {
                Capture = true;
                Cursor = Cursors.Hand;
                isIn = true;
                pos = e.Location;
            }
            else 
            if (TimeLineContent.IsInRect(e.Location, rect1))
            {
                if (TimeLineContent.GetInstance()!= null)
                {
                    TimeLineContent.GetInstance().MyMouseDown(e, true);
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isIn)
            {
                long tot = TimeLineContent.GetInstance().TotalTime;
                long t = (long)((e.Location.X-pos.X)/(Width - 8.0)*tot);
                TimeLineContent.GetInstance().MoveForwardScreen(t);
                pos = e.Location;
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isIn = false;
            Capture = false;
            Cursor = Cursors.Default;
            base.OnMouseUp(e);
        }

        public List<Point> SmallLines, BigLines, StringPos;
        public List<string> Strings;
    }
}
