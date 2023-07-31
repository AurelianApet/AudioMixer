using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AudioMixer
{
    public partial class ColorShow : UserControl
    {
        #region Events
        public event EventHandler ColorSelectBtnClick;
        public event EventHandler BColorChanged;
        #endregion
        private bool isFolder = false;
        public bool IsFolder
        {
            get { return isFolder; }
            set
            {
                isFolder = value;
                Invalidate();
            }
        }
        private int no = 1;
        public int Number
        {
            get { return no; }
            set
            {
                no = value;
                Invalidate();
            }
        }
        public ColorShow()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.Opaque |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
        }
        Color bcolor = Color.FromArgb(0xff, 0x4d, 0x4d);
        public Color BColor
        {
            get { return bcolor; }
            set
            {
                if (bcolor != value)
                {
                    bcolor = value;
                    BColorChanged?.Invoke(this, new EventArgs());
                    Invalidate();
                }
            }
        }
        private bool isIn = false;
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics gra = e.Graphics;
            System.Drawing.Drawing2D.GraphicsPath path = Video.Controls.ColorSlider.CreateRoundRectPath(new Rectangle(0, 0, Width-1, Height-1), new Size((int)MainForm.F(10), (int)MainForm.F(10)));
            gra.FillPath(new SolidBrush(BColor), path);
            gra.DrawPath(new Pen(MainForm.borderColor, 1f), path);
            Brush brush = new SolidBrush(Color.White);
            Pen pen = new Pen(brush, 1);
            Font font = MainForm.GetThinFont(14);
            SizeF sz = gra.MeasureString(Number < 0 ? "" : Number.ToString(), font);
            gra.DrawString(Number < 0 ? "" : Number.ToString(), font, brush, Width / 2 - sz.Width / 2, Height / 2 - sz.Height / 2);
            if (isIn)
            {
                pen.Color = Color.White;
                pen.Width = 3;
                PointF[] pts = new PointF[3];
                pts[0] = new PointF(7, -2);
                pts[1] = new PointF(-9, -2);
                pts[2] = new PointF(-1, 9);
                gra.TranslateTransform(Width / 2, 10);
                gra.DrawPolygon(pen, pts);
                gra.TranslateTransform(-Width / 2, -10);
            }
            if (IsFolder)
            {
                Bitmap a = new Bitmap(Properties.Resources.folder_icon, new Size(15, 15));
                gra.DrawImage(a, new Point(Width / 2 - a.Width / 2 - 1, Height / 2 - a.Height / 2));
            } else
            {
            }
        }

        private bool isInClickArea(Point p)
        {
            return p.X >= 2 && p.X <= 23 && p.Y >= 8 && p.Y <= 24;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool pre = isIn;
            if (isInClickArea(e.Location))
            {
                this.Cursor = Cursors.Hand;
                isIn = true;
            } else
            {
                isIn = false;
                this.Cursor = Cursors.Default;
            }
            if (pre != isIn) Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (isInClickArea(e.Location))
            {
                ColorSelectBtnClick?.Invoke(this, new EventArgs());
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            bool pre = isIn;
            isIn = false;
            Invalidate();
        }
    }
}
