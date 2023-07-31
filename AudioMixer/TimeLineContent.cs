using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using NAudio.Wave;
using System.Reflection;
using System.Threading;

namespace AudioMixer
{
    public partial class TimeLineContent : UserControl
    {
        public MyWaveOut audioPlayer;
        public MyMixingSampleProvider mixer = null;
        public List<MySampleProvider> samples;
        public static TimeLineContent instance = null;
        public Queue<WaveStruct> queue;
        public static TimeLineContent GetInstance()
        {
            return instance;
        }
        
        public List<WaveForm> selectedWaveForms = new List<WaveForm>();
        List<WaveForm> tempwaves = new List<WaveForm>();
        #region Constants
        //        private Int64[] units = new Int64[] { 3600000, 1800000, 1200000, 900000, 600000, 300000, 120000, 60000, 60000, 30000, 15000, 10000, 10000, 5000, 5000, 2000, 2000, 1000, 1000, 500, 500, 200, 200, 100, 50, 50, 50, 20, 10, 10, 5, 5, 2, 2, 1, 1, 1, 1 };

        private Int64[] units = new Int64[] { 1<<20, 1<<20, 1<<20, 1<<19, 1<<19, 1<<19, 1<<18, 1<<18, 1<<18, 1<<18, 131072, 131072, 131072, 65536, 65536, 65536, 32768, 32768, 32768, 16384, 16384, 8192, 8192, 8192, 4096, 4096, 4096, 2048, 2048, 2048, 1024, 1024, 1024, 512, 512, 512, 256, 256, 256, 128, 128, 128, 64, 64, 64, 32, 32, 32, 16, 16, 16, 8, 8, 8, 4, 4, 4, 2, 2, 2, 1, 1};
        #endregion

        #region Events

        #endregion

        #region Properties
        private int beat = 4;
        public int Beat
        {
            get { return beat; }
            set
            {
                beat = value;
                Invalidate();
                UpdateHeader();
            }
        }
        public int BPM
        {
            get
            {
                if (Pulse.it != null) return Pulse.it.Ppm;
                return 120;
            }
        }
        public Int64 CurUnit
        {
            get {
                long a = CurSmallUnit;
                if (a >= (1 << 20)) return a * Beat;
                return a * 4;
            }
        }

        public Int64 CurSmallUnit
        {
            get
            {
                Int64 x = units[0] * 1024;
                for (int i = 1; i < units.Length; i++)
                {
                    if (x * 20 <= RTime - LTime)
                    {
                        break;
                    }
                    x = units[i] * 1024;
                }
                return x;
            }
        }
        private Int64 totTime = 2 * 120 * (1<<20); // nanobeat
        public Int64 TotalTime
        {
            get { return totTime; }
            set
            {
                Int64 pre = totTime;
                totTime = value;
                if (pre != totTime)
                {
                    Invalidate();
                    UpdateHeader();
                }
            }
        }

        private Int64 lTime = 0;
        public Int64 LTime
        {
            get { return lTime; }
            set { lTime = value; }
        }

        private Int64 rTime = 2 * 120 * (1<<20); // nanosecond
        public Int64 RTime
        {
            get { return rTime; }
            set { rTime = value; }
        }

        public int CurTrackHeight
        {
            get { return MainForm.TrackHeight; }
        }

        public int CurShowTracks
        {
            get
            {
                TrackView mTrackView = TrackView.GetInstance();
                if (mTrackView == null) return 0;
                return mTrackView.Controls.Count;
            }
        }

        public Double CurTimeUnitPerPixel
        {
            get { return (RTime - LTime) / (Double)this.Width; }
        }

        public Double CurPixelPerTimeUnit
        {
            get { return (Double)this.Width / (RTime - LTime); }
        }

        private Int64 curPlayTime = 0;
        public Int64 CurPlayTime
        {
            get { return curPlayTime; }
            set
            {
                curPlayTime = value;
            }
        }

        private Int64 curPlayStartTime = 0;
        public Int64 CurPlayStartTime
        {
            get { return curPlayStartTime; }
            set
            {
                curPlayStartTime = value;
            }
        }

        private MyPanel mPlayTimeLine, mPlayStartTimeLine;

        private int offsetY = 0;
        public int OffsetY
        {
            get { return offsetY; }
            set
            {
                int preY = offsetY;
                offsetY = value;
                if (preY != offsetY)
                {
                    Invalidate();
                    UpdateHeader();
                }
            }
        }

        #endregion

        System.Windows.Forms.Timer tm = new System.Windows.Forms.Timer();
        private int TimeInterval = 10;

        private bool waveFormDragged = false;
        private WaveForm dragged = null;
        private long dragOffsetTime = 0;

        public TimeLineContent()
        {
            instance = this;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint, true);
            InitializeComponent();
            Init();
        }
        public void WaveFormAdd(WaveForm wf)
        {
            lock(tempwaves)
            {
                if (tempwaves.Contains(wf)) return;
                if (Controls.Contains(wf)) return;
                tempwaves.Add(wf);
            }
        }
        public void WaveFormRemove(WaveForm wf)
        {
            lock(tempwaves)
            {
                tempwaves.Remove(wf);
                Controls.Remove(wf);
            }
        }
        //Queue<>
        public void Init()
        {
            Controls.Clear();
            samples = new List<MySampleProvider>();
            BackColor = Color.FromArgb(0xff, 0x26, 0x27, 0x2a);
            mPlayTimeLine = new MyPanel();
            mPlayTimeLine.Padding = new Padding(0);
            mPlayTimeLine.BackColor = Color.White;
            mPlayTimeLine.Size = new Size(2, Height);
            Controls.Add(mPlayTimeLine);
            Controls.SetChildIndex(mPlayTimeLine, 0);

            mPlayStartTimeLine = new MyPanel();
            mPlayStartTimeLine.Padding = new Padding(0);
            mPlayStartTimeLine.BackColor = Color.OrangeRed;
            mPlayStartTimeLine.Size = new Size(2, Height);
            Controls.Add(mPlayStartTimeLine);
            Controls.SetChildIndex(mPlayStartTimeLine, 1);

            tm.Interval = TimeInterval;
            tm.Tick += TimeTick;

            System.Windows.Forms.Timer detector = new System.Windows.Forms.Timer();
            detector.Interval = 10;
            detector.Tick += (sender, e) => {
                lock(tempwaves)
                {
                    if (tempwaves.Count>0)
                    {
                        for (int i = 0; i < tempwaves.Count; i++)
                        {
                            WaveForm wf = tempwaves[i];
                            if (!wf.isBusy)
                            {
                                Controls.Add(wf);
                                tempwaves.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            };
            detector.Start();

            audioPlayer = new MyWaveOut();
            if (TrackView.GetInstance()!= null)
            {
                makeMixer();
            }
            audioPlayer.PlaybackStopped += (sender, e) =>
            {
                if (MasterTrack.GetInstance() != null)
                {
                    MasterTrack.GetInstance().Ended();
                }
                if (TrackView.GetInstance()!= null)
                {
                    foreach (AudioTrack a in TrackView.GetInstance().root.GetAllChild(true))
                    {
                        a.ClearDb();
                    }
                }
            };

            queue = new Queue<WaveStruct>();
            isLoading = false;
            System.Windows.Forms.Timer makeTimer = new System.Windows.Forms.Timer();
            makeTimer.Tick += (sender, e) =>
            {
                if (isLoading) return;
                lock(queue)
                {
                    if (queue.Count > 0)
                    {
                        isLoading = true;
                        WaveStruct now = queue.Dequeue();
                        long t = now.startTime;
                        AudioTrack tmp = now.track;
                        string path1 = now.path;
                        WaveForm wf = new WaveForm(t, tmp, path1);
                        if (wf.notSupported)
                        {
                            wf.Dispose();
                            isLoading = false;
                            return;
                        }
                        wf.ReadFile(path1);
                        isLoading = false;
                    }
                }
            };
            makeTimer.Interval = 100;
            makeTimer.Start();
        }
        public bool isLoading;
        public new void Load(BinaryReader bin)
        {
            List<Control> tmp = new List<Control>();
            foreach(Control a in Controls)
            {
                if (a.GetType() == typeof(WaveForm))
                {
                    tmp.Add(a);
                }
            }
            foreach (Control a in tmp) Controls.Remove(a);
            TotalTime = bin.ReadInt64();
            LTime = bin.ReadInt64();
            RTime = bin.ReadInt64();
            CurPlayStartTime = bin.ReadInt64();
            CurPlayTime = bin.ReadInt64();
            Invalidate();
            UpdateHeader();
        }
        public void Save(BinaryWriter bin)
        {
            bin.Write(TotalTime);
            bin.Write(LTime);
            bin.Write(RTime);
            bin.Write(CurPlayStartTime);
            bin.Write(CurPlayTime);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            Pen bigPen = new Pen(Color.FromArgb(0xff, 0x7f, 0x7f, 0x7f));
            Pen smallPen = new Pen(Color.FromArgb(0xff, 0x58, 0x58, 0x58));
            Int64 curUnit = CurUnit, st = LTime / curUnit * curUnit, ed = RTime / curUnit * curUnit;
            Int64 curSmallUnit = CurSmallUnit;
            for (Int64 x = st; x <= ed; x += curUnit)
            {
                for (Int64 tx = x + curSmallUnit; tx < x + curUnit; tx += curSmallUnit)
                {
                    int X = (int)(CurPixelPerTimeUnit * (tx - LTime));
                    gr.DrawLine(smallPen, X, 0, X, Height);
                }
                if (x >= LTime && x <= RTime)
                {
                    int X = (int)(CurPixelPerTimeUnit * (x - LTime));
                    gr.DrawLine(bigPen, X, 0, X, Height);
                }
            }
            int h = MainForm.TrackHeight;
            for (int y = 0; y <= CurShowTracks; y++)
            {
                int yy = y * h + offsetY;
                gr.DrawLine(smallPen, 0, yy, Width, yy);
            }
            UpdateTimeLines();
        }

        public void UpdateHeader()
        {
            if (TimeLineHeader.GetInstance() != null)
            {
                List<Point> sm = new List<Point>();
                List<Point> bi = new List<Point>();
                List<string> strs = new List<string>();
                List<Point> strPos = new List<Point>();
                Int64 curUnit = CurUnit, st = LTime / curUnit * curUnit, ed = RTime / curUnit * curUnit;
                Int64 curSmallUnit = CurSmallUnit;
                for (Int64 x = st; x <= ed; x += curUnit)
                {
                    for (Int64 tx = x + curSmallUnit; tx < x + curUnit; tx += curSmallUnit)
                    {
                        int X = (int)(CurPixelPerTimeUnit * (tx - LTime));
                        sm.Add(new Point(X, 0));
                        sm.Add(new Point(X, Height));
                    }
                    if (x >= LTime && x <= RTime)
                    {
                        int X = (int)(CurPixelPerTimeUnit * (x - LTime));
                        bi.Add(new Point(X, 0));
                        bi.Add(new Point(X, Height));
                        strs.Add(GetStringFromNanoTime(x));
                        strPos.Add(new Point(X, 36 - Font.Height));
                    }
                }
                TimeLineHeader t = TimeLineHeader.GetInstance();
                t.SmallLines = sm;
                t.BigLines = bi;
                t.StringPos = strPos;
                t.Strings = strs;
                t.Invalidate();
            }
        }

        public void UpdateTimeLines()
        {
            if (CurPlayTime >= LTime && CurPlayTime <= RTime)
            {
                int pos = GetTimePos(CurPlayTime);
                if (mPlayTimeLine.Left != pos || !mPlayTimeLine.Visible)
                {
                    mPlayTimeLine.Left = pos;
                    mPlayTimeLine.Visible = true;
                }
            }
            else mPlayTimeLine.Visible = false;
            mPlayTimeLine.Height = Height;
            if (CurPlayStartTime >= LTime && CurPlayStartTime <= RTime)
            {
                int pos = GetTimePos(CurPlayStartTime);
                if (mPlayStartTimeLine.Left != pos || !mPlayStartTimeLine.Visible)
                {
                    mPlayStartTimeLine.Left = pos;
                    mPlayStartTimeLine.Visible = true;
                }
            }
            else mPlayStartTimeLine.Visible = false;
            mPlayStartTimeLine.Height = Height;
        }

        public string GetStringFromNanoTime(Int64 x)
        {
            Int64 min = x / 1024 / 1024 / beat;
            Int64 sec = (x - min * (1<<20) * beat) / (1<<20);
            Int64 mil = (x - min * (1<<20) * beat - sec * (1<<20));
            if (mil % (1 << 18) != 0) return "";
            mil /= (1 << 18);
            string ans = min + 1 + "";
            if (mil == 0 && sec != 0) ans += "." + (sec+1);
            if (mil != 0) ans += "."+(sec+1)+"." + (mil + 1);
            return ans;
        }

        public void UpdateCurUnit(MouseEventArgs e)
        {
            Int64 curPosTime = LTime + (Int64)(CurTimeUnitPerPixel * e.X);
            Int64 preLTime = LTime, preRTime = RTime;
            if (e.Delta<0)
            {
                Int64 tLTime = (LTime * 4 - curPosTime) / 3;
                Int64 tRTime = (RTime * 4 - curPosTime) / 3;
                RTime = Math.Min(TotalTime, tRTime);
                LTime = Math.Max(0, tLTime);
            } else
            {
                Int64 tLTime = (curPosTime + LTime * 3) / 4;
                Int64 tRTime = (curPosTime + RTime * 3) / 4;
                if (tRTime - tLTime > 2000)
                {
                    LTime = tLTime;
                    RTime = tRTime;
                }
            }
            if (preLTime != LTime || preRTime != RTime)
            {
                Invalidate();
                UpdateHeader();
                RefreshWaveForms();
            }
        }

        public void MoveForwardScreen(MouseEventArgs e)
        {
            Int64 delta = e.Delta<0?Math.Min(CurUnit*2/3, LTime):Math.Min(TotalTime, RTime+CurUnit*2/3)-RTime;
            Int64 preLTime = LTime, preRTime = RTime;
            LTime += delta * (e.Delta / 120);
            RTime += delta * (e.Delta / 120);
            if (preLTime != LTime || preRTime != RTime)
            {
                Invalidate();
                UpdateHeader();
                RefreshWaveForms();
            }
        }

        public void MoveForwardScreen(long t)
        {
            Int64 preLTime = LTime, preRTime = RTime;
            if (LTime+t<0)
            {
                t = -LTime;
            }
            if (RTime + t>TotalTime)
            {
                t = TotalTime - RTime;
            }
            LTime += t; RTime += t;
            if (preLTime != LTime || preRTime != RTime)
            {
                Invalidate();
                UpdateHeader();
                RefreshWaveForms();
            }
        }

        public void CalcHeight()
        {
            if (Parent != null) Height = Math.Max(Parent.Height, CurTrackHeight * (CurShowTracks) + 50);
            else Height = CurTrackHeight * (CurShowTracks) + 50;
            Invalidate();
            UpdateHeader();
            RefreshWaveForms();
        }

        public void RefreshWaveForms()
        {
            foreach (Control ctrl in Controls)
            {
                if (ctrl.GetType() != typeof(WaveForm)) continue;
                WaveForm a = (WaveForm)ctrl;
                a.Redraw();
            }
        }

        public void UpdateWidth(int w)
        {
            if (w == 0) return;
            Int64 tRTime = (Int64)(CurTimeUnitPerPixel * w + LTime);
            RTime = Math.Min(tRTime, TotalTime);
            Width = w;
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            // If file is dragged, show cursor "Drop allowed"
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else if (e.Data.GetDataPresent("AudioMixer.WaveForm"))
            {
                e.Effect = DragDropEffects.Move;
            } else
                e.Effect = DragDropEffects.None;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            Point pos = new Point(e.X, e.Y);
            pos = this.PointToClient(pos);
            AudioTrack tmp = GetPosAudioTrack(pos.Y);
            if (tmp == TrackView.GetInstance().root || tmp != null && tmp.IsFolder())
            {
                e.Effect = DragDropEffects.None;
            } else
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
        protected override void OnDragDrop(DragEventArgs e)
        {
            Cursor = Cursors.Default;
            if (e.Data.GetDataPresent("AudioMixer.WaveForm"))
            {
                //MainForm.GetInstance().Text = ((WaveForm)e.Data.GetData("AudioMixer.WaveForm")).Name;
                return;
            }
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length>0)
            {
                foreach(string str in files)
                {
                    string tp = MyWaveFile.GetType(str);
                    if (tp != "mp3" && tp != "wav") return;
                }
                MakeWaveForm(files, new Point(e.X, e.Y));
            }
        }

        public void New()
        {
            samples.Clear();
            if (audioPlayer!=null) audioPlayer.Dispose();
            Init();
        }
        string GetFileName(string path)
        {
            int t = path.LastIndexOf('/');
            if (t == -1) t = path.LastIndexOf('\\');
            int e = path.LastIndexOf('.');
            return path.Substring(t + 1, e - t - 1);
        }
        public void MakeWaveForm(string[] path, Point pos)
        {
            Thread th = new Thread(mkcf);
            th.Start(new object[] { path, this.PointToClient(pos) });
        }
        void mkcf(object obj) {
            object[] a = (object[])obj;
            Point pos = (Point)a[1];
            string[] path = (string[])a[0];
            AudioTrack tmp = GetPosAudioTrack(pos.Y);
            List<Object> waveforms = new List<object>();
            bool added = false;
            for (int i = 0; i < path.Length; i++)
            {
                if (tmp == null)
                {
                    MainForm.GetInstance().ShowProgressBar(true, 0);
                    int t = TrackView.GetInstance().Controls.Count;
                    MainForm.GetInstance().Invoke((Action)delegate { TrackView.GetInstance().AddTrack(false, true, true, false); });
                    added = true;
                    try
                    {
                        Thread.Sleep(1);
                    } catch
                    {

                    }
                    tmp = (AudioTrack)TrackView.GetInstance().temparory[TrackView.GetInstance().temparory.Count-1];
                    tmp.SetName(GetFileName(path[i]));
                    Thread.Sleep(1);
                }
                if (tmp != TrackView.GetInstance().root && !tmp.IsFolder())
                {
                    waveforms.Add(GetPosTime(pos.X));
                    waveforms.Add(tmp);
                    waveforms.Add(path[i]);
                }
                else break;
                Control tt = TrackView.GetInstance().GetNextControl1(tmp);
                if (tt == null) tmp = null;
                else tmp = (AudioTrack)tt;
            }
            if (added)
            {
                MainForm.GetInstance().Invoke((Action)delegate { TrackView.GetInstance().RefreshAll(); });
            }
            for (int i = 0; i < waveforms.Count; i+=3)
            {
                MakeWaveForm((long)waveforms[i], (AudioTrack)waveforms[i + 1], (string)waveforms[i + 2]);
            }
        }
        public void ShowWaveForm(WaveForm wf, bool vis)
        {
            if (InvokeRequired)
            {
                Invoke((Action)delegate { wf.Visible = vis; });
            } else
            {
                wf.Visible = vis;
            }
        }
        public void PlaceWaveForm(WaveForm wf, Point curPos)
        {
            if (InvokeRequired)
            {
                Invoke((Action)delegate { wf.Location = curPos; });
            }
            else
            {
                wf.Location = curPos;
            }
        }
        public struct WaveStruct
        {
            public long startTime;
            public AudioTrack track;
            public string path;
        }
        public void MakeWaveForm(long t, AudioTrack tmp, string path1, bool flag = false)
        {
//            Thread makeWaveFormThread = new Thread(mkwf);
////            makeWaveFormThread.Start(new object[] { t, tmp, path, flag });
//        }
//        void mkwf(object obj) {
//            object[] a = (object[])obj;
//            long t = (long)a[0];
//            AudioTrack tmp = (AudioTrack)a[1];
//            string path = (string)a[2];
//            bool flag = (bool)a[3];
            if (flag) tmp.SetName(GetFileName(path1));
            MainForm.isChanged = true;
            WaveStruct tp = new WaveStruct { startTime = t, track = tmp, path = path1 };
            lock(queue)
            {
                queue.Enqueue(tp);
            }
        }

        public void MakeWaveForm(string path)
        {
            MakeWaveForm((LTime+RTime)/2/CurSmallUnit*CurSmallUnit, GetPosAudioTrack(CurTrackHeight*(TrackView.GetInstance().Controls.Count+1), true), path, true);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            MyMouseDown(e);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            try {
                Point p = Parent.PointToClient(PointToScreen(e.Location));
                if (waveFormDragged && dragged != null && IsInRect(p, Parent.ClientRectangle))
                {
                    long s = CurSmallUnit;
                    if (IsMouseMoved && GetPosAudioTrack(e.Location.Y) != null && GetPosAudioTrack(e.Location.Y) != TrackView.GetInstance().root && !GetPosAudioTrack(e.Location.Y).IsFolder())
                    {
                        if (ModifierKeys == Keys.Alt)
                        {
                            WaveForm a = dragged.copy();
                            Controls.Add(a);
                            a.StartTime = Math.Max(0, (GetPosTime(e.Location.X) - dragOffsetTime + s / 2) / s * s);
                            TotalTime = Math.Max(TotalTime, a.StartTime + a.duration);
                            a.trackCtrl.RemoveWaveForm(a);
                            a.trackCtrl = GetPosAudioTrack(e.Location.Y);
                            a.SetEQ(a.trackCtrl.eqProperty);
                            a.trackCtrl.AddWaveForm(a);
                            a.Redraw();
                        }
                        else
                        {
                            dragged.StartTime = Math.Max(0, (GetPosTime(e.Location.X) - dragOffsetTime + s / 2) / s * s);
                            TotalTime = Math.Max(TotalTime, dragged.StartTime + dragged.duration);
                            dragged.trackCtrl.RemoveWaveForm(dragged);
                            dragged.trackCtrl.SetDb(double.MinValue, double.MinValue);
                            dragged.trackCtrl = GetPosAudioTrack(e.Location.Y);
                            dragged.SetEQ(dragged.trackCtrl.eqProperty);
                            dragged.trackCtrl.AddWaveForm(dragged);
                            dragged.Redraw();
                        }
                        if (CurPlayTime >= dragged.StartTime && CurPlayTime < dragged.StartTime + dragged.duration)
                        {
                            dragged.m_sprovider.ws.SetPosition(CurPlayTime - dragged.StartTime, dragged.duration, dragged.m_Wavefile.os.TotalTime.TotalSeconds);
                        }
                        else
                        {
                            mixer?.RemoveMixerInput(dragged.m_sprovider);
                        }
                    }
                    if (ModifierKeys == Keys.Control)
                    {
                        if (selectedWaveForms.Contains(dragged))
                        {
                            dragged.UnSelected();
                            selectedWaveForms.Remove(dragged);
                        } else
                        {
                            dragged.Selected();
                            selectedWaveForms.Add(dragged);
                        }
                    } else {
                        if (selectedWaveForms.Contains(dragged) && selectedWaveForms.Count == 1)
                        {
                            dragged.UnSelected();
                            selectedWaveForms.Remove(dragged);
                        } else
                        {
                            foreach (WaveForm wf in selectedWaveForms) wf.UnSelected();
                            selectedWaveForms.Clear();
                            selectedWaveForms.Add(dragged);
                            dragged.Selected();
                        }
                    }
                } else
                {
                    foreach (WaveForm wf in selectedWaveForms) wf.UnSelected();
                    selectedWaveForms.Clear();
                }
                Cursor = Cursors.Default;
                dragged = null;
                waveFormDragged = false;
                Capture = false;
                base.OnMouseUp(e);
            } catch (Exception f)
            {
            }
        }
        
        public static bool IsInRect(Point p, Rectangle r)
        {
            return p.X>=r.Left && p.X<=r.Right && p.Y>=r.Top && p.Y<=r.Bottom;
        }

        IntPtr pre = IntPtr.Zero;
        bool IsMouseMoved = false;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && waveFormDragged)
            {
                IsMouseMoved = true;
                Point p = Parent.PointToClient(PointToScreen(e.Location));
                if (IsInRect(p, Parent.ClientRectangle)) {
                    if (GetPosAudioTrack(p.Y) != null && GetPosAudioTrack(p.Y) != TrackView.GetInstance().root && !GetPosAudioTrack(p.Y).IsFolder())
                        Cursor = GenerateCursor(e, 1, ModifierKeys);
                    else Cursor = GenerateCursor(e, 0, ModifierKeys);
                } else
                {
                    Cursor = Cursors.Default;
                }
            }
            base.OnMouseMove(e);
        }

        [DllImport("user32.dll", EntryPoint = "CreateIconIndirect")]
        private static extern IntPtr CreateIconIndirect(IntPtr iconInfo);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        private struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32")]
        private static extern bool GetIconInfo(IntPtr hIcon, out IconInfo pIconInfo);

        [DllImport("User32.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private static extern IntPtr LoadCursorFromFile(string filename);

        private Bitmap BitmapFromCursor(Cursor cur)
        {
            IconInfo ii;
            GetIconInfo(cur.Handle, out ii);

            Bitmap bmp = Bitmap.FromHbitmap(ii.hbmColor);
            DeleteObject(ii.hbmColor);
            DeleteObject(ii.hbmMask);

            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            Bitmap dstBitmap = new Bitmap(bmData.Width, bmData.Height, bmData.Stride, PixelFormat.Format32bppArgb, bmData.Scan0);
            bmp.UnlockBits(bmData);

            return new Bitmap(dstBitmap);
        }
        private Cursor GenerateCursor(MouseEventArgs e, int t, Keys keys)
        {
            if (t == 1)
            {
                long s = CurSmallUnit;
                long stime = (GetPosTime(e.Location.X) - dragOffsetTime + s / 2) / s * s;
                long duration = dragged.duration;
                long tLTime = (long)(GetPosTime(e.Location.X) - CurTimeUnitPerPixel * PointToScreen(e.Location).X);
                long tRTime = tLTime + (long)(Screen.PrimaryScreen.WorkingArea.Width * CurTimeUnitPerPixel);
                long L = Math.Max(stime, tLTime), R = Math.Min(stime + duration, tRTime);
                Size imgSize = new Size((int)((R - L) * CurPixelPerTimeUnit), CurTrackHeight);
                Bitmap imgColor = new Bitmap(imgSize.Width + 30, imgSize.Height + 30, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Cursor c = new Cursor(new MemoryStream(Properties.Resources.Move));
                SolidBrush brush = new SolidBrush(Color.Green);
                SolidBrush brushMask = new SolidBrush(Color.White);
                using (Graphics g = Graphics.FromImage(imgColor))
                {
                    g.FillRectangle(brushMask, 0, 0, imgSize.Width + 30, imgSize.Height + 30);
                    g.DrawRectangle(new Pen(brush, 2), 1, 1, imgSize.Width + 1, imgSize.Height + 1);
                    g.Flush();
                }
                imgColor.MakeTransparent(Color.White);
                IconInfo ii = new IconInfo();
                ii.fIcon = false;
                ii.xHotspot = (int)((GetPosTime(e.Location.X) - L) * CurPixelPerTimeUnit) + 1;
                ii.yHotspot = e.Location.Y % CurTrackHeight + 1;
                using (Graphics g = Graphics.FromImage(imgColor))
                {
                    c.Draw(g, new Rectangle(ii.xHotspot, ii.yHotspot, 25, 25));
                    g.Flush();
                }
                ii.hbmMask = imgColor.GetHbitmap();
                ii.hbmColor = imgColor.GetHbitmap();
                IntPtr iiPtr = Marshal.AllocHGlobal(Marshal.SizeOf(ii));
                Marshal.StructureToPtr(ii, iiPtr, true);
                IntPtr curPtr = CreateIconIndirect(iiPtr);
                Cursor cur = new Cursor(curPtr);
                if (pre != IntPtr.Zero)
                {
                    DestroyIcon(pre);
                }
                imgColor.Dispose();
                DestroyIcon(iiPtr);
                DeleteObject(ii.hbmMask);
                DeleteObject(ii.hbmColor);
                pre = curPtr;
                return cur;
            }
            if (t == 0)
            {
                MemoryStream cursorStream = new MemoryStream(Properties.Resources.no_r);
                return new Cursor(cursorStream);;
            }
            return null;
        }

        public void MyMouseDown(MouseEventArgs e, bool flag = false)
        {
            Control control = GetChildAtPoint(e.Location);
            IsMouseMoved = false;
            if (control != null && control.GetType() == typeof(WaveForm))
            {
                WaveForm wf = (WaveForm)control;
                if (wf.Location.Y + wf.Height / 2 > e.Y)
                {
                    dragged = wf;
                    dragOffsetTime = GetPosTime(e.X, false) - wf.StartTime;
                    waveFormDragged = true;
                    Capture = true;
                    Cursor = Cursors.Hand;
                    CurPlayStartTime = wf.StartTime;
                }
                else CurPlayStartTime = GetPosTime(e.Location.X, !flag);
            } else CurPlayStartTime = GetPosTime(e.Location.X, !flag);
            UpdateTimeLines();
        }
        public Int64 GetPosTime(int x, bool ok = true)
        {
            if (ok) return (Int64)Math.Round((x * CurTimeUnitPerPixel + LTime)/CurSmallUnit)*CurSmallUnit;
            return (Int64)(x * CurTimeUnitPerPixel + LTime);
        }
        public int GetTimePos(Int64 x)
        {
            return (int)((x - LTime) * CurPixelPerTimeUnit);
        }
        public AudioTrack GetPosAudioTrack(int y, bool ok = false)
        {
            int t = (y + 6) / CurTrackHeight;
            if (t>=TrackView.GetInstance().Controls.Count)
            {
                if (ok)
                {
                    t = TrackView.GetInstance().Controls.Count;
                    TrackView.GetInstance().AddTrack(false, true);
                }
                else
                {
                    return null;
                }
            }
            return (AudioTrack)(TrackView.GetInstance().Controls[t]);
        }

        public void DeleteKeyDown()
        {
            if (selectedWaveForms.Count == 0) return;
            HistoryManager.Do(new HistoryManager.Operation(Copy(selectedWaveForms), HistoryManager.OperationType.Delete));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Delete)
            {
                DeleteKeyDown();
            }
            //MainForm.GetInstance().Text = keyData + "";
            if (keyData == (Keys.Control | Keys.Z))
            {
                HistoryManager.Undo();
            }
            if (keyData == (Keys.Control | Keys.Shift | Keys.Z))
            {
                HistoryManager.Redo();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        public List<WaveForm> Copy(List<WaveForm> a)
        {
            List<WaveForm> b = new List<WaveForm>();
            foreach (WaveForm c in a) b.Add(c);
            return b;
        }
        public bool isPlaying = false;
        public void PlayOrStop(int t = 0)
        {
            bool pre = isPlaying;
            isPlaying = !isPlaying;
            if (t == 1) isPlaying = false;
            else if (t == 2) isPlaying = true;
            if (isPlaying)
            {
                if (!pre && MasterTrack.GetInstance() != null) MasterTrack.GetInstance().Clear();
                CurPlayTime = CurPlayStartTime;
                UpdateTimeLines();
                PlayAudio();
                preTime = DateTime.Now;
//                PlayWaveControls(t==2);
                tm.Start();
                Pulse.it.Play(CurPlayTime);
            }
            else
            {
                CurPlayTime = CurPlayStartTime;
                UpdateTimeLines();
                StopAudio();
//                StopWaveControls();
                Pulse.it.Stop();
                tm.Stop();
            }
        }

        DateTime preTime = DateTime.Now;
        private void TimeTick(object sender, EventArgs e)
        {
            //MasterTrack mt = MasterTrack.GetInstance();
            //if (mt != null)
            //{
            //    mt.Invalidate();
            //    mt.Cnt = 0;
            //    mt.CurVal = double.MinValue;
            //}
            DateTime curTime = DateTime.Now;
            CurPlayTime += (long)(curTime.Subtract(preTime).TotalMinutes * BPM * (1<<20));
            preTime = curTime;
            UpdateAudio();
//            PlayWaveControls();
            UpdateTimeLines();
        }

        public void UpdateAudio()
        {
            bool ok = false;
            if (mixer == null) makeMixer(true);
            if (mixer == null) return;
            foreach (var a in samples)
            {
                if (CurPlayTime >= a.wf.StartTime && CurPlayTime < a.wf.StartTime + a.wf.duration)
                {
                    bool flag = false;
                    foreach(var c in mixer.MixerInputs) if (a == c)
                        {
                            flag = true; break;
                        }
                    if (flag)
                    {
                        continue;
                    }
                    a.ws.SetPosition(CurPlayTime - a.wf.StartTime, a.wf.duration, a.wf.m_Wavefile.os.TotalTime.TotalSeconds);
                    mixer.AddMixerInput(a);
                    ok = true;
                } else
                {
                    foreach(var c in mixer.MixerInputs) if (a == c)
                        {
                            mixer.RemoveMixerInput(c);
                            break;
                        }
                }
            }
            if (ok)
            {
                if (mixer!=null) audioPlayer.Play();
            }
        }

        public bool makeMixer(bool ok = false)
        {
            try
            {
                mixer = new MyMixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2), TrackView.GetInstance().root.eqProperty);
                audioPlayer.Init(mixer);
            }
            catch
            {
                mixer = null;
                if (!ok) MyMessageBox.Show("Please verify your audio devices.\nIt's not opened!");
                return false;
            }
            return true;
        }

        public void PlayAudio()
        {
            try
            {
                if (mixer == null) makeMixer();
                if (mixer == null) return;
                if (audioPlayer == null) return;
                audioPlayer.Stop();
                mixer.RemoveAllMixerInputs();
                foreach (var a in samples)
                {
                    if (CurPlayTime >= a.wf.StartTime && CurPlayTime < a.wf.StartTime + a.wf.duration)
                    {
                        mixer.AddMixerInput(a);
                        a.ws.SetPosition(CurPlayTime - a.wf.StartTime, a.wf.duration, a.wf.m_Wavefile.os.TotalTime.TotalSeconds);
                    }
                }
                audioPlayer.Play();
            } catch
            {

            }
        }

        public void StopAudio()
        {
            try
            {
                if (mixer == null) makeMixer();
                if (mixer == null) return;
                if (audioPlayer == null) return;
                audioPlayer.Stop();
            } catch
            {

            }
        }

        private void PlayWaveControls(bool ok = false)
        {
            foreach (Control ctrl in Controls)
            {
                if (ctrl.GetType() != typeof(WaveForm)) continue;
                if (ok) ((WaveForm)ctrl).ReStart();
                else ((WaveForm)ctrl).Play();
            }
        }

        private void StopWaveControls()
        {
            foreach (Control ctrl in Controls)
            {
                if (ctrl.GetType() != typeof(WaveForm)) continue;
                ((WaveForm)ctrl).Stop();
            }
        }
    }
}
