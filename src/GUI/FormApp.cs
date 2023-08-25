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

namespace GUI
{
    public partial class FormApp : Form
    {
        //core instance
        public static Core myCore;
        OpcClientService ccService = null;

        private LidorSystems.IntegralUI.Containers.TabPage lastPage { get; set; } = null;

        private readonly SplashScreen _splashScreen = null;
        private Form _configForm { get; set; } = null;
        private Form _clientForm { get; set; } = null;

        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private ReadProgramsService _readProgramService { get; set; } = null;

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
        }

        private void InitLastParameter()
        {
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

            //dataGridViewM2TestPoints.Rows.Add(4);
            //dataGridViewM2TestPoints.Rows[0].Cells[0].Value = 1;
            //dataGridViewM2TestPoints.Rows[1].Cells[0].Value = 2;
            //dataGridViewM2TestPoints.Rows[2].Cells[0].Value = 3;
            //dataGridViewM2TestPoints.Rows[3].Cells[0].Value = 4;

            //dataGridViewM2TestPoints.Rows[0].Cells[1].Value = 100;
            //dataGridViewM2TestPoints.Rows[1].Cells[1].Value = 200;
            //dataGridViewM2TestPoints.Rows[2].Cells[1].Value = 300;
            //dataGridViewM2TestPoints.Rows[3].Cells[1].Value = 400;

            //dataGridViewM2TestPoints.Rows[0].Cells[2].Value = 10;
            //dataGridViewM2TestPoints.Rows[1].Cells[2].Value = 20;
            //dataGridViewM2TestPoints.Rows[2].Cells[2].Value = 30;
            //dataGridViewM2TestPoints.Rows[3].Cells[2].Value = 40;

            //dataGridViewM2TestPoints.ClearSelection();
            //#endregion

            //#region(* init datagridviewM3 *)
            //dataGridViewM3TeachPoints.Rows.Add(4);
            //dataGridViewM3TeachPoints.Rows[0].Cells[0].Value = 1;
            //dataGridViewM3TeachPoints.Rows[1].Cells[0].Value = 2;
            //dataGridViewM3TeachPoints.Rows[2].Cells[0].Value = 3;
            //dataGridViewM3TeachPoints.Rows[3].Cells[0].Value = 4;

            //dataGridViewM3TeachPoints.Rows[0].Cells[1].Value = 100;
            //dataGridViewM3TeachPoints.Rows[1].Cells[1].Value = 200;
            //dataGridViewM3TeachPoints.Rows[2].Cells[1].Value = 300;
            //dataGridViewM3TeachPoints.Rows[3].Cells[1].Value = 400;

            //dataGridViewM3TeachPoints.Rows[0].Cells[2].Value = 10;
            //dataGridViewM3TeachPoints.Rows[1].Cells[2].Value = 20;
            //dataGridViewM3TeachPoints.Rows[2].Cells[2].Value = 30;
            //dataGridViewM3TeachPoints.Rows[3].Cells[2].Value = 40;
            //dataGridViewM3TeachPoints.ClearSelection();

            //dataGridViewM3TestPoints.Rows.Add(4);
            //dataGridViewM3TestPoints.Rows[0].Cells[0].Value = 1;
            //dataGridViewM3TestPoints.Rows[1].Cells[0].Value = 2;
            //dataGridViewM3TestPoints.Rows[2].Cells[0].Value = 3;
            //dataGridViewM3TestPoints.Rows[3].Cells[0].Value = 4;

            //dataGridViewM3TestPoints.Rows[0].Cells[1].Value = 100;
            //dataGridViewM3TestPoints.Rows[1].Cells[1].Value = 200;
            //dataGridViewM3TestPoints.Rows[2].Cells[1].Value = 300;
            //dataGridViewM3TestPoints.Rows[3].Cells[1].Value = 400;

            //dataGridViewM3TestPoints.Rows[0].Cells[2].Value = 10;
            //dataGridViewM3TestPoints.Rows[1].Cells[2].Value = 20;
            //dataGridViewM3TestPoints.Rows[2].Cells[2].Value = 30;
            //dataGridViewM3TestPoints.Rows[3].Cells[2].Value = 40;

            //dataGridViewM3TestPoints.ClearSelection();
            //#endregion

            #region (* init AUTO combobox model name list *)
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            ReadProgramsConfiguration config = null;
            List<string> mList = new List<string>();
            ReadProgramsService progRS = (ReadProgramsService)dummyS[0];

            if (dummyS != null && dummyS.Count > 0)
            {
                config = progRS.Configuration as ReadProgramsConfiguration;
                mList = progRS.GetModel(config.ProgramsPath, config.Extensions);

                foreach (string modelName in mList)
                {
                    comboBoxAutoModelNameLst.Items.Add(modelName);
                }
            }
            #endregion

            //#region (* init M2 combobox model name list *)
            //mList = progRS.GetModel(config.ProgramsPath[1], config.Extensions);

            //foreach (string modelName in mList)
            //{
            //    comboBoxM2TeachModelName.Items.Add(modelName);
            //}

            //#endregion

            //#region (* init M3 combobox model name list *)
            //mList = progRS.GetModel(config.ProgramsPath[2], config.Extensions);

            //foreach (string modelName in mList)
            //{
            //    comboBoxM3TeachModelName.Items.Add(modelName);
            //}

            #endregion

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
                if (nextPage != null) this.tabControlMain.SelectedPage = nextPage;
            }
            else
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
            catch(Exception Ex)
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

        private void button1_Click(object sender, EventArgs e)
        {
            SetPriorityToDataGridRow(1, Priority.normal);
            UpdateColorToDataGridRow();
        }

        private void buttonAddNewRecipe_Click(object sender, EventArgs e)
        {
            if (textBoxRecipeName.Text.Length >= 0 && textBoxRecipeName.Text.Length < 4)
            {
                //todo
                MessageBox.Show("recipe name not valid");
            }
        }
    }
}
