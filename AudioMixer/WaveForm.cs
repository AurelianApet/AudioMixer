using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using NAudio.Wave;

namespace AudioMixer
{
    public partial class WaveForm : UserControl
    {
        PointF[] path1 = null, path2 = null;
        Thread readThread, drawThread;
        public string FileName;
        private Int64 startTime;
        public Int64 duration;
        public AudioTrack trackCtrl;
        public Point[] lftPoint = null, rgtPoint = null;

        public bool IsMute
        {
            get
            {
                return trackCtrl.IsMute;
            }
        }
        public long StartTime
        {
            get { return startTime; }
            set
            {
                if (startTime == value) return;
                startTime = value;
                Int64 curPlayTime = TimeLineContent.GetInstance().CurPlayTime;
                trackCtrl.SetDb(double.MinValue, double.MinValue);
                if (curPlayTime >= startTime && curPlayTime <= startTime + duration)
                {
                    if (isPlaying)
                    {
                        m_Wavefile.os.SetPosition(curPlayTime - startTime, duration, m_Wavefile.os.TotalTime.TotalSeconds);
                    }
                }
                else
                {
//                    if (mp != null) mp.Stop();
                    isPlaying = false;
                }
            }
        }

        public bool isBusy;
        public bool isVisible
        {
            get { return trackCtrl.isVisible; }
        }
        
        /// <summary>
        /// Each pixel value (X direction) represents this many samples in the wavefile
        /// Starting value is based on control width so that the .WAV will cover the entire width.
        /// </summary>
        private double m_SamplesPerPixel = 0.0;
        private double SamplesPerPixel
        {
            set
            {
                m_SamplesPerPixel = value;
            }
        }

        public WaveForm()
        {
            // sets up double buffering
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint, true);
            InitializeComponent();
        }

        public bool notSupported = false;
        public WaveForm(Int64 stTime, AudioTrack audioTrack, string fileName, bool copy = false)
        {
            // sets up double buffering
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | 
                     ControlStyles.UserPaint, true);
            isBusy = true;
            string type = MyWaveFile.GetType(fileName);
            if (type != "mp3" && type != "wav")
            {
                notSupported = true;
                return;
            }

            InitializeComponent();

            startTime = stTime;
            trackCtrl = audioTrack;
            trackCtrl.AddWaveForm(this);
            FileName = fileName;
            Height = MainForm.TrackHeight;
            BackColor = MyColor;
            Location = CurPos;
            TimeLineContent.GetInstance().WaveFormAdd(this);
            Disposed += (sender, e) =>
            {
                if (drawThread != null && drawThread.IsAlive) drawThread.Abort();
                drawThread = null;
            };
        }
        public System.Windows.Forms.Timer tm1;
        public bool Redrawable = false;
        public WaveForm copy()
        {
            WaveForm cur = new WaveForm();
            cur.m_Wavefile = this.m_Wavefile;
            cur.trackCtrl = this.trackCtrl;
            cur.trackCtrl.AddWaveForm(cur);
            cur.StartTime = this.StartTime;
            cur.FileName = this.FileName;
            cur.Height = this.Height;
            cur.BackColor = this.MyColor;
            cur.Location = cur.CurPos;
            TimeLineContent.GetInstance().WaveFormAdd(cur);
            cur.Disposed += (sender, e) =>
            {
                if (cur.drawThread != null && cur.drawThread.IsAlive) cur.drawThread.Abort();
                cur.drawThread = null;
            };
            cur.path1 = null;
            cur.path2 = null;
            cur.isBusy = false;
            cur.duration = (long)(m_Wavefile.Duration / 1e6 / 60 * m_Wavefile.Tempo * (1 << 20));
            string outfilename = cur.FileName + ".wf";
            WaveFileReader ws1 = new WaveFileReader(outfilename);
            WaveStream f = WaveFormatConversionStream.CreatePcmStream(ws1);
            MyWaveStream os = new MyWaveStream(f);
            cur.m_sprovider = new MySampleProvider(os.ToSampleProvider(), trackCtrl.eqProperty, cur, os);
            TimeLineContent.GetInstance().samples.Add(cur.m_sprovider);
            return cur;
        }

        public void Save(BinaryWriter bin)
        {
            bin.Write(startTime);
            AudioTrack.WriteString(bin, dllButton.GetFileName(FileName));
            MainForm.writeData.Write(m_Wavefile.os.Length^19970321);
            byte[] tmp = new byte[m_Wavefile.os.Length];
            m_Wavefile.os.Seek(0, SeekOrigin.Begin);
            m_Wavefile.os.Read(tmp, 0, tmp.Length);
            for (int i = 0; i < tmp.Length; i++) tmp[i] ^= MainForm.GetKey();
            MainForm.writeData.Write(tmp, 0, tmp.Length);
        }

        private void Mp_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            trackCtrl.SetDb(double.MinValue, double.MinValue);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Clicks==0)
            {
                if (e.Y < Height / 2) this.Cursor = Cursors.Hand;
                else this.Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Point p = PointToScreen(e.Location);
            p = TimeLineContent.GetInstance().PointToClient(p);
            TimeLineContent.GetInstance().MyMouseDown(new MouseEventArgs(MouseButtons.Left, 1, p.X, p.Y, 0));
            base.OnMouseDown(e);
        }
        public Color MyColor
        {
            get
            {
                return trackCtrl.GetColor();
            }
        }
        public void UpdateBackColor()
        {
            if (BackColor == Color.Cyan) Selected();
            else UnSelected();
        }
        public void Selected()
        {
            BackColor = Color.Cyan;
        }
        public void UnSelected()
        {
            BackColor = MyColor;
        }
        public void Redraw()
        {
            if (!isVisible)
            {
                if (TimeLineContent.GetInstance() != null)
                {
                    TimeLineContent.GetInstance().ShowWaveForm(this, false);
                }
                return;
            }
            try
            {
                if (!Visible && TimeLineContent.GetInstance() != null)
                {
                    TimeLineContent.GetInstance().ShowWaveForm(this, true);
                }
            }
            catch
            {
            }
            if (m_Wavefile == null) return;
            if (!m_Wavefile.isPlayable) return;
            long st = TimeLineContent.GetInstance().LTime, ed = TimeLineContent.GetInstance().RTime;
            long s = Math.Max(st, startTime), e = Math.Min(ed, startTime + duration);
            ss = s; ee = e;
            if (s > e)
            {
                if (TimeLineContent.GetInstance() != null)
                {
                    TimeLineContent.GetInstance().ShowWaveForm(this, false);
                }
                return;
            }
            try
            {
                if (TimeLineContent.GetInstance() != null)
                {
                    TimeLineContent.GetInstance().ShowWaveForm(this, true);
                    TimeLineContent.GetInstance().PlaceWaveForm(this, CurPos);
                }
            }
            catch
            {

            }
            int width = Math.Max((int)(TimeLineContent.GetInstance().CurPixelPerTimeUnit * (e - s)), 1);
            tSize = new Size(width, MainForm.TrackHeight);
            try
            {
                if (TimeLineContent.GetInstance() != null)
                {
                    if (TimeLineContent.GetInstance().InvokeRequired)
                    {
                        TimeLineContent.GetInstance().Invoke((Action)delegate { Size = tSize; });
                    } else
                    {
                        Size = tSize;
                    }
                }
            }
            catch
            {

            }
            Redrawable = true;
            isBusy = false;
            if (tm1 != null) tm1.Stop();
            tm1 = new System.Windows.Forms.Timer();
            tm1.Interval = 10;
            tm1.Enabled = true;
            tm1.Tick += (sender, ef) =>
            {
                if (Redrawable)
                {
                    DrawBit(tSize, ss, ee);
                    Redrawable = false;
                }
            };
            tm1.Start();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Redraw();
        }
        long ss, ee;
        public Point CurPos
        {
            get { return GetCurPos(startTime, trackCtrl.Location.Y - 6); }
        }

        public Point GetCurPos(Int64 sTime, int y)
        {
            long st = TimeLineContent.GetInstance().LTime, ed = TimeLineContent.GetInstance().RTime;
            long s = Math.Max(st, sTime), e = Math.Min(ed, sTime + duration);
            if (s > e) return new Point(0, 0);
            return new Point(TimeLineContent.GetInstance().GetTimePos(s), y);
        }

        public void ReadFile(string inFile)
        {
            FileName = inFile;
            readThread = new Thread(new ThreadStart(ReadFileProcess));
            MainForm.GetInstance().AddThread(readThread);
        }
        public void ReadFile1(string inFile)
        {
            FileName = inFile;
            m_Wavefile = new MyWaveFile(FileName);
            long length = MainForm.readData.ReadInt64() ^ 19970321;
            string outfilename = MainForm.CurProjectPath + "\\" + FileName + ".wf";
            byte[] temporary = new byte[length];
            isBusy = true;
            MainForm.readData.Read(temporary, 0, (int)length);
            for (int i = 0; i < temporary.Length; i++) temporary[i] ^= MainForm.GetKey();
            if (!File.Exists(outfilename))
            {
                MemoryStream ms = new MemoryStream();
                ms.Position = 0;
                ms.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);
                ms.Write(BitConverter.GetBytes((temporary.Length) + 36), 0, 4);
                ms.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);
                ms.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);
                ms.Write(BitConverter.GetBytes(16), 0, 4);
                ms.Write(BitConverter.GetBytes((ushort)(1)), 0, 2);
                ms.Write(BitConverter.GetBytes((ushort)2), 0, 2);
                ms.Write(BitConverter.GetBytes(44100), 0, 4);
                ms.Write(BitConverter.GetBytes(176400), 0, 4);
                ms.Write(BitConverter.GetBytes((ushort)4), 0, 2);
                ms.Write(BitConverter.GetBytes((ushort)16), 0, 2);
                ms.Write(Encoding.ASCII.GetBytes("data"), 0, 4);
                ms.Write(BitConverter.GetBytes(temporary.Length), 0, 4);
                ms.Write(temporary, 0, temporary.Length);
                ms.Position = 0;
                WaveFileReader ws1 = new WaveFileReader(ms);
                WaveFileWriter.CreateWaveFile(outfilename, ws1);
                ms.Close();
                ws1.Close();
                ms = null;
            }
            temporary = null;
            readThread = new Thread(new ThreadStart(ReadFileProcess1));
            MainForm.GetInstance().AddThread(readThread);
        }
        public void ReadFileProcess1() {
            if (!m_Wavefile.Read1()) return;
            isBusy = false;
            m_sprovider = new MySampleProvider(m_Wavefile.os.ToSampleProvider(), trackCtrl.eqProperty, this, m_Wavefile.os);
            duration = (long)(m_Wavefile.Duration / 1e6 / 60 * m_Wavefile.Tempo * (1 << 20));
            TimeLineContent.GetInstance().TotalTime = Math.Max(TimeLineContent.GetInstance().TotalTime, StartTime + duration);
            TimeLineContent.GetInstance().samples.Add(m_sprovider);
            Redraw();
        }
        public void Error()
        {
            TimeLineContent.GetInstance().WaveFormRemove(this);
            trackCtrl.RemoveWaveForm(this);
            MyMessageBox.Show("This file is not supported format. Please add mp3 or wav files.");
            Dispose();
        }
        public void ReadFileProcess()
        {
            if (FileName == null) {
                Error();
                return;
            }
            isBusy = true;
            m_Wavefile = new MyWaveFile(FileName);
            if (!m_Wavefile.Read())
            {
                Error();
                return;
            }

            m_sprovider = new MySampleProvider(m_Wavefile.os.ToSampleProvider(), trackCtrl.eqProperty, this, m_Wavefile.os);
            duration = (long)(m_Wavefile.Duration/1e6/60*m_Wavefile.Tempo*(1<<20));
            TimeLineContent.GetInstance().TotalTime = Math.Max(TimeLineContent.GetInstance().TotalTime, StartTime + duration);
            TimeLineContent.GetInstance().samples.Add(m_sprovider);
            TimeLineContent.GetInstance().WaveFormAdd(this);
            Redraw();
        }

        private int sign(int a)
        {
            if (a > 0) return 1;
            if (a < 0) return -1;
            return 0;
        }

        public void UpdateHeight()
        {
            path1 = null;
            path2 = null;
            if (readThread != null && readThread.ThreadState == ThreadState.Running)
            {
                readThread.Abort();
            }
            readThread = new Thread(new ThreadStart(ReadFileProcess));
            readThread.Start();
        }

        public void UpdateWidth()
        {
            path1 = null;
            path2 = null;
            Redraw();
        }

        protected override void OnPaint(PaintEventArgs pea)
        {
            if (!isBusy)
            {
                if (Redrawable)
                {
                    DrawBit(tSize, ss, ee);
                    Redrawable = false;
                }
                Draw(pea.Graphics);
            }
            pea.Graphics.DrawRectangle(new Pen(Color.FromArgb(0x69, 0x69, 0x69), 2f), new Rectangle(1, 1, Width-2, Height-2));
        }

        public void Draw(Graphics grfx)
        {

            RectangleF visBounds = Bounds;
            grfx.SmoothingMode = SmoothingMode.AntiAlias;
            grfx.TranslateTransform(0, visBounds.Height);
            grfx.ScaleTransform(1, -1);

            if (m_Wavefile.BitsPerSample % 8 == 0)
            {
                float f = 2f;
                if (TimeLineContent.GetInstance()!=null && TimeLineContent.GetInstance().CurSmallUnit < (1 << 15)) f = 1f;
                Pen pen = new Pen(Color.FromArgb(230, Color.Black), f);
                if (path1 != null && path1.Length>3)
                {
                    grfx.DrawCurve(pen, path1, 0, path1.Length - 1, 0.5f);
                }
                if (path2 != null && path2.Length > 1)
                {
                    grfx.DrawCurve(pen, path2, 0, path2.Length - 1, 0.5f);
                    grfx.DrawLine(pen, 0, visBounds.Height / 4, Width, visBounds.Height / 4);
                    grfx.DrawLine(pen, 0, visBounds.Height * 3 / 4, Width, visBounds.Height * 3 / 4);
                }
                else
                {
                    grfx.DrawLine(pen, 0, visBounds.Height / 2, Width, visBounds.Height / 2);
                }
            }
        }
        private short GetVal(int st, int ed, int r, int m)
        {
            return m_Wavefile.GetVal(st, ed, r, m);
        }

        public Size tSize;
        private void DrawBit(Size tSize, long s, long e)
        {

            Size visBounds = tSize;

            int tot = m_Wavefile.tot;

            // index is how far to offset into the data array
            int startIndex = (int)(tot * ((double)(s - StartTime) / duration));
            int endIndex = (int)(tot * ((double)(e - StartTime) / duration)) + 1;
            endIndex = Math.Min(tot, endIndex);

            m_SamplesPerPixel = (double)(endIndex - startIndex) / Width;

            int m = Math.Max((int)m_SamplesPerPixel, 1);
            int maxSampleToShow = (Math.Max(1, endIndex - startIndex) + m - 1)/m;
            int[] pts = new int[maxSampleToShow * 2 + 10];
            int[] pts1 = new int[maxSampleToShow * 2 + 10];
            int n = 0;
            bool ok = m_Wavefile.Channels == 1;
            if (ok) pts[n++] = (visBounds.Height / 2);
            else
            {
                pts[n] = (visBounds.Height * 3 / 4);
                pts1[n++] = (visBounds.Height / 4);
            }
            float pv;
            if (m_Wavefile.max_pitch < 32767 / 5) pv = m_Wavefile.max_pitch * 5;
            else if (m_Wavefile.max_pitch < 32767 / 4) pv = m_Wavefile.max_pitch * 4;
            else pv = 65535;
            short mx = 0;
            for (int i=startIndex; i< endIndex; i+=m)
            {
                int ee = i + m < endIndex ? i + m : endIndex;
                short minValLeft = GetVal(i, ee, 0, 0), minValRight = 0;
                short t1 = minValLeft;
                if (t1 < 0) t1 = (short)-t1;
                if (mx < t1) mx = t1;
                if (!ok)
                {
                    minValRight = GetVal(i, ee, 1, 0);
                    short t = minValRight;
                    if (t < 0) t = (short)-t;
                    if (mx < t) mx = t;
                }
                short maxValLeft = GetVal(i, ee, 0, 1), maxValRight = 0;
                t1 = maxValLeft;
                if (t1 < 0) t1 = (short)-t1;
                if (mx < t1) mx = t1;
                if (!ok)
                {
                    maxValRight = GetVal(i, ee, 1, 1);
                    short t = maxValRight;
                    if (t < 0) t = (short)-t;
                    if (mx < t) mx = t;
                }
                // scales based on height of window
                int scaledMinValLeft, scaledMaxValLeft;
                int scaledMinValRight, scaledMaxValRight;
                if (ok)
                {
                    scaledMinValLeft = (int)((minValLeft + pv / 2) / pv * visBounds.Height);
                    scaledMaxValLeft = (int)((maxValLeft + pv / 2) / pv * visBounds.Height);
                    if (scaledMaxValLeft==scaledMinValLeft)
                    {
                        pts[n++] = scaledMinValLeft;
                    } else
                    {
                        pts[n++] = scaledMinValLeft;
                        pts[n++] = scaledMaxValLeft;
                    }
                }
                else
                {
                    scaledMinValLeft = (int)((minValLeft + pv / 2) / pv * visBounds.Height / 2 + visBounds.Height / 2);
                    scaledMaxValLeft = (int)((maxValLeft + pv / 2) / pv * visBounds.Height / 2 + visBounds.Height / 2);
                    scaledMinValRight = (int)((minValRight + pv / 2) / pv * visBounds.Height / 2);
                    scaledMaxValRight = (int)((maxValRight + pv / 2) / pv * visBounds.Height / 2);
                    if (scaledMinValLeft == scaledMaxValLeft && scaledMaxValRight == scaledMinValRight)
                    {
                        pts[n] = scaledMinValLeft;
                        pts1[n++] = scaledMinValRight;
                    } else
                    {
                        pts[n] = scaledMinValLeft;
                        pts1[n++] = scaledMinValRight;
                        pts[n] = scaledMaxValLeft;
                        pts1[n++] = scaledMaxValRight;
                    }
                }
            }
            path1 = new PointF[n];
            if (!ok) path2 = new PointF[n];
            float prev = 0;
            for (int i = 0; i < n; i++)
            {
                if (!ok)
                {
                    int y = (int)((double)(pts[i] - 3 * Height / 4) / (Height / 4) * (Height / 4 - 2))  + 3 * Height / 4;
                    path1[i] = new PointF(prev, y);
                    y = (int)((double)(pts1[i] - Height / 4) / (Height / 4) * (Height / 4 - 2)) + Height / 4;
                    path2[i] = new PointF(prev, y);
                } else
                {
                    int y = (int)((double)(pts[i] - Height / 2) / (Height / 2) * (Height / 2 - 2)) + Height / 2;
                    path1[i] = new PointF(prev, y);
                }
                prev += (float)Width/(n-1);
            }
            if (TimeLineContent.GetInstance()!= null)
            {
                TimeLineContent.GetInstance().WaveFormAdd(this);
            }
            Invalidate();
        }

        //        WindowsMediaPlayer mp;

//        MyWaveOut mp = null;
        bool isPlaying = false;
        public void SetMute()
        {
//            if (mp != null) mp.Mute = true;
        }
        public void SetSolo()
        {
//            if (mp != null) mp.Mute = false;
        }

        public void SetEQ()
        {

        }

        public void Play()
        {
//            mp.Mute = IsMute;
            Int64 curPlayTime = TimeLineContent.GetInstance().CurPlayTime;
            if (curPlayTime >= startTime && curPlayTime <= startTime + duration)
            {
                if (Size != tSize)
                {
                    Size = tSize;
                }
                if (isPlaying)
                {
                    double maxVal, avgVal;
                    m_Wavefile.GetDb((double)(TimeLineContent.GetInstance().CurPlayTime - startTime) / duration, trackCtrl.CurDb, out maxVal, out avgVal);
                    if (IsMute) trackCtrl.SetDb(double.MinValue, double.MinValue);
                    else
                    {
                        trackCtrl.SetDb(maxVal, avgVal);
                        MasterTrack mt = MasterTrack.GetInstance();
                        if (mt != null)
                        {
                            if (mt.CurVal < -1e100) {
                                mt.Cnt = 1;
                                mt.CurVal = maxVal;
                            }  else
                            {
                                mt.Cnt++;
                                mt.CurVal += maxVal;
                            }
                        }
                    }
                    return;
                }
                m_Wavefile.os.SetPosition(curPlayTime-startTime, duration, m_Wavefile.os.TotalTime.TotalSeconds);
                isPlaying = true;
            }
            else
            {
//                if (mp!=null) mp.Stop();
                isPlaying = false;
            }
        }
        public void SetEQ(EQProperty eq)
        {
            if (m_sprovider!=null) m_sprovider.SetEq(eq);
        }
        public void Stop()
        {
            trackCtrl.SetDb(double.MinValue, double.MinValue);
            if (!isPlaying) return;
//            if (mp!=null) mp.Stop();
            isPlaying = false;
        }

        public void ReStart()
        {
//            mp.Mute = IsMute;
            Int64 curPlayTime = TimeLineContent.GetInstance().CurPlayTime;
            if (curPlayTime >= startTime && curPlayTime <= startTime + duration)
            {
                m_Wavefile.os.SetPosition(curPlayTime - startTime, duration, m_Wavefile.os.TotalTime.TotalSeconds);
//                if (!isPlaying) mp.Play();
                isPlaying = true;
            }
            else
            {
//                if (mp != null) mp.Stop();
                isPlaying = false;
            }
        }

        public MyWaveFile m_Wavefile;
        public MySampleProvider m_sprovider;
   }
}
