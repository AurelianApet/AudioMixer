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
    public partial class BandHandle : UserControl
    {
        public EventHandler FactorChanged;
        public EventHandler FrequencyChanged;
        public EventHandler DBChanged;
        public EQProperty.BandHandle bandHandle;
        public int id = 0;
        public static int H = 24;
        public static int W = 12;
        private bool isActive = false;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    MainForm.isChanged = true;
                }
                isActive = value;
                Visible = value;
                bandHandle.SetDefault();
                bandHandle.enabled = value;
                SetPosFromValue();
                Invalidate();
            }
        }
        public float Factor
        {
            get { return bandHandle.factor; }
            set
            {
                float pre = bandHandle.factor;
                if (pre != value)
                {
                    MainForm.isChanged = true;
                }
                bandHandle.factor = value;
                if (pre != bandHandle.factor && FactorChanged!=null) FactorChanged(this, new EventArgs());
            }
        }
        public float Frequency
        {
            get { return bandHandle.frequency; }
            set
            {
                float pre = bandHandle.frequency;
                if (pre != value)
                {
                    MainForm.isChanged = true;
                }
                bandHandle.frequency = value;
                SetPos(new Point((int)GraphPanel.GetX(bandHandle.frequency), Location.Y+H/4*3));
                if (pre != value && FrequencyChanged != null) FrequencyChanged(this, new EventArgs());
            }
        }
        public float DB
        {
            get { return bandHandle.db; }
            set
            {
                float pre = bandHandle.db;
                if (pre !=value)
                {
                    MainForm.isChanged = true;
                }
                bandHandle.db = value;
                SetPos(new Point(Location.X+W/2, (int)GraphPanel.GetY(bandHandle.db)));
                if (bandHandle.db !=pre && DBChanged != null) DBChanged(this, new EventArgs());
            }
        }
        public BandHandle()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            InitializeComponent();
            BackColor = Color.Transparent;
            Size = new Size(W, H);
            Font = MainForm.GetFont(8.25f);
        }
        public void Init(EQProperty.BandHandle item)
        {
            bandHandle = item;
            Visible = item.enabled;
            Factor = item.factor;
            Frequency = item.frequency;
            DB = item.db;
            id = item.type;
            SetPosFromValue();
            Invalidate();
        }
        public void SetPosFromValue()
        {
            SetPos(new PointF(GraphPanel.GetX(Frequency), GraphPanel.GetY(DB)));
        }
        public float GetFactor()
        {
            if (id == 0 || id == 3) return (Factor+0.1f) * 1e-8f;
            return (Factor+0.1f) * 1e-3f;
        }
        public void SetPos(PointF p)
        {
            Location = new Point((int)(p.X - W / 2), (int)(p.Y - H / 4 * 3));
        }
        
        public Point GetPos()
        {
            return new Point(Location.X + W / 2, Location.Y + H / 4 * 3);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Brush brush = new SolidBrush(Color.Gray);
            g.FillEllipse(brush, new RectangleF(2, H/2+2, W-4, H/2-4));
            SizeF s = g.MeasureString(id + 1 + "", Font);
            g.DrawString(id + 1 + "", Font, brush, W / 2 - s.Width / 2, H / 4 - s.Height / 2);
        }
    }
}
