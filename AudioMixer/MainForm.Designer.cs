namespace AudioMixer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.mLogoBack = new System.Windows.Forms.ToolStripTextBox();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openAAudioFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.muteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.soloToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.equalizerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.volumeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.RenameTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenameTitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteWaveFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findATrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAAudiotrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomInTrackHeightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutTrackHeightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forwardScrollToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backwardScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleFullScreenModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.playToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pulseStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pulseStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mFileName = new System.Windows.Forms.Label();
            this.mBottomPanel = new AudioMixer.MyPanel();
            this.mSplitterPanel = new AudioMixer.MyPanel();
            this.mMySplitter = new AudioMixer.MySplitter();
            this.mTrackViewPanel = new AudioMixer.MyPanel();
            this.mTrackViewContentPanel = new AudioMixer.MyPanel();
            this.mTrackView = new AudioMixer.TrackView();
            this.mAudioName = new AudioMixer.ShowAndEditName();
            this.mTimeLinePanel = new AudioMixer.MyPanel();
            this.scrollContent1 = new AudioMixer.ScrollContent();
            this.mTimeLineHeader = new AudioMixer.TimeLineHeader();
            this.mMasterTrackPanel = new AudioMixer.MyPanel();
            this.mTopPanel = new AudioMixer.MyPanel();
            this.progressBar = new AudioMixer.MyPanel();
            this.mPlayerController = new AudioMixer.PlayerController();
            this.mPulse = new AudioMixer.Pulse();
            this.mMin = new System.Windows.Forms.PictureBox();
            this.mMax = new System.Windows.Forms.PictureBox();
            this.mClose = new System.Windows.Forms.PictureBox();
            this.mLogo = new AudioMixer.MyPanel();
            this.MainMenu.SuspendLayout();
            this.mBottomPanel.SuspendLayout();
            this.mSplitterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mMySplitter)).BeginInit();
            this.mMySplitter.Panel1.SuspendLayout();
            this.mMySplitter.Panel2.SuspendLayout();
            this.mMySplitter.SuspendLayout();
            this.mTrackViewPanel.SuspendLayout();
            this.mTrackViewContentPanel.SuspendLayout();
            this.mTimeLinePanel.SuspendLayout();
            this.mTopPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mClose)).BeginInit();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.AutoSize = false;
            this.MainMenu.BackColor = System.Drawing.SystemColors.ControlLight;
            this.MainMenu.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenu.GripMargin = new System.Windows.Forms.Padding(0);
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mLogoBack,
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.createToolStripMenuItem,
            this.playToolStripMenuItem,
            this.playToolStripMenuItem1});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Padding = new System.Windows.Forms.Padding(0);
            this.MainMenu.Size = new System.Drawing.Size(1182, 24);
            this.MainMenu.TabIndex = 3;
            this.MainMenu.Text = "menuStrip2";
            // 
            // mLogoBack
            // 
            this.mLogoBack.BackColor = System.Drawing.SystemColors.ControlLight;
            this.mLogoBack.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mLogoBack.Enabled = false;
            this.mLogoBack.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.mLogoBack.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.mLogoBack.Name = "mLogoBack";
            this.mLogoBack.Size = new System.Drawing.Size(40, 24);
            this.mLogoBack.Text = "      ";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveProjectToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.loadProjectToolStripMenuItem,
            this.openAAudioFileToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Padding = new System.Windows.Forms.Padding(0);
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(30, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveProjectToolStripMenuItem.Text = "Save Project";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // loadProjectToolStripMenuItem
            // 
            this.loadProjectToolStripMenuItem.Name = "loadProjectToolStripMenuItem";
            this.loadProjectToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadProjectToolStripMenuItem.Text = "Load Project";
            this.loadProjectToolStripMenuItem.Click += new System.EventHandler(this.loadProjectToolStripMenuItem_Click_1);
            // 
            // openAAudioFileToolStripMenuItem
            // 
            this.openAAudioFileToolStripMenuItem.Name = "openAAudioFileToolStripMenuItem";
            this.openAAudioFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openAAudioFileToolStripMenuItem.Text = "Open a Audio File";
            this.openAAudioFileToolStripMenuItem.Click += new System.EventHandler(this.openAAudioFileToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click_1);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.muteToolStripMenuItem,
            this.soloToolStripMenuItem,
            this.equalizerToolStripMenuItem,
            this.volumeToolStripMenuItem1,
            this.RenameTrackToolStripMenuItem,
            this.RenameTitleToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.findATrackToolStripMenuItem,
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Padding = new System.Windows.Forms.Padding(0);
            this.editToolStripMenuItem.Size = new System.Drawing.Size(35, 24);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // muteToolStripMenuItem
            // 
            this.muteToolStripMenuItem.Name = "muteToolStripMenuItem";
            this.muteToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.muteToolStripMenuItem.Text = "Mute";
            this.muteToolStripMenuItem.Click += new System.EventHandler(this.muteToolStripMenuItem_Click);
            // 
            // soloToolStripMenuItem
            // 
            this.soloToolStripMenuItem.Name = "soloToolStripMenuItem";
            this.soloToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.soloToolStripMenuItem.Text = "Solo";
            this.soloToolStripMenuItem.Click += new System.EventHandler(this.soloToolStripMenuItem_Click);
            // 
            // equalizerToolStripMenuItem
            // 
            this.equalizerToolStripMenuItem.Name = "equalizerToolStripMenuItem";
            this.equalizerToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.equalizerToolStripMenuItem.Text = "Equalizer";
            this.equalizerToolStripMenuItem.Click += new System.EventHandler(this.equalizerToolStripMenuItem_Click);
            // 
            // volumeToolStripMenuItem1
            // 
            this.volumeToolStripMenuItem1.Name = "volumeToolStripMenuItem1";
            this.volumeToolStripMenuItem1.Size = new System.Drawing.Size(159, 22);
            this.volumeToolStripMenuItem1.Text = "Volume";
            this.volumeToolStripMenuItem1.Click += new System.EventHandler(this.volumeToolStripMenuItem1_Click);
            // 
            // RenameTrackToolStripMenuItem
            // 
            this.RenameTrackToolStripMenuItem.Name = "RenameTrackToolStripMenuItem";
            this.RenameTrackToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.RenameTrackToolStripMenuItem.Text = "Rename Track";
            this.RenameTrackToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // RenameTitleToolStripMenuItem
            // 
            this.RenameTitleToolStripMenuItem.Name = "RenameTitleToolStripMenuItem";
            this.RenameTitleToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.RenameTitleToolStripMenuItem.Text = "Rename Title";
            this.RenameTitleToolStripMenuItem.Click += new System.EventHandler(this.volumeToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteTrackToolStripMenuItem,
            this.deleteWaveFormToolStripMenuItem});
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // deleteTrackToolStripMenuItem
            // 
            this.deleteTrackToolStripMenuItem.Name = "deleteTrackToolStripMenuItem";
            this.deleteTrackToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.deleteTrackToolStripMenuItem.Text = "Delete Track";
            this.deleteTrackToolStripMenuItem.Click += new System.EventHandler(this.deleteTrackToolStripMenuItem_Click);
            // 
            // deleteWaveFormToolStripMenuItem
            // 
            this.deleteWaveFormToolStripMenuItem.Name = "deleteWaveFormToolStripMenuItem";
            this.deleteWaveFormToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.deleteWaveFormToolStripMenuItem.Text = "Delete Wave Form";
            this.deleteWaveFormToolStripMenuItem.Click += new System.EventHandler(this.deleteWaveFormToolStripMenuItem_Click);
            // 
            // findATrackToolStripMenuItem
            // 
            this.findATrackToolStripMenuItem.Name = "findATrackToolStripMenuItem";
            this.findATrackToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.findATrackToolStripMenuItem.Text = "Find a Track";
            this.findATrackToolStripMenuItem.Click += new System.EventHandler(this.findATrackToolStripMenuItem_Click);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.undoToolStripMenuItem.Text = "Undo...";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.redoToolStripMenuItem.Text = "Redo..";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // createToolStripMenuItem
            // 
            this.createToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAAudiotrackToolStripMenuItem,
            this.addAFolderToolStripMenuItem});
            this.createToolStripMenuItem.Name = "createToolStripMenuItem";
            this.createToolStripMenuItem.Padding = new System.Windows.Forms.Padding(0);
            this.createToolStripMenuItem.Size = new System.Drawing.Size(51, 24);
            this.createToolStripMenuItem.Text = "Create";
            // 
            // addAAudiotrackToolStripMenuItem
            // 
            this.addAAudiotrackToolStripMenuItem.Name = "addAAudiotrackToolStripMenuItem";
            this.addAAudiotrackToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.addAAudiotrackToolStripMenuItem.Text = "Add a Track";
            this.addAAudiotrackToolStripMenuItem.Click += new System.EventHandler(this.addAAudiotrackToolStripMenuItem_Click);
            // 
            // addAFolderToolStripMenuItem
            // 
            this.addAFolderToolStripMenuItem.Name = "addAFolderToolStripMenuItem";
            this.addAFolderToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.addAFolderToolStripMenuItem.Text = "Add a Folder";
            this.addAFolderToolStripMenuItem.Click += new System.EventHandler(this.addAFolderToolStripMenuItem_Click_1);
            // 
            // playToolStripMenuItem
            // 
            this.playToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomInToolStripMenuItem,
            this.zoomInTrackHeightToolStripMenuItem,
            this.zoomOutToolStripMenuItem,
            this.zoomOutTrackHeightToolStripMenuItem,
            this.forwardScrollToolStripMenuItem,
            this.backwardScreenToolStripMenuItem,
            this.toggleFullScreenModeToolStripMenuItem});
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            this.playToolStripMenuItem.Padding = new System.Windows.Forms.Padding(0);
            this.playToolStripMenuItem.Size = new System.Drawing.Size(40, 24);
            this.playToolStripMenuItem.Text = "View";
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.zoomInToolStripMenuItem.Text = "Zoom In(Track Width)";
            this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.zoomInToolStripMenuItem_Click);
            // 
            // zoomInTrackHeightToolStripMenuItem
            // 
            this.zoomInTrackHeightToolStripMenuItem.Name = "zoomInTrackHeightToolStripMenuItem";
            this.zoomInTrackHeightToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.zoomInTrackHeightToolStripMenuItem.Text = "Zoom In(Track Height)";
            this.zoomInTrackHeightToolStripMenuItem.Click += new System.EventHandler(this.zoomInTrackHeightToolStripMenuItem_Click);
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.zoomOutToolStripMenuItem.Text = "Zoom Out(Track Width)";
            this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.zoomOutToolStripMenuItem_Click);
            // 
            // zoomOutTrackHeightToolStripMenuItem
            // 
            this.zoomOutTrackHeightToolStripMenuItem.Name = "zoomOutTrackHeightToolStripMenuItem";
            this.zoomOutTrackHeightToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.zoomOutTrackHeightToolStripMenuItem.Text = "Zoom Out(Track Height)";
            this.zoomOutTrackHeightToolStripMenuItem.Click += new System.EventHandler(this.zoomOutTrackHeightToolStripMenuItem_Click);
            // 
            // forwardScrollToolStripMenuItem
            // 
            this.forwardScrollToolStripMenuItem.Name = "forwardScrollToolStripMenuItem";
            this.forwardScrollToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.forwardScrollToolStripMenuItem.Text = "Forward Screen";
            this.forwardScrollToolStripMenuItem.Click += new System.EventHandler(this.forwardScrollToolStripMenuItem_Click);
            // 
            // backwardScreenToolStripMenuItem
            // 
            this.backwardScreenToolStripMenuItem.Name = "backwardScreenToolStripMenuItem";
            this.backwardScreenToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.backwardScreenToolStripMenuItem.Text = "Backward Screen";
            this.backwardScreenToolStripMenuItem.Click += new System.EventHandler(this.backwardScreenToolStripMenuItem_Click);
            // 
            // toggleFullScreenModeToolStripMenuItem
            // 
            this.toggleFullScreenModeToolStripMenuItem.Name = "toggleFullScreenModeToolStripMenuItem";
            this.toggleFullScreenModeToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.toggleFullScreenModeToolStripMenuItem.Text = "Toggle Full Screen Mode";
            this.toggleFullScreenModeToolStripMenuItem.Click += new System.EventHandler(this.toggleFullScreenModeToolStripMenuItem_Click);
            // 
            // playToolStripMenuItem1
            // 
            this.playToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playToolStripMenuItem2,
            this.stopToolStripMenuItem,
            this.pulseStartToolStripMenuItem,
            this.pulseStopToolStripMenuItem});
            this.playToolStripMenuItem1.Name = "playToolStripMenuItem1";
            this.playToolStripMenuItem1.Padding = new System.Windows.Forms.Padding(0);
            this.playToolStripMenuItem1.Size = new System.Drawing.Size(36, 24);
            this.playToolStripMenuItem1.Text = "Play";
            // 
            // playToolStripMenuItem2
            // 
            this.playToolStripMenuItem2.Name = "playToolStripMenuItem2";
            this.playToolStripMenuItem2.Size = new System.Drawing.Size(129, 22);
            this.playToolStripMenuItem2.Text = "Play";
            this.playToolStripMenuItem2.Click += new System.EventHandler(this.playToolStripMenuItem2_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // pulseStartToolStripMenuItem
            // 
            this.pulseStartToolStripMenuItem.Name = "pulseStartToolStripMenuItem";
            this.pulseStartToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.pulseStartToolStripMenuItem.Text = "Pulse On";
            this.pulseStartToolStripMenuItem.Click += new System.EventHandler(this.pulseStartToolStripMenuItem_Click);
            // 
            // pulseStopToolStripMenuItem
            // 
            this.pulseStopToolStripMenuItem.Name = "pulseStopToolStripMenuItem";
            this.pulseStopToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.pulseStopToolStripMenuItem.Text = "Pulse Off";
            this.pulseStopToolStripMenuItem.Click += new System.EventHandler(this.pulseStopToolStripMenuItem_Click);
            // 
            // mFileName
            // 
            this.mFileName.AutoSize = true;
            this.mFileName.Location = new System.Drawing.Point(246, 3);
            this.mFileName.Name = "mFileName";
            this.mFileName.Size = new System.Drawing.Size(59, 13);
            this.mFileName.TabIndex = 10;
            this.mFileName.Text = "EJA Studio";
            // 
            // mBottomPanel
            // 
            this.mBottomPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(37)))), ((int)(((byte)(39)))), ((int)(((byte)(42)))));
            this.mBottomPanel.Controls.Add(this.mSplitterPanel);
            this.mBottomPanel.Controls.Add(this.mMasterTrackPanel);
            this.mBottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mBottomPanel.IsProgressBar = false;
            this.mBottomPanel.Location = new System.Drawing.Point(0, 70);
            this.mBottomPanel.Name = "mBottomPanel";
            this.mBottomPanel.Size = new System.Drawing.Size(1182, 530);
            this.mBottomPanel.TabIndex = 5;
            this.mBottomPanel.Value = 0;
            // 
            // mSplitterPanel
            // 
            this.mSplitterPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.mSplitterPanel.Controls.Add(this.mMySplitter);
            this.mSplitterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mSplitterPanel.IsProgressBar = false;
            this.mSplitterPanel.Location = new System.Drawing.Point(0, 0);
            this.mSplitterPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mSplitterPanel.Name = "mSplitterPanel";
            this.mSplitterPanel.Padding = new System.Windows.Forms.Padding(20, 10, 10, 20);
            this.mSplitterPanel.Size = new System.Drawing.Size(1006, 530);
            this.mSplitterPanel.TabIndex = 1;
            this.mSplitterPanel.Value = 0;
            // 
            // mMySplitter
            // 
            this.mMySplitter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.mMySplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMySplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.mMySplitter.Location = new System.Drawing.Point(20, 10);
            this.mMySplitter.Name = "mMySplitter";
            // 
            // mMySplitter.Panel1
            // 
            this.mMySplitter.Panel1.Controls.Add(this.mTrackViewPanel);
            this.mMySplitter.Panel1.Controls.Add(this.mAudioName);
            this.mMySplitter.Panel1MinSize = 220;
            // 
            // mMySplitter.Panel2
            // 
            this.mMySplitter.Panel2.Controls.Add(this.mTimeLinePanel);
            this.mMySplitter.Panel2MinSize = 220;
            this.mMySplitter.Size = new System.Drawing.Size(976, 500);
            this.mMySplitter.SplitterDistance = 400;
            this.mMySplitter.TabIndex = 0;
            // 
            // mTrackViewPanel
            // 
            this.mTrackViewPanel.Controls.Add(this.mTrackViewContentPanel);
            this.mTrackViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTrackViewPanel.IsProgressBar = false;
            this.mTrackViewPanel.Location = new System.Drawing.Point(0, 30);
            this.mTrackViewPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mTrackViewPanel.Name = "mTrackViewPanel";
            this.mTrackViewPanel.Padding = new System.Windows.Forms.Padding(5);
            this.mTrackViewPanel.Size = new System.Drawing.Size(400, 470);
            this.mTrackViewPanel.TabIndex = 1;
            this.mTrackViewPanel.Value = 0;
            // 
            // mTrackViewContentPanel
            // 
            this.mTrackViewContentPanel.Controls.Add(this.mTrackView);
            this.mTrackViewContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTrackViewContentPanel.IsProgressBar = false;
            this.mTrackViewContentPanel.Location = new System.Drawing.Point(5, 5);
            this.mTrackViewContentPanel.Name = "mTrackViewContentPanel";
            this.mTrackViewContentPanel.Size = new System.Drawing.Size(390, 460);
            this.mTrackViewContentPanel.TabIndex = 0;
            this.mTrackViewContentPanel.Value = 0;
            // 
            // mTrackView
            // 
            this.mTrackView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(37)))), ((int)(((byte)(39)))), ((int)(((byte)(42)))));
            this.mTrackView.Location = new System.Drawing.Point(0, 0);
            this.mTrackView.Name = "mTrackView";
            this.mTrackView.Padding = new System.Windows.Forms.Padding(3);
            this.mTrackView.Size = new System.Drawing.Size(378, 448);
            this.mTrackView.TabIndex = 0;
            // 
            // mAudioName
            // 
            this.mAudioName.AutoSize = true;
            this.mAudioName.Dock = System.Windows.Forms.DockStyle.Top;
            this.mAudioName.Location = new System.Drawing.Point(0, 0);
            this.mAudioName.Margin = new System.Windows.Forms.Padding(0);
            this.mAudioName.MinimumSize = new System.Drawing.Size(0, 30);
            this.mAudioName.Name = "mAudioName";
            this.mAudioName.Padding = new System.Windows.Forms.Padding(1);
            this.mAudioName.Size = new System.Drawing.Size(400, 30);
            this.mAudioName.TabIndex = 0;
            // 
            // mTimeLinePanel
            // 
            this.mTimeLinePanel.Controls.Add(this.scrollContent1);
            this.mTimeLinePanel.Controls.Add(this.mTimeLineHeader);
            this.mTimeLinePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTimeLinePanel.IsProgressBar = false;
            this.mTimeLinePanel.Location = new System.Drawing.Point(0, 0);
            this.mTimeLinePanel.Name = "mTimeLinePanel";
            this.mTimeLinePanel.Padding = new System.Windows.Forms.Padding(5);
            this.mTimeLinePanel.Size = new System.Drawing.Size(572, 500);
            this.mTimeLinePanel.TabIndex = 0;
            this.mTimeLinePanel.Value = 0;
            // 
            // scrollContent1
            // 
            this.scrollContent1.AllowDrop = true;
            this.scrollContent1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(38)))), ((int)(((byte)(39)))), ((int)(((byte)(42)))));
            this.scrollContent1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollContent1.Location = new System.Drawing.Point(5, 55);
            this.scrollContent1.Name = "scrollContent1";
            this.scrollContent1.Size = new System.Drawing.Size(562, 440);
            this.scrollContent1.TabIndex = 1;
            // 
            // mTimeLineHeader
            // 
            this.mTimeLineHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.mTimeLineHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.mTimeLineHeader.Font = new System.Drawing.Font("Noto Sans KR", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTimeLineHeader.Location = new System.Drawing.Point(5, 5);
            this.mTimeLineHeader.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.mTimeLineHeader.Name = "mTimeLineHeader";
            this.mTimeLineHeader.Size = new System.Drawing.Size(562, 50);
            this.mTimeLineHeader.TabIndex = 0;
            // 
            // mMasterTrackPanel
            // 
            this.mMasterTrackPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.mMasterTrackPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.mMasterTrackPanel.IsProgressBar = false;
            this.mMasterTrackPanel.Location = new System.Drawing.Point(1006, 0);
            this.mMasterTrackPanel.Name = "mMasterTrackPanel";
            this.mMasterTrackPanel.Padding = new System.Windows.Forms.Padding(3, 5, 23, 20);
            this.mMasterTrackPanel.Size = new System.Drawing.Size(176, 530);
            this.mMasterTrackPanel.TabIndex = 0;
            this.mMasterTrackPanel.Value = 0;
            // 
            // mTopPanel
            // 
            this.mTopPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(37)))), ((int)(((byte)(39)))), ((int)(((byte)(42)))));
            this.mTopPanel.Controls.Add(this.progressBar);
            this.mTopPanel.Controls.Add(this.mPlayerController);
            this.mTopPanel.Controls.Add(this.mPulse);
            this.mTopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.mTopPanel.IsProgressBar = false;
            this.mTopPanel.Location = new System.Drawing.Point(0, 24);
            this.mTopPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mTopPanel.Name = "mTopPanel";
            this.mTopPanel.Padding = new System.Windows.Forms.Padding(20, 10, 22, 5);
            this.mTopPanel.Size = new System.Drawing.Size(1182, 46);
            this.mTopPanel.TabIndex = 4;
            this.mTopPanel.Value = 0;
            // 
            // progressBar
            // 
            this.progressBar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.progressBar.IsProgressBar = false;
            this.progressBar.Location = new System.Drawing.Point(1130, 10);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(30, 31);
            this.progressBar.TabIndex = 3;
            this.progressBar.Value = 0;
            // 
            // mPlayerController
            // 
            this.mPlayerController.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.mPlayerController.Location = new System.Drawing.Point(20, 8);
            this.mPlayerController.Margin = new System.Windows.Forms.Padding(0);
            this.mPlayerController.Name = "mPlayerController";
            this.mPlayerController.Size = new System.Drawing.Size(206, 30);
            this.mPlayerController.TabIndex = 2;
            // 
            // mPulse
            // 
            this.mPulse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.mPulse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mPulse.IsActive = false;
            this.mPulse.Location = new System.Drawing.Point(250, 8);
            this.mPulse.Margin = new System.Windows.Forms.Padding(0);
            this.mPulse.Name = "mPulse";
            this.mPulse.Pcnt = 4;
            this.mPulse.Ppm = 120;
            this.mPulse.Size = new System.Drawing.Size(200, 30);
            this.mPulse.TabIndex = 0;
            // 
            // mMin
            // 
            this.mMin.BackColor = System.Drawing.SystemColors.ControlLight;
            this.mMin.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mMin.BackgroundImage")));
            this.mMin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mMin.Location = new System.Drawing.Point(1107, 2);
            this.mMin.Name = "mMin";
            this.mMin.Size = new System.Drawing.Size(21, 21);
            this.mMin.TabIndex = 9;
            this.mMin.TabStop = false;
            // 
            // mMax
            // 
            this.mMax.BackColor = System.Drawing.SystemColors.ControlLight;
            this.mMax.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mMax.BackgroundImage")));
            this.mMax.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mMax.Location = new System.Drawing.Point(1130, 2);
            this.mMax.Name = "mMax";
            this.mMax.Size = new System.Drawing.Size(21, 21);
            this.mMax.TabIndex = 8;
            this.mMax.TabStop = false;
            // 
            // mClose
            // 
            this.mClose.BackColor = System.Drawing.SystemColors.ControlLight;
            this.mClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mClose.BackgroundImage")));
            this.mClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mClose.Location = new System.Drawing.Point(1153, 2);
            this.mClose.Name = "mClose";
            this.mClose.Size = new System.Drawing.Size(21, 21);
            this.mClose.TabIndex = 7;
            this.mClose.TabStop = false;
            // 
            // mLogo
            // 
            this.mLogo.BackColor = System.Drawing.SystemColors.ControlLight;
            this.mLogo.BackgroundImage = global::AudioMixer.Properties.Resources.logo;
            this.mLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mLogo.IsProgressBar = false;
            this.mLogo.Location = new System.Drawing.Point(7, 4);
            this.mLogo.Name = "mLogo";
            this.mLogo.Size = new System.Drawing.Size(40, 16);
            this.mLogo.TabIndex = 6;
            this.mLogo.Value = 0;
            // 
            // MainForm
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1182, 600);
            this.Controls.Add(this.mFileName);
            this.Controls.Add(this.mMin);
            this.Controls.Add(this.mMax);
            this.Controls.Add(this.mClose);
            this.Controls.Add(this.mLogo);
            this.Controls.Add(this.mBottomPanel);
            this.Controls.Add(this.mTopPanel);
            this.Controls.Add(this.MainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.KeyPreview = true;
            this.MainMenuStrip = this.MainMenu;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Audio Mixer";
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.mBottomPanel.ResumeLayout(false);
            this.mSplitterPanel.ResumeLayout(false);
            this.mMySplitter.Panel1.ResumeLayout(false);
            this.mMySplitter.Panel1.PerformLayout();
            this.mMySplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mMySplitter)).EndInit();
            this.mMySplitter.ResumeLayout(false);
            this.mTrackViewPanel.ResumeLayout(false);
            this.mTrackViewContentPanel.ResumeLayout(false);
            this.mTimeLinePanel.ResumeLayout(false);
            this.mTopPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mClose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem muteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem soloToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem equalizerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem volumeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem RenameTrackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RenameTitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findATrackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addAAudiotrackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addAFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forwardScrollToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backwardScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pulseStartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pulseStopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteTrackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteWaveFormToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openAAudioFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomInTrackHeightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomOutTrackHeightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleFullScreenModeToolStripMenuItem;
        private AudioMixer.MyPanel mTopPanel;
        private AudioMixer.MyPanel mBottomPanel;
        private Pulse mPulse;
        private AudioMixer.MyPanel mMasterTrackPanel;
        private AudioMixer.MyPanel mSplitterPanel;
        private MySplitter mMySplitter;
        private ShowAndEditName mAudioName;
        private AudioMixer.MyPanel mTrackViewPanel;
        private AudioMixer.MyPanel mTimeLinePanel;
        private TimeLineHeader mTimeLineHeader;
        private ScrollContent scrollContent1;
        private AudioMixer.MyPanel mTrackViewContentPanel;
        private TrackView mTrackView;
        private PlayerController mPlayerController;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox mLogoBack;
        private AudioMixer.MyPanel mLogo;
        private System.Windows.Forms.PictureBox mClose;
        private System.Windows.Forms.PictureBox mMax;
        private System.Windows.Forms.PictureBox mMin;
        private System.Windows.Forms.ToolStripMenuItem loadProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.Label mFileName;
        private MyPanel progressBar;
    }
}

