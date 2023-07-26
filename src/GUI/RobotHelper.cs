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
using OpcCustom;
using RSACommon.Service;

namespace GUI
{
    public partial class FormMain : Form
    {
        Dictionary<string, DiagnosticWindowsControl> DiagnosticVariableGroupbox = new Dictionary<string, DiagnosticWindowsControl>();
        public async Task UpdateGraphicsGUI(TimeSpan interval, CancellationTokenSource cancellationToken)
        {

            //while(true)
            //{
            //    if (myCore.ApiSharedList.GetWebSharedUserInstance(myRSAUser) != null)
            //    {
            //        //TODO: RSWare è connesso?
            //        //lbLedUserConnection.State = (myCore.ApiSharedList.GetWebSharedUserInstance(myRSAUser).clientisconnected ) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            //    }

            //    FutureOpcServerCustom service = null;

            //    if ((service = (FutureOpcServerCustom)myCore.FindPerType(typeof(FutureOpcServerCustom))) != null)
            //        if (service != null)
            //        {
            //            //TODO: c'è il client MES connesso?
            //            //lbLedMESConnection.State = (myCore.OpcServerService.clientisconnected ) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            //        }
            //    //UpdateRobotLamp();
            //    await Task.Delay(interval, cancellationToken.Token);
            //}
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

        public async Task UpdateOPCUAStatus(TimeSpan interval, CancellationTokenSource cancellationToken)
        {

            while (true)
            {
                await UpdateOPCUAStatus();
                await Task.Delay(interval, cancellationToken.Token);
            }
        }


        private void StartDiagnosticGUI()
        {

            //int xCoord = 10;
            //MatrixPanel diagnosticMatrix = new MatrixPanel(xCoord, 5, this.TabPageDiagnostic.Width - 5, this.TabPageDiagnostic.Height - 150, myCore.DiagnosticConfigurator.Configuration.DiagnosticFormRow, myCore.DiagnosticConfigurator.Configuration.DiagnosticFormColumn, this.TabPageDiagnostic);

            //foreach (string value in myCore.DiagnosticConfigurator.VariableList)
            //{
            //    DiagnosticWindowsControl form = new DiagnosticWindowsControl(0, 0, value, null, null);
            //    DiagnosticVariableGroupbox[value] = form;

            //    if (!diagnosticMatrix.AddElements(form, 5, 5))
            //    {
            //        MessageBox.Show("Too much Diagnostic Variable for Panel Matrix");
            //        break;
            //    }

            //}
        }

        public int counter = 0;
        private Dictionary<string, int> _lastState = new Dictionary<string, int>();

        public async Task CheckDiagnostic()
        {
            return;
            //List<string> variableList = myCore.DiagnosticConfigurator.DiagnosticStatus.Keys.ToList();

            //foreach(string diagnosticVariableName in variableList)
            //{
            //    //int value = await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>(diagnosticVariableName);
            //    string command = KawasakiMemoryVariable.MakeCommand(KawasakiCommand.ty, diagnosticVariableName);

            //    if (myRobot == null)
            //        return;


            //    int value = await myRobot?.ReadCommandAsync<int>(command);

            //    if (myCore.DiagnosticConfigurator.DiagnosticResult(diagnosticVariableName, value, out DiagnosticState state))
            //    {

            //        //controllo aggiunto per tracciare solo la variazione di Stato.
            //        if(!_lastState.ContainsKey(diagnosticVariableName) || _lastState[diagnosticVariableName] != value)
            //        {
            //            string stateOutput = state.DiagnosticMessage;

            //            if (DiagnosticVariableGroupbox.Count != 0)
            //            {
            //                DiagnosticVariableGroupbox[diagnosticVariableName].ThreadSafeWriteMessage($"| {value:000} | {stateOutput}");
            //            }

            //            _lastState[diagnosticVariableName] = value;
            //        }

            //    }
            //}
        }

        public async Task UpdateRobotStatus()
        {
            //int toReadInfo = await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("job_result");
            //int jobId = await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>("job_id");

            //if (toReadInfo != 0)
            //{
            //    IService service = myCore.ServiceList.Find(t => t.GetType() == typeof(FutureOpcServerCustom));

            //    if(service != null)
            //    {
            //        (service as FutureOpcServerCustom)?.M2FNodeManager?.SendCommandJobEnded((PasubioCommands)toReadInfo, jobId);
            //    }
            //    myRobot.SetVariable("job_result",0);
            //}
        }
        
        public void WriteOnLabelAsync(string textToAppend, Label lbl)
        {
            if (lbl.InvokeRequired)
            {
                lbl.Invoke((MethodInvoker)delegate
                {
                    lbl.Text = textToAppend;
                });
            }
        }

        #region (* machines status *)
        public async Task UpdateOPCUAStatus()
        {
            try
            {
                List<string> keys = new List<string>()
                {
                    "pcM1Status",
                    "pcM2Status",
                    "pcM3Status",
                    "pcM4Status",
                    "pcM5Status",
                    "pcM6Status"
                };

                var readResult = await ccService.Read(keys);
                UpdateOPCUAMStatus((short)readResult["pcM1Status"]?.Value, lbLedM1Status);
                UpdateOPCUAMStatus((short)readResult["pcM2Status"]?.Value, lbLedM2Status);
                UpdateOPCUAMStatus((short)readResult["pcM3Status"]?.Value, lbLedM3Status);
                UpdateOPCUAMStatus((short)readResult["pcM4Status"]?.Value, lbLedM4Status);
                UpdateOPCUAMStatus((short)readResult["pcM5Status"]?.Value, lbLedM5Status);
                UpdateOPCUAMStatus((short)readResult["pcM6Status"]?.Value, lbLedM6Status);
            }
            catch(Exception Ex)
            {

            }

            try
            {
                List<string> keys = new List<string>()
                {
                    "pcM1HomingDone",
                    "pcM2HomingDone",
                    "pcM3HomingDone",
                    "pcM4HomingDone",
                    "pcM5HomingDone",
                    "pcM6HomingDone"
                };

                var readResult = await ccService.Read(keys);
                UpdateOPCUAMHomingDone((bool)readResult["pcM1HomingDone"]?.Value, lbLedM1HomingDone);
                UpdateOPCUAMHomingDone((bool)readResult["pcM2HomingDone"]?.Value, lbLedM2HomingDone);
                UpdateOPCUAMHomingDone((bool)readResult["pcM3HomingDone"]?.Value, lbLedM3HomingDone);
                UpdateOPCUAMHomingDone((bool)readResult["pcM4HomingDone"]?.Value, lbLedM4HomingDone);
                UpdateOPCUAMHomingDone((bool)readResult["pcM5HomingDone"]?.Value, lbLedM5HomingDone);
                UpdateOPCUAMHomingDone((bool)readResult["pcM6HomingDone"]?.Value, lbLedM6HomingDone);
            }
            catch (Exception Ex)
            {

            }

            try 
            { 

                //current axis quote
                var varResult = await ccService.Read("pcM2CurrentAxisQuote");
                if (varResult.OpcResult) UpdateM2CurrentAxisQuote((short)varResult.Value);
                else
                {
                    //todo
                }
            }
            catch(Exception ex)
            {

            }

        }

        public void UpdateM2CurrentAxisQuote(short value)
        {
            try
            {
                WriteOnLabelAsync(value.ToString() + " " + "(mm)", labelM2TeachAxisQuoteValue);
            }
            catch(Exception EX)
            {

            }
        }

        public void UpdateOPCUAMStatus(short value, LBSoft.IndustrialCtrls.Leds.LBLed lblLed)
        {

            if (value == 0)
            {
                lblLed.LedColor = Color.Red;
                lblLed.Label = "emergency";
            }

            if (value == 1)
            {
                lblLed.LedColor = Color.FromArgb(195, 222, 155);
                lblLed.Label = "automatic";
            }

            if (value == 2)
            {
                lblLed.LedColor = Color.Orange;
                lblLed.Label = "manual";
            }

            if (value == 3)
            {
                lblLed.LedColor = Color.Blue;
                lblLed.Label = "in cycle";
            }

            if (value == 4)
            {
                lblLed.LedColor = Color.Red;
                lblLed.Label = "in alarm";
            }
        }

        public void UpdateOPCUAMHomingDone(bool value, LBSoft.IndustrialCtrls.Leds.LBLed lblLed)
        {
            if (!value)
            {
                lblLed.LedColor = Color.Red;
                lblLed.Label = "homing not done";
            }

            else
            {
                lblLed.LedColor = Color.FromArgb(195, 222, 155);
                lblLed.Label = "homing done";
            }
        }


   

        #region (* send M1 teaching package *)
        public async void OPCUAM1TeachPckSend(short pointID, short[] pointQuote, short[] pointSpeed, bool[] pointReg)
        {
            List<string> keys = new List<string>();
            keys.Add("pcM1TeachPointID");
            keys.Add("pcM1TeachQuote");
            keys.Add("pcM1TeachSpeed");            
            keys.Add("pcM1TeachPointReg");
            
            List<object> obj = new List<object>()
            {
                pointID,
                pointQuote,
                pointSpeed,
                pointReg
            };

            Dictionary<string, ClientResult> sendResult = new Dictionary<string, ClientResult>();
            try
            {
                sendResult = await ccService.Send(keys, obj);
            }
            catch(Exception ex)
            {

            }
        }

        public async void OPCUAM2TeachPckSend(short pointID, short[] pointQuote, short[] pointSpeed, bool[] pointReg)
        {
            List<string> keys = new List<string>();
            keys.Add("pcM2TeachPointID");
            keys.Add("pcM2TeachSpeed");
            keys.Add("pcM2TeachQuote");
            keys.Add("pcM2TeachPointReg");

            List<object> obj = new List<object>()
            {
                pointID,
                pointSpeed,
                pointQuote,
                pointReg
            };

            Dictionary<string, ClientResult> sendResult = new Dictionary<string, ClientResult>();
            try
            {
                sendResult = await ccService.Send(keys, obj);
            }
            catch (Exception ex)
            {

            }
        }

        public async void OPCUAM2TestPckSend(short[] pointQuote, short[] pointSpeed)
        {
            List<string> keys = new List<string>();
            keys.Add("pcM2TestSpeed");
            keys.Add("pcM2TestQuote");

            List<object> obj = new List<object>()
            {
                pointSpeed,
                pointQuote
            };

            Dictionary<string, ClientResult> sendResult = new Dictionary<string, ClientResult>();
            try
            {
                sendResult = await ccService.Send(keys, obj);
            }
            catch (Exception ex)
            {

            }
        }        
        #endregion

        #endregion

    }
}
