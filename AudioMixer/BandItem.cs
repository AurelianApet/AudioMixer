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
    public partial class BandItem : UserControl
    {
        public EventHandler DesibelChange;
        public EventHandler FrequencyChange;
        public EventHandler FactorChange;
        public EventHandler ActiveChange;
        public MyEditBox[] myEditValues;
        public EQProperty.BandHandle bandHandle;

        private bool isActive = false;
        public bool IsActive
        {
            get { return isActive; }
            set {
                if (isActive != value)
                {
                    MainForm.isChanged = true;
                }
                isActive = value;
                foreach(MyEditBox t in myEditValues)
                {
                    t.Enabled = value;
                }
                if (isActive)
                {
                    if (bandHandle != null)
                    {
                        bandHandle.SetDefault();
                        bandHandle.enabled = true;
                    }
                }
                else
                {
                    if (bandHandle != null)
                    {
                        bandHandle.SetDefault();
                        bandHandle.enabled = false;
                    }
                }
                if (ActiveChange != null) ActiveChange(this, new EventArgs());
                Invalidate();
            }
        }
        public float Desibel
        {
            get { return bandHandle.db; }
            set
            {
                float pre = bandHandle.db;
                bandHandle.db = value;
                myEditValues[0].Value = value;
                if (pre != value)
                {
                    MainForm.isChanged = true;
                    if (DesibelChange != null) DesibelChange(this, new EventArgs());
                    Invalidate();
                }
            }
        }
        public float Frequency
        {
            get { return bandHandle.frequency; }
            set
            {
                float pre = bandHandle.frequency;
                bandHandle.frequency = value;
                myEditValues[1].Value = value;
                if (pre != value)
                {
                    MainForm.isChanged = true;
                    if (FrequencyChange != null) FrequencyChange(this, new EventArgs());
                    Invalidate();
                }
            }
        }
        public float Factor
        {
            get { return bandHandle.factor; }
            set
            {
                float pre = bandHandle.factor;
                bandHandle.factor = value;
                myEditValues[2].Value = value;
                if (pre != value)
                {
                    MainForm.isChanged = true;
                    if (FactorChange != null) FactorChange(this, new EventArgs());
                    Invalidate();
                }
            }
        }
        public BandItem()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            InitializeComponent();
        }

        public void Init(EQProperty.BandHandle band)
        {
            bandHandle = band;
            SuspendLayout();
            Controls.Clear();
            AutoScaleMode = AutoScaleMode.None;
            Font = MainForm.GetBoldFont(8);
            MyEditBox factor = new MyEditBox(bandHandle.factor, 0, 12);
            MyEditBox frequency = new MyEditBox(bandHandle.frequency, 20, 20000);
            MyEditBox desibel = new MyEditBox(bandHandle.db, -24, 24);
            myEditValues = new MyEditBox[] { desibel, frequency, factor };
            isActive = band.enabled;
            foreach (MyEditBox t in myEditValues)
            {
                t.Enabled = isActive;
            }

            factor.ValueChanged += RealFactorChange;
            frequency.ValueChanged += RealFrequencyChange;
            desibel.ValueChanged += RealDesibelChange;

            factor.Dock = DockStyle.Bottom;
            frequency.Dock = DockStyle.Bottom;
            desibel.Dock = DockStyle.Bottom;

            factor.Unit = "";
            frequency.Unit = "Hz";
            desibel.Unit = "dB";

            Controls.Add(desibel);
            Controls.Add(frequency);
            Controls.Add(factor);
            ResumeLayout();
        }
        private void RealFactorChange(object sender, EventArgs e)
        {
            Factor = ((MyEditBox)sender).Value;
        }
        private void RealFrequencyChange(object sender, EventArgs e)
        {
            Frequency = ((MyEditBox)sender).Value;
        }
        private void RealDesibelChange(object sender, EventArgs e)
        {
            Desibel = ((MyEditBox)sender).Value;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            if (isActive)
            {
                g.FillRectangle(Brushes.Gray, new Rectangle(3, 3, 15, 15));
                g.FillPie(Brushes.Black, new Rectangle(5, 5, 11, 11), -60, 300);
            } else
            {
                g.FillPie(Brushes.Gray, new Rectangle(5, 5, 11, 11), -60, 300);
            }
            string str = "";
            switch (bandHandle.type)
            {
                case 0: str = "1.LO"; break;
                case 1: str = "2.LMF"; break;
                case 2: str = "3.HMF"; break;
                case 3: str = "4.HI"; break;
            }
            g.DrawString(str, Font, Brushes.Wheat, new Point(23, 10 - Font.Height / 2));
            if (isActive)
            {
                Brush brush = new SolidBrush(Color.FromArgb(150, Color.Green));
                g.FillRectangle(brush, new Rectangle(0, 0, Width, 20));
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (TimeLineContent.IsInRect(e.Location, new Rectangle(3, 3, 15, 15)))
            {
                IsActive = !IsActive;
                if (IsActive)
                {
                    bandHandle.SetDefault();
                    bandHandle.enabled = true;
                    Init(bandHandle);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (TimeLineContent.IsInRect(e.Location, new Rectangle(3, 3, 15, 15)))
            {
                Cursor = Cursors.Hand;
            }
            else Cursor = Cursors.Default;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Cursor = Cursors.Default;
        }
    }
}
