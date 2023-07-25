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
            //List<string> keys = new List<string>();
            //keys.Add("pcM1Status");
            //keys.Add("pcM2Status");
            //keys.Add("pcM3Status");
            //keys.Add("pcM4Status");
            //keys.Add("pcM5Status");
            //keys.Add("pcM6Status");

            //var varRresultS = await ccService.Read(keys);

            //opcua M1 status
            var varResult = await ccService.Read("pcM1Status");

            if (varResult.OpcResult) UpdateOPCUAM1Lamp((short)varResult.Value);
            else
            {
                //todo
            }

            //opcua M2 status
            varResult = await ccService.Read("pcM2Status");

            if (varResult.OpcResult) UpdateOPCUAM2Lamp((short)varResult.Value);
            else
            {
                //todo
            }

            //opcua M3 status
            varResult = await ccService.Read("pcM3Status");

            if (varResult.OpcResult) UpdateOPCUAM3Lamp((short)varResult.Value);
            else
            {
                //todo
            }

            //opcua M4 status
            varResult = await ccService.Read("pcM4Status");

            if (varResult.OpcResult) UpdateOPCUAM4Lamp((short)varResult.Value);
            else
            {
                //todo
            }

            //opcua M5 status
            varResult = await ccService.Read("pcM5Status");

            if (varResult.OpcResult) UpdateOPCUAM5Lamp((short)varResult.Value);
            else
            {
                //todo
            }

            //opcua M6 status
            varResult = await ccService.Read("pcM6Status");

            if (varResult.OpcResult) UpdateOPCUAM6Lamp((short)varResult.Value);
            else
            {
                //todo
            }

            //opcua M1 ready
            varResult = await ccService.Read("pcM1Ready");

            if (varResult.OpcResult) UpdateOPCUAM1ReadyLamp((short)varResult.Value);
            else
            {
                //todo
            }

            //opcua M2 ready
            varResult = await ccService.Read("pcM2Ready");

            if (varResult.OpcResult) UpdateOPCUAM2ReadyLamp((bool)varResult.Value);
            else
            {
                //todo
            }

            //opcua M3 ready
            varResult = await ccService.Read("pcM3Ready");

            if (varResult.OpcResult) UpdateOPCUAM3ReadyLamp((bool)varResult.Value);
            else
            {
                //todo
            }

            //opcua M4 ready
            varResult = await ccService.Read("pcM4Ready");

            if (varResult.OpcResult) UpdateOPCUAM4ReadyLamp((bool)varResult.Value);
            else
            {
                //todo
            }

            //opcua M5 ready
            varResult = await ccService.Read("pcM5Ready");

            if (varResult.OpcResult) UpdateOPCUAM5ReadyLamp((bool)varResult.Value);
            else
            {
                //todo
            }

            //opcua M6 ready
            varResult = await ccService.Read("pcM6Ready");

            if (varResult.OpcResult) UpdateOPCUAM6ReadyLamp((bool)varResult.Value);
            else
            {
                //todo
            }

            //opcua M1 homing done
            varResult = await ccService.Read("pcM1HomingDone");

            if (varResult.OpcResult) UpdateOPCUAM1HomingDoneLamp((bool)varResult.Value);
            else
            {
                //todo
            }

            //opcua M2 homing done
            varResult = await ccService.Read("pcM2HomingDone");

            if (varResult.OpcResult) UpdateOPCUAM2HomingDoneLamp((bool)varResult.Value);
            else
            {
                //todo
            }

            //opcua M3 homing done
            varResult = await ccService.Read("pcM3HomingDone");

            if (varResult.OpcResult) UpdateOPCUAM3HomingDoneLamp((bool)varResult.Value);
            else
            {
                //todo
            }

            //opcua M4 homing done
            varResult = await ccService.Read("pcM4HomingDone");

            if (varResult.OpcResult) UpdateOPCUAM4HomingDoneLamp((bool)varResult.Value);
            else
            {
                //todo
            }

            //opcua M5 homing done
            varResult = await ccService.Read("pcM5HomingDone");

            if (varResult.OpcResult) UpdateOPCUAM5HomingDoneLamp((bool)varResult.Value);
            else
            {
                //todo
            }

            //opcua M6 homing done
            varResult = await ccService.Read("pcM6HomingDone");

            if (varResult.OpcResult) UpdateOPCUAM6HomingDoneLamp((bool)varResult.Value);
            else
            {
                //todo
            }

            //current axis quote
            varResult = await ccService.Read("pcM2CurrentAxisQuote");
            if (varResult.OpcResult) UpdateM2CurrentAxisQuote((short)varResult.Value);
            else
            {
                //todo
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

        public void UpdateOPCUAM1Lamp(short value)
        {

            if (value == 0)
            {
                lbLedM1Status.LedColor = Color.Red;
                lbLedM1Status.Label = "emergency";
            }

            if (value == 1)
            {
                lbLedM1Status.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM1Status.Label = "automatic";
            }

            if (value == 2)
            {
                lbLedM1Status.LedColor = Color.Orange;
                lbLedM1Status.Label = "manual";
            }

            if (value == 3)
            {
                lbLedM1Status.LedColor = Color.Blue;
                lbLedM1Status.Label = "in cycle";
            }

            if (value == 4)
            {
                lbLedM1Status.LedColor = Color.Red;
                lbLedM1Status.Label = "in alarm";
            }
        }

        public void UpdateOPCUAM2Lamp(short value)
        {
            if (value == 0)
            {
                lbLedM2Status.LedColor = Color.Red;
                lbLedM2Status.Label = "emergency";
            }

            if (value == 1)
            {
                lbLedM2Status.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM2Status.Label = "automatic";
            }

            if (value == 2)
            {
                lbLedM2Status.LedColor = Color.Orange;
                lbLedM2Status.Label = "manual";
            }

            if (value == 3)
            {
                lbLedM2Status.LedColor = Color.Blue;
                lbLedM2Status.Label = "in cycle";
            }

            if (value == 4)
            {
                lbLedM2Status.LedColor = Color.Red;
                lbLedM2Status.Label = "in alarm";
            }
        }

        public void UpdateOPCUAM3Lamp(short value)
        {
            if (value == 0)
            {
                lbLedM3Status.LedColor = Color.Red;
                lbLedM3Status.Label = "emergency";
            }

            if (value == 1)
            {
                lbLedM3Status.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM3Status.Label = "automatic";
            }

            if (value == 2)
            {
                lbLedM3Status.LedColor = Color.Orange;
                lbLedM3Status.Label = "manual";
            }

            if (value == 3)
            {
                lbLedM3Status.LedColor = Color.Blue;
                lbLedM3Status.Label = "in cycle";
            }

            if (value == 4)
            {
                lbLedM3Status.LedColor = Color.Red;
                lbLedM3Status.Label = "in alarm";
            }
        }

        public void UpdateOPCUAM4Lamp(short value)
        {
            if (value == 0)
            {
                lbLedM4Status.LedColor = Color.Red;
                lbLedM4Status.Label = "emergency";
            }

            if (value == 1)
            {
                lbLedM4Status.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM4Status.Label = "automatic";
            }

            if (value == 2)
            {
                lbLedM4Status.LedColor = Color.Orange;
                lbLedM4Status.Label = "manual";
            }

            if (value == 3)
            {
                lbLedM4Status.LedColor = Color.Blue;
                lbLedM4Status.Label = "in cycle";
            }

            if (value == 4)
            {
                lbLedM4Status.LedColor = Color.Red;
                lbLedM4Status.Label = "in alarm";
            }
        }

        public void UpdateOPCUAM5Lamp(short value)
        {
            if (value == 0)
            {
                lbLedM5Status.LedColor = Color.Red;
                lbLedM5Status.Label = "emergency";
            }

            if (value == 1)
            {
                lbLedM5Status.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM5Status.Label = "automatic";
            }

            if (value == 2)
            {
                lbLedM5Status.LedColor = Color.Orange;
                lbLedM5Status.Label = "manual";
            }

            if (value == 3)
            {
                lbLedM5Status.LedColor = Color.Blue;
                lbLedM5Status.Label = "in cycle";
            }

            if (value == 4)
            {
                lbLedM5Status.LedColor = Color.Red;
                lbLedM5Status.Label = "in alarm";
            }
        }

        public void UpdateOPCUAM6Lamp(short value)
        {
            if (value == 0)
            {
                lbLedM6Status.LedColor = Color.Red;
                lbLedM6Status.Label = "emergency";
            }

            if (value == 1)
            {
                lbLedM6Status.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM6Status.Label = "automatic";
            }

            if (value == 2)
            {
                lbLedM6Status.LedColor = Color.Orange;
                lbLedM6Status.Label = "manual";
            }

            if (value == 3)
            {
                lbLedM6Status.LedColor = Color.Blue;
                lbLedM6Status.Label = "in cycle";
            }

            if (value == 4)
            {
                lbLedM6Status.LedColor = Color.Red;
                lbLedM6Status.Label = "in alarm";
            }
        }

        public void UpdateOPCUAM1ReadyLamp(short value)
        {
            if (value == 0)
            {
                lbLedM1Ready.LedColor = Color.Red;
                lbLedM1Ready.Label = "not ready";
            }

            if (value == 1)
            {
                lbLedM1Ready.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM1Ready.Label = "ready";
            }
        }

        public void UpdateOPCUAM2ReadyLamp(bool value)
        {
            if (!value)
            {
                lbLedM2Ready.LedColor = Color.Red;
                lbLedM2Ready.Label = "not ready";
            }

            else
            {
                lbLedM2Ready.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM2Ready.Label = "ready";
            }
        }

        public void UpdateOPCUAM3ReadyLamp(bool value)
        {
            if (!value)
            {
                lbLedM3Ready.LedColor = Color.Red;
                lbLedM3Ready.Label = "not ready";
            }
            else
            {
                lbLedM3Ready.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM3Ready.Label = "ready";
            }
        }

        public void UpdateOPCUAM4ReadyLamp(bool value)
        {
            if (!value)
            {
                lbLedM4Ready.LedColor = Color.Red;
                lbLedM4Ready.Label = "not ready";
            }
            else
            {
                lbLedM4Ready.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM4Ready.Label = "ready";
            }
        }

        public void UpdateOPCUAM5ReadyLamp(bool value)
        {
            if (!value)
            {
                lbLedM5Ready.LedColor = Color.Red;
                lbLedM5Ready.Label = "not ready";
            }
            else
            {
                lbLedM5Ready.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM5Ready.Label = "ready";
            }
        }

        public void UpdateOPCUAM6ReadyLamp(bool value)
        {
            if (!value)
            {
                lbLedM6Ready.LedColor = Color.Red;
                lbLedM6Ready.Label = "not ready";
            }
            else            
            {
                lbLedM6Ready.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM6Ready.Label = "ready";
            }
        }

        public void UpdateOPCUAM1HomingDoneLamp(bool value)
        {
            if (!value)
            {
                lbLedM1HomingDone.LedColor = Color.Red;
                lbLedM1HomingDone.Label = "homing not done";
            }

            else
            {
                lbLedM1HomingDone.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM1HomingDone.Label = "homing done";
            }
        }

        public void UpdateOPCUAM2HomingDoneLamp(bool value)
        {
            if (!value)
            {
                lbLedM2HomingDone.LedColor = Color.Red;
                lbLedM2HomingDone.Label = "homing not done";
            }

            else
            {
                lbLedM2HomingDone.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM2HomingDone.Label = "homing done";
            }
        }

        public void UpdateOPCUAM3HomingDoneLamp(bool value)
        {
            if (!value)
            {
                lbLedM3HomingDone.LedColor = Color.Red;
                lbLedM3HomingDone.Label = "homing not done";
            }

            else
            {
                lbLedM3HomingDone.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM3HomingDone.Label = "homing done";
            }
        }

        public void UpdateOPCUAM4HomingDoneLamp(bool value)
        {
            if (!value)
            {
                lbLedM4HomingDone.LedColor = Color.Red;
                lbLedM4HomingDone.Label = "homing not done";
            }

            else
            {
                lbLedM4HomingDone.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM4HomingDone.Label = "homing done";
            }
        }

        public void UpdateOPCUAM5HomingDoneLamp(bool value)
        {
            if (!value)
            {
                lbLedM5HomingDone.LedColor = Color.Red;
                lbLedM5HomingDone.Label = "homing not done";
            }

            else
            {
                lbLedM5HomingDone.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM5HomingDone.Label = "homing done";
            }
        }

        public void UpdateOPCUAM6HomingDoneLamp(bool value)
        {
            if (!value)
            {
                lbLedM6HomingDone.LedColor = Color.Red;
                lbLedM6HomingDone.Label = "homing not done";
            }
            else
            {
                lbLedM6HomingDone.LedColor = Color.FromArgb(195, 222, 155);
                lbLedM6HomingDone.Label = "homing done";
            }
        }

        #region (* send M1 teaching package *)
        public async void OPCUAM1TeachPckSend(short pointID, short[] pointQuote, short[] pointSpeed, bool[] pointReg)
        {
            List<string> keys = new List<string>();
            keys.Add("pcM2TeachPointID");
            keys.Add("pcM2TeachQuote");
            keys.Add("pcM2TeachSpeed");            
            keys.Add("pcM2TeachPointReg");
            
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
        #endregion

        #endregion

    }
}
