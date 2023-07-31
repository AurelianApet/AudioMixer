using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AudioMixer
{
    public partial class ExitWindow : Form
    {
        public DialogResult result;
        public ExitWindow()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            InitializeComponent();
            Init();
        }

        void Init()
        {
            AutoScaleMode = AutoScaleMode.None;
            Font = MainForm.GetFont(10);
            label1.Font = MainForm.GetFont(10);
            result = DialogResult.Cancel;
            MyMessageBox.MyButton saveBtn = new MyMessageBox.MyButton();
            Controls.Add(saveBtn);
            saveBtn.Location = new Point(40, 100);
            saveBtn.Size = new Size(100, 30);
            saveBtn.BackColor = Color.Transparent;
            saveBtn.TextAlign = ContentAlignment.MiddleCenter;
            saveBtn.ForeColor = Color.Black;
            saveBtn.Font = MainForm.GetFont(10);
            saveBtn.Text = "Save";
            saveBtn.Click += (sender, e) =>
            {
                result = DialogResult.Yes;
                Close();
            };
            MyMessageBox.MyButton nosaveBtn = new MyMessageBox.MyButton();
            Controls.Add(nosaveBtn);
            nosaveBtn.Location = new Point(150, 100);
            nosaveBtn.Size = new Size(100, 30);
            nosaveBtn.BackColor = Color.Transparent;
            nosaveBtn.TextAlign = ContentAlignment.MiddleCenter;
            nosaveBtn.ForeColor = Color.Black;
            nosaveBtn.Font = MainForm.GetFont(10);
            nosaveBtn.Text = "Don't Save";
            nosaveBtn.Click += (sender, e) =>
            {
                result = DialogResult.No;
                Close();
            };
            MyMessageBox.MyButton cancelBtn = new MyMessageBox.MyButton();
            Controls.Add(cancelBtn);
            cancelBtn.Location = new Point(260, 100);
            cancelBtn.Size = new Size(100, 30);
            cancelBtn.BackColor = Color.Transparent;
            cancelBtn.TextAlign = ContentAlignment.MiddleCenter;
            cancelBtn.ForeColor = Color.Black;
            cancelBtn.Font = MainForm.GetFont(10);
            cancelBtn.Text = "Cancel";
            cancelBtn.Click += (sender, e) =>
            {
                result = DialogResult.Cancel;
                Close();
            };
        }
    }
}
