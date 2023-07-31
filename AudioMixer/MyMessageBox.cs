using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace AudioMixer
{
    public partial class MyMessageBox : Form
    {
        public class MyLabel : Label
        {
            public MyLabel() : base()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                         ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                         ControlStyles.UserPaint, true);
            }
        }
        public class MyButton : Button
        {
            public MyButton() : base()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                         ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                         ControlStyles.UserPaint, true);
                Font = MainForm.GetFont(8f);
            }
        }

        MyLabel content;
        MyButton okBtn;
        public MyMessageBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            InitializeComponent();
            Init();
            it = this;
        }

        public void Init()
        {
            Text = "\u26a0Warnning";
            Padding = new Padding(0);
            BackColor = Color.FromKnownColor(KnownColor.ControlDark);
            content = new MyLabel();
            Size = new Size(400, 200);
            Controls.Add(content);
            content.Size = new Size(250, 70);
            content.Location = new Point(65, 20);
            content.TextAlign = ContentAlignment.MiddleCenter;
            content.Font = MainForm.GetFont(11);
            content.BackColor = Color.Transparent;
            content.ForeColor = Color.Black;

            okBtn = new MyButton();
            Controls.Add(okBtn);
            okBtn.Size = new Size(100, 30);
            okBtn.BackColor = Color.Transparent;
            okBtn.Location = new Point(Width/2-okBtn.Width/2-10, content.Bottom + 10);
            okBtn.TextAlign = ContentAlignment.MiddleCenter;
            okBtn.ForeColor = Color.Black;
            okBtn.Font = MainForm.GetFont(10);
            okBtn.Text = "OK";
            okBtn.Click += (sender, e) =>
            {
                Close();
            };
        }

        public void SetText(string str)
        {
            content.Text = str;
        }
        public static MyMessageBox it;
        public static void Show(string str)
        {
            MainForm.GetInstance().ShowMessage(str);
        }
        public static DialogResult Show1(Form f, string str)
        {
            MyMessageBox a = new MyMessageBox();
            a.SetText(str);
            return a.ShowDialog(f);
        }
    }
}
