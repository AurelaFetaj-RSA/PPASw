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

            mysqlService.AddTable<recipies>("recipies");
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

            #region (* init AUTO combobox model name list *)
            List<string> mList = new List<string>();

            //var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            //ReadProgramsConfiguration config = null;
            //List<string> mList = new List<string>();
            //ReadProgramsService progRS = (ReadProgramsService)dummyS[0];

            //if (dummyS != null && dummyS.Count > 0)
            //{
            //    config = progRS.Configuration as ReadProgramsConfiguration;
            //    mList = progRS.GetModel(config.ProgramsPath, config.Extensions);

            //get model name list from DB
            MySqlResult<recipies> result = await mysqlService.DBTable[0].SelectAllAsync<recipies>();
            result.Result.ForEach(x => mList.Add(x.model_name));
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
            }
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

                if (nextPage != null)
                {
                    if (tabControlMain.SelectedPage.Index == 0)
                    {
                        tabControlMain.SelectedPage.ContextMenuStrip.Show();
                    }
                }
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

        private void button1_Click(object sender, EventArgs e)
        {
            SetPriorityToDataGridRow(1, Priority.normal);
            UpdateColorToDataGridRow();
        }

        private async void buttonAddNewRecipe_Click(object sender, EventArgs e)
        {
            if (textBoxMRecipeName.Text.Length >= 0 && textBoxMRecipeName.Text.Length < 4)
            {
                xDialog.MsgBox.Show("recipe name not valid", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Exclamation, xDialog.MsgBox.AnimateStyle.FadeIn);
            }

            object[] value = new object[]
                   {
                    textBoxMRecipeName.Text,
                    (checkBoxM1Param1.CheckState == CheckState.Checked)?1:0,
                    0,
                    (checkBoxM2Param1.CheckState == CheckState.Checked)?1:0,
                    0,
                    (checkBoxM3Param1.CheckState == CheckState.Checked)?1:0,
                    0,
                    (checkBoxM4Param1.CheckState == CheckState.Checked)?1:0,
                    0,
                    textBoxLaserLine1.Text,
                    textBoxLaserLine2.Text,
                    (checkBoxM5Param1.CheckState == CheckState.Checked)?1:0,
                    0,
                    (checkBoxM6Param1.CheckState == CheckState.Checked)?1:0,
                    0,
                   };

            var testResult = await mysqlService.DBTable[0].InsertAutomaticAsync(value);

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
            short[] quote = new short[5];
            short[] speed = new short[5];

            try
            {
                for (i = 0; i <= dataGridViewM2TestPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = short.Parse(dataGridViewM3TestPoints[1, i].Value.ToString());
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
            short[] quote = new short[5];
            short[] speed = new short[5];

            try
            {
                for (i = 0; i <= dataGridViewM2TestPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = short.Parse(dataGridViewM2TestPoints[1, i].Value.ToString());
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
            short[] quote = new short[5];
            short[] speed = new short[5];

            try
            {
                for (i = 0; i <= dataGridViewM1TestPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = short.Parse(dataGridViewM1TestPoints[1, i].Value.ToString());
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
            if (_configForm == null)
                _configForm = new ServiceSetup(myCore);

            _configForm.Show();
            _configForm.Activate();
        }

        private void buttonMRecipiesShowAll_Click(object sender, EventArgs e)
        {
            //fill data
            string connectionString = "Data Source=localhost;database=plasticaucho;uid=USER;pwd=Robots2023!";
            string query = "Select * from recipies";
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
                    dataGridViewMEditRecipe.Columns[6].Visible = false;

                    dataGridViewMEditRecipe.Columns[7].HeaderText = "padlaser on/off";
                    dataGridViewMEditRecipe.Columns[7].Width = 140;
                    dataGridViewMEditRecipe.Columns[8].HeaderText = "line top";
                    dataGridViewMEditRecipe.Columns[8].Width = 140;
                    dataGridViewMEditRecipe.Columns[9].HeaderText = "line bottom";
                    dataGridViewMEditRecipe.Columns[9].Width = 140;


                }
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

        private async void buttonM3StartTest_Click(object sender, EventArgs e)
        {
            //send quote/speed
            M3TestSendProgram();

            //send start command
            string keyToSend = "pcM3StartTest";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else xDialog.MsgBox.Show("offline", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Exclamation, xDialog.MsgBox.AnimateStyle.FadeIn);

        }
    }    
}
