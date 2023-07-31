using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace AudioMixer
{
    public partial class MasterTrack : UserControl
    {
        private float[] scaleVal = new float[] { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60};
        private float[] scaleY = new float[] { 149, 227, 306, 384, 462, 528, 586, 637, 681, 716, 744, 769, 788 };
        private double curVal = double.MinValue;
        public double CurVal
        {
            get { return curVal; }
            set
            {
                curVal = value;
//                BlockMax = Math.Max(curVal * coef[Cnt], BlockMax);
            }
        }

        private int cnt = 0;
        public int Cnt
        {
            get { return cnt; }
            set { cnt = value; }
        }

        private double[] coef = new double[1000];

        private double blockMax = double.MinValue;
        public double BlockMax
        {
            get { return blockMax; }
            set { blockMax = value; }
        }
        Timer tm;
        public MasterTrack()
        {
            instance = this;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint, true);
            InitializeComponent();
            InitializeValue();
        }

        private double prelftVal = -70;
        private double prergtVal = -70;
        private double tarLeft = double.MinValue;
        private double tarRight = double.MinValue;
        private Queue<double> leftVals = new Queue<double>();
        private Queue<double> rightVals = new Queue<double>();
        public void AddVal(double lft, double rgt)
        {
            lock(leftVals)
            {
                lock(rightVals)
                {
                    if (lft < 1e-3) lft = -70;
                    else lft = Math.Log10(lft) * 20;
                    if (rgt < 1e-3) rgt = -70;
                    else rgt = Math.Log10(rgt) * 20;
                    double preL = -70;
                    if (prelftVal != double.MinValue) preL = prelftVal;
                    double preR = -70;  
                    if (prergtVal != double.MinValue) preR = prergtVal;
                    double curL = -70;
                    if (lft != double.MinValue) curL = lft;
                    double curR = -70;
                    if (rgt != double.MinValue) curR = rgt;
                    //int c = 4;
                    //for (int i = 1; i<c; i++)
                    //{
                    //    leftVals.Enqueue(preL + (curL - preL) * i / c);
                    //    rightVals.Enqueue(preR + (curR - preR) * i / c);
                    //}
                    //leftVals.Enqueue(lft);
                    //rightVals.Enqueue(rgt);
                    tarLeft = lft;
                    tarRight = rgt;
                }
            }
        }

        private void InitializeValue()
        {
            coef[0] = 1; coef[1] = 1;
            for (int i = 2; i < coef.Length; i++) coef[i] = coef[i - 1] * 0.3;
            BlockMax = double.MinValue;
            tm = new Timer();
            tm.Interval = 20;
            tm.Tick += (sender, e) =>
            {
                Invalidate();
            };
            tm.Start();
        }
        public void Ended()
        {
            lock(leftVals) lock(rightVals)
                {
                    double lftmx = double.MinValue, rgtmx = double.MinValue;
                    while (leftVals.Count > 0) lftmx = Math.Max(lftmx, leftVals.Dequeue());
                    while (rightVals.Count > 0) rgtmx = Math.Max(rgtmx, rightVals.Dequeue());
                    tarLeft = double.MinValue;
                    tarRight = double.MinValue;
                    //double preL = -100;
                    //if (lftmx != double.MinValue) preL = lftmx;
                    //double preR = -100;
                    //if (rgtmx != double.MinValue) preR = rgtmx;
                    //double curL = -100;
                    //double curR = -100;
                    //int c = 100;
                    //for (int i = 1; i < c; i++)
                    //{
                    //    leftVals.Enqueue(preL + (curL - preL) * i / c);
                    //    rightVals.Enqueue(preR + (curR - preR) * i / c);
                    //}
                    //leftVals.Enqueue(double.MinValue);
                    //rightVals.Enqueue(double.MinValue);
                }
        }
        public void Clear()
        {
            lock(leftVals) lock(rightVals)
                {
                    leftVals.Clear();
                    rightVals.Clear();
                    Invalidate();
                }
        }
        double sgn(double x)
        {
            return x < -1e-5 ? -3 : x > 1e-5 ? x * 10 : 0;
        }
        Rectangle border, content, left, right, maxBorder, maxContent;
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            GraphicsPath path = Video.Controls.ColorSlider.CreateRoundRectPath(new Rectangle(1, 1, Width - 3, Height - 3), new Size((int)MainForm.F(15), (int)MainForm.F(15)));
            g.FillPath(new SolidBrush(Color.FromArgb(0xff, 0x43, 0x44, 0x47)), path);
            g.DrawPath(new Pen(MainForm.borderColor, 2.5f), path);
            int H = Size.Height, W = Size.Width;

            int X = (int)MainForm.F(45), Y = (int)MainForm.F(20);
            border = new Rectangle(X - 1, Y - 1, W - (X - 1) * 2, H - 69);
            content = new Rectangle(X, Y, W - X * 2, H - 70);
            left = new Rectangle(X, Y, (W / 2 - X) - 1, content.Height);
            right = new Rectangle(W/2 + 1, Y, (W / 2 - X) - 1, content.Height);

            g.FillRectangle(Brushes.Black, content);

            Color fontColor = Color.FromArgb(0xff, 0xc4, 0xe1, 0xfb);
            Font font = MainForm.GetThinFont(7);
            for (int i=0; i<scaleVal.Length; i++)
            {
                float y = GetY(i);
                SizeF s = g.MeasureString(scaleVal[i].ToString(), font);
                if (scaleVal[i] != 45)
                {
                    g.DrawString(scaleVal[i].ToString(), font, new SolidBrush(fontColor), left.Left - 15 - s.Width / 2, y - s.Height / 2);
                    g.DrawString(scaleVal[i].ToString(), font, new SolidBrush(fontColor), right.Right + 15 - s.Width / 2, y - s.Height / 2);
                }
                if (scaleVal[i] != 45)
                {
                    g.DrawLine(new Pen(Color.White, 1), left.Left - 7, y, left.Left - 1, y);
                    g.DrawLine(new Pen(Color.White, 1), right.Right, y, right.Right + 7, y);
                }
                else
                {
                    g.DrawLine(new Pen(Color.White, 1), left.Left - 3, y, left.Left - 1, y);
                    g.DrawLine(new Pen(Color.White, 1), right.Right, y, right.Right + 3, y);
                }
            }
            for (int i=1; i<5; i++)
            {
                for (int j=1; j<5; j++)
                {
                    float y = (GetY(i - 1) * (5 - j) + GetY(i) * j)/5;
                    g.DrawLine(new Pen(Color.White, 1), left.Left - 6, y, left.Left - 1, y);
                    g.DrawLine(new Pen(Color.White, 1), right.Right, y, right.Right + 6, y);
                }
            }

            //lock(leftVals)
            //{
            //    lock(rightVals)
            //    {
            //        if (leftVals.Count>0)
            //        {
                        double lft = tarLeft;// leftVals.Dequeue();
                        double rgt = tarRight;// rightVals.Count == 0 ? lft : rightVals.Dequeue();
                        BlockMax = Math.Max(BlockMax, lft);
                        BlockMax = Math.Max(BlockMax, rgt);
                        lft = sgn(lft - prelftVal)/20 + prelftVal;
                        rgt = sgn(rgt - prergtVal)/20 + prergtVal;
                        prelftVal = Math.Max(lft, -70);
                        prergtVal = Math.Max(rgt, -70);
                        if (lft>=0)
                        {
                            g.FillRectangle(Brushes.Red, left);
                        } else
                        {
                            lft = -lft;
                            Brush brushl = new LinearGradientBrush(new Point(left.Left, left.Top), new Point(left.Left, left.Bottom), Color.GreenYellow, Color.Green);
                            g.FillRectangle(brushl, left);
                            g.FillRectangle(Brushes.Black, GetLeftRectFromValue(lft));
                        }

                        if (rgt>=0)
                        {
                            g.FillRectangle(Brushes.Red, right);
                        } else
                        {
                            rgt = -rgt;
                            Brush brushr = new LinearGradientBrush(new Point(right.Left, right.Top), new Point(right.Left, right.Bottom), Color.GreenYellow, Color.Green);
                            g.FillRectangle(brushr, right);
                            g.FillRectangle(Brushes.Black, GetRightRectFromValue(rgt));
                        }
            //        } else if (prelftVal>-100) {
            //            double lft = double.MinValue, rgt = double.MinValue;
            //            lft = sgn(lft - prelftVal) / 3 + prelftVal;
            //            rgt = sgn(rgt - prergtVal) / 3 + prergtVal;
            //            prelftVal = lft;
            //            prergtVal = rgt;
            //            if (lft >= 0)
            //            {
            //                g.FillRectangle(Brushes.Red, left);
            //            }
            //            else
            //            {
            //                lft = -lft;
            //                Brush brushl = new LinearGradientBrush(new Point(left.Left, left.Top), new Point(left.Left, left.Bottom), Color.GreenYellow, Color.Green);
            //                g.FillRectangle(brushl, left);
            //                g.FillRectangle(Brushes.Black, GetLeftRectFromValue(lft));
            //            }

            //            if (rgt >= 0)
            //            {
            //                g.FillRectangle(Brushes.Red, right);
            //            }
            //            else
            //            {
            //                rgt = -rgt;
            //                Brush brushr = new LinearGradientBrush(new Point(right.Left, right.Top), new Point(right.Left, right.Bottom), Color.GreenYellow, Color.Green);
            //                g.FillRectangle(brushr, right);
            //                g.FillRectangle(Brushes.Black, GetRightRectFromValue(rgt));
            //            }
            //        }
            //    }
            //}

            g.DrawRectangle(new Pen(Color.DimGray, 2), border);

            maxBorder = new Rectangle(15, H - 40, W - 15 * 2, 32);
            maxContent = new Rectangle(15, H - 40, W - 15 * 2, 32);

            g.FillRectangle(Brushes.Black, maxContent);

            SizeF s1 = g.MeasureString("Peak Max", font);
            g.DrawString("Peak Max", font, Brushes.White, maxContent.Left + maxContent.Width / 2 - s1.Width / 2, maxContent.Top + 10 - s1.Height / 2);

            if (BlockMax == double.MinValue)
            {
                font = MainForm.GetFont(10);
                SizeF s2 = g.MeasureString("-∞", font);
                g.DrawString("-∞", font, Brushes.White, maxContent.Left + maxContent.Width / 2 - s2.Width / 2, maxContent.Bottom - 8 - s2.Height / 2);
            } else
            {
                SizeF s2 = g.MeasureString(BlockMax.ToString("0.00"), font);
                g.DrawString(BlockMax.ToString("0.00"), font, Brushes.White, maxContent.Left + maxContent.Width / 2 - s2.Width / 2, maxContent.Bottom - 8 - s2.Height / 2);
            }

            g.DrawRectangle(new Pen(Color.DimGray, 2), maxBorder);
        }

        float GetY(int i)
        {
            return left.Top + left.Height / (scaleY[scaleY.Length - 1] - scaleY[0]) * (scaleY[i] - scaleY[0]);
        }

        Rectangle GetLeftRectFromValue(double x)
        {
            return new Rectangle(left.Left, left.Top, left.Width, (int)(GetY((float)x) - left.Top));
        }

        Rectangle GetRightRectFromValue(double x)
        {
            return new Rectangle(right.Left, right.Top, right.Width, (int)(GetY((float)x) - right.Top));
        }

        // val is positive
        float GetY(float val)
        {
            for (int i=1; i<scaleVal.Length; i++)
            {
                if (val > scaleVal[i]) continue;
                return (val - scaleVal[i - 1]) / (scaleVal[i] - scaleVal[i - 1]) * left.Height / (scaleY[scaleVal.Length] - scaleY[0]) * (scaleY[i] - scaleY[i - 1]) + GetY(i - 1);
            }
            return left.Bottom;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            //Point p = e.Location;
            //int X = 50, Y = 40;
            //int H = Size.Height, W = Size.Width;
            //Rectangle upperRect = new Rectangle(X, Y, W - X * 2, H - 50 - Y * 2);
            //if (p.X < upperRect.Left) return;
            //if (p.X > upperRect.Right) return;
            //if (p.Y < upperRect.Top) return;
            //if (p.Y > upperRect.Bottom) return;
            //CurVal = 40.0f * (p.Y - upperRect.Top)/(upperRect.Height - 20);
            base.OnMouseDown(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (TimeLineContent.IsInRect(e.Location, maxContent))
            {
                BlockMax = double.MinValue;
                Invalidate();
            }
            base.OnMouseDoubleClick(e);
        }

        public static MasterTrack instance = null;
        public static MasterTrack GetInstance()
        {
            return instance;
        }
    }
}
