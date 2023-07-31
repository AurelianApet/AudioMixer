using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace AudioMixer
{
    public partial class AudioTrack : UserControl
    {
        private List<AudioTrack> child = new List<AudioTrack>();
        public List<WaveForm> waves = new List<WaveForm>();
        public AudioTrack parentCtrl = null;
        public EQProperty eqProperty = null;
        public FaderCtrl mFaderCtrl;
        private bool showChild = true;
        public bool ShowChild
        {
            get { return showChild; }
            set { showChild = value; }
        }
        public void AddWaveForm(WaveForm wf)
        {
            MainForm.isChanged = true;
            waves.Add(wf);
        }
        public void RemoveWaveForm(WaveForm wf)
        {
            waves.Remove(wf);
            MainForm.isChanged = true;
        }
        public void Delete()
        {
            foreach(AudioTrack a in child)
            {
                a.Delete();
            }
            child.Clear();
            eqProperty = null;
            eqProperty = new EQProperty();
            foreach(WaveForm a in waves)
            {
                a.Dispose();
            }
            waves.Clear();
            if (parentCtrl!= null)
            {
                Dispose();
            } else
            {
                ClearDb();
            }
            mFaderCtrl.New();
            MainForm.isChanged = true;
        }
        public void ClearDb()
        {
            mFaderCtrl.Clear();
//            MainForm.isChanged = true;
        }
        public bool isVisible
        {
            get
            {
                for (AudioTrack a = parentCtrl; a!= null; a = a.parentCtrl)
                {
                    if (!a.ShowChild) return false;
                }
                return true;
            }
        }
        public void EditName()
        {
            TrackName.Edit();
        }
        public void EditVolume()
        {
            mFaderCtrl.EditVolume();
        }
        public bool Editing
        {
            get
            {
                return TrackName.Editing;
            }
        }
        public void AddDb(float f1, float f2)
        {
            mFaderCtrl.AddValue(f1, f2);
        }
        public Color GetColor()
        {
            return mColorShow.BColor;
        }
        public void Inserted()
        {
            foreach (AudioTrack at in GetAllChild(true))
            {
                foreach (WaveForm wf in at.waves)
                {
                    if (TimeLineContent.GetInstance()!=null) TimeLineContent.GetInstance().WaveFormRemove(wf);
                }
            }
        }
        public void AddChildCtrl(AudioTrack ctrl, AudioTrack cur = null, int t = 2)
        {
            if (t == 2) child.Add(ctrl);
            else child.Insert(child.IndexOf(cur)+t, ctrl);
        }
        public void AddChildCtrl(AudioTrack ctrl, int id)
        {
            child.Insert(id, ctrl);
        }
        public void SetParentCtrl(AudioTrack ctrl)
        {
            parentCtrl = ctrl;
            MainForm.isChanged = true;
        }
        public AudioTrack GetParentCtrl()
        {
            return parentCtrl;
        }
        public List<AudioTrack> GetChildCtrls()
        {
            return child;
        }
        public int RemoveChild(AudioTrack ctrl)
        {
            MainForm.isChanged = true;
            int id = child.IndexOf(ctrl);
            child.Remove(ctrl);
            return id;
        }
        public int GetChildSize()
        {
            return child.Count;
        }
        public int GetAllChildSize(bool flag = false)
        {
            int ans = 1;
            if (ShowChild || flag) foreach(AudioTrack a in child)
            {
                if (a.IsFolder()) ans += a.GetAllChildSize(flag);
                else ans++;
            }
            return ans;
        }
        public List<AudioTrack> GetAllChild(bool flag = false)
        {
            List<AudioTrack> ans = new List<AudioTrack>();
            ans.Add(this);
            if (ShowChild || flag) foreach(AudioTrack a in child)
            {
                if (a.IsFolder()) ans.AddRange(a.GetAllChild(flag));
                else ans.Add(a);
            }
            return ans;
        }
        public int GetPadding()
        {
            int t = 0;
            AudioTrack ctrl = parentCtrl;
            while (ctrl != null)
            {
                t += 10;
                ctrl = ctrl.GetParentCtrl();
            }
            return t;
        }
        public enum AudioTrackType
        {
            Default = 0, Selected = 1, Down = 2, Middle = 4, Up = 8, Folder = 16
        }
        private AudioTrackType curType = AudioTrackType.Default;
        public AudioTrackType CurType
        {
            get { return curType; }
            set
            {
                AudioTrackType tmp = curType;
                curType = value;
                if (tmp != curType)
                {
                    MainForm.isChanged = true;
                    Invalidate();
                }
            }
        }
        public void SetFolder()
        {
            CurType = (AudioTrackType)((int)curType & 15) | AudioTrackType.Folder;
            Controls.Remove(mFaderCtrl);
            MainForm.isChanged = true;
            //mColorShow.Width += 10;
            //Mute.Location += new Size(10, 0);
            //Solo.Location += new Size(10, 0);
            //EQSetting.Location += new Size(10, 0);
            //TrackName.Location += new Size(10, 0);
        }
        public void Selected()
        {
            CurType = (AudioTrackType)((int)curType & 30) | AudioTrackType.Selected;
        }
        public void UnSelected()
        {
            CurType = (AudioTrackType)((int)curType & 30);
        }
        public void DrawUpBorder()
        {
            CurType = (AudioTrackType)((int)curType & 17) | AudioTrackType.Up;
        }
        public void DrawDownBorder()
        {
            CurType = (AudioTrackType)((int)curType & 17) | AudioTrackType.Down;
        }
        public void DrawMidBorder()
        {
            CurType = (AudioTrackType)((int)curType & 17) | AudioTrackType.Middle;
        }
        public bool IsSelected()
        {
            return (CurType & AudioTrackType.Selected) == AudioTrackType.Selected;
        }
        public bool IsDrawMidBorder()
        {
            return (CurType & AudioTrackType.Middle) == AudioTrackType.Middle;
        }
        public bool IsDrawUpBorder()
        {
            return (CurType & AudioTrackType.Up) == AudioTrackType.Up;
        }
        public bool IsDrawDownBorder()
        {
            return (CurType & AudioTrackType.Down) == AudioTrackType.Down;
        }
        public void DrawNone()
        {
            CurType = (AudioTrackType)((int)curType & 17);
        }
        public bool IsFolder()
        {
            return (CurType & AudioTrackType.Folder) == AudioTrackType.Folder;
        }
        public AudioTrack()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor | 
                     ControlStyles.UserPaint, true);
            mFaderCtrl = new FaderCtrl();
            Controls.Add(mFaderCtrl);
            mFaderCtrl.Dock = DockStyle.Right;
            mFaderCtrl.BackColor = Color.Transparent;
            InitializeComponent();
            Controls.SetChildIndex(mFaderCtrl, 1);
            eqProperty = new EQProperty();
            eqProperty.Volume = 1.0f;
            waves = new List<WaveForm>();
            Solo.SetType(CheckBoxType.S);
            EQSetting.SetType(CheckBoxType.E);
            mColorShow.BColor = (Color.FromArgb(0xff, 0x4d, 0x4d));
            mColorShow.BorderStyle = BorderStyle.None;
            Mute.MouseClick += new MouseEventHandler(MuteAll);
            Solo.MouseClick += new MouseEventHandler(SoloAll);
            EQSetting.MouseClick += new MouseEventHandler(EQClick);
            this.DoubleClick += DoubleClickEvent;
            mFaderCtrl.ValueChanged += FaderCtrl1_ValueChanged;
            mColorShow.BColorChanged += MColorShow_BackColorChanged;
            TrackName.TrackName.MouseDown += (sender, e) =>
            {
                if (TrackView.GetInstance()!=null)
                {
                    TrackView.GetInstance().Ctrl_MouseDown(this, e);
                }
            };
            TrackName.TrackName.MouseUp += (sender, e) =>
            {
                if (TrackView.GetInstance() != null)
                {
                    TrackView.GetInstance().Ctrl_MouseUp(this, e);
                }
            };
            mFaderCtrl.MouseDown += (sender, e) =>
            {
                if (!((FaderCtrl)sender).isHandled && TrackView.GetInstance() != null)
                {
                    TrackView.GetInstance().Ctrl_MouseDown(this, e);
                }
            };
            mFaderCtrl.MouseUp += (sender, e) =>
            {
                if (!((FaderCtrl)sender).isHandled && TrackView.GetInstance() != null)
                {
                    TrackView.GetInstance().Ctrl_MouseUp(this, e);
                }
            };
            ContextMenuStrip = new ContextMenuStrip();
            ContextMenuStrip.Items.Add("Add Audio Track", null, new EventHandler(AddAudioTrack));
            ContextMenuStrip.Items.Add("Add Folder", null, new EventHandler(AddFolder));
            ContextMenuStrip.Items.Add("Duplicate", null, new EventHandler(Duplicate));
        }
        public void AddAudioTrack(object sender, EventArgs e)
        {
            if (TrackView.GetInstance() == null) return;
            TrackView.GetInstance().AddTrack(false);
            MainForm.isChanged = true;
        }
        public void AddFolder(object sender, EventArgs e)
        {
            if (TrackView.GetInstance() == null) return;
            TrackView.GetInstance().AddTrack(true);
            MainForm.isChanged = true;
        }
        public void Duplicate(object sender, EventArgs e)
        {
            if (TrackView.GetInstance() == null) return;
            TrackView.GetInstance().Duplicate(this);
            MainForm.isChanged = true;
        }
        public double CurDb = 0;
        private void FaderCtrl1_ValueChanged(object sender, EventArgs e)
        {
            CurDb = ((FaderCtrl)sender).Value;
            eqProperty.Volume = (float)Math.Pow(10, CurDb / 20);
            MainForm.isChanged = true;
        }

        public float v = 1.0f;
        private void MColorShow_BackColorChanged(object sender, EventArgs e)
        {
            foreach(WaveForm wf in waves) wf.UpdateBackColor();
        }

        public bool IsMute
        {
            get { return Mute.IsOn; }
        }
        public bool IsSolo
        {
            get { return Solo.IsOn; }
        }
        public void SetMute1()
        {
            Mute.IsOn = false;
            MainForm.isChanged = true;
        }
        public void SetSolo1()
        {
            Solo.IsOn = false;
            MainForm.isChanged = true;
        }
        public void SetMute(bool flag = false)
        {
            Mute.IsOn = !Mute.IsOn;
            if (flag) Mute.IsOn = true;
            Solo.IsOn = false;
            eqProperty.Mute = true;
            MainForm.isChanged = true;
            if (mColorShow.Number == -1)
            {
                if (MasterTrack.GetInstance()!=null) MasterTrack.GetInstance().Ended();
            }
            if (!IsFolder())
            {

            }
        }

        public void SetSolo(bool flag = false)
        {
            Mute.IsOn = false;
            Solo.IsOn = !Solo.IsOn;
            if (flag) Solo.IsOn = true;
            eqProperty.Mute = false;
            MainForm.isChanged = true;
        }
        public void MuteAll(bool flag = false)
        {
            foreach (AudioTrack a in GetAllChild())
            {
                try
                {
                    a.SetMute(flag);
                } catch
                {

                }
            }
        }
        private void MuteAll(object sender, MouseEventArgs e)
        {
            if (Mute.IsOn)
            {
                Mute.IsOn = false;
            }
            else
            {
                MuteAll(true);
//                TrackView.GetInstance()?.UnSetOther();
            }
        }

        public void SoloAll(bool flag = false)
        {
            foreach (AudioTrack a in GetAllChild())
            {
                try
                {
                    a.SetSolo(flag);
                } catch
                {

                }
            }
            if (flag)
                if (TrackView.GetInstance() != null)
                {
                    TrackView.GetInstance().SetSoloOther();
                }
        }

        private void SoloAll(object sender, MouseEventArgs e)
        {
            if (Solo.IsOn)
            {
                Solo.IsOn = false;
                if (TrackView.GetInstance() != null)
                {
                    TrackView.GetInstance().UnSetOther();
                }
            }
            else SoloAll(true);
        }

        MyEqualizer eq;
        public void SetEQ()
        {
            EQSetting.IsOn = !EQSetting.IsOn;
            if (EQSetting.IsOn)
            {
                if (eq==null) eq = new MyEqualizer();
                eq.Init(eqProperty);
                eq.Text = "Equalizer of " + GetName();
                eq.KeyPreview = true;
                eq.KeyDown += (sender, e) =>
                {
                    if (e.KeyData == Keys.E)
                    {
                        ((MyEqualizer)sender).Hide();
                        EQSetting.IsOn = false;
                        eq = null;
                    }
                    if (e.KeyData == Keys.Space)
                    {
                        if (TimeLineContent.GetInstance() != null) TimeLineContent.GetInstance().PlayOrStop();
                    }
                };
                eq.Disposed += (sender, e) => {
                    EQSetting.IsOn = false;
                    eq = null;
                };
                eq.Show();
            } else
            {
                if (eq != null)
                {
                    eq.Dispose();
                    eq = null;
                }
            }
        }

        private void EQClick(object sender, MouseEventArgs e)
        {
            if (!IsFolder()) SetEQ();
        }

        public void SetNum(int id)
        {
            mColorShow.Number = id;
            MainForm.isChanged = true;
        }

        public void SetDb(double blockMax, double blockAvg)
        {
            mFaderCtrl.BlockMax = blockMax;
            mFaderCtrl.BlockAvg = blockAvg;
            mFaderCtrl.Invalidate();
        }

        public void SetName(string str)
        {
            TrackName.SetText(str);
        }

        public string GetName()
        {
            return TrackName.GetName();
        }

        private void numShow1_Click(object sender, EventArgs e)
        {
            colorselector.Visible = true;
            MainForm.isChanged = true;
        }

        private void colorselector_ColorChanged(object sender, EventArgs e)
        {
            mColorShow.BColor = (((Panel)sender).BackColor);
            MainForm.isChanged = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect1 = new Rectangle(0, 0, Width-1, Height-1);
            System.Drawing.Drawing2D.GraphicsPath path = Video.Controls.ColorSlider.CreateRoundRectPath(rect1, new Size(8, 8));
            e.Graphics.DrawPath(new Pen(MainForm.borderColor, 1.5f), path);
            if (IsFolder())
            {
                mColorShow.IsFolder = true;
                mColorShow.Invalidate();
            }
            if ((CurType & AudioTrackType.Selected)==AudioTrackType.Selected)
            {
                BackColor = Color.FromArgb(0xff, 0x11, 0x11, 0x11);
            } else
            {
                BackColor = Color.FromArgb(0xff, 0x43, 0x43, 0x43);
            }
            if ((CurType & AudioTrackType.Down)==AudioTrackType.Down)
            {
                ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.Green, 0, ButtonBorderStyle.None, Color.Green, 0, ButtonBorderStyle.None, Color.Green, 0, ButtonBorderStyle.None, Color.Green, 2, ButtonBorderStyle.Solid);
            }
            if ((curType & AudioTrackType.Up)==AudioTrackType.Up)
            {
                ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.Green, 0, ButtonBorderStyle.None, Color.Green, 2, ButtonBorderStyle.Solid, Color.Green, 0, ButtonBorderStyle.None, Color.Green, 0, ButtonBorderStyle.None);
            }
            if ((curType & AudioTrackType.Middle) == AudioTrackType.Middle)
            {
                Rectangle rect = ClientRectangle;
                rect.Width = rect.Width / 2 - mColorShow.Width;
                rect.Height /= 2;
                rect.X += mColorShow.Width;
                ControlPaint.DrawBorder(e.Graphics, rect, Color.Green, 0, ButtonBorderStyle.None, Color.Green, 0, ButtonBorderStyle.None, Color.Green, 0, ButtonBorderStyle.None, Color.Green, 2, ButtonBorderStyle.Solid);
            }
            base.OnPaint(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            mFaderCtrl.Visible = Height >= 70;
            TrackName.Width = Size.Width - TrackName.Location.X - mFaderCtrl.Width - 5;
            TrackName.Location = new Point(TrackName.Location.X, (Height - TrackName.Height) / 2);
            base.OnSizeChanged(e);
        }

        private void DoubleClickEvent(object other, EventArgs e)
        {
            AudioTrack a = (AudioTrack)other;
            if (a.IsFolder())
            {
                a.ShowChild = !a.ShowChild;
                if (TrackView.GetInstance()!=null) TrackView.GetInstance().RefreshAll();
            }
        }

        public static string ReadString(BinaryReader bin)
        {
            return Encoding.UTF8.GetString(bin.ReadBytes(bin.ReadInt32()));
        }
        public static void WriteString(BinaryWriter bin, string str)
        {
            byte[] val = Encoding.UTF8.GetBytes(str);
            bin.Write(val.Length);
            bin.Write(val);
        }

        public new void Load(BinaryReader bin)
        {
            if (bin.ReadBoolean()) SetMute();
            else SetSolo();

            SetName(ReadString(bin));

            SetNum(bin.ReadInt32());

            mColorShow.BackColor = Color.FromName(ReadString(bin));

            mFaderCtrl.Load(bin);

            eqProperty.Load(bin);

            if (bin.ReadBoolean()) SetFolder();

            ShowChild = bin.ReadBoolean();

            int len = bin.ReadInt32();
            child.Clear();
            for (int i = 0; i < len; i++)
            {
                var a = new AudioTrack();
                a.SetParentCtrl(this);
                a.MouseDown += TrackView.GetInstance().Ctrl_MouseDown;
                a.MouseUp += TrackView.GetInstance().Ctrl_MouseUp;
                a.Load(bin);
                child.Add(a);
            }

            len = bin.ReadInt32();
            for (int i = 0; i < len; i++)
            {
                long stTime = bin.ReadInt64();
                string filename = ReadString(bin);
                temp.Add(stTime);
                temp.Add(filename);
            }
        }
        List<object> temp = new List<object>();
        public void AddWaveFormsFromTemporary()
        {
            foreach (AudioTrack a in GetAllChild(true))
            {
                if (a == this) continue;
                a.AddWaveFormsFromTemporary();
            }
            if (temp.Count == 0) return;
            waves.Clear();
            for (int i = 0; i<temp.Count; i+=2)
            {
                long startTime = (long)temp[i];
                string filename = (string)temp[i + 1];
                WaveForm wf = new WaveForm(startTime, this, filename);
                wf.ReadFile1(filename);
            }
            temp.Clear();
            if (mColorShow.Number == -1)
            {
                MainForm.GetInstance().ShowProgressBar(false, 100);
            }
        }

        public void Save(BinaryWriter bin)
        {
            bin.Write(IsMute);

            WriteString(bin, GetName());

            bin.Write(mColorShow.Number);

            WriteString(bin, mColorShow.BackColor.Name);

            mFaderCtrl.Save(bin);

            eqProperty.Save(bin);

            bin.Write(IsFolder());

            bin.Write(ShowChild);

            bin.Write(GetChildSize());
            lock(child) foreach (AudioTrack a in child) a.Save(bin);

            bin.Write(waves.Count);
            lock(waves) 
                foreach (WaveForm a in waves) a.Save(bin);
        }

        private void AudioTrack_Load(object sender, EventArgs e)
        {

        }
    }
}
