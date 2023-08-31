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
using RSAPoints.Points;
using RSACommon.Points;
using RSAPoints.ConcretePoints;

namespace GUI
{
    public partial class FormMain : Form
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
        public FormMain(SplashScreen splashScreen)
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

        public void Start()
        {
            //myCore.Start();
        }

        public async void SetEvent()
        {

        }

        //public void StartUpdateTask()
        //{

        //    //.Run(async () => await UpdateGraphicsGUI(TimeSpan.FromMilliseconds(Settings.Default.UpdateGUILed), _cancellationTokenSource));
        //    //Task.Run(async () => await UpdateDiagnosticGUI(TimeSpan.FromMilliseconds(myCore.DiagnosticConfigurator.Configuration.DiagnosticPolling), _cancellationTokenSource));
        //    Task.Run(async () => await UpdateOPCUAStatus(TimeSpan.FromMilliseconds(1000), _cancellationTokenSource));

        //}


        private void RSACustomEvents_ServiceDisconnectionEvent(object sender, RSACustomEvents.ServiceConnectionEventArgs e)
        {
            if (e.Service is IRobot<KawasakiMemoryVariable>)
            {

            }
            else if (e.Service is OpcServerService opcService) //In questo caso no nfaccio niente perchè non ho modo, solo Subscription ( per ora )
            {

            }
            else if (e.Service is WebApiCore api)
            {
                //null
            }

        }

        private void RSACustomEvents_ServiceConnectionEvent(object sender, RSACustomEvents.ServiceConnectionEventArgs e)
        {

            if (e.Service is IRobot<KawasakiMemoryVariable>)
            {

            }
            else if (e.Service is OpcServerService opcService) //In questo caso no nfaccio niente perchè non ho modo, solo Subscription ( per ora )
            {
                //null
            }
            else if (e.Service is WebApiCore api)
            {
                //null
            }

        }

        private void RSACustomEvents_KeepAliveOkEvent(object sender, RSACustomEvents.KeepAliveOkEventArgs e)
        {

            if (e.Service is OpcServerService opcService)
            {

            }

        }

        private void RSACustomEvents_KeepAliveTimeoutEvent(object sender, RSACustomEvents.KeepAliveTimeOutEventArgs e)
        {

            if (e.Service is OpcServerService opcService)
            {

            }

        }

        private void RSACustomEvents_OPCSubscriptionEvent(object sender, RSACustomEvents.OPCSubscriptionEventArgs e)
        {
            if (e.Service is OpcServerService opcService)
            {

            }

        }

        private void RSACustomEvents_AckTimeoutEvent(object sender, RSACustomEvents.TimeoutEventArgs e)
        {

            if (e.Service is OpcServerService)
            {
                if (e.Result == MessageResponse.Success)
                {

                }
                else
                {

                }
            }

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

        private void RSACustomEvents_ServiceHasLoadProgramEvent(object sender, RSACustomEvents.ProgramsReadEndedEventArgs e)
        {
            if (e.Service is ReadProgramsService progRS)
            {
                //modelCombobox.Items.Clear();
                //modelCombobox.Items.AddRange(progRS.ModelDictionary.Keys.ToArray<string>());
            }
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

            #region (* init M2 combobox model name list *)
            mList = progRS.GetModel(config.ProgramsPath[1], config.Extensions);

            foreach (string modelName in mList)
            {
                comboBoxM2TeachModelName.Items.Add(modelName);
            }

            #endregion

            #region (* init M3 combobox model name list *)
            mList = progRS.GetModel(config.ProgramsPath[2], config.Extensions);

            foreach (string modelName in mList)
            {
                comboBoxM3TeachModelName.Items.Add(modelName);
            }

            #endregion
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

        private void InitServices()
        {
            //webapi config
            /*
            myCore.ApiSharedList.AddUser<WebApiRSWareSharedClass>(myRSAUser);
            IServerShared sharedClass = myCore.ApiSharedList.GetWebSharedUserInstance(myRSAUser);

            LinkEventManager(sharedClass);
            if(myCore.Robot is IRobot<IRobotVariable> r)
            {
                r.LoadMemoryConfiguration();
                myRobot = r;
            }


            //opcua server config
            FutureOpcServerCustom opcFutureServer = null;
            if((opcFutureServer = (FutureOpcServerCustom)myCore.FindPerType(typeof(FutureOpcServerCustom))) != null)
            {
                opcFutureServer.M2FNodeManager.AddUser(myRSAUser);
            }
            */
        }

        private void InitGUI()
        {
            InitLastParameter();
        }

        #region(* GUI callback *)

        private void tabControlMain_SelectedPageChanged(object sender, LidorSystems.IntegralUI.ObjectEventArgs e)
        {

            //I will not save the state if is hide
            if (tabControlMain.SelectedPage.Key == "Hide")
            {
                this.WindowState = FormWindowState.Minimized;
            }
            else
                lastPage = tabControlMain.SelectedPage;
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPageHide_Paint(object sender, PaintEventArgs e)
        {

        }
        #endregion





        private void memoryDumpBtn_Click(object sender, EventArgs e)
        {
            myCore?.Robot?.SaveMemoryConfiguration("MemoryDump.json");
        }

        private void ReconnectionRobotBtn_Click(object sender, EventArgs e)
        {
            myCore?.Robot?.Start();
        }

        private void tabControlMain_ToolItemClicked(object sender, LidorSystems.IntegralUI.ObjectClickEventArgs e)
        {
            StopExecution();
        }

        public void StopExecution()
        {
            try
            {
                //CHiudo i task 
                _cancellationTokenSource.Cancel();

                myCore.Stop();

                RSACustomEvents.OPCServerSubscriptionEvent -= RSACustomEvents_OPCSubscriptionEvent;
                RSACustomEvents.KeepAliveTimeoutEvent -= RSACustomEvents_KeepAliveTimeoutEvent;
                RSACustomEvents.ServiceConnectionEvent -= RSACustomEvents_ServiceConnectionEvent;
                RSACustomEvents.KeepAliveOkEvent -= RSACustomEvents_KeepAliveOkEvent;
                RSACustomEvents.ServiceDisconnectionEvent -= RSACustomEvents_ServiceDisconnectionEvent;

                Application.Exit();
            }
            catch
            {
                myCore.Log?.Warn("Problem in application exit");
                Application.Exit();
            }

        }

        private void tabPageMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void openConfigFormTextbox_Click(object sender, EventArgs e)
        {
            if (_configForm == null)
                _configForm = new ServiceSetup(myCore);

            _configForm.Show();
            _configForm.Activate();

        }

        private void Client_Click(object sender, EventArgs e)
        {


            if (_clientForm == null || _clientForm.IsDisposed)
            {
                OpcClientService clientService = (OpcClientService)myCore.FindPerType(typeof(OpcClientService))[0];
                _clientForm = new ClientTest(clientService);
            }

            _clientForm.Show();
            _clientForm.Activate();

        }

        private async void buttonM2SmallClampOpening_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM2SmallClampOpening";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void buttonM2SmallClampClosing_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM2SmallClampClosing";

            var readResult = await ccService.Send(keyToSend, true);

            if (readResult.OpcResult)
            {

            }
        }

        private async void lbButtonM2JogUp_Click(object sender, EventArgs e)
        {

        }

        private async void lbButtonM2JogDown_Click(object sender, EventArgs e)
        {

        }

        private async void numericUpDownM2JogSpeed_ValueChanged(object sender, EventArgs e)
        {
            string keyToSend = "pcM2JogSpeed";

            var readResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM2JogSpeed.Value.ToString()));

            if (readResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM2JogReset_Click(object sender, EventArgs e)
        {
            string keyToSend = null;

            keyToSend = "pcM2JogReset";
            var readResult = await ccService.Send(keyToSend, true);
            //todo: chi lo mette a false
        }

        private async void buttonM2StartQuote_Click(object sender, EventArgs e)
        {
            string keyToSend = null;

            //quote 
            keyToSend = "pcM2ManualQuote";
            var readResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM2ManualQuote.Value.ToString()));
            if (readResult.OpcResult)
            {
                //await Task.Run(() => ThreadSafeWriteMessage("Value set"));
            }
            else
            {
                //await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }

            keyToSend = "pcM2ManualSpeed";
            var readResult1 = await ccService.Send(keyToSend, short.Parse(numericUpDownM2ManualSpeed.Value.ToString()));

            keyToSend = "pcM2QuoteStart";
            var readResult2 = await ccService.Send(keyToSend, true);
            //todo: chi lo mette a false
        }

        private async void checkBoxM1Inclusion_CheckedChanged(object sender, EventArgs e)
        {
            string keyToSend = null;

            keyToSend = "pcM1Inclusion";
            if (checkBoxM1Inclusion.CheckState == CheckState.Checked)
            {
                var readResult = await ccService.Send(keyToSend, true);
            }
            else
            {
                var readResult = await ccService.Send(keyToSend, false);
            }
        }

        private async void checkBoxM3Inclusion_CheckedChanged(object sender, EventArgs e)
        {
            string keyToSend = null;

            keyToSend = "pcM3Inclusion";
            if (checkBoxM3Inclusion.CheckState == CheckState.Checked)
            {
                var readResult = await ccService.Send(keyToSend, true);
                checkBoxM3Inclusion.Text = "inclusa";
            }
            else
            {
                var readResult = await ccService.Send(keyToSend, false);
                checkBoxM3Inclusion.Text = "esclusa";
            }
        }

        private async void checkBoxM4Inclusion_CheckedChanged(object sender, EventArgs e)
        {
            string keyToSend = null;

            keyToSend = "pcM4Inclusion";
            if (checkBoxM4Inclusion.CheckState == CheckState.Checked)
            {
                var readResult = await ccService.Send(keyToSend, true);
                checkBoxM4Inclusion.Text = "inclusa";
            }
            else
            {
                var readResult = await ccService.Send(keyToSend, false);
                checkBoxM4Inclusion.Text = "esclusa";
            }
        }

        private async void checkBoxM5Inclusion_CheckedChanged(object sender, EventArgs e)
        {
            string keyToSend = null;

            keyToSend = "pcM5Inclusion";
            if (checkBoxM5Inclusion.CheckState == CheckState.Checked)
            {
                var readResult = await ccService.Send(keyToSend, true);
                checkBoxM5Inclusion.Text = "inclusa";
            }
            else
            {
                var readResult = await ccService.Send(keyToSend, false);
                checkBoxM5Inclusion.Text = "esclusa";
            }
        }

        private async void checkBoxM6Inclusion_CheckedChanged(object sender, EventArgs e)
        {
            string keyToSend = null;

            keyToSend = "pcM6Inclusion";
            if (checkBoxM6Inclusion.CheckState == CheckState.Checked)
            {
                var readResult = await ccService.Send(keyToSend, true);
            }
            else
            {
                var readResult = await ccService.Send(keyToSend, false);
            }
        }

        private async void numericUpDownM2ManualSpeed_ValueChanged(object sender, EventArgs e)
        {
            string keyToSend = "pcM2ManualSpeed";

            var readResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM2ManualSpeed.Value.ToString()));

            if (readResult.OpcResult)
            {
                //await Task.Run(() => ThreadSafeWriteMessage("Value set"));
            }
            else
            {
                //await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }
        }

        private async void numericUpDownM2ManualQuote_ValueChanged(object sender, EventArgs e)
        {
            string keyToSend = "pcM2ManualQuote";

            var readResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM2ManualQuote.Value.ToString()));

            if (readResult.OpcResult)
            {
                //await Task.Run(() => ThreadSafeWriteMessage("Value set"));
            }
            else
            {
                //await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }
        }

        private async void buttonM2ResetServo_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM2ResetServoAlarm";

            var readResult = await ccService.Send(keyToSend, true);

            if (readResult.OpcResult)
            {
                //await Task.Run(() => ThreadSafeWriteMessage("Value set"));
            }
            else
            {
                //await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }
        }

        private async void buttonM2Home_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM2Homing";

            var readResult = await ccService.Send(keyToSend, true);

            if (readResult.OpcResult)
            {
                //await Task.Run(() => ThreadSafeWriteMessage("Value set"));
            }
            else
            {
                //await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }
        }

        private async void buttonM2ResetHome_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM2ResetHoming";

            var readResult = await ccService.Send(keyToSend, true);

            if (readResult.OpcResult)
            {
                //await Task.Run(() => ThreadSafeWriteMessage("Value set"));
            }
            else
            {
                //await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }
        }

        private void dataGridViewM2Points_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = 0;
            short[] quote = new short[5];
            short[] speed = new short[5];
            bool[] start = new bool[5] { false, false, false, false, false };


            try
            {
                //get selected row index
                int currentRow = int.Parse(e.RowIndex.ToString());

                short idPoint = (short)(currentRow + 1);
                for (i = 0; i <= dataGridViewM2TeachPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = short.Parse(dataGridViewM2TeachPoints[1, i].Value.ToString());
                    speed[i + 1] = short.Parse(dataGridViewM2TeachPoints[2, i].Value.ToString());
                }

                if (idPoint < 0 || idPoint > 4)
                {
                    //todo message
                    return;
                }

                // register button
                if ((e.ColumnIndex == 3) & currentRow >= 0)
                {
                    //register current axis value
                    dataGridViewM2TeachPoints[1, currentRow].Value = Convert.ToInt32((labelM2TeachAxisQuoteValue.Text));
                    //register current speed value
                    dataGridViewM2TeachPoints[2, currentRow].Value = Convert.ToInt32((numericUpDownM2JogSpeed.Value));
                }

                // start quote button
                if ((e.ColumnIndex == 4) & currentRow >= 0)
                {
                    start[idPoint] = true;
                    //OPCUAM2TeachPckSend(idPoint, quote, speed, start);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async void buttonM2BigClampOpening_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM2BigClampOpening";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void buttonM2BigGripperClosing_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM2BigClampClosing";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void buttonM2CenteringClampsOpening_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM2CentrClampOpening";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void buttonM2CenteringClampsClosing_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM2CentrClampClosing";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void buttonM2ContrastOpening_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM2ContrOpening";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void buttonM2ContrastClosing_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM2ContrClosing";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void buttonM2PrintCycle_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM2Print";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void lbButtonM2StartStopWorkingBelt_Click(object sender, EventArgs e)
        {

        }

        private async void lbButtonM2StartStopExitBelt_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDownM2JogSpeed_Click(object sender, EventArgs e)
        {

        }

        private async void lbButtonM2JogUp_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            string keyToSend = null;

            if (e.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
            {
                //send quote
                keyToSend = "pcM2JogDown";
                var readResult1 = await ccService.Send(keyToSend, false);
                if (readResult1.OpcResult)
                {
                }

                keyToSend = "pcM2JogUp";
                var readResult2 = await ccService.Send(keyToSend, true);
                if (readResult2.OpcResult)
                {
                }
            }
            else
            {
                keyToSend = "pcM2JogDown";
                var readResult1 = await ccService.Send(keyToSend, false);
                if (readResult1.OpcResult)
                {
                }

                keyToSend = "pcM2JogUp";
                var readResult2 = await ccService.Send(keyToSend, false);
                if (readResult2.OpcResult)
                {
                }
            }

        }

        private async void lbButtonM2JogDown_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            string keyToSend = null;

            if (e.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
            {
                keyToSend = "pcM2JogUp";
                var readResult1 = await ccService.Send(keyToSend, false);
                if (readResult1.OpcResult)
                {
                }

                keyToSend = "pcM2JogDown";
                var readResult2 = await ccService.Send(keyToSend, true);
                if (readResult2.OpcResult)
                {
                }
            }
            else
            {
                keyToSend = "pcM2JogUp";
                var readResult1 = await ccService.Send(keyToSend, false);
                if (readResult1.OpcResult)
                {
                }

                keyToSend = "pcM2JogDown";
                var readResult2 = await ccService.Send(keyToSend, false);
                if (readResult2.OpcResult)
                {
                }
            }
        }

        private void dataGridViewM2TestPoints_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = 0;
            short[] quote = new short[5];
            short[] speed = new short[5];

            try
            {
                //get selected row index
                int currentRow = int.Parse(e.RowIndex.ToString());

                short idPoint = (short)(currentRow + 1);
                for (i = 0; i <= dataGridViewM2TeachPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = short.Parse(dataGridViewM2TeachPoints[1, i].Value.ToString());
                    speed[i + 1] = short.Parse(dataGridViewM2TeachPoints[2, i].Value.ToString());
                }

                if (idPoint < 0 || idPoint > 4)
                {
                    //todo message
                    return;
                }

                // edit button
                if ((e.ColumnIndex == 3) & currentRow >= 0)
                {

                    //OPCUAM2TeachPckSend(idPoint, quote, speed, reg);
                }
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

                //OPCUAM2TestPckSend(quote, speed);
            }
            catch (Exception ex)
            {

            }
        }

        private void M3TestSendProgram()
        {
            int i = 0;
            short[] quote = new short[5];
            short[] speed = new short[5];

            try
            {


                for (i = 0; i <= dataGridViewM3TestPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = short.Parse(dataGridViewM3TestPoints[1, i].Value.ToString());
                    speed[i + 1] = short.Parse(dataGridViewM3TestPoints[2, i].Value.ToString());
                }

                //OPCUAM3TestPckSend(quote, speed);
            }
            catch (Exception ex)
            {

            }
        }

        private async void lbButtonM2StartTest_Click(object sender, EventArgs e)
        {
            M2TestSendProgram();

            string keyToSend = "pcM2StartTest";
            var readResult = await ccService.Send(keyToSend, true);
        }

        private async void buttonM5TranslatorFwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5TranslatorFwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5TranslatorBwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5TranslatorBwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5ClampFwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5ClampFwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5ClampBwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5ClampBwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5ClampOpening_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5ClampOpening";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5ClampClosing_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5ClampClosing";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5CWRotation_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5CWRotation";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5CCWRotation_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5CCWRotation";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5NoRotation_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5NoRotation";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5V1ExtFwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5V1ExtFwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5V1ExtBwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5V1ExtBwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5V2ExtFwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5V2ExtFwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5V2ExtBwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5V2ExtBwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void lbButtonM5StartStopTranslBelt_Click(object sender, EventArgs e)
        {

        }

        private async void lbButtonM5StartStopOutBelt1_Click(object sender, EventArgs e)
        {

        }

        private async void lbButtonM5StartStopOutBelt2_Click(object sender, EventArgs e)
        {

        }

        private async void lbButtonM5StartStopOutBelt3_Click(object sender, EventArgs e)
        {

        }

        private async void lbButtonM2StartStopWorkingBelt_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            string keyToSend = null;

            keyToSend = "pcM2StartStopWorkingBelt";
            if (lbButtonM2StartStopWorkingBelt.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
            {
                var readResult = await ccService.Send(keyToSend, true);
            }
            else
            {
                var readResult = await ccService.Send(keyToSend, false);
            }
        }

        private async void lbButtonM2StartStopExitBelt_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            string keyToSend = null;

            keyToSend = "pcM2StartStopExitBelt";
            if (lbButtonM2StartStopWorkingBelt.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
            {
                var readResult = await ccService.Send(keyToSend, true);
            }
            else
            {
                var readResult = await ccService.Send(keyToSend, false);
            }
        }

        private async void lbButtonM5StartStopTranslBelt_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            string keyToSend = "pcM5StartStopTranslBelt";

            if (lbButtonM5StartStopTranslBelt.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
            {
                var readResult = await ccService.Send(keyToSend, true);
            }
            else
            {
                var readResult = await ccService.Send(keyToSend, false);
            }
        }

        private async void lbButtonM5StartStopOutBelt1_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            string keyToSend = "pcM5StartStopOutBelt1";

            if (lbButtonM5StartStopOutBelt1.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
            {
                var sendResult = await ccService.Send(keyToSend, true);
            }
            else
            {
                var sendResult = await ccService.Send(keyToSend, false);
            }
        }

        private async void lbButtonM5StartStopOutBelt3_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            string keyToSend = "pcM5StartStopOutBelt3";

            if (lbButtonM5StartStopOutBelt3.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
            {
                var sendResult = await ccService.Send(keyToSend, true);
            }
            else
            {
                var sendResult = await ccService.Send(keyToSend, false);
            }
        }

        private async void lbButtonM5StartStopOutBelt2_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            string keyToSend = "pcM5StartStopOutBelt2";

            if (lbButtonM5StartStopOutBelt2.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
            {
                var sendResult = await ccService.Send(keyToSend, true);
            }
            else
            {
                var sendResult = await ccService.Send(keyToSend, false);
            }
        }

        private void buttonM2TeachNewProgram_Click(object sender, EventArgs e)
        {
            //reset points datagrid value to default
            ResetM2Datagrid();

        }

        private void buttonM2TeachSaveProgram_Click(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;

                int p1 = Convert.ToInt32(dataGridViewM2TeachPoints[1, 0].Value);
                int p2 = Convert.ToInt32(dataGridViewM2TeachPoints[1, 1].Value);
                int p3 = Convert.ToInt32(dataGridViewM2TeachPoints[1, 2].Value);
                int p4 = Convert.ToInt32(dataGridViewM2TeachPoints[1, 3].Value);
                int s1 = Convert.ToInt32(dataGridViewM2TeachPoints[2, 0].Value);
                int s2 = Convert.ToInt32(dataGridViewM2TeachPoints[2, 1].Value);
                int s3 = Convert.ToInt32(dataGridViewM2TeachPoints[2, 2].Value);
                int s4 = Convert.ToInt32(dataGridViewM2TeachPoints[2, 3].Value);
                ConcretePointsContainer<PointAxis> prgObj = new ConcretePointsContainer<PointAxis>(comboBoxM2TeachProgramList.Text);
                prgObj.AddPoint(new PointAxis(p1, p2, p3, p4, s1, s2, s3, s4));
                prgObj.Save(comboBoxM2TeachProgramList.Text + config.Extensions[0], config.ProgramsPath[1], true);
            }

        }

        private async void buttonM2TeachLoadProgram_Click(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[1] + "\\" + comboBoxM2TeachProgramList.Text + config.Extensions[0]);
                if (objPoints != null)
                {

                    dataGridViewM2TeachPoints[1, 0].Value = objPoints.Points[0].Q1;
                    dataGridViewM2TeachPoints[1, 1].Value = objPoints.Points[0].Q2;
                    dataGridViewM2TeachPoints[1, 2].Value = objPoints.Points[0].Q3;
                    dataGridViewM2TeachPoints[1, 3].Value = objPoints.Points[0].Q4;
                    dataGridViewM2TeachPoints[2, 0].Value = objPoints.Points[0].V1;
                    dataGridViewM2TeachPoints[2, 1].Value = objPoints.Points[0].V2;
                    dataGridViewM2TeachPoints[2, 2].Value = objPoints.Points[0].V3;
                    dataGridViewM2TeachPoints[2, 3].Value = objPoints.Points[0].V4;

                }
            }
        }

        private void buttonM2TestSaveProgram_Click(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;

                int p1 = Convert.ToInt32(dataGridViewM2TestPoints[1, 0].Value);
                int p2 = Convert.ToInt32(dataGridViewM2TestPoints[1, 1].Value);
                int p3 = Convert.ToInt32(dataGridViewM2TestPoints[1, 2].Value);
                int p4 = Convert.ToInt32(dataGridViewM2TestPoints[1, 3].Value);
                int s1 = Convert.ToInt32(dataGridViewM2TestPoints[2, 0].Value);
                int s2 = Convert.ToInt32(dataGridViewM2TestPoints[2, 1].Value);
                int s3 = Convert.ToInt32(dataGridViewM2TestPoints[2, 2].Value);
                int s4 = Convert.ToInt32(dataGridViewM2TestPoints[2, 3].Value);
                ConcretePointsContainer<PointAxis> prgObj = new ConcretePointsContainer<PointAxis>(comboBoxM2TestProgramList.Text);
                prgObj.AddPoint(new PointAxis(p1, p2, p3, p4, s1, s2, s3, s4));
                prgObj.Save(comboBoxM2TestProgramList.Text + config.Extensions[0], config.ProgramsPath[1], true);
            }
        }

        private async void buttonM2TestLoadProgram_Click(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");

                string path = Path.Combine(config.ProgramsPath[0], textBoxM2TestProgramName.Text + config.Extensions[0]);
                objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(path);

                if (objPoints != null)
                {

                    dataGridViewM2TestPoints[1, 0].Value = objPoints.Points[0].Q1;
                    dataGridViewM2TestPoints[1, 1].Value = objPoints.Points[0].Q2;
                    dataGridViewM2TestPoints[1, 2].Value = objPoints.Points[0].Q3;
                    dataGridViewM2TestPoints[1, 3].Value = objPoints.Points[0].Q4;
                    dataGridViewM2TestPoints[2, 0].Value = objPoints.Points[0].V1;
                    dataGridViewM2TestPoints[2, 1].Value = objPoints.Points[0].V2;
                    dataGridViewM2TestPoints[2, 2].Value = objPoints.Points[0].V3;
                    dataGridViewM2TestPoints[2, 3].Value = objPoints.Points[0].V4;

                }
            }
        }

        private async void buttonM3TeachLoadProgram_Click(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[2] + "\\" + comboBoxM3TeachProgramList.Text + config.Extensions[0]);
                if (objPoints != null)
                {

                    dataGridViewM3TeachPoints[1, 0].Value = objPoints.Points[0].Q1;
                    dataGridViewM3TeachPoints[1, 1].Value = objPoints.Points[0].Q2;
                    dataGridViewM3TeachPoints[1, 2].Value = objPoints.Points[0].Q3;
                    dataGridViewM3TeachPoints[1, 3].Value = objPoints.Points[0].Q4;
                    dataGridViewM3TeachPoints[2, 0].Value = objPoints.Points[0].V1;
                    dataGridViewM3TeachPoints[2, 1].Value = objPoints.Points[0].V2;
                    dataGridViewM3TeachPoints[2, 2].Value = objPoints.Points[0].V3;
                    dataGridViewM3TeachPoints[2, 3].Value = objPoints.Points[0].V4;

                }
            }
        }

        private void buttonM3TeachSaveProgram_Click(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;

                int p1 = Convert.ToInt32(dataGridViewM3TeachPoints[1, 0].Value);
                int p2 = Convert.ToInt32(dataGridViewM3TeachPoints[1, 1].Value);
                int p3 = Convert.ToInt32(dataGridViewM3TeachPoints[1, 2].Value);
                int p4 = Convert.ToInt32(dataGridViewM3TeachPoints[1, 3].Value);
                int s1 = Convert.ToInt32(dataGridViewM3TeachPoints[2, 0].Value);
                int s2 = Convert.ToInt32(dataGridViewM3TeachPoints[2, 1].Value);
                int s3 = Convert.ToInt32(dataGridViewM3TeachPoints[2, 2].Value);
                int s4 = Convert.ToInt32(dataGridViewM3TeachPoints[2, 3].Value);
                ConcretePointsContainer<PointAxis> prgObj = new ConcretePointsContainer<PointAxis>(comboBoxM3TeachProgramList.Text);
                prgObj.AddPoint(new PointAxis(p1, p2, p3, p4, s1, s2, s3, s4));
                prgObj.Save(comboBoxM3TeachProgramList.Text + config.Extensions[0], config.ProgramsPath[2], true);
            }
        }

        private void buttonM3TeachNewProgram_Click(object sender, EventArgs e)
        {
            //reset points datagrid value to default
            ResetM3Datagrid();
        }

        private void dataGridViewM3TeachPoints_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = 0;
            short[] quote = new short[5];
            short[] speed = new short[5];
            bool[] start = new bool[5] { false, false, false, false, false };


            try
            {
                //get selected row index
                int currentRow = int.Parse(e.RowIndex.ToString());

                short idPoint = (short)(currentRow + 1);
                for (i = 0; i <= dataGridViewM3TeachPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = short.Parse(dataGridViewM3TeachPoints[1, i].Value.ToString());
                    speed[i + 1] = short.Parse(dataGridViewM3TeachPoints[2, i].Value.ToString());
                }

                if (idPoint < 0 || idPoint > 4)
                {
                    //todo message
                    return;
                }

                // register button
                if ((e.ColumnIndex == 3) & currentRow >= 0)
                {
                    //register current axis value
                    dataGridViewM3TeachPoints[1, currentRow].Value = Convert.ToInt32((labelM3TeachAxisQuoteValue.Text));
                    //register current speed value
                    dataGridViewM3TeachPoints[2, currentRow].Value = Convert.ToInt32((numericUpDownM3JogSpeed.Value));
                }

                // start quote button
                if ((e.ColumnIndex == 4) & currentRow >= 0)
                {
                    start[idPoint] = true;
                    //OPCUAM3TeachPckSend(idPoint, quote, speed, start);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async void lbButtonM3JogUp_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            string keyToSend = null;

            if (e.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
            {
                //send quote
                keyToSend = "pcM3JogDown";
                var readResult1 = await ccService.Send(keyToSend, false);
                if (readResult1.OpcResult)
                {
                }

                keyToSend = "pcM3JogUp";
                var readResult2 = await ccService.Send(keyToSend, true);
                if (readResult2.OpcResult)
                {
                }
            }
            else
            {
                keyToSend = "pcM3JogDown";
                var readResult1 = await ccService.Send(keyToSend, false);
                if (readResult1.OpcResult)
                {
                }

                keyToSend = "pcM3JogUp";
                var readResult2 = await ccService.Send(keyToSend, false);
                if (readResult2.OpcResult)
                {
                }
            }
        }

        private async void lbButtonM3JogDown_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            string keyToSend = null;

            if (e.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
            {
                keyToSend = "pcM3JogUp";
                var readResult1 = await ccService.Send(keyToSend, false);
                if (readResult1.OpcResult)
                {
                }

                keyToSend = "pcM3JogDown";
                var readResult2 = await ccService.Send(keyToSend, true);
                if (readResult2.OpcResult)
                {
                }
            }
            else
            {
                keyToSend = "pcM3JogUp";
                var readResult1 = await ccService.Send(keyToSend, false);
                if (readResult1.OpcResult)
                {
                }

                keyToSend = "pcM3JogDown";
                var readResult2 = await ccService.Send(keyToSend, false);
                if (readResult2.OpcResult)
                {
                }
            }
        }

        private async void numericUpDownM3JogSpeed_ValueChanged(object sender, EventArgs e)
        {
            string keyToSend = "pcM3JogSpeed";

            var readResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM2JogSpeed.Value.ToString()));

            if (readResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM3SmallClampOpening_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM3SmallClampOpening";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void buttonM3SmallClampClosing_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM3SmallClampClosing";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void buttonM3BigClampOpening_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM3BigClampOpening";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void buttonM3BigClampClosing_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM3BigClampClosing";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void buttonM3CenteringClampOpening_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM3CentrClampOpening";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void buttonM3CenteringClampClosing_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM3CentrClampClosing";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void buttonM3ContrastOpening_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM3ContrOpening";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void buttonM3ContrastClosing_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM3ContrClosing";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private async void buttonM3ResetServo_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM3ResetServoAlarm";

            var readResult = await ccService.Send(keyToSend, true);

            if (readResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM3PrintCycle_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM3Print";

            var readResult = await ccService.Send(keyToSend, true);
            if (readResult.OpcResult)
            {

            }
        }

        private void lbM3StartStopExitBelt_Click(object sender, EventArgs e)
        {

        }

        private void lbM3StartStopWorkingBelt_Click(object sender, EventArgs e)
        {

        }

        private async void lbM3StartStopWorkingBelt_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            string keyToSend = null;

            keyToSend = "pcM3StartStopWorkingBelt";
            if (lbM3StartStopWorkingBelt.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
            {
                var readResult = await ccService.Send(keyToSend, true);
            }
            else
            {
                var readResult = await ccService.Send(keyToSend, false);
            }
        }

        private async void lbM3StartStopExitBelt_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            string keyToSend = null;

            keyToSend = "pcM3StartStopExitBelt";
            if (lbM3StartStopExitBelt.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
            {
                var readResult = await ccService.Send(keyToSend, true);
            }
            else
            {
                var readResult = await ccService.Send(keyToSend, false);
            }
        }

        private async void buttonM3Home_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM3Homing";

            var readResult = await ccService.Send(keyToSend, true);

            if (readResult.OpcResult)
            {
                //await Task.Run(() => ThreadSafeWriteMessage("Value set"));
            }
            else
            {
                //await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }
        }

        private async void buttonM3ResetHome_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM3ResetHoming";

            var readResult = await ccService.Send(keyToSend, true);

            if (readResult.OpcResult)
            {
                //await Task.Run(() => ThreadSafeWriteMessage("Value set"));
            }
            else
            {
                //await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }
        }

        private async void buttonM3StartQuote_Click(object sender, EventArgs e)
        {
            string keyToSend = null;

            //quote 
            keyToSend = "pcM3ManualQuote";
            var readResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM3ManualQuote.Value.ToString()));
            if (readResult.OpcResult)
            {
                //await Task.Run(() => ThreadSafeWriteMessage("Value set"));
            }
            else
            {
                //await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }

            keyToSend = "pcM3ManualSpeed";
            var readResult1 = await ccService.Send(keyToSend, short.Parse(numericUpDownM3ManualSpeed.Value.ToString()));

            keyToSend = "pcM3QuoteStart";
            var readResult2 = await ccService.Send(keyToSend, true);
            //todo: chi lo mette a false
        }

        private async void numericUpDownM3ManualQuote_ValueChanged(object sender, EventArgs e)
        {
            string keyToSend = "pcM3ManualQuote";

            var readResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM3ManualQuote.Value.ToString()));

            if (readResult.OpcResult)
            {
                //await Task.Run(() => ThreadSafeWriteMessage("Value set"));
            }
            else
            {
                //await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }
        }

        private async void numericUpDownM3ManualSpeed_ValueChanged(object sender, EventArgs e)
        {
            string keyToSend = "pcM3ManualSpeed";

            var readResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM3ManualSpeed.Value.ToString()));

            if (readResult.OpcResult)
            {
                //await Task.Run(() => ThreadSafeWriteMessage("Value set"));
            }
            else
            {
                //await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }
        }

        private async void buttonM3TestLoadProgram_Click(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[2] + "\\" + comboBoxM3TestProgramList.Text + config.Extensions[0]);
                if (objPoints != null)
                {

                    dataGridViewM3TestPoints[1, 0].Value = objPoints.Points[0].Q1;
                    dataGridViewM3TestPoints[1, 1].Value = objPoints.Points[0].Q2;
                    dataGridViewM3TestPoints[1, 2].Value = objPoints.Points[0].Q3;
                    dataGridViewM3TestPoints[1, 3].Value = objPoints.Points[0].Q4;
                    dataGridViewM3TestPoints[2, 0].Value = objPoints.Points[0].V1;
                    dataGridViewM3TestPoints[2, 1].Value = objPoints.Points[0].V2;
                    dataGridViewM3TestPoints[2, 2].Value = objPoints.Points[0].V3;
                    dataGridViewM3TestPoints[2, 3].Value = objPoints.Points[0].V4;

                }
            }
        }

        private async void buttonM3TestSaveProgram_Click(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;

                int p1 = Convert.ToInt32(dataGridViewM3TestPoints[1, 0].Value);
                int p2 = Convert.ToInt32(dataGridViewM3TestPoints[1, 1].Value);
                int p3 = Convert.ToInt32(dataGridViewM3TestPoints[1, 2].Value);
                int p4 = Convert.ToInt32(dataGridViewM3TestPoints[1, 3].Value);
                int s1 = Convert.ToInt32(dataGridViewM3TestPoints[2, 0].Value);
                int s2 = Convert.ToInt32(dataGridViewM3TestPoints[2, 1].Value);
                int s3 = Convert.ToInt32(dataGridViewM3TestPoints[2, 2].Value);
                int s4 = Convert.ToInt32(dataGridViewM3TestPoints[2, 3].Value);
                ConcretePointsContainer<PointAxis> prgObj = new ConcretePointsContainer<PointAxis>(comboBoxM3TestProgramList.Text);
                prgObj.AddPoint(new PointAxis(p1, p2, p3, p4, s1, s2, s3, s4));
                prgObj.Save(comboBoxM3TestProgramList.Text + config.Extensions[0], config.ProgramsPath[2], true);
            }
        }

        private async void lbButtonM3StartTest_Click(object sender, EventArgs e)
        {
            string[] parsed = null;
            int type = -1;
            parsed = comboBoxM3TestProgramList.Text.ToString().Split('-');
            string keyToSend = "pcM3TestType";
            if (parsed[2] == "DX")
            {
                type = 1;

            }
            else
            {
                type = 2;

            }

            var readResult = await ccService.Send(keyToSend, type);

            M3TestSendProgram();

            keyToSend = "pcM3StartTest";
            readResult = await ccService.Send(keyToSend, true);
        }

        private void comboBoxM3TeachProgramList_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void comboBoxM3TeachModelName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                List<IObjProgram> pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, comboBoxM3TeachModelName.Text);

                comboBoxM3TeachProgramList.Items.Clear();

                comboBoxM3TestProgramList.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM3TeachProgramList.Items.Add(prgName.ProgramName);
                    comboBoxM3TestProgramList.Items.Add(prgName.ProgramName);
                }
            }
        }

        private async void comboBoxM4PrgName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //send recipe (todo: waiting mysql)
            string keyValue = "pcM4Param1";
            var sendResult = await ccService.Send(keyValue, short.Parse(textBoxM4Test.Text));
            labelM4Param1Value.Text = textBoxM4Test.Text;

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

            }
            else
            {

            }
        }

        private void comboBoxM2TeachModelName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                List<IObjProgram> pList = progRS.GetProgram(config.ProgramsPath[1], config.Extensions, comboBoxM2TeachModelName.Text);

                comboBoxM2TeachProgramList.Items.Clear();
                comboBoxM2TestProgramList.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM2TeachProgramList.Items.Add(prgName.ProgramName);
                    comboBoxM2TestProgramList.Items.Add(prgName.ProgramName);
                }
            }
        }

        private void comboBoxAutoModelNameLst_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            List<IObjProgram> pList = new List<IObjProgram>();

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                pList = progRS.GetProgram(config.ProgramsPath[0], config.Extensions, comboBoxAutoModelNameLst.Text);
                comboBoxM1PrgName.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM1PrgName.Items.Add(prgName.ProgramName);
                }

                pList = progRS.GetProgram(config.ProgramsPath[1], config.Extensions, comboBoxAutoModelNameLst.Text);
                comboBoxM2PrgName.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM2PrgName.Items.Add(prgName.ProgramName);
                }

                pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, comboBoxAutoModelNameLst.Text);
                comboBoxM3PrgName_st1.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM3PrgName_st1.Items.Add(prgName.ProgramName);
                }

                pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, comboBoxAutoModelNameLst.Text);
                comboBoxM3PrgName_st2.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM3PrgName_st2.Items.Add(prgName.ProgramName);
                }

                //da rimuovere
                pList = progRS.GetProgram(config.ProgramsPath[1], config.Extensions, comboBoxAutoModelNameLst.Text);
                comboBoxM4PrgName.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM4PrgName.Items.Add(prgName.ProgramName);
                }
            }
        }

        private async void comboBoxM1PrgName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //send recipe (todo: waiting mysql)
            string keyValue = "pcM1Param1";
            var sendResult = await ccService.Send(keyValue, short.Parse(textBoxM1Test.Text));
            labelM1Param1Value.Text = textBoxM1Test.Text;

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
                        new short[] { (short)objPoints.Points[0].Q1, (short)objPoints.Points[0].Q2, (short)objPoints.Points[0].Q3, (short)objPoints.Points[0].Q4},
                        new short[] { (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                    };

                    var mSendResult = await ccService.Send(keys, values);
                }
            }
        }

        private async void comboBoxM2PrgName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //send recipe (todo: waiting mysql)
            string keyValue = "pcM2Param1";
            var sendResult = await ccService.Send(keyValue, short.Parse(textBoxM2Test.Text));
            labelM2Param1Value.Text = textBoxM2Test.Text;

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
                        new short[] {0, (short)objPoints.Points[0].Q1, (short)objPoints.Points[0].Q2, (short)objPoints.Points[0].Q3, (short)objPoints.Points[0].Q4},
                        new short[] {0, (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                    };


                    var readResult = await ccService.Send(keys, values);
                }
            }
        }

        private async void comboBoxM3PrgName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //send recipe
            string keyValue = "pcM3Param1";
            var sendResult = await ccService.Send(keyValue, short.Parse(textBoxM3Test.Text));

            //send type order: RG, LF
            //type order 2: LF, RG
            keyValue = "pcM3TypeOrder";
            sendResult = await ccService.Send(keyValue, short.Parse(textBoxTypeOrder.Text));


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
                        new short[] {0, (short)objPoints.Points[0].Q1, (short)objPoints.Points[0].Q2, (short)objPoints.Points[0].Q3, (short)objPoints.Points[0].Q4},
                        new short[] {0, (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                    };

                    var readResult = await ccService.Send(keys, values);
                }
            }
        }

        private async void comboBoxM3PrgName_st2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //send recipe
            string keyValue = "pcM3Param1";
            var sendResult = await ccService.Send(keyValue, short.Parse(textBoxM3Test.Text));
            labelM3Param1Value.Text = textBoxM3Test.Text;
            keyValue = "pcM3TypeOrder";
            sendResult = await ccService.Send(keyValue, short.Parse(textBoxTypeOrder.Text));
            labelM3Param1Value.Text = textBoxM3Test.Text;
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
                        new short[] {0, (short)objPoints.Points[0].Q1, (short)objPoints.Points[0].Q2, (short)objPoints.Points[0].Q3, (short)objPoints.Points[0].Q4},
                        new short[] {0,  (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                    };

                    var readResult = await ccService.Send(keys, values);
                }
            }
        }      

        private async void buttonM2Reset_Click(object sender, EventArgs e)
        {
            string keyValue = "pcM2Reset";
            var sendResult = await ccService.Send(keyValue, true);

            if ((sendResult == null) || (sendResult.OpcResult == false))
            {
                //todo manage error

            }            
        }

        private void tabPageT0_3_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            Pen myPen = new Pen(Color.FromArgb(107, 227, 162));
            myPen.Width = 10;


            int lineHeight = 80;
            int groupboxXShift = groupBoxKeepAliveFromPlc.Location.X;
            int groupboxYShift = groupBoxKeepAliveFromPlc.Location.Y;
            g.DrawLine(myPen, pictureBoxM1PLCNode.Location.X + pictureBoxM1PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM1PLCNode.Location.Y - pictureBoxM1PLCNode.Height + groupboxYShift, pictureBoxM1PLCNode.Location.X + pictureBoxM1PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM1PLCNode.Location.Y - pictureBoxM1PLCNode.Height + lineHeight + groupboxYShift);
            g.DrawLine(myPen, pictureBoxM2PLCNode.Location.X + pictureBoxM2PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM2PLCNode.Location.Y - pictureBoxM2PLCNode.Height + groupboxYShift, pictureBoxM2PLCNode.Location.X + pictureBoxM2PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM2PLCNode.Location.Y - pictureBoxM2PLCNode.Height + lineHeight + groupboxYShift);
            g.DrawLine(myPen, pictureBoxM3PLCNode.Location.X + pictureBoxM3PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM3PLCNode.Location.Y - pictureBoxM3PLCNode.Height + groupboxYShift, pictureBoxM3PLCNode.Location.X + pictureBoxM3PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM3PLCNode.Location.Y - pictureBoxM3PLCNode.Height + lineHeight + groupboxYShift);
            g.DrawLine(myPen, pictureBoxM4PLCNode.Location.X + pictureBoxM4PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM4PLCNode.Location.Y - pictureBoxM4PLCNode.Height + groupboxYShift, pictureBoxM4PLCNode.Location.X + pictureBoxM4PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM4PLCNode.Location.Y - pictureBoxM4PLCNode.Height + lineHeight + groupboxYShift);
            g.DrawLine(myPen, pictureBoxM5PLCNode.Location.X + pictureBoxM5PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM5PLCNode.Location.Y - pictureBoxM5PLCNode.Height + groupboxYShift, pictureBoxM5PLCNode.Location.X + pictureBoxM5PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM5PLCNode.Location.Y - pictureBoxM5PLCNode.Height + lineHeight + groupboxYShift);

            g.DrawLine(myPen, pictureBoxM1PLCNode.Location.X + groupboxXShift, pictureBoxM1PLCNode.Location.Y - lineHeight + groupboxYShift, pictureBoxM5PLCNode.Location.X + pictureBoxM1PLCNode.Width + groupboxXShift, pictureBoxM1PLCNode.Location.Y - lineHeight + groupboxYShift);
            g.DrawLine(myPen, pictureBoxIOTNode.Location.X + pictureBoxIOTNode.Width / 2 + groupboxXShift, pictureBoxIOTNode.Location.Y + pictureBoxIOTNode.Height + groupboxYShift, pictureBoxIOTNode.Location.X + pictureBoxIOTNode.Width / 2 + groupboxXShift, pictureBoxIOTNode.Location.Y + pictureBoxIOTNode.Height + lineHeight - 10 + groupboxYShift);
        }

        private async void buttonMReset_Click(object sender, EventArgs e)
        {
            //reset only if line is in stop or pause
            if (checkBoxStartStop.CheckState == CheckState.Unchecked || checkBoxPause.CheckState == CheckState.Checked)
            {
                string keyValue = "pcM1Reset";
                var sendResult = await ccService.Send(keyValue, true);

                keyValue = "pcM2Reset";
                sendResult = await ccService.Send(keyValue, true);

                keyValue = "pcM3Reset";
                sendResult = await ccService.Send(keyValue, true);

                keyValue = "pcM4Reset";
                sendResult = await ccService.Send(keyValue, true);

                keyValue = "pcM5Reset";
                sendResult = await ccService.Send(keyValue, true);

                keyValue = "pcM6Reset";
                sendResult = await ccService.Send(keyValue, true);
            }
            else
            {
                //manage error message
            }
        }

   
        private async void checkBoxM2Start_CheckStateChanged(object sender, EventArgs e)
        {
            bool state = false;
            //send start/stop 
            state = (checkBoxM2Start.CheckState == CheckState.Checked) ? true : false;
            string keyValue = "pcM2StartStop";
            var sendResult = await ccService.Send(keyValue, state);
            if (sendResult.OpcResult)
            {
                //update gui

                checkBoxM2Start.Text = "";
                if (state) checkBoxM2Start.Image = imageListStart.Images[1];
                else checkBoxM2Start.Image = imageListStart.Images[0];
            }
        }

        private async void checkBoxM1Start_CheckStateChanged(object sender, EventArgs e)
        {
            bool state = false;
            //send start/stop 
            state = (checkBoxM1Start.CheckState == CheckState.Checked) ? true : false;
            string keyValue = "pcM1StartStop";
            var sendResult = await ccService.Send(keyValue, state);
            if (sendResult.OpcResult)
            {
                //update gui

                checkBoxM1Start.Text = "";
                if (state) checkBoxM1Start.Image = imageListStart.Images[1];
                else checkBoxM1Start.Image = imageListStart.Images[0];
            }
        }

        private async void checkBoxM3Start_CheckStateChanged(object sender, EventArgs e)
        {
            bool state = false;
            //send start/stop 
            state = (checkBoxM3Start.CheckState == CheckState.Checked) ? true : false;
            string keyValue = "pcM3StartStop";
            var sendResult = await ccService.Send(keyValue, state);
            if (sendResult.OpcResult)
            {
                //update gui

                checkBoxM3Start.Text = "";
                if (state) checkBoxM3Start.Image = imageListStart.Images[1];
                else checkBoxM3Start.Image = imageListStart.Images[0];
            }
        }

        private async void checkBoxM4Start_CheckStateChanged(object sender, EventArgs e)
        {
            bool state = false;
            //send start/stop 
            state = (checkBoxM4Start.CheckState == CheckState.Checked) ? true : false;
            string keyValue = "pcM4StartStop";
            var sendResult = await ccService.Send(keyValue, state);
            if (sendResult.OpcResult)
            {
                //update gui

                checkBoxM4Start.Text = "";
                if (state) checkBoxM4Start.Image = imageListStart.Images[1];
                else checkBoxM4Start.Image = imageListStart.Images[0];
            }
        }

        private async void checkBoxM5Start_CheckStateChanged(object sender, EventArgs e)
        {
            bool state = false;
            //send start/stop 
            state = (checkBoxM5Start.CheckState == CheckState.Checked) ? true : false;
            string keyValue = "pcM5StartStop";
            var sendResult = await ccService.Send(keyValue, state);
            if (sendResult.OpcResult)
            {
                //update gui

                checkBoxM5Start.Text = "";
                if (state) checkBoxM5Start.Image = imageListStart.Images[1];
                else checkBoxM5Start.Image = imageListStart.Images[0];
            }
        }

        private async void checkBoxM6Start_CheckStateChanged(object sender, EventArgs e)
        {
            bool state = false;
            //send start/stop 
            state = (checkBoxM6Start.CheckState == CheckState.Checked) ? true : false;
            string keyValue = "pcM6StartStop";
            var sendResult = await ccService.Send(keyValue, state);
            if (sendResult.OpcResult)
            {
                //update gui

                checkBoxM6Start.Text = "";
                if (state) checkBoxM6Start.Image = imageListStart.Images[1];
                else checkBoxM6Start.Image = imageListStart.Images[0];
            }
        }

        private async void checkBoxM2Pause_CheckStateChanged(object sender, EventArgs e)
        {
            bool state = false;
            //send start/stop 
            state = (checkBoxM2Pause.CheckState == CheckState.Checked) ? true : false;
            string keyValue = "pcM2Pause";
            var sendResult = await ccService.Send(keyValue, state);
            if (sendResult.OpcResult)
            {
                //update gui

                checkBoxM2Pause.Text = "";
                if (state) checkBoxM2Pause.Image = imageListStart.Images[3];
                else checkBoxM2Pause.Image = imageListStart.Images[2];
            }
        }

        private async void checkBoxStartStop_CheckStateChanged(object sender, EventArgs e)
        {
            bool state = false;
            string keyValue = "";
            //send start/stop 
            state = (checkBoxStartStop.CheckState == CheckState.Checked) ? true : false;

            if (state)
            {
                keyValue = "pcM1StartStop";
                var readResult = await ccService.Send(keyValue, true);
                keyValue = "pcM2StartStop";
                readResult = await ccService.Send(keyValue, true);
                keyValue = "pcM3StartStop";
                readResult = await ccService.Send(keyValue, true);
                keyValue = "pcM4StartStop";
                readResult = await ccService.Send(keyValue, true);
                keyValue = "pcM5StartStop";
                readResult = await ccService.Send(keyValue, true);
                keyValue = "pcM6StartStop";
                readResult = await ccService.Send(keyValue, true);
            }
            else
            {
                keyValue = "pcM1StartStop";
                var readResult = await ccService.Send(keyValue, false);
                keyValue = "pcM2StartStop";
                readResult = await ccService.Send(keyValue, false);
                keyValue = "pcM3StartStop";
                readResult = await ccService.Send(keyValue, false);
                keyValue = "pcM4StartStop";
                readResult = await ccService.Send(keyValue, false);
                keyValue = "pcM5StartStop";
                readResult = await ccService.Send(keyValue, false);
                keyValue = "pcM6StartStop";
                readResult = await ccService.Send(keyValue, false);
            }

            if (state)
            {                
                checkBoxStartStop.Image = imageListStartStop.Images[1];
                checkBoxStartStop.Text = "STOP";
            }
            else
            {                
                checkBoxStartStop.Image = imageListStartStop.Images[0];
                checkBoxStartStop.Text = "START";
            }
        }

        private async void checkBoxPause_CheckStateChanged(object sender, EventArgs e)
        {
            bool state = false;
            string keyValue = "";
            //send start/stop 
            state = (checkBoxPause.CheckState == CheckState.Checked) ? true : false;

            if (state)
            {
                keyValue = "pcM1Pause";
                var readResult = await ccService.Send(keyValue, true);
                keyValue = "pcM2Pause";
                readResult = await ccService.Send(keyValue, true);
                keyValue = "pcM3Pause";
                readResult = await ccService.Send(keyValue, true);
                keyValue = "pcM4Pause";
                readResult = await ccService.Send(keyValue, true);
                keyValue = "pcM5Pause";
                readResult = await ccService.Send(keyValue, true);
                keyValue = "pcM6Pause";
                readResult = await ccService.Send(keyValue, true);

                checkBoxPause.Image = imageListStartStop.Images[3];
                checkBoxPause.Text = "IN PAUSE";
            }
            else
            {
                keyValue = "pcM1Pause";
                var readResult = await ccService.Send(keyValue, false);
                keyValue = "pcM2Pause";
                readResult = await ccService.Send(keyValue, false);
                keyValue = "pcM3Pause";
                readResult = await ccService.Send(keyValue, false);
                keyValue = "pcM4Pause";
                readResult = await ccService.Send(keyValue, false);
                keyValue = "pcM5Pause";
                readResult = await ccService.Send(keyValue, false);
                keyValue = "pcM6Pause";
                readResult = await ccService.Send(keyValue, false);
                checkBoxPause.Image = imageListStartStop.Images[2];
                checkBoxPause.Text = "PAUSE";
            }
        }

        private async void checkBoxM3Pause_CheckStateChanged(object sender, EventArgs e)
        {
            bool state = false;
            //send start/stop 
            state = (checkBoxM3Pause.CheckState == CheckState.Checked) ? true : false;
            string keyValue = "pcM3Pause";
            var sendResult = await ccService.Send(keyValue, state);
            if (sendResult.OpcResult)
            {
                //update gui

                checkBoxM3Pause.Text = "";
                if (state) checkBoxM3Pause.Image = imageListStart.Images[4];
                else checkBoxM3Pause.Image = imageListStart.Images[3];
            }
        }

        private async void checkBoxM6Pause_CheckStateChanged(object sender, EventArgs e)
        {
            bool state = false;
            //send start/stop 
            state = (checkBoxM6Pause.CheckState == CheckState.Checked) ? true : false;
            string keyValue = "pcM6Pause";
            var sendResult = await ccService.Send(keyValue, state);
            if (sendResult.OpcResult)
            {
                //update gui

                checkBoxM6Pause.Text = "";
                if (state) checkBoxM6Pause.Image = imageListStart.Images[4];
                else checkBoxM6Pause.Image = imageListStart.Images[3];
            }
        }

        private async void checkBoxM5Pause_CheckStateChanged(object sender, EventArgs e)
        {
            bool state = false;
            //send start/stop 
            state = (checkBoxM5Pause.CheckState == CheckState.Checked) ? true : false;
            string keyValue = "pcM5Pause";
            var sendResult = await ccService.Send(keyValue, state);
            if (sendResult.OpcResult)
            {
                //update gui

                checkBoxM5Pause.Text = "";
                if (state) checkBoxM5Pause.Image = imageListStart.Images[4];
                else checkBoxM5Pause.Image = imageListStart.Images[3];
            }
        }

        private async void checkBoxM4Pause_CheckStateChanged(object sender, EventArgs e)
        {
            bool state = false;
            //send start/stop 
            state = (checkBoxM4Pause.CheckState == CheckState.Checked) ? true : false;
            string keyValue = "pcM4Pause";
            var sendResult = await ccService.Send(keyValue, state);
            if (sendResult.OpcResult)
            {
                //update gui

                checkBoxM4Pause.Text = "";
                if (state) checkBoxM4Pause.Image = imageListStart.Images[4];
                else checkBoxM4Pause.Image = imageListStart.Images[3];
            }
        }

        private async void checkBoxM1Pause_CheckStateChanged(object sender, EventArgs e)
        {
            bool state = false;
            //send start/stop 
            state = (checkBoxM1Pause.CheckState == CheckState.Checked) ? true : false;
            string keyValue = "pcM1Pause";
            var sendResult = await ccService.Send(keyValue, state);
            if (sendResult.OpcResult)
            {
                //update gui

                checkBoxM1Pause.Text = "";
                if (state) checkBoxM1Pause.Image = imageListStart.Images[4];
                else checkBoxM1Pause.Image = imageListStart.Images[3];
            }
        }

        private async void checkBoxM2Inclusion_CheckStateChanged(object sender, EventArgs e)
        {
            string keyToSend = null;
            bool chkValue = false;

            checkBoxM2Inclusion.Text = ""; //todo: to remove once initialized
            keyToSend = "pcM2Inclusion";
            chkValue = (checkBoxM2Inclusion.CheckState == CheckState.Checked) ? true : false;

            var sendResult = await ccService.Send(keyToSend, chkValue);
            if (sendResult.OpcResult)
            {
                checkBoxM2Inclusion.BackgroundImage = (chkValue) ? imageListMIncEsc.Images[0] : imageListMIncEsc.Images[1];
            }
            else checkBoxM2Inclusion.BackgroundImage = imageListMIncEsc.Images[2];
        }

        private void tabPageConsole_Paint(object sender, PaintEventArgs e)
        {

        }
    
    }
}
