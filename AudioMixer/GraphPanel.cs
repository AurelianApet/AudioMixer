using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace AudioMixer
{
    public partial class GraphPanel : UserControl
    {
        public static float[] scale = { 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000 };
        public static float[] pos = { 0, 70, 110, 150, 220, 260, 300, 370, 410, 450 };
        public static Rectangle rect = new Rectangle(30, 20, 450, 216);
        public BandHandle[] handles;
        private EQProperty eqProperty;
        RectangleF[] tRect;

        public GraphPanel()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            Font = MainForm.GetFont(8f);
        }

        public void Init(EQProperty eq)
        {
            eqProperty = eq;
            BackColor = Color.FromArgb(100, 40, 40, 40);
            Control ctrl = this;
            Rectangle back = new Rectangle(10, 10, ctrl.Width - 20, ctrl.Height - 20);
            tRect = new RectangleF[4];
            tRect[0] = new RectangleF(GetPos(0), rect.Top, (GetPos(2) - GetPos(0)), rect.Height);
            tRect[1] = new RectangleF(GetPos(2), rect.Top, (GetPos(5) - GetPos(2)), rect.Height);
            tRect[2] = new RectangleF(GetPos(5), rect.Top, (GetPos(8) - GetPos(5)), rect.Height);
            tRect[3] = new RectangleF(GetPos(8), rect.Top, (GetPos(9) - GetPos(8)), rect.Height);
            handles = new BandHandle[4];
            for (int i = 0; i < 4; i++)
            {
                handles[i] = new BandHandle();
                handles[i].Init(eqProperty.handleItem[i]);
                handles[i].FactorChanged += (sender, e) => { Invalidate(); };
                handles[i].DBChanged += (sender, e) => { Invalidate(); };
                handles[i].FrequencyChanged += (sender, e) => { Invalidate(); };
                handles[i].MouseDown += HandleClicked;
                Controls.Add(handles[i]);
            }
        }

        public void ActiveChange(BandItem item)
        {
            handles[item.bandHandle.type].Visible = item.IsActive;
            SetPosition(item);
        }
        public void SetPosition(BandItem item)
        {
            handles[item.bandHandle.type].Factor = item.Factor;
            handles[item.bandHandle.type].SetPos(new PointF(GetX(item.Frequency), GetY(item.Desibel)));
            Invalidate();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            tRect = new RectangleF[4];
            tRect[0] = new RectangleF(GetPos(0), rect.Top, (GetPos(2) - GetPos(0)), rect.Height);
            tRect[1] = new RectangleF(GetPos(2), rect.Top, (GetPos(5) - GetPos(2)), rect.Height);
            tRect[2] = new RectangleF(GetPos(5), rect.Top, (GetPos(8) - GetPos(5)), rect.Height);
            tRect[3] = new RectangleF(GetPos(8), rect.Top, (GetPos(9) - GetPos(8)), rect.Height);
        }

        BandHandle dragged = null;
        bool isDragged = false;
        private void HandleClicked(object sender, MouseEventArgs e)
        {
            Capture = true;
            dragged = (BandHandle)sender;
            isDragged = true;
        }

        private void HandleLocationChange(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left && isDragged)
            {
                if (TimeLineContent.IsInRect(e.Location, rect))
                {
                    dragged.SetPos(e.Location);
                    dragged.DB = GetDB(e.Location.Y);
                    dragged.Frequency = GetHz(e.Location.X);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            dragged = null;
            isDragged = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Control ctrl = this;
            Rectangle back = new Rectangle(10, 10, ctrl.Width - 20, ctrl.Height - 20);
            g.FillRectangle(new SolidBrush(Color.Black), back);
            Pen grayPen = new Pen(Color.Gray);
            Pen grayPen1 = new Pen(Color.FromArgb(255, 60, 60, 60));
            Brush[] brush = new Brush[4];
            brush[0] = new LinearGradientBrush(new PointF(GetPos(2)-GetPos(8)+GetPos(5), rect.Bottom), new PointF(GetPos(2), rect.Bottom), Color.Black, Color.FromArgb(255, 30, 30, 30));
            brush[1] = new LinearGradientBrush(new PointF(GetPos(2), rect.Bottom), new PointF(GetPos(5), rect.Bottom), Color.Black, Color.FromArgb(255, 30, 30, 30));
            brush[2] = new LinearGradientBrush(new PointF(GetPos(5), rect.Bottom), new PointF(GetPos(8), rect.Bottom), Color.Black, Color.FromArgb(255, 30, 30, 30));
            brush[3] = new LinearGradientBrush(new PointF(GetPos(8), rect.Bottom), new PointF((GetPos(8) + GetPos(8) - GetPos(5)), rect.Bottom), Color.Black, Color.FromArgb(255, 30, 30, 30));
            for (int i = 0; i < 4; i++) g.FillRectangle(brush[i], tRect[i]);
            g.DrawRectangle(grayPen, rect);
            for (int i = -24; i <= 24; i += 6)
            {
                SizeF s = g.MeasureString(GetSign(i) + Math.Abs(i), this.Font);
                g.DrawString(GetSign(i) + Math.Abs(i), this.Font, new SolidBrush(Color.Gray), 30 - s.Width, 15 + rect.Height / 2 * (1 - i / 24.0f));
                if (i % 12 == 0)
                {
                    g.DrawLine(grayPen1, rect.Left, (rect.Height / 2 * (1 - i / 24.0f)) + rect.Top, rect.Right, (rect.Height / 2 * (1 - i / 24.0f)) + rect.Top);
                }
            }
            for (int i = 0; i < scale.Length; i++)
            {
                string str = scale[i].ToString();
                if (scale[i] % 1000 == 0) str = (scale[i] / 1000) + "K";
                SizeF s = g.MeasureString(str, this.Font);
                g.DrawString(str, this.Font, new SolidBrush(Color.Gray), GetPos(i) - s.Width / 2, rect.Bottom + 20 - s.Height);
                g.DrawLine(grayPen1, GetPos(i), rect.Top, GetPos(i), rect.Bottom);
            }
            PointF[] p = new PointF[rect.Right - rect.Left + 3];
            for (int j = rect.Left; j <= rect.Right; j++)
            {
                float y1 = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (!handles[i].Visible) continue;
                    y1 += GetYPosValue(handles[i].GetPos(), j, handles[i].GetFactor(), i);
                }
                y1 = Math.Min(Math.Max(y1, -rect.Height / 2), rect.Height / 2);
                p[j - rect.Left] = new PointF(j, rect.Top + rect.Height/2 - y1);
            }
            p[rect.Right - rect.Left + 1] = new PointF(rect.Right, rect.Top + rect.Height/2);
            p[rect.Right - rect.Left + 2] = new PointF(rect.Left, rect.Top + rect.Height / 2);
            g.FillPolygon(new SolidBrush(Color.FromArgb(100, Color.Green)), p);
            g.DrawLine(new Pen(Color.FromArgb(100, Color.Green), 2), rect.Left, rect.Top + rect.Height / 2, rect.Right, rect.Top + rect.Height / 2);
            for (int j = rect.Left; j <= rect.Right; j++)
            {
                float y1 = 0;
                y1 = GetYPosValueFromHiCut(j, 0, eqProperty.isHC, eqProperty.highcut);
                y1 += GetYPosValueFromLoCut(j, 0, eqProperty.isLC, eqProperty.lowcut);
                y1 = Math.Min(Math.Max(y1, -rect.Height / 2), rect.Height / 2);
                p[j - rect.Left] = new PointF(j, rect.Top + rect.Height / 2 - y1);
            }
            p[rect.Right - rect.Left + 1] = new PointF(rect.Right, rect.Top + rect.Height / 2);
            p[rect.Right - rect.Left + 2] = new PointF(rect.Left, rect.Top + rect.Height / 2);
            g.FillPolygon(new SolidBrush(Color.FromArgb(100, Color.Green)), p);
            g.DrawLine(new Pen(Color.FromArgb(100, Color.Green), 2), rect.Left, rect.Top + rect.Height / 2, rect.Right, rect.Top + rect.Height / 2);
        }
        public static float GetYPosValueFromHiCut(float x, float y, bool ok, float val)
        {
            if (!ok) return y;
            float pos = GraphPanel.GetX(val);
            if (Math.Abs(pos+15-x)<20)
            {
                return -(x - pos + 5) * (x - pos + 5) / 400f * 27f;
            } else if (x<pos)
            {
                return y;
            } else if (x> pos)
            {
                return -rect.Height/2;
            }
            return y;
        }
        public static float GetYPosValueFromLoCut(float x, float y, bool ok, float val)
        {
            if (!ok) return y;
            float pos = GraphPanel.GetX(val);
            if (Math.Abs(pos - 15 - x) < 20)
            {
                return -(pos + 5 - x) * (pos + 5 - x) / 400f * 27f;
            }
            else if (x > pos)
            {
                return y;
            }
            else if (x < pos)
            {
                return -rect.Height / 2;
            }
            return y;
        }
        public static float GetYPosValue(Point p, float x, float f, int id)
        {
            if (id==0)
            {
                float fac = 50 + 150f * (p.X-rect.Left) / rect.Width;
                if (x < p.X - fac) return (rect.Top + rect.Height / 2 - p.Y);
                return (rect.Top + rect.Height / 2 - p.Y) / (1 + (float)Math.Pow(x - (p.X-fac), 4) * f);
            }
            if (id==3)
            {
                float fac = 50 + 150* (rect.Right-p.X) / rect.Width;
                if (x > p.X + fac) return (rect.Top + rect.Height / 2 - p.Y);
                return (rect.Top + rect.Height / 2 - p.Y) / (1 + (float)Math.Pow(x - (p.X+fac), 4) * f);
            }
            if (id == 4)
            {
                return 0;
            }
            return (rect.Top + rect.Height / 2 - p.Y) / (1 + (float)Math.Pow(x - p.X, 2) * f);
        }
        public static float GetHz(float x)
        {
            for (int i=1; i<pos.Length; i++)
            {
                if (x <= pos[i] + rect.Left + 1e-1) return (x - pos[i-1] - rect.Left) / (pos[i] - pos[i-1]) * (scale[i] - scale[i - 1]) + scale[i-1];
            }
            return 20000;
        }
        public static float GetX(float hz)
        {
            for (int i=1; i<scale.Length; i++)
            {
                if (hz <= scale[i] + 1e-5) return ((hz - scale[i - 1]) * (pos[i] - pos[i - 1]) / (scale[i] - scale[i - 1]) + pos[i - 1] + rect.Left);
            }
            return 0;
        }
        public static float GetDB(int y)
        {
            return (rect.Bottom - y) * 48f / rect.Height - 24f;
        }

        public static float GetY(float db)
        {
            return ((24f - db) / 48f * rect.Height + rect.Top);
        }

        public static float GetPos(int i)
        {
            float t = pos[i] / pos[pos.Length - 1] * rect.Width + rect.Left;
            return t;
        }

        public static string GetSign(int i)
        {
            if (i < 0) return "-";
            if (i > 0) return "+";
            return "";
        }
    }
}