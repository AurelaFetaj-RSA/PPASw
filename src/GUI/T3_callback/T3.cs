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
       

        private async void buttonM2TeachLoadProgram_Click(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[1] + "\\" + comboBoxM2TeachProgramList.Text + config.Extensions[0]);
                if (objPoints != null)
                {
                    dataGridViewM2TeachPoints[1, 0].Value = objPoints.Points[0].Q1;
                    dataGridViewM2TeachPoints[1, 1].Value = objPoints.Points[0].Q2;
                    dataGridViewM2TeachPoints[1, 2].Value = objPoints.Points[0].Q3;
                    dataGridViewM2TeachPoints[1, 3].Value = objPoints.Points[0].Q4;
                    dataGridViewM2TeachPoints[2, 0].Value = objPoints.Points[0].V1;
                    dataGridViewM2TeachPoints[2, 1].Value = objPoints.Points[0].V2;
                    dataGridViewM2TeachPoints[2, 2].Value = objPoints.Points[0].V3;
                    dataGridViewM2TeachPoints[2, 3].Value = objPoints.Points[0].V4;
                    numericUpDownM2TimerBootTeach.Value = Convert.ToDecimal(objPoints.Points[0].CustomFloatParam.ToString());                    
                }
            }
        }

        public async void M2TeachLoadProgram()
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[1] + "\\" + comboBoxM2TeachProgramList.Text + config.Extensions[0]);
                if (objPoints != null)
                {
                    dataGridViewM2TeachPoints[1, 0].Value = objPoints.Points[0].Q1;
                    dataGridViewM2TeachPoints[1, 1].Value = objPoints.Points[0].Q2;
                    dataGridViewM2TeachPoints[1, 2].Value = objPoints.Points[0].Q3;
                    dataGridViewM2TeachPoints[1, 3].Value = objPoints.Points[0].Q4;
                    dataGridViewM2TeachPoints[2, 0].Value = objPoints.Points[0].V1;
                    dataGridViewM2TeachPoints[2, 1].Value = objPoints.Points[0].V2;
                    dataGridViewM2TeachPoints[2, 2].Value = objPoints.Points[0].V3;
                    dataGridViewM2TeachPoints[2, 3].Value = objPoints.Points[0].V4;
                    numericUpDownM2TimerBootTeach.Value = Convert.ToDecimal(objPoints.Points[0].CustomFloatParam.ToString());                    
                }
            }
        }
        private void buttonM2TeachSaveProgram_Click(object sender, EventArgs e)
        {
            if (comboBoxM2TeachProgramList.Text == "") return;
            if (CheckProgramSyntaxName(comboBoxM2TeachProgramList.Text) == false)
            {
                xDialog.MsgBox.Show("Nombre de programa incorrecto. Ejemplo: PRXXXX-YYY-XX00", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Application, xDialog.MsgBox.AnimateStyle.FadeIn);
                return;
            }

            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;

                float p1 = float.Parse(dataGridViewM2TeachPoints[1, 0].Value.ToString());
                float p2 = float.Parse(dataGridViewM2TeachPoints[1, 1].Value.ToString());
                float p3 = float.Parse(dataGridViewM2TeachPoints[1, 2].Value.ToString());
                float p4 = float.Parse(dataGridViewM2TeachPoints[1, 3].Value.ToString());
                int s1 = Convert.ToInt32(dataGridViewM2TeachPoints[2, 0].Value);
                int s2 = Convert.ToInt32(dataGridViewM2TeachPoints[2, 1].Value);
                int s3 = Convert.ToInt32(dataGridViewM2TeachPoints[2, 2].Value);
                int s4 = Convert.ToInt32(dataGridViewM2TeachPoints[2, 3].Value);

                ConcretePointsContainer<PointAxis> prgObj = new ConcretePointsContainer<PointAxis>(comboBoxM1TeachProgramList.Text);
                PointAxis p = new PointAxis(p1, p2, p3, p4, s1, s2, s3, s4);
                p.CustomFloatParam = float.Parse((numericUpDownM2TimerBootTeach.Value.ToString()));
                
                prgObj.AddPoint(p);

                prgObj.Save(comboBoxM2TeachProgramList.Text + config.Extensions[0], config.ProgramsPath[1], true);
                //check if is in auto
                if (comboBoxM2TeachProgramList.Text == M2PrgName)
                {
                    RestartRequestFromM2();
                }
                RefreshM2TeachProgramList();

                //program succesfully saved
                xDialog.MsgBox.Show("programa " + comboBoxM2TeachProgramList.Text + " guardado correctamente", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Application, xDialog.MsgBox.AnimateStyle.FadeIn);

            }
        }
        private void RefreshM2TeachProgramList()
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            List<IObjProgram> pList = new List<IObjProgram>();

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                pList = progRS.GetProgram(config.ProgramsPath[1], config.Extensions, comboBoxM2TeachRecipeName.Text);
                comboBoxM2TeachProgramList.Items.Clear();
                if (pList != null)
                {
                    foreach (IObjProgram prgName in pList)
                    {
                        //filter by model name
                        if (prgName.ProgramName.Contains(comboBoxM2TeachRecipeName.Text))
                            comboBoxM2TeachProgramList.Items.Add(prgName.ProgramName);
                    }
                }
            }
        }
        private void buttonM2TeachNewProgram_Click(object sender, EventArgs e)
        {
            ResetM2Datagrid();
        }

        private void buttonM2TeachDeleteProgram_Click(object sender, EventArgs e)
        {
            if (comboBoxM2TeachProgramList.Text == "") return;

            try
            {
                DialogResult ret = xDialog.MsgBox.Show("Está seguro de que desea eliminar " + comboBoxM2TeachProgramList.Text + "?", "PBoot", xDialog.MsgBox.Buttons.YesNo, xDialog.MsgBox.Icon.Application, xDialog.MsgBox.AnimateStyle.FadeIn);

                if (ret == DialogResult.Yes)
                {
                    var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

                    if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                    {
                        ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                        if (System.IO.File.Exists(config.ProgramsPath[1] + "\\" + comboBoxM2TeachProgramList.Text + config.Extensions[0]))
                        {
                            System.IO.File.Delete(config.ProgramsPath[1] + "\\" + comboBoxM2TeachProgramList.Text + config.Extensions[0]);
                        }
                    }
                    RefreshM2TeachProgramList();
                }
            }
            catch (Exception ex)
            {
                //todo log
            }
        }

        private void dataGridViewM2TeachPoints_CellContentClick(object sender, DataGridViewCellEventArgs e)
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
                for (i = 0; i <= dataGridViewM2TeachPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = float.Parse(dataGridViewM2TeachPoints[1, i].Value.ToString());
                    speed[i + 1] = short.Parse(dataGridViewM2TeachPoints[2, i].Value.ToString());
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
                    dataGridViewM2TeachPoints[1, currentRow].Value = float.Parse((labelM2TeachAxisQuoteValue.Text));
                    //register current speed value
                    dataGridViewM2TeachPoints[2, currentRow].Value = Convert.ToInt32((numericUpDownM2JogSpeed.Value));
                }

                // start quote button
                if ((e.ColumnIndex == 4) & currentRow >= 0)
                {
                    start[idPoint] = true;
                    OPCUAM2TeachPckSend(idPoint, quote, speed, start);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async void buttonM2SmallClampOpening_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2SmallClampOpening";

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

        private async void buttonM2SmallClampClosing_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2SmallClampClosing";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }

        }

        private async void buttonM2BigClampOpening_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2BigClampOpening";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
            }
        }

        private async void buttonM2BigClampClosing_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2BigClampClosing";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
            }
        }

        private async void buttonM2ContrastOpening_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2ContrOpening";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
            }
        }

        private async void buttonM2ContrastClosing_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2ContrClosing";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
            }
        }

        private async void buttonM2CenteringClampsOpening_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2CentrClampOpening";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
            }
        }

        private async void buttonM2CenteringClampsClosing_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2CentrClampClosing";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
            }
        }

        private async void buttonM2ResetServo_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2ResetServoAlarm";

                var sendResult = await ccService.Send(keyToSend, true);

                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonM2PrintCycle_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2Print";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonM2ResetHome_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2ResetHoming";

                var readResult = await ccService.Send(keyToSend, true);

                if (readResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonM2Home_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2Homing";

                var readResult = await ccService.Send(keyToSend, true);

                if (readResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void lbButtonM2JogUp_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;

                if (e.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
                {
                    //send quote
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
                else
                {
                    keyToSend = "pcM2JogDown";
                    var readResult1 = await ccService.Send(keyToSend, false);
                    if (readResult1.OpcResult)
                    {
                    }

                    keyToSend = "pcM2JogUp";
                    var readResult2 = await ccService.Send(keyToSend, false);
                    if (readResult2.OpcResult)
                    {
                    }
                }
            }
        }

        private async void lbButtonM2JogDown_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;

                if (e.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
                {
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
                else
                {
                    keyToSend = "pcM2JogUp";
                    var readResult1 = await ccService.Send(keyToSend, false);
                    if (readResult1.OpcResult)
                    {
                    }

                    keyToSend = "pcM2JogDown";
                    var readResult2 = await ccService.Send(keyToSend, false);
                    if (readResult2.OpcResult)
                    {
                    }
                }
            }
        }

        private async void numericUpDownM2JogSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2JogSpeed";

                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM2JogSpeed.Value.ToString()));

                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void numericUpDownM2ManualQuote_ValueChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2ManualQuote";

                var sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownM2ManualQuote.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
        }

        private async void numericUpDownM2ManualSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM2ManualSpeed";

                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM2ManualSpeed.Value.ToString()));

                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonM2StartQuote_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;

                //quote 
                keyToSend = "pcM2ManualQuote";
                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM2ManualQuote.Value.ToString()));
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }

                keyToSend = "pcM2ManualSpeed";
                var sendResult1 = await ccService.Send(keyToSend, short.Parse(numericUpDownM2ManualSpeed.Value.ToString()));

                keyToSend = "pcM2QuoteStart";
                var sendResult2 = await ccService.Send(keyToSend, true);
                //todo: chi lo mette a false
            }
        }

        private async void checkBoxM2WorkingBelt_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM2StartStopWorkingBelt";
                chkValue = (checkBoxM2WorkingBelt.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM2WorkingBelt.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM2WorkingBelt.ImageIndex = 2;                    
                }
            }
            else
            {
                
            }
        }

        private async void checkBoxM2ExitBelt_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM2StartStopExitBelt";
                chkValue = (checkBoxM2ExitBelt.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM2ExitBelt.ImageIndex = (chkValue) ? 2 : 3;
                }
                else
                {
                    checkBoxM2WorkingBelt.ImageIndex = 2;                    
                }
            }
            else
            {                
            }
        }

        private async void buttonM2TestLoadProgram_Click(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[1] + "\\" + comboBoxM2TestProgramList.Text + config.Extensions[0]);
                if (objPoints != null)
                {

                    dataGridViewM2TestPoints[1, 0].Value = objPoints.Points[0].Q1;
                    dataGridViewM2TestPoints[1, 1].Value = objPoints.Points[0].Q2;
                    dataGridViewM2TestPoints[1, 2].Value = objPoints.Points[0].Q3;
                    dataGridViewM2TestPoints[1, 3].Value = objPoints.Points[0].Q4;
                    dataGridViewM2TestPoints[2, 0].Value = objPoints.Points[0].V1;
                    dataGridViewM2TestPoints[2, 1].Value = objPoints.Points[0].V2;
                    dataGridViewM2TestPoints[2, 2].Value = objPoints.Points[0].V3;
                    dataGridViewM2TestPoints[2, 3].Value = objPoints.Points[0].V4;
                    numericUpDownM2BootDelayTest.Value = Convert.ToDecimal(objPoints.Points[0].CustomFloatParam);
                }
            }
        }

        public async void M2TestLoadProgram()
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[1] + "\\" + comboBoxM2TestProgramList.Text + config.Extensions[0]);
                if (objPoints != null)
                {

                    dataGridViewM2TestPoints[1, 0].Value = objPoints.Points[0].Q1;
                    dataGridViewM2TestPoints[1, 1].Value = objPoints.Points[0].Q2;
                    dataGridViewM2TestPoints[1, 2].Value = objPoints.Points[0].Q3;
                    dataGridViewM2TestPoints[1, 3].Value = objPoints.Points[0].Q4;
                    dataGridViewM2TestPoints[2, 0].Value = objPoints.Points[0].V1;
                    dataGridViewM2TestPoints[2, 1].Value = objPoints.Points[0].V2;
                    dataGridViewM2TestPoints[2, 2].Value = objPoints.Points[0].V3;
                    dataGridViewM2TestPoints[2, 3].Value = objPoints.Points[0].V4;
                    numericUpDownM2BootDelayTest.Value = Convert.ToDecimal(objPoints.Points[0].CustomFloatParam);
                }
            }
        }

        private void buttonM2TestSaveProgram_Click(object sender, EventArgs e)
        {
            if (comboBoxM2TestProgramList.Text == "") return;
            if (CheckProgramSyntaxName(comboBoxM2TestProgramList.Text) == false)
            {
                xDialog.MsgBox.Show("Nombre de programa incorrecto. Ejemplo: PRXXXX-YYY-XX00", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Application, xDialog.MsgBox.AnimateStyle.FadeIn);
                return;
            }

            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;

                float p1 = float.Parse(dataGridViewM2TestPoints[1, 0].Value.ToString());
                float p2 = float.Parse(dataGridViewM2TestPoints[1, 1].Value.ToString());
                float p3 = float.Parse(dataGridViewM2TestPoints[1, 2].Value.ToString());
                float p4 = float.Parse(dataGridViewM2TestPoints[1, 3].Value.ToString());
                int s1 = Convert.ToInt32(dataGridViewM2TestPoints[2, 0].Value);
                int s2 = Convert.ToInt32(dataGridViewM2TestPoints[2, 1].Value);
                int s3 = Convert.ToInt32(dataGridViewM2TestPoints[2, 2].Value);
                int s4 = Convert.ToInt32(dataGridViewM2TestPoints[2, 3].Value);
                ConcretePointsContainer<PointAxis> prgObj = new ConcretePointsContainer<PointAxis>(comboBoxM2TestProgramList.Text);
                PointAxis p = new PointAxis(p1, p2, p3, p4, s1, s2, s3, s4);
                p.CustomFloatParam = float.Parse(numericUpDownM2BootDelayTest.Value.ToString());
                prgObj.AddPoint(p);
                prgObj.Save(comboBoxM2TestProgramList.Text + config.Extensions[0], config.ProgramsPath[1], true);
                if (comboBoxM2TestProgramList.Text == M2PrgName)
                {
                    RestartRequestFromM2();
                }

                //program succesfully saved
                xDialog.MsgBox.Show("programa " + comboBoxM2TestProgramList.Text + " guardado correctamente", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Application, xDialog.MsgBox.AnimateStyle.FadeIn);

            }
        }

        private void dataGridViewM2TestPoints_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = 0;
            float[] quote = new float[5];
            short[] speed = new short[5];

            try
            {
                //get selected row index
                int currentRow = int.Parse(e.RowIndex.ToString());

                short idPoint = (short)(currentRow + 1);
                for (i = 0; i <= dataGridViewM2TestPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = float.Parse(dataGridViewM2TestPoints[1, i].Value.ToString());
                    speed[i + 1] = short.Parse(dataGridViewM2TestPoints[2, i].Value.ToString());
                }

                if (idPoint < 0 || idPoint > 4)
                {
                    //todo message
                    return;
                }

                // edit button
                if ((e.ColumnIndex == 3) & currentRow >= 0)
                {

                    OPCUAM2TestPckSend(quote, speed);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void dataGridViewM2TeachPoints_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
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
                if (dataGridViewM2TeachPoints.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText == "reached")
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

        private async void buttonM2StartTest_Click(object sender, EventArgs e)
        {
            //send quote/speed
            M2TestSendProgram();

            //check phase
            string key = "pcM2PadPrintIntState";
            var readResult = await ccService.Read(key);

            if (readResult.OpcResult)
            {
                if (short.Parse(readResult.Value.ToString()) != 0)
                {
                    xDialog.MsgBox.Show("pad int fase no 0. Prensa RESET.", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Error, xDialog.MsgBox.AnimateStyle.FadeIn);
                    return;
                }
            }
            //send start command
            string keyToSend = "pcM2StartTest";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else xDialog.MsgBox.Show("sin conexión", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Exclamation, xDialog.MsgBox.AnimateStyle.FadeIn);
        }
    }
}