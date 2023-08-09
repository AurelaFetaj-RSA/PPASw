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
        private bool[] lastLifeBit { get; set; } = { false, false, false, false, false, false };
        private int[] LifeBitCounter { get; set; } = { 0, 0, 0, 0, 0, 0};
        private bool[] LifeBitTimeout { get; set; } = { false, false, false, false, false, false };
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
        
        public void WriteOnLabelAsync(ClientResult cr, Label lbl)
        {
            string textToAppend = "";

            if ((cr == null) || (cr.OpcResult == false))
            {
                textToAppend = "offline";
            }
            else
            {
                textToAppend = cr.Value.ToString();
            }
            if (lbl.InvokeRequired)
            {
                lbl.Invoke((MethodInvoker)delegate
                {
                    lbl.Text = textToAppend;
                });
            }
        }

        public void WriteOnCheckBoxStartAsync(ClientResult cr, CheckBox chk)
        {
            short value = -1;

            if ((cr == null) || (cr.OpcResult == false))
            {

            }
            else
            {
                value = (short)cr.Value;
            }

            if (chk.InvokeRequired)
            {
                chk.Invoke((MethodInvoker)delegate
                {
                    chk.Text = "";
                    if (value == -1)
                    {
                        chk.CheckState = CheckState.Indeterminate;
                        chk.Image = imageListStart.Images[2];
                    }

                    if (value == 0)
                    {
                        chk.CheckState = CheckState.Unchecked;
                        chk.Image = imageListStart.Images[0];
                    }

                    if (value == 1)
                    {
                        chk.CheckState = CheckState.Unchecked;
                        chk.Image = imageListStart.Images[0];
                    }

                    if (value == 2)
                    {
                        chk.CheckState = CheckState.Unchecked;
                        chk.Image = imageListStart.Images[0];
                    }

                    if (value == 3)
                    {
                        chk.CheckState = CheckState.Checked;
                        chk.Image = imageListStart.Images[1];
                    }

                    if (value == 4)
                    {
                        chk.CheckState = CheckState.Unchecked;
                        chk.Image = imageListStart.Images[0];
                    }
                });
            }
        }

        public void WriteOnLabelStatusAsync(ClientResult cr, Label lbl)
        {
            short value = -1;

            if ((cr == null) || (cr.OpcResult == false))
            {

            }
            else
            {
                value = (short)cr.Value;
            }

            if (lbl.InvokeRequired)
            {
                lbl.Invoke((MethodInvoker)delegate
                {
                    lbl.Text = "";
                    if (value == -1)
                    {
                        lbl.Text = "offline";
                    }

                    if (value == 0)
                    {
                        lbl.Text = "emegency";
                    }

                    if (value == 1)
                    {
                        lbl.Text = "automatic";
                    }

                    if (value == 2)
                    {
                        lbl.Text = "manual";
                    }

                    if (value == 3)
                    {
                        lbl.Text = "in cycle";
                    }

                    if (value == 4)
                    {
                        lbl.Text = "in alarm";
                    }
                });
            }
        }


        public void WriteOnCheckBoxPauseAsync(ClientResult cr, CheckBox chk)
        {
            bool value = false;
            bool founded = false;

            if ((cr == null) || (cr.OpcResult == false))
            {
                
            }
            else
            {
                value = (bool)cr.Value;
                founded = true;
            }

            if (chk.InvokeRequired)
            {
                chk.Invoke((MethodInvoker)delegate
                {
                    chk.Text = "";
                    if (!founded)
                    {
                        chk.CheckState = CheckState.Indeterminate;
                        chk.Image = imageListStart.Images[2];
                    }
                    else
                    {
                        if (value == true)
                        {
                            chk.CheckState = CheckState.Checked;
                            chk.Image = imageListStart.Images[4];
                        }

                        if (value == false)
                        {
                            chk.CheckState = CheckState.Unchecked;
                            chk.Image = imageListStart.Images[3];
                        }
                    }
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
                if (ccService.ClientIsConnected)
                {

                    #region (* machine/line status *)
                    /*machine status
                     * = 0 -> emergency
                     * = 1 -> manual 
                     * = 2 -> automatic(with errors)
                     * = 3 -> in cycle (no errors)
                     * = 4 -> alarms
                     */
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

                    UpdateOPCUAMStatus(readResult["pcM1Status"], buttonM1Status, labelM1Status);
                    ManageOPCUAMBtnStatus(readResult["pcM1Status"], checkBoxM1Start);

                    UpdateOPCUAMStatus(readResult["pcM2Status"], buttonM2Status, labelM2Status);
                    ManageOPCUAMBtnStatus(readResult["pcM2Status"], checkBoxM2Start);

                    UpdateOPCUAMStatus(readResult["pcM3Status"], buttonM3Status, labelM3Status);
                    ManageOPCUAMBtnStatus(readResult["pcM3Status"], checkBoxM3Start);

                    UpdateOPCUAMStatus(readResult["pcM4Status"], buttonM4Status, labelM4Status);
                    ManageOPCUAMBtnStatus(readResult["pcM4Status"], checkBoxM4Start);

                    UpdateOPCUAMStatus(readResult["pcM5Status"], buttonM5Status, labelM5Status);
                    ManageOPCUAMBtnStatus(readResult["pcM5Status"], checkBoxM5Start);

                    UpdateOPCUAMStatus(readResult["pcM6Status"], buttonM6Status, labelM6Status);
                    ManageOPCUAMBtnStatus(readResult["pcM6Status"], checkBoxM6Start);

                    ManageStartStatus(readResult["pcM1Status"], readResult["pcM2Status"], readResult["pcM3Status"],
                    readResult["pcM4Status"], readResult["pcM5Status"], readResult["pcM6Status"]);
                    #endregion

                    #region(* system in pause *)
                    keys = new List<string>()
                    {
                        "pcM1Pause",
                        "pcM2Pause",
                        "pcM3Pause",
                        "pcM4Pause",
                        "pcM5Pause",
                        "pcM6Pause"
                    };

                    readResult = await ccService.Read(keys);

                    ManageOPCUAMBtnPause(readResult["pcM1Pause"], checkBoxM1Pause);
                    ManageOPCUAMBtnPause(readResult["pcM2Pause"], checkBoxM2Pause);
                    ManageOPCUAMBtnPause(readResult["pcM3Pause"], checkBoxM3Pause);
                    ManageOPCUAMBtnPause(readResult["pcM4Pause"], checkBoxM4Pause);
                    ManageOPCUAMBtnPause(readResult["pcM5Pause"], checkBoxM5Pause);
                    ManageOPCUAMBtnPause(readResult["pcM6Pause"], checkBoxM6Pause);

                    ManagePauseStatus(readResult["pcM1Pause"], readResult["pcM2Pause"], readResult["pcM3Pause"], readResult["pcM4Pause"],
                        readResult["pcM5Pause"], readResult["pcM6Pause"]);

                    #endregion

                    #region (* machine homing done *)
                    keys = new List<string>()
                    {
                        "pcM1HomingDone",
                        "pcM2HomingDone",
                        "pcM3HomingDone",
                        "pcM4HomingDone",
                        //"pcM5HomingDone",
                        "pcM6HomingDone"
                    };

                    readResult = await ccService.Read(keys);
                    //UpdateOPCUAMHomingDone(readResult["pcM1HomingDone"], lbLedM1HomingDone);
                    //UpdateOPCUAMHomingDone(readResult["pcM2HomingDone"], lbLedM2HomingDone);
                    //UpdateOPCUAMHomingDone(readResult["pcM3HomingDone"], lbLedM3HomingDone);
                    //UpdateOPCUAMHomingDone(readResult["pcM4HomingDone"], lbLedM4HomingDone);
                    //UpdateOPCUAMHomingDone(readResult["pcM5HomingDone"], lbLedM5HomingDone);
                    //UpdateOPCUAMHomingDone(readResult["pcM6HomingDone"], lbLedM6HomingDone);
                    #endregion

                    #region(* machine current vertical axis quote *9
                    keys = new List<string>()
                    {
                        "pcM1CurrentAxisQuote",
                        "pcM2CurrentAxisQuote",
                        "pcM3CurrentAxisQuote",
                        "pcM4CurrentAxisQuote"
                        //"pcM5CurrentAxisQuote",
                        //"pcM6CurrentAxisQuote"
                    };

                    readResult = await ccService.Read(keys);
                    UpdateOPCUAM1CAQ(readResult["pcM1CurrentAxisQuote"]);
                    UpdateOPCUAM2CAQ(readResult["pcM2CurrentAxisQuote"]);
                    UpdateOPCUAM3CAQ(readResult["pcM3CurrentAxisQuote"]);
                    UpdateOPCUAM4CAQ(readResult["pcM4CurrentAxisQuote"]);
                    #endregion

                    #region(* machine point reached - teach*)
                    keys = new List<string>()
                    {
                        //"pcM1PointReached",
                        "pcM2PointReached",
                        "pcM3PointReached"
                        //"pcM4PointReached",
                        //"pcM5PointReached",
                        //"pcM6PointReached"
                    };

                    readResult = await ccService.Read(keys);
                    UpdateOPCUAM2PointReached(readResult["pcM2PointReached"]);
                    #endregion

                    #region(* node keep alive/connection *)
                    //keep alive to plc
                    keys = new List<string>()
                    {
                        "pcM1KeepAliveW",
                        "pcM2KeepAliveW",
                        "pcM3KeepAliveW",
                        "pcM4KeepAliveW",
                        "pcM5KeepAliveW"
                    };

                    readResult = await ccService.Read(keys);
                    UpdateOPCUAMKeepAlive(readResult["pcM1KeepAliveW"], lbLedM1PCKeepAlive);
                    UpdateOPCUAMKeepAlive(readResult["pcM2KeepAliveW"], lbLedM2PCKeepAlive);
                    UpdateOPCUAMKeepAlive(readResult["pcM3KeepAliveW"], lbLedM3PCKeepAlive);
                    UpdateOPCUAMKeepAlive(readResult["pcM4KeepAliveW"], lbLedM4PCKeepAlive);
                    UpdateOPCUAMKeepAlive(readResult["pcM5KeepAliveW"], lbLedM5PCKeepAlive);

                    //keep alive from plc
                    keys = new List<string>()
                    {
                        "pcM1KeepAliveR",
                        "pcM2KeepAliveR",
                        "pcM3KeepAliveR",
                        "pcM4KeepAliveR",
                        "pcM5KeepAliveR"
                        //"pcM6KeepAliveR"
                    };

                    var readResultPLC = await ccService.Read(keys);
                    UpdateOPCUAMNodeConnection(readResultPLC["pcM1KeepAliveR"], lastLifeBit[0], ref LifeBitTimeout[0], ref LifeBitCounter[0], pictureBoxM1PLCNode);
                    UpdateOPCUAMNodeConnection(readResultPLC["pcM2KeepAliveR"], lastLifeBit[1], ref LifeBitTimeout[1], ref LifeBitCounter[1], pictureBoxM2PLCNode);
                    UpdateOPCUAMNodeConnection(readResultPLC["pcM3KeepAliveR"], lastLifeBit[2], ref LifeBitTimeout[2], ref LifeBitCounter[2], pictureBoxM3PLCNode);
                    UpdateOPCUAMNodeConnection(readResultPLC["pcM4KeepAliveR"], lastLifeBit[3], ref LifeBitTimeout[3], ref LifeBitCounter[3], pictureBoxM4PLCNode);
                    UpdateOPCUAMNodeConnection(readResultPLC["pcM5KeepAliveR"], lastLifeBit[4], ref LifeBitTimeout[4], ref LifeBitCounter[4], pictureBoxM5PLCNode);
                    ManageLastLifeBit(readResultPLC["pcM1KeepAliveR"], ref lastLifeBit[0]);
                    ManageLastLifeBit(readResultPLC["pcM2KeepAliveR"], ref lastLifeBit[1]);
                    ManageLastLifeBit(readResultPLC["pcM3KeepAliveR"], ref lastLifeBit[2]);
                    ManageLastLifeBit(readResultPLC["pcM4KeepAliveR"], ref lastLifeBit[3]);
                    ManageLastLifeBit(readResultPLC["pcM5KeepAliveR"], ref lastLifeBit[4]);
                    #endregion

                    #region(* system status *)
                    //if all nodes are connected -> not in timeout
                    lbLedSystemConnection.State = (LifeBitTimeout[0] & LifeBitTimeout[1] & LifeBitTimeout[2] & LifeBitTimeout[3] & LifeBitTimeout[4]) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                    lbLedSystemConnection.Label = (LifeBitTimeout[0] & LifeBitTimeout[1] & LifeBitTimeout[2] & LifeBitTimeout[3] & LifeBitTimeout[4]) ? "online": "offline";

                    //emergenza, air pressure, 

                    #endregion

                    #region(* *)

                    #endregion
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

                    //manipulator digital input

                    keys = new List<string>()
                    {
                        "pcM2DI",
                        "pcM2DO"
                    };

                    readResult = await ccService.Read(keys);
                    UpdateOPCUAM2DI(readResult["pcM2DI"]);

                    #region (* GUI *)
                    GUIWithOPCUAClientConnected();
                    #endregion
                }
                else
                {
                    GUIWithOPCUAClientDisconnected();
                    ccService.Connect();
                }
            }
            catch (Exception Ex)
            {


            }
        }

        private void GUIWithOPCUAClientConnected()
        {
            pictureBoxIOTNode.Image = imageListNodes.Images[2];
        }

        private void GUIWithOPCUAClientDisconnected()
        {
            ManageOPCUAMBtnStatus(null, checkBoxM1Start);

            UpdateOPCUAMStatus(null, buttonM1Status, labelM1Status);
            ManageOPCUAMBtnStatus(null, checkBoxM1Start);

            UpdateOPCUAMStatus(null, buttonM2Status, labelM2Status);
            ManageOPCUAMBtnStatus(null, checkBoxM2Start);

            UpdateOPCUAMStatus(null, buttonM3Status, labelM3Status);
            ManageOPCUAMBtnStatus(null, checkBoxM3Start);

            UpdateOPCUAMStatus(null, buttonM4Status, labelM4Status);
            ManageOPCUAMBtnStatus(null, checkBoxM4Start);

            UpdateOPCUAMStatus(null, buttonM5Status, labelM5Status);
            ManageOPCUAMBtnStatus(null, checkBoxM5Start);

            UpdateOPCUAMStatus(null, buttonM6Status, labelM6Status);
            ManageOPCUAMBtnStatus(null, checkBoxM6Start);

            //UpdateOPCUAMHomingDone(null, lbLedM1HomingDone);
            //UpdateOPCUAMHomingDone(null, lbLedM2HomingDone);
            //UpdateOPCUAMHomingDone(null, lbLedM3HomingDone);
            //UpdateOPCUAMHomingDone(null, lbLedM4HomingDone);
            //UpdateOPCUAMHomingDone(null, lbLedM5HomingDone);
            //UpdateOPCUAMHomingDone(null, lbLedM6HomingDone);

            //UpdateOPCUAMHomingDone(null, lbLedM1HomingDone);
            //UpdateOPCUAMHomingDone(null, lbLedM2HomingDone);
            //UpdateOPCUAMHomingDone(null, lbLedM3HomingDone);
            //UpdateOPCUAMHomingDone(null, lbLedM4HomingDone);
            //UpdateOPCUAMHomingDone(null, lbLedM5HomingDone);
            //UpdateOPCUAMHomingDone(null, lbLedM6HomingDone);
            UpdateOPCUAM1CAQ(null);
            UpdateOPCUAM2CAQ(null);
            UpdateOPCUAM3CAQ(null);
            UpdateOPCUAM4CAQ(null);
            lbLedSystemConnection.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLedSystemConnection.Label = "system offline";
            
            pictureBoxM1PLCNode.Image = imageListNodes.Images[1];
            //labelM2Node.Text = "padprint int node offline";
            pictureBoxM2PLCNode.Image = imageListNodes.Images[1];
            pictureBoxM3PLCNode.Image = imageListNodes.Images[1];
            pictureBoxM4PLCNode.Image = imageListNodes.Images[1];
            pictureBoxM5PLCNode.Image = imageListNodes.Images[1];

            pictureBoxIOTNode.Image = imageListNodes.Images[3];

            lbLedM1PCKeepAlive.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLedM2PCKeepAlive.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLedM3PCKeepAlive.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLedM4PCKeepAlive.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLedM5PCKeepAlive.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
        }

        private void UpdateOPCUAMNodeConnection(ClientResult cr, bool oldValue, ref bool lBitTimeout, ref int lBitCounter,PictureBox pict)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                pict.Image = imageListNodes.Images[1];
                lBitTimeout = true;
            }
            else
            { 
                if (!lBitTimeout)
                {
                    if ((bool)cr.Value == oldValue)
                    {
                        lBitCounter = lBitCounter + 1;
                        if (lBitCounter > 100)
                        {
                            lBitTimeout = true;
                        }
                    }
                    else
                    {
                        lBitCounter = 0;
                        lBitTimeout = false;
                        pict.Image = imageListNodes.Images[0];
                    }
                }
                else
                {
                    if ((bool)cr.Value != oldValue)
                    {
                        lBitCounter = 0;
                        lBitTimeout = false;                     
                        pict.Image = imageListNodes.Images[0];
                    }
                    else
                    {                       
                        pict.Image = imageListNodes.Images[1];
                    }
                }
            }
        }

        public void ManageLastLifeBit(ClientResult cr, ref bool lastLifeBit)
        {
            if ((cr.OpcResult == false) || (cr == null))
            {
                //todo manage error
            }
            else
            {
                lastLifeBit = (bool)cr.Value;
            }
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

        public void UpdateOPCUAM2DI(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                int i = 0;
                bool[] arrayBool = (bool[])cr.Value;

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
        }

        public void UpdateOPCUAM2PointReached(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo offline message
            }
            else 
            {
                //todo ,amage error
                return;

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

                bool[] arrayBool = (bool[])cr.Value;
                int i = 0;
                for (i = 1; i < arrayBool.Count(); i++)
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
        }

        public void UpdateOPCUAM1CAQ(ClientResult cr)
        {
           // WriteOnLabelAsync(cr, labelM1TeachAxisQuoteValue);
        }

        public void UpdateOPCUAM2CAQ(ClientResult cr)
        {
            WriteOnLabelAsync(cr, labelM2TeachAxisQuoteValue);
        }

        public void UpdateOPCUAM3CAQ(ClientResult cr)
        {
            WriteOnLabelAsync(cr, labelM3TeachAxisQuoteValue);
        }

        public void UpdateOPCUAM4CAQ(ClientResult cr)
        {
            WriteOnLabelAsync(cr, labelM4TeachAxisQuoteValue);
        }

        public async void UpdateOPCUAMKeepAlive(ClientResult cr, LBSoft.IndustrialCtrls.Leds.LBLed lblLed)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo manage error
            }
            else
            { 
                if ((bool)cr.Value)
                {
                    var sendResult = await ccService.Send(cr.Key, false);
                    lblLed.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                }
                else
                {
                    var sendResult = await ccService.Send(cr.Key, true);
                    lblLed.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On;
                }
            }
        }


        public void UpdateOPCUAMStatus(ClientResult cr, Button btn, Label lbl)
        {
            short value = -1;

            if ((cr == null) || (cr.OpcResult == false))
            {
                btn.BackColor = Color.Black;               
            }
            else
            {
                value = (short)cr.Value;

                if (value == 0)
                {
                    btn.BackColor = Color.Red;
                }

                if (value == 1)
                {
                    btn.BackColor = Color.Blue;
                }

                if (value == 2)
                {
                    btn.BackColor = Color.Orange;
                }

                if (value == 3)
                {
                    btn.BackColor = Color.FromArgb(107, 227, 162);
                }

                if (value == 4)
                {
                    btn.BackColor = Color.DarkOrange;
                }
            }
            WriteOnLabelStatusAsync(cr, lbl);
        }       

        public void ManageOPCUAMBtnStatus(ClientResult cr, CheckBox btn)
        {
            WriteOnCheckBoxStartAsync(cr, btn);
        }

        public void ManageOPCUAMBtnPause(ClientResult cr, CheckBox btn)
        {
            WriteOnCheckBoxPauseAsync(cr, btn);
        }

        public void ManageStartStatus(ClientResult crM1, ClientResult crM2, ClientResult crM3, ClientResult crM4, ClientResult crM5, ClientResult crM6)
        {
            if ((crM1 == null) || (crM2 == null) || (crM3 == null) || (crM4 == null) || (crM5 == null) || (crM6 == null) ||
                    (crM1.OpcResult == false) || (crM2.OpcResult == false) || (crM3.OpcResult == false) || (crM4.OpcResult == false) || (crM5.OpcResult == false) || (crM6.OpcResult == false))
            {
                //todo: manage error
            }
            else
            {
                if ((short)crM1.Value == 3 & (short)crM2.Value == 3 & (short)crM3.Value == 3 & (short)crM4.Value == 3 & (short)crM5.Value == 3 & (short)crM6.Value == 3)
                {                    
                    WriteAsyncMStartStopCheckBox(true, checkBoxStartStop);
                }
                else
                {
                    
                }
            }
        }

      

        public void ManagePauseStatus(ClientResult crM1, ClientResult crM2, ClientResult crM3, ClientResult crM4, ClientResult crM5, ClientResult crM6)
        {
            if ((crM1 == null) || (crM2 == null) || (crM3 == null) || (crM4 == null) || (crM5 == null) || (crM6 == null) ||
                    (crM1.OpcResult == false) || (crM2.OpcResult == false) || (crM3.OpcResult == false) || (crM4.OpcResult == false) || (crM5.OpcResult == false) || (crM6.OpcResult == false))
            {
                //todo: manage error
            }
            else
            {

                if ((bool)crM1.Value == true & (bool)crM2.Value == true & (bool)crM3.Value == true & (bool)crM4.Value == true & (bool)crM5.Value == true & (bool)crM6.Value == true)
                {
                    WriteAsyncMPauseCheckBox(true, checkBoxPause);
                }
                else
                {
                    //WriteAsyncMPauseCheckBox(false, checkBoxPause);
                }
            }

        }

        public void WriteAsyncMStartStopCheckBox(bool value, CheckBox chk)
        {
            if (chk.InvokeRequired)
            {
                chk.Invoke((MethodInvoker)delegate
                {
                    if (value == true)
                    {
                        chk.CheckState = CheckState.Checked;
                        chk.Image = imageListStartStop.Images[1];
                        chk.Text = "STOP";
                    }

                    if (value == false)
                    {
                        chk.CheckState = CheckState.Unchecked;
                        chk.Image = imageListStartStop.Images[0];
                        chk.Text = "START";
                    }
                });
            }
        }

        //public void WriteAsyncMStartStopNotInCycle(bool value, CheckBox chk)
        //{
        //    if (chk.InvokeRequired)
        //    {
        //        chk.Invoke((MethodInvoker)delegate
        //        {
        //            if (value == true)
        //            {
        //                chk.CheckState = CheckState.Checked;
        //                chk.Image = imageListStartStop.Images[1];
        //                chk.Text = "STOP";
        //                chk.CheckState = CheckState.Checked;
        //            }

        //            if (value == false)
        //            {
        //                chk.CheckState = CheckState.Unchecked;
        //                chk.Image = imageListStartStop.Images[0];
        //                chk.Text = "START";
        //                chk.CheckState = CheckState.Unchecked;
        //            }
        //        });
        //    }
        //}

        public void WriteAsyncMPauseCheckBox(bool value, CheckBox chk)
        {
            if (chk.InvokeRequired)
            {
                chk.Invoke((MethodInvoker)delegate
                {
                    if (value == true)
                    {
                        chk.CheckState = CheckState.Checked;
                        chk.Image = imageListStartStop.Images[3];
                        chk.Text = "IN PAUSE";
                        chk.CheckState = CheckState.Checked;
                    }

                    if (value == false)
                    {
                        chk.CheckState = CheckState.Unchecked;
                        chk.Image = imageListStartStop.Images[2];
                        chk.Text = "PAUSE";
                        chk.CheckState = CheckState.Unchecked;
                    }
                });
            }
        }

        public void UpdateOPCUAMHomingDone(ClientResult cr, LBSoft.IndustrialCtrls.Leds.LBLed lblLed)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                lblLed.LedColor = Color.Black;
                lblLed.Label = "offline";
            }
            else
            {
                if (!(bool)cr.Value)
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
