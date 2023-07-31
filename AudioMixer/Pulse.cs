using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NAudio.Wave;
using System.Threading;

namespace AudioMixer
{
    public partial class Pulse : UserControl
    {
        public static Pulse it;

        public float Volume
        {
            get {
                if (TrackView.GetInstance()!= null)
                {
                    return TrackView.GetInstance().root.eqProperty.Volume * 2;
                }
                return 1.0f;
            }
        }
        private bool isSet = false;
        public int firstBPM = 120;
        public bool SetBPM(int x)
        {
            if (!isSet)
            {
                Invoke((Action)delegate { Ppm = x; });
                isSet = true;
                firstBPM = x;
                return false;
            }
            isSet = true;
            return true;
        }
        private int pulsepermin = 120;

        public int Ppm
        {
            get { return pulsepermin; }
            set
            {
                if (value < 20) value = 20;
                if (value > 999) value = 999;
                if (pulsepermin != value)
                {
                    MainForm.isChanged = true;
                    if (TimeLineContent.GetInstance() != null && TimeLineContent.GetInstance().isPlaying) Play(TimeLineContent.GetInstance().CurPlayTime);
                }
                pulsepermin = value;
                if (TimeLineContent.GetInstance()!= null)
                {
                    TimeLineContent.GetInstance().Invalidate();
                }
                ppm.Text = pulsepermin + "";
            }
        }

        private int pcnt = 4;
        public int Pcnt
        {
            get { return pcnt; }
            set
            {
                if (pcnt != value)
                {
                    MainForm.isChanged = true;
                }
                pcnt = value;
                if (TimeLineContent.GetInstance() != null)
                {
                    TimeLineContent.GetInstance().Beat = value;
                }
                beat.Text = pcnt + "/4";
            }
        }

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
                ppm.Invalidate();
            }
        }

        public Pulse()
        {
            it = this;
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            Init();
        }
        public void Save(BinaryWriter bin)
        {
            bin.Write(IsActive);
            bin.Write(Ppm);
            bin.Write(Pcnt + (firstBPM<<10));
            bin.Write(isSet);
        }
        public new void Load(BinaryReader bin)
        {
            IsActive = bin.ReadBoolean();
            Ppm = bin.ReadInt32();
            firstBPM = bin.ReadInt32();
            Pcnt = firstBPM & 1023;
            firstBPM >>= 10;
            isSet = bin.ReadBoolean();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawPath(new Pen(MainForm.borderColor, 2.5f), Video.Controls.ColorSlider.CreateRoundRectPath(new Rectangle(1, 1, Width-3, Height-3), new Size((int)MainForm.F(10), (int)MainForm.F(10))));
            base.OnPaint(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ppm.Location = new Point(Width / 2 - ppm.Width / 2 - (int)MainForm.F(20), Height / 2 - ppm.Height / 2 - 1);
            editValue.Location = new Point(Width / 2 - editValue.Width / 2 - (int)MainForm.F(23), (int)(Height / 2 - editValue.Height / 2 + (int)MainForm.F(1)));
            beat.Location = new Point(Width * 7 / 9 - beat.Width + (int)MainForm.F(10), Height / 2 - beat.Height / 2 - 1);
            label.Location = new Point((int)MainForm.F(30), Height / 2 - label.Height / 2 - 1);
        }
        private Point pre, ori;
        WaveOut metronome, metronomeup;
        MySampleProvider ws0, ws1;
        public void Init()
        {
            metronome = new WaveOut();
            byte[] tmp = new byte[65 << 10];
            int cnt = Properties.Resources.Metronome.Read(tmp, 0, 65<<10);
            MemoryStream s = new MemoryStream(tmp, 0, cnt);
            WaveFileReader wf = new WaveFileReader(s);
            EQProperty tm0 = new EQProperty();
            WaveStream t0 = WaveFormatConversionStream.CreatePcmStream(wf);
            ws0 = new MySampleProvider(t0.ToSampleProvider(), tm0, null, new MyWaveStream(t0));
            try
            {
                metronome.Init(ws0);
            } catch
            {

            }

            metronomeup = new WaveOut();
            byte[] tmp1 = new byte[65 << 10];
            int cnt1 = Properties.Resources.MetronomeUp.Read(tmp1, 0, 65 << 10);
            MemoryStream s1 = new MemoryStream(tmp1, 0, cnt1);
            WaveFileReader wf1 = new WaveFileReader(s1);
            EQProperty tm1 = new EQProperty();
            WaveStream t1 = WaveFormatConversionStream.CreatePcmStream(wf1);
            ws1 = new MySampleProvider(t1.ToSampleProvider(), tm1, null, new MyWaveStream(t1));
            try
            {
                metronomeup.Init(ws1);
            }
            catch
            {

            }
            HandleDestroyed += (sender, e) =>
            {
                if (th != null) th.Abort();
                metronome.Dispose();
                metronomeup.Dispose();
            };

            IsActive = false;

            ppm.MouseDown += (sender, e) => { Cursor.Hide(); ((Label)sender).Capture = true; ori = pre = Cursor.Position; };
            ppm.MouseHover += (sender, e) => { Cursor = Cursors.Hand; };
            ppm.MouseMove += (sender, e) =>
            {
                if (!((Label)sender).Capture || e.Button != MouseButtons.Left) return;
                Point c = Cursor.Position;
                Rectangle rt = Screen.PrimaryScreen.Bounds;
                int t = pre.Y - c.Y;
                if (c.Y == 0)
                {
                    Cursor.Position = new Point(c.X, rt.Height - 2);
                    t = 0;
                }
                else if (c.Y > rt.Height - 2)
                {
                    Cursor.Position = new Point(c.X, 0);
                    t = 0;
                }
                Ppm = Ppm + (t>0?1:t==0?0:-1);
                pre = Cursor.Position;
            };

            ppm.MouseUp += (sender, e) => { Cursor = Cursors.Default; Cursor.Show(); ((Label)sender).Capture = false; Cursor.Position = ori; };
            ppm.MouseLeave += (sender, e) => { Cursor = Cursors.Default; };
            ppm.MouseClick += (sender, e) => { IsActive = !IsActive; };
            ppm.Paint += DrawBorder1;
            beat.MouseDown += (sender, e) => { Pcnt = 7 - Pcnt; };
            beat.MouseHover += (sender, e) => { Cursor = Cursors.Hand; };
            beat.MouseLeave += (sender, e) => { Cursor = Cursors.Default; };

            editValue.Visible = false;
            ppm.MouseDoubleClick += (sender, e) => { editValue.Text = "";  editValue.Visible = true; editValue.Enabled = true; editValue.Focus(); };
            editValue.LostFocus += (sender, e) =>
            {
                SetPpm();
                editValue.Visible = false;
            };
            editValue.KeyDown += (sender, e) =>
            {
                if (e.KeyData != Keys.Return) return;
                SetPpm();
                editValue.Visible = false;
                editValue.Enabled = false;
                ppm.Focus();
            };
            editValue.KeyUp += (sender, e) =>
            {
                if (editValue.Text.Length > 3)
                {
                    string value = editValue.Text.Substring(0, 3);
                    editValue.Text = "";
                    editValue.Text = value;
                }
            };
            if (Screen.PrimaryScreen.Bounds.Width < 1000)
            {
                label.Font = ppm.Font = beat.Font = MainForm.GetFont(6f);
                editValue.Font = MainForm.GetFont(6f);
            }
            else
            if (Screen.PrimaryScreen.Bounds.Width < 1100)
            {
                label.Font = ppm.Font = beat.Font = MainForm.GetFont(8f);
                editValue.Font = MainForm.GetFont(8f);
            } else
            {
                label.Font = ppm.Font = beat.Font = MainForm.GetFont(10f);
                editValue.Font = MainForm.GetFont(10f);
            }
        }

        public void SetPpm()
        {
            int t;
            if (Int32.TryParse(editValue.Text, out t))
            {
                Ppm = t;
            } else
            {
                t = 0;
                string ch = "０１２３４５６７８９";
                for (int i = 0; i < editValue.Text.Length; i++)
                {
                    if (ch.Contains(editValue.Text[i]))
                    {
                        t = t * 10 + ch.IndexOf(editValue.Text[i]);
                    } else
                    {
                        t = -1; break;
                    }
                }
                if (t != -1) Ppm = t;
            }
        }
        private void DrawBorder1(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Control a = (Control)sender;
            Label b = (Label)sender;
            if (it != null)
            {
                if (it.IsActive)
                {
                    if (Screen.PrimaryScreen.Bounds.Width<1000)
                    {
                        g.FillPath(Brushes.Orange, Video.Controls.ColorSlider.CreateRoundRectPath(new Rectangle(8, 11, a.Width - 19, 9), new Size(3, 3)));
                        SizeF s = g.MeasureString(b.Text, MainForm.GetFont(6f));
                        g.DrawString(b.Text, MainForm.GetFont(6f), new SolidBrush(MainForm.borderColor), (a.Width - s.Width) / 2 - 1, (a.Height - s.Height) / 2 + 1);
                    }
                    else
                    if (Screen.PrimaryScreen.Bounds.Width < 1100)
                    {
                        g.FillPath(Brushes.Orange, Video.Controls.ColorSlider.CreateRoundRectPath(new Rectangle(6, 9, a.Width - 17, 13), new Size(5, 5)));
                        SizeF s = g.MeasureString(b.Text, MainForm.GetFont(8f));
                        g.DrawString(b.Text, MainForm.GetFont(8f), new SolidBrush(MainForm.borderColor), (a.Width - s.Width) / 2 - 1, (a.Height - s.Height) / 2 + 1);
                    }
                    else
                    {
                        g.FillPath(Brushes.Orange, Video.Controls.ColorSlider.CreateRoundRectPath(new Rectangle(2, 7, a.Width - 8, 17), new Size(7, 7)));
                        SizeF s = g.MeasureString(b.Text, MainForm.GetFont(10f));
                        g.DrawString(b.Text, MainForm.GetFont(10f), new SolidBrush(MainForm.borderColor), (a.Width - s.Width) / 2 - 1, (a.Height - s.Height) / 2 + 1);
                    }
                }
            }
//            g.DrawPath(new Pen(MainForm.borderColor, 2.5f), Video.Controls.ColorSlider.CreateRoundRectPath(new Rectangle(1, 1, a.Width - 3, a.Height - 3), new Size(12, 12)));
        }

        Thread th;
        public void Play(long curTime)
        {
            if (th != null)
            {
                th.Abort();
                th = null;
            }
            th = new Thread(PulseStart);
            th.Start(curTime);
        }

        public void Stop()
        {
            if (th != null) th.Abort();
            th = null;
        }

        public void PulseStart(object obj)
        {
            long curTime = (long)obj;
            try
            {
                long nano = (1<<20);
                long tot = (curTime - 1) / nano;
                while (true)
                {
                    tot++;
                    double pm = 60.0 / Ppm;
                    DateTime pre = DateTime.Now;
                    Thread.Sleep((int)((tot * nano - curTime)/(1<<10)*pm));
                    DateTime now = DateTime.Now;
                    curTime += (long)(now.Subtract(pre).TotalMinutes * Ppm * (1 << 20));
                    if (isActive)
                    {
                        if (tot % Pcnt == 0)
                        {
                            ws1.eq.Volume = Volume;
                            ws1.ws.Position = 0;
                            metronomeup.Play();
                        }
                        else
                        {
                            ws0.eq.Volume = Volume;
                            ws0.ws.Position = 0;
                            metronome.Play();
                        }
                    }
                }
            } catch
            {

            }
        }
    }
}
