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
    public partial class MyEqualizer : Form
    {
        Panel dllPanel;
        Panel graphPanel;
        Panel bandItemPanel;
        EQProperty property;
        GraphPanel graph;
        LHCut lhcut;
        BandItem[] bandItems;

        public MyEqualizer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);
            InitializeComponent();
        }

        public void Init(EQProperty property)
        {
            SuspendLayout();
            Controls.Clear();
            this.property = property;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            BackColor = Color.FromArgb(255, 37, 39, 41);
            Size = new Size(716, 450);

            dllPanel = new Panel();
            dllPanel.Location = new Point(0, 0);
            dllPanel.Padding = new Padding(10);
            dllPanel.Size = new Size(200, 412);
            dllPanel.BackColor = Color.FromArgb(255, 42, 42, 42);
            dllPanel.BorderStyle = BorderStyle.FixedSingle;
            dllPanel.MouseDown += (sender, e) => { dllPanel.Focus(); };
            for (int i=0; i<13; i++)
            {
                dllButton a = new dllButton(property, i);
                a.Dock = DockStyle.Top;
                a.Height = 30;
                dllPanel.Controls.Add(a);
            }
            FormClosed += (sender, e) =>
            {
                for (int i=0; i<13; i++)
                {
                    (dllPanel.Controls[i] as dllButton).IsEq = false;
                }
            };

            graphPanel = new Panel();
            graphPanel.Location = new Point(200, 0);
            graphPanel.Size = new Size(500, 270);
            graphPanel.BorderStyle = BorderStyle.FixedSingle;
            graph = new GraphPanel();
            graph.Size = new Size(500, 266);
            graphPanel.Controls.Add(graph);
            graph.Init(this.property);
            graphPanel.MouseDown += (sender, e) => { graphPanel.Focus(); };

            bandItemPanel = new Panel();
            bandItemPanel.Size = new Size(500, 142);
            bandItemPanel.Location = new Point(200, 270);
            bandItemPanel.BorderStyle = BorderStyle.FixedSingle;
            bandItemPanel.MouseDown += (sender, e) => { bandItemPanel.Focus(); };

            lhcut = new LHCut();
            lhcut.Size = new Size(80, 104);
            lhcut.Location = new Point(30, 19);
            lhcut.Init(property);
            lhcut.ValueChanged += (sender, e) =>
            {
                graphPanel.Invalidate();
            };

            bandItems = new BandItem[4];
            for (int i=0; i<4; i++)
            {
                bandItems[i] = new BandItem();
                bandItems[i].Init(this.property.handleItem[i]);
                bandItems[i].Location = new Point(120 + 90 * i, 19);
                bandItems[i].ActiveChange += (sender, e) =>
                {
                    BandItem a = (BandItem)sender;
                    graph.handles[a.bandHandle.type].IsActive = a.IsActive;
                    graph.Invalidate();
                };
                bandItems[i].DesibelChange += (sender, e)=> {
                    BandItem a = (BandItem)sender;
                    graph.handles[a.bandHandle.type].DB = bandItems[a.bandHandle.type].Desibel;
                    graph.Invalidate();
                };
                bandItems[i].FrequencyChange += (sender, e) => {
                    BandItem a = (BandItem)sender;
                    graph.handles[a.bandHandle.type].Frequency = bandItems[a.bandHandle.type].Frequency;
                    graph.Invalidate();
                };
                bandItems[i].FactorChange += (sender, e) => {
                    BandItem a = (BandItem)sender;
                    graph.handles[a.bandHandle.type].Factor = bandItems[a.bandHandle.type].Factor;
                    graph.Invalidate();
                };
                graph.handles[i].DBChanged += (sender, e) =>
                {
                    BandHandle a = (BandHandle)sender;
                    bandItems[a.id].Desibel = a.DB;
                    graph.Invalidate();
                };
                graph.handles[i].FrequencyChanged += (sender, e) =>
                {
                    BandHandle a = (BandHandle)sender;
                    bandItems[a.id].Frequency = a.Frequency;
                    graph.Invalidate();
                };
                graph.handles[i].FactorChanged += (sender, e) =>
                {
                    BandHandle a = (BandHandle)sender;
                    bandItems[a.id].Factor = a.Factor;
                    graph.Invalidate();
                };
            }

            bandItemPanel.Controls.Add(lhcut);
            for (int i=0; i<4; i++)
            {
                bandItemPanel.Controls.Add(bandItems[i]);
            }

            Controls.Add(dllPanel);
            Controls.Add(graphPanel);
            Controls.Add(bandItemPanel);
            ResumeLayout();
        }
    }
}
