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
using System.IO;
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
        #region -----LETTURA PROGRAMMI-----








        #endregion


        #region (* machines status *)
        public async Task UpdateOPCUAStatus()
        {
            try
            {
                List<string> keys = new List<string>()
                {
                    //"pcM1Status",
                    "pcM2Status",
                    "pcM3Status",
                    "pcM4Status",
                    "pcM5Status"
                    //"pcM6Status"
                };

                var readResult = await ccService.Read(keys);
                //UpdateOPCUAMStatus((short)readResult["pcM1Status"]?.Value, lbLedM1Status);
                UpdateOPCUAMStatus((short)readResult["pcM2Status"]?.Value, lbLedM2Status);
                ManageOPCUAMBtnStatus((short)readResult["pcM2Status"]?.Value, lbButtonTestM2Start);
                UpdateOPCUAMStatus((short)readResult["pcM3Status"]?.Value, lbLedM3Status);
                ManageOPCUAMBtnStatus((short)readResult["pcM3Status"]?.Value, lbButtonTestM3Start);
                UpdateOPCUAMStatus((short)readResult["pcM4Status"]?.Value, lbLedM4Status);
                ManageOPCUAMBtnStatus((short)readResult["pcM4Status"]?.Value, lbButtonTestM4Start);
                UpdateOPCUAMStatus((short)readResult["pcM5Status"]?.Value, lbLedM5Status);
                ManageOPCUAMBtnStatus((short)readResult["pcM5Status"]?.Value, lbButtonTestM5Start);
                //UpdateOPCUAMStatus((short)readResult["pcM6Status"]?.Value, lbLedM6Status);
                short statusOven = 3;
                short statusTrimmer = 3;
                ManageLineStatus(statusTrimmer, (short)readResult["pcM2Status"]?.Value, (short)readResult["pcM3Status"]?.Value,
                    (short)readResult["pcM4Status"]?.Value, (short)readResult["pcM5Status"]?.Value, statusOven);
            }
            catch(Exception Ex)
            {

            }

            try
            {
                List<string> keys = new List<string>()
                {
                    //"pcM1HomingDone",
                    "pcM2HomingDone",
                    "pcM3HomingDone",
                    "pcM4HomingDone"
                    //"pcM5HomingDone",
                    //"pcM6HomingDone"
                };

                var readResult = await ccService.Read(keys);
                //UpdateOPCUAMHomingDone((bool)readResult["pcM1HomingDone"]?.Value, lbLedM1HomingDone);
                UpdateOPCUAMHomingDone((bool)readResult["pcM2HomingDone"]?.Value, lbLedM2HomingDone);
                UpdateOPCUAMHomingDone((bool)readResult["pcM3HomingDone"]?.Value, lbLedM3HomingDone);
                UpdateOPCUAMHomingDone((bool)readResult["pcM4HomingDone"]?.Value, lbLedM4HomingDone);
                //UpdateOPCUAMHomingDone((bool)readResult["pcM5HomingDone"]?.Value, lbLedM5HomingDone);
                //UpdateOPCUAMHomingDone((bool)readResult["pcM6HomingDone"]?.Value, lbLedM6HomingDone);
            }
            catch (Exception Ex)
            {

            }

            try
            {
                List<string> keys = new List<string>()
                {
                    //"pcM1PointReached",
                    "pcM2PointReached",
                    "pcM3PointReached"
                    //"pcM4PointReached",
                    //"pcM5PointReached",
                    //"pcM6PointReached"
                };

                var readResult = await ccService.Read(keys);
                UpdateOPCUAM2PointReached(readResult["pcM2PointReached"]);                
            }
            catch (Exception Ex)
            {

            }

            try
            {
                List<string> keys = new List<string>()
                {
                    //"pcM1CurrentAxisQuote",
                    "pcM2CurrentAxisQuote",
                    "pcM3CurrentAxisQuote",
                    "pcM4CurrentAxisQuote"
                    //"pcM5CurrentAxisQuote",
                    //"pcM6CurrentAxisQuote"
                };

                var readResult = await ccService.Read(keys);
                //UpdateOPCUAM1CAQ((short)readResult["pcM1CurrentAxisQuote"]?.Value);
                UpdateOPCUAM2CAQ((short)readResult["pcM2CurrentAxisQuote"]?.Value);
                UpdateOPCUAM3CAQ((short)readResult["pcM3CurrentAxisQuote"]?.Value);
                UpdateOPCUAM4CAQ((short)readResult["pcM4CurrentAxisQuote"]?.Value);
            }
            catch (Exception ex)
            {

            }


            //manipulator digital input
            try
            {
                List<string> keys = new List<string>()
                {
                    "pcM2DI",
                    "pcM2DO"
                };

                var readResult = await ccService.Read(keys);
                UpdateOPCUAM2DI(readResult["pcM2DI"]);

            }
            catch (Exception Ex)
            {

            }

                 

            //manipulator digital input
            //try
            //{
            //    List<string> keys = new List<string>()
            //    {
            //        "pcM5DI",
            //        "pcM5DO"
            //    };

            //    var readResult = await ccService.Read(keys);
            //    UpdateOPCUAM5DI(readResult["pcM5DI"]);

            //}
            //catch (Exception Ex)
            //{

            //}
        }

        public void UpdateOPCUAM5DI(ClientResult diREsult)
        {
            //int i = 0;
            
            //Dictionary<int, LBSoft.IndustrialCtrls.Leds.LBLed> myDict = new Dictionary<int, LBSoft.IndustrialCtrls.Leds.LBLed>();
            //myDict[0] = lbLedM5DI1;

            //foreach (var result in diREsult.Value)
            //{
            //    myDict[result]
            //}
        }

        public void UpdateOPCUAM2DI(ClientResult diREsult)
        {
            int i = 0;
            bool[] arrayBool = (bool[])diREsult.Value;

            lbLed1001M2.State = (arrayBool[1] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1002M2.State = (arrayBool[2] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1003M2.State = (arrayBool[3] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1004M2.State = (arrayBool[4] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1005M2.State = (arrayBool[5] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1006M2.State = (arrayBool[6] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1007M2.State = (arrayBool[7] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1008M2.State = (arrayBool[8] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1008M2.Label = (arrayBool[8] == true) ? "selector in AUTO" : "selector in MANUAL";
            lbLed1009M2.State = (arrayBool[9] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1010M2.State = (arrayBool[10] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1011M2.State = (arrayBool[11] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1012M2.State = (arrayBool[12] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1013M2.State = (arrayBool[13] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1014M2.State = (arrayBool[14] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1015M2.State = (arrayBool[15] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1016M2.State = (arrayBool[16] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;

            lbLed1017M2.State = (arrayBool[17] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1018M2.State = (arrayBool[18] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1019M2.State = (arrayBool[19] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1020M2.State = (arrayBool[20] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1021M2.State = (arrayBool[21] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1022M2.State = (arrayBool[22] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1023M2.State = (arrayBool[23] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1024M2.State = (arrayBool[24] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;

            lbLed1025M2.State = (arrayBool[25] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1026M2.State = (arrayBool[26] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1027M2.State = (arrayBool[27] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1028M2.State = (arrayBool[28] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1029M2.State = (arrayBool[29] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1030M2.State = (arrayBool[30] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1031M2.State = (arrayBool[31] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1032M2.State = (arrayBool[32] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;

            lbLed1033M2.State = (arrayBool[33] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1034M2.State = (arrayBool[34] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1035M2.State = (arrayBool[35] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1036M2.State = (arrayBool[36] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1037M2.State = (arrayBool[37] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1038M2.State = (arrayBool[38] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1039M2.State = (arrayBool[39] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLed1040M2.State = (arrayBool[40] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;

        }

        public void UpdateOPCUAM2PointReached(ClientResult readResult)
        {

            string fileNameReached = "";
            string fileNameNotReached = "";
            string fileNameNull = "";
            byte[] binaryDataReached = null;
            byte[] binaryDataNotReached = null;
            byte[] binaryDataNull = null;
            FileInfo fileInfoReached = null;
            FileInfo fileInfoNotReached = null;
            FileInfo fileInfoNull = null;
            MemoryStream msReached = null;
            MemoryStream msNotReached = null;
            MemoryStream msNull = null;
            Image returnImageReached = null;
            Image returnImageNotReached = null;
            Image returnImageNull = null;
            fileNameReached = "C:\\RSA\\github_repositories\\PPASw\\src\\GUI\\images\\preached.png";
            fileInfoReached = new FileInfo(fileNameReached);

            binaryDataReached = File.ReadAllBytes(fileNameReached);
            msReached = new MemoryStream(binaryDataReached);
            returnImageReached = Image.FromStream(msReached, false, true);

            fileNameNotReached = "C:\\RSA\\github_repositories\\PPASw\\src\\GUI\\images\\pnotreached.png";
            fileInfoNotReached = new FileInfo(fileNameNotReached);

            binaryDataNotReached = File.ReadAllBytes(fileNameNotReached);
            msNotReached = new MemoryStream(binaryDataNotReached);
            returnImageNotReached = Image.FromStream(msNotReached, false, true);

            fileNameNull = "C:\\RSA\\github_repositories\\PPASw\\src\\GUI\\images\\null.png";
            fileInfoNull = new FileInfo(fileNameNull);

            binaryDataNull = File.ReadAllBytes(fileNameNull);
            msNull = new MemoryStream(binaryDataNull);
            returnImageNull = Image.FromStream(msNull, false, true);

            if (readResult.OpcResult)
            {
                bool[] arrayBool = (bool[])readResult.Value;
                int i = 0;
                for(i = 1; i<arrayBool.Count();i++)
                {
                    if (arrayBool[i])
                    {
                        dataGridViewM2TeachPoints.Rows[i - 1].Cells[5].Value = returnImageNull;
                        dataGridViewM2TeachPoints.Refresh();
                        dataGridViewM2TeachPoints.Rows[i - 1].Cells[5].Value = returnImageReached;
                        dataGridViewM2TeachPoints.Refresh();
                        dataGridViewM2TeachPoints.Rows[1 - 1].Cells[5].ToolTipText = fileInfoReached.ToString();
                    }
                    else
                    {
                        dataGridViewM2TeachPoints.Rows[i - 1].Cells[5].Value = returnImageNull;
                        dataGridViewM2TeachPoints.Refresh();
                        dataGridViewM2TeachPoints.Rows[i - 1].Cells[5].Value = returnImageNotReached;
                        dataGridViewM2TeachPoints.Refresh();
                        dataGridViewM2TeachPoints.Rows[i - 1].Cells[5].ToolTipText = fileInfoNotReached.ToString();
                    }
                }
                
            } 
                
            //int i = 0;

            //Dictionary<int, LBSoft.IndustrialCtrls.Leds.LBLed> myDict = new Dictionary<int, LBSoft.IndustrialCtrls.Leds.LBLed>();
            //myDict[0] = lbLedM5DI1;

            //foreach (var result in diREsult.Value)
            //{
            //    myDict[result]
            //}




        }



        public void UpdateOPCUAM2CAQ(short value)
        {
            try
            {
                WriteOnLabelAsync(value.ToString(), labelM2TeachAxisQuoteValue);
            }
            catch(Exception EX)
            {

            }
        }

        public void UpdateOPCUAM3CAQ(short value)
        {
            try
            {
                WriteOnLabelAsync(value.ToString(), labelM3TeachAxisQuoteValue);
            }
            catch (Exception EX)
            {

            }
        }

        public void UpdateOPCUAM4CAQ(short value)
        {
            try
            {
                WriteOnLabelAsync(value.ToString(), labelM4TeachAxisQuoteValue);
            }
            catch (Exception EX)
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
                lblLed.LedColor = Color.Blue;
                lblLed.Label = "automatic";
            }

            if (value == 2)
            {
                lblLed.LedColor = Color.Orange;
                lblLed.Label = "manual";
            }

            if (value == 3)
            {
                lblLed.LedColor = Color.FromArgb(107, 227, 162);
                lblLed.Label = "in cycle";
            }

            if (value == 4)
            {
                lblLed.LedColor = Color.Red;
                lblLed.Label = "in alarm";
            }
        }

        public void ManageOPCUAMBtnStatus(short value, LBSoft.IndustrialCtrls.Buttons.LBButton btn)
        {
            if (value == 0)
            {
                btn.State = LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Normal;
                btn.Label = "START";
            }

            if (value == 1)
            {
                btn.State = LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Normal;
                btn.Label = "START";
            }

            if (value == 2)
            {
                btn.State = LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Normal;
                btn.Label = "START";
            }

            if (value == 3)
            {
                btn.State = LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed;
                btn.Label = "STOP";
            }

            if (value == 4)
            {
                lbButtonTestM2Start.State = LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Normal;
                lbButtonTestM2Start.Label = "START";
            }
        }

        public void ManageLineStatus(short valueM1, short valueM2, short valueM3, short valueM4, short valueM5, short valueM6)
        {
            if (valueM1 == 3 & valueM2 == 3 & valueM3 == 3 & valueM4 == 3 & valueM5 == 3 & valueM6 == 3)
            {
                lbButtonStartStop.State = LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed;
                lbButtonStartStop.Label = "STOP";
            }
            else
            {
                lbButtonStartStop.State = LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Normal;
                lbButtonStartStop.Label = "START";
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

        public async void OPCUAM3TeachPckSend(short pointID, short[] pointQuote, short[] pointSpeed, bool[] pointReg)
        {
            List<string> keys = new List<string>();
            keys.Add("pcM3TeachPointID");
            keys.Add("pcM3TeachSpeed");
            keys.Add("pcM3TeachQuote");
            keys.Add("pcM3TeachPointReg");

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

        public async void OPCUAM3TestPckSend(short[] pointQuote, short[] pointSpeed)
        {
            List<string> keys = new List<string>();
            keys.Add("pcM3TestSpeed");
            keys.Add("pcM3TestQuote");

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
