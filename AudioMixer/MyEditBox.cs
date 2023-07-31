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
    public partial class MyEditBox : UserControl
    {
        public EventHandler ValueChanged;
        private string unit = "";
        public string Unit
        {
            get { return unit; }
            set
            {
                unit = value;
                showValue.Text = Value.ToString("0.0") + unit;
            }
        }
        private float minValue = 0;
        private float maxValue = 0;
        public float MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }
        public float MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }
        private float f = 0;
        public float Value
        {
            get { return f; }
            set
            {
                float pre = f;
                f = value;
                if (f < MinValue) f = MinValue;
                if (f > MaxValue) f = MaxValue;
                showValue.Text = f.ToString("0.0") + Unit;
                if (pre != f && ValueChanged != null) ValueChanged(this, new EventArgs());
            }
        }
        public MyEditBox() : this(1.0f, 0f, 12f)
        {
        }

        public MyEditBox(float value, float min, float max)
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            Init(value, min, max);
        }

        public void Init(float v, float min, float max)
        {
            MinValue = min; MaxValue = max; Value = v;
            editValue.Visible = false;
            showValue.Visible = true;
            showValue.ForeColor = Color.White;
            editValue.KeyDown += EditValue_KeyDown;
            editValue.LostFocus += EditValue_LostFocus;
            showValue.MouseDown += ShowValue_MouseDown;
            editValue.Font = MainForm.GetFont(8.25f);
            showValue.Font = MainForm.GetFont(8.25f);
        }

        Point pre;
        DateTime preTime = DateTime.Now;
        private void ShowValue_MouseDown(object sender, MouseEventArgs e)
        {
            if (DateTime.Now.Subtract(preTime).TotalMilliseconds<300)
            {
                editValue.Visible = true;
                editValue.BackColor = Color.FromArgb(255, 30, 30, 30);
                editValue.ForeColor = Color.White;
                editValue.Text = Value.ToString("0.0");
                editValue.Focus();
                editValue.SelectAll();
                showValue.Visible = false;
                preTime = DateTime.Now;
                return;
            }
            preTime = DateTime.Now;
            Control c = (Control)sender;
            c.Focus();
            Capture = true;
            pre = PointToClient(c.PointToScreen(e.Location));
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = ClientRectangle;
            rect.Location += new Size(2, 1);
            rect.Size += new Size(-4, -2);
            ControlPaint.DrawBorder(e.Graphics, rect, Color.FromArgb(255, 80, 80, 80), ButtonBorderStyle.Solid);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (Capture && !TimeLineContent.IsInRect(e.Location, showValue.Bounds))
            {
                Rectangle tmp = Screen.PrimaryScreen.Bounds;
                Point p = PointToScreen(e.Location);
                int t = e.Location.Y - pre.Y;
                if (p.Y == 0)
                {
                    Cursor.Position = new Point(p.X, tmp.Height);
                    t = 0;
                }
                else if (p.Y > tmp.Height-2)
                {
                    Cursor.Position = new Point(p.X, 0);
                    t = 0;
                }
                if (MaxValue == 12)
                {
                    Value = Value + t / 200f * (MaxValue - MinValue);
                } else if (MaxValue == 24)
                {
                    Value = Value + t / 400f * (MaxValue - MinValue);
                } else
                {
                    float x = GraphPanel.GetX(Value) + t * 0.1f;
                    Value = GraphPanel.GetHz(x);
                }
            }
            pre = PointToClient(Cursor.Position);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Capture = false;
        }

        private void EditValue_LostFocus(object sender, EventArgs e)
        {
            UpdateValue();
        }

        private void EditValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Return)
            {
                UpdateValue();
            }
        }

        public void UpdateValue()
        {
            editValue.Visible = false;
            showValue.Visible = true;
            float outValue;
            if (float.TryParse(editValue.Text, out outValue))
            {
                Value = outValue;
            }
        }
    }
}
