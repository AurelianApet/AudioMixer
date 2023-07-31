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
    public partial class PluginEditorForm : Form
    {
        public PluginEditorForm()
        {
            InitializeComponent();
            KeyPreview = true;
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyData == Keys.Space)
            {
                if (TimeLineContent.GetInstance()!= null)
                {
                    TimeLineContent.GetInstance().PlayOrStop();
                }
            }
        }
    }
}
