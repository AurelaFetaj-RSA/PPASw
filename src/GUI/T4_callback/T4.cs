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
        private void toolStripComboBoxT4_1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                List<IObjProgram> pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, toolStripComboBoxT4_1.Text);

                comboBoxM3TeachProgramList.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM3TeachProgramList.Items.Add(prgName.ProgramName);
                }
            }
        }
        private void toolStripComboBoxT4_2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                List<IObjProgram> pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, toolStripComboBoxT4_2.Text);

                comboBoxM3TestProgramList.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM3TestProgramList.Items.Add(prgName.ProgramName);
                }
            }
        }

        private async void buttonM3TeachLoadProgram_Click(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[2] + "\\" + comboBoxM3TeachProgramList.Text + config.Extensions[0]);
                if (objPoints != null)
                {
                    dataGridViewM3TeachPoints[1, 0].Value = objPoints.Points[0].Q1;
                    dataGridViewM3TeachPoints[1, 1].Value = objPoints.Points[0].Q2;
                    dataGridViewM3TeachPoints[1, 2].Value = objPoints.Points[0].Q3;
                    dataGridViewM3TeachPoints[1, 3].Value = objPoints.Points[0].Q4;
                    dataGridViewM3TeachPoints[2, 0].Value = objPoints.Points[0].V1;
                    dataGridViewM3TeachPoints[2, 1].Value = objPoints.Points[0].V2;
                    dataGridViewM3TeachPoints[2, 2].Value = objPoints.Points[0].V3;
                    dataGridViewM3TeachPoints[2, 3].Value = objPoints.Points[0].V4;
                }
            }
        }

        private void buttonM3TeachSaveProgram_Click(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;

                int p1 = Convert.ToInt32(dataGridViewM2TeachPoints[1, 0].Value);
                int p2 = Convert.ToInt32(dataGridViewM2TeachPoints[1, 1].Value);
                int p3 = Convert.ToInt32(dataGridViewM2TeachPoints[1, 2].Value);
                int p4 = Convert.ToInt32(dataGridViewM2TeachPoints[1, 3].Value);
                int s1 = Convert.ToInt32(dataGridViewM2TeachPoints[2, 0].Value);
                int s2 = Convert.ToInt32(dataGridViewM2TeachPoints[2, 1].Value);
                int s3 = Convert.ToInt32(dataGridViewM2TeachPoints[2, 2].Value);
                int s4 = Convert.ToInt32(dataGridViewM2TeachPoints[2, 3].Value);
                ConcretePointsContainer<PointAxis> prgObj = new ConcretePointsContainer<PointAxis>(comboBoxM3TeachProgramList.Text);
                prgObj.AddPoint(new PointAxis(p1, p2, p3, p4, s1, s2, s3, s4));
                prgObj.Save(comboBoxM3TeachProgramList.Text + config.Extensions[0], config.ProgramsPath[2], true);
            }
        }

        private void buttonM3TeachNewProgram_Click(object sender, EventArgs e)
        {
            ResetM3Datagrid();
        }

        private void buttonM3TeachDeleteProgram_Click(object sender, EventArgs e)
        {

        }

        private void dataGridViewM3TeachPoints_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = 0;
            short[] quote = new short[5];
            short[] speed = new short[5];
            bool[] start = new bool[5] { false, false, false, false, false };

            try
            {
                //get selected row index
                int currentRow = int.Parse(e.RowIndex.ToString());

                short idPoint = (short)(currentRow + 1);
                for (i = 0; i <= dataGridViewM3TeachPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = short.Parse(dataGridViewM3TeachPoints[1, i].Value.ToString());
                    speed[i + 1] = short.Parse(dataGridViewM3TeachPoints[2, i].Value.ToString());
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
                    dataGridViewM3TeachPoints[1, currentRow].Value = Convert.ToInt32((labelM3TeachAxisQuoteValue.Text));
                    //register current speed value
                    dataGridViewM3TeachPoints[2, currentRow].Value = Convert.ToInt32((numericUpDownM3JogSpeed.Value));
                }

                // start quote button
                if ((e.ColumnIndex == 4) & currentRow >= 0)
                {
                    start[idPoint] = true;
                    OPCUAM3TeachPckSend(idPoint, quote, speed, start);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async void numericUpDownM3JogSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3JogSpeed";

                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM2JogSpeed.Value.ToString()));

                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }
        private async void lbButtonM3JogUp_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
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

        private async void lbButtonM3JogDown_ButtonChangeState(object sender, LBSoft.IndustrialCtrls.Buttons.LBButtonEventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;

                if (e.State == LBSoft.IndustrialCtrls.Buttons.LBButton.ButtonState.Pressed)
                {
                    keyToSend = "pcM3JogUp";
                    var readResult1 = await ccService.Send(keyToSend, false);
                    if (readResult1.OpcResult)
                    {
                    }

                    keyToSend = "pcM3JogDown";
                    var readResult2 = await ccService.Send(keyToSend, true);
                    if (readResult2.OpcResult)
                    {
                    }
                }
                else
                {
                    keyToSend = "pcM3JogUp";
                    var readResult1 = await ccService.Send(keyToSend, false);
                    if (readResult1.OpcResult)
                    {
                    }

                    keyToSend = "pcM3JogDown";
                    var readResult2 = await ccService.Send(keyToSend, false);
                    if (readResult2.OpcResult)
                    {
                    }
                }
            }
        }

        private async void buttonM3StartQuote_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;

                //quote 
                keyToSend = "pcM3ManualQuote";
                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM3ManualQuote.Value.ToString()));
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }

                keyToSend = "pcM3ManualSpeed";
                var sendResult1 = await ccService.Send(keyToSend, short.Parse(numericUpDownM3ManualSpeed.Value.ToString()));

                keyToSend = "pcM3QuoteStart";
                var sendResult2 = await ccService.Send(keyToSend, true);
            }
        }

        private async void numericUpDownM3ManualSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3ManualSpeed";

                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM3ManualSpeed.Value.ToString()));

                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void numericUpDownM3ManualQuote_ValueChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3ManualQuote";

                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM3ManualQuote.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
        }

        private async void buttonM3ResetHome_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3ResetHoming";

                var readResult = await ccService.Send(keyToSend, true);

                if (readResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonM3Home_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3Homing";

                var readResult = await ccService.Send(keyToSend, true);

                if (readResult.OpcResult)
                {

                }
                else
                {

                }
            }

        }

        private async void checkBoxM3ExitBelt_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM3StartStopExitBelt";
                chkValue = (checkBoxM2WorkingBelt.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM3WorkingBelt.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM3WorkingBelt.ImageIndex = 2;
                }
            }
            else
            {

            }


        }

        private async void checkBoxM3WorkingBelt_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM3StartStopWorkingBelt";
                chkValue = (checkBoxM2WorkingBelt.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM3WorkingBelt.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM3WorkingBelt.ImageIndex = 2;
                }
            }
            else
            {

            }

        }

        private async void buttonM3SmallClampOpening_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3SmallClampOpening";

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

        private async void buttonM3SmallClampClosing_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3SmallClampClosing";

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

        private async void buttonM3BigClampOpening_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3BigClampOpening";

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

        private async void buttonM3BigClampClosing_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3BigClampClosing";

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


        private async void buttonM3ResetServo_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3ResetServoAlarm";

                var sendResult = await ccService.Send(keyToSend, true);

                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonM3CenteringClampsOpening_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3CentrClampOpening";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
            }
        }

        private async void buttonM3CenteringClampsClosing_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3CentrClampClosing";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
            }
        }

        private async void buttonM3ContrastOpening_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3ContrOpening";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
            }
        }

        private async void buttonM3ContrastClosing_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3ContrClosing";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
            }
        }

        private async void buttonM3PrintCycle_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3Print";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }
    }
}
