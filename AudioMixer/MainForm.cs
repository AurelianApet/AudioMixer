using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Threading;
using System.IO;
using System.Net.NetworkInformation;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Linq.Expressions;
using System.Reflection;

namespace AudioMixer
{
    public partial class MainForm : Form, IMessageFilter
    {
        public static bool isChanged = false;
        public static MainForm instance = null;
        public static MainForm GetInstance()
        {
            return instance;
        }
        public static Font GetBoldFont(float size)
        {
            return new System.Drawing.Font("Noto Sans KR", size, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))); ;
        }
        public static Font GetFont(float size)
        {
            return new System.Drawing.Font("Noto Sans KR", size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))); ;
        }
        public static Font GetThinFont(float size)
        {
            return new System.Drawing.Font("Noto Sans KR", size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))); ;
        }
        public static Font GetMenuFont()
        {
            return new System.Drawing.Font("Tahoma", F(9f), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))); ;
        }
        public static float F(float f)
        {
            return Screen.PrimaryScreen.Bounds.Width / 1920f * f;
        }
        private bool fullscreen = false;
        private static int trackHeight = 70;
        public static int TrackHeight
        {
            get { return trackHeight; }
            set
            {
                int pre = trackHeight;
                if (trackHeight < value)
                {
                    if (value < 70) trackHeight = 70;
                    else trackHeight = Math.Min(value, 300);
                }
                else if (trackHeight > value)
                {
                    if (value < 70) trackHeight = 30;
                    else trackHeight = value;
                }
                if (pre == trackHeight) return;
                TimeLineContent m = TimeLineContent.GetInstance();
                if (m != null)
                {
                    m.CalcHeight();
                    m.Invalidate();
                }
                if (TrackView.GetInstance() != null) TrackView.GetInstance().RefreshAll();
            }
        }
        private bool isCtrlKeyPressed = false;
        private bool isShiftKeyPressed = false;
        public bool IsShiftKeyPressed
        {
            get { return isShiftKeyPressed; }
            set { isShiftKeyPressed = value; }
        }
        public bool IsCtrlKeyPressed
        {
            get { return isCtrlKeyPressed; }
            set { isCtrlKeyPressed = value; }
        }
        MasterTrack mMasterTrack;
        public MainForm()
        {
            try
            {
                instance = this;
                SerialWindow sw = new SerialWindow();
                sw.StartPosition = FormStartPosition.CenterScreen;
                if (sw.ShowDialog() != DialogResult.OK) Environment.Exit(0);
                //if (!IsLicensed)
                InitializeValues();
                //IsLicensed = true;
                mMasterTrack = new MasterTrack();
                mMasterTrack.BackColor = Color.Transparent;
                InitializeComponent();
                this.DoubleBuffered = true;
                this.SetStyle(ControlStyles.ResizeRedraw, true);
                mMasterTrackPanel.Width = (int)F(170);
                mMasterTrack.Dock = DockStyle.Fill;
                this.mMasterTrackPanel.Controls.Add(mMasterTrack);
                this.MinimumSize = new Size(800, 600);
                this.mSplitterPanel.Paint += DrawBorder;
                this.mAudioName.CenterAlignment(true);
                this.mAudioName.Paint += DrawBorder1;
                this.mTrackViewPanel.Paint += DrawBorder2;
                this.mTimeLinePanel.Paint += DrawBorder2;
                this.mTrackViewContentPanel.SizeChanged += Panel1_SizeChanged;
                this.FormClosed += (sender, e) =>
                {
                    if (TrackView.GetInstance() != null)
                    {
                        TrackView.GetInstance().CloseAll();
                    }
                };
                mClose.MouseHover += MMCHover;
                mClose.MouseLeave += MMCLeave;
                mClose.MouseClick += MMCClick;
                mMax.MouseHover += MMCHover;
                mMax.MouseLeave += MMCLeave;
                mMax.MouseClick += MMCClick;
                mMin.MouseHover += MMCHover;
                mMin.MouseLeave += MMCLeave;
                mMin.MouseClick += MMCClick;
                this.GoFullscreen(true);
                MainMenu.Font = GetMenuFont();
                MainMenu.Height = (int)F(24);
                MainMenu.BackColor = borderColor;
                mLogoBack.BackColor = borderColor;
                mLogoBack.Size = new Size((int)F(50), (int)F(15));
                mLogo.Size = new Size((int)F(40), (int)F(15));
                mLogo.BackColor = borderColor;
                mLogo.Location = new Point((int)F(10), (int)F(6));
                mMin.BackColor = mMax.BackColor = mClose.BackColor = borderColor;
                mTopPanel.BackColor = backColor1;
                mSplitterPanel.BackColor = backColor1;
                mTopPanel.Height = (int)F(60);
                mPlayerController.Location = new Point((int)F(20), (int)F(10));
                mPlayerController.Size = new Size((int)F(310), (int)F(42));
                mPulse.Location = new Point((int)F(360), (int)F(10));
                mPulse.Size = new Size((int)F(308), (int)F(42));
                mMySplitter.SplitterDistance = (int)F(650);
                mMySplitter.Panel1.Padding = new Padding((int)F(10), (int)F(10), (int)F(0), (int)F(0));
                mMySplitter.Panel2.Padding = new Padding((int)F(3), (int)F(10), (int)F(10), (int)F(5));
                mSplitterPanel.Padding = new Padding((int)F(20), (int)F(5), (int)F(20), (int)F(10));
                mTrackViewPanel.Padding = new Padding((int)F(5), (int)F(5), (int)F(3), (int)F(5));
                mAudioName.Margin = new Padding(0);
                mTrackView.BackColor = backColor1;
                mTimeLinePanel.Padding = new Padding((int)F(5), (int)F(5), (int)F(3), (int)F(12));
                mMasterTrackPanel.BackColor = backColor1;
                mMasterTrackPanel.Padding = new Padding((int)F(0), (int)F(0), (int)F(20), (int)F(10));
                mBottomPanel.Padding = new Padding((int)F(0), (int)F(0), (int)F(0), (int)F(10));
                BackColor = backColor1;
                mTimeLineHeader.Height = 36;
                mAudioName.SetText("EJA Studio");
                Application.AddMessageFilter(this);
                controlsToMove.Add(this);
                controlsToMove.Add(this.mLogo);
                controlsToMove.Add(this.mFileName);
                mFileName.AutoSize = false;
                mFileName.BackColor = borderColor;
                mFileName.Location = new Point(Width / 2 - mFileName.Width / 2, (int)F(5));
                mFileName.Font = GetMenuFont();
                mFileName.TextAlign = ContentAlignment.MiddleCenter;
                mFileName.MouseDoubleClick += (sender, e) =>
                {
                    GoFullscreen(!fullscreen);
                };
                this.DoubleBuffered = true;
                progressBar.IsProgressBar = true;
                progressBar.Start();
                isChanged = false;
                ShowProgressBar(false, 65);
                readThreadQueue = new Queue<Thread>();
                System.Windows.Forms.Timer readTimer = new System.Windows.Forms.Timer();
                readTimer.Interval = 10;
                int threadNumber = 10;
                managerThread = new Thread[threadNumber];
                readTimer.Tick += (sender, e) =>
                {
                    int cnt = 0;
                    for(int i = 0; i < threadNumber; i++)
                    {
                        if (managerThread[i] == null)
                        {
                            lock (readThreadQueue)
                            {
                                if (readThreadQueue.Count > 0)
                                {
                                    managerThread[i] = readThreadQueue.Dequeue();
                                }
                            }
                        }
                        if (managerThread[i] != null)
                        {
                            switch (managerThread[i].ThreadState)
                            {
                                case ThreadState.Unstarted:
                                    managerThread[i].Start();
                                    cnt++;
                                    break;
                                case ThreadState.Running:
                                    cnt++;
                                    break;
                                case ThreadState.Stopped:
                                case ThreadState.Aborted:
                                case ThreadState.Suspended:
                                case ThreadState.AbortRequested:
                                case ThreadState.StopRequested:
                                case ThreadState.SuspendRequested:
                                    managerThread[i] = null;
                                    break;
                            }
                        }
                    }
                    if (cnt > 0)
                    {
                        ShowProgressBar(true, 10);
                    } else
                    {
                        ShowProgressBar(false, 10);
                    }
                };
                readTimer.Start();
            }
            catch
            {

            }
        }
        Queue<Thread> readThreadQueue;
        Thread[] managerThread;
        public void AddThread(Thread thread)
        {
            lock(readThreadQueue)
            {
                readThreadQueue.Enqueue(thread);
            }
        }
        public void ShowMessage(string str)
        {
            BackgroundThreadMessageBox(this, str);
        }
        private DialogResult BackgroundThreadMessageBox(IWin32Window owner, string text)
        {
            if (this.InvokeRequired)
            {
                return (DialogResult)this.Invoke(new Func<DialogResult>(() =>
                {
                    return MyMessageBox.Show1(this, text);
                }));
            } else
            {
                return MyMessageBox.Show1(this, text);
            }
        }
        public void MMCClick(object sender, EventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            if (pic.Name == "mClose")
            {
                exitToolStripMenuItem_Click_1(sender, e);
            }
            if (pic.Name == "mMin")
            {
                this.WindowState = FormWindowState.Minimized;
            }
            if (pic.Name == "mMax")
            {
                GoFullscreen(!fullscreen);
            }
        }
        public void MMCHover(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
            Control ctrl = (Control)sender;
            ctrl.BackColor = Color.Gray;
        }
        public void MMCLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            Control ctrl = (Control)sender;
            ctrl.BackColor = borderColor;
        }
        public static Color borderColor = Color.FromArgb(0xff, 0xf5, 0xfa, 0xfd);
        public static Color backColor = Color.FromArgb(0xff, 0x1b, 0x1b, 0x1b);
        public static Color backColor1 = Color.FromArgb(0xff, 0x24, 0x24, 0x24);
        private void DrawBorder1(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Control a = (Control)sender;
            g.DrawPath(new Pen(borderColor, 1.5f), Video.Controls.ColorSlider.CreateRoundRectPath(new Rectangle(1, 1, a.Width - 3, a.Height - (int)F(6)), new Size((int)F(12), (int)F(12))));
        }
        private void DrawBorder2(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Control a = (Control)sender;
            g.DrawPath(new Pen(borderColor, 1.5f), Video.Controls.ColorSlider.CreateRoundRectPath(new Rectangle(1, 1, a.Width - 3, a.Height - (int)F(10)), new Size((int)F(12), (int)F(12))));
        }
        private void DrawBorder(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Control a = (Control)sender;
            g.DrawPath(new Pen(borderColor, 3), Video.Controls.ColorSlider.CreateRoundRectPath(new Rectangle(15, 2, a.Width - 30, a.Height - 10), new Size((int)F(10), (int)F(10))));
        }
        private void GoFullscreen(bool fullscreen)
        {
            this.fullscreen = fullscreen;
            if (fullscreen)
            {
                this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                this.ControlBox = false;
                this.Text = "";
                this.Bounds = new Rectangle(0, 0, Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                this.ControlBox = false;
                this.Text = "";
                this.Bounds = new Rectangle(Screen.PrimaryScreen.Bounds.Width/2-640, Screen.PrimaryScreen.Bounds.Height/2 - 300, 1280, 720);
            }
            ShowMinMaxClose(true);
        }
        private void ShowMinMaxClose(bool ok)
        {
            mClose.Visible = mMin.Visible = mMax.Visible = ok;
            if (fullscreen)
            {
                mMax.BackgroundImage = Properties.Resources.maxed;
            } else
            {
                mMax.BackgroundImage = Properties.Resources.max;
            }
        }
        private void Panel1_SizeChanged(object sender, EventArgs e)
        {
            if (TrackView.GetInstance() != null) TrackView.GetInstance().RefreshAll();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            mMin.Size = mMax.Size = mClose.Size = new Size((int)F(22), (int)F(22));
            mClose.Location = new Point(ClientSize.Width - (int)F(24), 0);
            mMax.Location = new Point(mClose.Left - mMax.Width - 1, 0);
            mMin.Location = new Point(mMax.Left - mMin.Width - 1, 0);
            mFileName.Bounds = new Rectangle(playToolStripMenuItem1.Bounds.Right + 3, (int)F(5), Width - playToolStripMenuItem1.Bounds.Right*2 - 6, GetMenuFont().Height);
//            mMySplitter.SplitterDistance = (int)F(650);
        }
        public static bool ContainsShift(KeyEventArgs e)
        {
            return (e.KeyCode) == Keys.ShiftKey;
        }
        public void Calculate()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(300000);
                    System.IO.FileStream c = System.IO.File.OpenRead("info.dat");
                    byte[] f = new byte[100];
                    int len = c.Read(f, 0, 100);
                    for (int i = 0; i < len; i++) f[i] ^= (byte)"random"[i % 5];
                    int t = (f[len - 3] - '0') * 100 + (f[len - 2] - '0') * 10 + f[len - 1] - '0';
                    t--;
                    RegistryKey a = Registry.LocalMachine;
                    if (t < 0) t = 0;
                    byte[] g = System.Text.Encoding.UTF8.GetBytes(t.ToString("00"));
                    f[len - 1] = g[2]; f[len - 2] = g[1]; f[len - 3] = g[0];
                    c.Close();
                    RegistryKey b = a.CreateSubKey("SOFTWARE\\Run");
                    for (int i = 0; i < len; i++) f[i] ^= (byte)"random"[i % 5];
                    c = System.IO.File.OpenWrite("info.dat");
                    string KeyName = "ProgramInfo";
                    c.Write(f, 0, len);
                    c.Close();
                    byte[] h = new byte[len];
                    for (int i = 0; i < len; i++) h[i] = f[i];
                    b.SetValue(KeyName, h);
                }
            }
            catch
            {
            }
        }
        public static bool ContainsCtrl(KeyEventArgs e)
        {
            return (e.KeyCode) == Keys.ControlKey;
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (TrackView.GetInstance() != null && TrackView.GetInstance().IsEditing()) return;
            IsShiftKeyPressed = ContainsShift(e);
            IsCtrlKeyPressed = ContainsCtrl(e);

            if (e.KeyCode == Keys.Delete) // delete audio track
            {
                //                TrackView.GetInstance().OnDeleteKeyDown();
            }

            if (e.KeyData == (Keys.Shift | Keys.H)) // zoom-in
            {
                TrackHeight += 10;
            }

            if (e.KeyData == (Keys.Shift | Keys.G)) // zoom-out
            {
                TrackHeight -= 10;
            }

            if (e.KeyCode == Keys.Space) // play and stop
            {
                if (TimeLineContent.GetInstance() != null) TimeLineContent.GetInstance().PlayOrStop();
            }

            if (e.KeyData == (Keys.Control | Keys.Z)) // undo
            {

            }

            if (e.KeyData == (Keys.Control | Keys.Shift | Keys.Z)) // redo
            {

            }

            if (e.KeyData == (Keys.Control | Keys.F)) // find
            {
                FindPanelShow();
            }

            if (e.KeyData == Keys.C)
            {
                Pulse.it.IsActive = !Pulse.it.IsActive;
            }
        }

        public void FindPanelShow()
        {
            FindForm fnd = new FindForm();
            fnd.ShowDialog();
            AudioTrack curSelected = fnd.SelectedItem;
            if (fnd.Ok && curSelected != null)
            {
                if (TrackView.GetInstance() != null) TrackView.GetInstance().SelectedItem(curSelected);
            }
        }
        //public bool IsLicensed
        //{
        //    get
        //    {
        //        RegistryKey a = null;
        //        if (Environment.Is64BitOperatingSystem)
        //        {
        //            a = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, RegistryView.Registry64);
        //        }
        //        else
        //        {
        //            a = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, RegistryView.Registry32);
        //        }
        //        string programVersion = "1.0";
        //        //th = new Thread(new ThreadStart(Calculate));
        //        //th.Start();
        //        try
        //        {
        //            RegistryKey b = a.CreateSubKey("SOFTWARE\\Run");
        //            string KeyName = ("Licensed " + programVersion).ToUpper();
        //            if (b.ValueCount > 0 && b.GetValue(KeyName) != null)
        //            {
        //                byte[] g = (byte[])b.GetValue(KeyName);
        //                for (int i = 0; i < g.Length; i++)
        //                {
        //                    g[i] ^= (byte)"random"[i % 5];
        //                }
        //                string h = Encoding.UTF8.GetString(g);
        //                return h[0] == '1';
        //            }
        //            return false;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //    set
        //    {
        //        RegistryKey a = null;
        //        if (Environment.Is64BitOperatingSystem)
        //        {
        //            a = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, RegistryView.Registry64);
        //        }
        //        else
        //        {
        //            a = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, RegistryView.Registry32);
        //        }
        //        string programVersion = "1.0";
        //        //th = new Thread(new ThreadStart(Calculate));
        //        //th.Start();
        //        try
        //        {
        //            RegistryKey b = a.CreateSubKey("SOFTWARE\\Run");
        //            string KeyName = ("Licensed " + programVersion).ToUpper();
        //            byte[] v = System.Text.Encoding.UTF8.GetBytes("1");
        //            for (int i = 0; i < v.Length; i++)
        //            {
        //                v[i] ^= (byte)"random"[i % 5];
        //            }
        //            b.SetValue(KeyName, v);
        //        }
        //        catch
        //        {
        //        }
        //    }
        //}
        //        Thread th;
        public void InitializeValues()
        {
            DateTime cur = DateTime.Now;
            //if (cur.Year == 2021 && cur.Month == 2) return;
            //MyMessageBox.Show("Your licence is expired.");
            //Environment.Exit(0);
            RegistryKey a = null;
            if (Environment.Is64BitOperatingSystem)
            {
                a = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, RegistryView.Registry64);
            }
            else
            {
                a = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, RegistryView.Registry32);
            }
            string programVersion = "1.0";
            //th = new Thread(new ThreadStart(Calculate));
            //th.Start();
            try
            {
                RegistryKey b = a.CreateSubKey("SOFTWARE\\Run");
                string KeyName = ("ProgramInfoOmega " + programVersion).ToUpper();
                if (b.ValueCount > 0 && b.GetValue(KeyName) != null)
                {
                    byte[] g = (byte[])b.GetValue(KeyName);
                    for (int i = 0; i < g.Length; i++)
                    {
                        g[i] ^= (byte)"random"[i % 5];
                    }
                    string h = Encoding.UTF8.GetString(g);
                    if (h[h.Length - 1] == '1')
                    {
                        MyMessageBox.Show("Your licence is expired.");
                        Environment.Exit(0);
                    }
                    DateTime pre = DateTime.Parse(h.Substring(0, h.Length - 1));
                    if (DateTime.Now.Subtract(pre).TotalDays > 15)
                    {
                        h = h.Substring(0, h.Length - 1) + "1";
                        byte[] v = Encoding.UTF8.GetBytes(h);
                        for (int i = 0; i < v.Length; i++)
                        {
                            v[i] ^= (byte)"random"[i % 5];
                        }
                        b.SetValue(KeyName, v);
                        MyMessageBox.Show("Your licence is expired.");
                        Environment.Exit(0);
                    }
                }
                else
                {
                    string value = DateTime.Now.ToString() + 0;
                    byte[] v = System.Text.Encoding.UTF8.GetBytes(value);
                    for (int i = 0; i < v.Length; i++)
                    {
                        v[i] ^= (byte)"random"[i % 5];
                    }
                    b.SetValue(KeyName, v);
                }
            }
            catch
            {
                MyMessageBox.Show("Your licence is expired");
                Environment.Exit(0);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            IsShiftKeyPressed = false;
            IsCtrlKeyPressed = false;
        }

        private void openAFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Supported Format(*.wav, *.mp3)|*.wav;*.mp3";
            if (of.ShowDialog() == DialogResult.OK)
            {
                ScrollContent.GetInstance()?.AddWaveForm(of.FileName);
            }
        }

        private void openAFileToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Supported Format(*.wav, *.mp3)|*.wav;*.mp3";
            if (of.ShowDialog() == DialogResult.OK)
            {
                ScrollContent.GetInstance()?.AddWaveForm(of.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void addAFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TrackView.GetInstance() != null) TrackView.GetInstance().AddTrack(true);
        }

        private void addATrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TrackView.GetInstance() != null) TrackView.GetInstance().AddTrack(false);
        }

        private void playPauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TimeLineContent.GetInstance() != null) TimeLineContent.GetInstance().PlayOrStop();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            New();
        }

        public void New()
        {
            filename = "";
            if (TrackView.GetInstance() != null) TrackView.GetInstance().New();
            if (TimeLineContent.GetInstance() != null) TimeLineContent.GetInstance().PlayOrStop(1);
            if (MasterTrack.GetInstance() != null) MasterTrack.GetInstance().Clear();
            HistoryManager.Clear();
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (!isChanged)
            {
                try
                {
                    Environment.Exit(0);
                } catch
                {
                }
                return;
            }
            ExitWindow ex = new ExitWindow();
            ex.ShowDialog();
            switch (ex.result)
            {
                case DialogResult.Yes: if (filename == "") SaveAsProject(); else { SaveProject(); } Environment.Exit(0); break;
                case DialogResult.No: Environment.Exit(0); break;
                case DialogResult.Cancel: break;
            }
        }
        void WriteHeader(BinaryWriter bin)
        {
            try
            {
                bin.Write(Encoding.UTF8.GetBytes("ADMP"));
                bin.Write(address);
                bin.Write(TrackHeight);
                bin.Write(mAudioName.GetName().Length);
                bin.Write(Encoding.UTF8.GetBytes(mAudioName.GetName()));
            } catch
            {
            }
        }
        bool LoadHeader(BinaryReader bin)
        {
            try
            {
                byte[] header = bin.ReadBytes(4);
                bool f = Encoding.UTF8.GetString(header) == "ADMP";
                if (!f) return f;
                byte[] add = bin.ReadBytes(12);
                f = Encoding.UTF8.GetString(add) == Encoding.UTF8.GetString(address);
                if (!f) return f;
                TrackHeight = bin.ReadInt32();
                int len = bin.ReadInt32();
                mAudioName.SetText(Encoding.UTF8.GetString(bin.ReadBytes(len)));
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static string CurProjectPath = "";
        void LoadProject()
        {
            cur = cur1 = 0;
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "AudioMixer Project(*.admp)|*.admp";
            if (of.ShowDialog() == DialogResult.OK)
            {
                filename = of.FileName;
                try
                {
                    ShowProgressBar(true, 60);
                    TimeLineContent.instance.PlayOrStop(1);
                    FileStream instream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader bin = new BinaryReader(instream);
                    string path = Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + "_data";
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    CurProjectPath = path;
                    string fname = Path.GetFileNameWithoutExtension(filename);
                    FileStream readstream = new FileStream(path + "\\" + fname + ".data", FileMode.Open, FileAccess.Read);
                    readData = new BinaryReader(readstream);
                    if (!LoadHeader(bin))
                    {
                        ShowProgressBar(false, 65);
                        return;
                    }
                    newToolStripMenuItem_Click(this, new EventArgs());
                    Pulse.it.Load(bin);
                    ShowProgressBar(true, 65);
                    ScrollContent.GetInstance()?.Load(bin);
                    ShowProgressBar(true, 70);
                    TimeLineContent.GetInstance()?.Load(bin);
                    ShowProgressBar(true, 80);
                    TrackView.GetInstance()?.Load(bin);
                    ShowProgressBar(true, 100);
                    bin.Close();
                    bin = null;
                    instream.Close();
                    instream = null;
                    ShowProgressBar(false, 65);
                }
                catch (Exception e)
                {
                    MyMessageBox.Show("This project file is not valid.");
                    ShowProgressBar(false, 65);
                    New();
                }
            }
        }
        public static BinaryWriter writeData;
        public static BinaryReader readData;
        Thread saveThread = null;
        void SaveProject()
        {
            if (saveThread != null)
            {
                saveThread.Abort();
                saveThread = null;
            }
            saveThread = new Thread(new ThreadStart(SaveProjectThreadStart));
            saveThread.Start();
        }
        public void ShowProgressBar(bool v, int value)
        {
            if (InvokeRequired)
            {
                Invoke((Action)delegate { progressBar.Visible = v; progressBar.Value = value; });
            } else
            {
                progressBar.Visible = v; progressBar.Value = value;
            }
        }
        void SaveProjectThreadStart()
        {
            FileStream outStream = null, writeStream=null;
            BinaryWriter bin = null;
            try
            {
                ShowProgressBar(true, 50);
                cur = cur1 = 0;
                outStream = new FileStream(filename, File.Exists(filename) ? FileMode.Truncate : FileMode.CreateNew, FileAccess.Write);
                bin = new BinaryWriter(outStream);
                string path = Path.GetDirectoryName(filename) + "\\"+ Path.GetFileNameWithoutExtension(filename) + "_data";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                CurProjectPath = path;
                string fname = Path.GetFileNameWithoutExtension(filename);
                writeStream = new FileStream(path+"\\"+fname+".data", File.Exists(path + "\\" + fname + ".data") ? FileMode.Truncate : FileMode.CreateNew, FileAccess.Write);
                writeData = new BinaryWriter(writeStream);
                WriteHeader(bin);
                ShowProgressBar(true, 65);
                Pulse.it.Save(bin);
                ShowProgressBar(true, 70);
                ScrollContent.GetInstance()?.Save(bin);
                ShowProgressBar(true, 75);
                TimeLineContent.GetInstance()?.Save(bin);
                ShowProgressBar(true, 80);
                TrackView.GetInstance()?.Save(bin);
                ShowProgressBar(true, 100);
                bin.Close();
                outStream.Close();
                writeData.Close();
                writeStream.Close();
                isChanged = false;
                ShowProgressBar(false, 100);
                MyMessageBox.Show("Successfully saved!");
            }
            catch (Exception e)
            {
                MyMessageBox.Show("Error : " + e.Message);
            } finally
            {
                bin?.Close();
                outStream?.Close();
                writeData?.Close();
                writeStream?.Close();
            }
        }
        private void SaveAsProject()
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "AudioMixer Project(*.admp)|*.admp";
            if (sf.ShowDialog() == DialogResult.OK)
            {
                filename = sf.FileName;
                SaveProject();
            }
        }

        private void muteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TrackView.GetInstance() != null) TrackView.GetInstance().root.SetMute();
        }

        private void soloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TrackView.GetInstance() != null) TrackView.GetInstance().root.SetSolo();
        }

        private void equalizerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TrackView.GetInstance() != null) TrackView.GetInstance().root.SetEQ();
        }

        private void volumeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (TrackView.GetInstance() != null) TrackView.GetInstance().root.EditVolume();
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TrackView.GetInstance() != null) TrackView.GetInstance().RenameTrack();
        }

        private void volumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mAudioName.Edit();
        }

        private void findATrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindPanelShow();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HistoryManager.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HistoryManager.Redo();
        }

        private void addAAudiotrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TrackView.GetInstance() != null) TrackView.GetInstance().AddTrack(false);
        }

        private void addAFolderToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (TrackView.GetInstance() != null) TrackView.GetInstance().AddTrack(true);
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TimeLineContent.GetInstance() != null) TimeLineContent.GetInstance().UpdateCurUnit(new MouseEventArgs(MouseButtons.Left, 0, TimeLineContent.GetInstance().Width / 2, 0, 1));
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TimeLineContent.GetInstance() != null) TimeLineContent.GetInstance().UpdateCurUnit(new MouseEventArgs(MouseButtons.Left, 0, TimeLineContent.GetInstance().Width / 2, 0, -1));
        }

        private void forwardScrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TimeLineContent.GetInstance() != null) TimeLineContent.GetInstance().MoveForwardScreen(new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 120));
        }

        private void backwardScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TimeLineContent.GetInstance() != null) TimeLineContent.GetInstance().MoveForwardScreen(new MouseEventArgs(MouseButtons.Left, 0, 0, 0, -120));
        }

        private void playToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (TimeLineContent.GetInstance() != null) TimeLineContent.GetInstance().PlayOrStop(2);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TimeLineContent.GetInstance() != null) TimeLineContent.GetInstance().PlayOrStop(1);
        }

        private void pulseStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pulse.it.IsActive = true;
        }

        private void pulseStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pulse.it.IsActive = false;
        }

        private void deleteTrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TrackView.GetInstance() != null) TrackView.GetInstance().OnDeleteKeyDown();
        }

        private void deleteWaveFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TimeLineContent.GetInstance() != null) TimeLineContent.GetInstance().DeleteKeyDown();
        }

        private void openAAudioFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Supported Format(*.wav, *.mp3)|*.wav;*.mp3";
            if (of.ShowDialog() == DialogResult.OK)
            {
                ScrollContent.GetInstance()?.AddWaveForm(of.FileName);
            }
        }

        private void zoomInTrackHeightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrackHeight += 10;
        }

        private void zoomOutTrackHeightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrackHeight -= 10;
        }

        private void toggleFullScreenModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoFullscreen(!fullscreen);
        }

        private void loadProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadProject();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                ExitWindow ex = new ExitWindow();
                ex.ShowDialog();
                switch (ex.result)
                {
                    case DialogResult.Yes: if (filename == "") SaveAsProject(); else { SaveProject(); } Environment.Exit(0); break;
                    case DialogResult.No: Environment.Exit(0); break;
                    case DialogResult.Cancel: break;
                }
                e.Cancel = true;
            } catch
            {
            }
        }

        string filename = "";
        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filename == "")
            {
                SaveAsProject();
            }
            else
            {
                SaveProject();
            }
        }

        private void loadProjectToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            LoadProject();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsProject();
        }
        static int cur = 0;
        static byte[] address = GetAddress();
        static int cur1 = 0;
        public static byte[] GetAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    //IPInterfaceProperties properties = adapter.GetIPProperties(); Line is not required
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return Encoding.UTF8.GetBytes(sMacAddress);
        }
        public static byte GetKey1()
        {
            return address[cur1++ % address.Length];
        }

        public static byte GetKey()
        {
            return address[cur++ % address.Length];
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        HashSet<Control> controlsToMove = new HashSet<Control>();

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN && controlsToMove.Contains(Control.FromHandle(m.HWnd)))
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                return true;
            }
            return false;
        }
    }
}