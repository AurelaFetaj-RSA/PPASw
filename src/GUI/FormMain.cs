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

namespace GUI
{
    public partial class FormMain : Form
    {
        //core instance
        public static Core myCore;
        RSWareUser myRSAUser;
        public static WebApiSharedList sharedLst;
        IRobot<IRobotVariable> myRobot;
        private LidorSystems.IntegralUI.Containers.TabPage lastPage { get; set; } = null;
        private readonly SplashScreen _splashScreen = null;
        private Form _configForm { get; set; } = null;

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
            myCore.Start();
        }

        public void SetEvent()
        {
            RSACustomEvents.OPCServerSubscriptionEvent += RSACustomEvents_OPCSubscriptionEvent;
            RSACustomEvents.KeepAliveTimeoutEvent += RSACustomEvents_KeepAliveTimeoutEvent;
            RSACustomEvents.ServiceConnectionEvent += RSACustomEvents_ServiceConnectionEvent;
            RSACustomEvents.KeepAliveOkEvent += RSACustomEvents_KeepAliveOkEvent;
            RSACustomEvents.ServiceDisconnectionEvent += RSACustomEvents_ServiceDisconnectionEvent;
            RSACustomEvents.AckTimeoutEvent += RSACustomEvents_AckTimeoutEvent;

        }

        public void StartUpdateTask()
        {

            //.Run(async () => await UpdateGraphicsGUI(TimeSpan.FromMilliseconds(Settings.Default.UpdateGUILed), _cancellationTokenSource));
            //Task.Run(async () => await UpdateDiagnosticGUI(TimeSpan.FromMilliseconds(myCore.DiagnosticConfigurator.Configuration.DiagnosticPolling), _cancellationTokenSource));
            //Task.Run(async () => await UpdateRobutStatus(TimeSpan.FromMilliseconds(1000), _cancellationTokenSource));
            
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

        private void InitCore()
        {
            myCore = new Core("PlasticCore");
            myCore.LoadConfiguration(myCore.ConfigFile);

            _splashScreen?.WriteOnTextboxAsync($"Init Core Configuration");

            string logName = $"Log\\{DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")}.log";
            LoggerConfigurator loadedloggerConfigurator = new LoggerConfigurator("LoggerConfigurations.json").Load().SetAllLogName(logName).Save();

            myCore.AddScoped<Diagnostic.Core.Diagnostic>();
            myCore.AddScoped<OpcClientService>();

            var listOfService = myCore.CreateServiceList(myCore.CoreConfigurations, loadedloggerConfigurator);

            foreach(var service in listOfService)
            {
                _splashScreen?.WriteOnTextboxAsync($"Service: {service.Name} loaded");
            }

            /*
            myRSAUser = new RSWareUser()
            {
                Ip = "",
                Name = "RSWare",
                FolderPath = Settings.Default.RSAUserRecipePath
            };
            */

            InitServices();
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

            InitRobotConsoleDT();
            InitRSWareUserConsoleDT();
            InitOpenUAConsoleDT();
        }

        public static void LinkEventManager(IServerShared toLink)
        {
            if (toLink is WebApiRSWareSharedClass webApiSharedClass)
            {
                webApiSharedClass.OnAckChangeValueEvent += WebApiSharedClass_OnAckChangeValueEvent;
                webApiSharedClass.OnErrorChangeValueEvent += WebApiSharedClass_OnErrorChangeValueEvent;
                webApiSharedClass.OnCommandReadEvent += WebApiSharedClass_OnCommandReadEvent;
            }
        }


        private static void WebApiSharedClass_OnAckChangeValueEvent(object sender, RSACommon.WebApiDefinitions.AckChangeEventArgs e)
        {
            myCore.Log?.Info($"{e.User}{e.AckValue}");
        }

        private static void WebApiSharedClass_OnErrorChangeValueEvent(object sender, RSACommon.WebApiDefinitions.ErrorEventArgs e)
        {
            myCore.Log?.Info($"{e.User}{e.EventError}");            
            //AddMessageToDT(e.User, e.EventError, dataGridViewRSWareUserConsole);
        }

        private static void WebApiSharedClass_OnCommandReadEvent(object sender, CommandRequestedEventArgs e)
        {
            myCore.Log?.Info($"{e.User}{e.Command.CommandString}");            
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

        private void buttonSendSetAckUserConsole_Click(object sender, EventArgs e)
        {
            IServerShared webAPI = myCore.ApiSharedList.GetWebSharedUserInstance(myRSAUser);
            webAPI.AckStatus = ACK.Ok;
        }
 
        private void buttonSendResetAckUserConsole_Click(object sender, EventArgs e)
        {
            IServerShared webAPI = myCore.ApiSharedList.GetWebSharedUserInstance(myRSAUser);
            webAPI.AckStatus = ACK.Free;
        }
        #endregion

        private void buttonSendCommandUserConsole_Click(object sender, EventArgs e)
        {
            //getcommandid
            //sulla base dell'id instanzio l'oggetto, rif. RecipeCommand
            //ID combobox = 0 CSTAA
            //new 
            switch (comboBoxIDCommandUserConsole.SelectedIndex)
            {
                case 0:
                    myRSAUser.MakeCommand(new StartAutomaticCommand(), "");
                    break;
                case 1:
                    myRSAUser.MakeCommand(new StopAutomaticCommand(), "");
                    break;
                case 2:
                    myRSAUser.MakeCommand(new ParkingCommand(), "");
                    break;
                case 3:
                    myRSAUser.MakeCommand(new RecipeCommand(), textBoxCommandParametersUserConsole.Text);
                    break;                
                default:
                    break;
            }
            
        }

        private void buttonSendCommandRobotConsole_Click(object sender, EventArgs e)
        {
            string strRet = "";

            myRobot.SetCommand(textBoxCommandRobotConsole.Text, ref strRet);
            AddMessageToDT(textBoxCommandRobotConsole.Text, strRet, dataGridViewRobotConsole);
        }

        private void tabControlMain_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                if(lastPage != null)
                    tabControlMain.SelectedPage = lastPage;
                else
                    tabControlMain.SelectedPage = tabPageMain;
            }
        }
        private void diagnosticSetupBtn_Click(object sender, EventArgs e)
        {

        }

        private void diagnosticSetupBtn_Click_1(object sender, EventArgs e)
        {
            //if(File.Exists(filePathDiagnosticFileTxtbox.Text))
            //{
            //    Properties.Settings.Default.DiagnosticFilePath = filePathDiagnosticFileTxtbox.Text;
            //    Properties.Settings.Default.Save();

            //    StartDiagnosticGUI(Properties.Settings.Default.DiagnosticFilePath);
            //}

        }

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

            _configForm.Activate();
            _configForm.Show();
        }
    }
}
