using FutureServerCore;
using FutureServerCore.Core;
using GUI.Properties;
using LidorSystems.IntegralUI.Containers;
using log4net;
using MySql.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Opc.UaFx;
using OpcCustom;
using PPAUtils;
using Robot;
using RSACommon;
using RSACommon.Configuration;
using RSACommon.Event;
using RSACommon.GraphicsForm;
using RSACommon.Points;
using RSACommon.ProgramParser;
using RSACommon.Service;
using RSACommon.WebApiDefinitions;
using RSAInterface;
using RSAInterface.Logger;
using RSAPoints.ConcretePoints;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using WebApi;
using xDialog;

namespace GUI
{
    public partial class FormApp : Form
    {

        //core instance
        public static Core myCore;
        OpcClientService ccService = null;
        MySQLService mysqlService = null;

        private LidorSystems.IntegralUI.Containers.TabPage lastPage { get; set; } = null;

        private readonly SplashScreen _splashScreen = null;
        private Form _configForm { get; set; } = null;
        private Form _clientForm { get; set; } = null;

        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private ReadProgramsService _readProgramService { get; set; } = null;

        //create static instance of gui configurator (singleton)
        public static Configurator guiConfigurator = new Configurator();
        //create static instance of input configurator (singleton)
        public static Configurator inputConfigurator = new Configurator();
        //create static instance of output configurator (singleton)
        public static Configurator outputConfigurator = new Configurator();
        //create static instance of alarms configurator (singleton)
        public static Configurator alarmsConfigurator = new Configurator();
        //create static instance of auto configurator (singleton)
        public static Configurator autoConfigurator = new Configurator();
        public static string loginLevel = "Operador";
        public static string loginPassword = "";
        public static string M1PrgName = "";
        public static string M2PrgName = "";
        public static string M3PrgName1 = "";
        public static string M4PrgName = "";
        public static string M3PrgName2 = "";

        public static string M1inc = "";
        public static string M2inc = "";
        public static string M3inc = "";
        public static string M4inc = "";
        public static string M5inc = "";
        public static string M6inc = "";
        public static string M3InLine = "0";
        public static bool restartApp = false;

        public DateTime dt = DateTime.Now;

        public Dictionary<int, bool> M1InputDictionary = new Dictionary<int, bool>();
        public Dictionary<int, bool> M1OutputDictionary = new Dictionary<int, bool>();
        public Dictionary<int, bool> M2InputDictionary = new Dictionary<int, bool>();
        public Dictionary<int, bool> M2OutputDictionary = new Dictionary<int, bool>();
        public Dictionary<int, bool> M3InputDictionary = new Dictionary<int, bool>();
        public Dictionary<int, bool> M3OutputDictionary = new Dictionary<int, bool>();
        public Dictionary<int, bool> M4InputDictionary = new Dictionary<int, bool>();
        public Dictionary<int, bool> M4OutputDictionary = new Dictionary<int, bool>();
        public Dictionary<int, bool> M5InputDictionary = new Dictionary<int, bool>();
        public Dictionary<int, bool> M5OutputDictionary = new Dictionary<int, bool>();

        public Dictionary<int, short> M1AlarmsDictionary = new Dictionary<int, short>();
        public Dictionary<int, short> M2AlarmsDictionary = new Dictionary<int, short>();
        public Dictionary<int, short> M3AlarmsDictionary = new Dictionary<int, short>();
        public Dictionary<int, short> M4AlarmsDictionary = new Dictionary<int, short>();
        public Dictionary<int, short> M5AlarmsDictionary = new Dictionary<int, short>();

        public Dictionary<int, short> M1StateDictionary = new Dictionary<int, short>();
        public Dictionary<int, short> M2StateDictionary = new Dictionary<int, short>();
        public Dictionary<int, short> M3StateDictionary = new Dictionary<int, short>();
        public Dictionary<int, short> M4StateDictionary = new Dictionary<int, short>();
        public Dictionary<int, short> M5StateDictionary = new Dictionary<int, short>();

        public Dictionary<int, short> M1AutoDictionary = new Dictionary<int, short>();
        public Dictionary<int, short> M2AutoDictionary = new Dictionary<int, short>();
        public Dictionary<int, short> M3AutoDictionary = new Dictionary<int, short>();
        public Dictionary<int, short> M4AutoDictionary = new Dictionary<int, short>();
        public Dictionary<int, short> M5AutoDictionary = new Dictionary<int, short>();
        public Dictionary<int, short> M6AutoDictionary = new Dictionary<int, short>();
        public Dictionary<int, string> ModelNameDictionary = new Dictionary<int, string>();
        public Dictionary<int, string> DescriptionDictionary = new Dictionary<int, string>();

        public Pen pBlack = new Pen(Color.Black, 10);
        public Brush bBlack = new SolidBrush(Color.Black);
        public Pen pRed = new Pen(Color.Red, 10);
        public Brush bRed = new SolidBrush(Color.Red);
        public Pen pOrange = new Pen(Color.Orange, 10);
        public Brush bOrange = new SolidBrush(Color.Orange);
        public Pen pDarkOrange = new Pen(Color.DarkOrange, 10);
        public Brush bDarkOrange = new SolidBrush(Color.DarkOrange);
        public Pen pc1 = new Pen(Color.FromArgb(59, 130, 246), 10);
        public Brush bc1 = new SolidBrush(Color.FromArgb(59, 130, 246));
        public Pen pc2 = new Pen(Color.FromArgb(107, 227, 162), 10);
        public Brush bc2 = new SolidBrush(Color.FromArgb(107, 227, 162));

        private ContextMenuStrip languageMenu;
        private string currentLanguage = "es";

        private string currentLanguageCode = "en";

        private Dictionary<string, Dictionary<string, string>> translations;

        public enum Priority
        {
            normal = 0,
            high = 1,
            critical = 2
        }

        public enum Machine
        {
            trimmer = 0,
            padprintInt = 1,
            padprintExt = 2,
            padLaser = 3,
            manipulator = 4,
            oven = 5,
            line = 6
        }
        public FormApp(SplashScreen splashScreen)
        {
            _splashScreen = splashScreen;
            _splashScreen?.WriteOnTextboxAsync($"Initialization...");

            SetEvent();

            InitCore();

            InitializeComponent();

            InitializeLanguageMenu();

            InitGUI();

            _splashScreen?.WriteOnTextboxAsync($"Set the GUI");
            StartDiagnosticGUI();
            _splashScreen?.WriteOnTextboxAsync($"Loaded Diagnostic File");

            //Splash Screen filler
            _splashScreen?.WriteOnTextboxAsync($"Update GUI syncrozionation Thread Started");
        }

        private async void InitCore()
        {
            myCore = new Core("PlasticCore");
            myCore.LoadConfiguration(myCore.ConfigFile);

            _splashScreen?.WriteOnTextboxAsync($"Init Core Configuration");

            string logName = $"Log\\{DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")}.log";
            LoggerConfigurator loadedloggerConfigurator = new LoggerConfigurator("LoggerConfigurations.json").Load().SetAllLogFileName(logName).Save();

            myCore.AddScoped<Diagnostic.Core.Diagnostic>();
            myCore.AddScoped<OpcClientService>();
            myCore.AddScoped<ReadProgramsService>();
            myCore.AddScoped<MySQLService>();

            var listOfService = myCore.CreateServiceList(myCore.CoreConfigurations, loadedloggerConfigurator);

            List<IService> listFound = myCore.FindPerType(typeof(OpcClientService));

            foreach (IService serv in listFound)
            {
                if (serv.ServiceURI == new Uri(Properties.Settings.Default.OpcClient_1_URI) && serv is OpcClientService clientOpcService)
                {
                    ccService = clientOpcService;
                    break;
                }
            }

            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                RSACustomEvents.ServiceHasLoadProgramEvent += RSACustomEvents_ServiceHasLoadProgramEvent;

                StandardProgramParser standardParser = new StandardProgramParser();
                progRS.SetProgramParser(standardParser);
                //await progRS.LoadProgramAsync<PointAxis>();

            }

            listFound = myCore.FindPerType(typeof(MySQLService));
            if (listFound[0] is MySQLService tmpService) mysqlService = tmpService;

            mysqlService.AddTable<recipies>("models");
            mysqlService.AddTable<padlaserprogram>("padlaserprograms");

            foreach (var service in listOfService)
            {
                _splashScreen?.WriteOnTextboxAsync($"Service: {service.Name} loaded");
            }

            if (ccService != null)
            {
                ccService.SetObjectData(new PlasticOpcClientConfig().Config());
            }

            myCore?.Start();

            _splashScreen?.WriteOnTextboxAsync($"Core Configuration ended");
            _splashScreen?.WriteOnTextboxAsync($"Core Started");
        }

        private void InitGUI()
        {
            InitLastParameter();
            this.Location = new Point(0, 0);
            Size formSize = new Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
            this.Size = new Size(formSize.Width, formSize.Height);
        }

        private async void InitLastParameter()
        {
            //load all config files
            guiConfigurator.LoadFromFile("guiconfig.xml", Configurator.FileType.Xml);
            RefreshModelNameComboBox();
            InitGUISettings();
            //inputConfigurator.LoadFromFile("inputconfig.xml", Configurator.FileType.Xml);
            inputConfigurator.LoadFromFile("inputLocalizationConfig.xml", Configurator.FileType.Xml);
            //outputConfigurator.LoadFromFile("outputconfig.xml", Configurator.FileType.Xml);
            outputConfigurator.LoadFromFile("outputLocalizationConfig.xml", Configurator.FileType.Xml);

            alarmsConfigurator.LoadFromFile("alarmsconfig.xml", Configurator.FileType.Xml);
            autoConfigurator.LoadFromFile("autoconfig.xml", Configurator.FileType.Xml);

            LoadSavedLanguage();

            #region (* init datagridviewM1 *)
            dataGridViewM1TeachPoints.Rows.Add(4);
            dataGridViewM1TeachPoints.Rows[0].Cells[0].Value = 1;
            dataGridViewM1TeachPoints.Rows[1].Cells[0].Value = 2;
            dataGridViewM1TeachPoints.Rows[2].Cells[0].Value = 3;
            dataGridViewM1TeachPoints.Rows[3].Cells[0].Value = 4;

            dataGridViewM1TeachPoints.Rows[0].Cells[1].Value = 100;
            dataGridViewM1TeachPoints.Rows[1].Cells[1].Value = 200;
            dataGridViewM1TeachPoints.Rows[2].Cells[1].Value = 300;
            dataGridViewM1TeachPoints.Rows[3].Cells[1].Value = 400;

            dataGridViewM1TeachPoints.Rows[0].Cells[2].Value = 10;
            dataGridViewM1TeachPoints.Rows[1].Cells[2].Value = 20;
            dataGridViewM1TeachPoints.Rows[2].Cells[2].Value = 30;
            dataGridViewM1TeachPoints.Rows[3].Cells[2].Value = 40;
            dataGridViewM1TeachPoints.ClearSelection();

            dataGridViewM1TestPoints.Rows.Add(4);
            dataGridViewM1TestPoints.Rows[0].Cells[0].Value = 1;
            dataGridViewM1TestPoints.Rows[1].Cells[0].Value = 2;
            dataGridViewM1TestPoints.Rows[2].Cells[0].Value = 3;
            dataGridViewM1TestPoints.Rows[3].Cells[0].Value = 4;

            dataGridViewM1TestPoints.Rows[0].Cells[1].Value = 100;
            dataGridViewM1TestPoints.Rows[1].Cells[1].Value = 200;
            dataGridViewM1TestPoints.Rows[2].Cells[1].Value = 300;
            dataGridViewM1TestPoints.Rows[3].Cells[1].Value = 400;

            dataGridViewM1TestPoints.Rows[0].Cells[2].Value = 10;
            dataGridViewM1TestPoints.Rows[1].Cells[2].Value = 20;
            dataGridViewM1TestPoints.Rows[2].Cells[2].Value = 30;
            dataGridViewM1TestPoints.Rows[3].Cells[2].Value = 40;

            dataGridViewM1TestPoints.ClearSelection();
            #endregion

            #region (* init datagridviewM2 *)
            dataGridViewM2TeachPoints.Rows.Add(4);
            dataGridViewM2TeachPoints.Rows[0].Cells[0].Value = 1;
            dataGridViewM2TeachPoints.Rows[1].Cells[0].Value = 2;
            dataGridViewM2TeachPoints.Rows[2].Cells[0].Value = 3;
            dataGridViewM2TeachPoints.Rows[3].Cells[0].Value = 4;

            dataGridViewM2TeachPoints.Rows[0].Cells[1].Value = 100;
            dataGridViewM2TeachPoints.Rows[1].Cells[1].Value = 200;
            dataGridViewM2TeachPoints.Rows[2].Cells[1].Value = 300;
            dataGridViewM2TeachPoints.Rows[3].Cells[1].Value = 400;

            dataGridViewM2TeachPoints.Rows[0].Cells[2].Value = 10;
            dataGridViewM2TeachPoints.Rows[1].Cells[2].Value = 20;
            dataGridViewM2TeachPoints.Rows[2].Cells[2].Value = 30;
            dataGridViewM2TeachPoints.Rows[3].Cells[2].Value = 40;
            dataGridViewM2TeachPoints.ClearSelection();

            dataGridViewM2TestPoints.Rows.Add(4);
            dataGridViewM2TestPoints.Rows[0].Cells[0].Value = 1;
            dataGridViewM2TestPoints.Rows[1].Cells[0].Value = 2;
            dataGridViewM2TestPoints.Rows[2].Cells[0].Value = 3;
            dataGridViewM2TestPoints.Rows[3].Cells[0].Value = 4;

            dataGridViewM2TestPoints.Rows[0].Cells[1].Value = 100;
            dataGridViewM2TestPoints.Rows[1].Cells[1].Value = 200;
            dataGridViewM2TestPoints.Rows[2].Cells[1].Value = 300;
            dataGridViewM2TestPoints.Rows[3].Cells[1].Value = 400;

            dataGridViewM2TestPoints.Rows[0].Cells[2].Value = 10;
            dataGridViewM2TestPoints.Rows[1].Cells[2].Value = 20;
            dataGridViewM2TestPoints.Rows[2].Cells[2].Value = 30;
            dataGridViewM2TestPoints.Rows[3].Cells[2].Value = 40;

            dataGridViewM2TestPoints.ClearSelection();
            #endregion

            #region(* init datagridviewM3 *)
            dataGridViewM3TeachPoints.Rows.Add(4);
            dataGridViewM3TeachPoints.Rows[0].Cells[0].Value = 1;
            dataGridViewM3TeachPoints.Rows[1].Cells[0].Value = 2;
            dataGridViewM3TeachPoints.Rows[2].Cells[0].Value = 3;
            dataGridViewM3TeachPoints.Rows[3].Cells[0].Value = 4;

            dataGridViewM3TeachPoints.Rows[0].Cells[1].Value = 100;
            dataGridViewM3TeachPoints.Rows[1].Cells[1].Value = 200;
            dataGridViewM3TeachPoints.Rows[2].Cells[1].Value = 300;
            dataGridViewM3TeachPoints.Rows[3].Cells[1].Value = 400;

            dataGridViewM3TeachPoints.Rows[0].Cells[2].Value = 10;
            dataGridViewM3TeachPoints.Rows[1].Cells[2].Value = 20;
            dataGridViewM3TeachPoints.Rows[2].Cells[2].Value = 30;
            dataGridViewM3TeachPoints.Rows[3].Cells[2].Value = 40;
            dataGridViewM3TeachPoints.ClearSelection();

            dataGridViewM3TestPoints.Rows.Add(4);
            dataGridViewM3TestPoints.Rows[0].Cells[0].Value = 1;
            dataGridViewM3TestPoints.Rows[1].Cells[0].Value = 2;
            dataGridViewM3TestPoints.Rows[2].Cells[0].Value = 3;
            dataGridViewM3TestPoints.Rows[3].Cells[0].Value = 4;

            dataGridViewM3TestPoints.Rows[0].Cells[1].Value = 100;
            dataGridViewM3TestPoints.Rows[1].Cells[1].Value = 200;
            dataGridViewM3TestPoints.Rows[2].Cells[1].Value = 300;
            dataGridViewM3TestPoints.Rows[3].Cells[1].Value = 400;

            dataGridViewM3TestPoints.Rows[0].Cells[2].Value = 10;
            dataGridViewM3TestPoints.Rows[1].Cells[2].Value = 20;
            dataGridViewM3TestPoints.Rows[2].Cells[2].Value = 30;
            dataGridViewM3TestPoints.Rows[3].Cells[2].Value = 40;

            dataGridViewM3TestPoints.ClearSelection();
            #endregion



            #region(* init tabControlMain *)
            tabControlMain.SelectedPage = tabPageT0;
            if (M3InLine == "1")
            {
                groupBoxM3.Visible = true;
                groupBoxM6.Visible = true;
            }
            else
            {
                groupBoxM3.Visible = false;
                groupBoxM6.Visible = false;
            }
            #endregion

            #region(* init T0 *)
            TimeZone zone = TimeZone.CurrentTimeZone;
            DateTime local = zone.ToLocalTime(DateTime.Now);
            toolStripStatusLabelDateTime.Text = local.ToString();
            toolStripStatusLabelSN.Text = Properties.Settings.Default.RSASN;
            AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.line, "PBoots application started");
            dataGridViewMessages.ClearSelection();

            checkBoxM1Inclusion.Text = "";
            checkBoxM2Inclusion.Text = "";
            checkBoxM3Inclusion.Text = "";
            checkBoxM4Inclusion.Text = "";
            checkBoxM5Inclusion.Text = "";
            checkBoxM6Inclusion.Text = "";

            #endregion


            InitAutoSettings();

            //DateTime value = DateTime.Now;
            int i = 1;
            for (i = 1; i <= 48; i++)
            {
                M1InputDictionary.Add(i, false);
                M1OutputDictionary.Add(i, false);
                M2InputDictionary.Add(i, false);
                M2OutputDictionary.Add(i, false);
                M3InputDictionary.Add(i, false);
                M3OutputDictionary.Add(i, false);
                M4InputDictionary.Add(i, false);
                M4OutputDictionary.Add(i, false);
                M5InputDictionary.Add(i, false);
                M5OutputDictionary.Add(i, false);
            }

            for (i = 1; i <= 64; i++)
            {
                M1AlarmsDictionary.Add(i, 0);
                M2AlarmsDictionary.Add(i, 0);
                M3AlarmsDictionary.Add(i, 0);
                M4AlarmsDictionary.Add(i, 0);
                M5AlarmsDictionary.Add(i, 0);
            }

            for (i = 1; i <= 4; i++)
            {
                M1StateDictionary.Add(i, 0);
            }

            for (i = 1; i <= 5; i++)
            {
                M5StateDictionary.Add(i, 0);
            }

            for (i = 1; i <= 3; i++)
            {
                M2StateDictionary.Add(i, 0);
                M3StateDictionary.Add(i, 0);
                M4StateDictionary.Add(i, 0);
            }

            for (i = 1; i <= 4; i++)
            {
                M1AutoDictionary.Add(i, 0);
                M2AutoDictionary.Add(i, 0);
                M4AutoDictionary.Add(i, 0);
            }
            for (i = 1; i <= 5; i++)
            {
                M3AutoDictionary.Add(i, 0);
                M5AutoDictionary.Add(i, 0);
            }
            M6AutoDictionary.Add(1, 0);
            M6AutoDictionary.Add(4, 0);
        }
        public void Start()
        {
            //myCore.Start();
        }

        public async void SetEvent()
        {

        }

        public void StartUpdateTask()
        {
            Task.Run(async () => await UpdateOPCUAStatus(TimeSpan.FromMilliseconds(1000), _cancellationTokenSource));
        }

        private void RSACustomEvents_ServiceHasLoadProgramEvent(object sender, RSACustomEvents.ProgramsReadEndedEventArgs e)
        {
            if (e.Service is ReadProgramsService progRS)
            {
                //modelCombobox.Items.Clear();
                //modelCombobox.Items.AddRange(progRS.ModelDictionary.Keys.ToArray<string>());
            }
        }
        public LidorSystems.IntegralUI.Containers.TabPage current = null;
        private void tabControlMain_SelectedPageChanged(object sender, LidorSystems.IntegralUI.ObjectEventArgs e)
        {
            if (this.tabControlMain.SelectedPage != null)
            {
                LidorSystems.IntegralUI.Containers.TabPage nextPage = null;
                this.tabControlMain.SelectedPage.TabShape = TabShape.Rectangular;
                this.tabControlMain.SelectedPage.TabStripPlacement = TabStripPlacement.Left;
                LidorSystems.IntegralUI.Containers.TabPage parentPage = this.tabControlMain.SelectedPage;

                if (parentPage.Pages.Count != 0) nextPage = parentPage.Pages[0];
                //    else nextPage = this.tabControlMain.SelectedPage;
                if (nextPage != null)
                {

                    this.tabControlMain.SelectedPage = nextPage;
                }

                if (nextPage != null)
                {
                    if (tabControlMain.SelectedPage.Index == 0)
                    {
                        tabControlMain.SelectedPage.ContextMenuStrip.Show();
                    }
                    nextPage.ResumeLayout();
                }
                parentPage.SuspendLayout();

                if (parentPage.Index == 1)
                {
                    //trimmer tab
                    //carico il programma in TEACH
                    comboBoxM1TeachProgramList.Text = M1PrgName;
                    M1TeachLoadProgram();
                    //carico il programma in TEST
                    comboBoxM1TestProgramList.Text = M1PrgName;
                    M1TestLoadProgram();
                }

                if (parentPage.Index == 3)
                {
                    //ask user to close application
                    this.WindowState = FormWindowState.Minimized;
                }

                if (parentPage.Index == 4)
                {
                    //ask user to close application
                    DialogResult res = xDialog.MsgBox.Show("Está seguro de que desea salir de la aplicación?", "PBoot", xDialog.MsgBox.Buttons.YesNo);
                    if (res == DialogResult.Yes)
                    {
                        SaveGUISettings();
                        //exit from application
                        myCore?.StopAllService();
                        System.Windows.Forms.Application.Exit();
                    }
                }

            }
            else
            {

            }
        }

        public void SaveGUISettings()
        {
            //save configuration file plconfig.xml
            guiConfigurator.AddValue("T0", "M3PRGNAME1", comboBoxM3PrgName_st1.Text, true);
            guiConfigurator.AddValue("T0", "M3PRGNAME2", comboBoxM3PrgName_st2.Text, true);
            guiConfigurator.AddValue("T0", "AUTOPRGNAME", comboBoxAutoPrgName.Text, true);
            guiConfigurator.AddValue("T0", "M1INC", (checkBoxM1Inclusion.CheckState == CheckState.Checked) ? "1" : "0", true);
            guiConfigurator.AddValue("T0", "M2INC", (checkBoxM2Inclusion.CheckState == CheckState.Checked) ? "1" : "0", true);
            guiConfigurator.AddValue("T0", "M3INC", (checkBoxM3Inclusion.CheckState == CheckState.Checked) ? "1" : "0", true);
            guiConfigurator.AddValue("T0", "M4INC", (checkBoxM4Inclusion.CheckState == CheckState.Checked) ? "1" : "0", true);
            guiConfigurator.AddValue("T0", "M5INC", (checkBoxM5Inclusion.CheckState == CheckState.Checked) ? "1" : "0", true);
            guiConfigurator.AddValue("T0", "M6INC", (checkBoxM6Inclusion.CheckState == CheckState.Checked) ? "1" : "0", true);
            guiConfigurator.AddValue("T0", "M6PERCENTAGE", numericUpDownM6OnPercentage.Value.ToString(), true);
            guiConfigurator.AddValue("T0", "RECIPENAME", comboBoxT0RecipeName.Text, true);

            guiConfigurator.AddValue("T1_1", "PRGNAME", comboBoxM1TeachProgramList.Text, true);
            guiConfigurator.AddValue("T1_1", "RECIPENAME", comboBoxM1TeachRecipeName.Text, true);
            guiConfigurator.AddValue("T1_1", "JOGSPEED", numericUpDownM1JogSpeed.Value.ToString(), true);
            guiConfigurator.AddValue("T1_1", "MANUALQUOTE", numericUpDownM1ManualQuote.Value.ToString(), true);
            guiConfigurator.AddValue("T1_1", "MANUALSPEED", numericUpDownM1ManualSpeed.Value.ToString(), true);

            guiConfigurator.AddValue("T1_2", "PRGNAME", comboBoxM1TestProgramList.Text, true);
            guiConfigurator.AddValue("T1_2", "RECIPENAME", comboBoxM1TestRecipeName.Text, true);

            guiConfigurator.AddValue("T2_1", "PRGNAME", comboBoxM2TeachProgramList.Text, true);
            guiConfigurator.AddValue("T2_1", "RECIPENAME", comboBoxM2TeachRecipeName.Text, true);
            guiConfigurator.AddValue("T2_1", "JOGSPEED", numericUpDownM2JogSpeed.Value.ToString(), true);
            guiConfigurator.AddValue("T2_1", "MANUALQUOTE", numericUpDownM2ManualQuote.Value.ToString(), true);

            guiConfigurator.AddValue("T2_1", "MANUALSPEED", numericUpDownM2ManualSpeed.Value.ToString(), true);

            guiConfigurator.AddValue("T2_2", "PRGNAME", comboBoxM2TestProgramList.Text, true);
            guiConfigurator.AddValue("T2_2", "RECIPENAME", comboBoxM2TestRecipeName.Text, true);

            guiConfigurator.AddValue("T3_1", "PRGNAME", comboBoxM3TeachProgramList.Text, true);
            guiConfigurator.AddValue("T3_1", "RECIPENAME", comboBoxM3TeachRecipeName.Text, true);
            guiConfigurator.AddValue("T3_1", "JOGSPEED", numericUpDownM3JogSpeed.Value.ToString(), true);
            guiConfigurator.AddValue("T3_1", "MANUALQUOTE", numericUpDownM3ManualQuote.Value.ToString(), true);
            guiConfigurator.AddValue("T3_1", "MANUALSPEED", numericUpDownM3ManualSpeed.Value.ToString(), true);

            guiConfigurator.AddValue("T3_2", "PRGNAME", comboBoxM3TestProgramList.Text, true);
            guiConfigurator.AddValue("T3_2", "RECIPENAME", comboBoxM3TestRecipeName.Text, true);
            guiConfigurator.AddValue("LOGIN", "PWD", loginPassword, true);
            guiConfigurator.AddValue("T2_1", "TIMEREXITBELT", numericUpDownM2ExitBeltTimer.Value.ToString(), true);
            guiConfigurator.AddValue("T3_1", "TIMEREXITBELT", numericUpDownM3TimerExitBelt.Value.ToString(), true);
            guiConfigurator.Save("guiconfig.xml", Configurator.FileType.Xml);
        }

        private void InitAutoSettings()
        {
            //save configuration file plconfig.xml
            groupBoxM1.Text = autoConfigurator.GetValue("T0", "G1NAME", "");
            groupBoxM2.Text = autoConfigurator.GetValue("T0", "G2NAME", "");
            groupBoxM3.Text = autoConfigurator.GetValue("T0", "G3NAME", "");
            groupBoxM4.Text = autoConfigurator.GetValue("T0", "G4NAME", "");
            groupBoxM5.Text = autoConfigurator.GetValue("T0", "G5NAME", "");
            groupBoxM6.Text = autoConfigurator.GetValue("T0", "G6NAME", "");
        }

        private async void InitGUISettings()
        {
            //save configuration file plconfig.xml
            comboBoxAutoPrgName.Text = guiConfigurator.GetValue("T0", "AUTOPRGNAME", "");
            M1PrgName = comboBoxAutoPrgName.Text;
            M2PrgName = comboBoxAutoPrgName.Text;
            M3PrgName1 = comboBoxM3PrgName_st1.Text;
            comboBoxM3PrgName_st2.Text = guiConfigurator.GetValue("T0", "M3PRGNAME2", "");
            M3PrgName2 = comboBoxM3PrgName_st2.Text;
            SendStationRGM3ToOpc(M3PrgName1, M3PrgName2);
            SendSizeFromM1ProgramToOpc();
            M4PrgName = comboBoxAutoPrgName.Text;
            checkBoxM1Inclusion.CheckState = (guiConfigurator.GetValue("T0", "M1INC", "") == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxM2Inclusion.CheckState = (guiConfigurator.GetValue("T0", "M2INC", "") == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxM3Inclusion.CheckState = (guiConfigurator.GetValue("T0", "M3INC", "") == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxM4Inclusion.CheckState = (guiConfigurator.GetValue("T0", "M4INC", "") == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxM5Inclusion.CheckState = (guiConfigurator.GetValue("T0", "M5INC", "") == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxM6Inclusion.CheckState = (guiConfigurator.GetValue("T0", "M6INC", "") == "1") ? CheckState.Checked : CheckState.Unchecked;
            M1inc = guiConfigurator.GetValue("T0", "M1INC", "");
            M2inc = guiConfigurator.GetValue("T0", "M2INC", "");
            M3inc = guiConfigurator.GetValue("T0", "M3INC", "");
            M4inc = guiConfigurator.GetValue("T0", "M4INC", "");
            M5inc = guiConfigurator.GetValue("T0", "M5INC", "");
            M6inc = guiConfigurator.GetValue("T0", "M6INC", "");
            M3InLine = guiConfigurator.GetValue("MAIN", "M3INLINE", "0");
            loginPassword = guiConfigurator.GetValue("LOGIN", "PWD", "RSA");
            RestartRequestFromM1();
            comboBoxM1TeachProgramList.Text = M1PrgName;
            comboBoxM1TestProgramList.Text = M1PrgName;
            M1TeachLoadProgram();
            M1TestLoadProgram();
            RestartRequestFromM2();
            comboBoxM2TeachProgramList.Text = M2PrgName;
            comboBoxM2TestProgramList.Text = M2PrgName;
            M2TeachLoadProgram();
            M2TestLoadProgram();
            RestartRequestFromM3();
            RestartRequestFromM4();
            RestartRequestFromM5();
            RestartRequestFromM6();
            UpdateGUIByUser();
            comboBoxT0RecipeName.Text = guiConfigurator.GetValue("T0", "RECIPENAME", "");
            comboBoxM1TeachRecipeName.Text = comboBoxT0RecipeName.Text;
            comboBoxM1TestRecipeName.Text = comboBoxT0RecipeName.Text;
            comboBoxM2TeachRecipeName.Text = comboBoxT0RecipeName.Text;
            comboBoxM2TestRecipeName.Text = comboBoxT0RecipeName.Text;
            comboBoxM3TeachRecipeName.Text = comboBoxT0RecipeName.Text;
            comboBoxM3TestRecipeName.Text = comboBoxT0RecipeName.Text;
            string keyToSend = null;

            //init machine inclusion to OPC
            InitSendInclusionToOpc();

            if (ccService == null) return;

            numericUpDownM6OnPercentage.Value = short.Parse(guiConfigurator.GetValue("T0", "M6PERCENTAGE", ""));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM6ONPercentage";
                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM6OnPercentage.Value.ToString()));
            }



            numericUpDownM1JogSpeed.Value = Convert.ToDecimal(guiConfigurator.GetValue("T1_1", "JOGSPEED", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM1JogSpeed";
                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM1JogSpeed.Value.ToString()));
            }

            numericUpDownM1ManualQuote.Value = Convert.ToDecimal(guiConfigurator.GetValue("T1_1", "MANUALQUOTE", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM1ManualQuote";
                var sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownM1ManualQuote.Value.ToString()));
            }

            numericUpDownM1ManualSpeed.Value = Convert.ToDecimal(guiConfigurator.GetValue("T1_1", "MANUALSPEED", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM1ManualSpeed";
                var sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownM1ManualSpeed.Value.ToString()));
            }

            numericUpDownM2JogSpeed.Value = Convert.ToDecimal(guiConfigurator.GetValue("T2_1", "JOGSPEED", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM2JogSpeed";

                var sendResult = await ccService.Send(keyToSend, Convert.ToDecimal(numericUpDownM2JogSpeed.Value.ToString()));
            }

            numericUpDownM2ManualQuote.Value = Convert.ToDecimal(guiConfigurator.GetValue("T2_1", "MANUALQUOTE", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM2ManualQuote";
                var sendResult = await ccService.Send(keyToSend, Convert.ToDecimal(numericUpDownM2ManualQuote.Value.ToString()));
            }

            numericUpDownM2ManualSpeed.Value = Convert.ToDecimal(guiConfigurator.GetValue("T2_1", "MANUALSPEED", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM2ManualSpeed";
                var sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownM2ManualSpeed.Value.ToString()));
            }

            comboBoxM3TeachProgramList.Text = guiConfigurator.GetValue("T3_1", "PRGNAME", "");
            comboBoxM3TeachRecipeName.Text = guiConfigurator.GetValue("T3_1", "RECIPENAME", "");
            numericUpDownM3JogSpeed.Value = Convert.ToDecimal(guiConfigurator.GetValue("T3_1", "JOGSPEED", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM3JogSpeed";
                var sendResult = await ccService.Send(keyToSend, Convert.ToDecimal(numericUpDownM3JogSpeed.Value.ToString()));
            }

            numericUpDownM3ManualQuote.Value = Convert.ToDecimal(guiConfigurator.GetValue("T3_1", "MANUALQUOTE", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM3ManualQuote";
                var sendResult = await ccService.Send(keyToSend, Convert.ToDecimal(numericUpDownM3ManualQuote.Value.ToString()));
            }
            numericUpDownM3ManualSpeed.Value = Convert.ToDecimal(guiConfigurator.GetValue("T3_1", "MANUALSPEED", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM3ManualSpeed";
                var sendResult = await ccService.Send(keyToSend, Convert.ToDecimal(numericUpDownM3ManualSpeed.Value.ToString()));
            }
            comboBoxM3TestProgramList.Text = guiConfigurator.GetValue("T3_2", "PRGNAME", "");
            comboBoxM3TestRecipeName.Text = guiConfigurator.GetValue("T3_2", "RECIPENAME", "");

            numericUpDownM3TimerExitBelt.Value = Convert.ToDecimal(guiConfigurator.GetValue("T3_1", "TIMEREXITBELT", "0.1"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM3AutoTimerExitBelt";
                var sendResult = await ccService.Send(keyToSend, Convert.ToDecimal(numericUpDownM3TimerExitBelt.Value.ToString()));
            }

            numericUpDownM2ExitBeltTimer.Value = Convert.ToDecimal(guiConfigurator.GetValue("T2_1", "TIMEREXITBELT", "0.1"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM2AutoTimerExitBelt";
                var sendResult = await ccService.Send(keyToSend, Convert.ToDecimal(numericUpDownM2ExitBeltTimer.Value.ToString()));
            }
        }

        private async void InitSendInclusionToOpc()
        {
            string keyToSend = null;
            if (ccService == null) return;

            if (ccService.ClientIsConnected)
            {

                bool chkValue = false;

                keyToSend = "pcM1Inclusion";
                chkValue = (M1inc == "1") ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM1Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM1Inclusion.ImageIndex = 2;
                }

                keyToSend = "pcM2Inclusion";
                chkValue = (M2inc == "1") ? true : false;

                sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM2Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM2Inclusion.ImageIndex = 2;
                }

                if (M3InLine == "1")
                {
                    keyToSend = "pcM3Inclusion";
                    chkValue = (M3inc == "1") ? true : false;

                    sendResult = await ccService.Send(keyToSend, chkValue);

                    if (sendResult.OpcResult)
                    {
                        checkBoxM3Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                    }
                    else
                    {
                        checkBoxM3Inclusion.ImageIndex = 2;
                    }
                }


                keyToSend = "pcM4Inclusion";
                chkValue = (M4inc == "1") ? true : false;

                sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM4Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM4Inclusion.ImageIndex = 2;
                }


                keyToSend = "pcM5Inclusion";
                chkValue = (M5inc == "1") ? true : false;

                sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM5Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM5Inclusion.ImageIndex = 2;
                }


                keyToSend = "pcM6Inclusion";
                chkValue = (M6inc == "1") ? true : false;

                sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM6Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM6Inclusion.ImageIndex = 2;
                }
            }
        }

        private async void SendSizeFromM1ProgramToOpc()
        {
            if (M1PrgName == "") return;
            if (ccService == null) return;
            short size = 0;

            try
            {
                size = short.Parse(M1PrgName.Substring(7, 3));

                if (ccService.ClientIsConnected)
                {
                    string keyToSend = "pcM1SizeInProduction";
                    var sendResult = await ccService.Send(keyToSend, size);

                    if (sendResult.OpcResult)
                    {
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async void SendStationRGM3ToOpc(string prgName1, string prgName2)
        {
            //send station
            string stationIDKey = "pcM3StationRG";
            string foot1 = "";
            string foot2 = "";

            try
            {
                foot1 = prgName1.Substring(11, 2);
                foot2 = prgName1.Substring(11, 2);

                if (ccService.ClientIsConnected)
                {
                    if (foot1 != "")
                    {
                        if (string.Equals(prgName1.Substring(11, 2), "DX", StringComparison.CurrentCultureIgnoreCase))
                        {
                            var sendResult3 = await ccService.Send(stationIDKey, 1);
                        }
                    }

                    if (foot2 != "")
                    {
                        if (string.Equals(prgName2.Substring(11, 2), "DX", StringComparison.CurrentCultureIgnoreCase))
                        {
                            var sendResult3 = await ccService.Send(stationIDKey, 2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void AddMessageToDataGridOnTop(DateTime dt, Priority pr, Machine machine, string messageText)
        {
            try
            {
                dataGridViewMessages.Rows.Insert(0, dt.ToString(), pr.ToString(), machine.ToString(), messageText.ToString());
                SetColorToDataGridRow(0, pr);

            }
            catch (Exception Ex)
            {

            }
        }

        private void SetColorToDataGridRow(int index, Priority pr)
        {
            try
            {
                if (pr == Priority.critical)
                {
                    dataGridViewMessages.Rows[index].DefaultCellStyle.BackColor = Color.Red;
                }
                else if (pr == Priority.high)
                {
                    dataGridViewMessages.Rows[index].DefaultCellStyle.BackColor = Color.Orange;
                }
                else if (pr == Priority.normal)
                {
                    dataGridViewMessages.Rows[index].DefaultCellStyle.BackColor = Color.FromArgb(65, 68, 65);
                }
            }
            catch (Exception Ex)
            {

            }
        }

        private void SetPriorityToDataGridRow(int index, Priority pr)
        {
            dataGridViewMessages.Rows[index].Cells["priority"].Value = pr.ToString();
            SetColorToDataGridRow(index, pr);
        }

        private void UpdateColorToDataGridRow()
        {
            int index = 0;
            foreach (DataGridViewRow item in dataGridViewMessages.Rows)
            {
                Priority tmp = Priority.normal;
                Enum.TryParse<Priority>(item.Cells["priority"].Value.ToString(), out tmp);
                SetColorToDataGridRow(index, tmp);
                index = index + 1;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM1Reset";
            var sendResult = await ccService.Send(keyToSend, 1);
            keyToSend = "pcM2Reset";
            sendResult = await ccService.Send(keyToSend, 1);
            keyToSend = "pcM3Reset";
            sendResult = await ccService.Send(keyToSend, 1);
            keyToSend = "pcM4Reset";
            sendResult = await ccService.Send(keyToSend, 1);
            keyToSend = "pcM5Reset";
            sendResult = await ccService.Send(keyToSend, 1);
            keyToSend = "pcM6Reset";
            sendResult = await ccService.Send(keyToSend, 1);
            //SetPriorityToDataGridRow(1, Priority.normal);
            //UpdateColorToDataGridRow();
        }

        private void RefreshModelNameComboBox()
        {
            return;
            #region (* refresh combobox model name list *)
            List<string> mList = new List<string>();
            List<string> dList = new List<string>();
            //get model name list from DB
            MySqlResult<recipies> result = mysqlService.DBTable[0].SelectAll<recipies>();
            result.Result.ForEach(x => mList.Add(x.model_name));
            result.Result.ForEach(x => dList.Add(x.m_description));
            comboBoxMRecipeName.Items.Clear();
            comboBoxT0RecipeName.Items.Clear();
            comboBoxM1TeachRecipeName.Items.Clear();
            comboBoxM1TestRecipeName.Items.Clear();
            comboBoxM2TeachRecipeName.Items.Clear();
            comboBoxM2TestRecipeName.Items.Clear();
            comboBoxM3TeachRecipeName.Items.Clear();
            comboBoxM3TestRecipeName.Items.Clear();
            comboBoxOriginRecipe.Items.Clear();
            comboBoxDestinationRecipe.Items.Clear();
            comboBoxRecipeDelete.Items.Clear();
            ModelNameDictionary.Clear();
            DescriptionDictionary.Clear();
            int i = 1;
            foreach (string modelName in mList)
            {
                comboBoxMRecipeName.Items.Add(modelName);
                comboBoxMRecipeName.SelectedIndex = 0;
                comboBoxT0RecipeName.Items.Add(modelName);
                //comboBoxT0RecipeName.SelectedIndex = 0;
                comboBoxM1TeachRecipeName.Items.Add(modelName);
                //comboBoxM1TeachRecipeName.SelectedIndex = 0;
                comboBoxM2TeachRecipeName.Items.Add(modelName);
                //comboBoxM2TeachRecipeName.SelectedIndex = 0;
                comboBoxM3TeachRecipeName.Items.Add(modelName);
                //comboBoxM3TeachRecipeName.SelectedIndex = 0;
                comboBoxM1TestRecipeName.Items.Add(modelName);
                //comboBoxM1TestRecipeName.SelectedIndex = 0;
                comboBoxM2TestRecipeName.Items.Add(modelName);
                //comboBoxM2TestRecipeName.SelectedIndex = 0;
                comboBoxM3TestRecipeName.Items.Add(modelName);
                //comboBoxM3TestRecipeName.SelectedIndex = 0;
                comboBoxOriginRecipe.Items.Add(modelName);
                //comboBoxOriginRecipe.SelectedIndex = 0;
                comboBoxDestinationRecipe.Items.Add(modelName);
                //comboBoxDestinationRecipe.SelectedIndex = 0;
                comboBoxRecipeDelete.Items.Add(modelName);
                //comboBoxRecipeDelete.SelectedIndex = 0;
                ModelNameDictionary[i] = modelName;

                i = i + 1;
            }
            i = 1;
            foreach (string description in dList)
            {
                DescriptionDictionary[i] = description;
                i = i + 1;
            }
        }
        #endregion

        private async void buttonAddNewRecipe_Click(object sender, EventArgs e)
        {
            if (textBoxMRecipeName.Text.Length >= 0 && textBoxMRecipeName.Text.Length < 4)
            {
                xDialog.MsgBox.Show("Nombre de receta no válido", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Exclamation, xDialog.MsgBox.AnimateStyle.FadeIn);
                return;
            }

            string top = GetTopFromPadLaserAddNewRecipe();
            string medium = GetMediumFromPadLaserAddNewRecipe();
            string bottom = GetBottomFromPadLaserAddNewRecipe();

            object[] value = new object[]
                   {
                    //recipe name
                    textBoxMRecipeName.Text,
                    //M1 params
                    (checkBoxM1Param1.CheckState == CheckState.Checked)?1:0,
                    0,
                    //M2 params
                    (checkBoxM2Param1.CheckState == CheckState.Checked)?1:0,
                    0,
                    //M3 params
                    (checkBoxM3Param1.CheckState == CheckState.Checked)?1:0,
                    (radioButtonFootOrderOpt1.Checked)?1:2,
                    //M4 params
                    //m4_param1
                    (checkBoxM4Param1.CheckState == CheckState.Checked)?1:0,
                    //m4_param2
                    //GetSelOptionTopBottomAddNewRecipe(),
                    GetM4RowsSelOptionAddNewRecipe(),
                    //m4_param3
                    top,
                    //m4_param4
                    bottom,
                    //m4_param5
                    medium,
                    //M6 params
                    (checkBoxM5Param1.CheckState == CheckState.Checked)?1:0,
                    0,
                    //M6 params
                    (checkBoxM6Param1.CheckState == CheckState.Checked)?1:0,
                    0,
                    textBoxRecipeDescription.Text,

                   };

            var testResult = await mysqlService.DBTable[0].InsertAutomaticAsync(value);

            if (testResult.Error == 0)
            {
                //send recipe update to pad laser plc
                string keyToSend = "pcM4RecipeUpdate";

                if (ccService.ClientIsConnected)
                {
                    var readResult = await ccService.Send(keyToSend, 1);
                    Thread.Sleep(200);
                    readResult = await ccService.Send(keyToSend, 0);
                }
                //refresh combobox
                RefreshModelNameComboBox();
                xDialog.MsgBox.Show("Receta creada con éxito positivo", "PBoot", xDialog.MsgBox.Buttons.OK);
            }
            else
            {
                xDialog.MsgBox.Show("Receta non creada. Error" + " " + testResult.Message, "PBoot", xDialog.MsgBox.Buttons.OK);
            }
        }

        private string GetTopFromPadLaserAddNewRecipe()
        {
            string strTop = "";
            if (radioButtonM4Sel1Top.Checked)
            {
                //dateontop
                strTop = DateTime.Now.ToString("dd-MM-yyyy");
            }
            else strTop = textBoxLaserLine1New.Text;

            return strTop;
        }

        private string GetMediumFromPadLaserAddNewRecipe()
        {
            string strMedio = "";
            if (radioButtonM4Sel1Medio.Checked)
            {
                //dateontop
                strMedio = DateTime.Now.ToString("dd-MM-yyyy");
            }
            else strMedio = textBoxLaserLine12New.Text;

            return strMedio;
        }

        private string GetBottomFromPadLaserAddNewRecipe()
        {
            string strBottom = "";
            if (radioButtonM4Sel1Bottom.Checked)
            {
                //dateontop
                strBottom = DateTime.Now.ToString("dd-MM-yyyy");
            }
            else strBottom = textBoxLaserLine2New.Text;

            return strBottom;
        }

        private int GetSelOptionTopBottomAddNewRecipe()
        {
            int seloption = 0;
            if ((radioButtonM4Sel1Top.Checked) & (radioButtonM4Sel1Bottom.Checked))
            {
                seloption = 3;
            }

            if ((radioButtonM4Sel1Top.Checked == false) & (radioButtonM4Sel1Bottom.Checked))
            {
                seloption = 2;
            }

            if ((radioButtonM4Sel1Top.Checked) & (radioButtonM4Sel1Bottom.Checked == false))
            {
                seloption = 1;
            }

            if ((radioButtonM4Sel1Top.Checked == false) & (radioButtonM4Sel1Bottom.Checked == false))
            {
                seloption = 0;
            }

            return seloption;
        }

        private string GetM4RowsSelOptionAddNewRecipe()
        {
            string seloption = "";
            seloption = (radioButtonM4Sel1Top.Checked) ? "1" : "0";
            seloption = seloption + ((radioButtonM4Sel1Medio.Checked) ? "1" : "0");
            seloption = seloption + ((radioButtonM4Sel1Bottom.Checked) ? "1" : "0");

            return seloption;
        }

        private string GetM4RowsSelOptionUpdateRecipe()
        {
            string seloption = "";
            seloption = (radioButtonMRecipeTopSel1.Checked) ? "1" : "0";
            seloption = seloption + ((radioButtonMRecipeMedioSel1.Checked) ? "1" : "0");
            seloption = seloption + ((radioButtonMRecipeBottomSel1.Checked) ? "1" : "0");

            return seloption;
        }

        private int GetSelOptionTopBottomUpdateRecipe()
        {
            int seloption = 0;
            if ((radioButtonMRecipeTopSel1.Checked) & (radioButtonMRecipeBottomSel2.Checked))
            {
                seloption = 3;
            }

            if ((radioButtonMRecipeTopSel1.Checked == false) & (radioButtonMRecipeBottomSel1.Checked))
            {
                seloption = 2;
            }

            if ((radioButtonMRecipeTopSel1.Checked) & (radioButtonMRecipeBottomSel1.Checked == false))
            {
                seloption = 1;
            }

            if ((radioButtonMRecipeTopSel1.Checked == false) & (radioButtonMRecipeBottomSel1.Checked == false))
            {
                seloption = 0;
            }

            return seloption;
        }

        private async void comboBoxM3PrgName_st1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected == false) return;
            if (comboBoxM3PrgName_st1.Text == "") return;
            //get model name
            string prgName = comboBoxM3PrgName_st1.Text;
            M3PrgName1 = comboBoxM3PrgName_st1.Text;
            string modelName = prgName.Substring(2, 4);

            //check model name
            if (modelName == "")
            {
                //program name with incorrect format
                //todo add message to the operator
                return;
            }

            //get data from DB
            MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

            if ((recs.Error == 0) & (recs.Result.Count != 0))
            {
                SendStationRGM3ToOpc(M3PrgName1, M3PrgName2);

                //send recipe
                string keyValue = "pcM3Param1";
                var sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param1.ToString()));

                //send type order: RG, LF
                //type order 2: LF, RG
                keyValue = "pcM3Param2";
                sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param2.ToString()));
                M3AutoDictionary[1] = short.Parse(recs.Result[0].m3_param1.ToString());
                //labelM3Param1Value.Text = recs.Result[0].m3_param1.ToString();
                //labelM3Param2Value.Text = ((recs.Result[0].m3_param2 == 1)? "start from derecha" : "start from izquierda");
                groupBoxM3.Text = "tampografia ext - " + ((recs.Result[0].m3_param2 == 1) ? "inicio derecha" : "inicio izquierda");

                //send quote, speed
                var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

                if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                {
                    ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                    ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                    objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[2] + "\\" + comboBoxM3PrgName_st1.Text + config.Extensions[0]);
                    if (objPoints != null)
                    {
                        List<string> keys = new List<string>()
                    {
                        "pcM3AutoQuoteSt1",
                        "pcM3AutoSpeedSt1"
                    };

                        List<object> values = new List<object>()
                    {
                        new float[] {0, (float)objPoints.Points[0].Q1, (float)objPoints.Points[0].Q2, (float)objPoints.Points[0].Q3, (float)objPoints.Points[0].Q4},
                        new short[] {0, (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                    };

                        var sendResults = await ccService.Send(keys, values);
                        bool allsent = true;
                        foreach (var result in sendResults)
                        {
                            if (result.Value.OpcResult)
                            {
                            }
                            else
                            {
                                AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padprintExt, "error sending " + result.Value.NodeString + " station 1");
                            }
                            allsent = allsent & result.Value.OpcResult;
                        }
                        if (allsent) AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padprintExt, "programa enviado exitosamente " + "station 1");
                    }
                }
                else
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padprintExt, "verify program file " + "station 1");
                }
            }
            else
            {
                //manage db error
            }
        }
        private async void comboBoxM3PrgName_st2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxM3PrgName_st2.Text == "") return;
            //get model name
            string prgName = comboBoxM3PrgName_st2.Text;
            M3PrgName2 = comboBoxM3PrgName_st2.Text;
            string modelName = prgName.Substring(2, 4);

            //check model name
            if (modelName == "")
            {
                //program name with incorrect format
                //todo add message to the operator
                return;
            }

            //get data from DB
            MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

            if ((recs.Error == 0) & (recs.Result.Count != 0))
            {
                //send station
                SendStationRGM3ToOpc(M3PrgName1, M3PrgName2);

                //send recipe
                string keyValue = "pcM3Param1";
                var sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param1.ToString()));

                keyValue = "pcM3Param2";
                sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param2.ToString()));
                M3AutoDictionary[1] = short.Parse(recs.Result[0].m3_param1.ToString());
                //labelM3Param1Value.Text = recs.Result[0].m3_param1.ToString();
                groupBoxM3.Text = "tampografia ext - " + ((recs.Result[0].m3_param2 == 1) ? "inicio derecha" : "inicio izquierda");
                //send quote, speed
                var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

                if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                {
                    ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                    ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                    objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[2] + "\\" + comboBoxM3PrgName_st2.Text + config.Extensions[0]);
                    if (objPoints != null)
                    {
                        List<string> keys = new List<string>()
                    {
                        "pcM3AutoQuoteSt2",
                        "pcM3AutoSpeedSt2"
                    };

                        List<object> values = new List<object>()
                    {
                        new float[] {0, (float)objPoints.Points[0].Q1, (float)objPoints.Points[0].Q2, (float)objPoints.Points[0].Q3, (float)objPoints.Points[0].Q4},
                        new short[] {0,  (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                    };

                        var sendResults = await ccService.Send(keys, values);
                        bool allsent = true;
                        foreach (var result in sendResults)
                        {
                            if (result.Value.OpcResult)
                            {
                            }
                            else
                            {
                                AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padprintExt, "error sending " + result.Value.NodeString + " station 2");
                            }
                            allsent = allsent & result.Value.OpcResult;
                        }
                        if (allsent) AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padprintExt, "programa enviado exitosamente " + "station 2");
                    }
                }
                else
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padprintExt, "verify program file " + "station 2");
                }
            }
            else
            {
                //manage db error
            }
        }

        private void ResetM1Datagrid()
        {
            dataGridViewM1TeachPoints.Rows.Clear();
            dataGridViewM1TeachPoints.Rows.Add(4);
            dataGridViewM1TeachPoints.Rows[0].Cells[0].Value = 1;
            dataGridViewM1TeachPoints.Rows[1].Cells[0].Value = 2;
            dataGridViewM1TeachPoints.Rows[2].Cells[0].Value = 3;
            dataGridViewM1TeachPoints.Rows[3].Cells[0].Value = 4;

            dataGridViewM1TeachPoints.Rows[0].Cells[1].Value = 100;
            dataGridViewM1TeachPoints.Rows[1].Cells[1].Value = 200;
            dataGridViewM1TeachPoints.Rows[2].Cells[1].Value = 300;
            dataGridViewM1TeachPoints.Rows[3].Cells[1].Value = 400;

            dataGridViewM1TeachPoints.Rows[0].Cells[2].Value = 10;
            dataGridViewM1TeachPoints.Rows[1].Cells[2].Value = 20;
            dataGridViewM1TeachPoints.Rows[2].Cells[2].Value = 30;
            dataGridViewM1TeachPoints.Rows[3].Cells[2].Value = 40;
            dataGridViewM1TeachPoints.ClearSelection();
        }

        private void ResetM2Datagrid()
        {
            dataGridViewM2TeachPoints.Rows.Clear();
            dataGridViewM2TeachPoints.Rows.Add(4);
            dataGridViewM2TeachPoints.Rows[0].Cells[0].Value = 1;
            dataGridViewM2TeachPoints.Rows[1].Cells[0].Value = 2;
            dataGridViewM2TeachPoints.Rows[2].Cells[0].Value = 3;
            dataGridViewM2TeachPoints.Rows[3].Cells[0].Value = 4;

            dataGridViewM2TeachPoints.Rows[0].Cells[1].Value = 100;
            dataGridViewM2TeachPoints.Rows[1].Cells[1].Value = 200;
            dataGridViewM2TeachPoints.Rows[2].Cells[1].Value = 300;
            dataGridViewM2TeachPoints.Rows[3].Cells[1].Value = 400;

            dataGridViewM2TeachPoints.Rows[0].Cells[2].Value = 10;
            dataGridViewM2TeachPoints.Rows[1].Cells[2].Value = 20;
            dataGridViewM2TeachPoints.Rows[2].Cells[2].Value = 30;
            dataGridViewM2TeachPoints.Rows[3].Cells[2].Value = 40;
            dataGridViewM2TeachPoints.ClearSelection();
        }

        private void ResetM3Datagrid()
        {
            dataGridViewM3TeachPoints.Rows.Clear();
            dataGridViewM3TeachPoints.Rows.Add(4);
            dataGridViewM3TeachPoints.Rows[0].Cells[0].Value = 1;
            dataGridViewM3TeachPoints.Rows[1].Cells[0].Value = 2;
            dataGridViewM3TeachPoints.Rows[2].Cells[0].Value = 3;
            dataGridViewM3TeachPoints.Rows[3].Cells[0].Value = 4;

            dataGridViewM3TeachPoints.Rows[0].Cells[1].Value = 100;
            dataGridViewM3TeachPoints.Rows[1].Cells[1].Value = 200;
            dataGridViewM3TeachPoints.Rows[2].Cells[1].Value = 300;
            dataGridViewM3TeachPoints.Rows[3].Cells[1].Value = 400;

            dataGridViewM3TeachPoints.Rows[0].Cells[2].Value = 10;
            dataGridViewM3TeachPoints.Rows[1].Cells[2].Value = 20;
            dataGridViewM3TeachPoints.Rows[2].Cells[2].Value = 30;
            dataGridViewM3TeachPoints.Rows[3].Cells[2].Value = 40;
            dataGridViewM3TeachPoints.ClearSelection();
        }

        private void M3TestSendProgram()
        {
            int i = 0;
            float[] quote = new float[5];
            short[] speed = new short[5];

            try
            {
                for (i = 0; i <= dataGridViewM2TestPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = float.Parse(dataGridViewM3TestPoints[1, i].Value.ToString());
                    speed[i + 1] = short.Parse(dataGridViewM3TestPoints[2, i].Value.ToString());
                }

                OPCUAM3TestPckSend(quote, speed);
            }
            catch (Exception ex)
            {

            }
        }

        private void M2TestSendProgram()
        {
            int i = 0;
            float[] quote = new float[5];
            short[] speed = new short[5];

            try
            {
                for (i = 0; i <= dataGridViewM2TestPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = float.Parse(dataGridViewM2TestPoints[1, i].Value.ToString());
                    speed[i + 1] = short.Parse(dataGridViewM2TestPoints[2, i].Value.ToString());
                }

                OPCUAM2TestPckSend(quote, speed);
            }
            catch (Exception ex)
            {

            }
        }

        private void M1TestSendProgram()
        {
            int i = 0;
            float[] quote = new float[5];
            short[] speed = new short[5];

            try
            {
                for (i = 0; i <= dataGridViewM1TestPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = float.Parse(dataGridViewM1TestPoints[1, i].Value.ToString());
                    speed[i + 1] = short.Parse(dataGridViewM1TestPoints[2, i].Value.ToString());
                }

                OPCUAM1TestPckSend(quote, speed);
            }
            catch (Exception ex)
            {

            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //if (_configForm == null)
            //    _configForm = new ServiceSetup(myCore);

            //_configForm.Show();
            //_configForm.Activate();
        }

        private async void buttonM3StartTest_Click(object sender, EventArgs e)
        {
            //send quote/speed
            M3TestSendProgram();

            //send start command
            string keyToSend = "pcM3TestType";

            if (radioButtonFootOrderOpt1Test.Checked)
            {
                var sendResult1 = await ccService.Send(keyToSend, 1);
            }
            else
            {
                var sendResult1 = await ccService.Send(keyToSend, 2);
            }

            //check phase
            string key = "pcM2PadPrintExtState";
            var readResult = await ccService.Read(key);

            if (readResult.OpcResult)
            {
                if (short.Parse(readResult.Value.ToString()) != 0)
                {
                    xDialog.MsgBox.Show("pad ext fase no 0. Prensa RESET.", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Error, xDialog.MsgBox.AnimateStyle.FadeIn);
                    return;
                }
            }

            //send start command
            keyToSend = "pcM3StartTest";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else xDialog.MsgBox.Show("sin conexión", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Exclamation, xDialog.MsgBox.AnimateStyle.FadeIn);
        }



        private async void radioButtonFootOrderOpt1Test_CheckedChanged(object sender, EventArgs e)
        {
            //send start command
            string keyToSend = "pcM3TestType";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else xDialog.MsgBox.Show("sin conexión", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Exclamation, xDialog.MsgBox.AnimateStyle.FadeIn);
        }

        private async void radioButtonFootOrderOpt2Test_CheckedChanged(object sender, EventArgs e)
        {
            string keyToSend = "pcM3TestType";

            if (radioButtonFootOrderOpt2Test.Checked)
            {
                var sendResult = await ccService.Send(keyToSend, 1);
            }
            else
            {
                var sendResult = await ccService.Send(keyToSend, 2);
            }
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            string keyToSend = "pcM1ResetCycle";
            var sendResult = await ccService.Send(keyToSend, true);
            keyToSend = "pcM2ResetCycle";
            sendResult = await ccService.Send(keyToSend, true);
            keyToSend = "pcM3ResetCycle";
            sendResult = await ccService.Send(keyToSend, true);
            keyToSend = "pcM4ResetCycle";
            sendResult = await ccService.Send(keyToSend, true);
            keyToSend = "pcM5ResetCycle";
            sendResult = await ccService.Send(keyToSend, true);
            //keyToSend = "pcM6ResetCycle";
            //sendResult = await ccService.Send(keyToSend, true);
        }


        public async void RestartRequestFromM1()
        {
            if (ccService == null) return;
            if (M1PrgName == "" || ccService.ClientIsConnected == false)
            {

            }
            else
            {
                string prgName = M1PrgName;
                string modelName = prgName.Substring(2, 4);

                //check model name
                if (modelName == "")
                {
                    return;
                }

                //get data from DB
                MySqlResult<recipies> recs = mysqlService.DBTable[0].SelectByPrimaryKey<recipies>(modelName);

                if ((recs.Result.Count != 0))
                {
                    string keyValue = "pcM1Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m1_param1.ToString()));
                    M1AutoDictionary[1] = short.Parse(recs.Result[0].m1_param1.ToString());

                    //send quote, speed
                    var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

                    if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                    {
                        ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                        ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                        objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[0] + "\\" + prgName + config.Extensions[0]);
                        if (objPoints != null)
                        {
                            List<string> keys = new List<string>()
                            {
                                "pcM1AutoQuote",
                                "pcM1AutoSpeed"
                            };

                            List<object> values = new List<object>()
                            {
                                new float[] { 0, (float)objPoints.Points[0].Q1, (float)objPoints.Points[0].Q2, (float)objPoints.Points[0].Q3, (float)objPoints.Points[0].Q4},
                                new short[] { 0, (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                            };

                            var sendResults = await ccService.Send(keys, values);

                            foreach (var result in sendResults)
                            {
                                if (result.Value.OpcResult)
                                {
                                }
                                else
                                {
                                    //AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.trimmer, "error sending " + result.Value.NodeString);
                                }
                            }

                            var sendResultsTimer = await ccService.Send("pcM1AutoTimer", objPoints.Points[0].CustomFloatParam);
                            if (sendResultsTimer.OpcResult)
                            {
                            }
                            else
                            {

                            }


                        }
                    }
                }

                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM1Inclusion";
                chkValue = (M1inc == "1") ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);
            }
        }

        public async void UpdateRecipeToM1(string modelName)
        {
            if (ccService == null) return;

            if (M1PrgName == "" || ccService.ClientIsConnected == false)
            {

            }
            else
            {
                string prgName = M1PrgName;
                if (M1PrgName == "") return;

                string modelNameAuto = prgName.Substring(2, 4);

                if (modelNameAuto != modelName) return;
                //get data from DB
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Result.Count != 0))
                {
                    string keyValue = "pcM1Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m1_param1.ToString()));

                    labelAutoDescription.Text = recs.Result[0].m_description.ToString();
                    //labelM1Param1Value.Text = recs.Result[0].m1_param1.ToString();
                    M1AutoDictionary[1] = short.Parse(recs.Result[0].m1_param1.ToString());
                }
            }
        }

        public async void UpdateRecipeToM2(string modelName)
        {
            if (ccService == null) return;
            if (M2PrgName == "" || ccService.ClientIsConnected == false)
            {

            }
            else
            {
                string prgName = M2PrgName;
                if (M2PrgName == "") return;

                string modelNameAuto = prgName.Substring(2, 4);

                if (modelNameAuto != modelName) return;
                //get data from DB
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Result.Count != 0))
                {
                    string keyValue = "pcM2Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m2_param1.ToString()));

                    //labelM2Param1Value.Text = recs.Result[0].m2_param1.ToString();
                    M2AutoDictionary[1] = short.Parse(recs.Result[0].m2_param1.ToString());
                }
            }
        }

        public async void UpdateRecipeToM3(string modelName)
        {
            if (M3PrgName1 == "" || ccService.ClientIsConnected == false)
            {

            }
            else
            {
                string prgName = M3PrgName1;
                if (M3PrgName1 == "") return;

                string modelNameAuto = prgName.Substring(2, 4);

                if (modelNameAuto != modelName) return;
                //get data from DB
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Result.Count != 0))
                {
                    string keyValue = "pcM3Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param1.ToString()));

                    M3AutoDictionary[1] = short.Parse(recs.Result[0].m3_param1.ToString());
                    keyValue = "pcM3Param2";
                    sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param2.ToString()));
                    //WriteOnLabelAsync(labelM3Param2Value, txtFoot);
                    //labelM3Param2Value.Text = txtFoot;
                    groupBoxM3.Text = "tampografia ext - " + ((recs.Result[0].m3_param2 == 1) ? "inicio derecha" : "inicio izquierda");
                }
            }
        }

        public async void UpdateRecipeToM4(string modelName, string shift)
        {
            if (ccService == null) return;

            if (M4PrgName == "" || ccService.ClientIsConnected == false)
            {

            }
            else
            {
                string prgName = M4PrgName;
                if (M4PrgName == "") return;

                string modelNameAuto = prgName.Substring(2, 4);

                if (modelNameAuto != modelName) return;


                //get data from DB
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Result.Count != 0))
                {
                    string keyValue = "pcM4Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m4_param1.ToString()));

                    //WriteOnLabelAsync(labelM3Param1Value, recs.Result[0].m3_param1.ToString());
                    //labelM4Param1Value.Text = recs.Result[0].m4_param1.ToString();
                    M4AutoDictionary[1] = short.Parse(recs.Result[0].m4_param1.ToString());
                    string top = "";
                    string bottom = "";
                    string medium = "";

                    //selection code
                    if (recs.Result[0].m4_param2.Substring(0, 1).Equals("0"))
                    {
                        top = recs.Result[0].m4_param3.ToString();
                    }
                    else top = DateTime.Now.ToString("dd-MM-yyyy");

                    if (recs.Result[0].m4_param2.Substring(1, 1).Equals("0"))
                    {
                        medium = recs.Result[0].m4_param4.ToString();
                    }
                    else medium = DateTime.Now.ToString("dd-MM-yyyy");

                    if (recs.Result[0].m4_param2.Substring(2, 1).Equals("0"))
                    {
                        bottom = recs.Result[0].m4_param5.ToString();
                    }
                    else bottom = DateTime.Now.ToString("dd-MM-yyyy");
                    shift = comboBoxShift.Text;
                    WritePadLaserRecipe(top, Properties.Settings.Default.PadLaserFilePathTop);
                    WritePadLaserRecipe(medium, Properties.Settings.Default.PadLaserFilePathMedium);
                    WritePadLaserRecipe(comboBoxInj.Text + " " + bottom + " " + shift, Properties.Settings.Default.PadLaserFilePathBottom);
                }
            }
        }

        public async void UpdateRecipeToM6(string modelName)
        {
            if (ccService == null) return;
            if (M1PrgName == "" || ccService.ClientIsConnected == false)
            {

            }
            else
            {
                //get data from DB
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Result.Count != 0))
                {
                    string keyValue = "pcM6Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m6_param1.ToString()));

                    //labelM6Param1Value.Text = recs.Result[0].m6_param1.ToString();
                    M6AutoDictionary[1] = short.Parse(recs.Result[0].m6_param1.ToString());
                }
            }
        }

        public async void RestartRequestFromM2()
        {
            if (ccService == null) return;

            if (M2PrgName == "" || ccService.ClientIsConnected == false)
            {

            }
            else
            {
                string prgName = M2PrgName;
                string modelName = prgName.Substring(2, 4);

                //check model name
                if (modelName == "")
                {
                    //program name with incorrect format
                    //todo add message to the operator
                    return;
                }

                //get data from DB
                MySqlResult<recipies> recs = mysqlService.DBTable[0].SelectByPrimaryKey<recipies>(modelName);

                if ((recs.Result.Count != 0))
                {
                    string keyValue = "pcM2Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m2_param1.ToString()));
                    M2AutoDictionary[1] = short.Parse(recs.Result[0].m2_param1.ToString());
                    // WriteOnLabelAsync(labelM2Param1Value, recs.Result[0].m2_param1.ToString());

                    //send quote, speed
                    var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

                    if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                    {
                        ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                        ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                        objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[1] + "\\" + prgName + config.Extensions[0]);
                        if (objPoints != null)
                        {
                            List<string> keys = new List<string>()
                            {
                                "pcM2AutoQuote",
                                "pcM2AutoSpeed"
                            };

                            List<object> values = new List<object>()
                            {
                                new float[] { 0, (float)objPoints.Points[0].Q1, (float)objPoints.Points[0].Q2, (float)objPoints.Points[0].Q3, (float)objPoints.Points[0].Q4},
                                new short[] { 0, (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                            };

                            var sendResults = await ccService.Send(keys, values);

                            foreach (var result in sendResults)
                            {
                                if (result.Value.OpcResult)
                                {
                                }
                                else
                                {
                                    //AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.trimmer, "error sending " + result.Value.NodeString);
                                }
                            }

                            var sendResultsTimer = await ccService.Send("pcM2AutoTimer", objPoints.Points[0].CustomFloatParam);
                            if (sendResultsTimer.OpcResult)
                            {
                            }
                            else
                            {

                            }


                        }
                    }
                }

                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM2Inclusion";
                chkValue = (M2inc == "1") ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);
            }
        }

        public async void RestartRequestFromM3()
        {
            if (M3InLine == "1") return;

            if (M3PrgName1 == "" || ccService.ClientIsConnected == false)
            {

            }
            else
            {
                string prgName = M3PrgName1;
                string modelName = prgName.Substring(2, 4);

                //check model name
                if (modelName == "")
                {
                    //program name with incorrect format
                    //todo add message to the operator
                    return;
                }

                //get data from DB
                MySqlResult<recipies> recs = mysqlService.DBTable[0].SelectByPrimaryKey<recipies>(modelName);

                if ((recs.Result.Count != 0))
                {
                    string keyValue = "pcM3Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param1.ToString()));
                    M3AutoDictionary[1] = short.Parse(recs.Result[0].m3_param1.ToString());

                    keyValue = "pcM3Param2";
                    sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param2.ToString()));

                    //string txtFoot = ((recs.Result[0].m3_param2 == 1) ? "start from derecha" : "start from izquierda");

                    //WriteOnLabelAsync(labelM3Param2Value, txtFoot);
                    groupBoxM3.Text = "tampografia ext - " + ((recs.Result[0].m3_param2 == 1) ? "inicio derecha" : "inicio izquierda");
                    //send quote, speed
                    var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

                    if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                    {
                        ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                        ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                        objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[2] + "\\" + prgName + config.Extensions[0]);
                        if (objPoints != null)
                        {
                            List<string> keys = new List<string>()
                            {
                                "pcM3AutoQuoteSt1",
                                "pcM3AutoSpeedSt1"
                            };

                            List<object> values = new List<object>()
                            {
                                new float[] { 0, (float)objPoints.Points[0].Q1, (float)objPoints.Points[0].Q2, (float)objPoints.Points[0].Q3, (float)objPoints.Points[0].Q4},
                                new short[] { 0, (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                            };

                            var sendResults = await ccService.Send(keys, values);

                            foreach (var result in sendResults)
                            {
                                if (result.Value.OpcResult)
                                {
                                }
                                else
                                {
                                    //AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.trimmer, "error sending " + result.Value.NodeString);
                                }
                            }

                            var sendResultsTimer = await ccService.Send("pcM3AutoTimer", objPoints.Points[0].CustomFloatParam);
                            if (sendResultsTimer.OpcResult)
                            {
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }

            if (M3PrgName2 == "" || ccService.ClientIsConnected == false)
            {

            }
            else
            {
                string prgName = M3PrgName2;
                string modelName = prgName.Substring(2, 4);

                //check model name
                if (modelName == "")
                {
                    //program name with incorrect format
                    //todo add message to the operator
                    return;
                }

                //get data from DB
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    //send quote, speed
                    var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

                    if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                    {
                        ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                        ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                        objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[2] + "\\" + prgName + config.Extensions[0]);
                        if (objPoints != null)
                        {
                            List<string> keys = new List<string>()
                            {
                                "pcM3AutoQuoteSt2",
                                "pcM3AutoSpeedSt2"
                            };

                            List<object> values = new List<object>()
                            {
                                new float[] { 0, (float)objPoints.Points[0].Q1, (float)objPoints.Points[0].Q2, (float)objPoints.Points[0].Q3, (float)objPoints.Points[0].Q4},
                                new short[] { 0, (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                            };

                            var sendResults = await ccService.Send(keys, values);

                            foreach (var result in sendResults)
                            {
                                if (result.Value.OpcResult)
                                {
                                }
                                else
                                {
                                    //AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.trimmer, "error sending " + result.Value.NodeString);
                                }
                            }
                        }
                    }
                }

                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM3Inclusion";
                chkValue = (M3inc == "1") ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);
            }
        }

        public async void RestartRequestFromM4()
        {
            if (ccService == null) return;
            if (M4PrgName == "" || ccService.ClientIsConnected == false)
            {

            }
            else
            {
                string prgName = M4PrgName;
                string modelName = prgName.Substring(2, 4);

                //check model name
                if (modelName == "")
                {
                    //program name with incorrect format
                    //todo add message to the operator
                    return;
                }

                //get data from DB
                MySqlResult<recipies> recs = mysqlService.DBTable[0].SelectByPrimaryKey<recipies>(modelName);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    string keyValue = "pcM4Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m4_param1.ToString()));

                    //WriteOnLabelAsync(labelM4Param1Value, recs.Result[0].m4_param1.ToString());
                    M4AutoDictionary[1] = short.Parse(recs.Result[0].m4_param1.ToString());
                    keyValue = "pcM4ProgramName";

                    var readResult = await ccService.Send(keyValue, "");

                    keyValue = "pcM4ProgramName";
                    readResult = await ccService.Send(keyValue, M4PrgName);

                    //aggiornare controlli
                    string top = "";
                    string medium = "";
                    string bottom = "";

                    //selection code
                    if (recs.Result[0].m4_param2.Substring(0, 1).Equals("0"))
                    {
                        top = recs.Result[0].m4_param3.ToString();
                    }
                    else top = DateTime.Now.ToString("dd-MM-yyyy");

                    if (recs.Result[0].m4_param2.Substring(1, 1).Equals("0"))
                    {
                        medium = recs.Result[0].m4_param4.ToString();
                    }
                    else medium = DateTime.Now.ToString("dd-MM-yyyy");

                    if (recs.Result[0].m4_param2.Substring(2, 1).Equals("0"))
                    {
                        bottom = recs.Result[0].m4_param5.ToString();
                    }
                    else bottom = DateTime.Now.ToString("dd-MM-yyyy");

                    string shift = comboBoxShift.Text;
                    WritePadLaserRecipe(top, Properties.Settings.Default.PadLaserFilePathTop);
                    WritePadLaserRecipe(medium, Properties.Settings.Default.PadLaserFilePathMedium);
                    WritePadLaserRecipe(comboBoxInj.Text + " " + bottom + " " + shift, Properties.Settings.Default.PadLaserFilePathBottom);
                }

                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM4Inclusion";
                chkValue = (M4inc == "1") ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

            }
        }


        public async void RestartRequestFromM5()
        {
            if (ccService == null) return;

            if (M1PrgName == "" || ccService.ClientIsConnected == false)
            {

            }
            else
            {
                string prgName = M1PrgName;
                string modelName = prgName.Substring(2, 4);

                //check model name
                if (modelName == "")
                {
                    //program name with incorrect format
                    //todo add message to the operator
                    return;
                }

                //get data from DB
                MySqlResult<recipies> recs = mysqlService.DBTable[0].SelectByPrimaryKey<recipies>(modelName);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    string keyValue = "pcM5Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m5_param1.ToString()));

                    //WriteOnLabelAsync(labelM5Param1Value, recs.Result[0].m5_param1.ToString());                    
                    M5AutoDictionary[1] = short.Parse(recs.Result[0].m5_param1.ToString());
                }

                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM5Inclusion";
                chkValue = (M5inc == "1") ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);


            }
        }

        public async void RestartRequestFromM6()
        {
            if (ccService == null) return;
            if (M1PrgName == "" || ccService.ClientIsConnected == false)
            {

            }
            else
            {
                string prgName = M1PrgName;
                string modelName = prgName.Substring(2, 4);

                //check model name
                if (modelName == "")
                {
                    //program name with incorrect format
                    //todo add message to the operator
                    return;
                }

                //get data from DB
                MySqlResult<recipies> recs = mysqlService.DBTable[0].SelectByPrimaryKey<recipies>(modelName);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    string keyValue = "pcM6Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m6_param1.ToString()));

                    //WriteOnLabelAsync(labelM6Param1Value, recs.Result[0].m6_param1.ToString());
                    M6AutoDictionary[1] = short.Parse(recs.Result[0].m6_param1.ToString());
                }

                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM6Inclusion";
                chkValue = (M6inc == "1") ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

            }
        }

        private async void comboBoxMRecipeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxMRecipeName.Text == "") return;
            MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(comboBoxMRecipeName.Text);

            if ((recs.Error == 0) & (recs.Result.Count != 0))
            {
                checkBoxM1RecipeModify.CheckState = ((short.Parse(recs.Result[0].m1_param1.ToString()) == 1) ? CheckState.Checked : CheckState.Unchecked);
                checkBoxM2RecipeModify.CheckState = ((short.Parse(recs.Result[0].m2_param1.ToString()) == 1) ? CheckState.Checked : CheckState.Unchecked);
                checkBoxM3RecipeModify.CheckState = ((short.Parse(recs.Result[0].m3_param1.ToString()) == 1) ? CheckState.Checked : CheckState.Unchecked);
                checkBoxM4RecipeModify.CheckState = ((short.Parse(recs.Result[0].m4_param1.ToString()) == 1) ? CheckState.Checked : CheckState.Unchecked);
                checkBoxM5RecipeModify.CheckState = ((short.Parse(recs.Result[0].m5_param1.ToString()) == 1) ? CheckState.Checked : CheckState.Unchecked);
                checkBoxM6RecipeModify.CheckState = ((short.Parse(recs.Result[0].m6_param1.ToString()) == 1) ? CheckState.Checked : CheckState.Unchecked);
                radioButtonM3Option1Recipe.Checked = ((short.Parse(recs.Result[0].m3_param2.ToString()) == 1) ? true : false);
                radioButtonM3Option2Recipe.Checked = ((short.Parse(recs.Result[0].m3_param2.ToString()) == 2) ? true : false);
                textBoxModifyDescription.Text = recs.Result[0].m_description.ToString();

                //aggiornare controlli
                if (recs.Result[0].m4_param2.Substring(0, 1) == "0")
                {
                    radioButtonMRecipeTopSel1.Checked = false;
                    radioButtonMRecipeTopSel2.Checked = true;
                }

                if (recs.Result[0].m4_param2.Substring(0, 1) == "1")
                {
                    radioButtonMRecipeTopSel1.Checked = true;
                    radioButtonMRecipeTopSel2.Checked = false;
                }

                if (recs.Result[0].m4_param2.Substring(1, 1) == "0")
                {
                    radioButtonMRecipeMedioSel1.Checked = false;
                    radioButtonMRecipeMedioSel2.Checked = true;
                }

                if (recs.Result[0].m4_param2.Substring(1, 1) == "1")
                {
                    radioButtonMRecipeMedioSel1.Checked = true;
                    radioButtonMRecipeMedioSel2.Checked = false;
                }

                if (recs.Result[0].m4_param2.Substring(2, 1) == "0")
                {
                    radioButtonMRecipeBottomSel1.Checked = false;
                    radioButtonMRecipeBottomSel2.Checked = true;
                }

                if (recs.Result[0].m4_param2.Substring(2, 1) == "1")
                {
                    radioButtonMRecipeBottomSel1.Checked = true;
                    radioButtonMRecipeBottomSel2.Checked = false;
                }

                textBoxM4TopLineRecipe.Text = recs.Result[0].m4_param3.ToString();
                textBoxM4BottomLineRecipe.Text = recs.Result[0].m4_param4.ToString();
                textBoxM4MedioLineRecipe.Text = recs.Result[0].m4_param5.ToString();
            }
        }

        private void buttonMUpdateRecipe_Click(object sender, EventArgs e)
        {
            if (comboBoxMRecipeName.Text == "") return;

            //update recipe
            DialogResult res = xDialog.MsgBox.Show("Estás seguro de que quieres actualizar la receta?", "PBoot", xDialog.MsgBox.Buttons.YesNo);
            if (res == DialogResult.Yes)
            {
                if (comboBoxMRecipeName.Text == "") return;
                object[] values = new object[]
                        {
                                    (checkBoxM1RecipeModify.CheckState == CheckState.Checked) ? 1 : 0,
                                    (checkBoxM2RecipeModify.CheckState == CheckState.Checked) ? 1 : 0,
                                    (checkBoxM3RecipeModify.CheckState == CheckState.Checked) ? 1 : 0,
                                    (checkBoxM4RecipeModify.CheckState == CheckState.Checked) ? 1 : 0,
                                    (checkBoxM5RecipeModify.CheckState == CheckState.Checked) ? 1 : 0,
                                    (checkBoxM6RecipeModify.CheckState == CheckState.Checked) ? 1 : 0,

            (radioButtonM3Option1Recipe.Checked == true) ? 1 : 2,
            //GetSelOptionTopBottomUpdateRecipe(),
            GetM4RowsSelOptionUpdateRecipe(),
            textBoxM4TopLineRecipe.Text,
            textBoxM4MedioLineRecipe.Text,
            textBoxModifyDescription.Text,
            textBoxM4BottomLineRecipe.Text,
            };
                string[] rParams = new string[]
                        {
                        "m1_param1",
                        "m2_param1",
                        "m3_param1",
                        "m4_param1",
                        "m5_param1",
                        "m6_param1",
                        "m3_param2",
                        "m4_param2",
                        "m4_param3",
                        "m4_param4",
                        "m_description",
                        "m4_param5",
                        };

                MySqlResult recs = mysqlService.DBTable[0].UpdateAutomaticPrimaryKey(comboBoxMRecipeName.Text, rParams, values);
                if (recs.Error == 0)
                {
                    res = xDialog.MsgBox.Show("Receta actualizada con éxito", "PBoot", xDialog.MsgBox.Buttons.OK);
                }
            }

            //update auto recipe
            UpdateRecipeToM1(comboBoxMRecipeName.Text);
            UpdateRecipeToM2(comboBoxMRecipeName.Text);
            UpdateRecipeToM3(comboBoxMRecipeName.Text);
            string shift = comboBoxShift.Text;
            UpdateRecipeToM4(comboBoxMRecipeName.Text, shift);
            UpdateRecipeToM6(comboBoxMRecipeName.Text);
            RefreshModelNameComboBox();
        }

        private void checkBoxM1RecipeModify_CheckedChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM1RecipeModify.CheckState == CheckState.Checked) ? true : false;

            checkBoxM1RecipeModify.ImageIndex = (chkValue) ? 0 : 1;
            checkBoxM1RecipeModify.Text = (chkValue) ? "on" : "off";
        }

        private void checkBoxM2RecipeModify_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM2RecipeModify.CheckState == CheckState.Checked) ? true : false;

            checkBoxM2RecipeModify.ImageIndex = (chkValue) ? 0 : 1;
            checkBoxM2RecipeModify.Text = (chkValue) ? "on" : "off";
        }

        private void checkBoxM3RecipeModify_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM3RecipeModify.CheckState == CheckState.Checked) ? true : false;

            checkBoxM3RecipeModify.ImageIndex = (chkValue) ? 0 : 1;
            checkBoxM3RecipeModify.Text = (chkValue) ? "on" : "off";
        }

        private void checkBoxM4RecipeModify_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM4RecipeModify.CheckState == CheckState.Checked) ? true : false;

            checkBoxM4RecipeModify.ImageIndex = (chkValue) ? 0 : 1;
            checkBoxM4RecipeModify.Text = (chkValue) ? "on" : "off";
        }

        private void checkBoxM5RecipeModify_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM5RecipeModify.CheckState == CheckState.Checked) ? true : false;

            checkBoxM5RecipeModify.ImageIndex = (chkValue) ? 0 : 1;
            checkBoxM5RecipeModify.Text = (chkValue) ? "on" : "off";
        }

        private void checkBoxM6RecipeModify_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM6RecipeModify.CheckState == CheckState.Checked) ? true : false;

            checkBoxM6RecipeModify.ImageIndex = (chkValue) ? 0 : 1;
            checkBoxM6RecipeModify.Text = (chkValue) ? "on" : "off";
        }

        private async void buttonMRecipeDeleteAll_Click(object sender, EventArgs e)
        {
            //update recipe
            DialogResult res = xDialog.MsgBox.Show("Are you sure you want to DELETE all recipies?", "PBoot", xDialog.MsgBox.Buttons.YesNo);
            if (res == DialogResult.Yes)
            {
                MySqlResult<recipies> result = await mysqlService.DBTable[0].SelectAllAsync<recipies>();

                foreach (var key in result.Result)
                {
                    mysqlService.DBTable[0].DeleteRowPrimaryKey(key.model_name);
                }
                comboBoxMRecipeName.Text = "";
                RefreshModelNameComboBox();
                res = xDialog.MsgBox.Show("All recipies deleted", "PBoot", xDialog.MsgBox.Buttons.OK);
            }
        }

        private async void buttonMRecipeDelete_Click(object sender, EventArgs e)
        {
            if (comboBoxMRecipeName.Text == "") return;

            //update recipe
            DialogResult res = xDialog.MsgBox.Show("Are you sure you want to DELETE recipe?", "PBoot", xDialog.MsgBox.Buttons.YesNo);
            if (res == DialogResult.Yes)
            {
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(comboBoxMRecipeName.Text);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    mysqlService.DBTable[0].DeleteRowPrimaryKey(comboBoxMRecipeName.Text);
                    comboBoxMRecipeName.Text = "";
                }
                RefreshModelNameComboBox();
                res = xDialog.MsgBox.Show("Recipe succesfully deleted", "PBoot", xDialog.MsgBox.Buttons.OK);
            }
        }

        private void comboBoxT0RecipeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
                List<IObjProgram> pList = new List<IObjProgram>();
                List<string> prgNameList = new List<string>();

                if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                {
                    ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                    pList = progRS.GetProgram(config.ProgramsPath[0], config.Extensions, comboBoxT0RecipeName.Text);

                    if (pList != null)
                    {
                        foreach (IObjProgram prgName in pList)
                        {
                            prgNameList.Add(prgName.ProgramName);
                        }
                    }

                    pList = progRS.GetProgram(config.ProgramsPath[1], config.Extensions, comboBoxT0RecipeName.Text);

                    if (pList != null)
                    {
                        foreach (IObjProgram prgName in pList)
                        {
                            prgNameList.Add(prgName.ProgramName);
                        }
                    }
                    pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, comboBoxT0RecipeName.Text);
                    comboBoxM3PrgName_st1.Items.Clear();

                    if (pList != null)
                    {
                        foreach (IObjProgram prgName in pList)
                        {
                            comboBoxM3PrgName_st1.Items.Add(prgName.ProgramName);
                            //prgNameList.Add(prgName.ProgramName);
                        }
                    }

                    pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, comboBoxT0RecipeName.Text);
                    comboBoxM3PrgName_st2.Items.Clear();
                    if (pList != null)
                    {
                        foreach (IObjProgram prgName in pList)
                        {
                            comboBoxM3PrgName_st2.Items.Add(prgName.ProgramName);
                            //prgNameList.Add(prgName.ProgramName);
                        }
                    }

                    //pad laser combobox
                    List<string> mList = new List<string>();
                    MySqlResult<padlaserprogram> result = mysqlService.DBTable[1].SelectAll<padlaserprogram>();
                    result.Result.ForEach(x => mList.Add(x.program_name));

                    if (mList != null)
                    {
                        foreach (string prgName in mList)
                        {
                            //filter by model name
                            if (prgName.Contains(comboBoxT0RecipeName.Text))
                                prgNameList.Add((string)prgName);
                        }
                    }
                    List<string> distinctList = prgNameList.Distinct().ToList();

                    comboBoxAutoPrgName.Items.Clear();

                    foreach (string item in distinctList.FindAll(X => X.Length == 15))
                    {
                        comboBoxAutoPrgName.Items.Add(item);
                    }
                    MySqlResult<recipies> resultA = mysqlService.DBTable[0].SelectByPrimaryKey<recipies>(comboBoxT0RecipeName.Text);
                    {
                        if (resultA.Result.Count != 0)
                        {
                            labelAutoDescription.Text = resultA.Result[0].m_description;
                        }
                    }
                }
                comboBoxM1TeachRecipeName.Text = comboBoxT0RecipeName.Text;
                comboBoxM1TestRecipeName.Text = comboBoxT0RecipeName.Text;
                comboBoxM2TeachRecipeName.Text = comboBoxT0RecipeName.Text;
                comboBoxM2TestRecipeName.Text = comboBoxT0RecipeName.Text;
                comboBoxM3TeachRecipeName.Text = comboBoxT0RecipeName.Text;
                comboBoxM3TestRecipeName.Text = comboBoxT0RecipeName.Text;
            }
            catch (Exception ex)
            {

            }
        }

        private void comboBoxM1TeachRecipeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            List<IObjProgram> pList = new List<IObjProgram>();

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                pList = progRS.GetProgram(config.ProgramsPath[0], config.Extensions, comboBoxM1TeachRecipeName.Text);
                comboBoxM1TeachProgramList.Items.Clear();

                if (pList != null)
                {
                    foreach (IObjProgram prgName in pList)
                    {
                        //filter by model name
                        if (prgName.ProgramName.Contains(comboBoxM1TeachRecipeName.Text))
                            comboBoxM1TeachProgramList.Items.Add(prgName.ProgramName);
                    }
                }
            }
        }

        private void RefreshM1TeachProgramList()
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            List<IObjProgram> pList = new List<IObjProgram>();

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                pList = progRS.GetProgram(config.ProgramsPath[0], config.Extensions, comboBoxM1TeachRecipeName.Text);
                comboBoxM1TeachProgramList.Items.Clear();
                if (pList != null)
                {
                    foreach (IObjProgram prgName in pList)
                    {
                        //filter by model name
                        if (prgName.ProgramName.Contains(comboBoxM1TeachRecipeName.Text))
                            comboBoxM1TeachProgramList.Items.Add(prgName.ProgramName);
                    }
                }
            }
        }

        private void comboBoxM1TestRecipeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            List<IObjProgram> pList = new List<IObjProgram>();

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                pList = progRS.GetProgram(config.ProgramsPath[0], config.Extensions, comboBoxM1TestRecipeName.Text);
                comboBoxM1TestProgramList.Items.Clear();
                if (pList != null)
                {
                    foreach (IObjProgram prgName in pList)
                    {
                        //filter by model name
                        if (prgName.ProgramName.Contains(comboBoxM1TestRecipeName.Text))
                            comboBoxM1TestProgramList.Items.Add(prgName.ProgramName);
                    }
                }
            }
        }

        private void comboBoxM2TeachRecipeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            List<IObjProgram> pList = new List<IObjProgram>();

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                pList = progRS.GetProgram(config.ProgramsPath[1], config.Extensions, comboBoxM2TeachRecipeName.Text);
                comboBoxM2TeachProgramList.Items.Clear();
                if (pList != null)
                {
                    foreach (IObjProgram prgName in pList)
                    {
                        //filter by model name
                        if (prgName.ProgramName.Contains(comboBoxM2TeachRecipeName.Text))
                            comboBoxM2TeachProgramList.Items.Add(prgName.ProgramName);
                    }
                }
            }
        }

        private void comboBoxM2TestRecipeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            List<IObjProgram> pList = new List<IObjProgram>();

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                pList = progRS.GetProgram(config.ProgramsPath[1], config.Extensions, comboBoxM2TestRecipeName.Text);
                comboBoxM2TestProgramList.Items.Clear();
                if (pList != null)
                {
                    foreach (IObjProgram prgName in pList)
                    {
                        //filter by model name
                        if (prgName.ProgramName.Contains(comboBoxM2TestRecipeName.Text))
                            comboBoxM2TestProgramList.Items.Add(prgName.ProgramName);
                    }
                }
            }
        }

        private void comboBoxM3TeachRecipeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            List<IObjProgram> pList = new List<IObjProgram>();

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, comboBoxM3TeachRecipeName.Text);
                comboBoxM3TeachProgramList.Items.Clear();
                if (pList != null)
                {
                    foreach (IObjProgram prgName in pList)
                    {
                        //filter by model name
                        if (prgName.ProgramName.Contains(comboBoxM3TeachRecipeName.Text))
                            comboBoxM3TeachProgramList.Items.Add(prgName.ProgramName);
                    }
                }
            }
        }

        private void comboBoxM3TestRecipeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            List<IObjProgram> pList = new List<IObjProgram>();

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, comboBoxM3TestRecipeName.Text);
                comboBoxM3TestProgramList.Items.Clear();
                if (pList != null)
                {
                    foreach (IObjProgram prgName in pList)
                    {
                        //filter by model name
                        if (prgName.ProgramName.Contains(comboBoxM3TestRecipeName.Text))
                            comboBoxM3TestProgramList.Items.Add(prgName.ProgramName);
                    }
                }
            }
        }

        private async void buttonM1ResetTest_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM1Reset";
            var sendResult = await ccService.Send(keyToSend, 1);
        }

        private async void buttonM2ResetTest_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM2Reset";
            var sendResult = await ccService.Send(keyToSend, 1);
        }

        private async void buttonM3ResetTest_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM3Reset";
            var sendResult = await ccService.Send(keyToSend, 1);
        }

        private void RunKeyboard()
        {
            Process foo = new Process();

            foo.StartInfo.FileName = @AppDomain.CurrentDomain.BaseDirectory + "RSAKeyboard.exe.lnk";

            //foo.StartInfo.Arguments = " 100 500 1 ";
            bool isRunning = false; //TODO: Check to see if process foo.exe is already running
            var processExists = Process.GetProcesses().Any(p => p.ProcessName.Contains("RSAKeyboard.exe.lnk"));

            if (processExists)
            {
                //TODO: Switch to foo.exe process
                foo.CloseMainWindow();
                foo.Start();
            }
            else
            {
                foo.Start();
            }
        }
        private void checkBoxT0Keyboard_Click(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBoxRecipeNewKey_CheckedChanged(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBox4_Click(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBox5_Click(object sender, EventArgs e)
        {
            RunKeyboard();
        }


        public void WritePadLaserRecipe(string strLine, string FileName)
        {
            try
            {
                using (TextWriter tw = new StreamWriter(FileName, false))
                {
                    tw.Write(strLine);
                }
                AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padLaser, "parametro " + strLine + "enviado a la laser");
            }
            catch (Exception ex)  //Writing to log has failed, send message to trace in case anyone is listening.
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padLaser, "error de acceso a la láser PC");
            }
        }

        private void radioButtonMRecipeTopSel1_CheckedChanged(object sender, EventArgs e)
        {
            textBoxM4TopLineRecipe.Enabled = (radioButtonMRecipeTopSel1.Checked) ? false : true;
            if (radioButtonMRecipeTopSel1.Checked) textBoxM4TopLineRecipe.Text = "___________________";
        }

        private void tabPageT1_3_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pInc = new Pen(Color.FromArgb(107, 227, 162), 10);
            Pen pExt = new Pen(Color.Black, 10);
            Brush bInc = new SolidBrush(Color.FromArgb(107, 227, 162));
            Brush bExt = new SolidBrush(Color.Black);
            Brush bText = new SolidBrush(Color.Black);
            int w = 12;
            int h = 12;
            int i = 1001;
            g.TranslateTransform(10, 10);

            foreach (KeyValuePair<int, bool> entry in M1InputDictionary)
            {
                if (entry.Value == true)
                {
                    g.DrawEllipse(pInc, 0, 0, w, h);
                    g.FillEllipse(bInc, new Rectangle(new Point(0, 0), new Size(w, h)));
                }
                else
                {
                    g.DrawEllipse(pExt, 0, 0, w, h);
                    g.FillEllipse(bExt, new Rectangle(new Point(0, 0), new Size(w, h)));
                }
                string tmp1 = "M1" + "_INPUT";
                string tmp2 = i.ToString();
                //g.DrawString(inputConfigurator.GetValue(tmp1, tmp2, ""), new Font("Verdana", 10), bText, new Point(20, 0));
                string localizedText = inputConfigurator.GetValueWithLanguage(tmp1, tmp2, currentLanguageCode, "");
                g.DrawString(localizedText, new Font("Verdana", 10), bText, new Point(20, 0));
                if (i < 1024)
                    g.TranslateTransform(0, 30);
                else if (i == 1024)
                {
                    g.ResetTransform();
                    g.TranslateTransform(360, 10);
                }
                else
                {
                    g.TranslateTransform(0, 30);

                }
                i = i + 1;
            }
            tabPageT1_3.Invalidate();

        }

        private void tabPageT1_4_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pInc = new Pen(Color.FromArgb(107, 227, 162), 10);
            Pen pExt = new Pen(Color.Black, 10);
            Brush bInc = new SolidBrush(Color.FromArgb(107, 227, 162));
            Brush bExt = new SolidBrush(Color.Black);
            Brush bText = new SolidBrush(Color.Black);
            int w = 12;
            int h = 12;
            int i = 2001;
            g.TranslateTransform(10, 10);

            foreach (KeyValuePair<int, bool> entry in M1OutputDictionary)
            {
                if (entry.Value == true)
                {
                    g.DrawEllipse(pInc, 0, 0, w, h);
                    g.FillEllipse(bInc, new Rectangle(new Point(0, 0), new Size(w, h)));
                }
                else
                {
                    g.DrawEllipse(pExt, 0, 0, w, h);
                    g.FillEllipse(bExt, new Rectangle(new Point(0, 0), new Size(w, h)));
                }
                string tmp1 = "M1" + "_OUTPUT";
                string tmp2 = i.ToString();
                //g.DrawString(outputConfigurator.GetValue(tmp1, tmp2, ""), new Font("Verdana", 10), bText, new Point(20, 0));
                string localizedText = outputConfigurator.GetValueWithLanguage(tmp1, tmp2, currentLanguageCode, "");
                g.DrawString(localizedText, new Font("Verdana", 10), bText, new Point(20, 0));
                if (i < 2024)
                    g.TranslateTransform(0, 30);
                else if (i == 2024)
                {
                    g.ResetTransform();
                    g.TranslateTransform(360, 10);
                }
                else
                {
                    g.TranslateTransform(0, 30);

                }
                i = i + 1;
            }
            tabPageT1_4.Invalidate();
        }

        public bool CheckProgramSyntaxName(string prgName)
        {
            //check lenght
            int size = prgName.Length;
            if (size > 15) return false;
            //check separator
            string[] splitted = prgName.Split('-');
            if (splitted.Length != 3) return false;

            return true;
        }

        private async void buttonCopyPrograms_Click(object sender, EventArgs e)
        {
            List<IObjProgram> pList = new List<IObjProgram>();

            //check if origin recipe and destination recipe is already created
            //get data from DB
            MySqlResult<recipies> recO = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(comboBoxOriginRecipe.Text);
            MySqlResult<recipies> recD = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(comboBoxDestinationRecipe.Text);

            if ((recO.Error != 0) || (recO.Result.Count == 0))
            {
                xDialog.MsgBox.Show("La receta de ORIGEN no esiste", "PBoot", xDialog.MsgBox.Buttons.OK);
                return;
            }

            if ((recD.Error != 0) || (recD.Result.Count == 0))
            {
                xDialog.MsgBox.Show("La receta de DESTINACION no esiste", "PBoot", xDialog.MsgBox.Buttons.OK);
                return;
            }

            //condition to copy
            if ((comboBoxOriginRecipe.Text != "" & comboBoxDestinationRecipe.Text != ""))
            {
                //ask user to close application
                DialogResult res = xDialog.MsgBox.Show("Está seguro de que desea copiar la receta?", "PBoot", xDialog.MsgBox.Buttons.YesNo);
                if (res == DialogResult.Yes)
                {
                    var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
                    List<string> prgNameList = new List<string>();

                    if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                    {
                        //copy trimmer programs
                        ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                        pList = progRS.GetProgram(config.ProgramsPath[0], config.Extensions, comboBoxOriginRecipe.Text);

                        if (pList != null)
                        {
                            foreach (IObjProgram prgName in pList)
                            {
                                string suffix = prgName.ProgramName.Substring(13, 2);
                                //copy program with destination
                                string destinationFile = config.ProgramsPath[0] + "\\" + "PR" + comboBoxDestinationRecipe.Text + "-" + prgName.Size + "-" + prgName.Foot + suffix + ".prog";
                                File.Copy(prgName.FilePath, destinationFile, true);
                            }
                        }

                        //copy padint programs
                        pList = progRS.GetProgram(config.ProgramsPath[1], config.Extensions, comboBoxOriginRecipe.Text);

                        if (pList != null)
                        {
                            foreach (IObjProgram prgName in pList)
                            {
                                string suffix = prgName.ProgramName.Substring(13, 2);
                                //copy program with destination
                                string destinationFile = config.ProgramsPath[1] + "\\" + "PR" + comboBoxDestinationRecipe.Text + "-" + prgName.Size + "-" + prgName.Foot + suffix + ".prog";
                                File.Copy(prgName.FilePath, destinationFile, true);
                            }
                        }

                        //copy padext programs
                        pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, comboBoxOriginRecipe.Text);

                        if (pList != null)
                        {
                            foreach (IObjProgram prgName in pList)
                            {
                                string suffix = prgName.ProgramName.Substring(13, 2);
                                //copy program with destination
                                string destinationFile = config.ProgramsPath[2] + "\\" + "PR" + comboBoxDestinationRecipe.Text + "-" + prgName.Size + "-" + prgName.Foot + suffix + ".prog";
                                File.Copy(prgName.FilePath, destinationFile, true);
                            }
                        }

                        CopyLaserProgram(comboBoxOriginRecipe.Text, comboBoxDestinationRecipe.Text);
                    }
                }
                xDialog.MsgBox.Show("copia realizada exitosamente", "PBoot", xDialog.MsgBox.Buttons.OK);
            }
            else xDialog.MsgBox.Show("receta no seleccionada", "PBoot", xDialog.MsgBox.Buttons.OK);
        }

        private void buttonDeleteByRecipeM1_Click(object sender, EventArgs e)
        {
            List<IObjProgram> pList = new List<IObjProgram>();

            if (comboBoxRecipeDelete.Text == "") return;
            //ask user to close application
            DialogResult res = xDialog.MsgBox.Show("Está seguro de que desea eliminar todos los programas de la receta en la REFILADORA?", "PBoot", xDialog.MsgBox.Buttons.YesNo);
            if (res == DialogResult.Yes)
            {
                var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
                List<string> prgNameList = new List<string>();

                if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                {
                    //copy trimmer programsReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                    ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                    pList = progRS.GetProgram(config.ProgramsPath[0], config.Extensions, comboBoxRecipeDelete.Text);

                    if (pList != null)
                    {
                        foreach (IObjProgram prgName in pList)
                        {
                            //DELETE program with destination
                            File.Delete(prgName.FilePath);
                        }
                    }
                    xDialog.MsgBox.Show("todos los programas de la refiladora con receta " + comboBoxRecipeDelete.Text + "eliminados.", "PBoot", xDialog.MsgBox.Buttons.OK);
                }
            }
        }

        private void buttonDeleteByRecipeM2_Click(object sender, EventArgs e)
        {
            List<IObjProgram> pList = new List<IObjProgram>();

            if (comboBoxRecipeDelete.Text == "") return;
            //ask user to close application
            DialogResult res = xDialog.MsgBox.Show("Está seguro de que desea eliminar todos los programas de la receta " + comboBoxRecipeDelete.Text + " en la TAMPOGRAFIA INTIERNA?", "PBoot", xDialog.MsgBox.Buttons.YesNo);
            if (res == DialogResult.Yes)
            {
                var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
                List<string> prgNameList = new List<string>();

                if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                {
                    //copy trimmer programsReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                    ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                    pList = progRS.GetProgram(config.ProgramsPath[1], config.Extensions, comboBoxRecipeDelete.Text);

                    if (pList != null)
                    {
                        foreach (IObjProgram prgName in pList)
                        {
                            //DELETE program with destination
                            File.Delete(prgName.FilePath);
                        }
                    }
                    xDialog.MsgBox.Show("todos los programas de la TAMPOGRAFIA INTIERNA con receta " + comboBoxRecipeDelete.Text + " eliminados.", "PBoot", xDialog.MsgBox.Buttons.OK);
                }
            }
        }

        private void buttonDeleteByRecipeM3_Click(object sender, EventArgs e)
        {
            List<IObjProgram> pList = new List<IObjProgram>();

            if (comboBoxRecipeDelete.Text == "") return;
            //ask user to close application
            DialogResult res = xDialog.MsgBox.Show("Está seguro de que desea eliminar todos los programas de la receta " + comboBoxRecipeDelete.Text + " en la TAMPOGRAFIA ESTERNA?", "PBoot", xDialog.MsgBox.Buttons.YesNo);
            if (res == DialogResult.Yes)
            {
                var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
                List<string> prgNameList = new List<string>();

                if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                {
                    //copy trimmer programsReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                    ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                    pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, comboBoxRecipeDelete.Text);

                    if (pList != null)
                    {
                        foreach (IObjProgram prgName in pList)
                        {
                            //DELETE program with destination
                            File.Delete(prgName.FilePath);
                        }
                    }
                    xDialog.MsgBox.Show("todos los programas de la TAMPOGRAFIA ESTERNA con receta " + comboBoxRecipeDelete.Text + " eliminados.", "PBoot", xDialog.MsgBox.Buttons.OK);
                }
            }
        }

        private void buttonDeleteByRecipeM4_Click(object sender, EventArgs e)
        {
            List<string> pList = new List<string>();

            if (comboBoxRecipeDelete.Text == "") return;
            //ask user to close application
            DialogResult res = xDialog.MsgBox.Show("Está seguro de que desea eliminar todos los programas de la receta " + comboBoxRecipeDelete.Text + " en la LASER?", "PBoot", xDialog.MsgBox.Buttons.YesNo);
            if (res == DialogResult.Yes)
            {
                MySqlResult<padlaserprogram> result = mysqlService.DBTable[1].SelectPrimaryCointains<padlaserprogram>(comboBoxRecipeDelete.Text);


                result.Result.ForEach(x => pList.Add(x.program_name));

                if (pList != null)
                {
                    foreach (string prgName in pList)
                    {
                        MySqlResult recd = mysqlService.DBTable[1].DeleteRowPrimaryKey(prgName);
                    }
                    xDialog.MsgBox.Show("todos los programas de la LASER con receta " + comboBoxRecipeDelete.Text + " eliminados.", "PBoot", xDialog.MsgBox.Buttons.OK);
                }
            }
        }


        private void CopyLaserProgram(string modelOrigin, string modelDestination)
        {
            object[] value2 = null;

            //get all programs with model name origin
            MySqlResult<padlaserprogram> result = mysqlService.DBTable[1].SelectPrimaryCointains<padlaserprogram>(modelOrigin);
            if (result.Result.Count != 0)
            {
                foreach (var key in result.Result)
                {
                    //new program name
                    string copyPrgName = "PR" + modelDestination + "-" + key.program_name.Substring(7, 8);

                    value2 = new object[]
                    {
                        copyPrgName,
                        key.Q1,
                        key.Q2,
                        key.Q3,
                        key.Q4,
                        key.S1,
                        key.S2,
                        key.S3,
                        key.S4,
                        key.T1,
                    };

                    string[] rParams = new string[]
                       {
                        "Q1",
                        "Q2",
                        "Q3",
                        "Q4",
                        "S1",
                        "S2",
                        "S3",
                        "S4",
                        "T1",
                       };

                    //check if it's present
                    MySqlResult<padlaserprogram> resultP = mysqlService.DBTable[1].SelectByPrimaryKey<padlaserprogram>(copyPrgName);
                    if (resultP.Result.Count != 0)
                    {
                        //it's present update
                        MySqlResult resultI = mysqlService.DBTable[1].UpdateAutomaticPrimaryKey(copyPrgName, rParams, value2);
                    }
                    else if (resultP.Result.Count == 0)
                    {
                        //not present insert
                        MySqlResult resultI = mysqlService.DBTable[1].InsertAutomatic(value2);

                        if (resultI.Error == 0)
                        {

                        }
                    }
                }
            }
        }

        private void comboBoxRecipeDelete_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            List<IObjProgram> pList = new List<IObjProgram>();
            List<string> prgNameList = new List<string>();

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                pList = progRS.GetProgram(config.ProgramsPath[0], config.Extensions, comboBoxRecipeDelete.Text);

                if (pList != null)
                {
                    foreach (IObjProgram prgName in pList)
                    {
                        prgNameList.Add(prgName.ProgramName);
                    }
                }

                pList = progRS.GetProgram(config.ProgramsPath[1], config.Extensions, comboBoxRecipeDelete.Text);

                if (pList != null)
                {
                    foreach (IObjProgram prgName in pList)
                    {
                        prgNameList.Add(prgName.ProgramName);
                    }
                }
                pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, comboBoxRecipeDelete.Text);

                if (pList != null)
                {
                    foreach (IObjProgram prgName in pList)
                    {
                        prgNameList.Add(prgName.ProgramName);
                    }
                }

                pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, comboBoxRecipeDelete.Text);

                if (pList != null)
                {
                    foreach (IObjProgram prgName in pList)
                    {
                        prgNameList.Add(prgName.ProgramName);
                    }
                }

                //pad laser combobox
                List<string> mList = new List<string>();
                MySqlResult<padlaserprogram> result = mysqlService.DBTable[1].SelectAll<padlaserprogram>();
                result.Result.ForEach(x => mList.Add(x.program_name));
                List<string> distinctList = prgNameList.Distinct().ToList();

                comboBoxProgramNameDelete.Items.Clear();

                foreach (string item in distinctList.FindAll(X => X.Length == 15))
                {
                    comboBoxProgramNameDelete.Items.Add(item);
                }
            }
        }

        private void buttonDeleteProgramM1_Click(object sender, EventArgs e)
        {
            if (comboBoxProgramNameDelete.Text == "") return;
            //ask user to close application
            DialogResult res = xDialog.MsgBox.Show("Está seguro de que desea eliminar el programa " + comboBoxProgramNameDelete.Text + "en la REFILADORA?", "PBoot", xDialog.MsgBox.Buttons.YesNo);
            if (res == DialogResult.Yes)
            {
                var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
                List<string> prgNameList = new List<string>();

                if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                {
                    ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;

                    //DELETE program with destination
                    File.Delete(config.ProgramsPath[0] + "\\" + comboBoxProgramNameDelete.Text + ".prog");

                    xDialog.MsgBox.Show("el programa de la refiladora " + comboBoxProgramNameDelete.Text + "eliminado.", "PBoot", xDialog.MsgBox.Buttons.OK);
                }
            }
        }

        private void buttonDeleteProgramM4_Click(object sender, EventArgs e)
        {
            List<string> pList = new List<string>();

            if (comboBoxProgramNameDelete.Text == "") return;
            //ask user to close application
            DialogResult res = xDialog.MsgBox.Show("Está seguro de que desea eliminar el programa " + comboBoxProgramNameDelete.Text + " en la LASER?", "PBoot", xDialog.MsgBox.Buttons.YesNo);
            if (res == DialogResult.Yes)
            {
                MySqlResult recd = mysqlService.DBTable[1].DeleteRowPrimaryKey(comboBoxProgramNameDelete.Text);
                if (recd.Error == 0)
                    xDialog.MsgBox.Show("el programa " + comboBoxProgramNameDelete.Text + " eliminado en la LASER.", "PBoot", xDialog.MsgBox.Buttons.OK);
                else
                    xDialog.MsgBox.Show("el programa " + comboBoxProgramNameDelete.Text + " non esiste en la LASER.", "PBoot", xDialog.MsgBox.Buttons.OK);
            }
        }

        private void buttonDeleteProgramM2_Click(object sender, EventArgs e)
        {
            if (comboBoxProgramNameDelete.Text == "") return;
            //ask user to close application
            DialogResult res = xDialog.MsgBox.Show("Está seguro de que desea eliminar el programa " + comboBoxProgramNameDelete.Text + "en la TAMPOGRAFIA INTIERNA?", "PBoot", xDialog.MsgBox.Buttons.YesNo);
            if (res == DialogResult.Yes)
            {
                var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
                List<string> prgNameList = new List<string>();

                if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                {
                    ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;

                    //DELETE program with destination
                    File.Delete(config.ProgramsPath[1] + "\\" + comboBoxProgramNameDelete.Text + ".prog");

                    xDialog.MsgBox.Show("el programa de la tampografia intierna " + comboBoxProgramNameDelete.Text + "eliminado.", "PBoot", xDialog.MsgBox.Buttons.OK);
                }
            }

        }

        private void buttonDeleteProgramM3_Click(object sender, EventArgs e)
        {
            if (comboBoxProgramNameDelete.Text == "") return;
            //ask user to close application
            DialogResult res = xDialog.MsgBox.Show("Está seguro de que desea eliminar el programa " + comboBoxProgramNameDelete.Text + "en la TAMPOGRAFIA ESTERNA?", "PBoot", xDialog.MsgBox.Buttons.YesNo);
            if (res == DialogResult.Yes)
            {
                var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
                List<string> prgNameList = new List<string>();

                if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                {
                    ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;

                    //DELETE program with destination
                    File.Delete(config.ProgramsPath[2] + "\\" + comboBoxProgramNameDelete.Text + ".prog");

                    xDialog.MsgBox.Show("el programa de la tampografia esterna " + comboBoxProgramNameDelete.Text + "eliminado.", "PBoot", xDialog.MsgBox.Buttons.OK);
                }
            }
        }

        private async void comboBoxAutoPrgName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected == false) return;

            bool allProgramsPresent = true;
            #region(* send to trimmer *)
            //send to trimmer
            if (comboBoxAutoPrgName.Text == "") return;

            //get model name
            string prgName = comboBoxAutoPrgName.Text;
            M1PrgName = comboBoxAutoPrgName.Text;
            string modelName = prgName.Substring(2, 4);

            //check model name
            if (modelName == "")
            {
                //program name with incorrect format
                AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.line, "nombre de receta incorrecto " + modelName);
                return;
            }

            //send size in production (to be shown on panel)
            SendSizeFromM1ProgramToOpc();

            try
            {
                //get data from DB
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    //send recipe on/off
                    string keyValue = "pcM1Param1";
                    var sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m1_param1.ToString()));
                    //                labelM1Param1Value.Text = recs.Result[0].m1_param1.ToString();
                    M1AutoDictionary[1] = short.Parse(recs.Result[0].m1_param1.ToString());

                    //send quote, speed, timer
                    var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

                    if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                    {
                        ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                        ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                        objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[0] + "\\" + comboBoxAutoPrgName.Text + config.Extensions[0]);
                        if (objPoints != null)
                        {
                            List<string> keys = new List<string>()
                    {
                        "pcM1AutoQuote",
                        "pcM1AutoSpeed"
                    };

                            List<object> values = new List<object>()
                    {
                        new float[] { 0, (float)objPoints.Points[0].Q1, (float)objPoints.Points[0].Q2, (float)objPoints.Points[0].Q3, (float)objPoints.Points[0].Q4},
                        new short[] { 0, (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                    };

                            var sendResults = await ccService.Send(keys, values);
                            bool allsent = true;
                            foreach (var result in sendResults)
                            {
                                if (result.Value.OpcResult)
                                {
                                }
                                else
                                {
                                    AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.trimmer, "error al enviar quotas " + result.Value.NodeString);
                                }
                                allsent = allsent & result.Value.OpcResult;
                            }
                            if (allsent) AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.trimmer, "programa enviado exitosamente " + M1PrgName);

                            string key = "pc_timer_stop_stivale";
                            var sendResultsTimer = await ccService.Send("pcM1AutoTimer", objPoints.Points[0].CustomFloatParam);
                            if (sendResultsTimer.OpcResult)
                            {
                            }
                            else
                            {

                            }
                            groupBoxM1.Text = "refiladora " + M1PrgName;
                        }
                        else
                        {
                            allProgramsPresent = allProgramsPresent & false;
                        }
                    }
                }
                else
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.trimmer, "revisa el programa " + M1PrgName);
                }
            }
            catch (Exception ex)
            {

            }
            #endregion

            #region(* send to padint *)
            //get model name
            M2PrgName = comboBoxAutoPrgName.Text;

            try
            {
                //get data from DB
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    //send recipe on/off
                    string keyValue = "pcM2Param1";
                    var sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m2_param1.ToString()));
                    if (sendResult.OpcResult)
                    {
                        //labelM2Param1Value.Text = recs.Result[0].m2_param1.ToString();
                        M2AutoDictionary[1] = short.Parse(recs.Result[0].m2_param1.ToString());
                    }
                    else
                    {
                        //todo manage log/user error
                    }

                    //send quote, speed
                    var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

                    if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                    {
                        ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                        ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                        objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[1] + "\\" + comboBoxAutoPrgName.Text + config.Extensions[0]);
                        if (objPoints != null)
                        {
                            List<string> keys = new List<string>()
                    {
                        "pcM2AutoQuote",
                        "pcM2AutoSpeed"
                    };

                            List<object> values = new List<object>()
                    {
                        new float[] {0, (float)objPoints.Points[0].Q1, (float)objPoints.Points[0].Q2, (float)objPoints.Points[0].Q3, (float)objPoints.Points[0].Q4},
                        new short[] {0, (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                    };

                            var sendResults = await ccService.Send(keys, values);
                            bool allsent = true;
                            foreach (var result in sendResults)
                            {
                                if (result.Value.OpcResult)
                                {
                                }
                                else
                                {
                                    AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padprintInt, "error al enviar quotas  " + result.Value.NodeString);
                                }
                                allsent = allsent & result.Value.OpcResult;
                            }
                            if (allsent) AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padprintInt, "programa enviado exitosamente " + M2PrgName);
                            string key = "pc_timer_stop_stivale";
                            var sendResultsTimer = await ccService.Send("pcM2AutoTimer", objPoints.Points[0].CustomFloatParam);
                            if (sendResultsTimer.OpcResult)
                            {
                            }
                            else
                            {

                            }
                            groupBoxM2.Text = "tamp. int. " + M2PrgName;
                        }
                    }
                    else
                    {
                        AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padprintInt, "revisa el programa " + M2PrgName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            #endregion

            #region(* send to laser *)
            //if (comboBoxM4PrgName.Text == "") return;

            //get model name
            //string prgName = comboBoxM4PrgName.Text;
            M4PrgName = comboBoxAutoPrgName.Text;
            //string modelName = prgName.Substring(2, 4);

            try
            {
                //get data from DB
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    //send recipe
                    string keyValue = "pcM4Param1";
                    var sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m4_param1.ToString()));
                    //labelM4Param1Value.Text = recs.Result[0].m4_param1.ToString();
                    M4AutoDictionary[1] = short.Parse(recs.Result[0].m4_param1.ToString());
                    string keyToSend = "pcM4ProgramName";

                    var readResult = await ccService.Send(keyToSend, "");
                    if (readResult.OpcResult)
                    {

                    }
                    else
                    {

                    }

                    keyToSend = "pcM4ProgramName";

                    readResult = await ccService.Send(keyToSend, M4PrgName);
                    if (readResult.OpcResult)
                    {
                        AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padLaser, "programa enviado exitosamente " + M4PrgName);
                    }
                    else
                    {
                        AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padLaser, "error en enviar programa " + readResult.NodeString);
                    }

                    string top = "";
                    string medium = "";
                    string bottom = "";

                    //selection code
                    if (recs.Result[0].m4_param2.Substring(0, 1).Equals("0"))
                    {
                        top = recs.Result[0].m4_param3.ToString();
                    }
                    else top = DateTime.Now.ToString("dd-MM-yyyy");

                    if (recs.Result[0].m4_param2.Substring(1, 1).Equals("0"))
                    {
                        medium = recs.Result[0].m4_param4.ToString();
                    }
                    else medium = DateTime.Now.ToString("dd-MM-yyyy");

                    if (recs.Result[0].m4_param2.Substring(2, 1).Equals("0"))
                    {
                        bottom = recs.Result[0].m4_param5.ToString();
                    }
                    else bottom = DateTime.Now.ToString("dd-MM-yyyy");
                    string shift = comboBoxShift.Text;
                    //sned to padlaser
                    WritePadLaserRecipe(top, Properties.Settings.Default.PadLaserFilePathTop);
                    WritePadLaserRecipe(medium, Properties.Settings.Default.PadLaserFilePathMedium);
                    WritePadLaserRecipe(comboBoxInj.Text + " " + bottom + " " + shift, Properties.Settings.Default.PadLaserFilePathBottom);

                    groupBoxM4.Text = "laser " + M4PrgName;
                }
                else
                {
                    //db error manage
                }
            }
            catch (Exception ex)
            {

            }

            ////check all programs
            //if (M1PrgName.Equals(comboBoxAutoPrgName.Text))
            //{
            //    groupBoxM1.ForeColor = Color.Black;
            //}
            //else
            //{
            //    groupBoxM1.ForeColor = Color.Red;
            //    AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.trimmer, "programa no existe ");
            //}
            //if (M2PrgName.Equals(comboBoxAutoPrgName.Text))
            //{
            //    groupBoxM2.ForeColor = Color.Black;
            //}
            //else
            //{
            //    groupBoxM2.ForeColor = Color.Red;
            //    AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padprintInt, "programa no existe ");
            //}
            //if (M4PrgName.Equals(comboBoxAutoPrgName.Text))
            //{
            //    groupBoxM4.ForeColor = Color.Black;
            //} groupBoxM4.ForeColor = Color.Red;

            #endregion
        }

        private void UpdateGUIByUser()
        {
            tabPageT0_5.Enabled = (loginLevel == "Operador") ? false : true;
        }

        private void buttonSwitchOperator_Click(object sender, EventArgs e)
        {
            loginLevel = "Operador";
            labelUserLogin.Text = loginLevel;
            UpdateGUIByUser();
            AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.line, "usuario actual: Operador");
        }

        private void buttonAdminLogin_Click(object sender, EventArgs e)
        {
            if ((textBoxPasswordLogin.Text).Equals(loginPassword))
            {
                loginLevel = "Administrador";
                labelUserLogin.Text = loginLevel;
                UpdateGUIByUser();
                AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.line, "usuario actual: Administrador");
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.line, "password incorrecta");
            }
        }

        private void buttonChangePassword_Click(object sender, EventArgs e)
        {
            groupBoxLogin.Visible = true;
        }

        private void buttonApplyNewPassword_Click(object sender, EventArgs e)
        {
            if ((RemoveWhitespace(textBoxCurrentPassword.Text)).Equals(loginPassword))
            {
                if ((RemoveWhitespace(textBoxNewPassword.Text)).Equals(RemoveWhitespace(textBoxRepeatNewPassword.Text)))
                {
                    loginPassword = textBoxNewPassword.Text;
                    groupBoxLogin.Visible = false;
                }
                else
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.line, "password repetida incorrecta");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.line, "password incorrecta");
            }
        }
        public string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            groupBoxLogin.Visible = false;
        }

        private void tabPageT5_4_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pInc = new Pen(Color.FromArgb(107, 227, 162), 10);
            Pen pExt = new Pen(Color.Red, 10);
            Brush bInc = new SolidBrush(Color.FromArgb(107, 227, 162));
            Brush bExt = new SolidBrush(Color.Red);
            Brush bText = new SolidBrush(Color.Black);
            int w = 12;
            int h = 12;
            int i = 1;
            g.TranslateTransform(10, 10);

            foreach (KeyValuePair<int, short> entry in M5AlarmsDictionary)
            {
                if ((i <= 7) || ((i > 32) & (i < 39)))
                {
                    if (entry.Value == 0)
                    {
                        g.DrawEllipse(pInc, 0, 0, w, h);
                        g.FillEllipse(bInc, new Rectangle(new Point(0, 0), new Size(w, h)));
                    }
                    else
                    {
                        g.DrawEllipse(pExt, 0, 0, w, h);
                        g.FillEllipse(bExt, new Rectangle(new Point(0, 0), new Size(w, h)));
                    }
                }
                string tmp1 = "M5";
                string tmp2 = "A" + i.ToString();

                if (i <= 7)
                {
                    g.DrawString(alarmsConfigurator.GetValue(tmp1, tmp2, ""), new Font("Verdana", 10), bText, new Point(20, 0));
                    g.DrawString("consulte el alarma " + tmp2 + " en el manual.", new Font("Verdana", 10), bText, new Point(300, 0));
                    g.TranslateTransform(0, 30);
                }
                else if ((i > 32) & (i < 39))
                {
                    g.DrawString(alarmsConfigurator.GetValue(tmp1, tmp2, ""), new Font("Verdana", 10), bText, new Point(20, 0));
                    g.DrawString("consulte el alarma " + tmp2 + " en el manual.", new Font("Verdana", 10), bText, new Point(300, 0));
                    g.TranslateTransform(0, 30);

                }
                i = i + 1;
            }

            g.TranslateTransform(0, 100);

            foreach (KeyValuePair<int, short> entry in M5StateDictionary)
            {
                string stateStr = "";
                if (entry.Key == 1) stateStr = "fase del ciclo";
                if (entry.Key == 2) stateStr = "fase del la banda de traslacion";
                if (entry.Key == 3) stateStr = "fase del la banda de salida 1";
                if (entry.Key == 4) stateStr = "fase del la banda de salida 2";
                if (entry.Key == 5) stateStr = "fase del la banda de salida 3";

                g.DrawString(stateStr + " " + entry.Value, new Font("Verdana", 10), bText, new Point(0, 0));

                g.TranslateTransform(0, 30);
            }
            tabPageT5_4.Invalidate();
        }

        private void groupBoxM1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush bText = new SolidBrush(Color.Black);
            int w = 12;
            int h = 12;
            int i = 1;
            g.TranslateTransform(10, 20);
            g.DrawString((M1AutoDictionary[1] == 0) ? "par. receta OFF" : "par. receta ON", new Font("Verdana", 10), bText, new Point(0, 0));
            g.TranslateTransform(0, 30);
            //cycle counter
            g.DrawString("ciclos " + M1AutoDictionary[2], new Font("Verdana", 10), bText, new Point(0, 0));
            g.TranslateTransform(0, 30);
            float value = float.Parse((M1AutoDictionary[3] / 100.0f).ToString());
            g.DrawString("tiempo (s) " + value.ToString(), new Font("Verdana", 10), bText, new Point(0, 0));
            g.TranslateTransform(140, -54);
            Rectangle r = new Rectangle(new Point(0, 0), new Size(6, 6));

            string statusStr = "";
            if (M1AutoDictionary[4] == -1)
            {
                g.DrawRectangle(pBlack, r);
                g.FillRectangle(bBlack, r);
                statusStr = "sin conexión";
            }
            if (M1AutoDictionary[4] == 0)
            {
                g.DrawRectangle(pRed, r);
                g.FillRectangle(bRed, r);
                statusStr = "emergencia";
            }
            if (M1AutoDictionary[4] == 1)
            {
                g.DrawRectangle(pc1, r);
                g.FillRectangle(bc1, r);
                statusStr = "automatico";
            }
            if (M1AutoDictionary[4] == 2)
            {
                g.DrawRectangle(pOrange, r);
                g.FillRectangle(bOrange, r);
                statusStr = "manual";
            }
            if (M1AutoDictionary[4] == 3)
            {
                g.DrawRectangle(pc2, r);
                g.FillRectangle(bc2, r);
                statusStr = "en ciclo";
            }
            if (M1AutoDictionary[4] == 4)
            {
                g.DrawRectangle(pDarkOrange, r);
                g.FillRectangle(bDarkOrange, r);
                statusStr = "en alarma";
            }
            g.DrawString(statusStr, new Font("Verdana", 10), bText, new Point(10, -5));

        }

        private void checkboxKeyboardLogin_Click(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void buttonRecipeTable_Click(object sender, EventArgs e)
        {
            this.tabControlMain.SelectedPage = tabPageT0_2_3;
        }

        private void tabPageT0_2_3_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pInc = new Pen(Color.FromArgb(107, 227, 162), 10);
            Pen pExt = new Pen(Color.Red, 10);
            Brush bInc = new SolidBrush(Color.FromArgb(107, 227, 162));
            Brush bExt = new SolidBrush(Color.Red);
            Brush bText = new SolidBrush(Color.Black);
            int i = 1;

            g.TranslateTransform(10, 10);
            g.DrawString("receta" + "  " + "descripcion", new Font("Verdana", 14), bText, new Point(20, 0));
            g.TranslateTransform(0, 30);
            int j = 1;
            foreach (var key in ModelNameDictionary)
            {

                g.DrawString(key.Value + "      " + DescriptionDictionary[j], new Font("Verdana", 12), bText, new Point(20, 0));
                if ((i > 25))
                {
                    g.TranslateTransform(160, 0);
                }

                if ((i > 50))
                {
                    g.TranslateTransform(320, 0);
                }
                g.TranslateTransform(0, 30);
                i = i + 1;
                j = j + 1;
            }
            //tabPageT2_3.Invalidate();
        }

        private void groupBoxM2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush bText = new SolidBrush(Color.Black);
            int w = 12;
            int h = 12;
            int i = 1;
            g.TranslateTransform(10, 20);
            g.DrawString((M2AutoDictionary[1] == 0) ? "par. receta OFF" : "par. receta ON", new Font("Verdana", 10), bText, new Point(0, 0));
            g.TranslateTransform(0, 30);
            //cycle counter
            g.DrawString("ciclos " + M2AutoDictionary[2], new Font("Verdana", 10), bText, new Point(0, 0));
            g.TranslateTransform(0, 30);
            float value = float.Parse((M2AutoDictionary[3] / 100.0f).ToString());
            g.DrawString("tiempo (s) " + value.ToString(), new Font("Verdana", 10), bText, new Point(0, 0));

            g.TranslateTransform(140, -54);
            Rectangle r = new Rectangle(new Point(0, 0), new Size(6, 6));

            string statusStr = "";
            if (M2AutoDictionary[4] == -1)
            {
                g.DrawRectangle(pBlack, r);
                g.FillRectangle(bBlack, r);
                statusStr = "sin conexión";
            }
            if (M2AutoDictionary[4] == 0)
            {
                g.DrawRectangle(pRed, r);
                g.FillRectangle(bRed, r);
                statusStr = "emergencia";
            }
            if (M2AutoDictionary[4] == 1)
            {
                g.DrawRectangle(pc1, r);
                g.FillRectangle(bc1, r);
                statusStr = "automatico";
            }
            if (M2AutoDictionary[4] == 2)
            {
                g.DrawRectangle(pOrange, r);
                g.FillRectangle(bOrange, r);
                statusStr = "manual";
            }
            if (M2AutoDictionary[4] == 3)
            {
                g.DrawRectangle(pc2, r);
                g.FillRectangle(bc2, r);
                statusStr = "en ciclo";
            }
            if (M2AutoDictionary[4] == 4)
            {
                g.DrawRectangle(pDarkOrange, r);
                g.FillRectangle(bDarkOrange, r);
                statusStr = "en alarma";
            }
            g.DrawString(statusStr, new Font("Verdana", 10), bText, new Point(10, -5));
            //groupBoxM2.Invalidate();
        }

        private void groupBoxM4_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush bText = new SolidBrush(Color.Black);
            g.TranslateTransform(10, 20);
            g.DrawString((M4AutoDictionary[1] == 0) ? "par. receta OFF" : "par. receta ON", new Font("Verdana", 10), bText, new Point(0, 0));
            g.TranslateTransform(0, 30);
            //cycle counter
            g.DrawString("ciclos " + M4AutoDictionary[2], new Font("Verdana", 10), bText, new Point(0, 0));
            g.TranslateTransform(0, 30);
            float value = float.Parse((M4AutoDictionary[3] / 100.0f).ToString());
            g.DrawString("tiempo (s) " + value.ToString(), new Font("Verdana", 10), bText, new Point(0, 0));

            g.TranslateTransform(140, -54);
            Rectangle r = new Rectangle(new Point(0, 0), new Size(6, 6));

            string statusStr = "";
            if (M4AutoDictionary[4] == -1)
            {
                g.DrawRectangle(pBlack, r);
                g.FillRectangle(bBlack, r);
                statusStr = "sin conexión";
            }
            if (M4AutoDictionary[4] == 0)
            {
                g.DrawRectangle(pRed, r);
                g.FillRectangle(bRed, r);
                statusStr = "emergencia";
            }
            if (M4AutoDictionary[4] == 1)
            {
                g.DrawRectangle(pc1, r);
                g.FillRectangle(bc1, r);
                statusStr = "automatico";
            }
            if (M4AutoDictionary[4] == 2)
            {
                g.DrawRectangle(pOrange, r);
                g.FillRectangle(bOrange, r);
                statusStr = "manual";
            }
            if (M4AutoDictionary[4] == 3)
            {
                g.DrawRectangle(pc2, r);
                g.FillRectangle(bc2, r);
                statusStr = "en ciclo";
            }
            if (M4AutoDictionary[4] == 4)
            {
                g.DrawRectangle(pDarkOrange, r);
                g.FillRectangle(bDarkOrange, r);
                statusStr = "en alarma";
            }
            g.DrawString(statusStr, new Font("Verdana", 10), bText, new Point(10, -5));
            //groupBoxM4.Invalidate();
        }

        private void groupBoxM5_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush bText = new SolidBrush(Color.Black);
            g.TranslateTransform(10, 20);
            //            g.DrawString((M5AutoDictionary[1] == 0) ? "par. receta OFF" : "par. receta ON", new Font("Verdana", 10), bText, new Point(0, 0));
            //            g.TranslateTransform(0, 30);
            //cycle counter
            g.DrawString("ciclos " + M5AutoDictionary[2], new Font("Verdana", 10), bText, new Point(0, 0));
            g.TranslateTransform(0, 30);
            float value = float.Parse((M5AutoDictionary[3] / 100.0f).ToString());
            g.DrawString("tiempo (s) " + value.ToString(), new Font("Verdana", 10), bText, new Point(0, 0));
            g.TranslateTransform(0, 30);
            value = float.Parse((M5AutoDictionary[5] / 100.0f).ToString());
            g.DrawString("tiempo (s) " + value.ToString(), new Font("Verdana", 10), bText, new Point(0, 0));

            g.TranslateTransform(140, -54);
            Rectangle r = new Rectangle(new Point(0, 0), new Size(6, 6));

            string statusStr = "";
            if (M5AutoDictionary[4] == -1)
            {
                g.DrawRectangle(pBlack, r);
                g.FillRectangle(bBlack, r);
                statusStr = "sin conexión";
            }
            if (M5AutoDictionary[4] == 0)
            {
                g.DrawRectangle(pRed, r);
                g.FillRectangle(bRed, r);
                statusStr = "emergencia";
            }
            if (M5AutoDictionary[4] == 1)
            {
                g.DrawRectangle(pc1, r);
                g.FillRectangle(bc1, r);
                statusStr = "automatico";
            }
            if (M5AutoDictionary[4] == 2)
            {
                g.DrawRectangle(pOrange, r);
                g.FillRectangle(bOrange, r);
                statusStr = "manual";
            }
            if (M5AutoDictionary[4] == 3)
            {
                g.DrawRectangle(pc2, r);
                g.FillRectangle(bc2, r);
                statusStr = "en ciclo";
            }
            if (M5AutoDictionary[4] == 4)
            {
                g.DrawRectangle(pDarkOrange, r);
                g.FillRectangle(bDarkOrange, r);
                statusStr = "en alarma";
            }
            g.DrawString(statusStr, new Font("Verdana", 10), bText, new Point(10, -5));
            //groupBoxM5.Invalidate();
        }

        private void tabPageT0_2_1_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = e.Graphics;
            //Pen pExt = new Pen(Color.LightGray, 5);
            //Brush pText = new SolidBrush(Color.Black);
            //Rectangle rM1 = new Rectangle(new Point(20, 80), new Size(260, 200));
            //g.DrawRectangle(pExt, rM1);
            //g.DrawString("refiladora", new Font("Verdana", 10), pText, new Point(20, 60));

            //Rectangle rM2 = new Rectangle(new Point(300, 80), new Size(200, 200));
            //g.DrawString("tampografia intierna", new Font("Verdana", 10), pText, new Point(300, 60));
            //g.DrawRectangle(pExt, rM2);

            //Rectangle rM3 = new Rectangle(new Point(540, 80), new Size(200, 200));
            //g.DrawString("tampografia extierna", new Font("Verdana", 10), pText, new Point(540, 60));
            //g.DrawRectangle(pExt, rM3);


            //tabPageT0_2_1.Invalidate();
        }

        private void groupBoxM6_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush bText = new SolidBrush(Color.Black);
            g.TranslateTransform(10, 20);
            g.DrawString((M6AutoDictionary[1] == 0) ? "par. receta OFF" : "par. receta ON", new Font("Verdana", 10), bText, new Point(0, 0));

            g.TranslateTransform(140, -54);
            Rectangle r = new Rectangle(new Point(0, 0), new Size(6, 6));

            string statusStr = "";
            if (M6AutoDictionary[4] == -1)
            {
                g.DrawRectangle(pBlack, r);
                g.FillRectangle(bBlack, r);
                statusStr = "sin conexión";
            }
            if (M6AutoDictionary[4] == 0)
            {
                g.DrawRectangle(pRed, r);
                g.FillRectangle(bRed, r);
                statusStr = "emergencia";
            }
            if (M6AutoDictionary[4] == 1)
            {
                g.DrawRectangle(pc1, r);
                g.FillRectangle(bc1, r);
                statusStr = "automatico";
            }
            if (M6AutoDictionary[4] == 2)
            {
                g.DrawRectangle(pOrange, r);
                g.FillRectangle(bOrange, r);
                statusStr = "manual";
            }
            if (M6AutoDictionary[4] == 3)
            {
                g.DrawRectangle(pc2, r);
                g.FillRectangle(bc2, r);
                statusStr = "en ciclo";
            }
            if (M6AutoDictionary[4] == 4)
            {
                g.DrawRectangle(pDarkOrange, r);
                g.FillRectangle(bDarkOrange, r);
                statusStr = "en alarma";
            }
            g.DrawString(statusStr, new Font("Verdana", 10), bText, new Point(10, -5));
            //groupBoxM6.Invalidate();
        }

        private void groupBoxM3_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush bText = new SolidBrush(Color.Black);

            g.DrawString((M3AutoDictionary[1] == 0) ? "par. receta OFF" : "par. receta ON", new Font("Verdana", 10), bText, new Point(10, 90));
            //cycle counter
            g.DrawString("ciclos " + M3AutoDictionary[2], new Font("Verdana", 10), bText, new Point(10, 110));
            //cycle time 1
            g.DrawString("tiempo (s)", new Font("Verdana", 10), bText, new Point(195, 12));
            float value = float.Parse((M3AutoDictionary[3] / 100.0f).ToString());
            g.DrawString(value.ToString(), new Font("Verdana", 10), bText, new Point(195, 30));
            value = float.Parse((M3AutoDictionary[5] / 100.0f).ToString());
            g.DrawString(value.ToString(), new Font("Verdana", 10), bText, new Point(195, 60));

            Rectangle r = new Rectangle(new Point(165, 100), new Size(6, 6));

            string statusStr = "";
            if (M3AutoDictionary[4] == -1)
            {
                g.DrawRectangle(pBlack, r);
                g.FillRectangle(bBlack, r);
                statusStr = "sin conexión";
            }
            if (M3AutoDictionary[4] == 0)
            {
                g.DrawRectangle(pRed, r);
                g.FillRectangle(bRed, r);
                statusStr = "emergencia";
            }
            if (M3AutoDictionary[4] == 1)
            {
                g.DrawRectangle(pc1, r);
                g.FillRectangle(bc1, r);
                statusStr = "automatico";
            }
            if (M3AutoDictionary[4] == 2)
            {
                g.DrawRectangle(pOrange, r);
                g.FillRectangle(bOrange, r);
                statusStr = "manual";
            }
            if (M3AutoDictionary[4] == 3)
            {
                g.DrawRectangle(pc2, r);
                g.FillRectangle(bc2, r);
                statusStr = "en ciclo";
            }
            if (M3AutoDictionary[4] == 4)
            {
                g.DrawRectangle(pDarkOrange, r);
                g.FillRectangle(bDarkOrange, r);
                statusStr = "en alarma";
            }
            g.DrawString(statusStr, new Font("Verdana", 10), bText, new Point(175, 95));
            //groupBoxM3.Invalidate();
        }

        private async void numericUpDownM2ExitBeltTimer_ValueChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2AutoTimerExitBelt";

                var sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownM2ExitBeltTimer.Value.ToString()));

                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void numericUpDownM3TimerExitBelt_ValueChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3AutoTimerExitBelt";

                var sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownM3TimerExitBelt.Value.ToString()));

                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }



        private async void buttonV1Up_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1PosV1Up";

                var sendResult = await ccService.Send(keyToSend, true);

                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonV1Down_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1PosV1Down";

                var sendResult = await ccService.Send(keyToSend, true);

                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonV2up_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2PosV2Up";

                var sendResult = await ccService.Send(keyToSend, true);

                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonV2down_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2PosV2Down";

                var sendResult = await ccService.Send(keyToSend, true);

                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private void radioButtonMRecipeTopSel2_CheckedChanged(object sender, EventArgs e)
        {
            textBoxM4TopLineRecipe.Enabled = (radioButtonMRecipeTopSel2.Checked) ? true : false;
        }

        private void radioButtonMRecipeMedioSel1_CheckedChanged(object sender, EventArgs e)
        {
            textBoxM4MedioLineRecipe.Enabled = (radioButtonMRecipeMedioSel1.Checked) ? false : true;
            if (radioButtonMRecipeMedioSel1.Checked) textBoxM4MedioLineRecipe.Text = "___________________";
        }

        private void radioButtonMRecipeMedioSel2_CheckedChanged(object sender, EventArgs e)
        {
            textBoxM4MedioLineRecipe.Enabled = (radioButtonMRecipeMedioSel2.Checked) ? true : false;
        }

        private void radioButtonMRecipeBottomSel1_CheckedChanged(object sender, EventArgs e)
        {
            textBoxM4BottomLineRecipe.Enabled = (radioButtonMRecipeBottomSel1.Checked) ? false : true;
            if (radioButtonMRecipeBottomSel1.Checked) textBoxM4MedioLineRecipe.Text = "___________________";
        }

        private void radioButtonMRecipeBottomSel2_CheckedChanged(object sender, EventArgs e)
        {
            textBoxM4BottomLineRecipe.Enabled = (radioButtonMRecipeBottomSel2.Checked) ? true : false;
        }

        private void radioButtonM4Sel1Top_CheckedChanged(object sender, EventArgs e)
        {
            textBoxLaserLine1New.Enabled = (radioButtonM4Sel1Top.Checked) ? false : true;
            if (radioButtonM4Sel1Top.Checked) textBoxLaserLine1New.Text = "___________________";
        }

        private void radioButtonM4Sel2Top_CheckedChanged(object sender, EventArgs e)
        {
            textBoxLaserLine1New.Enabled = (radioButtonM4Sel2Top.Checked) ? true : false;
        }

        private void radioButtonM4Sel1Medio_CheckedChanged(object sender, EventArgs e)
        {
            textBoxLaserLine12New.Enabled = (radioButtonM4Sel1Medio.Checked) ? false : true;
            if (radioButtonM4Sel1Medio.Checked) textBoxLaserLine12New.Text = "___________________";
        }

        private void radioButtonM4Sel2Medio_CheckedChanged(object sender, EventArgs e)
        {
            textBoxLaserLine12New.Enabled = (radioButtonM4Sel2Medio.Checked) ? true : false;
        }

        private void radioButtonM4Sel1Bottom_CheckedChanged(object sender, EventArgs e)
        {
            textBoxLaserLine2New.Enabled = (radioButtonM4Sel1Bottom.Checked) ? false : true;
            if (radioButtonM4Sel1Bottom.Checked) textBoxLaserLine2New.Text = "___________________";
        }

        private void radioButtonM4Sel2Bottom_CheckedChanged(object sender, EventArgs e)
        {
            textBoxLaserLine2New.Enabled = (radioButtonM4Sel2Bottom.Checked) ? true : false;
        }

        private async void comboBoxInj_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected == false) return;
            if (comboBoxAutoPrgName.Text == "") return;
            M4PrgName = comboBoxAutoPrgName.Text;
            //string modelName = prgName.Substring(2, 4);
            string prgName = comboBoxAutoPrgName.Text;

            string modelName = prgName.Substring(2, 4);
            //get data from DB
            MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

            if ((recs.Error == 0) & (recs.Result.Count != 0))
            {
                //send recipe
                string keyValue = "pcM4Param1";
                var sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m4_param1.ToString()));
                //labelM4Param1Value.Text = recs.Result[0].m4_param1.ToString();
                M4AutoDictionary[1] = short.Parse(recs.Result[0].m4_param1.ToString());
                string keyToSend = "pcM4ProgramName";

                var readResult = await ccService.Send(keyToSend, "");
                if (readResult.OpcResult)
                {

                }
                else
                {

                }

                keyToSend = "pcM4ProgramName";

                readResult = await ccService.Send(keyToSend, M4PrgName);
                if (readResult.OpcResult)
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padLaser, "programa enviado exitosamente " + M4PrgName);
                }
                else
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padLaser, "error en enviar programa " + readResult.NodeString);
                }

                string top = "";
                string medium = "";
                string bottom = "";

                //selection code
                if (recs.Result[0].m4_param2.Substring(0, 1).Equals("0"))
                {
                    top = recs.Result[0].m4_param3.ToString();
                }
                else top = DateTime.Now.ToString("dd-MM-yyyy");

                if (recs.Result[0].m4_param2.Substring(1, 1).Equals("0"))
                {
                    medium = recs.Result[0].m4_param4.ToString();
                }
                else medium = DateTime.Now.ToString("dd-MM-yyyy");

                if (recs.Result[0].m4_param2.Substring(2, 1).Equals("0"))
                {
                    bottom = recs.Result[0].m4_param5.ToString();
                }
                else bottom = DateTime.Now.ToString("dd-MM-yyyy");
                string shift = comboBoxShift.Text;
                //sned to padlaser
                WritePadLaserRecipe(top, Properties.Settings.Default.PadLaserFilePathTop);
                WritePadLaserRecipe(medium, Properties.Settings.Default.PadLaserFilePathMedium);
                WritePadLaserRecipe(comboBoxInj.Text + " " + bottom + " " + shift, Properties.Settings.Default.PadLaserFilePathBottom);

                groupBoxM4.Text = "laser " + M4PrgName;
            }
            else
            {
                //db error manage
            }

        }

        private async void comboBoxShift_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected == false) return;
            if (comboBoxAutoPrgName.Text == "") return;
            M4PrgName = comboBoxAutoPrgName.Text;
            //string modelName = prgName.Substring(2, 4);
            string prgName = comboBoxAutoPrgName.Text;

            string modelName = prgName.Substring(2, 4);
            //get data from DB
            MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

            if ((recs.Error == 0) & (recs.Result.Count != 0))
            {
                //send recipe
                string keyValue = "pcM4Param1";
                var sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m4_param1.ToString()));
                //labelM4Param1Value.Text = recs.Result[0].m4_param1.ToString();
                M4AutoDictionary[1] = short.Parse(recs.Result[0].m4_param1.ToString());
                string keyToSend = "pcM4ProgramName";

                var readResult = await ccService.Send(keyToSend, "");
                if (readResult.OpcResult)
                {

                }
                else
                {

                }

                keyToSend = "pcM4ProgramName";

                readResult = await ccService.Send(keyToSend, M4PrgName);
                if (readResult.OpcResult)
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padLaser, "programa enviado exitosamente " + M4PrgName);
                }
                else
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padLaser, "error en enviar programa " + readResult.NodeString);
                }

                string top = "";
                string medium = "";
                string bottom = "";

                //selection code
                if (recs.Result[0].m4_param2.Substring(0, 1).Equals("0"))
                {
                    top = recs.Result[0].m4_param3.ToString();
                }
                else top = DateTime.Now.ToString("dd-MM-yyyy");

                if (recs.Result[0].m4_param2.Substring(1, 1).Equals("0"))
                {
                    medium = recs.Result[0].m4_param4.ToString();
                }
                else medium = DateTime.Now.ToString("dd-MM-yyyy");

                if (recs.Result[0].m4_param2.Substring(2, 1).Equals("0"))
                {
                    bottom = recs.Result[0].m4_param5.ToString();
                }
                else bottom = DateTime.Now.ToString("dd-MM-yyyy");
                string shift = comboBoxShift.Text;
                //sned to padlaser
                WritePadLaserRecipe(top, Properties.Settings.Default.PadLaserFilePathTop);
                WritePadLaserRecipe(medium, Properties.Settings.Default.PadLaserFilePathMedium);
                WritePadLaserRecipe(comboBoxInj.Text + " " + bottom + " " + shift, Properties.Settings.Default.PadLaserFilePathBottom);

                groupBoxM4.Text = "laser " + M4PrgName;
            }
            else
            {
                //db error manage
            }


        }

        private void InitializeLanguageMenu()
        {
            languageComboBox.DataSource = new[]
            {
                new { Code = "en", DisplayName = "English" },
                new { Code = "it", DisplayName = "Italian" },
                new { Code = "es", DisplayName = "Spanish" }
            };
            languageComboBox.DisplayMember = "DisplayName";
            languageComboBox.ValueMember = "Code";

            languageComboBox2.DataSource = new[]
            {
                new { Code = "en", DisplayName = "English" },
                new { Code = "it", DisplayName = "Italian" },
                new { Code = "es", DisplayName = "Spanish" }
            };
            languageComboBox2.DisplayMember = "DisplayName";
            languageComboBox2.ValueMember = "Code";
        }

        public class Language
        {
            public string Code { get; set; }
            public string DisplayName { get; set; }
        }

        private void comboBoxLanguageName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (languageComboBox.SelectedItem != null)
            {
                // Get selected language code
                var selectedItem = languageComboBox.SelectedItem;
                var codeProperty = selectedItem.GetType().GetProperty("Code");
                if (codeProperty != null)
                {
                    currentLanguageCode = codeProperty.GetValue(selectedItem)?.ToString() ?? "en";

                    // Save the language preference
                    Properties.Settings.Default.SelectedLanguage = currentLanguageCode;
                    Properties.Settings.Default.Save();

                    // Simply refresh/repaint the tab to show new language
                    tabPageT1_3.Invalidate();
                }
            }
        }

        private void LoadSavedLanguage()
        {
            try
            {
                string savedLanguage = Properties.Settings.Default.SelectedLanguage;
                if (string.IsNullOrEmpty(savedLanguage))
                {
                    savedLanguage = "en"; // Default
                }

                currentLanguageCode = savedLanguage;

                // Set combo box to saved language
                for (int i = 0; i < languageComboBox.Items.Count; i++)
                {
                    var item = languageComboBox.Items[i];
                    var item2 = languageComboBox2.Items[i];
                    var codeProperty = item.GetType().GetProperty("Code");
                    var codeProperty2 = item2.GetType().GetProperty("Code");

                    if (codeProperty?.GetValue(item)?.ToString() == savedLanguage)
                    {
                        languageComboBox.SelectedIndex = i;
                        break;
                    }

                    if (codeProperty2?.GetValue(item2)?.ToString() == savedLanguage)
                    {
                        languageComboBox2.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading saved language: {ex.Message}");
                currentLanguageCode = "en";
            }
        }

        private void comboBoxLanguageName2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (languageComboBox2.SelectedItem != null)
            {
                // Get selected language code
                var selectedItem = languageComboBox2.SelectedItem;
                var codeProperty = selectedItem.GetType().GetProperty("Code");
                if (codeProperty != null)
                {
                    currentLanguageCode = codeProperty.GetValue(selectedItem)?.ToString() ?? "en";

                    // Save the language preference
                    Properties.Settings.Default.SelectedLanguage = currentLanguageCode;
                    Properties.Settings.Default.Save();

                    // Simply refresh/repaint the tab to show new language
                    tabPageT1_4.Invalidate();
                }
            }
        }
    }
}
