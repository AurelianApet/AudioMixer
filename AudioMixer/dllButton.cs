using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Vst;

namespace AudioMixer
{
    public partial class dllButton : UserControl
    {
        EQProperty eq;
        private int id = 0;
        public int ID
        {
            get { return id; }
            set { id = value; Invalidate(); }
        }
        public bool IsActive
        {
            get { return eq.dllActive[ID]; }
            set
            {
                if (eq.dllActive[ID] != value)
                {
                    MainForm.isChanged = true;
                }
                eq.dllActive[ID] = value;
                Invalidate();
            }
        }

        Font font1 = MainForm.GetFont(15), font2 = MainForm.GetFont(7f);
        public string Filepath
        {
            get { return eq.eqDll[ID]; }
            set
            {
                if (value == "")
                {
                    if (eq.eqDll[ID] != "")
                    {
                        eq.HideDllGui(ID);
                        eq.Close(ID);
                    }
                }
                if (eq.eqDll[ID] != value)
                {
                    MainForm.isChanged = true;
                }
                eq.eqDll[ID] = value;
                iseq = true;
                IsActive = value != "";
                if (value != "")
                {
                    font1 = MainForm.GetBoldFont(17);
                    font2 = MainForm.GetThinFont(8.25f);
                }
                Invalidate();
                eq.ReLoad(ID, this);
            }
        }

        private bool iseq = false;
        public bool IsEq
        {
            get { return iseq; }
            set
            {
                if (iseq != value)
                {
                    MainForm.isChanged = true;
                }
                iseq = value; Invalidate();
                if (value) eq.ShowDllGui(ID, this);
                else eq.HideDllGui(ID);
            }
        }
        public dllButton(EQProperty eq, int id)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            InitializeComponent();
            Init(eq, id);
        }

        public void Init(EQProperty eq, int id = 0)
        {
            this.ID = id;
            this.eq = eq;
            BackColor = Color.Transparent;
            iseq = false;
        }

        Rectangle border, ActiveRect, EqRect, LoadRect, DelRect;
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            border = new Rectangle(10, 4, Width - 20, Height - 8);
            ActiveRect = new Rectangle(14, 7, 16, 16);
            EqRect = new Rectangle(34, 7, 18, 16);
            LoadRect = new Rectangle(56, 7, border.Right - 56 - 18, 16);
            DelRect = new Rectangle(border.Right - 15, 10, 10, 10);
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(Color.FromArgb(255, 52, 52, 60)), border);
            g.DrawRectangle(new Pen(Color.FromArgb(100, Color.Gray), 2), border);

            if (Filepath != "")
            {
                if (IsActive)
                {
                    g.FillRectangle(Brushes.Gray, ActiveRect);
                    g.FillPie(Brushes.Black, new Rectangle(ActiveRect.Left + 2, ActiveRect.Top + 2, ActiveRect.Width - 4, ActiveRect.Height - 4), -60, 300);
                }
                else
                {
                    g.FillPie(Brushes.Gray, new Rectangle(ActiveRect.Left + 2, ActiveRect.Top + 2, ActiveRect.Width - 4, ActiveRect.Height - 4), -60, 300);
                }
                if (IsEq)
                {
                    g.FillEllipse(new SolidBrush(Color.FromArgb(100, Color.Cyan)), EqRect);
                    SizeF s = g.MeasureString("e", font1);
                    g.DrawString("e", font1, Brushes.White, EqRect.Left + EqRect.Width * 0.5f - s.Width * 0.5f+0.5f, EqRect.Top + EqRect.Height * 0.5f - s.Height * 0.5f - 1.5f);
                }
                else
                {
                    g.FillEllipse(new SolidBrush(Color.FromArgb(100, Color.Gray)), EqRect);
                    SizeF s = g.MeasureString("e", font1);
                    g.DrawString("e", font1, Brushes.Black, EqRect.Left + EqRect.Width * 0.5f - s.Width * 0.5f+0.5f, EqRect.Top + EqRect.Height * 0.5f - s.Height * 0.5f - 1.5f);
                }
                string str;
                GetStringAndSize(GetFileName(Filepath), g, font2, LoadRect.Width, out str);
                g.DrawString(str, font2, Brushes.White, LoadRect.Left + 2, LoadRect.Top + LoadRect.Height * 0.5f - font2.Height * 0.5f);

                g.DrawLine(new Pen(Color.Gray, 2), DelRect.Left, DelRect.Top, DelRect.Right, DelRect.Bottom);
                g.DrawLine(new Pen(Color.Gray, 2), DelRect.Left, DelRect.Bottom, DelRect.Right, DelRect.Top);
            }

            if (IsActive)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.LightBlue)), border);
            }
        }
        public static void GetStringAndSize(string str, Graphics g, Font font, float width, out string result)
        {
            SizeF s = g.MeasureString(str, font);
            if (s.Width <= width)
            {
                result = str;
            }
            else
            {
                while (g.MeasureString(str + "...", font).Width > width) str = str.Substring(0, str.Length - 1);
                result = str + "...";
            }
        }
        public static string GetFileName(string path)
        {
            string ans = "";
            int t = path.LastIndexOf('\\');
            if (t < 0 || t >= path.Length) t = path.LastIndexOf('/');
            if (t >= 0 && t < path.Length)
            {
                for (int i = t + 1; i < path.Length; i++) ans += path[i];
            }
            else ans = path;
            return ans;
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            Cursor = Cursors.Hand;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (Filepath != "" && TimeLineContent.IsInRect(e.Location, ActiveRect))
            {
                IsActive = !IsActive;
            }
            if (TimeLineContent.IsInRect(e.Location, LoadRect))
            {
                OpenFileDialog of = new OpenFileDialog();
                of.Filter = "Dll files (*.dll)|*.dll";
                if (of.ShowDialog() == DialogResult.OK)
                {
                    if (IsDllPlugin(of.FileName)) Filepath = of.FileName;
                }
            }
            if (Filepath != "" && TimeLineContent.IsInRect(e.Location, EqRect))
            {
                IsEq = !IsEq;
            }
            if (Filepath != "" && TimeLineContent.IsInRect(e.Location, DelRect))
            {
                Filepath = "";
                IsActive = false;
            }
        }

        bool IsDllPlugin(string path)
        {

            try
            {
                FileInfo info = new FileInfo(path);
                if (info.Length == 84796232) return false;
                Vst.PluginItem a = new Vst.PluginItem(path);
                return a.isValid();
            }
            catch
            {
                return false;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Cursor = Cursors.Default;
            Invalidate();
        }
    }
}
