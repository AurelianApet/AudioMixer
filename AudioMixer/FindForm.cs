using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace AudioMixer
{
    public partial class FindForm : Form
    {
        Thread findThread = null;
        AudioTrack[] curResult;
        public FindForm()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.UserMouse, true);
            InitializeComponent();
            mSearchName.Height = 20;
            mSearchName.Font = MainForm.GetFont(9);
        }

        private void mSearchName_TextChanged(object sender, EventArgs e)
        {
            if (findThread != null)
            {
                findThread.Abort();
            }
            mSearchResults.Controls.Clear();
            preSelected = null;
            findThread = new Thread(FindTracks);
            findThread.Start(((TextBox)sender).Text);
        }

        private void FindTracks(object obj)
        {
            string findstring = (string)obj;
            if (findstring == "") return;
            TrackView trackView = TrackView.GetInstance();
            if (trackView == null) return;
            AudioTrack[] curRes = trackView.GetAllTracks();
            curResult = curRes;
            for (int i=0; i<curRes.Length; i++)
            {
                string str = curRes[i].GetName();
                if (str.IndexOf(findstring)!=-1)
                {
                    Label a = new Label();
                    a.Font = MainForm.GetFont(9);
                    a.MouseDown += SelectItem;
                    a.MouseDoubleClick += OpenItem;
                    a.Dock = DockStyle.Top;
                    a.Text = str;
                    a.Padding = new Padding(2);
                    a.Height = 20;
                    a.Name = i.ToString();
                    preSelected = a;
                    SelectedItem = curRes[i];
                    AddCandidates(a);
                }
            }
            if (preSelected != null)
            {
                preSelected.BackColor = Color.OrangeRed;
            }
        }

        Label preSelected = null;
        public AudioTrack SelectedItem = null;
        public bool Ok = false;
        private void OpenItem(object sender, MouseEventArgs e)
        {
            Label a = (Label)sender;
            SelectedItem = curResult[int.Parse(a.Name)];
            Ok = true;
            this.Close();
        }

        private void SelectItem(object sender, MouseEventArgs e)
        {
            if (preSelected != null)
            {
                preSelected.BackColor = Color.DarkGray;
            }
            Label a = (Label)sender;
            preSelected = a;
            a.BackColor = Color.OrangeRed;
            SelectedItem = curResult[int.Parse(a.Name)];
        }

        protected void AddCandidates(Control ctrl)
        {
            this.BeginInvoke(new AddCand(AddCandidatesFunc), new object[] { ctrl });
        }
        public delegate void AddCand(Control ctrl);
        public void AddCandidatesFunc(Control ctrl)
        {
            mSearchResults.Controls.Add(ctrl);
        }
        private void mSearchName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                ((TextBox)sender).SelectAll();
                Ok = true;
                this.Close();
            }
        }
    }
}
