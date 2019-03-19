namespace SimpleBackup
{
    partial class FormEditBackup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditBackup));
            this.btCancel = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.clbWeekDays = new System.Windows.Forms.CheckedListBox();
            this.cbWeekDaysAll = new System.Windows.Forms.CheckBox();
            this.cbMonthsAll = new System.Windows.Forms.CheckBox();
            this.clbMonths = new System.Windows.Forms.CheckedListBox();
            this.clbDays = new System.Windows.Forms.CheckedListBox();
            this.cbDaysAll = new System.Windows.Forms.CheckBox();
            this.cbHoursAll = new System.Windows.Forms.CheckBox();
            this.clbHours = new System.Windows.Forms.CheckedListBox();
            this.cbMinutesAll = new System.Windows.Forms.CheckBox();
            this.clbMinutes = new System.Windows.Forms.CheckedListBox();
            this.lvDates = new System.Windows.Forms.ListView();
            this.colWeekDay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colYear = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMonth = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colWeek = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLocalFormat = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.tbBackupName = new System.Windows.Forms.TextBox();
            this.tbBackupDirFrom = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbBackupFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbBackupDirTo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.nudAmout = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbUseCronEntry = new System.Windows.Forms.CheckBox();
            this.tbCronEntry = new System.Windows.Forms.TextBox();
            this.lbCronHelp = new System.Windows.Forms.Label();
            this.ttTimeWarning = new System.Windows.Forms.ToolTip(this.components);
            this.tbProcess = new System.Windows.Forms.TextBox();
            this.lbProcess = new System.Windows.Forms.Label();
            this.cbKillProcess = new System.Windows.Forms.CheckBox();
            this.nudProcessKill = new System.Windows.Forms.NumericUpDown();
            this.btSelectProcess = new System.Windows.Forms.Button();
            this.pbTimeWarning = new System.Windows.Forms.PictureBox();
            this.btSelectBackupTo = new System.Windows.Forms.Button();
            this.btSelectBackupFrom = new System.Windows.Forms.Button();
            this.lbOnFailure1 = new System.Windows.Forms.Label();
            this.nudRetryHours = new System.Windows.Forms.NumericUpDown();
            this.lbOnFailure2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudAmout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudProcessKill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTimeWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRetryHours)).BeginInit();
            this.SuspendLayout();
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Image = global::SimpleBackup.Properties.Resources.Cancel;
            this.btCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btCancel.Location = new System.Drawing.Point(12, 546);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(86, 26);
            this.btCancel.TabIndex = 0;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Image = global::SimpleBackup.Properties.Resources.OK;
            this.btOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btOK.Location = new System.Drawing.Point(551, 546);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(86, 26);
            this.btOK.TabIndex = 1;
            this.btOK.Text = "OK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // clbWeekDays
            // 
            this.clbWeekDays.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.clbWeekDays.CheckOnClick = true;
            this.clbWeekDays.FormattingEnabled = true;
            this.clbWeekDays.Location = new System.Drawing.Point(12, 35);
            this.clbWeekDays.Name = "clbWeekDays";
            this.clbWeekDays.Size = new System.Drawing.Size(120, 124);
            this.clbWeekDays.TabIndex = 2;
            this.clbWeekDays.Click += new System.EventHandler(this.clbClick);
            this.clbWeekDays.SelectedIndexChanged += new System.EventHandler(this.clbClick);
            // 
            // cbWeekDaysAll
            // 
            this.cbWeekDaysAll.AutoSize = true;
            this.cbWeekDaysAll.Location = new System.Drawing.Point(12, 12);
            this.cbWeekDaysAll.Name = "cbWeekDaysAll";
            this.cbWeekDaysAll.Size = new System.Drawing.Size(77, 17);
            this.cbWeekDaysAll.TabIndex = 3;
            this.cbWeekDaysAll.Text = "Weekdays";
            this.cbWeekDaysAll.UseVisualStyleBackColor = true;
            this.cbWeekDaysAll.Click += new System.EventHandler(this.cbClick);
            // 
            // cbMonthsAll
            // 
            this.cbMonthsAll.AutoSize = true;
            this.cbMonthsAll.Location = new System.Drawing.Point(138, 12);
            this.cbMonthsAll.Name = "cbMonthsAll";
            this.cbMonthsAll.Size = new System.Drawing.Size(61, 17);
            this.cbMonthsAll.TabIndex = 5;
            this.cbMonthsAll.Text = "Months";
            this.cbMonthsAll.UseVisualStyleBackColor = true;
            this.cbMonthsAll.Click += new System.EventHandler(this.cbClick);
            // 
            // clbMonths
            // 
            this.clbMonths.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.clbMonths.CheckOnClick = true;
            this.clbMonths.FormattingEnabled = true;
            this.clbMonths.Location = new System.Drawing.Point(138, 35);
            this.clbMonths.Name = "clbMonths";
            this.clbMonths.Size = new System.Drawing.Size(120, 124);
            this.clbMonths.TabIndex = 4;
            this.clbMonths.Click += new System.EventHandler(this.clbClick);
            this.clbMonths.SelectedIndexChanged += new System.EventHandler(this.clbClick);
            // 
            // clbDays
            // 
            this.clbDays.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.clbDays.CheckOnClick = true;
            this.clbDays.FormattingEnabled = true;
            this.clbDays.Location = new System.Drawing.Point(264, 35);
            this.clbDays.Name = "clbDays";
            this.clbDays.Size = new System.Drawing.Size(120, 124);
            this.clbDays.TabIndex = 8;
            this.clbDays.Click += new System.EventHandler(this.clbClick);
            this.clbDays.SelectedIndexChanged += new System.EventHandler(this.clbClick);
            // 
            // cbDaysAll
            // 
            this.cbDaysAll.AutoSize = true;
            this.cbDaysAll.Location = new System.Drawing.Point(264, 12);
            this.cbDaysAll.Name = "cbDaysAll";
            this.cbDaysAll.Size = new System.Drawing.Size(50, 17);
            this.cbDaysAll.TabIndex = 10;
            this.cbDaysAll.Text = "Days";
            this.cbDaysAll.UseVisualStyleBackColor = true;
            this.cbDaysAll.Click += new System.EventHandler(this.cbClick);
            // 
            // cbHoursAll
            // 
            this.cbHoursAll.AutoSize = true;
            this.cbHoursAll.Location = new System.Drawing.Point(390, 12);
            this.cbHoursAll.Name = "cbHoursAll";
            this.cbHoursAll.Size = new System.Drawing.Size(54, 17);
            this.cbHoursAll.TabIndex = 13;
            this.cbHoursAll.Text = "Hours";
            this.cbHoursAll.UseVisualStyleBackColor = true;
            this.cbHoursAll.Click += new System.EventHandler(this.cbClick);
            // 
            // clbHours
            // 
            this.clbHours.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.clbHours.CheckOnClick = true;
            this.clbHours.FormattingEnabled = true;
            this.clbHours.Location = new System.Drawing.Point(390, 35);
            this.clbHours.Name = "clbHours";
            this.clbHours.Size = new System.Drawing.Size(120, 124);
            this.clbHours.TabIndex = 11;
            this.clbHours.Click += new System.EventHandler(this.clbClick);
            this.clbHours.SelectedIndexChanged += new System.EventHandler(this.clbClick);
            // 
            // cbMinutesAll
            // 
            this.cbMinutesAll.AutoSize = true;
            this.cbMinutesAll.Location = new System.Drawing.Point(516, 12);
            this.cbMinutesAll.Name = "cbMinutesAll";
            this.cbMinutesAll.Size = new System.Drawing.Size(63, 17);
            this.cbMinutesAll.TabIndex = 16;
            this.cbMinutesAll.Text = "Minutes";
            this.cbMinutesAll.UseVisualStyleBackColor = true;
            this.cbMinutesAll.Click += new System.EventHandler(this.cbClick);
            // 
            // clbMinutes
            // 
            this.clbMinutes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.clbMinutes.CheckOnClick = true;
            this.clbMinutes.FormattingEnabled = true;
            this.clbMinutes.Location = new System.Drawing.Point(516, 35);
            this.clbMinutes.Name = "clbMinutes";
            this.clbMinutes.Size = new System.Drawing.Size(120, 124);
            this.clbMinutes.TabIndex = 14;
            this.clbMinutes.Click += new System.EventHandler(this.clbClick);
            this.clbMinutes.SelectedIndexChanged += new System.EventHandler(this.clbClick);
            // 
            // lvDates
            // 
            this.lvDates.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colWeekDay,
            this.colYear,
            this.colM,
            this.colMonth,
            this.colWeek,
            this.colDay,
            this.colTime,
            this.colLocalFormat});
            this.lvDates.FullRowSelect = true;
            this.lvDates.GridLines = true;
            this.lvDates.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvDates.Location = new System.Drawing.Point(12, 191);
            this.lvDates.MultiSelect = false;
            this.lvDates.Name = "lvDates";
            this.lvDates.Size = new System.Drawing.Size(625, 95);
            this.lvDates.TabIndex = 17;
            this.lvDates.UseCompatibleStateImageBehavior = false;
            this.lvDates.View = System.Windows.Forms.View.Details;
            this.lvDates.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.lvDates_ColumnWidthChanging);
            // 
            // colWeekDay
            // 
            this.colWeekDay.Tag = "Name=colWeekDay";
            this.colWeekDay.Text = "Week day";
            this.colWeekDay.Width = 100;
            // 
            // colYear
            // 
            this.colYear.Tag = "Name=colYear";
            this.colYear.Text = "Year";
            // 
            // colM
            // 
            this.colM.Tag = "Name=colM";
            this.colM.Text = "M";
            this.colM.Width = 30;
            // 
            // colMonth
            // 
            this.colMonth.Tag = "Name=colMonth";
            this.colMonth.Text = "Month";
            this.colMonth.Width = 100;
            // 
            // colWeek
            // 
            this.colWeek.Tag = "Name=colWeek";
            this.colWeek.Text = "Week";
            this.colWeek.Width = 45;
            // 
            // colDay
            // 
            this.colDay.Tag = "Name=colDay";
            this.colDay.Text = "Day";
            this.colDay.Width = 40;
            // 
            // colTime
            // 
            this.colTime.Tag = "Name=colTime";
            this.colTime.Text = "Time";
            this.colTime.Width = 40;
            // 
            // colLocalFormat
            // 
            this.colLocalFormat.Tag = "Name=colLocalFormat";
            this.colLocalFormat.Text = "Local date/time";
            this.colLocalFormat.Width = 185;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 295);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Backup name:";
            // 
            // tbBackupName
            // 
            this.tbBackupName.Location = new System.Drawing.Point(161, 292);
            this.tbBackupName.Name = "tbBackupName";
            this.tbBackupName.Size = new System.Drawing.Size(476, 20);
            this.tbBackupName.TabIndex = 19;
            this.tbBackupName.TextChanged += new System.EventHandler(this.tbTextChange);
            // 
            // tbBackupDirFrom
            // 
            this.tbBackupDirFrom.Location = new System.Drawing.Point(161, 318);
            this.tbBackupDirFrom.Name = "tbBackupDirFrom";
            this.tbBackupDirFrom.Size = new System.Drawing.Size(436, 20);
            this.tbBackupDirFrom.TabIndex = 21;
            this.tbBackupDirFrom.TextChanged += new System.EventHandler(this.tbTextChange);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 321);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Backup directory:";
            // 
            // tbBackupFile
            // 
            this.tbBackupFile.Location = new System.Drawing.Point(161, 344);
            this.tbBackupFile.Name = "tbBackupFile";
            this.tbBackupFile.Size = new System.Drawing.Size(475, 20);
            this.tbBackupFile.TabIndex = 24;
            this.tbBackupFile.TextChanged += new System.EventHandler(this.tbTextChange);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 347);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Backup file name:";
            // 
            // tbBackupDirTo
            // 
            this.tbBackupDirTo.Location = new System.Drawing.Point(161, 370);
            this.tbBackupDirTo.Name = "tbBackupDirTo";
            this.tbBackupDirTo.Size = new System.Drawing.Size(436, 20);
            this.tbBackupDirTo.TabIndex = 26;
            this.tbBackupDirTo.TextChanged += new System.EventHandler(this.tbTextChange);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 373);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Backup to:";
            // 
            // nudAmout
            // 
            this.nudAmout.Location = new System.Drawing.Point(12, 468);
            this.nudAmout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAmout.Name = "nudAmout";
            this.nudAmout.Size = new System.Drawing.Size(50, 20);
            this.nudAmout.TabIndex = 28;
            this.nudAmout.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 452);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 29;
            this.label5.Text = "Keep";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(96, 452);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(479, 91);
            this.label6.TabIndex = 30;
            this.label6.Text = resources.GetString("label6.Text");
            // 
            // cbUseCronEntry
            // 
            this.cbUseCronEntry.AutoSize = true;
            this.cbUseCronEntry.Location = new System.Drawing.Point(12, 167);
            this.cbUseCronEntry.Name = "cbUseCronEntry";
            this.cbUseCronEntry.Size = new System.Drawing.Size(98, 17);
            this.cbUseCronEntry.TabIndex = 31;
            this.cbUseCronEntry.Text = "Use cron entry:";
            this.cbUseCronEntry.UseVisualStyleBackColor = true;
            this.cbUseCronEntry.CheckedChanged += new System.EventHandler(this.cbUseCronEntry_CheckedChanged);
            this.cbUseCronEntry.Click += new System.EventHandler(this.cbClick);
            // 
            // tbCronEntry
            // 
            this.tbCronEntry.Location = new System.Drawing.Point(161, 165);
            this.tbCronEntry.Name = "tbCronEntry";
            this.tbCronEntry.Size = new System.Drawing.Size(476, 20);
            this.tbCronEntry.TabIndex = 32;
            this.tbCronEntry.Text = "* * * * *";
            this.tbCronEntry.TextChanged += new System.EventHandler(this.tbCronEntry_TextChanged);
            // 
            // lbCronHelp
            // 
            this.lbCronHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCronHelp.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCronHelp.Location = new System.Drawing.Point(12, 9);
            this.lbCronHelp.Name = "lbCronHelp";
            this.lbCronHelp.Size = new System.Drawing.Size(624, 153);
            this.lbCronHelp.TabIndex = 33;
            this.lbCronHelp.Text = resources.GetString("lbCronHelp.Text");
            this.lbCronHelp.Visible = false;
            // 
            // ttTimeWarning
            // 
            this.ttTimeWarning.IsBalloon = true;
            // 
            // tbProcess
            // 
            this.tbProcess.Location = new System.Drawing.Point(161, 396);
            this.tbProcess.Name = "tbProcess";
            this.tbProcess.Size = new System.Drawing.Size(231, 20);
            this.tbProcess.TabIndex = 36;
            // 
            // lbProcess
            // 
            this.lbProcess.AutoSize = true;
            this.lbProcess.Location = new System.Drawing.Point(12, 399);
            this.lbProcess.Name = "lbProcess";
            this.lbProcess.Size = new System.Drawing.Size(110, 13);
            this.lbProcess.TabIndex = 35;
            this.lbProcess.Text = "Close/restart process:";
            // 
            // cbKillProcess
            // 
            this.cbKillProcess.AutoSize = true;
            this.cbKillProcess.Location = new System.Drawing.Point(398, 398);
            this.cbKillProcess.Name = "cbKillProcess";
            this.cbKillProcess.Size = new System.Drawing.Size(121, 17);
            this.cbKillProcess.TabIndex = 37;
            this.cbKillProcess.Text = "Kill after wait time(s):";
            this.cbKillProcess.UseVisualStyleBackColor = true;
            // 
            // nudProcessKill
            // 
            this.nudProcessKill.Location = new System.Drawing.Point(553, 396);
            this.nudProcessKill.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudProcessKill.Name = "nudProcessKill";
            this.nudProcessKill.Size = new System.Drawing.Size(44, 20);
            this.nudProcessKill.TabIndex = 38;
            this.nudProcessKill.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btSelectProcess
            // 
            this.btSelectProcess.Image = global::SimpleBackup.Properties.Resources.Application;
            this.btSelectProcess.Location = new System.Drawing.Point(603, 396);
            this.btSelectProcess.Name = "btSelectProcess";
            this.btSelectProcess.Size = new System.Drawing.Size(33, 20);
            this.btSelectProcess.TabIndex = 39;
            this.btSelectProcess.UseVisualStyleBackColor = true;
            this.btSelectProcess.Click += new System.EventHandler(this.btSelectProcess_Click);
            // 
            // pbTimeWarning
            // 
            this.pbTimeWarning.Location = new System.Drawing.Point(594, 508);
            this.pbTimeWarning.Name = "pbTimeWarning";
            this.pbTimeWarning.Size = new System.Drawing.Size(32, 32);
            this.pbTimeWarning.TabIndex = 34;
            this.pbTimeWarning.TabStop = false;
            // 
            // btSelectBackupTo
            // 
            this.btSelectBackupTo.Image = global::SimpleBackup.Properties.Resources.SelectDir;
            this.btSelectBackupTo.Location = new System.Drawing.Point(603, 370);
            this.btSelectBackupTo.Name = "btSelectBackupTo";
            this.btSelectBackupTo.Size = new System.Drawing.Size(33, 20);
            this.btSelectBackupTo.TabIndex = 27;
            this.btSelectBackupTo.UseVisualStyleBackColor = true;
            this.btSelectBackupTo.Click += new System.EventHandler(this.btSelectBackupTo_Click);
            // 
            // btSelectBackupFrom
            // 
            this.btSelectBackupFrom.Image = global::SimpleBackup.Properties.Resources.SelectDir;
            this.btSelectBackupFrom.Location = new System.Drawing.Point(603, 318);
            this.btSelectBackupFrom.Name = "btSelectBackupFrom";
            this.btSelectBackupFrom.Size = new System.Drawing.Size(33, 20);
            this.btSelectBackupFrom.TabIndex = 22;
            this.btSelectBackupFrom.UseVisualStyleBackColor = true;
            this.btSelectBackupFrom.Click += new System.EventHandler(this.btSelectBackupFrom_Click);
            // 
            // lbOnFailure1
            // 
            this.lbOnFailure1.AutoSize = true;
            this.lbOnFailure1.Location = new System.Drawing.Point(12, 424);
            this.lbOnFailure1.Name = "lbOnFailure1";
            this.lbOnFailure1.Size = new System.Drawing.Size(102, 13);
            this.lbOnFailure1.TabIndex = 40;
            this.lbOnFailure1.Text = "On failure, retry after";
            // 
            // nudRetryHours
            // 
            this.nudRetryHours.Location = new System.Drawing.Point(161, 422);
            this.nudRetryHours.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.nudRetryHours.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRetryHours.Name = "nudRetryHours";
            this.nudRetryHours.Size = new System.Drawing.Size(50, 20);
            this.nudRetryHours.TabIndex = 41;
            this.nudRetryHours.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lbOnFailure2
            // 
            this.lbOnFailure2.AutoSize = true;
            this.lbOnFailure2.Location = new System.Drawing.Point(217, 424);
            this.lbOnFailure2.Name = "lbOnFailure2";
            this.lbOnFailure2.Size = new System.Drawing.Size(241, 13);
            this.lbOnFailure2.TabIndex = 42;
            this.lbOnFailure2.Text = "hours. (NOTE: The schedule will not be updated.)";
            // 
            // FormEditBackup
            // 
            this.AcceptButton = this.btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(649, 584);
            this.Controls.Add(this.lbOnFailure2);
            this.Controls.Add(this.nudRetryHours);
            this.Controls.Add(this.lbOnFailure1);
            this.Controls.Add(this.btSelectProcess);
            this.Controls.Add(this.nudProcessKill);
            this.Controls.Add(this.cbKillProcess);
            this.Controls.Add(this.tbProcess);
            this.Controls.Add(this.lbProcess);
            this.Controls.Add(this.pbTimeWarning);
            this.Controls.Add(this.lbCronHelp);
            this.Controls.Add(this.tbCronEntry);
            this.Controls.Add(this.cbUseCronEntry);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nudAmout);
            this.Controls.Add(this.btSelectBackupTo);
            this.Controls.Add(this.tbBackupDirTo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbBackupFile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btSelectBackupFrom);
            this.Controls.Add(this.tbBackupDirFrom);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbBackupName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvDates);
            this.Controls.Add(this.cbMinutesAll);
            this.Controls.Add(this.clbMinutes);
            this.Controls.Add(this.cbHoursAll);
            this.Controls.Add(this.clbHours);
            this.Controls.Add(this.cbDaysAll);
            this.Controls.Add(this.clbDays);
            this.Controls.Add(this.cbMonthsAll);
            this.Controls.Add(this.clbMonths);
            this.Controls.Add(this.cbWeekDaysAll);
            this.Controls.Add(this.clbWeekDays);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.btCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditBackup";
            this.ShowInTaskbar = false;
            this.Text = "Add backup";
            ((System.ComponentModel.ISupportInitialize)(this.nudAmout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudProcessKill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTimeWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRetryHours)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.CheckedListBox clbWeekDays;
        private System.Windows.Forms.CheckBox cbWeekDaysAll;
        private System.Windows.Forms.CheckBox cbMonthsAll;
        private System.Windows.Forms.CheckedListBox clbMonths;
        private System.Windows.Forms.CheckedListBox clbDays;
        private System.Windows.Forms.CheckBox cbDaysAll;
        private System.Windows.Forms.CheckBox cbHoursAll;
        private System.Windows.Forms.CheckedListBox clbHours;
        private System.Windows.Forms.CheckBox cbMinutesAll;
        private System.Windows.Forms.CheckedListBox clbMinutes;
        private System.Windows.Forms.ListView lvDates;
        private System.Windows.Forms.ColumnHeader colWeekDay;
        private System.Windows.Forms.ColumnHeader colYear;
        private System.Windows.Forms.ColumnHeader colMonth;
        private System.Windows.Forms.ColumnHeader colWeek;
        private System.Windows.Forms.ColumnHeader colDay;
        private System.Windows.Forms.ColumnHeader colTime;
        private System.Windows.Forms.ColumnHeader colLocalFormat;
        private System.Windows.Forms.ColumnHeader colM;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbBackupName;
        private System.Windows.Forms.TextBox tbBackupDirFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btSelectBackupFrom;
        private System.Windows.Forms.TextBox tbBackupFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btSelectBackupTo;
        private System.Windows.Forms.TextBox tbBackupDirTo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudAmout;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox cbUseCronEntry;
        private System.Windows.Forms.TextBox tbCronEntry;
        private System.Windows.Forms.Label lbCronHelp;
        private System.Windows.Forms.ToolTip ttTimeWarning;
        private System.Windows.Forms.PictureBox pbTimeWarning;
        private System.Windows.Forms.TextBox tbProcess;
        private System.Windows.Forms.Label lbProcess;
        private System.Windows.Forms.CheckBox cbKillProcess;
        private System.Windows.Forms.NumericUpDown nudProcessKill;
        private System.Windows.Forms.Button btSelectProcess;
        private System.Windows.Forms.Label lbOnFailure1;
        private System.Windows.Forms.NumericUpDown nudRetryHours;
        private System.Windows.Forms.Label lbOnFailure2;
    }
}