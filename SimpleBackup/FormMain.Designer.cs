namespace SimpleBackup
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.noIco = new System.Windows.Forms.NotifyIcon(this.components);
            this.msTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.btRunNow = new System.Windows.Forms.Button();
            this.btQuit = new System.Windows.Forms.Button();
            this.lvBackups = new System.Windows.Forms.ListView();
            this.colBackupName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTimeTaken = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colNextBackup = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imIcons = new System.Windows.Forms.ImageList(this.components);
            this.msMain = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddBackup = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditBackup = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRunNow = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDeleteBackup = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.pbProgress = new System.Windows.Forms.PictureBox();
            this.tsMain = new System.Windows.Forms.ToolStrip();
            this.tsbAddBackup = new System.Windows.Forms.ToolStripButton();
            this.tsbEditBackup = new System.Windows.Forms.ToolStripButton();
            this.tsbDeleteBackup = new System.Windows.Forms.ToolStripButton();
            this.tsbAbout = new System.Windows.Forms.ToolStripButton();
            this.msTray.SuspendLayout();
            this.msMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress)).BeginInit();
            this.tsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // noIco
            // 
            this.noIco.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.noIco.BalloonTipText = "Running on the background...";
            this.noIco.BalloonTipTitle = "SimpleBackup";
            this.noIco.ContextMenuStrip = this.msTray;
            this.noIco.Icon = ((System.Drawing.Icon)(resources.GetObject("noIco.Icon")));
            this.noIco.Text = "SimpleBackup";
            this.noIco.Visible = true;
            this.noIco.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.noIco_MouseDoubleClick);
            // 
            // msTray
            // 
            this.msTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuExit});
            this.msTray.Name = "msTray";
            this.msTray.Size = new System.Drawing.Size(98, 26);
            // 
            // mnuExit
            // 
            this.mnuExit.Image = global::SimpleBackup.Properties.Resources.Exit;
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Size = new System.Drawing.Size(97, 22);
            this.mnuExit.Text = "Quit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // btRunNow
            // 
            this.btRunNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btRunNow.Image = global::SimpleBackup.Properties.Resources.Application;
            this.btRunNow.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btRunNow.Location = new System.Drawing.Point(411, 257);
            this.btRunNow.Name = "btRunNow";
            this.btRunNow.Size = new System.Drawing.Size(120, 26);
            this.btRunNow.TabIndex = 4;
            this.btRunNow.Text = "Run now";
            this.btRunNow.UseVisualStyleBackColor = true;
            this.btRunNow.Click += new System.EventHandler(this.btRunNow_Click);
            // 
            // btQuit
            // 
            this.btQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btQuit.Image = global::SimpleBackup.Properties.Resources.Exit;
            this.btQuit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btQuit.Location = new System.Drawing.Point(12, 257);
            this.btQuit.Name = "btQuit";
            this.btQuit.Size = new System.Drawing.Size(120, 26);
            this.btQuit.TabIndex = 5;
            this.btQuit.Text = "Quit";
            this.btQuit.UseVisualStyleBackColor = true;
            this.btQuit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // lvBackups
            // 
            this.lvBackups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvBackups.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colBackupName,
            this.colTimeTaken,
            this.colNextBackup});
            this.lvBackups.FullRowSelect = true;
            this.lvBackups.GridLines = true;
            this.lvBackups.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvBackups.HideSelection = false;
            this.lvBackups.Location = new System.Drawing.Point(12, 52);
            this.lvBackups.Name = "lvBackups";
            this.lvBackups.ShowItemToolTips = true;
            this.lvBackups.Size = new System.Drawing.Size(519, 199);
            this.lvBackups.SmallImageList = this.imIcons;
            this.lvBackups.TabIndex = 18;
            this.lvBackups.UseCompatibleStateImageBehavior = false;
            this.lvBackups.View = System.Windows.Forms.View.Details;
            this.lvBackups.SelectedIndexChanged += new System.EventHandler(this.lvBackups_SelectedIndexChanged);
            // 
            // colBackupName
            // 
            this.colBackupName.Tag = "Name=colBackupName";
            this.colBackupName.Text = "Backup";
            this.colBackupName.Width = 250;
            // 
            // colTimeTaken
            // 
            this.colTimeTaken.Tag = "Name=colTimeTaken";
            this.colTimeTaken.Text = "Last backup";
            this.colTimeTaken.Width = 120;
            // 
            // colNextBackup
            // 
            this.colNextBackup.Tag = "Name=colNextBackup";
            this.colNextBackup.Text = "Next backup";
            this.colNextBackup.Width = 120;
            // 
            // imIcons
            // 
            this.imIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imIcons.ImageStream")));
            this.imIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.imIcons.Images.SetKeyName(0, "Apply.png");
            this.imIcons.Images.SetKeyName(1, "Trackback.png");
            this.imIcons.Images.SetKeyName(2, "Warning.png");
            this.imIcons.Images.SetKeyName(3, "Sync.png");
            // 
            // msMain
            // 
            this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuHelp});
            this.msMain.Location = new System.Drawing.Point(0, 0);
            this.msMain.Name = "msMain";
            this.msMain.Size = new System.Drawing.Size(543, 24);
            this.msMain.TabIndex = 21;
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAddBackup,
            this.mnuEditBackup,
            this.mnuRunNow,
            this.mnuDeleteBackup,
            this.toolStripMenuItem1,
            this.mnuQuit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(37, 20);
            this.mnuFile.Text = "File";
            // 
            // mnuAddBackup
            // 
            this.mnuAddBackup.Image = global::SimpleBackup.Properties.Resources.New_document;
            this.mnuAddBackup.Name = "mnuAddBackup";
            this.mnuAddBackup.Size = new System.Drawing.Size(180, 22);
            this.mnuAddBackup.Text = "Add backup";
            this.mnuAddBackup.Click += new System.EventHandler(this.mnuAddBackup_Click);
            // 
            // mnuEditBackup
            // 
            this.mnuEditBackup.Image = global::SimpleBackup.Properties.Resources.Modify;
            this.mnuEditBackup.Name = "mnuEditBackup";
            this.mnuEditBackup.Size = new System.Drawing.Size(180, 22);
            this.mnuEditBackup.Text = "Edit backup";
            this.mnuEditBackup.Click += new System.EventHandler(this.mnuEditBackup_Click);
            // 
            // mnuRunNow
            // 
            this.mnuRunNow.Image = global::SimpleBackup.Properties.Resources.Application;
            this.mnuRunNow.Name = "mnuRunNow";
            this.mnuRunNow.Size = new System.Drawing.Size(180, 22);
            this.mnuRunNow.Text = "Run now";
            this.mnuRunNow.Click += new System.EventHandler(this.btRunNow_Click);
            // 
            // mnuDeleteBackup
            // 
            this.mnuDeleteBackup.Image = global::SimpleBackup.Properties.Resources.Delete;
            this.mnuDeleteBackup.Name = "mnuDeleteBackup";
            this.mnuDeleteBackup.Size = new System.Drawing.Size(180, 22);
            this.mnuDeleteBackup.Text = "Delete backup";
            this.mnuDeleteBackup.Click += new System.EventHandler(this.mnuDeleteBackup_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // mnuQuit
            // 
            this.mnuQuit.Image = global::SimpleBackup.Properties.Resources.Exit;
            this.mnuQuit.Name = "mnuQuit";
            this.mnuQuit.Size = new System.Drawing.Size(180, 22);
            this.mnuQuit.Text = "Quit";
            this.mnuQuit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(44, 20);
            this.mnuHelp.Text = "Help";
            // 
            // mnuAbout
            // 
            this.mnuAbout.Image = global::SimpleBackup.Properties.Resources.About;
            this.mnuAbout.Name = "mnuAbout";
            this.mnuAbout.Size = new System.Drawing.Size(180, 22);
            this.mnuAbout.Text = "About";
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgress.Image = global::SimpleBackup.Properties.Resources.AnimatedBar;
            this.pbProgress.Location = new System.Drawing.Point(415, 0);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(128, 25);
            this.pbProgress.TabIndex = 22;
            this.pbProgress.TabStop = false;
            // 
            // tsMain
            // 
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAddBackup,
            this.tsbEditBackup,
            this.tsbDeleteBackup,
            this.tsbAbout});
            this.tsMain.Location = new System.Drawing.Point(0, 24);
            this.tsMain.Name = "tsMain";
            this.tsMain.Size = new System.Drawing.Size(543, 25);
            this.tsMain.TabIndex = 23;
            this.tsMain.Text = "toolStrip1";
            // 
            // tsbAddBackup
            // 
            this.tsbAddBackup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddBackup.Image = global::SimpleBackup.Properties.Resources.New_document;
            this.tsbAddBackup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddBackup.Name = "tsbAddBackup";
            this.tsbAddBackup.Size = new System.Drawing.Size(23, 22);
            this.tsbAddBackup.Text = "toolStripButton1";
            this.tsbAddBackup.Click += new System.EventHandler(this.mnuAddBackup_Click);
            // 
            // tsbEditBackup
            // 
            this.tsbEditBackup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbEditBackup.Image = global::SimpleBackup.Properties.Resources.Modify;
            this.tsbEditBackup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEditBackup.Name = "tsbEditBackup";
            this.tsbEditBackup.Size = new System.Drawing.Size(23, 22);
            this.tsbEditBackup.Text = "toolStripButton1";
            this.tsbEditBackup.Click += new System.EventHandler(this.mnuEditBackup_Click);
            // 
            // tsbDeleteBackup
            // 
            this.tsbDeleteBackup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteBackup.Image = global::SimpleBackup.Properties.Resources.Delete;
            this.tsbDeleteBackup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDeleteBackup.Name = "tsbDeleteBackup";
            this.tsbDeleteBackup.Size = new System.Drawing.Size(23, 22);
            this.tsbDeleteBackup.Text = "toolStripButton1";
            this.tsbDeleteBackup.Click += new System.EventHandler(this.mnuDeleteBackup_Click);
            // 
            // tsbAbout
            // 
            this.tsbAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAbout.Image = global::SimpleBackup.Properties.Resources.About;
            this.tsbAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAbout.Name = "tsbAbout";
            this.tsbAbout.Size = new System.Drawing.Size(23, 22);
            this.tsbAbout.Text = "toolStripButton1";
            this.tsbAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            // 
            // FormMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(543, 295);
            this.Controls.Add(this.tsMain);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.msMain);
            this.Controls.Add(this.lvBackups);
            this.Controls.Add(this.btQuit);
            this.Controls.Add(this.btRunNow);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.msMain;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.Text = "SimpleBackup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.VisibleChanged += new System.EventHandler(this.FormMain_VisibleChanged);
            this.msTray.ResumeLayout(false);
            this.msMain.ResumeLayout(false);
            this.msMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress)).EndInit();
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon noIco;
        private System.Windows.Forms.ContextMenuStrip msTray;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.Button btRunNow;
        private System.Windows.Forms.Button btQuit;
        private System.Windows.Forms.ListView lvBackups;
        private System.Windows.Forms.ColumnHeader colBackupName;
        private System.Windows.Forms.ColumnHeader colTimeTaken;
        private System.Windows.Forms.ColumnHeader colNextBackup;
        private System.Windows.Forms.ImageList imIcons;
        private System.Windows.Forms.MenuStrip msMain;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuAddBackup;
        private System.Windows.Forms.ToolStripMenuItem mnuEditBackup;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuQuit;
        private System.Windows.Forms.PictureBox pbProgress;
        private System.Windows.Forms.ToolStripMenuItem mnuRunNow;
        private System.Windows.Forms.ToolStripMenuItem mnuDeleteBackup;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuAbout;
        private System.Windows.Forms.ToolStrip tsMain;
        private System.Windows.Forms.ToolStripButton tsbAddBackup;
        private System.Windows.Forms.ToolStripButton tsbEditBackup;
        private System.Windows.Forms.ToolStripButton tsbDeleteBackup;
        private System.Windows.Forms.ToolStripButton tsbAbout;
    }
}

