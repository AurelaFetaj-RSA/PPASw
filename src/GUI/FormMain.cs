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
            StartDiagnosticGUI();
            _splashScreen?.WriteOnTextboxAsync($"Loaded Diagnostic File");

            //Splash Screen filler
            _splashScreen?.WriteOnTextboxAsync($"Update GUI syncrozionation Thread Started");

            tabControlMain.SelectedPage = tabPageMain;
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

            //.Run(async () => await UpdateGraphicsGUI(TimeSpan.FromMilliseconds(Settings.Default.UpdateGUILed), _cancellationTokenSource));
            //Task.Run(async () => await UpdateDiagnosticGUI(TimeSpan.FromMilliseconds(myCore.DiagnosticConfigurator.Configuration.DiagnosticPolling), _cancellationTokenSource));
            Task.Run(async () => await UpdateOPCUAStatus(TimeSpan.FromMilliseconds(1000), _cancellationTokenSource));

        }


        private void RSACustomEvents_ServiceDisconnectionEvent(object sender, RSACustomEvents.ServiceConnectionEventArgs e)
        {
            if (e.Service is IRobot<KawasakiMemoryVariable>)
            {
                lbLedRobotConnection.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            }
            else if (e.Service is OpcServerService opcService) //In questo caso no nfaccio niente perchè non ho modo, solo Subscription ( per ora )
            {
                lbLedRobotConnection.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
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
                lbLedRobotConnection.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On;
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
                lbLedMESConnection.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On;
            }

        }

        private void RSACustomEvents_KeepAliveTimeoutEvent(object sender, RSACustomEvents.KeepAliveTimeOutEventArgs e)
        {

            if (e.Service is OpcServerService opcService)
            {
                lbLedMESConnection.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            }

        }

        private void RSACustomEvents_OPCSubscriptionEvent(object sender, RSACustomEvents.OPCSubscriptionEventArgs e)
        {
            if (e.Service is OpcServerService opcService)
            {
                lbLedMESConnection.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On;
            }

        }

        private void RSACustomEvents_AckTimeoutEvent(object sender, RSACustomEvents.TimeoutEventArgs e)
        {

            if (e.Service is OpcServerService)
            {
                if (e.Result == MessageResponse.Success)
                {
                    handshakeMESLed.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Blink;
                }
                else
                {
                    handshakeMESLed.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                }
            }

        }

        private async void InitCore()
        {
            myCore = new Core("PlasticCore");
            myCore.LoadConfiguration(myCore.ConfigFile);

            _splashScreen?.WriteOnTextboxAsync($"Init Core Configuration");

            string logName = $"Log\\{DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")}.log";
            LoggerConfigurator loadedloggerConfigurator = new LoggerConfigurator("LoggerConfigurations.json").Load().SetAllLogName(logName).Save();

            myCore.AddScoped<Diagnostic.Core.Diagnostic>();
            myCore.AddScoped<OpcClientService>();
            myCore.AddScoped<ReadProgramsService>();
            //OpcClientConfiguration Config = new OpcClientConfiguration()
            //{
            //    ServiceName = "OpcClient",
            //    Active = true,
            //    Host = "192.168.0.38",
            //    Port = 48011,
            //    DefaultKeepAliveFutureValueExpected = 0,
            //    DefaultKeepAliveFutureValueToSet = 1,
            //    Scheme = "opc.tcp",
            //    TimeoutMilliseconds = 5000,
            //    DisconnectionTimeoutMilliseconds = 5000
            //};

            //CoreConfigurations newConfiguration = new CoreConfigurations();
            //newConfiguration.ServiceConfigurations.Add(Config);

            //myCore.AddScoped<RSACommon.Service.OpcClientService>();
            //myCore.CreateServiceList(newConfiguration, null);
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

            if(dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                    RSACustomEvents.ServiceHasLoadProgramEvent += RSACustomEvents_ServiceHasLoadProgramEvent;

                    StandardProgramParser standardParser = new StandardProgramParser();
                    progRS.SetProgramParser(standardParser);
                    await progRS.LoadProgramAsync<PointAxis>();

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
            if(e.Service is ReadProgramsService progRS)
            {
                modelCombobox.Items.Clear();
                modelCombobox.Items.AddRange(progRS.ModelDictionary.Keys.ToArray<string>());
            }
        }

        private void InitLastParameter()
        {
            //filePathDiagnosticFileTxtbox.Text = Settings.Default.DiagnosticFilePath;
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
                _configForm = new ServiceSetup();

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
                //await Task.Run(() => ThreadSafeWriteMessage("Value set"));
            }
            else
            {
                //await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
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

        private async void checkBoxM2Inclusion_CheckedChanged(object sender, EventArgs e)
        {
            string keyToSend = null;

            keyToSend = "pcM2Inclusion";
            if (checkBoxM2Inclusion.CheckState == CheckState.Checked)
            {
                var readResult = await ccService.Send(keyToSend, true);
            }
            else
            {
                var readResult = await ccService.Send(keyToSend, false);
            }
        }

        private async void checkBoxM1Inclusion_CheckedChanged(object sender, EventArgs e)
        {
            string keyToSend = null;

            keyToSend = "pcM1Inclusion";
            if (checkBoxM2Inclusion.CheckState == CheckState.Checked)
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
            if (checkBoxM2Inclusion.CheckState == CheckState.Checked)
            {
                var readResult = await ccService.Send(keyToSend, true);
            }
            else
            {
                var readResult = await ccService.Send(keyToSend, false);
            }
        }

        private async void checkBoxM4Inclusion_CheckedChanged(object sender, EventArgs e)
        {
            string keyToSend = null;

            keyToSend = "pcM4Inclusion";
            if (checkBoxM2Inclusion.CheckState == CheckState.Checked)
            {
                var readResult = await ccService.Send(keyToSend, true);
            }
            else
            {
                var readResult = await ccService.Send(keyToSend, false);
            }
        }

        private async void checkBoxM5Inclusion_CheckedChanged(object sender, EventArgs e)
        {
            string keyToSend = null;

            keyToSend = "pcM5Inclusion";
            if (checkBoxM2Inclusion.CheckState == CheckState.Checked)
            {
                var readResult = await ccService.Send(keyToSend, true);
            }
            else
            {
                var readResult = await ccService.Send(keyToSend, false);
            }
        }

        private async void checkBoxM6Inclusion_CheckedChanged(object sender, EventArgs e)
        {
            string keyToSend = null;

            keyToSend = "pcM6Inclusion";
            if (checkBoxM2Inclusion.CheckState == CheckState.Checked)
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
            bool[] reg = new bool[5] { false, false, false, false, false };


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
                    reg[idPoint] = true;
                    OPCUAM2TeachPckSend(idPoint, quote, speed, reg);
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

        private void buttonM2LoadTestProgram_Click(object sender, EventArgs e)
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
            catch(Exception ex)
            {

            }
        }

        private async void lbButtonM2StartTest_Click(object sender, EventArgs e)
        {            
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
    }
}
