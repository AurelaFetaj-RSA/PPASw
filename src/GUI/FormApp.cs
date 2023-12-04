using FutureServerCore;
using FutureServerCore.Core;
using Robot;
using RSACommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using WebApi;
using RSACommon.Logger;
using RSACommon.WebApiDefinitions;
using LidorSystems.IntegralUI.Containers;
using System.IO;
using System.Web.UI.WebControls;
using log4net;
using System.Threading;
using RSACommon.GraphicsForm;
using GUI.Properties;
using RSACommon.Event;
using Newtonsoft.Json.Linq;
using System.Timers;
using Opc.UaFx;
using OpcCustom;
using RSACommon.Service;
using RSACommon.Configuration;
using RSACommon.ProgramParser;
using RSACommon.Points;
using RSAPoints.ConcretePoints;
using xDialog;
using PPAUtils;
using MySql.Data;
using MySql.Data.MySqlClient;

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
        //create static instance of I7O configurator (singleton)
        public static Configurator ioConfigurator = new Configurator();
        //create static instance of I7O configurator (singleton)
        public static Configurator alarmsConfigurator = new Configurator();

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
        public static bool restartApp = false;
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

            InitGUI();

            _splashScreen?.WriteOnTextboxAsync($"Set the GUI");
            //StartDiagnosticGUI();
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

            RefreshModelNameComboBox();

            #region(* init tabControlMain *)
            tabPageT0.Text = "";
            tabPageT1.Text = "";
            tabPageT2.Text = "";
            tabPageT3.Text = "";
            tabPageT4.Text = "";
            tabPageT5.Text = "";
            tabPageT6.Text = "";
            tabPageT7.Text = "";
            tabControlMain.SelectedPage = tabPageT0;
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


            guiConfigurator.LoadFromFile("guiconfig.xml", Configurator.FileType.Xml);
            InitGUISettings();
            ioConfigurator.LoadFromFile("ioconfig.xml", Configurator.FileType.Xml);
            InitM1IOSettings();
            InitM2IOSettings();
            InitM3IOSettings();
            InitM5IOSettings();
            alarmsConfigurator.LoadFromFile("alarmsconfig.xml", Configurator.FileType.Xml);
            InitM1AlarmsSettings();
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

        private void tabControlMain_SelectedPageChanged(object sender, LidorSystems.IntegralUI.ObjectEventArgs e)
        {
            if (this.tabControlMain.SelectedPage != null)
            {
                LidorSystems.IntegralUI.Containers.TabPage nextPage = null;
                this.tabControlMain.SelectedPage.TabShape = TabShape.Rectangular;
                this.tabControlMain.SelectedPage.TabStripPlacement = TabStripPlacement.Left;
                LidorSystems.IntegralUI.Containers.TabPage parentPage = this.tabControlMain.SelectedPage;

                if (parentPage.Pages.Count != 0) nextPage = parentPage.Pages[0];
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
                
                if (parentPage.Index == 7)
                {
                    //ask user to close application
                    DialogResult res = xDialog.MsgBox.Show("Are you sure you want to exit from application?", "PBoot", xDialog.MsgBox.Buttons.YesNo);
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
            guiConfigurator.AddValue("T0", "M1PRGNAME", comboBoxM1PrgName.Text, true);
            guiConfigurator.AddValue("T0", "M2PRGNAME", comboBoxM2PrgName.Text, true);
            guiConfigurator.AddValue("T0", "M3PRGNAME1", comboBoxM3PrgName_st1.Text, true);
            guiConfigurator.AddValue("T0", "M3PRGNAME2", comboBoxM3PrgName_st2.Text, true);
            guiConfigurator.AddValue("T0", "M4PRGNAME", comboBoxM4PrgName.Text, true);

            guiConfigurator.AddValue("T0", "M1INC", (checkBoxM1Inclusion.CheckState == CheckState.Checked)?"1":"0", true);
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

            guiConfigurator.Save("guiconfig.xml", Configurator.FileType.Xml);
        }

        private async void InitGUISettings()
        {
            //save configuration file plconfig.xml
            comboBoxM1PrgName.Text = guiConfigurator.GetValue("T0", "M1PRGNAME", "");
            M1PrgName = comboBoxM1PrgName.Text;
            comboBoxM2PrgName.Text = guiConfigurator.GetValue("T0", "M2PRGNAME", "");
            M2PrgName = comboBoxM2PrgName.Text;
            comboBoxM3PrgName_st1.Text = guiConfigurator.GetValue("T0", "M3PRGNAME1", "");
            M3PrgName1 = comboBoxM3PrgName_st1.Text;
            comboBoxM3PrgName_st2.Text = guiConfigurator.GetValue("T0", "M3PRGNAME2", "");
            M3PrgName2 = comboBoxM3PrgName_st2.Text;
            comboBoxM4PrgName.Text = guiConfigurator.GetValue("T0", "M4PRGNAME", "");
            M4PrgName = comboBoxM4PrgName.Text;
            checkBoxM1Inclusion.CheckState = (guiConfigurator.GetValue("T0", "M1INC", "") == "1")?CheckState.Checked:CheckState.Unchecked;

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
            RestartRequestFromM1();
            RestartRequestFromM2();
            RestartRequestFromM3();
            RestartRequestFromM4();
            RestartRequestFromM5();
            RestartRequestFromM6();

            string keyToSend = null;
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
            numericUpDownM6OnPercentage.Value = short.Parse(guiConfigurator.GetValue("T0", "M6PERCENTAGE", ""));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM6ONPercentage";

                sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM6OnPercentage.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }        

            comboBoxT0RecipeName.Text = guiConfigurator.GetValue("T0", "RECIPENAME", "");
            comboBoxM1TeachProgramList.Text = guiConfigurator.GetValue("T1_1", "PRGNAME", "");
            comboBoxM1TeachRecipeName.Text = guiConfigurator.GetValue("T1_1", "RECIPENAME", "");

            numericUpDownM1JogSpeed.Value = Convert.ToDecimal(guiConfigurator.GetValue("T1_1", "JOGSPEED", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM1JogSpeed";

                sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM1JogSpeed.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }

            
            numericUpDownM1ManualQuote.Value = Convert.ToDecimal(guiConfigurator.GetValue("T1_1", "MANUALQUOTE", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM1ManualQuote";
                sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownM1ManualQuote.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }

            numericUpDownM1ManualSpeed.Value = Convert.ToDecimal(guiConfigurator.GetValue("T1_1", "MANUALSPEED", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM1ManualSpeed";
                sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownM1ManualSpeed.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
            comboBoxM1TestProgramList.Text = guiConfigurator.GetValue("T1_2", "PRGNAME", "");
            comboBoxM1TestRecipeName.Text = guiConfigurator.GetValue("T1_2", "RECIPENAME", "");


            comboBoxM2TeachProgramList.Text = guiConfigurator.GetValue("T2_1", "PRGNAME", "");
            comboBoxM2TeachRecipeName.Text = guiConfigurator.GetValue("T2_1", "RECIPENAME", "");
            numericUpDownM2JogSpeed.Value = Convert.ToDecimal(guiConfigurator.GetValue("T2_1", "JOGSPEED", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM2JogSpeed";

                sendResult = await ccService.Send(keyToSend, Convert.ToDecimal(numericUpDownM2JogSpeed.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }

            numericUpDownM2ManualQuote.Value = Convert.ToDecimal(guiConfigurator.GetValue("T2_1", "MANUALQUOTE", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM2ManualQuote";
                sendResult = await ccService.Send(keyToSend, Convert.ToDecimal(numericUpDownM2ManualQuote.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
            numericUpDownM2ManualSpeed.Value = Convert.ToDecimal(guiConfigurator.GetValue("T2_1", "MANUALSPEED", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM2ManualSpeed";
                sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownM2ManualSpeed.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
            comboBoxM2TestProgramList.Text = guiConfigurator.GetValue("T2_2", "PRGNAME", "");
            comboBoxM2TestRecipeName.Text = guiConfigurator.GetValue("T2_2", "RECIPENAME", "");

            comboBoxM3TeachProgramList.Text = guiConfigurator.GetValue("T3_1", "PRGNAME", "");
            comboBoxM3TeachRecipeName.Text = guiConfigurator.GetValue("T3_1", "RECIPENAME", "");
            numericUpDownM3JogSpeed.Value = Convert.ToDecimal(guiConfigurator.GetValue("T3_1", "JOGSPEED", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM3JogSpeed";

                sendResult = await ccService.Send(keyToSend, Convert.ToDecimal(numericUpDownM3JogSpeed.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }

            numericUpDownM3ManualQuote.Value = Convert.ToDecimal(guiConfigurator.GetValue("T3_1", "MANUALQUOTE", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM3ManualQuote";
                sendResult = await ccService.Send(keyToSend, Convert.ToDecimal(numericUpDownM3ManualQuote.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
            numericUpDownM3ManualSpeed.Value = Convert.ToDecimal(guiConfigurator.GetValue("T3_1", "MANUALSPEED", "10"));
            if (ccService.ClientIsConnected)
            {
                keyToSend = "pcM3ManualSpeed";
                sendResult = await ccService.Send(keyToSend, Convert.ToDecimal(numericUpDownM3ManualSpeed.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
            comboBoxM3TestProgramList.Text = guiConfigurator.GetValue("T3_2", "PRGNAME", "");
            comboBoxM3TestRecipeName.Text = guiConfigurator.GetValue("T3_2", "RECIPENAME", "");
        }

        private void InitM1AlarmsSettings()
        {
            try
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Rows.Add(8);

                dataGridView1.Rows[0].Cells[0].Value = alarmsConfigurator.GetValue("M1", "A1", "");
                dataGridView1.Rows[1].Cells[0].Value = alarmsConfigurator.GetValue("M1", "A2", "");
                dataGridView1.Rows[2].Cells[0].Value = alarmsConfigurator.GetValue("M1", "A3", "");
                dataGridView1.Rows[3].Cells[0].Value = alarmsConfigurator.GetValue("M1", "A4", "");
                dataGridView1.Rows[4].Cells[0].Value = alarmsConfigurator.GetValue("M1", "A5", "");
                dataGridView1.Rows[5].Cells[0].Value = alarmsConfigurator.GetValue("M1", "A6", "");
                dataGridView1.Rows[6].Cells[0].Value = alarmsConfigurator.GetValue("M1", "A7", "");
                dataGridView1.Rows[7].Cells[0].Value = alarmsConfigurator.GetValue("M1", "A8", "");
                //dataGridViewM1AlarmsTimeout.Rows[0].Cells[1]. = ;

                //dataGridViewM1AlarmsTimeout.Rows[0].Cells[1].Value = "1";
                //dataGridViewM1AlarmsTimeout.Rows[1].Cells[1].Value = "1";
                //dataGridViewM1AlarmsTimeout.Rows[2].Cells[1].Value = "1";
                //dataGridViewM1AlarmsTimeout.Rows[3].Cells[1].Value = "1";
                //dataGridViewM1AlarmsTimeout.Rows[4].Cells[1].Value = "1";
                //dataGridViewM1AlarmsTimeout.Rows[5].Cells[1].Value = "1";
                //dataGridViewM1AlarmsTimeout.Rows[6].Cells[1].Value = "1";
                //dataGridViewM1AlarmsTimeout.Rows[7].Cells[1].Value = "1";
            }
            catch(Exception Ex)
            {

            }

        }

            private void InitM1IOSettings()
        {
            //digital input
            lbLed1001M1.Label = ioConfigurator.GetValue("M1_INPUT", "1001", "");
            lbLed1002M1.Label = ioConfigurator.GetValue("M1_INPUT", "1002", "");
            lbLed1003M1.Label = ioConfigurator.GetValue("M1_INPUT", "1003", "");
            lbLed1004M1.Label = ioConfigurator.GetValue("M1_INPUT", "1004", "");
            lbLed1005M1.Label = ioConfigurator.GetValue("M1_INPUT", "1005", "");
            lbLed1006M1.Label = ioConfigurator.GetValue("M1_INPUT", "1006", "");
            lbLed1007M1.Label = ioConfigurator.GetValue("M1_INPUT", "1007", "");
            lbLed1008M1.Label = ioConfigurator.GetValue("M1_INPUT", "1008", "");
            lbLed1009M1.Label = ioConfigurator.GetValue("M1_INPUT", "1009", "");
            lbLed1010M1.Label = ioConfigurator.GetValue("M1_INPUT", "1010", "");

            lbLed1011M1.Label = ioConfigurator.GetValue("M1_INPUT", "1011", "");
            lbLed1012M1.Label = ioConfigurator.GetValue("M1_INPUT", "1012", "");
            lbLed1013M1.Label = ioConfigurator.GetValue("M1_INPUT", "1013", "");
            lbLed1014M1.Label = ioConfigurator.GetValue("M1_INPUT", "1014", "");
            lbLed1015M1.Label = ioConfigurator.GetValue("M1_INPUT", "1015", "");
            lbLed1016M1.Label = ioConfigurator.GetValue("M1_INPUT", "1016", "");
            lbLed1017M1.Label = ioConfigurator.GetValue("M1_INPUT", "1017", "");
            lbLed1018M1.Label = ioConfigurator.GetValue("M1_INPUT", "1018", "");
            lbLed1019M1.Label = ioConfigurator.GetValue("M1_INPUT", "1019", "");
            lbLed1020M1.Label = ioConfigurator.GetValue("M1_INPUT", "1020", "");

            lbLed1021M1.Label = ioConfigurator.GetValue("M1_INPUT", "1021", "");
            lbLed1022M1.Label = ioConfigurator.GetValue("M1_INPUT", "1022", "");
            lbLed1023M1.Label = ioConfigurator.GetValue("M1_INPUT", "1023", "");
            lbLed1024M1.Label = ioConfigurator.GetValue("M1_INPUT", "1024", "");
            lbLed1025M1.Label = ioConfigurator.GetValue("M1_INPUT", "1025", "");
            lbLed1026M1.Label = ioConfigurator.GetValue("M1_INPUT", "1026", "");
            lbLed1027M1.Label = ioConfigurator.GetValue("M1_INPUT", "1027", "");
            lbLed1028M1.Label = ioConfigurator.GetValue("M1_INPUT", "1028", "");
            lbLed1029M1.Label = ioConfigurator.GetValue("M1_INPUT", "1029", "");
            lbLed1030M1.Label = ioConfigurator.GetValue("M1_INPUT", "1030", "");

            lbLed1031M1.Label = ioConfigurator.GetValue("M1_INPUT", "1031", "");
            lbLed1032M1.Label = ioConfigurator.GetValue("M1_INPUT", "1032", "");
            lbLed1033M1.Label = ioConfigurator.GetValue("M1_INPUT", "1033", "");
            lbLed1034M1.Label = ioConfigurator.GetValue("M1_INPUT", "1034", "");
            lbLed1035M1.Label = ioConfigurator.GetValue("M1_INPUT", "1035", "");
            lbLed1036M1.Label = ioConfigurator.GetValue("M1_INPUT", "1036", "");
            lbLed1037M1.Label = ioConfigurator.GetValue("M1_INPUT", "1037", "");
            lbLed1038M1.Label = ioConfigurator.GetValue("M1_INPUT", "1038", "");
            lbLed1039M1.Label = ioConfigurator.GetValue("M1_INPUT", "1039", "");
            lbLed1040M1.Label = ioConfigurator.GetValue("M1_INPUT", "1040", "");

            lbLed1041M1.Label = ioConfigurator.GetValue("M1_INPUT", "1041", "");
            lbLed1042M1.Label = ioConfigurator.GetValue("M1_INPUT", "1042", "");
            lbLed1043M1.Label = ioConfigurator.GetValue("M1_INPUT", "1043", "");
            lbLed1044M1.Label = ioConfigurator.GetValue("M1_INPUT", "1044", "");
            lbLed1045M1.Label = ioConfigurator.GetValue("M1_INPUT", "1045", "");
            lbLed1046M1.Label = ioConfigurator.GetValue("M1_INPUT", "1046", "");
            lbLed1047M1.Label = ioConfigurator.GetValue("M1_INPUT", "1047", "");
            lbLed1048M1.Label = ioConfigurator.GetValue("M1_INPUT", "1048", "");

            //digital output
            lbLed2001M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2001", "");
            lbLed2002M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2002", "");
            lbLed2003M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2003", "");
            lbLed2004M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2004", "");
            lbLed2005M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2005", "");
            lbLed2006M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2006", "");
            lbLed2007M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2007", "");
            lbLed2008M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2008", "");
            lbLed2009M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2009", "");
            lbLed2010M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2010", "");

            lbLed2011M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2011", "");
            lbLed2012M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2012", "");
            lbLed2013M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2013", "");
            lbLed2014M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2014", "");
            lbLed2015M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2015", "");
            lbLed2016M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2016", "");
            lbLed2017M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2017", "");
            lbLed2018M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2018", "");
            lbLed2019M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2019", "");
            lbLed2020M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2020", "");

            lbLed2021M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2021", "");
            lbLed2022M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2022", "");
            lbLed2023M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2023", "");
            lbLed2024M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2024", "");
            lbLed2025M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2025", "");
            lbLed2026M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2026", "");
            lbLed2027M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2027", "");
            lbLed2028M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2028", "");
            lbLed2029M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2029", "");
            lbLed2030M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2030", "");

            lbLed2031M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2031", "");
            lbLed2032M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2032", "");
            lbLed2033M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2033", "");
            lbLed2034M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2034", "");
            lbLed2035M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2035", "");
            lbLed2036M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2036", "");
            lbLed2037M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2037", "");
            lbLed2038M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2038", "");
            lbLed2039M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2039", "");
            lbLed2040M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2040", "");

            lbLed2041M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2041", "");
            lbLed2042M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2042", "");
            lbLed2043M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2043", "");
            lbLed2044M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2044", "");
            lbLed2045M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2045", "");
            lbLed2046M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2046", "");
            lbLed2047M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2047", "");
            lbLed2048M1.Label = ioConfigurator.GetValue("M1_OUTPUT", "2048", "");
        }

        private void InitM2IOSettings()
        {
            //digital input
            lbLed1001M2.Label = ioConfigurator.GetValue("M2_INPUT", "1001", "");
            lbLed1002M2.Label = ioConfigurator.GetValue("M2_INPUT", "1002", "");
            lbLed1003M2.Label = ioConfigurator.GetValue("M2_INPUT", "1003", "");
            lbLed1004M2.Label = ioConfigurator.GetValue("M2_INPUT", "1004", "");
            lbLed1005M2.Label = ioConfigurator.GetValue("M2_INPUT", "1005", "");
            lbLed1006M2.Label = ioConfigurator.GetValue("M2_INPUT", "1006", "");
            lbLed1007M2.Label = ioConfigurator.GetValue("M2_INPUT", "1007", "");
            lbLed1008M2.Label = ioConfigurator.GetValue("M2_INPUT", "1008", "");
            lbLed1009M2.Label = ioConfigurator.GetValue("M2_INPUT", "1009", "");
            lbLed1010M2.Label = ioConfigurator.GetValue("M2_INPUT", "1010", "");

            lbLed1011M2.Label = ioConfigurator.GetValue("M2_INPUT", "1011", "");
            lbLed1012M2.Label = ioConfigurator.GetValue("M2_INPUT", "1012", "");
            lbLed1013M2.Label = ioConfigurator.GetValue("M2_INPUT", "1013", "");
            lbLed1014M2.Label = ioConfigurator.GetValue("M2_INPUT", "1014", "");
            lbLed1015M2.Label = ioConfigurator.GetValue("M2_INPUT", "1015", "");
            lbLed1016M2.Label = ioConfigurator.GetValue("M2_INPUT", "1016", "");
            lbLed1017M2.Label = ioConfigurator.GetValue("M2_INPUT", "1017", "");
            lbLed1018M2.Label = ioConfigurator.GetValue("M2_INPUT", "1018", "");
            lbLed1019M2.Label = ioConfigurator.GetValue("M2_INPUT", "1019", "");
            lbLed1020M2.Label = ioConfigurator.GetValue("M2_INPUT", "1020", "");

            lbLed1021M2.Label = ioConfigurator.GetValue("M2_INPUT", "1021", "");
            lbLed1022M2.Label = ioConfigurator.GetValue("M2_INPUT", "1022", "");
            lbLed1023M2.Label = ioConfigurator.GetValue("M2_INPUT", "1023", "");
            lbLed1024M2.Label = ioConfigurator.GetValue("M2_INPUT", "1024", "");
            lbLed1025M2.Label = ioConfigurator.GetValue("M2_INPUT", "1025", "");
            lbLed1026M2.Label = ioConfigurator.GetValue("M2_INPUT", "1026", "");
            lbLed1027M2.Label = ioConfigurator.GetValue("M2_INPUT", "1027", "");
            lbLed1028M2.Label = ioConfigurator.GetValue("M2_INPUT", "1028", "");
            lbLed1029M2.Label = ioConfigurator.GetValue("M2_INPUT", "1029", "");
            lbLed1030M2.Label = ioConfigurator.GetValue("M2_INPUT", "1030", "");

            lbLed1031M2.Label = ioConfigurator.GetValue("M2_INPUT", "1031", "");
            lbLed1032M2.Label = ioConfigurator.GetValue("M2_INPUT", "1032", "");
            lbLed1033M2.Label = ioConfigurator.GetValue("M2_INPUT", "1033", "");
            lbLed1034M2.Label = ioConfigurator.GetValue("M2_INPUT", "1034", "");
            lbLed1035M2.Label = ioConfigurator.GetValue("M2_INPUT", "1035", "");
            lbLed1036M2.Label = ioConfigurator.GetValue("M2_INPUT", "1036", "");
            lbLed1037M2.Label = ioConfigurator.GetValue("M2_INPUT", "1037", "");
            lbLed1038M2.Label = ioConfigurator.GetValue("M2_INPUT", "1038", "");
            lbLed1039M2.Label = ioConfigurator.GetValue("M2_INPUT", "1039", "");
            lbLed1040M2.Label = ioConfigurator.GetValue("M2_INPUT", "1040", "");

            lbLed1041M2.Label = ioConfigurator.GetValue("M2_INPUT", "1041", "");
            lbLed1042M2.Label = ioConfigurator.GetValue("M2_INPUT", "1042", "");
            lbLed1043M2.Label = ioConfigurator.GetValue("M2_INPUT", "1043", "");
            lbLed1044M2.Label = ioConfigurator.GetValue("M2_INPUT", "1044", "");
            lbLed1045M2.Label = ioConfigurator.GetValue("M2_INPUT", "1045", "");
            lbLed1046M2.Label = ioConfigurator.GetValue("M2_INPUT", "1046", "");
            lbLed1047M2.Label = ioConfigurator.GetValue("M2_INPUT", "1047", "");
            lbLed1048M2.Label = ioConfigurator.GetValue("M2_INPUT", "1048", "");

            //digital output
            lbLed2001M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2001", "");
            lbLed2002M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2002", "");
            lbLed2003M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2003", "");
            lbLed2004M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2004", "");
            lbLed2005M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2005", "");
            lbLed2006M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2006", "");
            lbLed2007M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2007", "");
            lbLed2008M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2008", "");
            lbLed2009M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2009", "");
            lbLed2010M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2010", "");

            lbLed2011M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2011", "");
            lbLed2012M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2012", "");
            lbLed2013M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2013", "");
            lbLed2014M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2014", "");
            lbLed2015M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2015", "");
            lbLed2016M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2016", "");
            lbLed2017M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2017", "");
            lbLed2018M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2018", "");
            lbLed2019M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2019", "");
            lbLed2020M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2020", "");

            lbLed2021M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2021", "");
            lbLed2022M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2022", "");
            lbLed2023M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2023", "");
            lbLed2024M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2024", "");
            lbLed2025M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2025", "");
            lbLed2026M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2026", "");
            lbLed2027M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2027", "");
            lbLed2028M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2028", "");
            lbLed2028M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2029", "");
            lbLed2029M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2030", "");

            lbLed2030M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2031", "");
            lbLed2031M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2032", "");
            lbLed2032M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2033", "");
            lbLed2033M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2034", "");
            lbLed2034M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2035", "");
            lbLed2035M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2036", "");
            lbLed2036M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2037", "");
            lbLed2038M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2038", "");
            lbLed2039M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2039", "");
            lbLed2040M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2040", "");

            lbLed2041M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2041", "");
            lbLed2042M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2042", "");
            lbLed2043M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2043", "");
            lbLed2044M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2044", "");
            lbLed2045M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2045", "");
            lbLed2046M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2046", "");
            lbLed2047M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2047", "");
            lbLed2048M2.Label = ioConfigurator.GetValue("M2_OUTPUT", "2048", "");
        }

        private void InitM5IOSettings()
        {
            //digital input
            lbLed1001M5.Label = ioConfigurator.GetValue("M5_INPUT", "1001", "");
            lbLed1002M5.Label = ioConfigurator.GetValue("M5_INPUT", "1002", "");
            lbLed1003M5.Label = ioConfigurator.GetValue("M5_INPUT", "1003", "");
            lbLed1004M5.Label = ioConfigurator.GetValue("M5_INPUT", "1004", "");
            lbLed1005M5.Label = ioConfigurator.GetValue("M5_INPUT", "1005", "");
            lbLed1006M5.Label = ioConfigurator.GetValue("M5_INPUT", "1006", "");
            lbLed1007M5.Label = ioConfigurator.GetValue("M5_INPUT", "1007", "");
            lbLed1008M5.Label = ioConfigurator.GetValue("M5_INPUT", "1008", "");
            lbLed1009M5.Label = ioConfigurator.GetValue("M5_INPUT", "1009", "");
            lbLed1010M5.Label = ioConfigurator.GetValue("M5_INPUT", "1010", "");

            lbLed1011M5.Label = ioConfigurator.GetValue("M5_INPUT", "1011", "");
            lbLed1012M5.Label = ioConfigurator.GetValue("M5_INPUT", "1012", "");
            lbLed1013M5.Label = ioConfigurator.GetValue("M5_INPUT", "1013", "");
            lbLed1014M5.Label = ioConfigurator.GetValue("M5_INPUT", "1014", "");
            lbLed1015M5.Label = ioConfigurator.GetValue("M5_INPUT", "1015", "");
            lbLed1016M5.Label = ioConfigurator.GetValue("M5_INPUT", "1016", "");
            lbLed1017M5.Label = ioConfigurator.GetValue("M5_INPUT", "1017", "");
            lbLed1018M5.Label = ioConfigurator.GetValue("M5_INPUT", "1018", "");
            lbLed1019M5.Label = ioConfigurator.GetValue("M5_INPUT", "1019", "");
            lbLed1020M5.Label = ioConfigurator.GetValue("M5_INPUT", "1020", "");

            lbLed1021M5.Label = ioConfigurator.GetValue("M5_INPUT", "1021", "");
            lbLed1022M5.Label = ioConfigurator.GetValue("M5_INPUT", "1022", "");
            lbLed1023M5.Label = ioConfigurator.GetValue("M5_INPUT", "1023", "");
            lbLed1024M5.Label = ioConfigurator.GetValue("M5_INPUT", "1024", "");
            lbLed1025M5.Label = ioConfigurator.GetValue("M5_INPUT", "1025", "");
            lbLed1026M5.Label = ioConfigurator.GetValue("M5_INPUT", "1026", "");
            lbLed1027M5.Label = ioConfigurator.GetValue("M5_INPUT", "1027", "");
            lbLed1028M5.Label = ioConfigurator.GetValue("M5_INPUT", "1028", "");
            lbLed1029M5.Label = ioConfigurator.GetValue("M5_INPUT", "1029", "");
            lbLed1030M5.Label = ioConfigurator.GetValue("M5_INPUT", "1030", "");

            lbLed1031M5.Label = ioConfigurator.GetValue("M5_INPUT", "1031", "");
            lbLed1032M5.Label = ioConfigurator.GetValue("M5_INPUT", "1032", "");
            lbLed1033M5.Label = ioConfigurator.GetValue("M5_INPUT", "1033", "");
            lbLed1034M5.Label = ioConfigurator.GetValue("M5_INPUT", "1034", "");
            lbLed1035M5.Label = ioConfigurator.GetValue("M5_INPUT", "1035", "");
            lbLed1036M5.Label = ioConfigurator.GetValue("M5_INPUT", "1036", "");
            lbLed1037M5.Label = ioConfigurator.GetValue("M5_INPUT", "1037", "");
            lbLed1038M5.Label = ioConfigurator.GetValue("M5_INPUT", "1038", "");
            lbLed1039M5.Label = ioConfigurator.GetValue("M5_INPUT", "1039", "");
            lbLed1040M5.Label = ioConfigurator.GetValue("M5_INPUT", "1040", "");

            lbLed1041M5.Label = ioConfigurator.GetValue("M5_INPUT", "1041", "");
            lbLed1042M5.Label = ioConfigurator.GetValue("M5_INPUT", "1042", "");
            lbLed1043M5.Label = ioConfigurator.GetValue("M5_INPUT", "1043", "");
            lbLed1044M5.Label = ioConfigurator.GetValue("M5_INPUT", "1044", "");
            lbLed1045M5.Label = ioConfigurator.GetValue("M5_INPUT", "1045", "");
            lbLed1046M5.Label = ioConfigurator.GetValue("M5_INPUT", "1046", "");
            lbLed1047M5.Label = ioConfigurator.GetValue("M5_INPUT", "1047", "");
            lbLed1048M5.Label = ioConfigurator.GetValue("M5_INPUT", "1048", "");

            //digital output
            lbLed2001M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2001", "");
            lbLed2002M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2002", "");
            lbLed2003M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2003", "");
            lbLed2004M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2004", "");
            lbLed2005M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2005", "");
            lbLed2006M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2006", "");
            lbLed2007M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2007", "");
            lbLed2008M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2008", "");
            lbLed2009M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2009", "");
            lbLed2010M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2010", "");

            lbLed2011M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2011", "");
            lbLed2012M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2012", "");
            lbLed2013M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2013", "");
            lbLed2014M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2014", "");
            lbLed2015M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2015", "");
            lbLed2016M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2016", "");
            lbLed2017M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2017", "");
            lbLed2018M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2018", "");
            lbLed2019M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2019", "");
            lbLed2020M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2020", "");

            lbLed2021M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2021", "");
            lbLed2022M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2022", "");
            lbLed2023M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2023", "");
            lbLed2024M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2024", "");
            lbLed2025M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2025", "");
            lbLed2026M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2026", "");
            lbLed2027M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2027", "");
            lbLed2028M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2028", "");
            lbLed2028M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2029", "");
            lbLed2029M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2030", "");

            lbLed2030M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2031", "");
            lbLed2031M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2032", "");
            lbLed2032M5.Label = ioConfigurator.GetValue("M5_OUTPUT", "2033", "");            
        }

        private void InitM3IOSettings()
        {
            //digital input
            lbLed1001M3.Label = ioConfigurator.GetValue("M3_INPUT", "1001", "");
            lbLed1002M3.Label = ioConfigurator.GetValue("M3_INPUT", "1002", "");
            lbLed1003M3.Label = ioConfigurator.GetValue("M3_INPUT", "1003", "");
            lbLed1004M3.Label = ioConfigurator.GetValue("M3_INPUT", "1004", "");
            lbLed1005M3.Label = ioConfigurator.GetValue("M3_INPUT", "1005", "");
            lbLed1006M3.Label = ioConfigurator.GetValue("M3_INPUT", "1006", "");
            lbLed1007M3.Label = ioConfigurator.GetValue("M3_INPUT", "1007", "");
            lbLed1008M3.Label = ioConfigurator.GetValue("M3_INPUT", "1008", "");
            lbLed1009M3.Label = ioConfigurator.GetValue("M3_INPUT", "1009", "");
            lbLed1010M3.Label = ioConfigurator.GetValue("M3_INPUT", "1010", "");

            lbLed1011M3.Label = ioConfigurator.GetValue("M3_INPUT", "1011", "");
            lbLed1012M3.Label = ioConfigurator.GetValue("M3_INPUT", "1012", "");
            lbLed1013M3.Label = ioConfigurator.GetValue("M3_INPUT", "1013", "");
            lbLed1014M3.Label = ioConfigurator.GetValue("M3_INPUT", "1014", "");
            lbLed1015M3.Label = ioConfigurator.GetValue("M3_INPUT", "1015", "");
            lbLed1016M3.Label = ioConfigurator.GetValue("M3_INPUT", "1016", "");
            lbLed1017M3.Label = ioConfigurator.GetValue("M3_INPUT", "1017", "");
            lbLed1018M3.Label = ioConfigurator.GetValue("M3_INPUT", "1018", "");
            lbLed1019M3.Label = ioConfigurator.GetValue("M3_INPUT", "1019", "");
            lbLed1020M3.Label = ioConfigurator.GetValue("M3_INPUT", "1020", "");

            lbLed1021M3.Label = ioConfigurator.GetValue("M3_INPUT", "1021", "");
            lbLed1022M3.Label = ioConfigurator.GetValue("M3_INPUT", "1022", "");
            lbLed1023M3.Label = ioConfigurator.GetValue("M3_INPUT", "1023", "");
            lbLed1024M3.Label = ioConfigurator.GetValue("M3_INPUT", "1024", "");
            lbLed1025M3.Label = ioConfigurator.GetValue("M3_INPUT", "1025", "");
            lbLed1026M3.Label = ioConfigurator.GetValue("M3_INPUT", "1026", "");
            lbLed1027M3.Label = ioConfigurator.GetValue("M3_INPUT", "1027", "");
            lbLed1028M3.Label = ioConfigurator.GetValue("M3_INPUT", "1028", "");
            lbLed1029M3.Label = ioConfigurator.GetValue("M3_INPUT", "1029", "");
            lbLed1030M3.Label = ioConfigurator.GetValue("M3_INPUT", "1030", "");

            lbLed1031M3.Label = ioConfigurator.GetValue("M3_INPUT", "1031", "");
            lbLed1032M3.Label = ioConfigurator.GetValue("M3_INPUT", "1032", "");
            lbLed1033M3.Label = ioConfigurator.GetValue("M3_INPUT", "1033", "");
            lbLed1034M3.Label = ioConfigurator.GetValue("M3_INPUT", "1034", "");
            lbLed1035M3.Label = ioConfigurator.GetValue("M3_INPUT", "1035", "");
            lbLed1036M3.Label = ioConfigurator.GetValue("M3_INPUT", "1036", "");
            lbLed1037M3.Label = ioConfigurator.GetValue("M3_INPUT", "1037", "");
            lbLed1038M3.Label = ioConfigurator.GetValue("M3_INPUT", "1038", "");
            lbLed1039M3.Label = ioConfigurator.GetValue("M3_INPUT", "1039", "");
            lbLed1040M3.Label = ioConfigurator.GetValue("M3_INPUT", "1040", "");

            lbLed1041M3.Label = ioConfigurator.GetValue("M3_INPUT", "1041", "");
            lbLed1042M3.Label = ioConfigurator.GetValue("M3_INPUT", "1042", "");
            lbLed1043M3.Label = ioConfigurator.GetValue("M3_INPUT", "1043", "");
            lbLed1044M3.Label = ioConfigurator.GetValue("M3_INPUT", "1044", "");
            lbLed1045M3.Label = ioConfigurator.GetValue("M3_INPUT", "1045", "");
            lbLed1046M3.Label = ioConfigurator.GetValue("M3_INPUT", "1046", "");
            lbLed1047M3.Label = ioConfigurator.GetValue("M3_INPUT", "1047", "");
            lbLed1048M3.Label = ioConfigurator.GetValue("M3_INPUT", "1048", "");

            //digital output
            lbLed2001M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2001", "");
            lbLed2002M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2002", "");
            lbLed2003M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2003", "");
            lbLed2004M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2004", "");
            lbLed2005M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2005", "");
            lbLed2006M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2006", "");
            lbLed2007M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2007", "");
            lbLed2008M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2008", "");
            lbLed2009M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2009", "");
            lbLed2010M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2010", "");

            lbLed2011M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2011", "");
            lbLed2012M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2012", "");
            lbLed2013M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2013", "");
            lbLed2014M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2014", "");
            lbLed2015M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2015", "");
            lbLed2016M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2016", "");
            lbLed2017M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2017", "");
            lbLed2018M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2018", "");
            lbLed2019M2.Label = ioConfigurator.GetValue("M3_OUTPUT", "2019", "");
            lbLed2020M2.Label = ioConfigurator.GetValue("M3_OUTPUT", "2020", "");

            lbLed2021M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2021", "");
            lbLed2022M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2022", "");
            lbLed2023M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2023", "");
            lbLed2024M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2024", "");
            lbLed2025M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2025", "");
            lbLed2026M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2026", "");
            lbLed2027M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2027", "");
            lbLed2028M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2028", "");
            lbLed2028M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2029", "");
            lbLed2029M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2030", "");

            lbLed2030M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2031", "");
            lbLed2031M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2032", "");
            lbLed2032M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2033", "");
            lbLed2033M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2034", "");
            lbLed2034M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2035", "");
            lbLed2035M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2036", "");
            lbLed2036M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2037", "");
            lbLed2038M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2038", "");
            lbLed2039M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2039", "");
            lbLed2040M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2040", "");

            lbLed2041M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2041", "");
            lbLed2042M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2042", "");
            lbLed2043M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2043", "");
            lbLed2044M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2044", "");
            lbLed2045M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2045", "");
            lbLed2046M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2046", "");
            lbLed2047M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2047", "");
            lbLed2048M3.Label = ioConfigurator.GetValue("M3_OUTPUT", "2048", "");
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

        private async void RefreshModelNameComboBox()
        {
            #region (* refresh combobox model name list *)
            List<string> mList = new List<string>();
            //get model name list from DB
            MySqlResult<recipies> result = await mysqlService.DBTable[0].SelectAllAsync<recipies>();
            result.Result.ForEach(x => mList.Add(x.model_name));
            comboBoxMRecipeName.Items.Clear();
            comboBoxT0RecipeName.Items.Clear();
            comboBoxM1TeachRecipeName.Items.Clear();
            comboBoxM1TestRecipeName.Items.Clear();
            comboBoxM2TeachRecipeName.Items.Clear();
            comboBoxM2TestRecipeName.Items.Clear();
            comboBoxM3TeachRecipeName.Items.Clear();
            comboBoxM3TestRecipeName.Items.Clear();
            foreach (string modelName in mList)
            {
                toolStripComboBoxT0.Items.Add(modelName);
                toolStripComboBoxT0.SelectedIndex = 0;
                toolStripComboBoxT0.SelectedIndexChanged += toolStripComboBoxT0_SelectedIndexChanged;
                toolStripComboBoxT1_1.Items.Add(modelName);
                toolStripComboBoxT1_1.SelectedIndex = 0;
                toolStripComboBoxT1_1.SelectedIndexChanged += toolStripComboBoxT1_1_SelectedIndexChanged;
                toolStripComboBoxT1_2.Items.Add(modelName);
                toolStripComboBoxT1_2.SelectedIndex = 0;
                toolStripComboBoxT1_2.SelectedIndexChanged += toolStripComboBoxT1_2_SelectedIndexChanged;
                toolStripComboBoxT3_1.Items.Add(modelName);
                toolStripComboBoxT3_1.SelectedIndex = 0;
                toolStripComboBoxT3_1.SelectedIndexChanged += toolStripComboBoxT3_1_SelectedIndexChanged;
                toolStripComboBoxT3_2.Items.Add(modelName);
                toolStripComboBoxT3_2.SelectedIndex = 0;
                toolStripComboBoxT3_2.SelectedIndexChanged += toolStripComboBoxT3_2_SelectedIndexChanged;
                toolStripComboBoxT4_1.Items.Add(modelName);
                toolStripComboBoxT4_1.SelectedIndex = 0;
                toolStripComboBoxT4_1.SelectedIndexChanged += toolStripComboBoxT4_1_SelectedIndexChanged;
                toolStripComboBoxT4_2.Items.Add(modelName);
                toolStripComboBoxT4_2.SelectedIndex = 0;
                toolStripComboBoxT4_2.SelectedIndexChanged += toolStripComboBoxT4_2_SelectedIndexChanged;
                comboBoxMRecipeName.Items.Add(modelName);
                comboBoxMRecipeName.SelectedIndex = 0;
                comboBoxT0RecipeName.Items.Add(modelName);
                comboBoxT0RecipeName.SelectedIndex = 0;
                comboBoxM1TeachRecipeName.Items.Add(modelName);
                comboBoxM1TeachRecipeName.SelectedIndex = 0;
                comboBoxM2TeachRecipeName.Items.Add(modelName);
                comboBoxM2TeachRecipeName.SelectedIndex = 0;
                comboBoxM3TeachRecipeName.Items.Add(modelName);
                comboBoxM3TeachRecipeName.SelectedIndex = 0;
                comboBoxM1TestRecipeName.Items.Add(modelName);
                comboBoxM1TestRecipeName.SelectedIndex = 0;
                comboBoxM2TestRecipeName.Items.Add(modelName);
                comboBoxM2TestRecipeName.SelectedIndex = 0;
                comboBoxM3TestRecipeName.Items.Add(modelName);
                comboBoxM3TestRecipeName.SelectedIndex = 0;

            }
        }
        #endregion

        private async void buttonAddNewRecipe_Click(object sender, EventArgs e)
        {
            if (textBoxMRecipeName.Text.Length >= 0 && textBoxMRecipeName.Text.Length < 4)
            {
                xDialog.MsgBox.Show("recipe name not valid", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Exclamation, xDialog.MsgBox.AnimateStyle.FadeIn);
                return;
            }

            object[] value = new object[]
                   {
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
                    (checkBoxM4Param1.CheckState == CheckState.Checked)?1:0,
                    0,
                    textBoxLaserLine1.Text,
                    textBoxLaserLine2.Text,
                    //M6 params
                    (checkBoxM5Param1.CheckState == CheckState.Checked)?1:0,
                    0,
                    //M6 params
                    (checkBoxM6Param1.CheckState == CheckState.Checked)?1:0,
                    0,
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
                xDialog.MsgBox.Show("Recipe succesfully created", "PBoot", xDialog.MsgBox.Buttons.OK);
            }
            else
            {
                //manage db error
            }
        }

        private async void comboBoxM3PrgName_st1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxM3PrgName_st1.Text == "") return;
            //get model name
            string prgName = comboBoxM3PrgName_st1.Text;
            M3PrgName1 = comboBoxM3PrgName_st1.Text;
            string modelName = prgName.Substring(2, 4);

            //check model name
            if (modelName == "" )
            {
                //program name with incorrect format
                //todo add message to the operator
                return;
            }

            //get data from DB
            MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

            if ((recs.Error == 0) & (recs.Result.Count != 0))
            {                
                //send recipe
                string keyValue = "pcM3Param1";
                var sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param1.ToString()));

                //send type order: RG, LF
                //type order 2: LF, RG
                keyValue = "pcM3TypeOrder";
                sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param2.ToString()));

                labelM3Param1Value.Text = recs.Result[0].m3_param1.ToString();
                labelM3Param2Value.Text = recs.Result[0].m3_param2.ToString();

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
                        if (allsent) AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padprintExt, "program sent succesfully " + "station 1");
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
                //send recipe
                string keyValue = "pcM3Param1";
                var sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param1.ToString()));

                keyValue = "pcM3TypeOrder";
                sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param2.ToString()));

                labelM3Param1Value.Text = recs.Result[0].m3_param1.ToString();
                labelM3Param2Value.Text = recs.Result[0].m3_param2.ToString();
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
                        if (allsent) AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padprintExt, "program sent succesfully " + "station 2");
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

        private async void comboBoxM4PrgName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxM4PrgName.Text == "") return;

            //get model name
            string prgName = comboBoxM4PrgName.Text;
            M4PrgName = comboBoxM4PrgName.Text;
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
                //send recipe
                string keyValue = "pcM4Param1";
                var sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m4_param1.ToString()));
                labelM4Param1Value.Text = recs.Result[0].m4_param1.ToString();

                string keyToSend = "pcM4ProgramName";

                var readResult = await ccService.Send(keyToSend, "");
                if (readResult.OpcResult)
                {

                }
                else
                {

                }

                keyToSend = "pcM4ProgramName";

                readResult = await ccService.Send(keyToSend, comboBoxM4PrgName.Text);
                if (readResult.OpcResult)
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padLaser, "program sent succesfully");
                }
                else
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padLaser, "error sending " + readResult.NodeString);
                }

                //sned to padlaser
                WritePadLaserRecipe(recs.Result[0].m4_param3.ToString(), recs.Result[0].m4_param4.ToString(), Properties.Settings.Default.PadLaserFilePath);
            }
            else
            {
                //db error manage
            }
        }

        private async void comboBoxM2PrgName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxM2PrgName.Text == "") return;

            //get model name
            string prgName = comboBoxM2PrgName.Text;
            M2PrgName = comboBoxM2PrgName.Text;
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
                //todo: send recipe (todo: waiting mysql integration)
                string keyValue = "pcM2Param1";
                var sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m2_param1.ToString()));
                if (sendResult.OpcResult)
                {
                    labelM2Param1Value.Text = recs.Result[0].m2_param1.ToString();
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
                    objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[1] + "\\" + comboBoxM2PrgName.Text + config.Extensions[0]);
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
                                AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padprintInt, "error sending " + result.Value.NodeString);
                            }
                            allsent = allsent & result.Value.OpcResult;
                        }
                        if (allsent) AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padprintInt, "program sent succesfully");
                    }
                }
                else
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padprintInt, "verify program file");
                }
            }
        }

        private async void comboBoxM1PrgName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxM1PrgName.Text == "") return;

            //get model name
            string prgName = comboBoxM1PrgName.Text;
            M1PrgName = comboBoxM1PrgName.Text;
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
                //send recipe (todo: waiting mysql)
                string keyValue = "pcM1Param1";
                var sendResult = await ccService.Send(keyValue, short.Parse(recs.Result[0].m1_param1.ToString()));
                labelM1Param1Value.Text = recs.Result[0].m1_param1.ToString();

                //send quote, speed
                var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

                if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                {
                    ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                    ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                    objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[0] + "\\" + comboBoxM1PrgName.Text + config.Extensions[0]);
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
                                AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.trimmer, "error sending " + result.Value.NodeString);
                            }
                            allsent = allsent & result.Value.OpcResult;
                        }
                        if (allsent) AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.trimmer, "program sent succesfully");

                        string key = "pc_timer_stop_stivale";
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
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.trimmer, "verify program file");
            }
        }
        private void dataGridViewM2TeachPoints_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

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

            //send start command
            keyToSend = "pcM3StartTest";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else xDialog.MsgBox.Show("offline", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Exclamation, xDialog.MsgBox.AnimateStyle.FadeIn);
        }

        private void buttonMRecipiesShowAll_Click(object sender, EventArgs e)
        {

            return;

            //fill data
            string connectionString = "Data Source=localhost;database=plasticaucho;uid=USER;pwd=Robots2023!";
            string query = "Select * from models";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                {
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    dataGridViewMEditRecipe.DataSource = ds.Tables[0];
                    dataGridViewMEditRecipe.Columns[0].HeaderText = "recipe name";
                    dataGridViewMEditRecipe.Columns[0].Width = 120;
                    dataGridViewMEditRecipe.Columns[1].HeaderText = "trimmer on/off";
                    dataGridViewMEditRecipe.Columns[1].Width = 140;
                    dataGridViewMEditRecipe.Columns[2].Visible = false;
                    dataGridViewMEditRecipe.Columns[3].HeaderText = "padprint int on/off";
                    dataGridViewMEditRecipe.Columns[3].Width = 140;
                    dataGridViewMEditRecipe.Columns[4].Visible = false;
                    dataGridViewMEditRecipe.Columns[5].HeaderText = "padprint ext on/off";
                    dataGridViewMEditRecipe.Columns[5].Width = 140;
                    dataGridViewMEditRecipe.Columns[6].HeaderText = "padprint ext foot";
                    dataGridViewMEditRecipe.Columns[6].Width = 140;

                    //dataGridViewMEditRecipe.Columns[6].Visible = false;

                    dataGridViewMEditRecipe.Columns[7].HeaderText = "padlaser on/off";
                    dataGridViewMEditRecipe.Columns[7].Width = 140;
                    dataGridViewMEditRecipe.Columns[8].HeaderText = "line top";
                    dataGridViewMEditRecipe.Columns[8].Width = 140;
                    dataGridViewMEditRecipe.Columns[9].HeaderText = "line bottom";
                    dataGridViewMEditRecipe.Columns[9].Width = 140;
                }
            }
        }

        private void toolStripMenuItemT1_1Keyboard_Click(object sender, EventArgs e)
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

        private void toolStripMenuItemT1_2Keyboard_Click(object sender, EventArgs e)
        {
            Process foo = new Process();

            foo.StartInfo.FileName = @AppDomain.CurrentDomain.BaseDirectory + "Oskeyboard.exe";

            bool isRunning = false; //TODO: Check to see if process foo.exe is already running
            var processExists = Process.GetProcesses().Any(p => p.ProcessName.Contains("Oskeyboard"));

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

        private void toolStripMenuItemT0Keyboard_Click(object sender, EventArgs e)
        {
            Process foo = new Process();

            foo.StartInfo.FileName = @AppDomain.CurrentDomain.BaseDirectory + "Oskeyboard.exe";

            bool isRunning = false; //TODO: Check to see if process foo.exe is already running
            var processExists = Process.GetProcesses().Any(p => p.ProcessName.Contains("Oskeyboard"));

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

        private void toolStripComboBoxT1_1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBoxT1_1.Text == "") return;

            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                List<IObjProgram> pList = progRS.GetProgram(config.ProgramsPath[0], config.Extensions, toolStripComboBoxT1_1.Text);

                comboBoxM1TeachProgramList.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM1TeachProgramList.Items.Add(prgName.ProgramName);
                }
            }
        }

        private void toolStripComboBoxT1_2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBoxT1_2.Text == "") return;

            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                List<IObjProgram> pList = progRS.GetProgram(config.ProgramsPath[0], config.Extensions, toolStripComboBoxT1_2.Text);

                comboBoxM1TestProgramList.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM1TestProgramList.Items.Add(prgName.ProgramName);
                }
            }
        }


        private void toolStripComboBoxT0_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            List<IObjProgram> pList = new List<IObjProgram>();

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                pList = progRS.GetProgram(config.ProgramsPath[0], config.Extensions, toolStripComboBoxT0.Text);
                comboBoxM1PrgName.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM1PrgName.Items.Add(prgName.ProgramName);
                }

                pList = progRS.GetProgram(config.ProgramsPath[1], config.Extensions, toolStripComboBoxT0.Text);
                comboBoxM2PrgName.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM2PrgName.Items.Add(prgName.ProgramName);
                }

                pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, toolStripComboBoxT0.Text);
                comboBoxM3PrgName_st1.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM3PrgName_st1.Items.Add(prgName.ProgramName);
                }

                pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, toolStripComboBoxT0.Text);
                comboBoxM3PrgName_st2.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM3PrgName_st2.Items.Add(prgName.ProgramName);
                }

                //pad laser combobox
                List<string> mList = new List<string>();
                MySqlResult<padlaserprogram> result = mysqlService.DBTable[1].SelectAll<padlaserprogram>();
                result.Result.ForEach(x => mList.Add(x.program_name));

                
                comboBoxM4PrgName.Items.Clear();
                foreach (string prgName in mList)
                {
                    //filter by model name
                    if (prgName.Contains(toolStripComboBoxT0.Text))
                    comboBoxM4PrgName.Items.Add(prgName);
                }
            }
        }

        private async void radioButtonFootOrderOpt1Test_CheckedChanged(object sender, EventArgs e)
        {
            //send start command
            string keyToSend = "pcM3TestType";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else xDialog.MsgBox.Show("offline", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Exclamation, xDialog.MsgBox.AnimateStyle.FadeIn);
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
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    string keyValue = "pcM1Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m1_param1.ToString()));

                    WriteOnLabelAsync(labelM1Param1Value, recs.Result[0].m1_param1.ToString());

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
            if (ccService.ClientIsConnected == false)
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

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    string keyValue = "pcM1Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m1_param1.ToString()));

                    WriteOnLabelAsync(labelM1Param1Value, recs.Result[0].m1_param1.ToString());
                }
            }
        }

        public async void UpdateRecipeToM2(string modelName)
        {
            if (ccService.ClientIsConnected == false)
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

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    string keyValue = "pcM2Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m2_param1.ToString()));

                    WriteOnLabelAsync(labelM2Param1Value, recs.Result[0].m2_param1.ToString());
                }
            }
        }

        public async void UpdateRecipeToM3(string modelName)
        {
            if (ccService.ClientIsConnected == false)
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

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    string keyValue = "pcM3Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param1.ToString()));

                    WriteOnLabelAsync(labelM3Param1Value, recs.Result[0].m3_param1.ToString());

                    keyValue = "pcM3Param2";
                    sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param2.ToString()));

                    WriteOnLabelAsync(labelM3Param2Value, recs.Result[0].m3_param2.ToString());
                }
            }
        }

        public async void UpdateRecipeToM4(string modelName)
        {
            if (ccService.ClientIsConnected == false)
            {

            }
            else
            {
                //get data from DB
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    string keyValue = "pcM4Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m4_param1.ToString()));

                    WriteOnLabelAsync(labelM3Param1Value, recs.Result[0].m3_param1.ToString());

                    WritePadLaserRecipe(recs.Result[0].m4_param3.ToString(), recs.Result[0].m4_param3.ToString(), Properties.Settings.Default.PadLaserFilePath);
                }
            }
        }

        public async void UpdateRecipeToM6(string modelName)
        {
            if (ccService.ClientIsConnected == false)
            {

            }
            else
            {
                //get data from DB
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    string keyValue = "pcM6Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m6_param1.ToString()));

                    WriteOnLabelAsync(labelM6Param1Value, recs.Result[0].m6_param1.ToString());

                }
            }
        }

        public async void RestartRequestFromM2()
        {
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
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    string keyValue = "pcM2Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m2_param1.ToString()));

                    WriteOnLabelAsync(labelM2Param1Value, recs.Result[0].m2_param1.ToString());

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
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    string keyValue = "pcM3Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param1.ToString()));

                    WriteOnLabelAsync(labelM3Param1Value, recs.Result[0].m3_param1.ToString());

                    keyValue = "pcM3Param2";
                    sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m3_param2.ToString()));

                    WriteOnLabelAsync(labelM3Param2Value, recs.Result[0].m3_param2.ToString());

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
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    string keyValue = "pcM4Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m4_param1.ToString()));

                    WriteOnLabelAsync(labelM4Param1Value, recs.Result[0].m4_param1.ToString());

                    keyValue = "pcM4ProgramName";

                    var readResult = await ccService.Send(keyValue, "");
                    if (readResult.OpcResult)
                    {

                    }
                    else
                    {

                    }

                    keyValue = "pcM4ProgramName";
                    readResult = await ccService.Send(keyValue, M4PrgName);
                    if (readResult.OpcResult)
                    {
                    }
                    else
                    {
                    }
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
            if (ccService.ClientIsConnected == false)
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
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    string keyValue = "pcM5Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m5_param1.ToString()));

                    WriteOnLabelAsync(labelM5Param1Value, recs.Result[0].m5_param1.ToString());
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
            if (ccService.ClientIsConnected == false)
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
                MySqlResult<recipies> recs = await mysqlService.DBTable[0].SelectByPrimaryKeyAsync<recipies>(modelName);

                if ((recs.Error == 0) & (recs.Result.Count != 0))
                {
                    string keyValue = "pcM5Param1";
                    var sendResult1 = await ccService.Send(keyValue, short.Parse(recs.Result[0].m6_param1.ToString()));

                    WriteOnLabelAsync(labelM6Param1Value, recs.Result[0].m6_param1.ToString());
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
                textBoxM4TopLineRecipe.Text = recs.Result[0].m4_param3.ToString();
                textBoxM4BottomLineRecipe.Text = recs.Result[0].m4_param4.ToString();
            }
        }

        private void buttonMUpdateRecipe_Click(object sender, EventArgs e)
        {
            if (comboBoxMRecipeName.Text == "") return;

            //update recipe
            DialogResult res = xDialog.MsgBox.Show("Are you sure you want to update recipe?", "PBoot", xDialog.MsgBox.Buttons.YesNo);
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
            textBoxM4TopLineRecipe.Text,
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
                        "m4_param3",
                        "m4_param4",
                        };

                MySqlResult recs = mysqlService.DBTable[0].UpdateAutomaticPrimaryKey(comboBoxMRecipeName.Text, rParams, values);
                if (recs.Error == 0)
                {
                    res = xDialog.MsgBox.Show("Recipe succesfully updated", "PBoot", xDialog.MsgBox.Buttons.OK);
                }
            }

            //update auto recipe
            UpdateRecipeToM1(comboBoxMRecipeName.Text);
            UpdateRecipeToM2(comboBoxMRecipeName.Text);
            UpdateRecipeToM3(comboBoxMRecipeName.Text);
            UpdateRecipeToM4(comboBoxMRecipeName.Text);
            UpdateRecipeToM6(comboBoxMRecipeName.Text);
        }

        private void checkBoxM1RecipeModify_CheckedChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM1RecipeModify.CheckState == CheckState.Checked) ? true : false;

            checkBoxM1RecipeModify.ImageIndex = (chkValue) ? 0 : 1;
            labelM1ParamRecipe.Text = (chkValue) ? "on" : "off";
        }

        private void checkBoxM2RecipeModify_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM2RecipeModify.CheckState == CheckState.Checked) ? true : false;

            checkBoxM2RecipeModify.ImageIndex = (chkValue) ? 0 : 1;
            labelM2ParamRecipe.Text = (chkValue) ? "on" : "off";
        }

        private void checkBoxM3RecipeModify_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM3RecipeModify.CheckState == CheckState.Checked) ? true : false;

            checkBoxM3RecipeModify.ImageIndex = (chkValue) ? 0 : 1;
            labelM3ParamRecipe.Text = (chkValue) ? "on" : "off";
        }

        private void checkBoxM4RecipeModify_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM4RecipeModify.CheckState == CheckState.Checked) ? true : false;

            checkBoxM4RecipeModify.ImageIndex = (chkValue) ? 0 : 1;
            labelM4ParamRecipe.Text = (chkValue) ? "on" : "off";
        }

        private void checkBoxM5RecipeModify_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM5RecipeModify.CheckState == CheckState.Checked) ? true : false;

            checkBoxM5RecipeModify.ImageIndex = (chkValue) ? 0 : 1;
            labelM5ParamRecipe.Text = (chkValue) ? "on" : "off";
        }

        private void checkBoxM6RecipeModify_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM6RecipeModify.CheckState == CheckState.Checked) ? true : false;

            checkBoxM6RecipeModify.ImageIndex = (chkValue) ? 0 : 1;
            labelM6ParamRecipe.Text = (chkValue) ? "on" : "off";
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
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            List<IObjProgram> pList = new List<IObjProgram>();

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                pList = progRS.GetProgram(config.ProgramsPath[0], config.Extensions, toolStripComboBoxT0.Text);
                comboBoxM1PrgName.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM1PrgName.Items.Add(prgName.ProgramName);
                }

                pList = progRS.GetProgram(config.ProgramsPath[1], config.Extensions, toolStripComboBoxT0.Text);
                comboBoxM2PrgName.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM2PrgName.Items.Add(prgName.ProgramName);
                }

                pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, toolStripComboBoxT0.Text);
                comboBoxM3PrgName_st1.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM3PrgName_st1.Items.Add(prgName.ProgramName);
                }

                pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, toolStripComboBoxT0.Text);
                comboBoxM3PrgName_st2.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM3PrgName_st2.Items.Add(prgName.ProgramName);
                }

                //pad laser combobox
                List<string> mList = new List<string>();
                MySqlResult<padlaserprogram> result = mysqlService.DBTable[1].SelectAll<padlaserprogram>();
                result.Result.ForEach(x => mList.Add(x.program_name));


                comboBoxM4PrgName.Items.Clear();
                foreach (string prgName in mList)
                {
                    //filter by model name
                    if (prgName.Contains(toolStripComboBoxT0.Text))
                        comboBoxM4PrgName.Items.Add(prgName);
                }
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

                foreach (IObjProgram prgName in pList)
                {
                    //filter by model name
                    if (prgName.ProgramName.Contains(comboBoxM1TeachRecipeName.Text))
                        comboBoxM1TeachProgramList.Items.Add(prgName.ProgramName);
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

                foreach (IObjProgram prgName in pList)
                {
                    //filter by model name
                    if (prgName.ProgramName.Contains(comboBoxM1TestRecipeName.Text))
                        comboBoxM1TestProgramList.Items.Add(prgName.ProgramName);
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

                foreach (IObjProgram prgName in pList)
                {
                    //filter by model name
                    if (prgName.ProgramName.Contains(comboBoxM2TeachRecipeName.Text))
                        comboBoxM2TeachProgramList.Items.Add(prgName.ProgramName);
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

                foreach (IObjProgram prgName in pList)
                {
                    //filter by model name
                    if (prgName.ProgramName.Contains(comboBoxM2TestRecipeName.Text))
                        comboBoxM2TestProgramList.Items.Add(prgName.ProgramName);
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

                foreach (IObjProgram prgName in pList)
                {
                    //filter by model name
                    if (prgName.ProgramName.Contains(comboBoxM3TeachRecipeName.Text))
                        comboBoxM3TeachProgramList.Items.Add(prgName.ProgramName);
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

                foreach (IObjProgram prgName in pList)
                {
                    //filter by model name
                    if (prgName.ProgramName.Contains(comboBoxM3TestRecipeName.Text))
                        comboBoxM3TestProgramList.Items.Add(prgName.ProgramName);
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


        public void WritePadLaserRecipe(string top, string bottom, string FileName)
        {
            try
            {
                using (TextWriter tw = new StreamWriter(FileName, false))
                {
                    tw.WriteLine(top);
                    tw.WriteLine(bottom);
                }                
                AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padLaser, "parameters succesfully sent");
            }
            catch (Exception ex)  //Writing to log has failed, send message to trace in case anyone is listening.
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padLaser, "check il file is opened");
            }
        }



     

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if ((e.ColumnIndex == 1) && (e.RowIndex >= 0))
            {
                System.Drawing.Image img = new Bitmap(Properties.Settings.Default.ImagesFilepath + "\\notinalarm.png");
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText == "notinalarm")
                {
                    img = new Bitmap(Properties.Settings.Default.ImagesFilepath + "\\notinalarm.png");
                }
                else
                {
                    img = new Bitmap(Properties.Settings.Default.ImagesFilepath + "\\inalarm.png");
                }
                var w = 24;// img.Width;
                var h = 24;// img.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(img, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }
    }    

        
}
