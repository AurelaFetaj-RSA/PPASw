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
    public partial class FormApp : Form
    {
        Dictionary<string, DiagnosticWindowsControl> DiagnosticVariableGroupbox = new Dictionary<string, DiagnosticWindowsControl>();
        private bool[] lastLifeBit { get; set; } = { false, false, false, false, false, false };
        private int[] LifeBitCounter { get; set; } = { 0, 0, 0, 0, 0, 0 };
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

        //public int counter = 0;
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

                    #region(* machine pause status *)
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
                    UpdateOPCUAMNodeConnection(readResultPLC["pcM1KeepAliveR"], lastLifeBit[0], ref LifeBitTimeout[0], ref LifeBitCounter[0], pictureBoxM1PLCNode, labelM1Node);
                    UpdateOPCUAMNodeConnection(readResultPLC["pcM2KeepAliveR"], lastLifeBit[1], ref LifeBitTimeout[1], ref LifeBitCounter[1], pictureBoxM2PLCNode, labelM2Node);
                    UpdateOPCUAMNodeConnection(readResultPLC["pcM3KeepAliveR"], lastLifeBit[2], ref LifeBitTimeout[2], ref LifeBitCounter[2], pictureBoxM3PLCNode, labelM3Node);
                    UpdateOPCUAMNodeConnection(readResultPLC["pcM4KeepAliveR"], lastLifeBit[3], ref LifeBitTimeout[3], ref LifeBitCounter[3], pictureBoxM4PLCNode, labelM4Node);
                    UpdateOPCUAMNodeConnection(readResultPLC["pcM5KeepAliveR"], lastLifeBit[4], ref LifeBitTimeout[4], ref LifeBitCounter[4], pictureBoxM5PLCNode, labelM5Node);
                    ManageLastLifeBit(readResultPLC["pcM1KeepAliveR"], ref lastLifeBit[0]);
                    ManageLastLifeBit(readResultPLC["pcM2KeepAliveR"], ref lastLifeBit[1]);
                    ManageLastLifeBit(readResultPLC["pcM3KeepAliveR"], ref lastLifeBit[2]);
                    ManageLastLifeBit(readResultPLC["pcM4KeepAliveR"], ref lastLifeBit[3]);
                    ManageLastLifeBit(readResultPLC["pcM5KeepAliveR"], ref lastLifeBit[4]);
                    UpdateOPCUAMNodeConnection(readResultPLC["pcM1KeepAliveR"], lastLifeBit[0], ref LifeBitTimeout[0], ref LifeBitCounter[0], pictureBoxM1PLCNode, labelM1Node);
                    //if all nodes are connected -> not in timeout
                    //UpdateOPCUASystemConnection((LifeBitTimeout[0] & LifeBitTimeout[1] & LifeBitTimeout[2] & LifeBitTimeout[3] & LifeBitTimeout[4]), buttonSystemStatus, labelSystemStatus);
                    WriteOnToolStripAsync((LifeBitTimeout[0] & LifeBitTimeout[1] & LifeBitTimeout[2] & LifeBitTimeout[3] & LifeBitTimeout[4]), toolStripStatusLabelSystem);
                    #endregion

                    #region (* machine homing done status *)
                    keys = new List<string>()
                    {
                        "pcM1HomingDone",
                        "pcM2HomingDone",
                        "pcM3HomingDone",
                        "pcM4HomingDone"
                        //"pcM5HomingDone",
                        //"pcM6HomingDone"
                    };

                    readResult = await ccService.Read(keys);
                    UpdateOPCUAMHomingDone(readResult["pcM1HomingDone"], buttonM1HomingDone, labelM1HomingDone);
                    UpdateOPCUAMHomingDone(readResult["pcM2HomingDone"], buttonM2HomingDone, labelM2HomingDone);
                    UpdateOPCUAMHomingDone(readResult["pcM3HomingDone"], buttonM3HomingDone, labelM3HomingDone);
                    UpdateOPCUAMHomingDone(readResult["pcM4HomingDone"], buttonM4HomingDone, labelM4HomingDone);
                    //UpdateOPCUAMHomingDone(readResult["pcM5HomingDone"], buttonM5HomingDone, labelM5HomingDone);
                    //UpdateOPCUAMHomingDone(readResult["pcM6HomingDone"], buttonM6HomingDone, labelM6HomingDone);
                    #endregion

                    #region(* machine runtime vertical axis quote * )
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
                    WriteOnLabelAsync(readResult["pcM1CurrentAxisQuote"], labelM1TeachAxisQuoteValue);
                    WriteOnLabelAsync(readResult["pcM2CurrentAxisQuote"], labelM2TeachAxisQuoteValue);
                    WriteOnLabelAsync(readResult["pcM3CurrentAxisQuote"], labelM3TeachAxisQuoteValue);
                    WriteOnLabelAsync(readResult["pcM4CurrentAxisQuote"], labelM4TeachAxisQuoteValue);
                    #endregion

                    #region(* machine point reached - teach*)
                    keys = new List<string>()
                    {
                        "pcM1PointReached",
                        "pcM2PointReached",
                        "pcM3PointReached"
                        //"pcM4PointReached",
                        //"pcM5PointReached",
                        //"pcM6PointReached"
                    };

                    readResult = await ccService.Read(keys);
                    WriteAsyncDataGridViewPointReached(readResult["pcM1PointReached"], dataGridViewM1TeachPoints);

                    readResult = await ccService.Read(keys);
                    WriteAsyncDataGridViewPointReached(readResult["pcM2PointReached"], dataGridViewM2TeachPoints);
                    //force repaint
                    dataGridViewM2TeachPoints.Invalidate(false);
                    
                    WriteAsyncDataGridViewPointReached(readResult["pcM3PointReached"], dataGridViewM3TeachPoints);
                    //force repaint
                    dataGridViewM3TeachPoints.Invalidate(false);
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

                    //M1 input/output
                    keys = new List<string>()
                    {
                        "pcM1DI",
                        "pcM1DO"
                    };

                    readResult = await ccService.Read(keys);
                    UpdateOPCUAM1DI(readResult["pcM1DI"]);
                    UpdateOPCUAM1DO(readResult["pcM1DO"]);

                    keys = new List<string>()
                    {
                        "pcM2DI",
                        "pcM2DO"
                    };

                    readResult = await ccService.Read(keys);
                    UpdateOPCUAM2DI(readResult["pcM2DI"]);
                   
                    #region (* M1 timeout alarms *)
                    keys = new List<string>()
                    {
                        "pcM1TimeoutAlarms",
                        "pcM1GeneralAlarms"
                    };
                    readResult = await ccService.Read(keys);
                    UpdateOPCUAM1TimeoutAlarms(readResult["pcM1TimeoutAlarms"]);
                    UpdateOPCUAM1GeneralAlarms(readResult["pcM1GeneralAlarms"]);

                    keys = new List<string>()
                    {
                        "pcM2TimeoutAlarms",
                        "pcM2GeneralAlarms"
                    };
                    readResult = await ccService.Read(keys);
                    UpdateOPCUAM2TimeoutAlarms(readResult["pcM2TimeoutAlarms"]);
                    UpdateOPCUAM2GeneralAlarms(readResult["pcM2GeneralAlarms"]);

                    keys = new List<string>()
                    {
                        "pcM4TimeoutAlarms",
                        "pcM4GeneralAlarms"
                    };

                    readResult = await ccService.Read(keys);
                    UpdateOPCUAM4TimeoutAlarms(readResult["pcM4TimeoutAlarms"]);
                    UpdateOPCUAM4GeneralAlarms(readResult["pcM4GeneralAlarms"]);

                    keys = new List<string>()
                    {
                        "pcM3TimeoutAlarms",
                        "pcM3GeneralAlarms"
                    };

                    readResult = await ccService.Read(keys);
                    UpdateOPCUAM3TimeoutAlarms(readResult["pcM3TimeoutAlarms"]);
                    UpdateOPCUAM3GeneralAlarms(readResult["pcM3GeneralAlarms"]);

                    keys = new List<string>()
                    {
                        "pcM5TimeoutAlarms",
                        "pcM5GeneralAlarms"
                    };

                    readResult = await ccService.Read(keys);
                    UpdateOPCUAM5TimeoutAlarms(readResult["pcM5TimeoutAlarms"]);
                    UpdateOPCUAM5GeneralAlarms(readResult["pcM5GeneralAlarms"]);
                    #endregion

                    #region(* cycle counter *)
                    keys = new List<string>()
                    {
                        "pcM1CycleCounter",
                        "pcM2CycleCounter",
                        "pcM3CycleCounter",
                        "pcM4CycleCounter",
                        "pcM5CycleCounter"
                    };

                    readResultPLC = await ccService.Read(keys);
                    WriteOnLabelAsync(readResultPLC["pcM1CycleCounter"], labelM1CycleId);
                    WriteOnLabelAsync(readResultPLC["pcM2CycleCounter"], labelM2CycleId);
                    WriteOnLabelAsync(readResultPLC["pcM3CycleCounter"], labelM3CycleId);
                    WriteOnLabelAsync(readResultPLC["pcM4CycleCounter"], labelM4CycleId);
                    WriteOnLabelAsync(readResultPLC["pcM5CycleCounter"], labelM5CycleId);                    

                    

                    #endregion

                    #region(* state machine *)
                    keys = new List<string>()
                    {
                        "pcM1TrimmingState",
                        "pcM1LoadingBeltState",
                        "pcM1WorkingBeltState",
                        "pcM1ExitBeltState"
                    };

                    readResultPLC = await ccService.Read(keys);
                    WriteOnLabelAsync(readResultPLC["pcM1TrimmingState"], labelM1TrimmerState);
                    WriteOnLabelAsync(readResultPLC["pcM1LoadingBeltState"], labelM1LoadingBeltState);
                    WriteOnLabelAsync(readResultPLC["pcM1WorkingBeltState"], labelM1WorkingBeltState);
                    WriteOnLabelAsync(readResultPLC["pcM1ExitBeltState"], labelM1ExitBeltState);

                    keys = new List<string>()
                    {
                        "pcM4State",                        
                        "pcM4WorkingBeltState",
                        "pcM4ExitBeltState"
                    };

                    readResultPLC = await ccService.Read(keys);
                    WriteOnLabelAsync(readResultPLC["pcM4State"], labelM4State);                    
                    WriteOnLabelAsync(readResultPLC["pcM4WorkingBeltState"], labelM4WorkingBeltState);
                    WriteOnLabelAsync(readResultPLC["pcM4ExitBeltState"], labelM4ExitBeltState);

                    keys = new List<string>()
                    {
                        "pcM3PadPrintExtState",
                        "pcM3WorkingBeltState",
                        "pcM3ExitBeltState"
                    };

                    readResultPLC = await ccService.Read(keys);
                    WriteOnLabelAsync(readResultPLC["pcM3PadPrintExtState"], labelM3State);
                    WriteOnLabelAsync(readResultPLC["pcM3WorkingBeltState"], labelM3WorkingBeltState);
                    WriteOnLabelAsync(readResultPLC["pcM3ExitBeltState"], labelM3ExitBeltState);

                    keys = new List<string>()
                    {
                        "pcM2PadPrintIntState",
                        "pcM2WorkingBeltState",
                        "pcM2ExitBeltState"
                    };

                    readResultPLC = await ccService.Read(keys);
                    WriteOnLabelAsync(readResultPLC["pcM2PadPrintIntState"], labelM2State);
                    WriteOnLabelAsync(readResultPLC["pcM2WorkingBeltState"], labelM2WorkingBeltState);
                    WriteOnLabelAsync(readResultPLC["pcM2ExitBeltState"], labelM2ExitBeltState);

                    keys = new List<string>()
                    {
                        "pcM5State",
                        "pcM5TranslationBeltState",
                        "pcM5ExitBelt1State",
                        "pcM5ExitBelt2State",
                        "pcM5ExitBelt3State"
                    };

                    readResultPLC = await ccService.Read(keys);
                    WriteOnLabelAsync(readResultPLC["pcM5State"], labelM5State);
                    WriteOnLabelAsync(readResultPLC["pcM5TranslationBeltState"], labelM5TranslatorBeltState);
                    WriteOnLabelAsync(readResultPLC["pcM5ExitBelt1State"], labelM5ExitBelt1State);
                    WriteOnLabelAsync(readResultPLC["pcM5ExitBelt2State"], labelM5ExitBelt2State);
                    WriteOnLabelAsync(readResultPLC["pcM5ExitBelt3State"], labelM5ExitBelt3State);

                    #endregion





                    #region (* GUI *)
                    GUIWithOPCUAClientConnected();
                    #endregion
                }
                else
                {
                    GUIWithOPCUAClientDisconnected();
                    await ccService.Connect();
                }
            }
            catch (Exception Ex)
            {


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
        public void WriteAsyncOnCheckBoxPause(ClientResult cr, CheckBox chk)
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
        public void WriteOnLabelMStatusAsync(ClientResult cr, Label lbl)
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
                        lbl.Text = "emergency";
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

        public void WriteOnLabelAsync(Label lbl, string textToSet)
        {
            if (lbl.InvokeRequired)
            {
                lbl.Invoke((MethodInvoker)delegate
                {
                    lbl.Text = textToSet;
                });
            }
        }

        public void WriteOnToolStripAsync(bool value, ToolStripLabel lbl)
        {
            if (lbl.GetCurrentParent().InvokeRequired)
            {
                lbl.GetCurrentParent().Invoke((MethodInvoker)delegate
                {
                    if (value)
                    {
                        lbl.BackColor = Color.FromArgb(107, 227, 162);
                        lbl.ForeColor = Color.FromArgb(51, 51, 51);
                        lbl.Text = "system online";
                    }
                    else
                    {
                        lbl.BackColor = Color.Red;
                        lbl.ForeColor = Color.FromArgb(241, 241, 241);
                        lbl.Text = "system offline";
                    }
                });
                
            }
        }

        #region (* machines status *)


        private void GUIWithOPCUAClientConnected()
        {
            pictureBoxIOTNode.Image = imageListNodes.Images[2];
        }

        private void GUIWithOPCUAClientDisconnected()
        {
            //machines status
            UpdateOPCUAMStatus(null, buttonM1Status, labelM1Status);
            WriteOnCheckBoxStartAsync(null, checkBoxM1Start);
            WriteAsyncOnCheckBoxPause(null, checkBoxM1Pause);

            UpdateOPCUAMStatus(null, buttonM2Status, labelM2Status);
            WriteOnCheckBoxStartAsync(null, checkBoxM2Start);
            WriteAsyncOnCheckBoxPause(null, checkBoxM2Pause);

            UpdateOPCUAMStatus(null, buttonM3Status, labelM3Status);
            WriteOnCheckBoxStartAsync(null, checkBoxM3Start);
            WriteAsyncOnCheckBoxPause(null, checkBoxM3Pause);

            UpdateOPCUAMStatus(null, buttonM4Status, labelM4Status);
            WriteOnCheckBoxStartAsync(null, checkBoxM4Start);
            WriteAsyncOnCheckBoxPause(null, checkBoxM4Pause);

            UpdateOPCUAMStatus(null, buttonM5Status, labelM5Status);
            WriteOnCheckBoxStartAsync(null, checkBoxM5Start);
            WriteAsyncOnCheckBoxPause(null, checkBoxM5Pause);

            UpdateOPCUAMStatus(null, buttonM6Status, labelM6Status);
            WriteOnCheckBoxStartAsync(null, checkBoxM6Start);
            WriteAsyncOnCheckBoxPause(null, checkBoxM6Pause);

            //line status
            WriteOnToolStripAsync(false, toolStripStatusLabelSystem);

            WriteAsyncSystemStartStopCheckBox(0, checkBoxStartStop);
            WriteAsyncSystemPauseCheckBox(0, checkBoxPause);

            pictureBoxM1PLCNode.Image = imageListNodes.Images[1];
            WriteOnLabelAsync(labelM1Node, "node offline");
            pictureBoxM2PLCNode.Image = imageListNodes.Images[1];
            WriteOnLabelAsync(labelM2Node, "node offline");
            pictureBoxM3PLCNode.Image = imageListNodes.Images[1];
            WriteOnLabelAsync(labelM3Node, "node offline");
            pictureBoxM4PLCNode.Image = imageListNodes.Images[1];
            WriteOnLabelAsync(labelM4Node, "node offline");
            pictureBoxM5PLCNode.Image = imageListNodes.Images[1];
            WriteOnLabelAsync(labelM5Node, "node offline");

            pictureBoxIOTNode.Image = imageListNodes.Images[3];

            lbLedM1PCKeepAlive.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLedM2PCKeepAlive.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLedM3PCKeepAlive.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLedM4PCKeepAlive.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLedM5PCKeepAlive.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;

            UpdateOPCUAMHomingDone(null, buttonM1HomingDone, labelM1HomingDone);
            UpdateOPCUAMHomingDone(null, buttonM2HomingDone, labelM2HomingDone);
            UpdateOPCUAMHomingDone(null, buttonM3HomingDone, labelM3HomingDone);
            UpdateOPCUAMHomingDone(null, buttonM4HomingDone, labelM4HomingDone);
            //UpdateOPCUAMHomingDone(readResult["pcM5HomingDone"], buttonM5HomingDone, labelM5HomingDone);
            //UpdateOPCUAMHomingDone(readResult["pcM6HomingDone"], buttonM6HomingDone, labelM6HomingDone);

            WriteOnLabelAsync(null, labelM1TeachAxisQuoteValue);
            WriteOnLabelAsync(null, labelM2TeachAxisQuoteValue);
            WriteOnLabelAsync(null, labelM3TeachAxisQuoteValue);
            WriteOnLabelAsync(null, labelM4TeachAxisQuoteValue);
            //WriteOnLabelAsync(null, labelM3TeachAxisQuoteValue);           
        }

        private void UpdateOPCUAMNodeConnection(ClientResult cr, bool oldValue, ref bool lBitTimeout, ref int lBitCounter, PictureBox pict, Label lbl)
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
                        WriteOnLabelAsync(lbl, "node online");
                    }
                }
                else
                {
                    if ((bool)cr.Value != oldValue)
                    {
                        lBitCounter = 0;
                        lBitTimeout = false;
                        pict.Image = imageListNodes.Images[0];
                        WriteOnLabelAsync(lbl, "node online");
                    }
                    else
                    {
                        pict.Image = imageListNodes.Images[1];
                        WriteOnLabelAsync(lbl, "node offline");
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

        public void UpdateOPCUAM1DI(ClientResult cr)
        {

            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                int i = 0;
                bool[] arrayBool = (bool[])cr.Value;

                lbLed1001M1.State = (arrayBool[1] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1002M1.State = (arrayBool[2] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1003M1.State = (arrayBool[3] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1004M1.State = (arrayBool[4] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1005M1.State = (arrayBool[5] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1006M1.State = (arrayBool[6] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1007M1.State = (arrayBool[7] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1008M1.State = (arrayBool[8] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1008M1.Label = (arrayBool[8] == true) ? "selector in AUTO" : "selector in MANUAL";
                lbLed1009M1.State = (arrayBool[9] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1010M1.State = (arrayBool[10] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1011M1.State = (arrayBool[11] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1012M1.State = (arrayBool[12] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1013M1.State = (arrayBool[13] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1014M1.State = (arrayBool[14] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1015M1.State = (arrayBool[15] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1016M1.State = (arrayBool[16] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;

                lbLed1017M1.State = (arrayBool[17] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1018M1.State = (arrayBool[18] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1019M1.State = (arrayBool[19] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1020M1.State = (arrayBool[20] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1021M1.State = (arrayBool[21] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1022M1.State = (arrayBool[22] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1023M1.State = (arrayBool[23] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1024M1.State = (arrayBool[24] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;

                lbLed1025M1.State = (arrayBool[25] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1026M1.State = (arrayBool[26] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1027M1.State = (arrayBool[27] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1028M1.State = (arrayBool[28] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1029M1.State = (arrayBool[29] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1030M1.State = (arrayBool[30] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1031M1.State = (arrayBool[31] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1032M1.State = (arrayBool[32] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;

                lbLed1033M1.State = (arrayBool[33] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1034M1.State = (arrayBool[34] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1035M1.State = (arrayBool[35] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1036M1.State = (arrayBool[36] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1037M1.State = (arrayBool[37] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1038M1.State = (arrayBool[38] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1039M1.State = (arrayBool[39] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1040M1.State = (arrayBool[40] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1041M1.State = (arrayBool[41] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1042M1.State = (arrayBool[42] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1043M1.State = (arrayBool[43] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1044M1.State = (arrayBool[44] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1045M1.State = (arrayBool[45] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1046M1.State = (arrayBool[46] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1047M1.State = (arrayBool[47] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1048M1.State = (arrayBool[48] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            }


        }

        public void UpdateOPCUAM1DO(ClientResult cr)
        {

            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                int i = 0;
                bool[] arrayBool = (bool[])cr.Value;

                lbLed2001M1.State = (arrayBool[1] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2002M1.State = (arrayBool[2] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2003M1.State = (arrayBool[3] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2004M1.State = (arrayBool[4] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2005M1.State = (arrayBool[5] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2006M1.State = (arrayBool[6] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2007M1.State = (arrayBool[7] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2008M1.State = (arrayBool[8] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2008M1.Label = (arrayBool[8] == true) ? "selector in AUTO" : "selector in MANUAL";
                lbLed2009M1.State = (arrayBool[9] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2010M1.State = (arrayBool[10] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2011M1.State = (arrayBool[11] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2012M1.State = (arrayBool[12] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2013M1.State = (arrayBool[13] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2014M1.State = (arrayBool[14] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2015M1.State = (arrayBool[15] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2016M1.State = (arrayBool[16] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;

                lbLed2017M1.State = (arrayBool[17] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2018M1.State = (arrayBool[18] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2019M1.State = (arrayBool[19] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2020M1.State = (arrayBool[20] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2021M1.State = (arrayBool[21] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2022M1.State = (arrayBool[22] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2023M1.State = (arrayBool[23] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2024M1.State = (arrayBool[24] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;

                lbLed2025M1.State = (arrayBool[25] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2026M1.State = (arrayBool[26] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2027M1.State = (arrayBool[27] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2028M1.State = (arrayBool[28] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2029M1.State = (arrayBool[29] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2030M1.State = (arrayBool[30] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2031M1.State = (arrayBool[31] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2032M1.State = (arrayBool[32] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;

                lbLed2033M1.State = (arrayBool[33] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2034M1.State = (arrayBool[34] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2035M1.State = (arrayBool[35] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2036M1.State = (arrayBool[36] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2037M1.State = (arrayBool[37] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2038M1.State = (arrayBool[38] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2039M1.State = (arrayBool[39] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2040M1.State = (arrayBool[40] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2041M1.State = (arrayBool[41] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2042M1.State = (arrayBool[42] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2043M1.State = (arrayBool[43] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2044M1.State = (arrayBool[44] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2045M1.State = (arrayBool[45] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2046M1.State = (arrayBool[46] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2047M1.State = (arrayBool[47] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed2048M1.State = (arrayBool[48] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            }


        }

        public void UpdateOPCUAM1TimeoutAlarms(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;

                for (i = alarms.Length - 1; i>=0;i--)
                {
                    string exe = alarms.Substring(i,1);
                    
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        Console.WriteLine("trimmer timeout start motore");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        Console.WriteLine("trimmer timeout homing motore");
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        Console.WriteLine("trimmer timeout start posizionamento motore");
                    }

                    if ((Int32.Parse(exe) == 1) &(i == alarms.Length - 4))
                    {
                        Console.WriteLine("trimmer timeout posizionamento motore");
                    }

                    if ((Int32.Parse(exe) == 1) &(i == alarms.Length - 5))
                    {
                        Console.WriteLine("trimmer timeout slitta motore taglio");
                    }

                    if ((Int32.Parse(exe) == 1) &(i == alarms.Length - 6))
                    {
                        Console.WriteLine("timeout pinza taglio stivale");
                    }

                    if ((Int32.Parse(exe) == 1) &(i == alarms.Length - 7))
                    {
                        Console.WriteLine("trimmer timeout pinza molle");
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        Console.WriteLine("trimmer timeout pinza blocco");
                    }
                }

            }
        }

        public void UpdateOPCUAM2TimeoutAlarms(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;

                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        Console.WriteLine("padint timeout start motore");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        Console.WriteLine("padint timeout homing motore");
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        Console.WriteLine("padint timeout start posizionamento motore");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        Console.WriteLine("padint timeout posizionamento motore");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        Console.WriteLine("padint timeout pinza bordo stivale");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        Console.WriteLine("padint timeout pinza grande stivale");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        Console.WriteLine("padint timeout pinza centraggio 1");
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        Console.WriteLine("padint timeout pinza centraggio 2");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 9))
                    {
                        Console.WriteLine("padint timeout contrasto");
                    }
                }

            }
        }

        public void UpdateOPCUAM4TimeoutAlarms(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;

                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        Console.WriteLine("timeout start motore");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        Console.WriteLine("timeout homing motore");
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        Console.WriteLine("timeout start posizionamento motore");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        Console.WriteLine("timeout posizionamento motore");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        Console.WriteLine("timeout pinza bordo stivale");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        Console.WriteLine("timeout pinza grande stivale");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        Console.WriteLine("timeout pinza centraggio 1");
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        Console.WriteLine("timeout pinza centraggio 2");
                    }
                }

            }
        }

        public void UpdateOPCUAM3TimeoutAlarms(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;

                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        Console.WriteLine("padext timeout start motore");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        Console.WriteLine("padext timeout homing motore");
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        Console.WriteLine("padext timeout start posizionamento motore");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        Console.WriteLine("padext timeout posizionamento motore");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        Console.WriteLine("padext timeout pinza bordo stivale");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        Console.WriteLine("padext timeout pinza grande stivale");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        Console.WriteLine("padext timeout pinza centraggio 1");
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        Console.WriteLine("padext timeout pinza centraggio 2");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 9))
                    {
                        Console.WriteLine("padext timeout contrasto");
                    }
                }

            }
        }

        public void UpdateOPCUAM5TimeoutAlarms(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;

                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        Console.WriteLine("manip timeout traslatore");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        Console.WriteLine("manip timeout avanzamento pinza");
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        Console.WriteLine("manip timeout pinza");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        Console.WriteLine("manip timeout rotazione antioraria");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        Console.WriteLine("manip timeout rotazione oraria");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        Console.WriteLine("manip timeout estensione verticale 1");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        Console.WriteLine("manip timeout estensione verticale 2");
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                    //    Console.WriteLine("timeout pinza centraggio 2");
                    }
                }

            }
        }

        public void UpdateOPCUAM1GeneralAlarms(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;

                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        Console.WriteLine("trimmer motore nastro carico in allarme");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        Console.WriteLine("trimmer motore nastro lavoro in allarme");
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        Console.WriteLine("trimmer servo in allarme");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        Console.WriteLine("trimmer dispositivo fpi4C disconnesso");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        Console.WriteLine("");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        Console.WriteLine("");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        Console.WriteLine("");
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        Console.WriteLine("");
                    }
                }

            }
        }

        public void UpdateOPCUAM4GeneralAlarms(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;

                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        Console.WriteLine("motore nastro lavoro in allarme");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        Console.WriteLine("motore nastro uscita in allarme");
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        Console.WriteLine("servo in allarme");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        Console.WriteLine("HMI disconnesso");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        Console.WriteLine("air pressure missing");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        //Console.WriteLine("");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        //Console.WriteLine("");
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        //Console.WriteLine("");
                    }
                }

            }
        }

        public void UpdateOPCUAM3GeneralAlarms(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;

                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        Console.WriteLine("motore nastro lavoro in allarme");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        Console.WriteLine("motore nastro uscita in allarme");
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        Console.WriteLine("servo in allarme");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        Console.WriteLine("FP-i4C disconnesso");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        Console.WriteLine("air pressure missing");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        Console.WriteLine("inverter aspirazione forno");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        Console.WriteLine("resistenza controllo temperatura in allarme");
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        Console.WriteLine("allarme padprint");
                    }
                }

            }
        }

        public void UpdateOPCUAM2GeneralAlarms(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;

                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        Console.WriteLine("padint motore nastro lavoro in allarme");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        Console.WriteLine("padint motore nastro uscita in allarme");
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        Console.WriteLine("padint servo in allarme");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        Console.WriteLine("padint FP-i4C disconnesso");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        Console.WriteLine("padint air pressure missing");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        Console.WriteLine("padint allarme padprint");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        
                    }
                }

            }
        }

        public void UpdateOPCUAM5GeneralAlarms(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;

                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        Console.WriteLine("manip motore nastro traslazione in allarme");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        Console.WriteLine("manip motore nastro uscita uscita 1 in allarme");
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        Console.WriteLine("manip motore nastro uscita 2 in allarme");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        Console.WriteLine("manip motore nastro uscita 3 in allarme");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        Console.WriteLine("manip dispositivo opc disconnesso");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        Console.WriteLine("manip air pressure missing");
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        //Console.WriteLine("");
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        //Console.WriteLine("");
                    }
                }

            }
        }

        public string ToBinary(int n)
        {
            if (n < 2) return n.ToString();

            var divisor = n / 2;
            var remainder = n % 2;

            return ToBinary(divisor) + remainder;
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
                lbLed1002M2_T.State = lbLed1002M2.State;
                lbLed1002M2.State = (arrayBool[2] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1003M2_T.State = lbLed1003M2.State;
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
                lbLed1016M2_T.State = lbLed1016M2.State;
                lbLed1017M2.State = (arrayBool[17] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1018M2.State = (arrayBool[18] == true) ? LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On : LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                lbLed1018M2_T.State = lbLed1018M2.State;
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
                lbLed1028M2_T.State = lbLed1028M2.State;
                lbLed1029M2_T.State = lbLed1029M2.State;
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

        public void WriteAsyncDataGridViewPointReached(ClientResult cr, DataGridView dt)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo offline message
            }
            else
            {
                if (dt.InvokeRequired)
                {
                    dt.Invoke((MethodInvoker)delegate
                    {
                        bool[] arrayBool = (bool[])cr.Value;
                        int i = 0;
                        for (i = 1; i < arrayBool.Count(); i++)
                        {
                            if (arrayBool[i])
                            {
                                dt.Rows[i - 1].Cells[5].ToolTipText = "reached";
                            }
                            else
                            {
                                dt.Rows[i - 1].Cells[5].ToolTipText = "notreached";
                            }
                        }
                    });
                }            
            }
        }      

        public async void UpdateOPCUAMKeepAlive(ClientResult cr, LBSoft.IndustrialCtrls.Leds.LBLed lblLed)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo manage error
                lblLed.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Blink;
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

                
        public void UpdateOPCUASystemConnection(bool value, Button btn, Label lbl)
        {
            
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
                    btn.BackColor = Color.FromArgb(59, 130, 246);
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
            WriteOnLabelMStatusAsync(cr, lbl);
        }       

        public void ManageOPCUAMBtnStatus(ClientResult cr, CheckBox btn)
        {
            WriteOnCheckBoxStartAsync(cr, btn);
        }

        public void ManageOPCUAMBtnPause(ClientResult cr, CheckBox btn)
        {
            WriteAsyncOnCheckBoxPause(cr, btn);
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
                    WriteAsyncSystemStartStopCheckBox(1, checkBoxStartStop);
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
                    WriteAsyncSystemPauseCheckBox(1, checkBoxPause);
                }
                else
                {
                    //WriteAsyncMPauseCheckBox(false, checkBoxPause);
                }
            }

        }

        public void WriteAsyncSystemStartStopCheckBox(int value, CheckBox chk)
        {
            if (chk.InvokeRequired)
            {
                chk.Invoke((MethodInvoker)delegate
                {
                    if (value == 0)
                    {
                        chk.CheckState = CheckState.Indeterminate;
                        chk.Image = imageListStartStop.Images[4];
                        chk.Text = "OFFLINE";
                    }

                    if (value == 1)
                    {
                        chk.CheckState = CheckState.Checked;
                        chk.Image = imageListStartStop.Images[1];
                        chk.Text = "STOP";
                    }

                    if (value == 2)
                    {
                        chk.CheckState = CheckState.Unchecked;
                        chk.Image = imageListStartStop.Images[0];
                        chk.Text = "START";
                    }
                });
            }
        }

        public void WriteAsyncSystemPauseCheckBox(int value, CheckBox chk)
        {
            if (chk.InvokeRequired)
            {
                chk.Invoke((MethodInvoker)delegate
                {
                    if (value == 0)
                    {
                        chk.CheckState = CheckState.Indeterminate;
                        chk.Image = imageListStartStop.Images[4];
                        chk.Text = "OFFLINE";
                    }

                    if (value == 1)
                    {
                        chk.CheckState = CheckState.Checked;
                        chk.Image = imageListStartStop.Images[3];
                        chk.Text = "IN PAUSE";
                    }

                    if (value == 2)
                    {
                        chk.CheckState = CheckState.Unchecked;
                        chk.Image = imageListStartStop.Images[2];
                        chk.Text = "PAUSE";
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
        public void UpdateOPCUAMHomingDone(ClientResult cr, Button btn, Label lbl)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                btn.BackColor = Color.Black;
                WriteOnLabelAsync(lbl, "offline");
            }
            else
            {
                if (!(bool)cr.Value)
                {
                    btn.BackColor = Color.Red;
                    WriteOnLabelAsync(lbl, "homing not done");
                }

                else
                {
                    btn.BackColor = Color.FromArgb(107, 227, 162);
                    WriteOnLabelAsync(lbl, "homing done");
                }
            }
        }

        #region (* send M1 teaching package *)
        public async void OPCUAM1TeachPckSend(short pointID, float[] pointQuote, short[] pointSpeed, bool[] pointReg)
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

        public async void OPCUAM2TeachPckSend(short pointID, float[] pointQuote, short[] pointSpeed, bool[] pointReg)
        {
            if (ccService != null)
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
                    bool allSent = true;
                    foreach(var result in sendResult)
                    {
                        allSent = (allSent & result.Value.OpcResult);
                    }
                    if (!allSent)
                    {
                        
                    }                    
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                //todo message box
            }
        }

        public async void OPCUAM3TeachPckSend(short pointID, float[] pointQuote, short[] pointSpeed, bool[] pointReg)
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

        public async void OPCUAM2TestPckSend(float[] pointQuote, short[] pointSpeed)
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

        public async void OPCUAM1TestPckSend(float[] pointQuote, short[] pointSpeed)
        {
            List<string> keys = new List<string>();
            keys.Add("pcM1TestSpeed");
            keys.Add("pcM1TestQuote");

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

        public async void OPCUAM3TestPckSend(float[] pointQuote, short[] pointSpeed)
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
