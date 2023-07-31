using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AudioMixer
{
    public partial class FaderCtrl : UserControl
    {
        #region Events

        public event EventHandler ValueChanged;

        #endregion

        #region Properties

        public bool isHandled = false;
        private Point preCurPos;
        private string infinity = "  -∞   ";

        private float faderValue = 0.0f;
        private double faderMaxValue = double.MinValue;

        [Description("Set Slider value")]
        [Category("FaderSlider")]
        [DefaultValue(0.0f)]

        public float Value
        {
            get { return faderValue; }
            set
            {
                if (value >= -50f && value <= 5)
                {
                    if (value != faderValue)
                    {
                        MainForm.isChanged = true;
                    }
                    faderValue = value;
                    MaxValue = double.MinValue;
                    if (ValueChanged != null) ValueChanged(this, new EventArgs());
                    Invalidate();
                }
            }
        }

        public double MaxValue
        {
            get { return faderMaxValue; }
            set
            {
                faderMaxValue = value;
            }
        }

        private PointF[] handlePts = new PointF[4];
        private float faderHeight = 0.0f;

        private double blockmax = double.MinValue;
        public double BlockMax
        {
            get { return blockmax; }
            set
            {
                blockmax = value;
                MaxValue = Math.Max(MaxValue, blockmax);
            }
        }

        private double blockavg = double.MinValue;
        public double BlockAvg
        {
            get { return blockavg; }
            set
            {
                blockavg = value;
            }
        }

        #endregion

        public FaderCtrl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.UserMouse | ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint, true);
            InitializeComponent();
            this.editSetValue.LostFocus += EditSetValue_LostFocus;
            this.maxValue.Click += MaxValue_Click;
            Timer tm = new Timer();
            tm.Interval = 15;
            tm.Tick += (sender, e) =>
            {
                Invalidate();
            };
            tm.Start();
            setValue.Font = MainForm.GetFont(7);
            setValue.Height = 20;
            editSetValue.Font = MainForm.GetFont(7);
            editSetValue.Height = 20;
            maxValue.Font = MainForm.GetFont(7);
            maxValue.Height = 20;
            setValue.TextAlign = ContentAlignment.MiddleCenter;
            editSetValue.TextAlign = HorizontalAlignment.Center;
            maxValue.TextAlign = ContentAlignment.MiddleCenter;
        }
        public void Clear()
        {
            lock (leftVals) lock (rightVals)
                {
                    leftVals.Clear();
                    rightVals.Clear();
                    tarLeft = tarRight = -70;
                    Invalidate();
                }
        }
        public void New()
        {
            Clear();
            BlockMax = double.MinValue;
            MaxValue = double.MinValue;
            Value = 0;
        }
        private void MaxValue_Click(object sender, EventArgs e)
        {
            ((Control)sender).Focus();
        }

        private void EditSetValue_LostFocus(object sender, EventArgs e)
        {
            UpdateSetValue();
        }
        public void Ended()
        {
            double lftmx = double.MinValue, rgtmx = double.MinValue;
            while (leftVals.Count > 0) lftmx = Math.Max(lftmx, leftVals.Dequeue());
            while (rightVals.Count > 0) rgtmx = Math.Max(rgtmx, rightVals.Dequeue());
            tarLeft = double.MinValue;
            tarRight = double.MinValue;
        }
        double sgn(double x)
        {
            return x < -1e-5 ? -1 : x > 1e-5 ? x * 10 : 0;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics gra = e.Graphics;
            Pen pen = new Pen(MainForm.borderColor, 2.5f);
            faderHeight = Height - 20;
            float Y = faderHeight / 8 + 10, X = Width - 11;
            
            
            // Draw Fader decibel
            //lock (leftVals) lock(rightVals)
            //    {
            //        if (leftVals.Count>0)
            //        {
                        double lft = tarLeft;// leftVals.Dequeue();
                        double rgt = tarRight;// rightVals.Count == 0 ? lft : rightVals.Dequeue();
                        BlockMax = Math.Max(BlockMax, lft);
                        BlockMax = Math.Max(BlockMax, rgt);
                        lft = sgn(lft - prelftVal) / 20 + prelftVal;
                        rgt = sgn(rgt - prergtVal) / 20 + prergtVal;
                        prelftVal = Math.Max(lft, -70);
                        prergtVal = Math.Max(rgt, -70);
                        RectangleF left = new RectangleF(X, 3, 4.5f, Height-4);
                        RectangleF right = new RectangleF(X + 5.5f, 3, 4.5f, Height-4);
                        gra.FillRectangle(Brushes.Black, left);
                        gra.FillRectangle(Brushes.Black, right);
                        float y = GetHeightFromValue((float)lft);
                        RectangleF aleft = new RectangleF(X, y, 4.5f, Height - y - 1);
                        y = GetHeightFromValue((float)rgt);
                        RectangleF aright = new RectangleF(X+5.5f, y, 4.5f, Height - y - 1);
                        gra.FillRectangle(lft >= 0 ? Brushes.Red : Brushes.GreenYellow, aleft);
                        gra.FillRectangle(rgt >= 0 ? Brushes.Red : Brushes.GreenYellow, aright);
            //    }
            //}
            //if (BlockAvg >= -45f)
            //{
            //    Rectangle rect = new Rectangle();
            //    rect.Width = 10;
            //    rect.X = (int)X;
            //    rect.Height = Height - (int)GetHeightFromValue((float)BlockAvg) - 1;
            //    rect.Y = Height - 1 - rect.Height;
            //    gra.FillRectangle(new SolidBrush(Color.Green), rect);
            //}
            if (MaxValue < -68)
            {
                maxValue.Text = infinity;
            }
            else
            {
                maxValue.Text = MaxValue.ToString("0.00");
            }
            // Draw Fader Rect
            gra.DrawRectangle(pen, X, 4, 10-1, Height-7);

            // Draw level
            gra.DrawLine(pen, X, Y, Width, Y);

            // Draw fader handle
            if (Value <= -50f + 1e-4) setValue.Text = infinity.ToString();
            else setValue.Text = Value.ToString("0.00");

            UpdateFaderHandle(gra, pen);
        }

        Queue<float> leftVals = new Queue<float>();
        Queue<float> rightVals = new Queue<float>();
        double tarLeft = -70;
        double tarRight = -70;
        double prelftVal = -70;
        double prergtVal = -70;
        public void AddValue(double lft, double rgt)
        {
            if (lft < 1e-3) lft = -70;
            else lft = Math.Log10(lft) * 20;
            if (rgt < 1e-3) rgt = -70;
            else rgt = Math.Log10(rgt) * 20;
            double preL = -70;
            if (prelftVal != double.MinValue) preL = prelftVal;
            double preR = -70;
            if (prergtVal != double.MinValue) preR = prergtVal;
            double curL = -70;
            if (lft != double.MinValue) curL = lft;
            double curR = -70;
            if (rgt != double.MinValue) curR = rgt;
            tarLeft = lft;
            tarRight = rgt;
        }

        public void UpdateFaderHandle(Graphics gra, Pen pen)
        {
            float H = 10, W = 10, X = Size.Width - 12, curY = GetHeightFromFaderValue();
            handlePts[0] = new PointF(X, curY);
            handlePts[1] = new PointF(X - W, curY - H);
            handlePts[2] = new PointF(X - W, curY + H);
            handlePts[3] = new PointF(X, curY);
            for (int i = 0; i < 3; i++) gra.DrawLine(pen, handlePts[i], handlePts[i + 1]);
        }

        public float GetHeightFromValue(float value)
        {
            float cur = value;
            float H = faderHeight;
            if (cur > 5f)
            {
                return (100 - cur) / 95 * 10;
            }
            else if (cur >= -20f && cur <= 5f)
            {
                return (5 - cur) / 25 * H * 5 / 8 + 10;
            }
            else if (cur >= -50f && cur <= -20f)
            {
                return (-20f - cur) / 20 * H * 2 / 8 + 10 + H * 5 / 8;
            }
            else if (cur >= -100f)
            {
                return (-50 - cur)/50f * 20 + H + 10;
            }
            else return 20 + H;
        }

        public float GetHeightFromFaderValue()
        {
            if (Value <= -50f + 1e-4) GetHeightFromValue(-50f);
            return GetHeightFromValue(Value);
        }

        public float GetFaderValueFromHeight(float h)
        {
            float H = faderHeight;
            h = h - 10;
            if (h <= H * 5 / 8)
            {
                return 5 - 25 * h / (H * 5 / 8);
            }
            return -20 - 20 * (h - H * 5 / 8) / (H * 2 / 8);
        }

        public static float Multi(PointF a, PointF b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        public static PointF Sub(PointF a, PointF b)
        {
            return new PointF(a.X - b.X, a.Y - b.Y);
        }

        public bool isInHandle(Point pts)
        {
            float area = Math.Abs(Multi(Sub(handlePts[1], handlePts[0]), Sub(handlePts[2], handlePts[0])));
            float area1 = 0;
            for (int i = 0; i < 3; i++) area1 += Math.Abs(Multi(Sub(handlePts[i], pts), Sub(handlePts[i + 1], pts)));
            return Math.Abs(area - area1) < 1e-3;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.Cursor = isInHandle(e.Location) ? Cursors.Hand : Cursors.Default;
            if (isHandled)
            {
                float curY = GetHeightFromFaderValue() + e.Location.Y - preCurPos.Y;
                curY = Math.Max(curY, 10);
                curY = Math.Min(curY, faderHeight + 10);
                if (Value.ToString("0.00") != GetFaderValueFromHeight(curY).ToString("0.00")) Value = GetFaderValueFromHeight(curY);
                preCurPos = e.Location;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            isHandled = isInHandle(e.Location);
            if (isHandled) Capture = true;
            preCurPos = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isHandled = false;
            Capture = false;
        }

        private void maxValue_DoubleClick(object sender, EventArgs e)
        {
            Label a = ((Label)sender);
            a.Text = infinity;
            MaxValue = double.MinValue;
            BlockMax = double.MinValue;
        }

        private void setValue_DoubleClick(object sender, EventArgs e)
        {
            setValue.Visible = false;
            editSetValue.Visible = true;
            editSetValue.Text = Value.ToString("0.00");
            editSetValue.Enabled = true;
            editSetValue.Focus();
            editSetValue.SelectAll();
        }

        public void EditVolume()
        {
            setValue_DoubleClick(this, new EventArgs());
        }

        private void editSetValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                UpdateSetValue();
            }
        }

        private void editSetValue_Leave(object sender, EventArgs e)
        {
            UpdateSetValue();
        }

        public void UpdateSetValue()
        {
            editSetValue.Enabled = false;
            editSetValue.Visible = false;
            setValue.Visible = true;
            try
            {
                float val = float.Parse(editSetValue.Text);
                Value = Math.Max(-50, Math.Min(5, val));
            } catch
            {
            }
        }

        public new void Load(System.IO.BinaryReader bin)
        {
            Value = bin.ReadSingle();
            MaxValue = bin.ReadDouble();
            Invalidate();
        }

        public void Save(System.IO.BinaryWriter bin)
        {
            bin.Write(Value);
            bin.Write(MaxValue);
        }
    }
}
