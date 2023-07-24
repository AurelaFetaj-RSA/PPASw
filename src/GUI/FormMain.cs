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

            foreach (var service in listOfService)
            {
                _splashScreen?.WriteOnTextboxAsync($"Service: {service.Name} loaded");
            }

            ccService = (OpcClientService)myCore.FindPerType(typeof(OpcClientService));

            if(ccService != null) 
            {
                ccService.SetObjectData(new PlasticOpcClientConfig().Config());
            }

            if (await ccService.Connect())
            {

            }

            _splashScreen?.WriteOnTextboxAsync($"Core Configuration ended");
            _splashScreen?.WriteOnTextboxAsync($"Core Started");
        }

        private void InitLastParameter()
        {
            //filePathDiagnosticFileTxtbox.Text = Settings.Default.DiagnosticFilePath;
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

        private async void Connect()
        {
            

            bool connectionResult = await ccService.Connect();

            if (connectionResult)
            {
                //await Task.Run(() => ThreadSafeWriteMessage("Connected"));

            }
            else
            {
                //await Task.Run(() => ThreadSafeWritkeMessage("Failed to connect"));
            }
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
                OpcClientService clientService = (OpcClientService)myCore.FindPerType(typeof(OpcClientService));
                _clientForm = new ClientTest(clientService);
            }

            _clientForm.Show();
            _clientForm.Activate();

        }

        private async void buttonM2SmallClampOpening_Click(object sender, EventArgs e)
        {
            string keyToSend = "pc_open_small_clamp";
            string keyToSend1 = "pc_chiusura_pinza_bordo_stivale";

            
            var readResult1 = await ccService.Send(keyToSend1, false);
            var readResult = await ccService.Send(keyToSend, true);       
            
        }

        private async void buttonM2SmallClampClosing_Click(object sender, EventArgs e)
        {
            string keyToSend = "pc_chiusura_pinza_bordo_stivale";
            string keyToSend1 = "pc_apertura_pinza_bordo_stivale";

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var readResult1 = await ccService.Send(keyToSend1, false);

            if (readResult1.OpcResult)
            {
                sw.Stop();
                myCore.Log.Debug(sw.ElapsedMilliseconds);
            }
            var readResult = await ccService.Send(keyToSend, true);            
        }

        private async void lbButtonM2JogUp_Click(object sender, EventArgs e)
        {
            string keyToSend = null;

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

        private async void lbButtonM2JogDown_Click(object sender, EventArgs e)
        {
            string keyToSend = null;

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

            keyToSend = "pcM2QuoteStart";
            var readResult = await ccService.Send(keyToSend, true);
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
    }
}
