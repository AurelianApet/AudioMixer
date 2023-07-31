using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AudioMixer
{
    public enum CheckBoxType
    {
        M, S, E
    }
    public partial class MyCheckBox : UserControl
    {
        private bool isOn = false;
        public bool IsOn
        {
            get { return isOn; }
            set
            {
                bool pre = isOn;
                isOn = value;
                if (pre != isOn) Invalidate();
            }
        }
        int[] TYPE = new int[] { 2, 3, 1 };
        public CheckBoxType type;
        Color[] colors;
        public MyCheckBox()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint, true);
            InitializeValue();
        }
        public void SetType(CheckBoxType t)
        {
            type = t;
            Invalidate();
        }
        public void InitializeValue(CheckBoxType t = CheckBoxType.M)
        {
            isOn = false;
            type = t;
            colors = new Color[6];
            colors[0] = Color.Transparent;
            colors[1] = Color.Transparent;
            colors[2] = Color.Transparent;
            colors[3] = Color.FromArgb(0xff, 0xf1, 0xb2, 0x4e);
            colors[4] = Color.FromArgb(0xff, 0xca, 0x38, 0x38);
            colors[5] = Color.FromArgb(0xff, 0x32, 0x91, 0x37);
        }

        private void Draw(PaintEventArgs pea)
        {
//            if (type == CheckBoxType.E) this.BorderStyle = BorderStyle.None;
//            else 
            this.BorderStyle = BorderStyle.None;
            this.BackColor = Color.Transparent;
            Graphics gra = pea.Graphics;
            Size visBounds = ClientSize;
            Brush brush = new SolidBrush(isOn ? Color.Black : colors[3 + (int)type]);
            Pen pen = new Pen(brush);
            //if (type==CheckBoxType.E)
            //{
            if (isOn) gra.FillPath(new SolidBrush(colors[3 + (int)type]), Video.Controls.ColorSlider.CreateRoundRectPath(new Rectangle(0, 0, Width - 1, Height - 1), new Size(5, 5), TYPE[(int)type]));
            gra.DrawPath(new Pen(Color.White, 1), Video.Controls.ColorSlider.CreateRoundRectPath(new Rectangle(0, 0, Width - 1, Height - 1), new Size(5, 5), TYPE[(int)type]));
            //}
            gra.TranslateTransform(visBounds.Width / 2, visBounds.Height / 2);
            Font font = MainForm.GetBoldFont(10.5f);
            SizeF sz = gra.MeasureString(type.ToString(), font);
            gra.DrawString(type.ToString(), font, brush, -sz.Width/2, -sz.Height/2);
        }

        private void MyCheckBox_Paint(object sender, PaintEventArgs e)
        {
            Draw(e);
        }
    }
}
