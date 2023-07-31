using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AudioMixer
{
    public partial class ScrollContent : UserControl
    {
        public static ScrollContent instance = null;
        public static ScrollContent GetInstance()
        {
            return instance;
        }

        public ScrollContent()
        {
            instance = this;
            InitializeComponent();
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            BackColor = Color.FromArgb(238, 38, 39, 42);
            hScrollBar.Visible = false;
            UpdateScrollBars();
            mTimeLineContent.Init();
        }

        public new void Load(BinaryReader bin)
        {
            vScrollBar.Maximum = bin.ReadInt32();
            vScrollBar.Value = bin.ReadInt32();
        }
        public void Save(BinaryWriter bin)
        {
            bin.Write(vScrollBar.Maximum);
            bin.Write(vScrollBar.Value);
        }

        protected override void OnResize(EventArgs e)
        {
            this.SuspendLayout();
            hScrollBar.SetBounds(0, Height - hScrollBar.Height, Width, hScrollBar.Height);
            vScrollBar.SetBounds(Width - vScrollBar.Width, 0, vScrollBar.Width, Height);
            mTimeLineContent.UpdateWidth(Width);
            mTimeLineContent.CalcHeight();
            UpdateScrollBars();
            UpdateScrollPosition();
            this.ResumeLayout();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            MouseWheelEvent(e);
        }

        public void SetVScrollValue(int t)
        {
            vScrollBar.Value = t + (t>20*MainForm.TrackHeight?MainForm.TrackHeight:0);
        }

        public void MouseWheelEvent(MouseEventArgs e)
        {
            if (MainForm.GetInstance().IsCtrlKeyPressed)
            {
                if (e.Location.X < 0 || e.Location.X > Width) return;
                mTimeLineContent.UpdateCurUnit(e);
                return;
            }
            if (MainForm.GetInstance().IsShiftKeyPressed)
            {
                mTimeLineContent.MoveForwardScreen(e);
                return;
            }
            vScrollBar.UpdateMouseWheelOnOther(e);
        }

        private void UpdateScrollBars()
        {
            if (mTimeLineContent == null) return;
            this.vScrollBar.Maximum = 1;
            if (mTimeLineContent.Height > ClientSize.Height)
            {
                this.vScrollBar.Maximum = mTimeLineContent.Height;
                this.vScrollBar.LargeChange = 10;
                this.vScrollBar.Visible = true;
            }
            else 
            {
                this.vScrollBar.Visible = false;
            }
        }

        private void UpdateScrollPosition()
        {
            if (mTimeLineContent == null) return;
            if (this.vScrollBar.Maximum != 0)
            {
                Int32 y = (Int32)Math.Round(this.vScrollBar.Value / (Double)this.vScrollBar.Maximum * (this.mTimeLineContent.Height - ClientSize.Height));
                if (y >= 0)
                {
                    this.mTimeLineContent.Top = -y;
                    if (TrackView.GetInstance()!= null)
                    {
                        TrackView.GetInstance().Top = -y;
                    }
                }
            }
        }

        private void TimeLineContent_Resize(object sender, EventArgs e)
        {
            Double vpct = 0;
            if (this.vScrollBar.Maximum > 0)
                vpct = this.vScrollBar.Value / (Double)this.vScrollBar.Maximum;

            UpdateScrollBars();

            this.vScrollBar.Value = (Int32)Math.Round(vpct * this.vScrollBar.Maximum);

            UpdateScrollPosition();
        }

        private void vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateScrollPosition();
        }

        public void ParentSizeChanged()
        {
            Size = Parent.Size - new Size(0, 50);
        }

        public void AudioTrackAddOrDelete()
        {
            if (mTimeLineContent!=null) mTimeLineContent.CalcHeight();
        }

        public void AddWaveForm(string path)
        {
            mTimeLineContent.MakeWaveForm(path);
        }
    }
}
