using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioMixer
{
    class MyPanel : Panel
    {
        public MyPanel() : base()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
        }
        int value = 0;
        public int Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }
        bool isProgressBar = false;
        public bool IsProgressBar
        {
            get { return isProgressBar; }
            set
            {
                isProgressBar = value;
            }
        }
        public void Start()
        {
            if (!IsProgressBar) return;
            BackgroundImageLayout = ImageLayout.Stretch;
            Timer tm = new Timer();
            tm.Interval = 20;
            int cur = 0;
            System.Drawing.Bitmap[] images = new System.Drawing.Bitmap[] {
                Properties.Resources.pointer_wait_0,
                Properties.Resources.pointer_wait_3,
                Properties.Resources.pointer_wait_6,
                Properties.Resources.pointer_wait_9,
                Properties.Resources.pointer_wait_12,
                Properties.Resources.pointer_wait_15,
                Properties.Resources.pointer_wait_18,
                Properties.Resources.pointer_wait_21,
                Properties.Resources.pointer_wait_24,
                Properties.Resources.pointer_wait_27,
                Properties.Resources.pointer_wait_30,
                Properties.Resources.pointer_wait_33,
            };
            tm.Tick += (sender, e) =>
            {
                BackgroundImage = images[cur % images.Length];
                cur++;
                cur %= images.Length;
            };
            tm.Start();
        }
    }
}
