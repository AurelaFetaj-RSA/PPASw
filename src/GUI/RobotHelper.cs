using Newtonsoft.Json.Linq;
using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Server;
using Robot;
using RSACommon;
using RSACommon.GraphicsForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Diagnostic;
using Diagnostic.State;
using OPC.Protocol;
using OPC.Service;

namespace GUI
{
    public partial class FormMain : Form
    {
        Dictionary<string, DiagnosticWindowsControl> DiagnosticVariableGroupbox = new Dictionary<string, DiagnosticWindowsControl>();
        public async Task UpdateGraphicsGUI(TimeSpan interval, CancellationTokenSource cancellationToken)
        {

            while(true)
            {
                if (myCore.ApiSharedList.GetWebSharedUserInstance(myRSAUser) != null)
                {
                    //TODO: RSWare è connesso?
                    //lbLedUserConnection.State = (myCore.ApiSharedList.GetWebSharedUserInstance(myRSAUser).clientisconnected ) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                }

                if (myCore.OpcServerService.M2FNodeManager != null)
                {
                    //TODO: c'è il client MES connesso?
                    //lbLedMESConnection.State = (myCore.OpcServerService.clientisconnected ) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                }

                //UpdateRobotLamp();
                await Task.Delay(interval, cancellationToken.Token);
            }
        }

        public async Task UpdateDiagnosticGUI(TimeSpan interval, CancellationTokenSource cancellationToken)
        {

            while (true)
            {
                await CheckDiagnostic();
                await Task.Delay(interval, cancellationToken.Token);
            }
        }

        public async Task UpdateRobutStatus(TimeSpan interval, CancellationTokenSource cancellationToken)
        {

            while (true)
            {
                await UpdateRobotStatus();
                await Task.Delay(interval, cancellationToken.Token);
            }
        }


        private void StartDiagnosticGUI()
        {

            int xCoord = 10;
            MatrixPanel diagnosticMatrix = new MatrixPanel(xCoord, 5, this.TabPageDiagnostic.Width - 5, this.TabPageDiagnostic.Height - 150, myCore.DiagnosticConfigurator.Configuration.DiagnosticFormRow, myCore.DiagnosticConfigurator.Configuration.DiagnosticFormColumn, this.TabPageDiagnostic);

            foreach (string value in myCore.DiagnosticConfigurator.VariableList)
            {
                DiagnosticWindowsControl form = new DiagnosticWindowsControl(0, 0, value, null, null);
                DiagnosticVariableGroupbox[value] = form;

                if (!diagnosticMatrix.AddElements(form, 5, 5))
                {
                    MessageBox.Show("Too much Diagnostic Variable for Panel Matrix");
                    break;
                }

            }
        }

        public int counter = 0;
        private Dictionary<string, int> _lastState = new Dictionary<string, int>();

        public async Task CheckDiagnostic()
        {
            List<string> variableList = myCore.DiagnosticConfigurator.DiagnosticStatus.Keys.ToList();

            foreach(string diagnosticVariableName in variableList)
            {
                //int value = await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>(diagnosticVariableName);
                string command = KawasakiMemoryVariable.MakeCommand(KawasakiCommand.ty, diagnosticVariableName);
                int value = await myRobot.ReadCommandAsync<int>(command);

                if (myCore.DiagnosticConfigurator.DiagnosticResult(diagnosticVariableName, value, out DiagnosticState state))
                {

                    //controllo aggiunto per tracciare solo la variazione di Stato.
                    if(!_lastState.ContainsKey(diagnosticVariableName) || _lastState[diagnosticVariableName] != value)
                    {
                        string stateOutput = state.DiagnosticMessage;

                        if (DiagnosticVariableGroupbox.Count != 0)
                        {
                            DiagnosticVariableGroupbox[diagnosticVariableName].ThreadSafeWriteMessage($"| {value:000} | {stateOutput}");
                        }

                        _lastState[diagnosticVariableName] = value;
                    }

                }
            }
        }

        public async Task UpdateRobotStatus()
        {
            int toReadInfo = await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("job_result");
            int jobId = await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("job_id");

            if (toReadInfo != 0)
            {
                IService service = myCore.ServiceList.Find(t => t.GetType() == typeof(OpcServerService));

                if(service != null)
                {
                    (service as OpcServerService)?.M2FNodeManager?.SendCommandJobEnded((PasubioCommands)toReadInfo, jobId);
                }

                myRobot.SetVariable("job_result",0);
            }
        }


        public void UpdateRobotLamp()
        {
            if (myRobot != null)
            {
                //lbLedRobotConnection.State = (myRobot.IsConnected) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;



                /*
                lbLed1001.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1001)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1002.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1002)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1003.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1003)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1004.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1004)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1005.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1005)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1006.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1006)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1007.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1007)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1008.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1008)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1009.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1009)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1010.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1010)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1011.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1011)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1012.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1012)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1013.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1013)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1014.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1014)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1015.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1015)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1016.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1016)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1017.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1017)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1018.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1018)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1019.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1019)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1020.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1020)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1021.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1021)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1022.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1022)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1023.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1023)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1024.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1024)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1025.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1025)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1026.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1026)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1027.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1027)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1028.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1028)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1029.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1029)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1030.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1030)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1031.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1031)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1032.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1032)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1033.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1033)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1034.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1034)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1035.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1035)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1036.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1036)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1037.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1037)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1038.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1038)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1039.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1039)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1040.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1040)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1041.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1041)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1042.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1042)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1043.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1043)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1044.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1044)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1045.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1045)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1046.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1046)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1047.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1047)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1048.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(1048)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;

                //Output
                lbLed2001.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2001)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2002.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2002)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2003.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2003)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2004.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2004)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2005.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2005)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2006.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2006)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2007.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2007)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2008.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2008)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2009.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2009)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2010.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2010)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2011.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2011)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2012.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2012)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2013.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2013)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2014.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2014)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2015.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2015)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2016.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2016)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2017.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2017)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2018.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2018)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2019.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2019)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2020.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2020)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2021.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2021)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2022.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2022)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2023.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2023)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2024.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2024)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2025.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2025)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2026.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2026)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2027.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2027)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2028.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2028)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2029.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2029)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2030.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2030)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2031.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2031)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2032.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2032)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2033.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2033)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2034.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2034)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2035.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2035)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2036.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2036)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2037.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2037)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2038.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2038)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2039.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2039)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2040.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2040)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2041.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2041)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2042.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2042)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2043.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2043)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2044.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2044)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2045.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2045)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2046.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2046)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2047.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2047)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2048.State = (await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("sig(2048)") == -1) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                */

                return;

            }
        }
    }
}
