using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;

namespace AudioMixer
{
    public partial class TrackView : UserControl
    {
        private bool controlDragStarted = false;
        public List<Control> selectedControls = new List<Control>();
        private Pen pen = null;
        private Size mouseOffset;
        private bool isMoved;
        private Control prevHoverControl = null;
        public AudioTrack root;
        private int ID = 0;

        public static TrackView instance = null;
        public static TrackView GetInstance()
        {
            return instance;
        }
        public AudioTrack[] GetAllTracks()
        {
            List<AudioTrack> f = root.GetAllChild(true);
            AudioTrack[] all = new AudioTrack[f.Count - 1];
            for (int i=0, r=0; i<f.Count; i++)
            {
                if (f[i] == root) continue;
                all[r++] = f[i];
            }
            return all;
        }
        public TrackView()
        {
            instance = this;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            InitializeComponent();
            InitComponents();
        }
        public TextBox tmp;
        public void InitComponents()
        {
            Disposed += (sender, e) => {
                if (st != null && st.IsAlive) st.Abort();
                st = null;
            };
            tmp = new TextBox();
            tmp.Visible = false;
            ContextMenuStrip = new ContextMenuStrip();
            ContextMenuStrip.Items.Add("Add Audio Track", null, new EventHandler(AddAudioTrack));
            ContextMenuStrip.Items.Add("Add Folder", null, new EventHandler(AddFolder));
            BackColor = MainForm.backColor;
            pen = new Pen(Color.Green, 5);
            selectedControls = new List<Control>();
            root = new AudioTrack();
            root.SetName("Master");
            root.SetNum(-1);
            for (int i=0; i<0; i++)
            {
                AddTrack(false, false);
            }
        }
        System.Threading.Thread st;
        public new void Load(BinaryReader bin)
        {
            if (st!=null && st.IsAlive) return;
            ID = bin.ReadInt32();
            root.Load(bin);
            RefreshAll();
            st = new System.Threading.Thread(new System.Threading.ThreadStart(root.AddWaveFormsFromTemporary));
            st.Start();
        }
        public void Save(BinaryWriter bin)
        {
            bin.Write(ID);
            root.Save(bin);
        }
        public void New()
        {
            ID = 0;
            root.Delete();
            TimeLineContent.GetInstance().New();
            HistoryManager.Clear();
            RefreshAll();
        }
        public void CloseAll()
        {
            foreach(AudioTrack a in root.GetAllChild(true))
            {
                a.eqProperty.Close();
            }
        }
        public void SelectedItem(AudioTrack tr)
        {
            foreach(AudioTrack ct in selectedControls)
            {
                ct.UnSelected();
            }
            selectedControls.Clear();
            tr.Selected();
            ScrollContent.GetInstance()?.SetVScrollValue(tr.Top);
            selectedControls.Add(tr);
        }
        public Control GetNextControl1(Control a)
        {
            for (int i = 0; i + 1 < Controls.Count; i++)
            {
                if (a == Controls[i]) return Controls[i + 1];
            }
            return null;
        }
        private Random rnd = new Random();
        public List<Object> temparory = new List<object>();
        public AudioTrack AddTrack(bool isFolder, bool select = true, bool isDrop = false, bool isRefresh = true)
        {
            AudioTrack a = new AudioTrack();
            if (!isFolder) a.SetNum(++ID);
            else a.SetNum(-2);
            if (isFolder) a.SetFolder();
            a.MouseDown += new MouseEventHandler(Ctrl_MouseDown);
            a.MouseUp += new MouseEventHandler(Ctrl_MouseUp);
            string str = rnd.Next(0, 100000).ToString();
            a.SetName(str);
            if (selectedControls.Count==0 || isDrop)
            {
                root.AddChildCtrl(a);
                a.SetParentCtrl(root);
            } else
            {
                AudioTrack b = (AudioTrack)selectedControls[selectedControls.Count - 1];
                a.SetParentCtrl(b.GetParentCtrl());
                if (b.GetParentCtrl() != null) b.GetParentCtrl().AddChildCtrl(a, b, 1);
            }
            if (select)
            {
                foreach (Control b in selectedControls)
                {
                    ((AudioTrack)b).UnSelected();
                }
                selectedControls.Clear();
                selectedControls.Add(a);
                a.Selected();
            }
            if (isRefresh) RefreshAll();
            else temparory.Add(a);
            return a;
        }
        private void AddFolder(object sender, EventArgs e)
        {
            AddTrack(true);
        }
        private void AddAudioTrack(object sender, EventArgs e)
        {
            AddTrack(false);
        }
        public void Duplicate(AudioTrack cur)
        {
            if (cur == root) return;
            AudioTrack now = AddTrack(cur.IsFolder());
            now.SetName(cur.GetName() + "(D)");
            //MainForm.GetInstance().Text = cur.waves.Count + "";
            for (int i = 0; i<cur.waves.Count; i++)
            {
                WaveForm wf = cur.waves[i].copy();
                TimeLineContent.GetInstance().WaveFormAdd(wf);
                wf.trackCtrl.RemoveWaveForm(wf);
                wf.trackCtrl = now;
                wf.SetEQ(now.eqProperty);
                now.AddWaveForm(wf);
                wf.Redraw();
            }
        }
        public void ParentSizeChanged()
        {
            if (Parent!=null) Width = Parent.Width;
        }
        public void MouseUpProcess(Point pt)
        {
            Control tmp = GetChildAtPoint(pt);
            if (tmp == null || tmp == root) return;
            if (MainForm.GetInstance().IsCtrlKeyPressed)
            {
                if (selectedControls.Contains(tmp))
                {
                    ((AudioTrack)tmp).UnSelected();
                    selectedControls.Remove(tmp);
                }
                else
                {
                    ((AudioTrack)tmp).Selected();
                    selectedControls.Add(tmp);
                }
            }
            else if (MainForm.GetInstance().IsShiftKeyPressed)
            {
                for (int i = 1; i < selectedControls.Count; i++)
                {
                    ((AudioTrack)selectedControls[i]).UnSelected();
                }
                if (selectedControls.Count>1) selectedControls.RemoveRange(1, selectedControls.Count-1);
                int st = Controls.GetChildIndex(tmp);
                if (selectedControls.Count == 1) st = Controls.GetChildIndex(selectedControls[0]);
                else st--;
                int ed = Controls.GetChildIndex(tmp), stp = st==ed?1:(ed - st) / Math.Abs(ed - st);
                for (int i = st; i != ed;)
                {
                    i += stp;
                    ((AudioTrack)Controls[i]).Selected();
                    selectedControls.Add(Controls[i]);
                }
            }
            else
            {
                if (!selectedControls.Contains(tmp) || selectedControls.Count > 1)
                {
                    for (int i = 0; i < selectedControls.Count; i++)
                    {
                        if (selectedControls[i] != tmp) ((AudioTrack)selectedControls[i]).UnSelected();
                    }
                    selectedControls.RemoveRange(0, selectedControls.Count);
                    ((AudioTrack)tmp).Selected();
                    selectedControls.Add(tmp);
                }
                else
                {
                    ((AudioTrack)tmp).UnSelected();
                    selectedControls.Remove(tmp);
                }
            }
        }
        public void Ctrl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point cursorPos = ((Control)sender).Location + new Size(e.Location.X, e.Location.Y);
                MouseUpProcess(cursorPos);
            }
        }
        public void Ctrl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //find out which control is being dragged
                Point cursorPos = ((Control)sender).Location + new Size(e.Location.X, e.Location.Y);
                Control tmp = GetChildAtPoint(cursorPos);
                isMoved = false;
                if (tmp != null && tmp != root)
                {
                    if (selectedControls != null && selectedControls.Contains(tmp))
                    {
                        Capture = true;
                        Cursor = Cursors.Hand;
                        mouseOffset = new Size(e.Location.X, e.Location.Y);
                        controlDragStarted = true;
                    }
                    if (tmp.GetType()==typeof(AudioTrack))
                    {
                        ((AudioTrack)tmp).Focus();
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (controlDragStarted)
            {
                isMoved = true;
                Control hoverControl = GetChildAtPoint(e.Location);
                if (hoverControl != null)
                {
                    if (hoverControl.GetType() == typeof(AudioTrack))
                    {
                        AudioTrack tmp = (AudioTrack)hoverControl;
                        if (tmp.IsFolder())
                        {
                            if (tmp.Location.Y + tmp.Size.Height / 3 > e.Location.Y) tmp.DrawUpBorder();
                            else if (tmp.Location.Y + tmp.Size.Height * 2 / 3 > e.Location.Y) tmp.DrawMidBorder();
                            else tmp.DrawDownBorder();
                        } else
                        {
                            if (tmp.Location.Y + tmp.Size.Height / 2 > e.Location.Y) tmp.DrawUpBorder();
                            else tmp.DrawDownBorder();
                        }
                    } else hoverControl = null;
                    if (prevHoverControl != null && prevHoverControl!=hoverControl)
                    {
                        ((AudioTrack)prevHoverControl).DrawNone();
                    }
                    prevHoverControl = hoverControl;
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && controlDragStarted)
            {
                if (!isMoved)
                {
                    MouseUpProcess(e.Location);
                } else
                {
                    if (prevHoverControl!=null)
                    {
                        AudioTrack cur = (AudioTrack)prevHoverControl;
                        if (!HasAncestorInSelections(cur))
                        {
                            if (cur.IsFolder() && cur.IsDrawMidBorder())
                            {
                                foreach (Control ctrl in selectedControls)
                                {
                                    AudioTrack a = (AudioTrack)ctrl;
                                    if (a.GetParentCtrl() != null) a.GetParentCtrl().RemoveChild(a);
                                    a.SetParentCtrl(cur);
                                    cur.AddChildCtrl(a);
                                }
                            }
                            else if (cur != root)
                            {
                                AudioTrack p = cur.GetParentCtrl();
                                AudioTrack pre = cur;
                                foreach (Control ctrl in selectedControls)
                                {
                                    AudioTrack a = (AudioTrack)ctrl;
                                    if (a.GetParentCtrl() != null) a.GetParentCtrl().RemoveChild(a);
                                    a.SetParentCtrl(p);
                                    if (p != null)
                                    {
                                        p.AddChildCtrl(a, cur.IsDrawUpBorder()? cur : pre, cur.IsDrawUpBorder() ? 0 : 1);
                                    }
                                    pre = a;
                                }
                            }
                        }
                        cur.DrawNone();
                        RefreshAll();
                    }
                }
                controlDragStarted = false;
                Capture = false;
                Cursor = Cursors.Default;
                isMoved = false;
            }
            base.OnMouseUp(e);
        }

        private bool HasAncestorInSelections(AudioTrack cur)
        {
            while (cur!= null)
            {
                if (selectedControls.Contains(cur)) return true;
                cur = cur.GetParentCtrl();
            }
            return false;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            RefreshAll();
            base.OnSizeChanged(e);
        }

        public void OnDeleteKeyDown()
        {
            if (selectedControls.Count == 0) return;
            List<AudioTrack> cur = new List<AudioTrack>();
            foreach(Control ctrl in selectedControls) cur.Add((AudioTrack)ctrl);
            HistoryManager.Do(new HistoryManager.Operation(cur, HistoryManager.OperationType.Delete));
        }

        public void RenameTrack()
        {
            if (selectedControls.Count != 1) return;
            ((AudioTrack)selectedControls[0]).EditName();
        }

        public void SelectArrow(int t)
        {
            Control curSel = null;
            if (selectedControls.Count==0)
            {
                if (Controls.Count < 2) return;
                if (t > 0) curSel = Controls[1];
                else curSel = Controls[Controls.Count - 1];
            } else
            {
                int preIndex = Controls.GetChildIndex(selectedControls[selectedControls.Count - 1]) + t;
                if (preIndex == 0) preIndex = 1;// Controls.Count - 1;
                else if (preIndex == Controls.Count) preIndex = Controls.Count - 1;// 1;
                curSel = Controls[preIndex];
                foreach(Control ctrl in selectedControls)
                {
                    ((AudioTrack)ctrl).UnSelected();
                }
                selectedControls.Clear();
            }
            ((AudioTrack)curSel).Selected();
            selectedControls.Add(curSel);
            ScrollContent.GetInstance()?.SetVScrollValue(curSel.Top);
        }

        public void RefreshAll()
        {
            if (Parent != null)
            {
                this.Height = Math.Max(Parent.Height-(int)MainForm.F(10), MainForm.TrackHeight * (root.GetAllChildSize() + 2));
                this.Width = Parent.Width;
            }
            this.SuspendLayout();
            Controls.Clear();
            int offsetY = 0, cur = 0;
            if (root!=null) foreach(AudioTrack ctrl in root.GetAllChild())
            {
//                if (ctrl == root) continue;
                ctrl.Bounds = new Rectangle(ctrl.GetPadding()-(ctrl==root?0:10), offsetY, Width- ctrl.GetPadding() + (ctrl == root ? 0 : 10), MainForm.TrackHeight+(ctrl==root?6:0));
                Controls.Add(ctrl);
                offsetY += MainForm.TrackHeight + (ctrl == root ? 6 : 0);
                if (ctrl != root && !ctrl.IsFolder())
                    {
                        ctrl.SetNum(++cur);
                    }
            }
            this.ResumeLayout(false);
            if (ScrollContent.GetInstance()!= null) ScrollContent.GetInstance().AudioTrackAddOrDelete();
            if (TimeLineContent.GetInstance() != null) TimeLineContent.GetInstance().RefreshWaveForms();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (MainForm.GetInstance().IsCtrlKeyPressed || MainForm.GetInstance().IsShiftKeyPressed) return;
            if (ScrollContent.GetInstance()!= null)
            {
                ScrollContent.GetInstance().MouseWheelEvent(e);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            //MainForm.GetInstance().Text += " TrackView : " + e.KeyData;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
//            base.OnKeyUp(e);
            switch (e.KeyData)
            {
                case Keys.Up:
                    SelectArrow(-1);
                    break;
                case Keys.Down:
                    SelectArrow(1);
                    break;
            }
        }
        public bool IsEditing()
        {
            foreach (Control ctrl in Controls)
            {
                if (ctrl is AudioTrack)
                {
                    AudioTrack a = (AudioTrack)ctrl;
                    if (a.Editing)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (IsEditing()) return false;
            switch (keyData)
            {
                case Keys.M:
                    SetMute();
                    return true;
                case Keys.S:
                    SetSolo();
                    return true;
                case Keys.E:
                    SetEQ();
                    return true;
                case Keys.Up:
                    SelectArrow(-1);
                    return true;
                case Keys.Down:
                    SelectArrow(1);
                    return true;
                case Keys.Delete:
                    OnDeleteKeyDown();
                    return true;
                case Keys.Control | Keys.Z:
                    HistoryManager.Undo();
                    return true;
                case Keys.Control | Keys.Shift | Keys.Z:
                    HistoryManager.Redo();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void SetMute()
        {
            foreach(Control ctrl in selectedControls)
            {
                ((AudioTrack)ctrl).MuteAll();
            }
            UnSetOther();
        }
        public void SetSoloOther()
        {
            int solo_cnt = 0;
            foreach (Control ctrl in Controls)
            {
                if (ctrl is AudioTrack)
                {
                    if (((AudioTrack)ctrl).IsMute)
                    {
//                        ((AudioTrack)ctrl).SetMute(true);
                    }
                    else
                    if (((AudioTrack)ctrl).IsSolo)
                    {
                        //                        ((AudioTrack)ctrl).SetSolo(true);
                        if (ctrl != root) solo_cnt++;
                    } else
                    {
                        ((AudioTrack)ctrl).SetMute(true);
                    }
                }
            }
            if (Controls.Count < 2) return;
            if (solo_cnt > 0) root.SetSolo(true);
            else root.SetMute(true);
        }
        public void UnSetOther()
        {
            int cnt = 0;
            foreach(AudioTrack a in root.GetAllChild())
            {
                if (a == root) continue;
                if (a.IsSolo) cnt++;
            }
            if (cnt==0)
            {
                foreach (AudioTrack a in root.GetAllChild())
                {
                    a.SetMute1();
                }
                root.SetSolo1();
            }
        }
        public void SetSolo()
        {
            foreach (Control ctrl in selectedControls)
            {
                ((AudioTrack)ctrl).SoloAll(((AudioTrack)ctrl).IsSolo == false);
            }
            UnSetOther();
        }

        public void SetEQ()
        {
            if (selectedControls.Count != 1) return;
            if (((AudioTrack)selectedControls[0]).IsFolder()) return;
            ((AudioTrack)selectedControls[0]).SetEQ();
        }
    }
}
