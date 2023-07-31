using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioMixer
{
    public partial class SerialWindow : Form
    {
        public SerialWindow()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            label1.Font = key.Font = MainForm.GetFont(10);
            MyMessageBox.MyButton okbtn = new MyMessageBox.MyButton();
            Controls.Add(okbtn);
            okbtn.Location = new Point(150, 100);
            okbtn.Size = new Size(100, 30);
            okbtn.BackColor = Color.Transparent;
            okbtn.TextAlign = ContentAlignment.MiddleCenter;
            okbtn.ForeColor = Color.Black;
            okbtn.Font = MainForm.GetFont(10);
            okbtn.Text = "Verify";
            okbtn.Click += (sender, e) =>
            {
                check();
            };
            key.KeyDown += (sender, e) =>
            {
                if (e.KeyData == Keys.Return)
                {
                    check();
                }
            };
        }
        public void check()
        {
            if (key.Text == "ok")
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MyMessageBox a = new MyMessageBox();
                a.SetText("Wrong Key!");
                a.ShowDialog();
                key.Text = "";
            }
        }
    }
}
