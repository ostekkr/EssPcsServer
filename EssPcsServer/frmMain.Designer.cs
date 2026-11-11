namespace EssPcsServer
{
    partial class frmMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.imageListTab = new System.Windows.Forms.ImageList(this.components);
            this.c1CommandHolder1 = new C1.Win.Command.C1CommandHolder();
            this.imageListBlank = new System.Windows.Forms.ImageList(this.components);
            this.timerMain = new System.Windows.Forms.Timer(this.components);
            this.Tray = new System.Windows.Forms.NotifyIcon(this.components);
            this.c1RibbonMenu = new C1.Win.Ribbon.C1Ribbon();
            this.ribbonApplicationMenu1 = new C1.Win.Ribbon.RibbonApplicationMenu();
            this.ribbonBottomToolBar1 = new C1.Win.Ribbon.RibbonBottomToolBar();
            this.ribbonConfigToolBar1 = new C1.Win.Ribbon.RibbonConfigToolBar();
            this.ribbonQat1 = new C1.Win.Ribbon.RibbonQat();
            this.tabBasic = new C1.Win.Ribbon.RibbonTab();
            this.ribbonGroupFile = new C1.Win.Ribbon.RibbonGroup();
            this.menuStripMainFileRefresh = new C1.Win.Ribbon.RibbonButton();
            this.ribbonSeparator1 = new C1.Win.Ribbon.RibbonSeparator();
            this.menuStripMainFileExit = new C1.Win.Ribbon.RibbonButton();
            this.ribbonGroupRun = new C1.Win.Ribbon.RibbonGroup();
            this.menuStripMainConnect = new C1.Win.Ribbon.RibbonButton();
            this.ribbonSeparator2 = new C1.Win.Ribbon.RibbonSeparator();
            this.menuStripMainRunStart = new C1.Win.Ribbon.RibbonButton();
            this.ribbonSeparator9 = new C1.Win.Ribbon.RibbonSeparator();
            this.menuStripMainRunStop = new C1.Win.Ribbon.RibbonButton();
            this.ribbonSeparator5 = new C1.Win.Ribbon.RibbonSeparator();
            this.menuStripMainDisconnect = new C1.Win.Ribbon.RibbonButton();
            this.ribbonSeparator3 = new C1.Win.Ribbon.RibbonSeparator();
            this.btnInvRun = new C1.Win.Ribbon.RibbonButton();
            this.btnInvStop = new C1.Win.Ribbon.RibbonButton();
            this.ribbonSeparator6 = new C1.Win.Ribbon.RibbonSeparator();
            this.btnInvFaultClear = new C1.Win.Ribbon.RibbonButton();
            this.ribbonSeparator7 = new C1.Win.Ribbon.RibbonSeparator();
            this.numInvSetP = new C1.Win.Ribbon.RibbonNumericBox();
            this.numInvSetQ = new C1.Win.Ribbon.RibbonNumericBox();
            this.ribbonSeparator8 = new C1.Win.Ribbon.RibbonSeparator();
            this.btnInvSet = new C1.Win.Ribbon.RibbonButton();
            this.ribbonGroupHelp = new C1.Win.Ribbon.RibbonGroup();
            this.menuStripMainHelpAbout = new C1.Win.Ribbon.RibbonButton();
            this.tabRun = new C1.Win.Ribbon.RibbonTab();
            this.groupEssControlCommand = new C1.Win.Ribbon.RibbonGroup();
            this.btnEssControlStart = new C1.Win.Ribbon.RibbonButton();
            this.ribbonSeparator10 = new C1.Win.Ribbon.RibbonSeparator();
            this.btnEssControlStop = new C1.Win.Ribbon.RibbonButton();
            this.operationGeneralGroup = new C1.Win.Ribbon.RibbonGroup();
            this.comboBoxOperationMode = new C1.Win.Ribbon.RibbonComboBox();
            this.ribbonButton1 = new C1.Win.Ribbon.RibbonButton();
            this.ribbonButton2 = new C1.Win.Ribbon.RibbonButton();
            this.ribbonButton3 = new C1.Win.Ribbon.RibbonButton();
            this.ribbonButton4 = new C1.Win.Ribbon.RibbonButton();
            this.ribbonButton5 = new C1.Win.Ribbon.RibbonButton();
            this.ribbonSeparator18 = new C1.Win.Ribbon.RibbonSeparator();
            this.numEssControlPeriod = new C1.Win.Ribbon.RibbonNumericBox();
            this.numEssPcsPower = new C1.Win.Ribbon.RibbonNumericBox();
            this.ribbonGroup2 = new C1.Win.Ribbon.RibbonGroup();
            this.ribbonGroup3 = new C1.Win.Ribbon.RibbonGroup();
            this.tabGeneralControl = new C1.Win.Ribbon.RibbonTab();
            this.groupEssChargeTime = new C1.Win.Ribbon.RibbonGroup();
            this.numEssChargeStartTimeHour = new C1.Win.Ribbon.RibbonNumericBox();
            this.numEssChargeStartTimeMin = new C1.Win.Ribbon.RibbonNumericBox();
            this.ribbonSeparator11 = new C1.Win.Ribbon.RibbonSeparator();
            this.numEssChargeEndTimeHour = new C1.Win.Ribbon.RibbonNumericBox();
            this.numEssChargeEndTimeMin = new C1.Win.Ribbon.RibbonNumericBox();
            this.groupEssChargeParametr = new C1.Win.Ribbon.RibbonGroup();
            this.numEssChargeSocLimit = new C1.Win.Ribbon.RibbonNumericBox();
            this.numEssChargeOutputLimit = new C1.Win.Ribbon.RibbonNumericBox();
            this.groupEssDischargeTime = new C1.Win.Ribbon.RibbonGroup();
            this.numEssDischargeStartTimeHour = new C1.Win.Ribbon.RibbonNumericBox();
            this.numEssDischargeStartTimeMin = new C1.Win.Ribbon.RibbonNumericBox();
            this.ribbonSeparator12 = new C1.Win.Ribbon.RibbonSeparator();
            this.numEssDischargeEndTimeHour = new C1.Win.Ribbon.RibbonNumericBox();
            this.numEssDischargeEndTimeMin = new C1.Win.Ribbon.RibbonNumericBox();
            this.groupEssDischargeParametr = new C1.Win.Ribbon.RibbonGroup();
            this.numEssDischargeSocLimit = new C1.Win.Ribbon.RibbonNumericBox();
            this.numEssDischargeOutputLimit = new C1.Win.Ribbon.RibbonNumericBox();
            this.groupEssGenParameter = new C1.Win.Ribbon.RibbonGroup();
            this.chkEssGen = new C1.Win.Ribbon.RibbonCheckBox();
            this.ribbonSeparator13 = new C1.Win.Ribbon.RibbonSeparator();
            this.numEssGenOutPower = new C1.Win.Ribbon.RibbonNumericBox();
            this.numEssGenOutPercent = new C1.Win.Ribbon.RibbonNumericBox();
            this.tabPvControl = new C1.Win.Ribbon.RibbonTab();
            this.groupPcsControl1 = new C1.Win.Ribbon.RibbonGroup();
            this.txtRtuIpAddress1 = new C1.Win.Ribbon.RibbonTextBox();
            this.txtPcsId1 = new C1.Win.Ribbon.RibbonTextBox();
            this.btnPcsSet1 = new C1.Win.Ribbon.RibbonButton();
            this.ribbonSeparator15 = new C1.Win.Ribbon.RibbonSeparator();
            this.numPcsOut1 = new C1.Win.Ribbon.RibbonNumericBox();
            this.numPcsPeriod1 = new C1.Win.Ribbon.RibbonNumericBox();
            this.btnPcsOut1 = new C1.Win.Ribbon.RibbonButton();
            this.groupPcsControl2 = new C1.Win.Ribbon.RibbonGroup();
            this.txtRtuIpAddress2 = new C1.Win.Ribbon.RibbonTextBox();
            this.txtPcsId2 = new C1.Win.Ribbon.RibbonTextBox();
            this.btnPcsSet2 = new C1.Win.Ribbon.RibbonButton();
            this.ribbonSeparator16 = new C1.Win.Ribbon.RibbonSeparator();
            this.numPcsOut2 = new C1.Win.Ribbon.RibbonNumericBox();
            this.numPcsPeriod2 = new C1.Win.Ribbon.RibbonNumericBox();
            this.btnPcsOut2 = new C1.Win.Ribbon.RibbonButton();
            this.groupPcsControl3 = new C1.Win.Ribbon.RibbonGroup();
            this.txtRtuIpAddress3 = new C1.Win.Ribbon.RibbonTextBox();
            this.txtPcsId3 = new C1.Win.Ribbon.RibbonTextBox();
            this.btnPcsSet3 = new C1.Win.Ribbon.RibbonButton();
            this.ribbonSeparator17 = new C1.Win.Ribbon.RibbonSeparator();
            this.numPcsOut3 = new C1.Win.Ribbon.RibbonNumericBox();
            this.numPcsPeriod3 = new C1.Win.Ribbon.RibbonNumericBox();
            this.btnPcsOut3 = new C1.Win.Ribbon.RibbonButton();
            this.tabBmsControl = new C1.Win.Ribbon.RibbonTab();
            this.groupControlBms = new C1.Win.Ribbon.RibbonGroup();
            this.btnDcLinkOn = new C1.Win.Ribbon.RibbonButton();
            this.btnDcLinkOff = new C1.Win.Ribbon.RibbonButton();
            this.ribbonSeparator14 = new C1.Win.Ribbon.RibbonSeparator();
            this.btnBmsFaultReset = new C1.Win.Ribbon.RibbonButton();
            this.btnBmsWatchdog = new C1.Win.Ribbon.RibbonButton();
            this.groupControlPcs1 = new C1.Win.Ribbon.RibbonGroup();
            this.numWaitPcs1 = new C1.Win.Ribbon.RibbonNumericBox();
            this.btnPcsRun1 = new C1.Win.Ribbon.RibbonButton();
            this.btnPcsStop1 = new C1.Win.Ribbon.RibbonButton();
            this.groupControlPcs2 = new C1.Win.Ribbon.RibbonGroup();
            this.numWaitPcs2 = new C1.Win.Ribbon.RibbonNumericBox();
            this.btnPcsRun2 = new C1.Win.Ribbon.RibbonButton();
            this.btnPcsStop2 = new C1.Win.Ribbon.RibbonButton();
            this.groupControlPcs3 = new C1.Win.Ribbon.RibbonGroup();
            this.numWaitPcs3 = new C1.Win.Ribbon.RibbonNumericBox();
            this.btnPcsRun3 = new C1.Win.Ribbon.RibbonButton();
            this.btnPcsStop3 = new C1.Win.Ribbon.RibbonButton();
            this.groupControlPcs = new C1.Win.Ribbon.RibbonGroup();
            this.btnPcsOutput = new C1.Win.Ribbon.RibbonButton();
            this.groupControlAxis = new C1.Win.Ribbon.RibbonGroup();
            this.btnPanelAxis = new C1.Win.Ribbon.RibbonButton();
            this.tabSetting = new C1.Win.Ribbon.RibbonTab();
            this.toolGroupSetting = new C1.Win.Ribbon.RibbonGroup();
            this.menuStripMainSettingTray = new C1.Win.Ribbon.RibbonCheckBox();
            this.chkHmiMode = new C1.Win.Ribbon.RibbonCheckBox();
            this.ribbonSeparator4 = new C1.Win.Ribbon.RibbonSeparator();
            this.chkStopTrend = new C1.Win.Ribbon.RibbonCheckBox();
            this.toolGroupSite1 = new C1.Win.Ribbon.RibbonGroup();
            this.txtDbServerSite1 = new C1.Win.Ribbon.RibbonTextBox();
            this.numericDbServerPort1 = new C1.Win.Ribbon.RibbonNumericBox();
            this.toolGroupSite2 = new C1.Win.Ribbon.RibbonGroup();
            this.txtDbServerSite2 = new C1.Win.Ribbon.RibbonTextBox();
            this.numericDbServerPort2 = new C1.Win.Ribbon.RibbonNumericBox();
            this.toolGroupSms = new C1.Win.Ribbon.RibbonGroup();
            this.btnSmsTest = new C1.Win.Ribbon.RibbonButton();
            this.ribbonTopToolBar1 = new C1.Win.Ribbon.RibbonTopToolBar();
            this.statusStripMain = new C1.Win.Ribbon.C1StatusBar();
            this.toolStripStatusDisplayLog = new C1.Win.Ribbon.RibbonLabel();
            this.txtTemp = new C1.Win.Ribbon.RibbonLabel();
            this.txtReferencePSet = new C1.Win.Ribbon.RibbonLabel();
            this.txtReferenceQSet = new C1.Win.Ribbon.RibbonLabel();
            this.txtBmsSoc = new C1.Win.Ribbon.RibbonLabel();
            this.txtPcsOutput = new C1.Win.Ribbon.RibbonLabel();
            this.txtStatusRun = new C1.Win.Ribbon.RibbonLabel();
            this.txtEmsRun = new C1.Win.Ribbon.RibbonLabel();
            this.txtInvStatus = new C1.Win.Ribbon.RibbonLabel();
            this.txtGenOut = new C1.Win.Ribbon.RibbonLabel();
            this.mainThemeController = new C1.Win.Themes.C1ThemeController();
            this.c1TopicBarDevice = new C1.Win.Command.C1TopicBar();
            this.timerEssPcs = new System.Windows.Forms.Timer(this.components);
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.commandPropertyGridControl = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkPvGenZero = new C1.Win.Ribbon.RibbonCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.c1CommandHolder1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1RibbonMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusStripMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainThemeController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1TopicBarDevice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).BeginInit();
            this.splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).BeginInit();
            this.splitContainerControl1.Panel2.SuspendLayout();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.commandPropertyGridControl)).BeginInit();
            this.SuspendLayout();
            // 
            // imageListTab
            // 
            this.imageListTab.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTab.ImageStream")));
            this.imageListTab.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListTab.Images.SetKeyName(0, "application-delay_16.png");
            this.imageListTab.Images.SetKeyName(1, "application-event_16.png");
            this.imageListTab.Images.SetKeyName(2, "application-stats_16.png");
            this.imageListTab.Images.SetKeyName(3, "application-download_16.png");
            this.imageListTab.Images.SetKeyName(4, "application-error_16.png");
            this.imageListTab.Images.SetKeyName(5, "application-gear_16.png");
            this.imageListTab.Images.SetKeyName(6, "abstract_class_16.png");
            this.imageListTab.Images.SetKeyName(7, "abstract_class-ok_16.png");
            this.imageListTab.Images.SetKeyName(8, "device_status.png");
            this.imageListTab.Images.SetKeyName(9, "waveform_data.png");
            this.imageListTab.Images.SetKeyName(10, "treenode1.png");
            this.imageListTab.Images.SetKeyName(11, "treenode2.png");
            this.imageListTab.Images.SetKeyName(12, "treenode3.png");
            this.imageListTab.Images.SetKeyName(13, "treenode4.png");
            this.imageListTab.Images.SetKeyName(14, "treenode5.png");
            this.imageListTab.Images.SetKeyName(15, "organization.png");
            // 
            // c1CommandHolder1
            // 
            this.c1CommandHolder1.Owner = this;
            // 
            // imageListBlank
            // 
            this.imageListBlank.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListBlank.ImageSize = new System.Drawing.Size(1, 20);
            this.imageListBlank.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // timerMain
            // 
            this.timerMain.Tick += new System.EventHandler(this.timerMain_Tick);
            // 
            // Tray
            // 
            this.Tray.Icon = ((System.Drawing.Icon)(resources.GetObject("Tray.Icon")));
            this.Tray.Text = "ESS-PCS Data Server";
            this.Tray.Click += new System.EventHandler(this.Tray_Click);
            // 
            // c1RibbonMenu
            // 
            this.c1RibbonMenu.ApplicationMenuHolder = this.ribbonApplicationMenu1;
            this.c1RibbonMenu.BottomToolBarHolder = this.ribbonBottomToolBar1;
            this.c1RibbonMenu.ConfigToolBarHolder = this.ribbonConfigToolBar1;
            this.c1RibbonMenu.Location = new System.Drawing.Point(0, 0);
            this.c1RibbonMenu.Name = "c1RibbonMenu";
            this.c1RibbonMenu.QatHolder = this.ribbonQat1;
            this.c1RibbonMenu.Size = new System.Drawing.Size(1253, 161);
            this.c1RibbonMenu.Tabs.Add(this.tabBasic);
            this.c1RibbonMenu.Tabs.Add(this.tabRun);
            this.c1RibbonMenu.Tabs.Add(this.tabGeneralControl);
            this.c1RibbonMenu.Tabs.Add(this.tabPvControl);
            this.c1RibbonMenu.Tabs.Add(this.tabBmsControl);
            this.c1RibbonMenu.Tabs.Add(this.tabSetting);
            this.mainThemeController.SetTheme(this.c1RibbonMenu, "(default)");
            this.c1RibbonMenu.TopToolBarHolder = this.ribbonTopToolBar1;
            // 
            // ribbonApplicationMenu1
            // 
            this.ribbonApplicationMenu1.Name = "ribbonApplicationMenu1";
            this.ribbonApplicationMenu1.TabIndex = 0;
            this.ribbonApplicationMenu1.Visible = false;
            // 
            // ribbonBottomToolBar1
            // 
            this.ribbonBottomToolBar1.Name = "ribbonBottomToolBar1";
            // 
            // ribbonConfigToolBar1
            // 
            this.ribbonConfigToolBar1.Name = "ribbonConfigToolBar1";
            // 
            // ribbonQat1
            // 
            this.ribbonQat1.Name = "ribbonQat1";
            this.ribbonQat1.Visible = false;
            // 
            // tabBasic
            // 
            this.tabBasic.Groups.Add(this.ribbonGroupFile);
            this.tabBasic.Groups.Add(this.ribbonGroupRun);
            this.tabBasic.Groups.Add(this.ribbonGroupHelp);
            this.tabBasic.Name = "tabBasic";
            this.tabBasic.TabIndex = 22;
            this.tabBasic.Text = "기본";
            // 
            // ribbonGroupFile
            // 
            this.ribbonGroupFile.Items.Add(this.menuStripMainFileRefresh);
            this.ribbonGroupFile.Items.Add(this.ribbonSeparator1);
            this.ribbonGroupFile.Items.Add(this.menuStripMainFileExit);
            this.ribbonGroupFile.LauncherTabIndex = 0;
            this.ribbonGroupFile.Name = "ribbonGroupFile";
            this.ribbonGroupFile.Text = "파일";
            // 
            // menuStripMainFileRefresh
            // 
            this.menuStripMainFileRefresh.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.tree_refresh));
            this.menuStripMainFileRefresh.Name = "menuStripMainFileRefresh";
            this.menuStripMainFileRefresh.TabIndex = 0;
            this.menuStripMainFileRefresh.Text = "새로 고침";
            this.menuStripMainFileRefresh.Click += new System.EventHandler(this.menuStripMainFileRefresh_Click);
            // 
            // ribbonSeparator1
            // 
            this.ribbonSeparator1.Name = "ribbonSeparator1";
            // 
            // menuStripMainFileExit
            // 
            this.menuStripMainFileExit.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.off));
            this.menuStripMainFileExit.Name = "menuStripMainFileExit";
            this.menuStripMainFileExit.TabIndex = 0;
            this.menuStripMainFileExit.Text = "프로그램 종료";
            this.menuStripMainFileExit.Click += new System.EventHandler(this.menuStripMainFileExit_Click);
            // 
            // ribbonGroupRun
            // 
            this.ribbonGroupRun.Items.Add(this.menuStripMainConnect);
            this.ribbonGroupRun.Items.Add(this.ribbonSeparator2);
            this.ribbonGroupRun.Items.Add(this.menuStripMainRunStart);
            this.ribbonGroupRun.Items.Add(this.ribbonSeparator9);
            this.ribbonGroupRun.Items.Add(this.menuStripMainRunStop);
            this.ribbonGroupRun.Items.Add(this.ribbonSeparator5);
            this.ribbonGroupRun.Items.Add(this.menuStripMainDisconnect);
            this.ribbonGroupRun.Items.Add(this.ribbonSeparator3);
            this.ribbonGroupRun.Items.Add(this.btnInvRun);
            this.ribbonGroupRun.Items.Add(this.btnInvStop);
            this.ribbonGroupRun.Items.Add(this.ribbonSeparator6);
            this.ribbonGroupRun.Items.Add(this.btnInvFaultClear);
            this.ribbonGroupRun.Items.Add(this.ribbonSeparator7);
            this.ribbonGroupRun.Items.Add(this.numInvSetP);
            this.ribbonGroupRun.Items.Add(this.numInvSetQ);
            this.ribbonGroupRun.Items.Add(this.ribbonSeparator8);
            this.ribbonGroupRun.Items.Add(this.btnInvSet);
            this.ribbonGroupRun.LauncherTabIndex = 0;
            this.ribbonGroupRun.Name = "ribbonGroupRun";
            this.ribbonGroupRun.Text = "실행";
            // 
            // menuStripMainConnect
            // 
            this.menuStripMainConnect.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.application_event_16));
            this.menuStripMainConnect.Name = "menuStripMainConnect";
            this.menuStripMainConnect.TabIndex = 1;
            this.menuStripMainConnect.Text = "접속";
            this.menuStripMainConnect.Click += new System.EventHandler(this.menuStripMainConnect_Click);
            // 
            // ribbonSeparator2
            // 
            this.ribbonSeparator2.Name = "ribbonSeparator2";
            // 
            // menuStripMainRunStart
            // 
            this.menuStripMainRunStart.Enabled = false;
            this.menuStripMainRunStart.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.project_debug_run_16));
            this.menuStripMainRunStart.Name = "menuStripMainRunStart";
            this.menuStripMainRunStart.TabIndex = 2;
            this.menuStripMainRunStart.Text = "시작";
            this.menuStripMainRunStart.Click += new System.EventHandler(this.menuStripMainRunStart_Click);
            // 
            // ribbonSeparator9
            // 
            this.ribbonSeparator9.Name = "ribbonSeparator9";
            // 
            // menuStripMainRunStop
            // 
            this.menuStripMainRunStop.Enabled = false;
            this.menuStripMainRunStop.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.project_debug_stop_16));
            this.menuStripMainRunStop.Name = "menuStripMainRunStop";
            this.menuStripMainRunStop.TabIndex = 3;
            this.menuStripMainRunStop.Text = "중지";
            this.menuStripMainRunStop.Click += new System.EventHandler(this.menuStripMainRunStop_Click);
            // 
            // ribbonSeparator5
            // 
            this.ribbonSeparator5.Name = "ribbonSeparator5";
            // 
            // menuStripMainDisconnect
            // 
            this.menuStripMainDisconnect.Enabled = false;
            this.menuStripMainDisconnect.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(20, 20), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.computer_delete2));
            this.menuStripMainDisconnect.Name = "menuStripMainDisconnect";
            this.menuStripMainDisconnect.TabIndex = 4;
            this.menuStripMainDisconnect.Text = "해제";
            this.menuStripMainDisconnect.Click += new System.EventHandler(this.menuStripMainDisconnect_Click);
            // 
            // ribbonSeparator3
            // 
            this.ribbonSeparator3.Name = "ribbonSeparator3";
            // 
            // btnInvRun
            // 
            this.btnInvRun.Enabled = false;
            this.btnInvRun.IconSet.Add(new C1.Framework.C1BitmapIcon("Run", new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, "Preset_SmallImages", 369));
            this.btnInvRun.Name = "btnInvRun";
            this.btnInvRun.TabIndex = 5;
            this.btnInvRun.Text = "인버터 동작";
            this.btnInvRun.Click += new System.EventHandler(this.btnInvRun_Click);
            // 
            // btnInvStop
            // 
            this.btnInvStop.Enabled = false;
            this.btnInvStop.IconSet.Add(new C1.Framework.C1BitmapIcon("Stop", new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, "Preset_SmallImages", 440));
            this.btnInvStop.Name = "btnInvStop";
            this.btnInvStop.TabIndex = 6;
            this.btnInvStop.Text = "인버터 정지";
            this.btnInvStop.Click += new System.EventHandler(this.btnInvStop_Click);
            // 
            // ribbonSeparator6
            // 
            this.ribbonSeparator6.Name = "ribbonSeparator6";
            // 
            // btnInvFaultClear
            // 
            this.btnInvFaultClear.Enabled = false;
            this.btnInvFaultClear.IconSet.Add(new C1.Framework.C1BitmapIcon("Refresh", new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, "Preset_SmallImages", 343));
            this.btnInvFaultClear.Name = "btnInvFaultClear";
            this.btnInvFaultClear.TabIndex = 7;
            this.btnInvFaultClear.Text = "Fault Clear";
            this.btnInvFaultClear.Click += new System.EventHandler(this.btnInvFaultClear_Click);
            // 
            // ribbonSeparator7
            // 
            this.ribbonSeparator7.Name = "ribbonSeparator7";
            // 
            // numInvSetP
            // 
            this.numInvSetP.DecimalPlaces = 1;
            this.numInvSetP.Format = "f";
            this.numInvSetP.Label = "유효전력 출력[kW]";
            this.numInvSetP.LabelWidth = 120;
            this.numInvSetP.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numInvSetP.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.numInvSetP.Name = "numInvSetP";
            this.numInvSetP.TabIndex = 9;
            this.numInvSetP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // numInvSetQ
            // 
            this.numInvSetQ.Label = "무효전력 출력[kVar]";
            this.numInvSetQ.LabelWidth = 120;
            this.numInvSetQ.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numInvSetQ.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.numInvSetQ.Name = "numInvSetQ";
            this.numInvSetQ.TabIndex = 11;
            this.numInvSetQ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ribbonSeparator8
            // 
            this.ribbonSeparator8.Name = "ribbonSeparator8";
            // 
            // btnInvSet
            // 
            this.btnInvSet.Enabled = false;
            this.btnInvSet.IconSet.Add(new C1.Framework.C1BitmapIcon("RunScript", new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, "Preset_SmallImages", 372));
            this.btnInvSet.Name = "btnInvSet";
            this.btnInvSet.TabIndex = 10;
            this.btnInvSet.Text = "인버터 제어";
            this.btnInvSet.Click += new System.EventHandler(this.btnInvSet_Click);
            // 
            // ribbonGroupHelp
            // 
            this.ribbonGroupHelp.Items.Add(this.menuStripMainHelpAbout);
            this.ribbonGroupHelp.LauncherTabIndex = 0;
            this.ribbonGroupHelp.Name = "ribbonGroupHelp";
            this.ribbonGroupHelp.Text = "도움말";
            // 
            // menuStripMainHelpAbout
            // 
            this.menuStripMainHelpAbout.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.project_info_16));
            this.menuStripMainHelpAbout.Name = "menuStripMainHelpAbout";
            this.menuStripMainHelpAbout.TabIndex = 0;
            this.menuStripMainHelpAbout.Text = "프로그램 정보";
            this.menuStripMainHelpAbout.Click += new System.EventHandler(this.menuStripMainHelpAbout_Click);
            // 
            // tabRun
            // 
            this.tabRun.Groups.Add(this.groupEssControlCommand);
            this.tabRun.Groups.Add(this.operationGeneralGroup);
            this.tabRun.Groups.Add(this.ribbonGroup2);
            this.tabRun.Groups.Add(this.ribbonGroup3);
            this.tabRun.Name = "tabRun";
            this.tabRun.TabIndex = 61;
            this.tabRun.Text = "실행";
            // 
            // groupEssControlCommand
            // 
            this.groupEssControlCommand.Items.Add(this.btnEssControlStart);
            this.groupEssControlCommand.Items.Add(this.ribbonSeparator10);
            this.groupEssControlCommand.Items.Add(this.btnEssControlStop);
            this.groupEssControlCommand.LauncherTabIndex = 75;
            this.groupEssControlCommand.Name = "groupEssControlCommand";
            this.groupEssControlCommand.Text = "EMS 동작";
            // 
            // btnEssControlStart
            // 
            this.btnEssControlStart.Enabled = false;
            this.btnEssControlStart.IconSet.Add(new C1.Framework.C1BitmapIcon("Run", new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, "Preset_SmallImages", 369));
            this.btnEssControlStart.Name = "btnEssControlStart";
            this.btnEssControlStart.TabIndex = 0;
            this.btnEssControlStart.Text = "START";
            this.btnEssControlStart.Click += new System.EventHandler(this.btnEssControlStart_Click);
            // 
            // ribbonSeparator10
            // 
            this.ribbonSeparator10.Name = "ribbonSeparator10";
            // 
            // btnEssControlStop
            // 
            this.btnEssControlStop.Enabled = false;
            this.btnEssControlStop.IconSet.Add(new C1.Framework.C1BitmapIcon("Stop", new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, "Preset_SmallImages", 440));
            this.btnEssControlStop.Name = "btnEssControlStop";
            this.btnEssControlStop.TabIndex = 0;
            this.btnEssControlStop.Text = "STOP";
            this.btnEssControlStop.Click += new System.EventHandler(this.btnEssControlStop_Click);
            // 
            // operationGeneralGroup
            // 
            this.operationGeneralGroup.Items.Add(this.comboBoxOperationMode);
            this.operationGeneralGroup.Items.Add(this.ribbonSeparator18);
            this.operationGeneralGroup.Items.Add(this.numEssControlPeriod);
            this.operationGeneralGroup.Items.Add(this.numEssPcsPower);
            this.operationGeneralGroup.LauncherTabIndex = 62;
            this.operationGeneralGroup.Name = "operationGeneralGroup";
            this.operationGeneralGroup.Text = "일반";
            // 
            // comboBoxOperationMode
            // 
            this.comboBoxOperationMode.DropDownStyle = C1.Win.Ribbon.RibbonComboBoxStyle.DropDownList;
            this.comboBoxOperationMode.Items.Add(this.ribbonButton1);
            this.comboBoxOperationMode.Items.Add(this.ribbonButton2);
            this.comboBoxOperationMode.Items.Add(this.ribbonButton3);
            this.comboBoxOperationMode.Items.Add(this.ribbonButton4);
            this.comboBoxOperationMode.Items.Add(this.ribbonButton5);
            this.comboBoxOperationMode.Label = "운영모드 선택";
            this.comboBoxOperationMode.LabelWidth = 80;
            this.comboBoxOperationMode.Name = "comboBoxOperationMode";
            this.comboBoxOperationMode.TabIndex = 65;
            this.comboBoxOperationMode.TextAreaWidth = 150;
            // 
            // ribbonButton1
            // 
            this.ribbonButton1.IconSet.Add(new C1.Framework.C1BitmapIcon("DefaultImage", new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, "DefaultImage", -1));
            this.ribbonButton1.Name = "ribbonButton1";
            this.ribbonButton1.TabIndex = 71;
            this.ribbonButton1.Text = "일반운영";
            // 
            // ribbonButton2
            // 
            this.ribbonButton2.IconSet.Add(new C1.Framework.C1BitmapIcon("DefaultImage", new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, "DefaultImage", -1));
            this.ribbonButton2.Name = "ribbonButton2";
            this.ribbonButton2.TabIndex = 72;
            this.ribbonButton2.Text = "PVCurtailment";
            // 
            // ribbonButton3
            // 
            this.ribbonButton3.IconSet.Add(new C1.Framework.C1BitmapIcon("DefaultImage", new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, "DefaultImage", -1));
            this.ribbonButton3.Name = "ribbonButton3";
            this.ribbonButton3.TabIndex = 73;
            this.ribbonButton3.Text = "PVESSControl";
            // 
            // ribbonButton4
            // 
            this.ribbonButton4.IconSet.Add(new C1.Framework.C1BitmapIcon("DefaultImage", new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, "DefaultImage", -1));
            this.ribbonButton4.Name = "ribbonButton4";
            this.ribbonButton4.TabIndex = 74;
            this.ribbonButton4.Text = "ESSFrequencySupport";
            // 
            // ribbonButton5
            // 
            this.ribbonButton5.IconSet.Add(new C1.Framework.C1BitmapIcon("DefaultImage", new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, "DefaultImage", -1));
            this.ribbonButton5.Name = "ribbonButton5";
            this.ribbonButton5.TabIndex = 75;
            this.ribbonButton5.Text = "ESSLoadReduction";
            // 
            // ribbonSeparator18
            // 
            this.ribbonSeparator18.Name = "ribbonSeparator18";
            // 
            // numEssControlPeriod
            // 
            this.numEssControlPeriod.Label = "제어 주기 [초]";
            this.numEssControlPeriod.LabelWidth = 80;
            this.numEssControlPeriod.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.numEssControlPeriod.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numEssControlPeriod.Name = "numEssControlPeriod";
            this.numEssControlPeriod.TabIndex = 70;
            this.numEssControlPeriod.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssControlPeriod.TextAreaWidth = 40;
            // 
            // numEssPcsPower
            // 
            this.numEssPcsPower.Label = "PCS정격 [kW]";
            this.numEssPcsPower.LabelWidth = 80;
            this.numEssPcsPower.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numEssPcsPower.Name = "numEssPcsPower";
            this.numEssPcsPower.TabIndex = 76;
            this.numEssPcsPower.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssPcsPower.TextAreaWidth = 40;
            // 
            // ribbonGroup2
            // 
            this.ribbonGroup2.LauncherTabIndex = 63;
            this.ribbonGroup2.Name = "ribbonGroup2";
            this.ribbonGroup2.Text = "Group";
            this.ribbonGroup2.Visible = false;
            // 
            // ribbonGroup3
            // 
            this.ribbonGroup3.LauncherTabIndex = 64;
            this.ribbonGroup3.Name = "ribbonGroup3";
            this.ribbonGroup3.Text = "Group";
            this.ribbonGroup3.Visible = false;
            // 
            // tabGeneralControl
            // 
            this.tabGeneralControl.Groups.Add(this.groupEssChargeTime);
            this.tabGeneralControl.Groups.Add(this.groupEssChargeParametr);
            this.tabGeneralControl.Groups.Add(this.groupEssDischargeTime);
            this.tabGeneralControl.Groups.Add(this.groupEssDischargeParametr);
            this.tabGeneralControl.Groups.Add(this.groupEssGenParameter);
            this.tabGeneralControl.Name = "tabGeneralControl";
            this.tabGeneralControl.TabIndex = 24;
            this.tabGeneralControl.Text = "일반운영";
            // 
            // groupEssChargeTime
            // 
            this.groupEssChargeTime.Items.Add(this.numEssChargeStartTimeHour);
            this.groupEssChargeTime.Items.Add(this.numEssChargeStartTimeMin);
            this.groupEssChargeTime.Items.Add(this.ribbonSeparator11);
            this.groupEssChargeTime.Items.Add(this.numEssChargeEndTimeHour);
            this.groupEssChargeTime.Items.Add(this.numEssChargeEndTimeMin);
            this.groupEssChargeTime.LauncherTabIndex = 0;
            this.groupEssChargeTime.Name = "groupEssChargeTime";
            this.groupEssChargeTime.Text = "충전 시간";
            // 
            // numEssChargeStartTimeHour
            // 
            this.numEssChargeStartTimeHour.Label = "시작 시각 [시]";
            this.numEssChargeStartTimeHour.LabelWidth = 80;
            this.numEssChargeStartTimeHour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.numEssChargeStartTimeHour.Name = "numEssChargeStartTimeHour";
            this.numEssChargeStartTimeHour.TabIndex = 0;
            this.numEssChargeStartTimeHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssChargeStartTimeHour.TextAreaWidth = 30;
            // 
            // numEssChargeStartTimeMin
            // 
            this.numEssChargeStartTimeMin.Label = "시작 시각 [분]";
            this.numEssChargeStartTimeMin.LabelWidth = 80;
            this.numEssChargeStartTimeMin.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numEssChargeStartTimeMin.Name = "numEssChargeStartTimeMin";
            this.numEssChargeStartTimeMin.TabIndex = 0;
            this.numEssChargeStartTimeMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssChargeStartTimeMin.TextAreaWidth = 30;
            // 
            // ribbonSeparator11
            // 
            this.ribbonSeparator11.Name = "ribbonSeparator11";
            // 
            // numEssChargeEndTimeHour
            // 
            this.numEssChargeEndTimeHour.Label = "종료 시각 [시]";
            this.numEssChargeEndTimeHour.LabelWidth = 80;
            this.numEssChargeEndTimeHour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.numEssChargeEndTimeHour.Name = "numEssChargeEndTimeHour";
            this.numEssChargeEndTimeHour.TabIndex = 0;
            this.numEssChargeEndTimeHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssChargeEndTimeHour.TextAreaWidth = 30;
            // 
            // numEssChargeEndTimeMin
            // 
            this.numEssChargeEndTimeMin.Label = "종료 시각 [분]";
            this.numEssChargeEndTimeMin.LabelWidth = 80;
            this.numEssChargeEndTimeMin.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numEssChargeEndTimeMin.Name = "numEssChargeEndTimeMin";
            this.numEssChargeEndTimeMin.TabIndex = 0;
            this.numEssChargeEndTimeMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssChargeEndTimeMin.TextAreaWidth = 30;
            // 
            // groupEssChargeParametr
            // 
            this.groupEssChargeParametr.Items.Add(this.numEssChargeSocLimit);
            this.groupEssChargeParametr.Items.Add(this.numEssChargeOutputLimit);
            this.groupEssChargeParametr.LauncherTabIndex = 0;
            this.groupEssChargeParametr.Name = "groupEssChargeParametr";
            this.groupEssChargeParametr.Text = "충전 설정";
            // 
            // numEssChargeSocLimit
            // 
            this.numEssChargeSocLimit.DecimalPlaces = 1;
            this.numEssChargeSocLimit.Format = "f";
            this.numEssChargeSocLimit.Label = "SOC [%]";
            this.numEssChargeSocLimit.LabelWidth = 60;
            this.numEssChargeSocLimit.Name = "numEssChargeSocLimit";
            this.numEssChargeSocLimit.TabIndex = 0;
            this.numEssChargeSocLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssChargeSocLimit.TextAreaWidth = 40;
            // 
            // numEssChargeOutputLimit
            // 
            this.numEssChargeOutputLimit.DecimalPlaces = 1;
            this.numEssChargeOutputLimit.Format = "f";
            this.numEssChargeOutputLimit.Label = "출력 [kW]";
            this.numEssChargeOutputLimit.LabelWidth = 60;
            this.numEssChargeOutputLimit.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numEssChargeOutputLimit.Name = "numEssChargeOutputLimit";
            this.numEssChargeOutputLimit.TabIndex = 0;
            this.numEssChargeOutputLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssChargeOutputLimit.TextAreaWidth = 40;
            // 
            // groupEssDischargeTime
            // 
            this.groupEssDischargeTime.Items.Add(this.numEssDischargeStartTimeHour);
            this.groupEssDischargeTime.Items.Add(this.numEssDischargeStartTimeMin);
            this.groupEssDischargeTime.Items.Add(this.ribbonSeparator12);
            this.groupEssDischargeTime.Items.Add(this.numEssDischargeEndTimeHour);
            this.groupEssDischargeTime.Items.Add(this.numEssDischargeEndTimeMin);
            this.groupEssDischargeTime.LauncherTabIndex = 0;
            this.groupEssDischargeTime.Name = "groupEssDischargeTime";
            this.groupEssDischargeTime.Text = "방전 시간";
            // 
            // numEssDischargeStartTimeHour
            // 
            this.numEssDischargeStartTimeHour.Label = "시작 시각 [시]";
            this.numEssDischargeStartTimeHour.LabelWidth = 80;
            this.numEssDischargeStartTimeHour.Name = "numEssDischargeStartTimeHour";
            this.numEssDischargeStartTimeHour.TabIndex = 0;
            this.numEssDischargeStartTimeHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssDischargeStartTimeHour.TextAreaWidth = 30;
            // 
            // numEssDischargeStartTimeMin
            // 
            this.numEssDischargeStartTimeMin.Label = "시작 시각 [분]";
            this.numEssDischargeStartTimeMin.LabelWidth = 80;
            this.numEssDischargeStartTimeMin.Name = "numEssDischargeStartTimeMin";
            this.numEssDischargeStartTimeMin.TabIndex = 0;
            this.numEssDischargeStartTimeMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssDischargeStartTimeMin.TextAreaWidth = 30;
            // 
            // ribbonSeparator12
            // 
            this.ribbonSeparator12.Name = "ribbonSeparator12";
            // 
            // numEssDischargeEndTimeHour
            // 
            this.numEssDischargeEndTimeHour.Label = "종료 시각 [시]";
            this.numEssDischargeEndTimeHour.LabelWidth = 80;
            this.numEssDischargeEndTimeHour.Name = "numEssDischargeEndTimeHour";
            this.numEssDischargeEndTimeHour.TabIndex = 0;
            this.numEssDischargeEndTimeHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssDischargeEndTimeHour.TextAreaWidth = 30;
            // 
            // numEssDischargeEndTimeMin
            // 
            this.numEssDischargeEndTimeMin.Label = "종료 시각 [분]";
            this.numEssDischargeEndTimeMin.LabelWidth = 80;
            this.numEssDischargeEndTimeMin.Name = "numEssDischargeEndTimeMin";
            this.numEssDischargeEndTimeMin.TabIndex = 0;
            this.numEssDischargeEndTimeMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssDischargeEndTimeMin.TextAreaWidth = 30;
            // 
            // groupEssDischargeParametr
            // 
            this.groupEssDischargeParametr.Items.Add(this.numEssDischargeSocLimit);
            this.groupEssDischargeParametr.Items.Add(this.numEssDischargeOutputLimit);
            this.groupEssDischargeParametr.LauncherTabIndex = 0;
            this.groupEssDischargeParametr.Name = "groupEssDischargeParametr";
            this.groupEssDischargeParametr.Text = "방전 설정";
            // 
            // numEssDischargeSocLimit
            // 
            this.numEssDischargeSocLimit.DecimalPlaces = 1;
            this.numEssDischargeSocLimit.Format = "f";
            this.numEssDischargeSocLimit.Label = "SOC [%]";
            this.numEssDischargeSocLimit.LabelWidth = 60;
            this.numEssDischargeSocLimit.Name = "numEssDischargeSocLimit";
            this.numEssDischargeSocLimit.TabIndex = 0;
            this.numEssDischargeSocLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssDischargeSocLimit.TextAreaWidth = 40;
            // 
            // numEssDischargeOutputLimit
            // 
            this.numEssDischargeOutputLimit.DecimalPlaces = 1;
            this.numEssDischargeOutputLimit.Format = "f";
            this.numEssDischargeOutputLimit.Label = "출력 [kW]";
            this.numEssDischargeOutputLimit.LabelWidth = 60;
            this.numEssDischargeOutputLimit.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numEssDischargeOutputLimit.Name = "numEssDischargeOutputLimit";
            this.numEssDischargeOutputLimit.TabIndex = 0;
            this.numEssDischargeOutputLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssDischargeOutputLimit.TextAreaWidth = 40;
            // 
            // groupEssGenParameter
            // 
            this.groupEssGenParameter.Items.Add(this.chkEssGen);
            this.groupEssGenParameter.Items.Add(this.chkPvGenZero);
            this.groupEssGenParameter.Items.Add(this.ribbonSeparator13);
            this.groupEssGenParameter.Items.Add(this.numEssGenOutPower);
            this.groupEssGenParameter.Items.Add(this.numEssGenOutPercent);
            this.groupEssGenParameter.LauncherTabIndex = 0;
            this.groupEssGenParameter.Name = "groupEssGenParameter";
            this.groupEssGenParameter.Text = "발전연계";
            // 
            // chkEssGen
            // 
            this.chkEssGen.Name = "chkEssGen";
            this.chkEssGen.TabIndex = 77;
            this.chkEssGen.Text = "연계유무";
            // 
            // ribbonSeparator13
            // 
            this.ribbonSeparator13.Name = "ribbonSeparator13";
            // 
            // numEssGenOutPower
            // 
            this.numEssGenOutPower.DecimalPlaces = 1;
            this.numEssGenOutPower.Format = "f";
            this.numEssGenOutPower.Label = "발전전력[kW]";
            this.numEssGenOutPower.LabelWidth = 80;
            this.numEssGenOutPower.Name = "numEssGenOutPower";
            this.numEssGenOutPower.TabIndex = 78;
            this.numEssGenOutPower.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssGenOutPower.TextAreaWidth = 40;
            // 
            // numEssGenOutPercent
            // 
            this.numEssGenOutPercent.DecimalPlaces = 1;
            this.numEssGenOutPercent.Format = "f";
            this.numEssGenOutPercent.Label = "충전율[%]";
            this.numEssGenOutPercent.LabelWidth = 80;
            this.numEssGenOutPercent.Name = "numEssGenOutPercent";
            this.numEssGenOutPercent.TabIndex = 79;
            this.numEssGenOutPercent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEssGenOutPercent.TextAreaWidth = 40;
            // 
            // tabPvControl
            // 
            this.tabPvControl.Groups.Add(this.groupPcsControl1);
            this.tabPvControl.Groups.Add(this.groupPcsControl2);
            this.tabPvControl.Groups.Add(this.groupPcsControl3);
            this.tabPvControl.Name = "tabPvControl";
            this.tabPvControl.TabIndex = 26;
            this.tabPvControl.Text = "출력제어";
            // 
            // groupPcsControl1
            // 
            this.groupPcsControl1.Items.Add(this.txtRtuIpAddress1);
            this.groupPcsControl1.Items.Add(this.txtPcsId1);
            this.groupPcsControl1.Items.Add(this.btnPcsSet1);
            this.groupPcsControl1.Items.Add(this.ribbonSeparator15);
            this.groupPcsControl1.Items.Add(this.numPcsOut1);
            this.groupPcsControl1.Items.Add(this.numPcsPeriod1);
            this.groupPcsControl1.Items.Add(this.btnPcsOut1);
            this.groupPcsControl1.LauncherTabIndex = 27;
            this.groupPcsControl1.Name = "groupPcsControl1";
            this.groupPcsControl1.Text = "양축식 출력 제어";
            // 
            // txtRtuIpAddress1
            // 
            this.txtRtuIpAddress1.Label = "RTU IP 주소";
            this.txtRtuIpAddress1.LabelWidth = 70;
            this.txtRtuIpAddress1.Name = "txtRtuIpAddress1";
            this.txtRtuIpAddress1.TabIndex = 30;
            this.txtRtuIpAddress1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtPcsId1
            // 
            this.txtPcsId1.Label = "PCS 아이디";
            this.txtPcsId1.LabelWidth = 70;
            this.txtPcsId1.Name = "txtPcsId1";
            this.txtPcsId1.TabIndex = 31;
            this.txtPcsId1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnPcsSet1
            // 
            this.btnPcsSet1.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(20, 20), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.button_rounded_blue_next));
            this.btnPcsSet1.Name = "btnPcsSet1";
            this.btnPcsSet1.TabIndex = 42;
            this.btnPcsSet1.Text = "설정 적용";
            this.btnPcsSet1.Click += new System.EventHandler(this.btnPcsSet_Click);
            // 
            // ribbonSeparator15
            // 
            this.ribbonSeparator15.Name = "ribbonSeparator15";
            // 
            // numPcsOut1
            // 
            this.numPcsOut1.Label = "출력 제어 [kW]";
            this.numPcsOut1.LabelWidth = 90;
            this.numPcsOut1.Name = "numPcsOut1";
            this.numPcsOut1.TabIndex = 32;
            this.numPcsOut1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numPcsOut1.TextAreaWidth = 30;
            // 
            // numPcsPeriod1
            // 
            this.numPcsPeriod1.Label = "제어 주기 [초]";
            this.numPcsPeriod1.LabelWidth = 90;
            this.numPcsPeriod1.Name = "numPcsPeriod1";
            this.numPcsPeriod1.TabIndex = 46;
            this.numPcsPeriod1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numPcsPeriod1.TextAreaWidth = 30;
            // 
            // btnPcsOut1
            // 
            this.btnPcsOut1.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(20, 20), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.button_rounded_blue_next));
            this.btnPcsOut1.Name = "btnPcsOut1";
            this.btnPcsOut1.TabIndex = 43;
            this.btnPcsOut1.Text = "출력값 전송";
            this.btnPcsOut1.Click += new System.EventHandler(this.btnPcsOut_Click);
            // 
            // groupPcsControl2
            // 
            this.groupPcsControl2.Items.Add(this.txtRtuIpAddress2);
            this.groupPcsControl2.Items.Add(this.txtPcsId2);
            this.groupPcsControl2.Items.Add(this.btnPcsSet2);
            this.groupPcsControl2.Items.Add(this.ribbonSeparator16);
            this.groupPcsControl2.Items.Add(this.numPcsOut2);
            this.groupPcsControl2.Items.Add(this.numPcsPeriod2);
            this.groupPcsControl2.Items.Add(this.btnPcsOut2);
            this.groupPcsControl2.LauncherTabIndex = 28;
            this.groupPcsControl2.Name = "groupPcsControl2";
            this.groupPcsControl2.Text = "단축식 출력 제어";
            // 
            // txtRtuIpAddress2
            // 
            this.txtRtuIpAddress2.Label = "RTU IP 주소";
            this.txtRtuIpAddress2.LabelWidth = 70;
            this.txtRtuIpAddress2.Name = "txtRtuIpAddress2";
            this.txtRtuIpAddress2.TabIndex = 34;
            this.txtRtuIpAddress2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtPcsId2
            // 
            this.txtPcsId2.Label = "PCS 아이디";
            this.txtPcsId2.LabelWidth = 70;
            this.txtPcsId2.Name = "txtPcsId2";
            this.txtPcsId2.TabIndex = 35;
            this.txtPcsId2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnPcsSet2
            // 
            this.btnPcsSet2.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(20, 20), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.button_rounded_blue_next));
            this.btnPcsSet2.Name = "btnPcsSet2";
            this.btnPcsSet2.TabIndex = 44;
            this.btnPcsSet2.Text = "설정 적용";
            this.btnPcsSet2.Click += new System.EventHandler(this.btnPcsSet_Click);
            // 
            // ribbonSeparator16
            // 
            this.ribbonSeparator16.Name = "ribbonSeparator16";
            // 
            // numPcsOut2
            // 
            this.numPcsOut2.Label = "출력 제어 [kW]";
            this.numPcsOut2.LabelWidth = 90;
            this.numPcsOut2.Name = "numPcsOut2";
            this.numPcsOut2.TabIndex = 36;
            this.numPcsOut2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numPcsOut2.TextAreaWidth = 30;
            // 
            // numPcsPeriod2
            // 
            this.numPcsPeriod2.Label = "제어 주기 [초]";
            this.numPcsPeriod2.LabelWidth = 90;
            this.numPcsPeriod2.Name = "numPcsPeriod2";
            this.numPcsPeriod2.TabIndex = 47;
            this.numPcsPeriod2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numPcsPeriod2.TextAreaWidth = 30;
            // 
            // btnPcsOut2
            // 
            this.btnPcsOut2.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(20, 20), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.button_rounded_blue_next));
            this.btnPcsOut2.Name = "btnPcsOut2";
            this.btnPcsOut2.TabIndex = 45;
            this.btnPcsOut2.Text = "출력값 전송";
            this.btnPcsOut2.Click += new System.EventHandler(this.btnPcsOut_Click);
            // 
            // groupPcsControl3
            // 
            this.groupPcsControl3.Items.Add(this.txtRtuIpAddress3);
            this.groupPcsControl3.Items.Add(this.txtPcsId3);
            this.groupPcsControl3.Items.Add(this.btnPcsSet3);
            this.groupPcsControl3.Items.Add(this.ribbonSeparator17);
            this.groupPcsControl3.Items.Add(this.numPcsOut3);
            this.groupPcsControl3.Items.Add(this.numPcsPeriod3);
            this.groupPcsControl3.Items.Add(this.btnPcsOut3);
            this.groupPcsControl3.LauncherTabIndex = 29;
            this.groupPcsControl3.Name = "groupPcsControl3";
            this.groupPcsControl3.Text = "고정식 출력 제어";
            // 
            // txtRtuIpAddress3
            // 
            this.txtRtuIpAddress3.Label = "RTU IP 주소";
            this.txtRtuIpAddress3.LabelWidth = 70;
            this.txtRtuIpAddress3.Name = "txtRtuIpAddress3";
            this.txtRtuIpAddress3.TabIndex = 37;
            this.txtRtuIpAddress3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtPcsId3
            // 
            this.txtPcsId3.Label = "PCS 아이디";
            this.txtPcsId3.LabelWidth = 70;
            this.txtPcsId3.Name = "txtPcsId3";
            this.txtPcsId3.TabIndex = 38;
            this.txtPcsId3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnPcsSet3
            // 
            this.btnPcsSet3.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(20, 20), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.button_rounded_blue_next));
            this.btnPcsSet3.Name = "btnPcsSet3";
            this.btnPcsSet3.TabIndex = 40;
            this.btnPcsSet3.Text = "설정 적용";
            this.btnPcsSet3.Click += new System.EventHandler(this.btnPcsSet_Click);
            // 
            // ribbonSeparator17
            // 
            this.ribbonSeparator17.Name = "ribbonSeparator17";
            // 
            // numPcsOut3
            // 
            this.numPcsOut3.Label = "출력 제어 [kW]";
            this.numPcsOut3.LabelWidth = 90;
            this.numPcsOut3.Name = "numPcsOut3";
            this.numPcsOut3.TabIndex = 39;
            this.numPcsOut3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numPcsOut3.TextAreaWidth = 30;
            // 
            // numPcsPeriod3
            // 
            this.numPcsPeriod3.Label = "제어 주기 [초]";
            this.numPcsPeriod3.LabelWidth = 90;
            this.numPcsPeriod3.Name = "numPcsPeriod3";
            this.numPcsPeriod3.TabIndex = 48;
            this.numPcsPeriod3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numPcsPeriod3.TextAreaWidth = 30;
            // 
            // btnPcsOut3
            // 
            this.btnPcsOut3.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(20, 20), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.button_rounded_blue_next));
            this.btnPcsOut3.Name = "btnPcsOut3";
            this.btnPcsOut3.TabIndex = 41;
            this.btnPcsOut3.Text = "출력값 전송";
            this.btnPcsOut3.Click += new System.EventHandler(this.btnPcsOut_Click);
            // 
            // tabBmsControl
            // 
            this.tabBmsControl.Groups.Add(this.groupControlBms);
            this.tabBmsControl.Groups.Add(this.groupControlPcs1);
            this.tabBmsControl.Groups.Add(this.groupControlPcs2);
            this.tabBmsControl.Groups.Add(this.groupControlPcs3);
            this.tabBmsControl.Groups.Add(this.groupControlPcs);
            this.tabBmsControl.Groups.Add(this.groupControlAxis);
            this.tabBmsControl.Name = "tabBmsControl";
            this.tabBmsControl.TabIndex = 23;
            this.tabBmsControl.Text = "제어";
            // 
            // groupControlBms
            // 
            this.groupControlBms.Items.Add(this.btnDcLinkOn);
            this.groupControlBms.Items.Add(this.btnDcLinkOff);
            this.groupControlBms.Items.Add(this.ribbonSeparator14);
            this.groupControlBms.Items.Add(this.btnBmsFaultReset);
            this.groupControlBms.Items.Add(this.btnBmsWatchdog);
            this.groupControlBms.LauncherTabIndex = 17;
            this.groupControlBms.Name = "groupControlBms";
            this.groupControlBms.Text = "BMS 제어";
            // 
            // btnDcLinkOn
            // 
            this.btnDcLinkOn.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.on));
            this.btnDcLinkOn.Name = "btnDcLinkOn";
            this.btnDcLinkOn.TabIndex = 18;
            this.btnDcLinkOn.Text = "DC Link ON";
            this.btnDcLinkOn.Click += new System.EventHandler(this.btnDcLinkOn_Click);
            // 
            // btnDcLinkOff
            // 
            this.btnDcLinkOff.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.off));
            this.btnDcLinkOff.Name = "btnDcLinkOff";
            this.btnDcLinkOff.TabIndex = 19;
            this.btnDcLinkOff.Text = "DC Link OFF";
            this.btnDcLinkOff.Click += new System.EventHandler(this.btnDcLinkOff_Click);
            // 
            // ribbonSeparator14
            // 
            this.ribbonSeparator14.Name = "ribbonSeparator14";
            // 
            // btnBmsFaultReset
            // 
            this.btnBmsFaultReset.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.reset_error));
            this.btnBmsFaultReset.Name = "btnBmsFaultReset";
            this.btnBmsFaultReset.TabIndex = 20;
            this.btnBmsFaultReset.Text = "Error Reset";
            this.btnBmsFaultReset.Click += new System.EventHandler(this.btnBmsFaultReset_Click);
            // 
            // btnBmsWatchdog
            // 
            this.btnBmsWatchdog.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(20, 20), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.alarm_ok));
            this.btnBmsWatchdog.Name = "btnBmsWatchdog";
            this.btnBmsWatchdog.TabIndex = 21;
            this.btnBmsWatchdog.Text = "Watchdog";
            this.btnBmsWatchdog.Visible = false;
            this.btnBmsWatchdog.Click += new System.EventHandler(this.btnBmsWatchdog_Click);
            // 
            // groupControlPcs1
            // 
            this.groupControlPcs1.Items.Add(this.numWaitPcs1);
            this.groupControlPcs1.Items.Add(this.btnPcsRun1);
            this.groupControlPcs1.Items.Add(this.btnPcsStop1);
            this.groupControlPcs1.LauncherTabIndex = 49;
            this.groupControlPcs1.Name = "groupControlPcs1";
            this.groupControlPcs1.Text = "양축식 PCS 제어";
            // 
            // numWaitPcs1
            // 
            this.numWaitPcs1.Label = "대기 시간 [초]";
            this.numWaitPcs1.LabelWidth = 80;
            this.numWaitPcs1.Name = "numWaitPcs1";
            this.numWaitPcs1.TabIndex = 52;
            this.numWaitPcs1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numWaitPcs1.TextAreaWidth = 30;
            this.numWaitPcs1.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // btnPcsRun1
            // 
            this.btnPcsRun1.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.on));
            this.btnPcsRun1.Name = "btnPcsRun1";
            this.btnPcsRun1.TabIndex = 53;
            this.btnPcsRun1.Text = "PCS RUN";
            this.btnPcsRun1.Click += new System.EventHandler(this.btnPcsRun_Click);
            // 
            // btnPcsStop1
            // 
            this.btnPcsStop1.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.off));
            this.btnPcsStop1.Name = "btnPcsStop1";
            this.btnPcsStop1.TabIndex = 54;
            this.btnPcsStop1.Text = "PCS STOP";
            this.btnPcsStop1.Click += new System.EventHandler(this.btnPcsStop_Click);
            // 
            // groupControlPcs2
            // 
            this.groupControlPcs2.Items.Add(this.numWaitPcs2);
            this.groupControlPcs2.Items.Add(this.btnPcsRun2);
            this.groupControlPcs2.Items.Add(this.btnPcsStop2);
            this.groupControlPcs2.LauncherTabIndex = 50;
            this.groupControlPcs2.Name = "groupControlPcs2";
            this.groupControlPcs2.Text = "단축식 PCS 제어";
            // 
            // numWaitPcs2
            // 
            this.numWaitPcs2.Label = "대기 시간 [초]";
            this.numWaitPcs2.LabelWidth = 80;
            this.numWaitPcs2.Name = "numWaitPcs2";
            this.numWaitPcs2.TabIndex = 55;
            this.numWaitPcs2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numWaitPcs2.TextAreaWidth = 30;
            this.numWaitPcs2.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // btnPcsRun2
            // 
            this.btnPcsRun2.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.on));
            this.btnPcsRun2.Name = "btnPcsRun2";
            this.btnPcsRun2.TabIndex = 56;
            this.btnPcsRun2.Text = "PCS RUN";
            this.btnPcsRun2.Click += new System.EventHandler(this.btnPcsRun_Click);
            // 
            // btnPcsStop2
            // 
            this.btnPcsStop2.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.off));
            this.btnPcsStop2.Name = "btnPcsStop2";
            this.btnPcsStop2.TabIndex = 57;
            this.btnPcsStop2.Text = "PCS STOP";
            this.btnPcsStop2.Click += new System.EventHandler(this.btnPcsStop_Click);
            // 
            // groupControlPcs3
            // 
            this.groupControlPcs3.Items.Add(this.numWaitPcs3);
            this.groupControlPcs3.Items.Add(this.btnPcsRun3);
            this.groupControlPcs3.Items.Add(this.btnPcsStop3);
            this.groupControlPcs3.LauncherTabIndex = 51;
            this.groupControlPcs3.Name = "groupControlPcs3";
            this.groupControlPcs3.Text = "고정식 PCS 제어";
            // 
            // numWaitPcs3
            // 
            this.numWaitPcs3.Label = "대기 시간 [초]";
            this.numWaitPcs3.LabelWidth = 80;
            this.numWaitPcs3.Name = "numWaitPcs3";
            this.numWaitPcs3.TabIndex = 58;
            this.numWaitPcs3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numWaitPcs3.TextAreaWidth = 30;
            this.numWaitPcs3.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // btnPcsRun3
            // 
            this.btnPcsRun3.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.on));
            this.btnPcsRun3.Name = "btnPcsRun3";
            this.btnPcsRun3.TabIndex = 59;
            this.btnPcsRun3.Text = "PCS RUN";
            this.btnPcsRun3.Click += new System.EventHandler(this.btnPcsRun_Click);
            // 
            // btnPcsStop3
            // 
            this.btnPcsStop3.IconSet.Add(new C1.Framework.C1BitmapIcon(null, new System.Drawing.Size(16, 16), System.Drawing.Color.Transparent, global::EssPcsServer.Properties.Resources.off));
            this.btnPcsStop3.Name = "btnPcsStop3";
            this.btnPcsStop3.TabIndex = 60;
            this.btnPcsStop3.Text = "PCS STOP";
            this.btnPcsStop3.Click += new System.EventHandler(this.btnPcsStop_Click);
            // 
            // groupControlPcs
            // 
            this.groupControlPcs.Items.Add(this.btnPcsOutput);
            this.groupControlPcs.LauncherTabIndex = 15;
            this.groupControlPcs.Name = "groupControlPcs";
            this.groupControlPcs.Text = "PCS";
            this.groupControlPcs.Visible = false;
            // 
            // btnPcsOutput
            // 
            this.btnPcsOutput.IconSet.Add(new C1.Framework.C1BitmapIcon("Gears", new System.Drawing.Size(32, 32), System.Drawing.Color.Transparent, "Preset_LargeImages", 124));
            this.btnPcsOutput.Name = "btnPcsOutput";
            this.btnPcsOutput.TabIndex = 0;
            this.btnPcsOutput.Text = "PCS 출력 제어";
            this.btnPcsOutput.Click += new System.EventHandler(this.btnPcsOutput_Click);
            // 
            // groupControlAxis
            // 
            this.groupControlAxis.Items.Add(this.btnPanelAxis);
            this.groupControlAxis.LauncherTabIndex = 16;
            this.groupControlAxis.Name = "groupControlAxis";
            this.groupControlAxis.Text = "AXIS";
            this.groupControlAxis.Visible = false;
            // 
            // btnPanelAxis
            // 
            this.btnPanelAxis.IconSet.Add(new C1.Framework.C1BitmapIcon("Refresh", new System.Drawing.Size(32, 32), System.Drawing.Color.Transparent, "Preset_LargeImages", 228));
            this.btnPanelAxis.Name = "btnPanelAxis";
            this.btnPanelAxis.TabIndex = 0;
            this.btnPanelAxis.Text = "태양광 패널 위치 제어";
            this.btnPanelAxis.Click += new System.EventHandler(this.btnPanelAxis_Click);
            // 
            // tabSetting
            // 
            this.tabSetting.Groups.Add(this.toolGroupSetting);
            this.tabSetting.Groups.Add(this.toolGroupSite1);
            this.tabSetting.Groups.Add(this.toolGroupSite2);
            this.tabSetting.Groups.Add(this.toolGroupSms);
            this.tabSetting.Name = "tabSetting";
            this.tabSetting.TabIndex = 25;
            this.tabSetting.Text = "설정";
            // 
            // toolGroupSetting
            // 
            this.toolGroupSetting.Items.Add(this.menuStripMainSettingTray);
            this.toolGroupSetting.Items.Add(this.chkHmiMode);
            this.toolGroupSetting.Items.Add(this.ribbonSeparator4);
            this.toolGroupSetting.Items.Add(this.chkStopTrend);
            this.toolGroupSetting.LauncherTabIndex = 66;
            this.toolGroupSetting.Name = "toolGroupSetting";
            this.toolGroupSetting.Text = "동작";
            // 
            // menuStripMainSettingTray
            // 
            this.menuStripMainSettingTray.Name = "menuStripMainSettingTray";
            this.menuStripMainSettingTray.TabIndex = 12;
            this.menuStripMainSettingTray.Text = "트레이로 이동";
            this.menuStripMainSettingTray.Click += new System.EventHandler(this.menuStripMainSettingTray_Click);
            // 
            // chkHmiMode
            // 
            this.chkHmiMode.Name = "chkHmiMode";
            this.chkHmiMode.TabIndex = 13;
            this.chkHmiMode.Text = "HMI 모드";
            this.chkHmiMode.Click += new System.EventHandler(this.chkHmiMode_Click);
            // 
            // ribbonSeparator4
            // 
            this.ribbonSeparator4.Name = "ribbonSeparator4";
            // 
            // chkStopTrend
            // 
            this.chkStopTrend.Name = "chkStopTrend";
            this.chkStopTrend.TabIndex = 14;
            this.chkStopTrend.Text = "실시간 데이터 취득 중지";
            // 
            // toolGroupSite1
            // 
            this.toolGroupSite1.Items.Add(this.txtDbServerSite1);
            this.toolGroupSite1.Items.Add(this.numericDbServerPort1);
            this.toolGroupSite1.LauncherTabIndex = 67;
            this.toolGroupSite1.Name = "toolGroupSite1";
            this.toolGroupSite1.Text = "SITE  #1";
            // 
            // txtDbServerSite1
            // 
            this.txtDbServerSite1.Label = "서버 주소";
            this.txtDbServerSite1.LabelWidth = 65;
            this.txtDbServerSite1.Name = "txtDbServerSite1";
            this.txtDbServerSite1.TabIndex = 0;
            this.txtDbServerSite1.TextAreaWidth = 250;
            // 
            // numericDbServerPort1
            // 
            this.numericDbServerPort1.Format = "d";
            this.numericDbServerPort1.Label = "서버 포트";
            this.numericDbServerPort1.LabelWidth = 65;
            this.numericDbServerPort1.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericDbServerPort1.Name = "numericDbServerPort1";
            this.numericDbServerPort1.TabIndex = 0;
            this.numericDbServerPort1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericDbServerPort1.TextAreaWidth = 50;
            // 
            // toolGroupSite2
            // 
            this.toolGroupSite2.Items.Add(this.txtDbServerSite2);
            this.toolGroupSite2.Items.Add(this.numericDbServerPort2);
            this.toolGroupSite2.LauncherTabIndex = 68;
            this.toolGroupSite2.Name = "toolGroupSite2";
            this.toolGroupSite2.Text = "SITE #2";
            // 
            // txtDbServerSite2
            // 
            this.txtDbServerSite2.Label = "서버 주소";
            this.txtDbServerSite2.LabelWidth = 65;
            this.txtDbServerSite2.Name = "txtDbServerSite2";
            this.txtDbServerSite2.TabIndex = 0;
            this.txtDbServerSite2.TextAreaWidth = 250;
            // 
            // numericDbServerPort2
            // 
            this.numericDbServerPort2.Format = "d";
            this.numericDbServerPort2.Label = "서버 포트";
            this.numericDbServerPort2.LabelWidth = 65;
            this.numericDbServerPort2.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericDbServerPort2.Name = "numericDbServerPort2";
            this.numericDbServerPort2.TabIndex = 0;
            this.numericDbServerPort2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericDbServerPort2.TextAreaWidth = 50;
            // 
            // toolGroupSms
            // 
            this.toolGroupSms.Items.Add(this.btnSmsTest);
            this.toolGroupSms.LauncherTabIndex = 69;
            this.toolGroupSms.Name = "toolGroupSms";
            this.toolGroupSms.Text = "SMS";
            this.toolGroupSms.Visible = false;
            // 
            // btnSmsTest
            // 
            this.btnSmsTest.Name = "btnSmsTest";
            this.btnSmsTest.TabIndex = 0;
            this.btnSmsTest.Text = "SMS 테스트";
            // 
            // ribbonTopToolBar1
            // 
            this.ribbonTopToolBar1.Name = "ribbonTopToolBar1";
            // 
            // statusStripMain
            // 
            this.statusStripMain.LeftPaneItems.Add(this.toolStripStatusDisplayLog);
            this.statusStripMain.Location = new System.Drawing.Point(0, 786);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.RightPaneItems.Add(this.txtTemp);
            this.statusStripMain.RightPaneItems.Add(this.txtReferencePSet);
            this.statusStripMain.RightPaneItems.Add(this.txtReferenceQSet);
            this.statusStripMain.RightPaneItems.Add(this.txtBmsSoc);
            this.statusStripMain.RightPaneItems.Add(this.txtPcsOutput);
            this.statusStripMain.RightPaneItems.Add(this.txtStatusRun);
            this.statusStripMain.RightPaneItems.Add(this.txtEmsRun);
            this.statusStripMain.RightPaneItems.Add(this.txtInvStatus);
            this.statusStripMain.RightPaneItems.Add(this.txtGenOut);
            this.statusStripMain.Size = new System.Drawing.Size(1253, 22);
            this.mainThemeController.SetTheme(this.statusStripMain, "(default)");
            // 
            // toolStripStatusDisplayLog
            // 
            this.toolStripStatusDisplayLog.Name = "toolStripStatusDisplayLog";
            this.toolStripStatusDisplayLog.Text = "서버 초기화 대기 중 ...";
            // 
            // txtTemp
            // 
            this.txtTemp.Name = "txtTemp";
            this.txtTemp.Text = "[TEMP: 0.0℃]";
            // 
            // txtReferencePSet
            // 
            this.txtReferencePSet.Name = "txtReferencePSet";
            this.txtReferencePSet.Text = "[SETP: 0000.0kW]";
            // 
            // txtReferenceQSet
            // 
            this.txtReferenceQSet.Name = "txtReferenceQSet";
            this.txtReferenceQSet.Text = "[SETQ: 0000.0kVar]";
            // 
            // txtBmsSoc
            // 
            this.txtBmsSoc.Name = "txtBmsSoc";
            this.txtBmsSoc.Text = "[SOC: 00.0%]";
            // 
            // txtPcsOutput
            // 
            this.txtPcsOutput.Name = "txtPcsOutput";
            this.txtPcsOutput.Text = "[OUT: 0000.0kW]";
            // 
            // txtStatusRun
            // 
            this.txtStatusRun.Name = "txtStatusRun";
            this.txtStatusRun.Text = " [정지] ";
            // 
            // txtEmsRun
            // 
            this.txtEmsRun.Name = "txtEmsRun";
            this.txtEmsRun.Text = "[EMS STOP]";
            // 
            // txtInvStatus
            // 
            this.txtInvStatus.Name = "txtInvStatus";
            this.txtInvStatus.Text = "[인버터상태]";
            // 
            // txtGenOut
            // 
            this.txtGenOut.Name = "txtGenOut";
            this.txtGenOut.Text = "[00:00:00 000.0kW]";
            // 
            // mainThemeController
            // 
            this.mainThemeController.Theme = "VS2013Light";
            // 
            // c1TopicBarDevice
            // 
            this.c1TopicBarDevice.AutoScrollMinSize = new System.Drawing.Size(0, 20);
            this.c1TopicBarDevice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(242)))));
            this.c1TopicBarDevice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c1TopicBarDevice.ImageList = this.imageListTab;
            this.c1TopicBarDevice.Location = new System.Drawing.Point(0, 25);
            this.c1TopicBarDevice.Name = "c1TopicBarDevice";
            this.c1TopicBarDevice.Padding = new System.Windows.Forms.Padding(10);
            this.c1TopicBarDevice.Size = new System.Drawing.Size(883, 600);
            this.c1TopicBarDevice.TabIndex = 0;
            this.mainThemeController.SetTheme(this.c1TopicBarDevice, "(default)");
            // 
            // timerEssPcs
            // 
            this.timerEssPcs.Interval = 1000;
            this.timerEssPcs.Tick += new System.EventHandler(this.timerEssPcs_Tick);
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 161);
            this.splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            this.splitContainerControl1.Panel1.Controls.Add(this.commandPropertyGridControl);
            this.splitContainerControl1.Panel1.Controls.Add(this.label1);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            this.splitContainerControl1.Panel2.Controls.Add(this.c1TopicBarDevice);
            this.splitContainerControl1.Panel2.Controls.Add(this.label2);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(1253, 625);
            this.splitContainerControl1.SplitterPosition = 360;
            this.splitContainerControl1.TabIndex = 3;
            // 
            // commandPropertyGridControl
            // 
            this.commandPropertyGridControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.commandPropertyGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commandPropertyGridControl.Location = new System.Drawing.Point(0, 25);
            this.commandPropertyGridControl.Name = "commandPropertyGridControl";
            this.commandPropertyGridControl.OptionsView.AllowReadOnlyRowAppearance = DevExpress.Utils.DefaultBoolean.True;
            this.commandPropertyGridControl.Size = new System.Drawing.Size(360, 600);
            this.commandPropertyGridControl.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(242)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(360, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = " > 시나리오 운영 정보";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mainThemeController.SetTheme(this.label1, "(default)");
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(242)))));
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(883, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = " > Device Log";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mainThemeController.SetTheme(this.label2, "(default)");
            // 
            // chkPvGenZero
            // 
            this.chkPvGenZero.Name = "chkPvGenZero";
            this.chkPvGenZero.TabIndex = 80;
            this.chkPvGenZero.Text = "출력제로";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1253, 808);
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.c1RibbonMenu);
            this.Controls.Add(this.statusStripMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.Text = "KERI TB 제어 프로그램";
            this.mainThemeController.SetTheme(this, "(default)");
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.c1CommandHolder1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1RibbonMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusStripMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainThemeController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1TopicBarDevice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).EndInit();
            this.splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).EndInit();
            this.splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.commandPropertyGridControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timerMain;
        private System.Windows.Forms.ImageList imageListTab;
        private System.Windows.Forms.ImageList imageListBlank;
        private C1.Win.Command.C1CommandHolder c1CommandHolder1;
        private System.Windows.Forms.NotifyIcon Tray;
        private C1.Win.Ribbon.C1Ribbon c1RibbonMenu;
        private C1.Win.Ribbon.RibbonApplicationMenu ribbonApplicationMenu1;
        private C1.Win.Ribbon.RibbonConfigToolBar ribbonConfigToolBar1;
        private C1.Win.Ribbon.RibbonQat ribbonQat1;
        private C1.Win.Ribbon.RibbonTab tabBasic;
        private C1.Win.Ribbon.RibbonGroup ribbonGroupFile;
        private C1.Win.Ribbon.RibbonGroup ribbonGroupHelp;
        private C1.Win.Ribbon.RibbonButton menuStripMainHelpAbout;
        private C1.Win.Ribbon.RibbonButton menuStripMainFileRefresh;
        private C1.Win.Ribbon.RibbonButton menuStripMainFileExit;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator1;
        private C1.Win.Ribbon.RibbonGroup ribbonGroupRun;
        private C1.Win.Ribbon.RibbonButton menuStripMainRunStart;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator9;
        private C1.Win.Ribbon.RibbonButton menuStripMainRunStop;
        private C1.Win.Ribbon.C1StatusBar statusStripMain;
        private C1.Win.Ribbon.RibbonLabel toolStripStatusDisplayLog;
        private C1.Win.Ribbon.RibbonLabel txtStatusRun;
        private C1.Win.Themes.C1ThemeController mainThemeController;
        private C1.Win.Ribbon.RibbonBottomToolBar ribbonBottomToolBar1;
        private C1.Win.Ribbon.RibbonTopToolBar ribbonTopToolBar1;
        private C1.Win.Ribbon.RibbonTab tabSetting;
        private C1.Win.Ribbon.RibbonGroup toolGroupSetting;
        private C1.Win.Ribbon.RibbonCheckBox menuStripMainSettingTray;
        private C1.Win.Ribbon.RibbonButton menuStripMainConnect;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator2;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator5;
        private C1.Win.Ribbon.RibbonButton menuStripMainDisconnect;
        private C1.Win.Ribbon.RibbonGroup toolGroupSite1;
        private C1.Win.Ribbon.RibbonNumericBox numericDbServerPort1;
        private C1.Win.Ribbon.RibbonCheckBox chkHmiMode;
        private C1.Win.Ribbon.RibbonTextBox txtDbServerSite1;
        private C1.Win.Ribbon.RibbonLabel txtBmsSoc;
        private C1.Win.Ribbon.RibbonLabel txtPcsOutput;
        private C1.Win.Ribbon.RibbonLabel txtTemp;
        private C1.Win.Ribbon.RibbonLabel txtReferencePSet;
        private System.Windows.Forms.Timer timerEssPcs;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator3;
        private C1.Win.Ribbon.RibbonButton btnInvSet;
        private C1.Win.Ribbon.RibbonGroup toolGroupSms;
        private C1.Win.Ribbon.RibbonButton btnSmsTest;
        private C1.Win.Ribbon.RibbonGroup toolGroupSite2;
        private C1.Win.Ribbon.RibbonTextBox txtDbServerSite2;
        private C1.Win.Ribbon.RibbonNumericBox numericDbServerPort2;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator4;
        private C1.Win.Ribbon.RibbonButton btnInvRun;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator6;
        private C1.Win.Ribbon.RibbonButton btnInvStop;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator7;
        private C1.Win.Ribbon.RibbonNumericBox numInvSetP;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator8;
        private C1.Win.Ribbon.RibbonTab tabGeneralControl;
        private C1.Win.Ribbon.RibbonGroup groupEssControlCommand;
        private C1.Win.Ribbon.RibbonButton btnEssControlStart;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator10;
        private C1.Win.Ribbon.RibbonButton btnEssControlStop;
        private C1.Win.Ribbon.RibbonGroup groupEssChargeTime;
        private C1.Win.Ribbon.RibbonNumericBox numEssChargeStartTimeHour;
        private C1.Win.Ribbon.RibbonNumericBox numEssChargeStartTimeMin;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator11;
        private C1.Win.Ribbon.RibbonNumericBox numEssChargeEndTimeHour;
        private C1.Win.Ribbon.RibbonNumericBox numEssChargeEndTimeMin;
        private C1.Win.Ribbon.RibbonGroup groupEssChargeParametr;
        private C1.Win.Ribbon.RibbonNumericBox numEssChargeSocLimit;
        private C1.Win.Ribbon.RibbonNumericBox numEssChargeOutputLimit;
        private C1.Win.Ribbon.RibbonGroup groupEssDischargeTime;
        private C1.Win.Ribbon.RibbonGroup groupEssDischargeParametr;
        private C1.Win.Ribbon.RibbonNumericBox numEssDischargeStartTimeHour;
        private C1.Win.Ribbon.RibbonNumericBox numEssDischargeStartTimeMin;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator12;
        private C1.Win.Ribbon.RibbonNumericBox numEssDischargeEndTimeHour;
        private C1.Win.Ribbon.RibbonNumericBox numEssDischargeEndTimeMin;
        private C1.Win.Ribbon.RibbonNumericBox numEssDischargeSocLimit;
        private C1.Win.Ribbon.RibbonNumericBox numEssDischargeOutputLimit;
        private C1.Win.Ribbon.RibbonLabel txtEmsRun;
        private C1.Win.Ribbon.RibbonGroup groupEssGenParameter;
        private C1.Win.Ribbon.RibbonCheckBox chkEssGen;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator13;
        private C1.Win.Ribbon.RibbonNumericBox numEssGenOutPower;
        private C1.Win.Ribbon.RibbonNumericBox numEssGenOutPercent;
        private C1.Win.Ribbon.RibbonLabel txtInvStatus;
        private C1.Win.Ribbon.RibbonNumericBox numEssControlPeriod;
        private C1.Win.Ribbon.RibbonLabel txtGenOut;
        private C1.Win.Ribbon.RibbonButton btnInvFaultClear;
        private C1.Win.Ribbon.RibbonTab tabBmsControl;
        private C1.Win.Ribbon.RibbonGroup groupControlPcs;
        private C1.Win.Ribbon.RibbonButton btnPcsOutput;
        private C1.Win.Ribbon.RibbonGroup groupControlAxis;
        private C1.Win.Ribbon.RibbonButton btnPanelAxis;
        private C1.Win.Ribbon.RibbonLabel txtReferenceQSet;
        private C1.Win.Ribbon.RibbonNumericBox numInvSetQ;
        private C1.Win.Ribbon.RibbonCheckBox chkStopTrend;
        private C1.Win.Ribbon.RibbonGroup groupControlBms;
        private C1.Win.Ribbon.RibbonButton btnDcLinkOn;
        private C1.Win.Ribbon.RibbonButton btnDcLinkOff;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator14;
        private C1.Win.Ribbon.RibbonButton btnBmsFaultReset;
        private C1.Win.Ribbon.RibbonButton btnBmsWatchdog;
        private C1.Win.Ribbon.RibbonTab tabPvControl;
        private C1.Win.Ribbon.RibbonGroup groupPcsControl1;
        private C1.Win.Ribbon.RibbonTextBox txtRtuIpAddress1;
        private C1.Win.Ribbon.RibbonTextBox txtPcsId1;
        private C1.Win.Ribbon.RibbonNumericBox numPcsOut1;
        private C1.Win.Ribbon.RibbonGroup groupPcsControl2;
        private C1.Win.Ribbon.RibbonGroup groupPcsControl3;
        private C1.Win.Ribbon.RibbonTextBox txtRtuIpAddress2;
        private C1.Win.Ribbon.RibbonTextBox txtPcsId2;
        private C1.Win.Ribbon.RibbonNumericBox numPcsOut2;
        private C1.Win.Ribbon.RibbonTextBox txtRtuIpAddress3;
        private C1.Win.Ribbon.RibbonTextBox txtPcsId3;
        private C1.Win.Ribbon.RibbonNumericBox numPcsOut3;
        private C1.Win.Ribbon.RibbonButton btnPcsSet3;
        private C1.Win.Ribbon.RibbonButton btnPcsOut3;
        private C1.Win.Ribbon.RibbonButton btnPcsSet1;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator15;
        private C1.Win.Ribbon.RibbonButton btnPcsOut1;
        private C1.Win.Ribbon.RibbonButton btnPcsSet2;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator16;
        private C1.Win.Ribbon.RibbonButton btnPcsOut2;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator17;
        private C1.Win.Ribbon.RibbonNumericBox numPcsPeriod1;
        private C1.Win.Ribbon.RibbonNumericBox numPcsPeriod2;
        private C1.Win.Ribbon.RibbonNumericBox numPcsPeriod3;
        private C1.Win.Ribbon.RibbonGroup groupControlPcs1;
        private C1.Win.Ribbon.RibbonGroup groupControlPcs2;
        private C1.Win.Ribbon.RibbonGroup groupControlPcs3;
        private C1.Win.Ribbon.RibbonNumericBox numWaitPcs1;
        private C1.Win.Ribbon.RibbonButton btnPcsRun1;
        private C1.Win.Ribbon.RibbonButton btnPcsStop1;
        private C1.Win.Ribbon.RibbonNumericBox numWaitPcs2;
        private C1.Win.Ribbon.RibbonButton btnPcsRun2;
        private C1.Win.Ribbon.RibbonButton btnPcsStop2;
        private C1.Win.Ribbon.RibbonNumericBox numWaitPcs3;
        private C1.Win.Ribbon.RibbonButton btnPcsRun3;
        private C1.Win.Ribbon.RibbonButton btnPcsStop3;
        private C1.Win.Ribbon.RibbonTab tabRun;
        private C1.Win.Ribbon.RibbonGroup operationGeneralGroup;
        private C1.Win.Ribbon.RibbonComboBox comboBoxOperationMode;
        private C1.Win.Ribbon.RibbonGroup ribbonGroup2;
        private C1.Win.Ribbon.RibbonGroup ribbonGroup3;
        private C1.Win.Ribbon.RibbonSeparator ribbonSeparator18;
        private C1.Win.Ribbon.RibbonButton ribbonButton1;
        private C1.Win.Ribbon.RibbonButton ribbonButton2;
        private C1.Win.Ribbon.RibbonButton ribbonButton3;
        private C1.Win.Ribbon.RibbonButton ribbonButton4;
        private C1.Win.Ribbon.RibbonButton ribbonButton5;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private C1.Win.Command.C1TopicBar c1TopicBarDevice;
        private DevExpress.XtraVerticalGrid.PropertyGridControl commandPropertyGridControl;
        private C1.Win.Ribbon.RibbonNumericBox numEssPcsPower;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private C1.Win.Ribbon.RibbonCheckBox chkPvGenZero;
    }
}

