using RSACommon.Configuration;
using RSACommon.Points;
using RSACommon.ProgramParser;
using RSACommon.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xDialog;
using RSAPoints.ConcretePoints;

namespace GUI
{
    public partial class FormApp : Form
    {
        private async void buttonM1TeachLoadProgram_Click(object sender, EventArgs e)
        {
            if (comboBoxM1TeachProgramList.Text == "") return;

            try
            {
                var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

                if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                {
                    ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                    ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                    objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[0] + "\\" + comboBoxM1TeachProgramList.Text + config.Extensions[0]);
                    if (objPoints != null)
                    {
                        dataGridViewM1TeachPoints[1, 0].Value = objPoints.Points[0].Q1;
                        dataGridViewM1TeachPoints[1, 1].Value = objPoints.Points[0].Q2;
                        dataGridViewM1TeachPoints[1, 2].Value = objPoints.Points[0].Q3;
                        dataGridViewM1TeachPoints[1, 3].Value = objPoints.Points[0].Q4;
                        dataGridViewM1TeachPoints[2, 0].Value = objPoints.Points[0].V1;
                        dataGridViewM1TeachPoints[2, 1].Value = objPoints.Points[0].V2;
                        dataGridViewM1TeachPoints[2, 2].Value = objPoints.Points[0].V3;
                        dataGridViewM1TeachPoints[2, 3].Value = objPoints.Points[0].V4;
                        numericUpDownM1TimerBootTeach.Value = Convert.ToDecimal(objPoints.Points[0].CustomFloatParam.ToString());
                        //program succesfully loaded
                        xDialog.MsgBox.Show("program " + comboBoxM1TeachProgramList.Text + " succesfully loaded", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Application, xDialog.MsgBox.AnimateStyle.FadeIn);
                    }
                    else
                    {
                        //parsing failed
                        xDialog.MsgBox.Show("program " + comboBoxM1TeachProgramList.Text + " not loaded", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Exclamation, xDialog.MsgBox.AnimateStyle.FadeIn);
                    }
                }
                else
                {
                    //parsing failed
                    xDialog.MsgBox.Show("program " + comboBoxM1TeachProgramList.Text + " not loaded", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Exclamation, xDialog.MsgBox.AnimateStyle.FadeIn);
                }
            }
            catch(Exception ex)
            {
                
            }
        }

        private async void buttonM1TeachSaveProgram_Click(object sender, EventArgs e)
        {
            if (comboBoxM1TeachProgramList.Text == "") return;

            try
            {
                var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

                if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                {
                    ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;

                    float p1 = float.Parse(dataGridViewM1TeachPoints[1, 0].Value.ToString());
                    float p2 = float.Parse(dataGridViewM1TeachPoints[1, 1].Value.ToString());
                    float p3 = float.Parse(dataGridViewM1TeachPoints[1, 2].Value.ToString());
                    float p4 = float.Parse(dataGridViewM1TeachPoints[1, 3].Value.ToString());
                    int s1 = Convert.ToInt32(dataGridViewM1TeachPoints[2, 0].Value);
                    int s2 = Convert.ToInt32(dataGridViewM1TeachPoints[2, 1].Value);
                    int s3 = Convert.ToInt32(dataGridViewM1TeachPoints[2, 2].Value);
                    int s4 = Convert.ToInt32(dataGridViewM1TeachPoints[2, 3].Value);
                    ConcretePointsContainer<PointAxis> prgObj = new ConcretePointsContainer<PointAxis>(comboBoxM1TeachProgramList.Text);
                    PointAxis p = new PointAxis(p1, p2, p3, p4, s1, s2, s3, s4);
                    p.CustomFloatParam = (numericUpDownM1TimerBootTeach.Value.ToString());
                    prgObj.AddPoint(p);

                    prgObj.Save(comboBoxM1TeachProgramList.Text + config.Extensions[0], config.ProgramsPath[0], true);
                    //program succesfully saved
                    xDialog.MsgBox.Show("program " + comboBoxM1TeachProgramList.Text + " succesfully saved", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Application, xDialog.MsgBox.AnimateStyle.FadeIn);
                }
                else
                {
                    //todo log
                }
            }
            catch(Exception ex)
            {
                //todo log
            }
        }

        private void buttonM1TeachNewProgram_Click(object sender, EventArgs e)
        {
            ResetM1Datagrid();
        }

        private void buttonM1TeachDeleteProgram_Click(object sender, EventArgs e)
        {
            if (comboBoxM1TeachProgramList.Text == "") return;

            try
            {

            }
            catch (Exception ex)
            {
                //todo log
            }
        }

        private async void lbButtonM1JogUp_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;

                if (e.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
                {
                    //send quote
                    keyToSend = "pcM1JogDown";
                    var readResult1 = await ccService.Send(keyToSend, false);
                    if (readResult1.OpcResult)
                    {
                    }

                    keyToSend = "pcM1JogUp";
                    var readResult2 = await ccService.Send(keyToSend, true);
                    if (readResult2.OpcResult)
                    {
                    }
                }
                else
                {
                    keyToSend = "pcM1JogDown";
                    var readResult1 = await ccService.Send(keyToSend, false);
                    if (readResult1.OpcResult)
                    {
                    }

                    keyToSend = "pcM1JogUp";
                    var readResult2 = await ccService.Send(keyToSend, false);
                    if (readResult2.OpcResult)
                    {
                    }
                }
            }
        }

        private async void lbButtonM1JogDown_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;

                if (e.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
                {
                    keyToSend = "pcM1JogUp";
                    var readResult1 = await ccService.Send(keyToSend, false);
                    if (readResult1.OpcResult)
                    {
                    }

                    keyToSend = "pcM1JogDown";
                    var readResult2 = await ccService.Send(keyToSend, true);
                    if (readResult2.OpcResult)
                    {
                    }
                }
                else
                {
                    keyToSend = "pcM1JogUp";
                    var readResult1 = await ccService.Send(keyToSend, false);
                    if (readResult1.OpcResult)
                    {
                    }

                    keyToSend = "pcM1JogDown";
                    var readResult2 = await ccService.Send(keyToSend, false);
                    if (readResult2.OpcResult)
                    {
                    }
                }
            }
        }

        private async void numericUpDownM1JogSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1JogSpeed";

                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM1JogSpeed.Value.ToString()));

                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void numericUpDownM1ManualQuote_ValueChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1ManualQuote";
                var sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownM1ManualQuote.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
        }

        private async void numericUpDownM1ManualSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1ManualSpeed";

                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM1ManualSpeed.Value.ToString()));

                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonM1StartQuote_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;

                //quote 
                keyToSend = "pcM1ManualQuote";
                var sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownM1ManualQuote.Value.ToString()));
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }

                keyToSend = "pcM1ManualSpeed";
                var sendResult1 = await ccService.Send(keyToSend, short.Parse(numericUpDownM1ManualSpeed.Value.ToString()));

                keyToSend = "pcM1QuoteStart";
                var sendResult2 = await ccService.Send(keyToSend, true);
                //todo: chi lo mette a false
            }
        }

        private async void buttonM1ResetServo_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1ResetServoAlarm";

                var sendResult = await ccService.Send(keyToSend, true);

                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonM1Home_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1Homing";

                var readResult = await ccService.Send(keyToSend, true);

                if (readResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonM1ResetHome_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1ResetHoming";

                var readResult = await ccService.Send(keyToSend, true);

                if (readResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonM1SpringsOpening_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1SpringsOpening";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
            else
            {

            }
        }

        private async void buttonM1SpringsClosing_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1SpringsClosing";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
            else
            {

            }
        }

        private async void buttonM1CutSlideForward_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1CutSlideForward";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
            else
            {

            }
        }

        private async void buttonM1CutSlideBackward_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1CutSlideBackward";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
            else
            {

            }
        }

        private async void buttonM1BlockOpening_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1BlockOpening";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
            else
            {

            }
        }

        private async void buttonM1BlockClosing_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1BlockClosing";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
            else
            {

            }
        }

        private async void buttonM1CutOpening_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1CutOpening";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
            else
            {

            }
        }

        private async void buttonM1CutClosing_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1CutClosing";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
            else
            {

            }
        }

        private async void buttonM1PosV1Up_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1PosV1Up";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
            else
            {

            }
        }

        private async void buttonM1PosV1Down_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1PosV1Down";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
            else
            {

            }
        }

        private async void buttonM1PosV2Up_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1PosV2Up";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
            else
            {

            }
        }

        private async void buttonM1PosV2Down_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1PosV2Down";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
            else
            {

            }
        }

        private async void checkBoxM1CuttingMotor_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM1StartStopCuttingMotor";
                chkValue = (checkBoxM1CuttingMotor.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM1CuttingMotor.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM1CuttingMotor.ImageIndex = 0;
                }
            }
            else
            {
            }
        }

        private async void checkBoxM1CuttingDrainBlow_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM1StartStopCuttingDrainBlow";
                chkValue = (checkBoxM1CuttingDrainBlow.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM1CuttingDrainBlow.ImageIndex = (chkValue) ? 2 : 3;
                }
                else
                {
                    checkBoxM1CuttingDrainBlow.ImageIndex = 2;
                }
            }
            else
            {
            }
        }

        private async void buttonM1Sharpening_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM1Sharpening";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
            else
            {

            }
        }

        private async void checkBoxM1CuttingSuction_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM1StartStopCuttingSuction";
                chkValue = (checkBoxM1CuttingSuction.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM1CuttingSuction.ImageIndex = (chkValue) ? 2 : 3;
                }
                else
                {
                    checkBoxM1CuttingSuction.ImageIndex = 2;
                }
            }
            else
            {
            }
        }

        private async void checkBoxM1ExitBelt_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;
            chkValue = (checkBoxM1ExitBelt.CheckState == CheckState.Checked) ? true : false;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                keyToSend = "pcM1StartStopExitBelt";

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM1ExitBelt.ImageIndex = (chkValue) ? 2 : 3;
                }
                else
                {
                    checkBoxM1ExitBelt.ImageIndex = 2;
                }
            }
            else
            {
                if (Properties.Settings.Default.OpcSimulation)
                {
                    checkBoxM1ExitBelt.ImageIndex = (chkValue) ? 2 : 3;
                }
            }
        }

        private async void checkBoxM1WorkingBelt_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;
            chkValue = (checkBoxM1WorkingBelt.CheckState == CheckState.Checked) ? true : false;

            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                keyToSend = "pcM1StartStopWorkingBelt";

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM1WorkingBelt.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM1WorkingBelt.ImageIndex = 2;
                }
            }
            else
            {
                if (Properties.Settings.Default.OpcSimulation)
                {
                    checkBoxM1WorkingBelt.ImageIndex = (chkValue) ? 0 : 1;
                }
            }
        }

        private async void checkBoxM1LoadingBelt_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;
            chkValue = (checkBoxM1LoadingBelt.CheckState == CheckState.Checked) ? true : false;

            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                

                keyToSend = "pcM1StartStopLoadingBelt";
                

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM1LoadingBelt.ImageIndex = (chkValue) ? 2 : 3;
                }
                else
                {
                    checkBoxM1LoadingBelt.ImageIndex = 2;
                }
            }
            else
            {
                if (Properties.Settings.Default.OpcSimulation)
                {
                    checkBoxM1LoadingBelt.ImageIndex = (chkValue) ? 2 : 3;
                }
            }
        }

        private async void buttonM1StartTest_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                //send quote/speed
                M1TestSendProgram();

                //send start command
                string keyToSend = "pcM1StartTest";
                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else xDialog.MsgBox.Show("offline", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Exclamation, xDialog.MsgBox.AnimateStyle.FadeIn);
            }
        }

        private async void buttonM1TestLoadProgram_Click(object sender, EventArgs e)
        {
            if (comboBoxM1TestProgramList.Text == "") return;

            try
            {
                var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

                if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                {
                    ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                    ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");

                    objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[0] + "\\" + comboBoxM1TestProgramList.Text + config.Extensions[0]);
                    if (objPoints != null)
                    {
                        dataGridViewM1TestPoints[1, 0].Value = objPoints.Points[0].Q1;
                        dataGridViewM1TestPoints[1, 1].Value = objPoints.Points[0].Q2;
                        dataGridViewM1TestPoints[1, 2].Value = objPoints.Points[0].Q3;
                        dataGridViewM1TestPoints[1, 3].Value = objPoints.Points[0].Q4;
                        dataGridViewM1TestPoints[2, 0].Value = objPoints.Points[0].V1;
                        dataGridViewM1TestPoints[2, 1].Value = objPoints.Points[0].V2;
                        dataGridViewM1TestPoints[2, 2].Value = objPoints.Points[0].V3;
                        dataGridViewM1TestPoints[2, 3].Value = objPoints.Points[0].V4;
                        numericUpDownM1BootDelayTest.Value = Convert.ToDecimal(objPoints.Points[0].CustomFloatParam);
                        //program succesfully loaded
                        xDialog.MsgBox.Show("program " + comboBoxM1TestProgramList.Text + " succesfully loaded", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Application, xDialog.MsgBox.AnimateStyle.FadeIn);
                    }
                    else
                    {
                        //program succesfully loaded
                        xDialog.MsgBox.Show("program " + comboBoxM1TestProgramList.Text + " not loaded", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Warning, xDialog.MsgBox.AnimateStyle.FadeIn);
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void buttonM1TestSaveProgram_Click(object sender, EventArgs e)
        {
            if (comboBoxM1TestProgramList.Text == "") return;

            try
            {
                var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

                if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                {
                    ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;

                    float p1 = Convert.ToInt32(dataGridViewM1TestPoints[1, 0].Value);
                    float p2 = Convert.ToInt32(dataGridViewM1TestPoints[1, 1].Value);
                    float p3 = Convert.ToInt32(dataGridViewM1TestPoints[1, 2].Value);
                    float p4 = Convert.ToInt32(dataGridViewM1TestPoints[1, 3].Value);
                    int s1 = Convert.ToInt32(dataGridViewM1TestPoints[2, 0].Value);
                    int s2 = Convert.ToInt32(dataGridViewM1TestPoints[2, 1].Value);
                    int s3 = Convert.ToInt32(dataGridViewM1TestPoints[2, 2].Value);
                    int s4 = Convert.ToInt32(dataGridViewM1TestPoints[2, 3].Value);
                    ConcretePointsContainer<PointAxis> prgObj = new ConcretePointsContainer<PointAxis>(comboBoxM1TestProgramList.Text);
                    PointAxis p = new PointAxis(p1, p2, p3, p4, s1, s2, s3, s4);
                    p.CustomFloatParam = float.Parse(numericUpDownM1BootDelayTest.Value.ToString());
                    prgObj.AddPoint(p);
                    prgObj.Save(comboBoxM1TestProgramList.Text + config.Extensions[0], config.ProgramsPath[0], true);
                    xDialog.MsgBox.Show("program " + comboBoxM1TestProgramList.Text + " succesfully saved", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Application, xDialog.MsgBox.AnimateStyle.FadeIn);
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void dataGridViewM1TestPoints_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = 0;
            float[] quote = new float[5];
            short[] speed = new short[5];

            try
            {
                //get selected row index
                int currentRow = int.Parse(e.RowIndex.ToString());

                short idPoint = (short)(currentRow + 1);
                for (i = 0; i <= dataGridViewM1TestPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = float.Parse(dataGridViewM1TestPoints[1, i].Value.ToString());
                    speed[i + 1] = short.Parse(dataGridViewM1TestPoints[2, i].Value.ToString());
                }

                if (idPoint < 0 || idPoint > 4)
                {
                    //todo message
                    return;
                }

                // edit button
                if ((e.ColumnIndex == 3) & currentRow >= 0)
                {
                    OPCUAM1TestPckSend(quote, speed);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void dataGridViewM1TeachPoints_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = 0;
            float[] quote = new float[5];
            short[] speed = new short[5];
            bool[] start = new bool[5] { false, false, false, false, false };

            try
            {
                //get selected row index
                int currentRow = int.Parse(e.RowIndex.ToString());

                short idPoint = (short)(currentRow + 1);
                for (i = 0; i <= dataGridViewM1TeachPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = float.Parse(dataGridViewM1TeachPoints[1, i].Value.ToString());
                    speed[i + 1] = short.Parse(dataGridViewM1TeachPoints[2, i].Value.ToString());
                }

                if (idPoint < 0 || idPoint > 4)
                {
                    xDialog.MsgBox.Show("point selection not admitted", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Exclamation, xDialog.MsgBox.AnimateStyle.FadeIn);
                    return;
                }

                // register button
                if ((e.ColumnIndex == 3) & currentRow >= 0)
                {
                    //register current axis value
                    dataGridViewM1TeachPoints[1, currentRow].Value = float.Parse((labelM1TeachAxisQuoteValue.Text));
                    //register current speed value
                    dataGridViewM1TeachPoints[2, currentRow].Value = Convert.ToInt32((numericUpDownM1JogSpeed.Value));
                }

                // start quote button
                if ((e.ColumnIndex == 4) & currentRow >= 0)
                {
                    start[idPoint] = true;
                    OPCUAM1TeachPckSend(idPoint, quote, speed, start);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void dataGridViewM1TeachPoints_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if ((e.ColumnIndex == 3) && (e.RowIndex >= 0))
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                System.Drawing.Image img = new Bitmap(Properties.Settings.Default.ImagesFilepath + "\\register.png");
                var w = 32;// img.Width;
                var h = 32;// img.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(img, new Rectangle(x, y, w, h));
                e.Handled = true;
            }

            if ((e.ColumnIndex == 4) && (e.RowIndex >= 0))
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                System.Drawing.Image img = new Bitmap(Properties.Settings.Default.ImagesFilepath + "\\startquote.png");
                var w = 32;// img.Width;
                var h = 32;// img.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(img, new Rectangle(x, y, w, h));
                e.Handled = true;
            }

            if ((e.ColumnIndex == 5) && (e.RowIndex >= 0))
            {
                System.Drawing.Image img = new Bitmap(Properties.Settings.Default.ImagesFilepath + "\\preached.png");
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                if (dataGridViewM1TeachPoints.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText == "reached")
                {
                    img = new Bitmap(Properties.Settings.Default.ImagesFilepath + "\\preached.png");
                }
                else
                {
                    img = new Bitmap(Properties.Settings.Default.ImagesFilepath + "\\pnotreached.png");
                }
                var w = 24;// img.Width;
                var h = 24;// img.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(img, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void dataGridViewM1TestPoints_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if ((e.ColumnIndex == 3) && (e.RowIndex >= 0))
            {
                System.Drawing.Image img = new Bitmap(Properties.Settings.Default.ImagesFilepath + "\\preached.png");
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                if (dataGridViewM1TestPoints.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText == "reached")
                {
                    img = new Bitmap(Properties.Settings.Default.ImagesFilepath + "\\preached.png");
                }
                else
                {
                    img = new Bitmap(Properties.Settings.Default.ImagesFilepath + "\\pnotreached.png");
                }
                var w = 24;// img.Width;
                var h = 24;// img.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(img, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }
    }
}