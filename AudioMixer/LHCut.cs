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
    public partial class LHCut : UserControl
    {
        public EventHandler ValueChanged;
        EQProperty property;
        bool isHi=false, isLo=false;
        float hi=20000, lo=20; 
        public bool IsHC
        {
            get { return isHi; }
            set
            {
                bool pre = isHi;
                if (pre != value)
                {
                    MainForm.isChanged = true;
                }
                isHi = value;
                property.isHC = value;
                if (pre != value && ValueChanged != null) ValueChanged(this, new EventArgs());
                Invalidate();
            }
        }
        public bool IsLC
        {
            get { return isLo; }
            set
            {
                bool pre = isLo;
                if (pre != value)
                {
                    MainForm.isChanged = true;
                }
                isLo = value;
                property.isLC = value;
                if (pre != value && ValueChanged != null) ValueChanged(this, new EventArgs());
                Invalidate();
            }
        }
        public float hightcut
        {
            get { return hi; }
            set
            {
                float pre = hi;
                hi = value;
                if (hi < 20) hi = 20;
                if (hi > 20000) hi = 20000;
                if (pre != hi)
                {
                    MainForm.isChanged = true;
                }
                property.highcut = value;
                if (pre != value && ValueChanged != null) ValueChanged(this, new EventArgs());
                Invalidate();
            }
        }
        public float lowcut
        {
            get { return lo; }
            set
            {
                float pre = lo;
                lo = value;
                if (lo < 20) lo = 20;
                if (lo > 20000) lo = 20000;
                if (pre != lo)
                {
                    MainForm.isChanged = true;
                }
                property.lowcut = value;
                if (pre != value && ValueChanged != null) ValueChanged(this, new EventArgs());
                Invalidate();
            }
        }
        public LHCut()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            BorderStyle = BorderStyle.FixedSingle;
            Font = MainForm.GetFont(8f);
        }

        public void Init(EQProperty property)
        {
            this.property = property;
            isHi = property.isHC;
            isLo = property.isLC;
            hi = property.highcut;
            lo = property.lowcut;
        }

        Rectangle hrect = new Rectangle(1, 22, 76, 20);
        Rectangle lrect = new Rectangle(1, 44, 76, 20);
        Rectangle hrectSetting = new Rectangle(2, 23, 18, 18);
        Rectangle lrectSetting = new Rectangle(2, 45, 18, 18);
        Rectangle hrectSettingIn = new Rectangle(4, 25, 14, 14);
        Rectangle lrectSettingIn = new Rectangle(4, 47, 14, 14);
        Point pre;
        bool isOnHC = false;
        bool isOnLC = false;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                if (TimeLineContent.IsInRect(e.Location, hrectSetting))
                {
                    IsHC = !IsHC;
                }
                else if (TimeLineContent.IsInRect(e.Location, lrectSetting))
                {
                    IsLC = !IsLC;
                } else if (TimeLineContent.IsInRect(e.Location, hrect) && IsHC)
                {
                    Capture = true;
                    pre = e.Location;
                    isOnHC = true;
                } else if (TimeLineContent.IsInRect(e.Location, lrect) && IsLC)
                {
                    Capture = true;
                    pre = e.Location;
                    isOnLC = true;
                }
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isOnHC = false;
            isOnLC = false;
            Capture = false;
            state = 0;
            Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.None)
            {
                if (TimeLineContent.IsInRect(e.Location, hrect))
                {
                    Cursor = Cursors.Hand;
                    state = 1;
                }
                else if (TimeLineContent.IsInRect(e.Location, lrect))
                {
                    Cursor = Cursors.Hand;
                    state = 2;
                }
                else
                {
                    state = 0;
                    Cursor = Cursors.Default;
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (isOnHC)
                {
                    int t = e.Location.Y - pre.Y;
                    hightcut = GraphPanel.GetHz(GraphPanel.GetX(hightcut) + t);
                } else if (isOnLC)
                {
                    int t = e.Location.Y - pre.Y;
                    lowcut = GraphPanel.GetHz(GraphPanel.GetX(lowcut) + t);
                }
            }
            pre = PointToClient(Cursor.Position);
            Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            state = 0;
            Cursor = Cursors.Default;
            Invalidate();
        }
        // 0 - none 1 - hrect 2 - lrect
        int state = 0;
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.FillPie(Brushes.Gray, new Rectangle(5, 5, 11, 11), -60, 300);
            string str = "PRE";
            g.DrawString(str, Font, Brushes.Wheat, new Point(23, 10 - Font.Height / 2));
            Brush brush = new SolidBrush(Color.FromArgb(150, Color.Green));
            g.FillRectangle(brush, new Rectangle(0, 0, Width, 20));
            ControlPaint.DrawBorder(g, hrect, Color.FromArgb(255, 100, 100, 100), ButtonBorderStyle.Solid);
            ControlPaint.DrawBorder(g, lrect, Color.FromArgb(255, 100, 100, 100), ButtonBorderStyle.Solid);
            if (state==1)
            {
                if (isHi)
                {
                    g.FillRectangle(Brushes.Gray, hrectSetting);
                    g.FillPie(Brushes.Black, hrectSettingIn, -60, 300);
                } else {
                    g.FillPie(Brushes.Gray, hrectSettingIn, -60, 300);
                }
                g.DrawString(hi.ToString("0.0") + "Hz", Font, Brushes.Gray, 20, (hrect.Top + hrect.Bottom - Font.Height) * 0.5f);
            } else
            {
                g.DrawString("HC", Font, Brushes.Gray, 2, (hrect.Top + hrect.Bottom - Font.Height) * 0.5f);
            }
            if (isHi)
            {
                if (hightcut<20000)
                {
                    float x = (GraphPanel.GetX(hightcut) - GraphPanel.GetX(20)) / (GraphPanel.GetX(20000) - GraphPanel.GetX(20)) * Width;
                    g.FillRectangle(isOnHC? new SolidBrush(Color.FromArgb(100, Color.Green)) : new SolidBrush(Color.FromArgb(100, Color.Gray)), x, hrect.Top, Width - x, hrect.Height);
                }
                g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Green)), 0, hrect.Top, Width, hrect.Height);
            }
            
            if (state == 2)
            {
                if (isLo)
                {
                    g.FillRectangle(Brushes.Gray, lrectSetting);
                    g.FillPie(Brushes.Black, lrectSettingIn, -60, 300);
                }
                else
                {
                    g.FillPie(Brushes.Gray, lrectSettingIn, -60, 300);
                }
                g.DrawString(lo.ToString("0.0") + "Hz", Font, Brushes.Gray, 20, (lrect.Top + lrect.Bottom - Font.Height) * 0.5f);
            }
            else
            {
                g.DrawString("LC", Font, Brushes.Gray, 2, (lrect.Top + lrect.Bottom - Font.Height) * 0.5f);
            }
            if (isLo)
            {
                if (lowcut > 0)
                {
                    float x = (GraphPanel.GetX(lowcut) - GraphPanel.GetX(20)) / (GraphPanel.GetX(20000) - GraphPanel.GetX(20)) * Width;
                    g.FillRectangle(isOnLC ? new SolidBrush(Color.FromArgb(100, Color.Green)) : new SolidBrush(Color.FromArgb(100, Color.Gray)), 0, lrect.Top, x, lrect.Height);
                }
                g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Green)), 0, lrect.Top, Width, lrect.Height);
            }
        }
    }
}
