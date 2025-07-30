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

        private string GetShiftString()
        {
            TimeSpan time1 = new TimeSpan(06, 0, 0);
            TimeSpan time2 = new TimeSpan(14, 0, 0);
            TimeSpan time3 = new TimeSpan(22, 0, 0);
            TimeSpan time4 = new TimeSpan(00, 0, 0);

            TimeSpan now = DateTime.Now.TimeOfDay;

            if ((now > time4) && (now < time1))
            {
                return "V";
            }

            if ((now > time3) && (now < time4))
            {
                return "V";
            }

            if ((now > time1) && (now < time2))
            {
                return "M";
            }

            if ((now > time2) && (now < time3))
            {
                return "T";
            }

            return "";
        }

        private bool IsShiftChanged()
        {
            TimeSpan time1 = new TimeSpan(06, 0, 0);
            TimeSpan time2 = new TimeSpan(06, 2, 0);

            TimeSpan time3 = new TimeSpan(22, 0, 0);
            TimeSpan time4 = new TimeSpan(22, 2, 0);

            TimeSpan time5 = new TimeSpan(14, 0, 0);
            TimeSpan time6 = new TimeSpan(14, 2, 0);

            TimeSpan time7 = new TimeSpan(00, 0, 0);
            TimeSpan time8 = new TimeSpan(00, 2, 0);

            TimeSpan now = DateTime.Now.TimeOfDay;

            if ((now > time1) && (now < time2))
            {
                return true;
            }

            if ((now > time3) && (now < time4))
            {
                return true;
            }

            if ((now > time5) && (now < time6))
            {
                return true;
            }

            if ((now > time7) && (now < time8))
            {
                return true;
            }

            return false;
        }

        public async Task UpdateOPCUAStatus()
        {
            if (ccService == null) return;

            try
            {
                if (ccService.ClientIsConnected)
                {
                 //   string shift = GetShiftString();

                    //if (IsShiftChanged())
                    //{
                        
                    //    UpdateRecipeToM4(comboBoxMRecipeName.Text, shift);
                    //}

                    try
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

                        UpdateOPCUAMStatus(readResult["pcM1Status"], M1AutoDictionary);
                        ManageOPCUAMBtnStatus(readResult["pcM1Status"], checkBoxM1Start);

                        UpdateOPCUAMStatus(readResult["pcM2Status"], M2AutoDictionary);
                        ManageOPCUAMBtnStatus(readResult["pcM2Status"], checkBoxM2Start);

                        if (M3InLine == "1")
                        {
                            UpdateOPCUAMStatus(readResult["pcM3Status"], M3AutoDictionary);
                            ManageOPCUAMBtnStatus(readResult["pcM3Status"], checkBoxM3Start);
                        }
                        UpdateOPCUAMStatus(readResult["pcM4Status"], M4AutoDictionary);
                        ManageOPCUAMBtnStatus(readResult["pcM4Status"], checkBoxM4Start);

                        UpdateOPCUAMStatus(readResult["pcM5Status"], M5AutoDictionary);
                        ManageOPCUAMBtnStatus(readResult["pcM5Status"], checkBoxM5Start);

                        UpdateOPCUAMStatus(readResult["pcM6Status"], M6AutoDictionary);
                        ManageOPCUAMBtnStatus(readResult["pcM6Status"], checkBoxM6Start);
                        groupBoxM1.Invalidate();
                        groupBoxM2.Invalidate();
                        groupBoxM3.Invalidate();
                        groupBoxM4.Invalidate();
                        groupBoxM5.Invalidate();
                        groupBoxM6.Invalidate();
                        //tabPageT1_3.Invalidate();
                        //ManageStartStatus(readResult["pcM1Status"], readResult["pcM2Status"], readResult["pcM3Status"],
                        //readResult["pcM4Status"], readResult["pcM5Status"], readResult["pcM6Status"]);
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
                        if (M3InLine == "1") ManageOPCUAMBtnPause(readResult["pcM3Pause"], checkBoxM3Pause);
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
                        if (M3InLine == "1") UpdateOPCUAMKeepAlive(readResult["pcM3KeepAliveW"], lbLedM3PCKeepAlive);
                        else UpdateOPCUAMKeepAlive(null, lbLedM3PCKeepAlive);
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
                        if (M3InLine == "1") UpdateOPCUAMNodeConnection(readResultPLC["pcM3KeepAliveR"], lastLifeBit[2], ref LifeBitTimeout[2], ref LifeBitCounter[2], pictureBoxM3PLCNode, labelM3Node);

                        UpdateOPCUAMNodeConnection(readResultPLC["pcM4KeepAliveR"], lastLifeBit[3], ref LifeBitTimeout[3], ref LifeBitCounter[3], pictureBoxM4PLCNode, labelM4Node);
                        UpdateOPCUAMNodeConnection(readResultPLC["pcM5KeepAliveR"], lastLifeBit[4], ref LifeBitTimeout[4], ref LifeBitCounter[4], pictureBoxM5PLCNode, labelM5Node);
                        ManageLastLifeBit(readResultPLC["pcM1KeepAliveR"], ref lastLifeBit[0]);
                        ManageLastLifeBit(readResultPLC["pcM2KeepAliveR"], ref lastLifeBit[1]);
                        if (M3InLine == "1") ManageLastLifeBit(readResultPLC["pcM3KeepAliveR"], ref lastLifeBit[2]);
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
                        if (M3InLine == "1") UpdateOPCUAMHomingDone(readResult["pcM3HomingDone"], buttonM3HomingDone, labelM3HomingDone);
                        //UpdateOPCUAMHomingDone(readResult["pcM4HomingDone"], buttonM4HomingDone, labelM4HomingDone);
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
                        WriteOnFloatLabelAsync(readResult["pcM1CurrentAxisQuote"], labelM1TeachAxisQuoteValue);
                        WriteOnFloatLabelAsync(readResult["pcM2CurrentAxisQuote"], labelM2TeachAxisQuoteValue);
                        if (M3InLine == "1") WriteOnFloatLabelAsync(readResult["pcM3CurrentAxisQuote"], labelM3TeachAxisQuoteValue);
                        //WriteOnFloatLabelAsync(readResult["pcM4CurrentAxisQuote"], labelM4TeachAxisQuoteValue);
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
                        WriteAsyncDataGridViewPointReached(readResult["pcM1PointReached"], dataGridViewM1TeachPoints, 5);
                        //force repaint
                        dataGridViewM1TeachPoints.Invalidate(false);

                        WriteAsyncDataGridViewPointReached(readResult["pcM2PointReached"], dataGridViewM2TeachPoints, 5);
                        //force repaint
                        dataGridViewM2TeachPoints.Invalidate(false);

                        if (M3InLine == "1") WriteAsyncDataGridViewPointReached(readResult["pcM3PointReached"], dataGridViewM3TeachPoints, 5);
                        //force repaint
                        if (M3InLine == "1") dataGridViewM3TeachPoints.Invalidate(false);

                        WriteAsyncDataGridViewPointReached(readResult["pcM1PointReached"], dataGridViewM1TestPoints, 3);
                        //force repaint
                        dataGridViewM1TestPoints.Invalidate(false);

                        WriteAsyncDataGridViewPointReached(readResult["pcM2PointReached"], dataGridViewM2TestPoints, 3);
                        //force repaint
                        dataGridViewM2TestPoints.Invalidate(false);

                        if (M3InLine == "1") WriteAsyncDataGridViewPointReached(readResult["pcM3PointReached"], dataGridViewM3TestPoints, 3);
                        //force repaint
                        if (M3InLine == "1") dataGridViewM3TestPoints.Invalidate(false);
                        #endregion

                        #region(+ I/O *)
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
                        UpdateOPCUAM2DO(readResult["pcM2DO"]);

                        if (M3InLine == "1")
                        {
                            keys = new List<string>()
                    {
                        "pcM3DI",
                        "pcM3DO"
                    };

                            readResult = await ccService.Read(keys);
                            UpdateOPCUAM3DI(readResult["pcM3DI"]);
                            UpdateOPCUAM3DO(readResult["pcM3DO"]);
                        }

                        keys = new List<string>()
                    {
                        "pcM4DI",
                        "pcM4DO"
                    };

                        readResult = await ccService.Read(keys);
                        UpdateOPCUAM4DI(readResult["pcM4DI"]);
                        UpdateOPCUAM4DO(readResult["pcM4DO"]);

                        keys = new List<string>()
                    {
                        "pcM5DI",
                        "pcM5DO"
                    };

                        readResult = await ccService.Read(keys);
                        UpdateOPCUAM5DI(readResult["pcM5DI"]);
                        UpdateOPCUAM5DO(readResult["pcM5DO"]);

                        #endregion

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

                        if (M3InLine == "1")
                        {
                            keys = new List<string>()
                    {
                        "pcM3TimeoutAlarms",
                        "pcM3GeneralAlarms"
                    };

                            readResult = await ccService.Read(keys);
                            UpdateOPCUAM3TimeoutAlarms(readResult["pcM3TimeoutAlarms"]);
                            UpdateOPCUAM3GeneralAlarms(readResult["pcM3GeneralAlarms"]);
                        }
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

                        try
                        {
                            readResultPLC = await ccService.Read(keys);
                            if (readResultPLC["pcM1CycleCounter"].Value != null)
                            M1AutoDictionary[2] = short.Parse(readResultPLC["pcM1CycleCounter"].Value.ToString());
                            //WriteOnLabelAsync(readResultPLC["pcM1CycleCounter"], labelM1CycleId);
                            //WriteOnLabelAsync(readResultPLC["pcM2CycleCounter"], labelM2CycleId);
                            if (readResultPLC["pcM2CycleCounter"].Value != null)
                                M2AutoDictionary[2] = short.Parse(readResultPLC["pcM2CycleCounter"].Value.ToString());
                            //WriteOnLabelAsync(readResultPLC["pcM3CycleCounter"], labelM3CycleId);
                            if (readResultPLC["pcM3CycleCounter"].Value != null)
                                if (M3InLine == "1") M3AutoDictionary[2] = short.Parse(readResultPLC["pcM3CycleCounter"].Value.ToString());
                            //WriteOnLabelAsync(readResultPLC["pcM4CycleCounter"], labelM4CycleId);
                            if (readResultPLC["pcM4CycleCounter"].Value != null)
                                M4AutoDictionary[2] = short.Parse(readResultPLC["pcM4CycleCounter"].Value.ToString());
                            //WriteOnLabelAsync(readResultPLC["pcM5CycleCounter"], labelM5CycleId);
                            if (readResultPLC["pcM5CycleCounter"].Value != null)
                                M5AutoDictionary[2] = short.Parse(readResultPLC["pcM5CycleCounter"].Value.ToString());
                        }
                        catch(Exception Ex)
                        {

                        }

                        #endregion

                        #region(* cycle time *)
                        keys = new List<string>()
                    {
                        "pcM1CycleTime",
                        "pcM2CycleTime",
                        "pcM4CycleTime",
                        "pcM5CycleTime1",
                        "pcM5CycleTime2",
                    };

                        readResultPLC = await ccService.Read(keys);
                        if(readResultPLC["pcM1CycleTime"].Value != null) M1AutoDictionary[3] = short.Parse(readResultPLC["pcM1CycleTime"].Value.ToString());
                        if (readResultPLC["pcM2CycleTime"].Value != null) M2AutoDictionary[3] = short.Parse(readResultPLC["pcM2CycleTime"].Value.ToString());
                        if (readResultPLC["pcM4CycleTime"].Value != null) M4AutoDictionary[3] = short.Parse(readResultPLC["pcM4CycleTime"].Value.ToString());
                        if (readResultPLC["pcM5CycleTime1"].Value != null) M5AutoDictionary[3] = short.Parse(readResultPLC["pcM5CycleTime1"].Value.ToString());
                        if (readResultPLC["pcM5CycleTime2"].Value != null) M5AutoDictionary[5] = short.Parse(readResultPLC["pcM5CycleTime2"].Value.ToString());
                        if (M3InLine == "1")
                        {
                            keys = new List<string>()
                            {
                                "pcM3CycleTimeLF",
                                "pcM3CycleTimeRG",
                            };
                            readResultPLC = await ccService.Read(keys);
                            M3AutoDictionary[3] = short.Parse(readResultPLC["pcM3CycleTimeLF"].Value.ToString());
                            M3AutoDictionary[5] = short.Parse(readResultPLC["pcM3CycleTimeRG"].Value.ToString());
                        }

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
                        if(readResultPLC["pcM1TrimmingState"].Value != null)M1StateDictionary[1] = short.Parse(readResultPLC["pcM1TrimmingState"].Value.ToString());
                        if (readResultPLC["pcM1LoadingBeltState"].Value != null) M1StateDictionary[2] = short.Parse(readResultPLC["pcM1LoadingBeltState"].Value.ToString());
                        if (readResultPLC["pcM1WorkingBeltState"].Value != null) M1StateDictionary[3] = short.Parse(readResultPLC["pcM1WorkingBeltState"].Value.ToString());
                        if (readResultPLC["pcM1ExitBeltState"].Value != null) M1StateDictionary[4] = short.Parse(readResultPLC["pcM1ExitBeltState"].Value.ToString());

                        keys = new List<string>()
                    {
                        "pcM4State",
                        "pcM4WorkingBeltState",
                        "pcM4ExitBeltState"
                    };

                        readResultPLC = await ccService.Read(keys);
                        if (readResultPLC["pcM4State"].Value !=null) M4StateDictionary[1] = short.Parse(readResultPLC["pcM4State"].Value.ToString());
                        if (readResultPLC["pcM4WorkingBeltState"].Value != null) M4StateDictionary[2] = short.Parse(readResultPLC["pcM4WorkingBeltState"].Value.ToString());
                        if (readResultPLC["pcM4ExitBeltState"].Value != null) M4StateDictionary[3] = short.Parse(readResultPLC["pcM4ExitBeltState"].Value.ToString());

                        if (M3InLine == "1")
                        {
                            keys = new List<string>()
                    {
                        "pcM3PadPrintExtState",
                        "pcM3WorkingBeltState",
                        "pcM3ExitBeltState"
                    };

                            readResultPLC = await ccService.Read(keys);
                            M3StateDictionary[1] = short.Parse(readResultPLC["pcM3PadPrintExtState"].Value.ToString());
                            M3StateDictionary[2] = short.Parse(readResultPLC["pcM3WorkingBeltState"].Value.ToString());
                            M3StateDictionary[3] = short.Parse(readResultPLC["pcM3ExitBeltState"].Value.ToString());
                        }

                        keys = new List<string>()
                    {
                        "pcM2PadPrintIntState",
                        "pcM2WorkingBeltState",
                        "pcM2ExitBeltState"
                    };

                        readResultPLC = await ccService.Read(keys);
                        if (readResultPLC["pcM2PadPrintIntState"].Value !=null) M2StateDictionary[1] = short.Parse(readResultPLC["pcM2PadPrintIntState"].Value.ToString());
                        if (readResultPLC["pcM2WorkingBeltState"].Value != null) M2StateDictionary[2] = short.Parse(readResultPLC["pcM2WorkingBeltState"].Value.ToString());
                        if (readResultPLC["pcM2ExitBeltState"].Value != null) M2StateDictionary[3] = short.Parse(readResultPLC["pcM2ExitBeltState"].Value.ToString());

                        keys = new List<string>()
                    {
                        "pcM5State",
                        "pcM5TranslationBeltState",
                        "pcM5ExitBelt1State",
                        "pcM5ExitBelt2State",
                        "pcM5ExitBelt3State"
                    };

                        readResultPLC = await ccService.Read(keys);
                        if (readResultPLC["pcM5State"].Value != null) M5StateDictionary[1] = short.Parse(readResultPLC["pcM5State"].Value.ToString());
                        if (readResultPLC["pcM5TranslationBeltState"].Value != null) M5StateDictionary[2] = short.Parse(readResultPLC["pcM5TranslationBeltState"].Value.ToString());
                        if (readResultPLC["pcM5ExitBelt1State"].Value != null) M5StateDictionary[3] = short.Parse(readResultPLC["pcM5ExitBelt1State"].Value.ToString());
                        if (readResultPLC["pcM5ExitBelt2State"].Value != null) M5StateDictionary[4] = short.Parse(readResultPLC["pcM5ExitBelt2State"].Value.ToString());
                        if (readResultPLC["pcM5ExitBelt3State"].Value != null) M5StateDictionary[5] = short.Parse(readResultPLC["pcM5ExitBelt3State"].Value.ToString());
                        #endregion

                        #region (* GUI *)
                        GUIWithOPCUAClientConnected();
                        #endregion

                        #region (* restart from plc *)
                        keys = new List<string>()
                    {
                        "pcM1RestartPlc",
                        "pcM2RestartPlc",
                        "pcM3RestartPlc",
                        "pcM4RestartPlc",
                        "pcM5RestartPlc",
                        "pcM6RestartPlc"
                    };

                        try
                        {
                            var readRes = await ccService.Read(keys);

                            if (readRes != null)
                            {

                            }
                            else
                            {
                                if (bool.Parse(readRes["pcM1RestartPlc"].Value.ToString()))
                                {
                                    var readRes2 = await ccService.Send("pcM1RestartPlc", false);

                                    RestartRequestFromM1();
                                }
                                else
                                {

                                }
                            }

                            if (readRes["pcM2RestartPlc"].Value != null)
                            {
                                if (bool.Parse(readRes["pcM2RestartPlc"].Value.ToString()))
                                {
                                    var readRes2 = await ccService.Send("pcM2RestartPlc", false);

                                    RestartRequestFromM2();
                                }
                                else
                                {

                                }
                            }

                            if (readRes["pcM3RestartPlc"].Value != null)
                            {
                                if (bool.Parse(readRes["pcM3RestartPlc"].Value.ToString()))
                                {
                                    var readRes2 = await ccService.Send("pcM3RestartPlc", false);

                                    RestartRequestFromM3();
                                }
                            }

                            if (readRes["pcM4RestartPlc"].Value != null)
                            {
                                if (bool.Parse(readRes["pcM4RestartPlc"].Value.ToString()))
                                {
                                    //var readRes2 = await ccService.Send("pcM4RestartPlc", false);

                                    RestartRequestFromM4();
                                }
                                else
                                {

                                }
                            }

                            if (readRes["pcM5RestartPlc"].Value != null)
                            {
                                if (bool.Parse(readRes["pcM5RestartPlc"].Value.ToString()))
                                {
                                    var readRes2 = await ccService.Send("pcM5RestartPlc", false);

                                    RestartRequestFromM5();
                                }
                                else
                                {

                                }
                            }

                        }
                        catch (Exception ex)
                        {

                        }
                        #endregion

                        #region(* send data to custom variables *)
                       // SendDataToCustom();
                        #endregion
                    }
                    catch (Exception ex)
                    {

                    }
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
    
        public async void SendDataToCustom()
        {
            List<string> keys = new List<string>()
                    {
                        "pcCustomData1",
                        //"pcCustomData2",
                        //"pcCustomData3",
                        //"pcCustomData4",
                        //"pcCustomData5",
                    };

            List<object> values = new List<object>()
                    {
                        new int[] {M1AutoDictionary[2], M2AutoDictionary[2], M3AutoDictionary[2], M4AutoDictionary[3], M5AutoDictionary[2],
                        M1AutoDictionary[3], M2AutoDictionary[3], M3AutoDictionary[3], M4AutoDictionary[3], M5AutoDictionary[3],
                        M1AutoDictionary[4], M2AutoDictionary[4], M3AutoDictionary[4], M4AutoDictionary[4], M5AutoDictionary[4]}
                    };

            var sendResults = await ccService.Send(keys, values);
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
                        //chk.CheckState = CheckState.Unchecked;
                        //chk.Image = imageListStart.Images[0];
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
                        //chk.Image = imageListStart.Images[2];
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

            if (cr == null)
            {

            }
            else
            {
                if (cr.OpcResult == false)
                {
                    
                }
                else value = (short)cr.Value;
            }

            if (lbl.InvokeRequired)
            {
                lbl.Invoke((MethodInvoker)delegate
                {
                    lbl.Text = "";
                    if (value == -1)
                    {
                        lbl.Text = "sin conexión";
                    }

                    if (value == 0)
                    {
                        lbl.Text = "emergencia";
                    }

                    if (value == 1)
                    {
                        lbl.Text = "automatico";
                    }

                    if (value == 2)
                    {
                        lbl.Text = "manual";
                    }

                    if (value == 3)
                    {
                        lbl.Text = "en ciclo";
                    }

                    if (value == 4)
                    {
                        lbl.Text = "en alarma";
                    }
                });
            }
        }

        public void WriteOnLabelAsync(ClientResult cr, Label lbl)
        {
            string textToAppend = "";

            if ((cr == null) || (cr.OpcResult == false))
            {
                textToAppend = "sin conexión";
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

        public void WriteOnLabelAsync(ClientResult cr, Label lbl, float divisor)
        {
            string textToAppend = "";

            if ((cr == null) || (cr.OpcResult == false))
            {
                textToAppend = "sin conexión";
            }
            else
            {
                textToAppend = (float.Parse(cr.Value.ToString()) / divisor).ToString();
            }
            if (lbl.InvokeRequired)
            {
                lbl.Invoke((MethodInvoker)delegate
                {
                    lbl.Text = textToAppend;
                });
            }
        }

        public void WriteOnFloatLabelAsync(ClientResult cr, Label lbl)
        {
            string textToAppend = "";

            if ((cr == null) || (cr.OpcResult == false))
            {
                textToAppend = "sin conexión";
            }
            else
            {                
                textToAppend = Math.Round((float)(cr.Value), 1).ToString();
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
                    if (!value)
                    {
                        lbl.BackColor = Color.FromArgb(107, 227, 162);
                        lbl.ForeColor = Color.FromArgb(51, 51, 51);
                        lbl.Text = "sistema conectado";
                    }
                    else
                    {
                        lbl.BackColor = Color.Red;
                        lbl.ForeColor = Color.FromArgb(241, 241, 241);
                        lbl.Text = "sistema no conectado";
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
            UpdateOPCUAMStatus(null, M1AutoDictionary);
            WriteOnCheckBoxStartAsync(null, checkBoxM1Start);
            WriteAsyncOnCheckBoxPause(null, checkBoxM1Pause);

            UpdateOPCUAMStatus(null, M2AutoDictionary);
            WriteOnCheckBoxStartAsync(null, checkBoxM2Start);
            WriteAsyncOnCheckBoxPause(null, checkBoxM2Pause);

            UpdateOPCUAMStatus(null, M3AutoDictionary);
            WriteOnCheckBoxStartAsync(null, checkBoxM3Start);
            WriteAsyncOnCheckBoxPause(null, checkBoxM3Pause);

            UpdateOPCUAMStatus(null, M4AutoDictionary);
            WriteOnCheckBoxStartAsync(null, checkBoxM4Start);
            WriteAsyncOnCheckBoxPause(null, checkBoxM4Pause);

            UpdateOPCUAMStatus(null, M5AutoDictionary);
            WriteOnCheckBoxStartAsync(null, checkBoxM5Start);
            WriteAsyncOnCheckBoxPause(null, checkBoxM5Pause);

            //UpdateOPCUAMStatus(null, buttonM6Status, labelM6Status);
            WriteOnCheckBoxStartAsync(null, checkBoxM6Start);
            WriteAsyncOnCheckBoxPause(null, checkBoxM6Pause);

            //line status
            WriteOnToolStripAsync(false, toolStripStatusLabelSystem);

            WriteAsyncSystemStartStopCheckBox(0, checkBoxStartStop);
            WriteAsyncSystemPauseCheckBox(0, checkBoxPause);

            pictureBoxM1PLCNode.Image = imageListNodes.Images[1];
            WriteOnLabelAsync(labelM1Node, "node sin conexión");
            pictureBoxM2PLCNode.Image = imageListNodes.Images[1];
            WriteOnLabelAsync(labelM2Node, "node sin conexión");
            pictureBoxM3PLCNode.Image = imageListNodes.Images[1];
            WriteOnLabelAsync(labelM3Node, "node sin conexión");
            pictureBoxM4PLCNode.Image = imageListNodes.Images[1];
            WriteOnLabelAsync(labelM4Node, "node sin conexión");
            pictureBoxM5PLCNode.Image = imageListNodes.Images[1];
            WriteOnLabelAsync(labelM5Node, "node sin conexión");

            pictureBoxIOTNode.Image = imageListNodes.Images[3];

            lbLedM1PCKeepAlive.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLedM2PCKeepAlive.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLedM3PCKeepAlive.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLedM4PCKeepAlive.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
            lbLedM5PCKeepAlive.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;

            UpdateOPCUAMHomingDone(null, buttonM1HomingDone, labelM1HomingDone);
            UpdateOPCUAMHomingDone(null, buttonM2HomingDone, labelM2HomingDone);
            UpdateOPCUAMHomingDone(null, buttonM3HomingDone, labelM3HomingDone);
            //UpdateOPCUAMHomingDone(null, buttonM4HomingDone, labelM4HomingDone);
            //UpdateOPCUAMHomingDone(readResult["pcM5HomingDone"], buttonM5HomingDone, labelM5HomingDone);
            //UpdateOPCUAMHomingDone(readResult["pcM6HomingDone"], buttonM6HomingDone, labelM6HomingDone);

            WriteOnLabelAsync(null, labelM1TeachAxisQuoteValue);
            WriteOnLabelAsync(null, labelM2TeachAxisQuoteValue);
            WriteOnLabelAsync(null, labelM3TeachAxisQuoteValue);
            //WriteOnLabelAsync(null, labelM4TeachAxisQuoteValue);
            //WriteOnLabelAsync(null, labelM3TeachAxisQuoteValue);
            //WriteOnToolStripAsync(true, toolStripStatusLabelSystem);
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
                        WriteOnLabelAsync(lbl, "node conectado");
                    }
                }
                else
                {
                    if ((bool)cr.Value != oldValue)
                    {
                        lBitCounter = 0;
                        lBitTimeout = false;
                        pict.Image = imageListNodes.Images[0];
                        WriteOnLabelAsync(lbl, "node conectado");
                    }
                    else
                    {
                        pict.Image = imageListNodes.Images[1];
                        WriteOnLabelAsync(lbl, "node sin conexión");
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

        public void UpdateOPCUAM5DI(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                int i = 0;
                bool[] arrayBool = (bool[])cr.Value;
                foreach (bool item in arrayBool)
                {
                    if (i != 0)
                    {
                        M5InputDictionary[i] = item;
                    }
                    i = i + 1;
                }
            }
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
                foreach (bool item in arrayBool)
                {
                    if (i != 0)
                    {
                        M1InputDictionary[i] = item;
                    }
                    i = i + 1;
                }
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
                foreach (bool item in arrayBool)
                {
                    if (i != 0)
                    {
                        M1OutputDictionary[i] = item;
                    }
                    i = i + 1;
                }
            }
        }

        public void UpdateOPCUAM4DI(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                int i = 0;
                bool[] arrayBool = (bool[])cr.Value;
                foreach (bool item in arrayBool)
                {
                    if (i != 0) M4InputDictionary[i] = item;
                    i = i + 1;
                }
            }
        }

        public void UpdateOPCUAM4DO(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                int i = 0;
                bool[] arrayBool = (bool[])cr.Value;
                foreach (bool item in arrayBool)
                {
                    if (i != 0) M4OutputDictionary[i] = item;
                    i = i + 1;
                }
            }
        }

        public void UpdateOPCUAM2DO(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                int i = 0;
                bool[] arrayBool = (bool[])cr.Value;
                foreach (bool item in arrayBool)
                {
                    if (i != 0) M2OutputDictionary[i] = item;
                    i = i + 1;
                }
            }
        }

        public void UpdateOPCUAM3DO(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                int i = 0;
                bool[] arrayBool = (bool[])cr.Value;
                foreach (bool item in arrayBool)
                {
                    if (i != 0) M3OutputDictionary[i] = item;
                    i = i + 1;
                }
            }
        }

        public void UpdateOPCUAM5DO(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                int i = 0;
                bool[] arrayBool = (bool[])cr.Value;
                foreach (bool item in arrayBool)
                {
                    if (i != 0) M5OutputDictionary[i] = item;
                    i = i + 1;
                }
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
                int k = 0;
                for (k = 0; k <= 32; k++)
                {
                    M1AlarmsDictionary[k] = 0;

                }
                short[] arrayShort = (short[])cr.Value;

                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;
                int j = 1;
                
                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        M1AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        Console.WriteLine("timeout homing motore");
                        M1AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        Console.WriteLine("timeout start posizionamento motore");
                        M1AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        Console.WriteLine("timeout posizionamento motore");
                        M1AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        Console.WriteLine("timeout pinza bordo stivale");
                        M1AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        Console.WriteLine("timeout pinza grande stivale");
                        M1AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        Console.WriteLine("timeout pinza centraggio 1");
                        M1AlarmsDictionary[j] = 1;
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        Console.WriteLine("timeout pinza centraggio 2");
                        M1AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
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
                int k = 0;
                for (k = 0; k <= 32; k++)
                {
                    M2AlarmsDictionary[k] = 0;

                }
                short[] arrayShort = (short[])cr.Value;

                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;
                int j = 1;

                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        M2AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        Console.WriteLine("timeout homing motore");
                        M2AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        Console.WriteLine("timeout start posizionamento motore");
                        M2AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        Console.WriteLine("timeout posizionamento motore");
                        M2AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        Console.WriteLine("timeout pinza bordo stivale");
                        M2AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        Console.WriteLine("timeout pinza grande stivale");
                        M2AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        Console.WriteLine("timeout pinza centraggio 1");
                        M2AlarmsDictionary[j] = 1;
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        M2AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
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
                int k = 0;
                for (k = 0; k <= 32; k++)
                {
                    M4AlarmsDictionary[k] = 0;

                }
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;
                int j = 1; 
                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        M4AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        M4AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        M4AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        M4AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        M4AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        M4AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        M4AlarmsDictionary[j] = 1;
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        M4AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
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
                int k = 0;
                for (k = 0; k <= 32; k++)
                {
                    M3AlarmsDictionary[k] = 0;

                }
                short[] arrayShort = (short[])cr.Value;

                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;
                int j = 1;

                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        M3AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        M3AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        M3AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        M3AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        M3AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        M3AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        M3AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        M3AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
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
                int k = 0;
                for (k = 0; k <= 32; k++)
                {
                    M5AlarmsDictionary[k] = 0;

                }
                short[] arrayShort = (short[])cr.Value;

                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;
                int j = 1;

                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        M5AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        M5AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        M5AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        M5AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        M5AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        M5AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        M5AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        M5AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
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
                int k = 0;
                for (k = 33; k <= 64; k++)
                {
                    M1AlarmsDictionary[k] = 0;

                }
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;
                int j = 33;
                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        M1AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        M1AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        M1AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        M1AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        M1AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        M1AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        M1AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {                        
                        M1AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
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
                int k = 0;
                for (k = 33; k <= 64; k++)
                {
                    M4AlarmsDictionary[k] = 0;

                }
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;
                int j = 33;
                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {                        
                        M4AlarmsDictionary[j] = 1;
                    } 

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {                        
                        M4AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        M4AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        M4AlarmsDictionary[j] = 1;
                    }
                   
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        M4AlarmsDictionary[j] = 1;
                    }
                    
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {                        
                        M4AlarmsDictionary[j] = 1;
                    }
                    
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {                    
                        M4AlarmsDictionary[j] = 1;
                    }
                    
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        M4AlarmsDictionary[j] = 1;
                    }
                    
                    j = j + 1;
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
                int k = 0;
                for (k = 33; k <= 64; k++)
                {
                    M3AlarmsDictionary[k] = 0;

                }
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;
                int j = 33;
                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {                        
                        M3AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {                        
                        M3AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {                        
                        M3AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {                        
                        M3AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {                        
                        M3AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {                        
                        M3AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {                        
                        M3AlarmsDictionary[j] = 1;
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {                        
                        M3AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
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
                int k = 0;
                for (k = 33; k <= 64; k++)
                {
                    M2AlarmsDictionary[k] = 0;

                }
                M2AlarmsDictionary[k] = 0;
                //}
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;
                int j = 33;
                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {                        
                        M2AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {                        
                        M2AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {                        
                        M2AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {                        
                        M2AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {                        
                        M2AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {                        
                        M2AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {                        
                        M2AlarmsDictionary[j] = 1;
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {                        
                        M2AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
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
                int k = 0;
                for (k = 33; k <= 64; k++)
                {
                    M5AlarmsDictionary[k] = 0;

                }
                short[] arrayShort = (short[])cr.Value;
                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;
                int j = 33;
                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {                        
                        M5AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {                     
                        M5AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {                     
                        M5AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {                        
                        M5AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {                     
                        M5AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {                       
                        M5AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {                        
                        M5AlarmsDictionary[j] = 1;
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {                        
                        M5AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
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
                foreach (bool item in arrayBool)
                {
                    if (i != 0)
                    {
                        M2InputDictionary[i] = item;
                    }
                    i = i + 1;
                }
            }
        }

        public void UpdateOPCUAM3DI(ClientResult cr)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                int i = 0;
                bool[] arrayBool = (bool[])cr.Value;
                foreach (bool item in arrayBool)
                {
                    if (i != 0)
                    {
                        M3InputDictionary[i] = item;
                    }
                    i = i + 1;
                }
            }
        }
        public void WriteAsyncDataGridViewPointReached(ClientResult cr, DataGridView dt, int dtIndex)
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
                                dt.Rows[i - 1].Cells[dtIndex].ToolTipText = "reached";
                            }
                            else
                            {
                                dt.Rows[i - 1].Cells[dtIndex].ToolTipText = "notreached";
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

        public void UpdateOPCUAMStatus(ClientResult cr, Dictionary<int, short> myDict)
        {
            short value = -1;

            if (cr == null)
            {
                myDict[4] = -1;
                //btn.BackColor = Color.Black;               
            }
            else
            {
                if (cr.OpcResult == false)
                {
                    myDict[4] = -1;
                    //btn.BackColor = Color.Black;
                }
                else
                {
                    myDict[4] = (short)cr.Value;
                    //value = (short)cr.Value;

                    //if (value == 0)
                    //{
                    //    btn.BackColor = Color.Red;
                    //}

                    //if (value == 1)
                    //{
                    //    btn.BackColor = Color.FromArgb(59, 130, 246);
                    //}

                    //if (value == 2)
                    //{
                    //    btn.BackColor = Color.Orange;
                    //}

                    //if (value == 3)
                    //{
                    //    btn.BackColor = Color.FromArgb(107, 227, 162);
                    //}

                    //if (value == 4)
                    //{
                    //    btn.BackColor = Color.DarkOrange;
                    //}
                }
            }
          //  WriteOnLabelMStatusAsync(cr, lbl);
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
                        //chk.Image = imageListStartStop.Images[4];
                        //chk.Text = "NO CONECTADO";
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
                        //chk.Image = imageListStartStop.Images[4];
                        chk.Text = "NO CONECTADO";
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
                WriteOnLabelAsync(lbl, "no conectado");
            }
            else
            {
                if (!(bool)cr.Value)
                {
                    btn.BackColor = Color.Red;
                    WriteOnLabelAsync(lbl, "home no listo");
                }

                else
                {
                    btn.BackColor = Color.FromArgb(107, 227, 162);
                    WriteOnLabelAsync(lbl, "home listo");
                }
            }
        }

        #region (* send M1 teaching package *)
        public async void OPCUAM1TeachPckSend(short pointID, float[] pointQuote, short[] pointSpeed, bool[] pointReg)
        {
            if (ccService.ClientIsConnected)
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
                catch (Exception ex)
                {

                }
            }
        }

        public async void OPCUAM2TeachPckSend(short pointID, float[] pointQuote, short[] pointSpeed, bool[] pointReg)
        {
            if (ccService.ClientIsConnected)
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
            if (ccService.ClientIsConnected)
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
        }

        public async void OPCUAM2TestPckSend(float[] pointQuote, short[] pointSpeed)
        {
            if (ccService.ClientIsConnected)
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
        }

        public async void OPCUAM1TestPckSend(float[] pointQuote, short[] pointSpeed)
        {
            if (ccService.ClientIsConnected)
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
        }

        public async void OPCUAM3TestPckSend(float[] pointQuote, short[] pointSpeed)
        {
            if (ccService.ClientIsConnected)
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
        }
        #endregion

        #endregion


    }
}
