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
    public partial class FormApp : Form    {
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
                    numericUpDownM3TimerBootTeach.Value = Convert.ToDecimal(objPoints.Points[0].CustomFloatParam.ToString());
                }
            }
        }

        private void buttonM3TeachSaveProgram_Click(object sender, EventArgs e)
        {
            if (comboBoxM3TeachProgramList.Text == "") return;
            if (CheckProgramSyntaxName(comboBoxM3TeachProgramList.Text) == false)
            {
                xDialog.MsgBox.Show("Nombre de programa incorrecto. Ejemplo: PRXXXX-YYY-XX00", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Application, xDialog.MsgBox.AnimateStyle.FadeIn);
                return;
            }

            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;

                float p1 = float.Parse(dataGridViewM3TeachPoints[1, 0].Value.ToString());
                float p2 = float.Parse(dataGridViewM3TeachPoints[1, 1].Value.ToString());
                float p3 = float.Parse(dataGridViewM3TeachPoints[1, 2].Value.ToString());
                float p4 = float.Parse(dataGridViewM3TeachPoints[1, 3].Value.ToString());
                int s1 = Convert.ToInt32(dataGridViewM3TeachPoints[2, 0].Value);
                int s2 = Convert.ToInt32(dataGridViewM3TeachPoints[2, 1].Value);
                int s3 = Convert.ToInt32(dataGridViewM3TeachPoints[2, 2].Value);
                int s4 = Convert.ToInt32(dataGridViewM3TeachPoints[2, 3].Value);

                ConcretePointsContainer<PointAxis> prgObj = new ConcretePointsContainer<PointAxis>(comboBoxM1TeachProgramList.Text);
                PointAxis p = new PointAxis(p1, p2, p3, p4, s1, s2, s3, s4);
                p.CustomFloatParam = float.Parse((numericUpDownM3TimerBootTeach.Value.ToString()));
                prgObj.AddPoint(p);

                prgObj.Save(comboBoxM3TeachProgramList.Text + config.Extensions[0], config.ProgramsPath[2], true);
                if ((comboBoxM3TeachProgramList.Text == comboBoxM3PrgName_st1.Text) || (comboBoxM3TeachProgramList.Text == comboBoxM3PrgName_st2.Text))
                {
                    RestartRequestFromM3();
                }
                RefreshM3TeachProgramList();

                //program succesfully saved
                xDialog.MsgBox.Show("program " + comboBoxM3TeachProgramList.Text + " succesfully saved", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Application, xDialog.MsgBox.AnimateStyle.FadeIn);

            }
        }
        private void RefreshM3TeachProgramList()
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            List<IObjProgram> pList = new List<IObjProgram>();

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, comboBoxM3TeachRecipeName.Text);
                comboBoxM3TeachProgramList.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    //filter by model name
                    if (prgName.ProgramName.Contains(comboBoxM3TeachRecipeName.Text))
                        comboBoxM3TeachProgramList.Items.Add(prgName.ProgramName);
                }
            }
        }
        private void buttonM3TeachNewProgram_Click(object sender, EventArgs e)
        {
            ResetM3Datagrid();
        }

        private void buttonM3TeachDeleteProgram_Click(object sender, EventArgs e)
        {
            if (comboBoxM3TeachProgramList.Text == "") return;

            try
            {
                DialogResult ret = xDialog.MsgBox.Show("Are you sure you want to delete " + comboBoxM3TeachProgramList.Text + "?", "PBoot", xDialog.MsgBox.Buttons.YesNo, xDialog.MsgBox.Icon.Application, xDialog.MsgBox.AnimateStyle.FadeIn);

                if (ret == DialogResult.Yes)
                {
                    var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

                    if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
                    {
                        ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                        if (System.IO.File.Exists(config.ProgramsPath[2] + "\\" + comboBoxM3TeachProgramList.Text + config.Extensions[0]))
                        {
                            System.IO.File.Delete(config.ProgramsPath[2] + "\\" + comboBoxM3TeachProgramList.Text + config.Extensions[0]);
                        }
                    }
                    RefreshM3TeachProgramList();
                }
            }
            catch (Exception ex)
            {
                //todo log
            }
        }

        private void dataGridViewM3TeachPoints_CellContentClick(object sender, DataGridViewCellEventArgs e)
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
                for (i = 0; i <= dataGridViewM3TeachPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = float.Parse(dataGridViewM3TeachPoints[1, i].Value.ToString());
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
                    dataGridViewM3TeachPoints[1, currentRow].Value = float.Parse((labelM3TeachAxisQuoteValue.Text));
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

                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM3JogSpeed.Value.ToString()));

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
                    keyToSend = "pcM3JogDown";
                    var readResult1 = await ccService.Send(keyToSend, false);
                    if (readResult1.OpcResult)
                    {
                    }

                    keyToSend = "pcM3JogUp";
                    var readResult2 = await ccService.Send(keyToSend, true);
                    if (readResult2.OpcResult)
                    {
                    }
                }
                else
                {
                    keyToSend = "pcM3JogDown";
                    var readResult1 = await ccService.Send(keyToSend, false);
                    if (readResult1.OpcResult)
                    {
                    }

                    keyToSend = "pcM3JogUp";
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
                var sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownM3ManualQuote.Value.ToString()));
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

                var sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownM3ManualQuote.Value.ToString()));

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
                chkValue = (checkBoxM3ExitBelt.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM3ExitBelt.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM3ExitBelt.ImageIndex = 2;
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
                chkValue = (checkBoxM3WorkingBelt.CheckState == CheckState.Checked) ? true : false;

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

        private async void buttonM3NormTransvFwd_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3NormTransvFwd";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonM3NormTransvBwd_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3NormTransvBwd";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonM3NormHoriFwd_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3NormHoriFwd";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonM3NormHoriBwd_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3NormHoriBwd";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private void dataGridViewM3TestPoints_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = 0;
            float[] quote = new float[5];
            short[] speed = new short[5];

            try
            {
                //get selected row index
                int currentRow = int.Parse(e.RowIndex.ToString());

                short idPoint = (short)(currentRow + 1);
                for (i = 0; i <= dataGridViewM3TestPoints.RowCount - 1; i++)
                {
                    quote[i + 1] = short.Parse(dataGridViewM3TestPoints[1, i].Value.ToString());
                    speed[i + 1] = short.Parse(dataGridViewM3TestPoints[2, i].Value.ToString());
                }

                if (idPoint < 0 || idPoint > 4)
                {
                    //todo message
                    return;
                }

                // edit button
                if ((e.ColumnIndex == 3) & currentRow >= 0)
                {
                    OPCUAM3TestPckSend(quote, speed);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void dataGridViewM3TeachPoints_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
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
                if (dataGridViewM3TeachPoints.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText == "reached")
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

        private async void buttonM3RotationCW_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3CWRotation";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonM3RotationCCW_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM3CCWRotation";

                var sendResult = await ccService.Send(keyToSend, true);
                if (sendResult.OpcResult)
                {

                }
                else
                {

                }
            }
        }

        private async void buttonM3TestLoadProgram_Click(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[2] + "\\" + comboBoxM3TestProgramList.Text + config.Extensions[0]);
                if (objPoints != null)
                {

                    dataGridViewM3TestPoints[1, 0].Value = objPoints.Points[0].Q1;
                    dataGridViewM3TestPoints[1, 1].Value = objPoints.Points[0].Q2;
                    dataGridViewM3TestPoints[1, 2].Value = objPoints.Points[0].Q3;
                    dataGridViewM3TestPoints[1, 3].Value = objPoints.Points[0].Q4;
                    dataGridViewM3TestPoints[2, 0].Value = objPoints.Points[0].V1;
                    dataGridViewM3TestPoints[2, 1].Value = objPoints.Points[0].V2;
                    dataGridViewM3TestPoints[2, 2].Value = objPoints.Points[0].V3;
                    dataGridViewM3TestPoints[2, 3].Value = objPoints.Points[0].V4;
                    numericUpDownM3BootDelayTest.Value = Convert.ToDecimal(objPoints.Points[0].CustomFloatParam);
                }
            }
        }
      

        private void buttonM3TestSaveProgram_Click(object sender, EventArgs e)
        {
            if (comboBoxM3TestProgramList.Text == "") return;
            if (CheckProgramSyntaxName(comboBoxM3TestProgramList.Text) == false)
            {
                xDialog.MsgBox.Show("Nombre de programa incorrecto. Ejemplo: PRXXXX-YYY-XX00", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Application, xDialog.MsgBox.AnimateStyle.FadeIn);
                return;
            }

            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;

                float p1 = float.Parse(dataGridViewM3TestPoints[1, 0].Value.ToString());
                float p2 = float.Parse(dataGridViewM3TestPoints[1, 1].Value.ToString());
                float p3 = float.Parse(dataGridViewM3TestPoints[1, 2].Value.ToString());
                float p4 = float.Parse(dataGridViewM3TestPoints[1, 3].Value.ToString());
                int s1 = Convert.ToInt32(dataGridViewM3TestPoints[2, 0].Value);
                int s2 = Convert.ToInt32(dataGridViewM3TestPoints[2, 1].Value);
                int s3 = Convert.ToInt32(dataGridViewM3TestPoints[2, 2].Value);
                int s4 = Convert.ToInt32(dataGridViewM3TestPoints[2, 3].Value);
                ConcretePointsContainer<PointAxis> prgObj = new ConcretePointsContainer<PointAxis>(comboBoxM3TestProgramList.Text);

                PointAxis p = new PointAxis(p1, p2, p3, p4, s1, s2, s3, s4);
                p.CustomFloatParam = float.Parse(numericUpDownM3BootDelayTest.Value.ToString());
                prgObj.AddPoint(p);
                prgObj.Save(comboBoxM3TestProgramList.Text + config.Extensions[0], config.ProgramsPath[2], true);
                if ((comboBoxM3TestProgramList.Text == comboBoxM3PrgName_st1.Text) || (comboBoxM3TestProgramList.Text == comboBoxM3PrgName_st2.Text))
                {
                    RestartRequestFromM3();
                }
                //program succesfully saved
                xDialog.MsgBox.Show("program " + comboBoxM3TestProgramList.Text + " succesfully saved", "PBoot", xDialog.MsgBox.Buttons.OK, xDialog.MsgBox.Icon.Application, xDialog.MsgBox.AnimateStyle.FadeIn);
            }
        }

        private void tabPageT4_3_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pInc = new Pen(Color.FromArgb(107, 227, 162), 10);
            Pen pExt = new Pen(Color.Black, 10);
            Brush bInc = new SolidBrush(Color.FromArgb(107, 227, 162));
            Brush bExt = new SolidBrush(Color.Black);
            Brush bText = new SolidBrush(Color.Black);
            int w = 12;
            int h = 12;
            int i = 1001;
            g.TranslateTransform(10, 10);

            foreach (KeyValuePair<int, bool> entry in M3InputDictionary)
            {
                if (entry.Value == true)
                {
                    g.DrawEllipse(pInc, 0, 0, w, h);
                    g.FillEllipse(bInc, new Rectangle(new Point(0, 0), new Size(w, h)));
                }
                else
                {
                    g.DrawEllipse(pExt, 0, 0, w, h);
                    g.FillEllipse(bExt, new Rectangle(new Point(0, 0), new Size(w, h)));
                }
                string tmp1 = "M3" + "_INPUT";
                string tmp2 = i.ToString();
                g.DrawString(inputConfigurator.GetValue(tmp1, tmp2, ""), new Font("Verdana", 10), bText, new Point(20, 0));
                if (i < 1024)
                    g.TranslateTransform(0, 30);
                else if (i == 1024)
                {
                    g.ResetTransform();
                    g.TranslateTransform(360, 10);
                }
                else
                {
                    g.TranslateTransform(0, 30);

                }
                i = i + 1;
            }
        }

        private void tabPageT4_4_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pInc = new Pen(Color.FromArgb(107, 227, 162), 10);
            Pen pExt = new Pen(Color.Black, 10);
            Brush bInc = new SolidBrush(Color.FromArgb(107, 227, 162));
            Brush bExt = new SolidBrush(Color.Black);
            Brush bText = new SolidBrush(Color.Black);
            int w = 12;
            int h = 12;
            int i = 2001;
            g.TranslateTransform(10, 10);

            foreach (KeyValuePair<int, bool> entry in M4OutputDictionary)
            {
                if (entry.Value == true)
                {
                    g.DrawEllipse(pInc, 0, 0, w, h);
                    g.FillEllipse(bInc, new Rectangle(new Point(0, 0), new Size(w, h)));
                }
                else
                {
                    g.DrawEllipse(pExt, 0, 0, w, h);
                    g.FillEllipse(bExt, new Rectangle(new Point(0, 0), new Size(w, h)));
                }
                string tmp1 = "M4" + "_OUTPUT";
                string tmp2 = i.ToString();
                g.DrawString(outputConfigurator.GetValue(tmp1, tmp2, ""), new Font("Verdana", 10), bText, new Point(20, 0));
                if (i < 2024)
                    g.TranslateTransform(0, 30);
                else if (i == 2024)
                {
                    g.ResetTransform();
                    g.TranslateTransform(360, 10);
                }
                else
                {
                    g.TranslateTransform(0, 30);

                }
                i = i + 1;
            }
        }
    }
}
