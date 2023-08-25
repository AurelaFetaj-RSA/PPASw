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

namespace GUI
{
    public partial class FormApp : Form
    {
        private void comboBoxAutoModelNameLst_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));
            List<IObjProgram> pList = new List<IObjProgram>();

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                pList = progRS.GetProgram(config.ProgramsPath[0], config.Extensions, comboBoxAutoModelNameLst.Text);
                comboBoxM1PrgName.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM1PrgName.Items.Add(prgName.ProgramName);
                }

                pList = progRS.GetProgram(config.ProgramsPath[1], config.Extensions, comboBoxAutoModelNameLst.Text);
                comboBoxM2PrgName.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM2PrgName.Items.Add(prgName.ProgramName);
                }

                pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, comboBoxAutoModelNameLst.Text);
                comboBoxM3PrgName_st1.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM3PrgName_st1.Items.Add(prgName.ProgramName);
                }

                pList = progRS.GetProgram(config.ProgramsPath[2], config.Extensions, comboBoxAutoModelNameLst.Text);
                comboBoxM3PrgName_st2.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM3PrgName_st2.Items.Add(prgName.ProgramName);
                }

                //da rimuovere
                pList = progRS.GetProgram(config.ProgramsPath[1], config.Extensions, comboBoxAutoModelNameLst.Text);
                comboBoxM4PrgName.Items.Clear();

                foreach (IObjProgram prgName in pList)
                {
                    comboBoxM4PrgName.Items.Add(prgName.ProgramName);
                }
            }
        }

        private async void comboBoxM1PrgName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //send recipe (todo: waiting mysql)
            string keyValue = "pcM1Param1";
            var sendResult = await ccService.Send(keyValue, short.Parse(textBoxM1Test.Text));
            labelM1Param1Value.Text = textBoxM1Test.Text;

            //send quote, speed
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[0] + "\\" + comboBoxM1PrgName.Text + config.Extensions[0]);
                if (objPoints != null)
                {
                    List<string> keys = new List<string>()
                    {
                        "pcM1AutoQuote",
                        "pcM1AutoSpeed"
                    };

                    List<object> values = new List<object>()
                    {
                        new short[] { (short)objPoints.Points[0].Q1, (short)objPoints.Points[0].Q2, (short)objPoints.Points[0].Q3, (short)objPoints.Points[0].Q4},
                        new short[] { (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                    };

                    var sendResults = await ccService.Send(keys, values);
                    bool allsent = true;
                    foreach (var result in sendResults)
                    {
                        if (result.Value.OpcResult)
                        {
                        }
                        else
                        {
                            AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.trimmer, "error sending " + result.Value.NodeString);
                        }
                        allsent = allsent & result.Value.OpcResult;
                    }
                    if (allsent) AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.trimmer, "program sent succesfully");  
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.trimmer, "verify program file");
            }
        }
        private async void comboBoxM2PrgName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //todo: send recipe (todo: waiting mysql integration)
            string keyValue = "pcM2Param1";
            var sendResult = await ccService.Send(keyValue, short.Parse(textBoxM2Test.Text));
            if (sendResult.OpcResult)
            {
                labelM2Param1Value.Text = textBoxM2Test.Text;
            }
            else
            {
                //todo manage log/user error
            }

            //send quote, speed
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[1] + "\\" + comboBoxM2PrgName.Text + config.Extensions[0]);
                if (objPoints != null)
                {
                    List<string> keys = new List<string>()
                    {
                        "pcM2AutoQuote",
                        "pcM2AutoSpeed"
                    };

                    List<object> values = new List<object>()
                    {
                        new short[] {0, (short)objPoints.Points[0].Q1, (short)objPoints.Points[0].Q2, (short)objPoints.Points[0].Q3, (short)objPoints.Points[0].Q4},
                        new short[] {0, (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                    };

                    var sendResults = await ccService.Send(keys, values);
                    bool allsent = true;
                    foreach (var result in sendResults)
                    {
                        if (result.Value.OpcResult)
                        {
                        }
                        else
                        {
                            AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padprintInt, "error sending " + result.Value.NodeString);
                        }
                        allsent = allsent & result.Value.OpcResult;
                    }
                    if (allsent) AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padprintInt, "program sent succesfully");                    
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padprintInt, "verify program file");
            }
        }
        private async void comboBoxM3PrgName_st1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //send recipe
            string keyValue = "pcM3Param1";
            var sendResult = await ccService.Send(keyValue, short.Parse(textBoxM3Test.Text));

            //send type order: RG, LF
            //type order 2: LF, RG
            keyValue = "pcM3TypeOrder";
            sendResult = await ccService.Send(keyValue, short.Parse(textBoxTypeOrder.Text));


            //send quote, speed
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[2] + "\\" + comboBoxM3PrgName_st1.Text + config.Extensions[0]);
                if (objPoints != null)
                {
                    List<string> keys = new List<string>()
                    {
                        "pcM3AutoQuoteSt1",
                        "pcM3AutoSpeedSt1"
                    };

                    List<object> values = new List<object>()
                    {
                        new short[] {0, (short)objPoints.Points[0].Q1, (short)objPoints.Points[0].Q2, (short)objPoints.Points[0].Q3, (short)objPoints.Points[0].Q4},
                        new short[] {0, (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                    };

                    var sendResults = await ccService.Send(keys, values);
                    bool allsent = true;
                    foreach (var result in sendResults)
                    {
                        if (result.Value.OpcResult)
                        {
                        }
                        else
                        {
                            AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padprintExt, "error sending " + result.Value.NodeString + " station 1");
                        }
                        allsent = allsent & result.Value.OpcResult;
                    }
                    if (allsent) AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padprintExt, "program sent succesfully " + "station 1");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padprintExt, "verify program file " + "station 1");
            }
        }
        private async void comboBoxM3PrgName_st2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //send recipe
            string keyValue = "pcM3Param1";
            var sendResult = await ccService.Send(keyValue, short.Parse(textBoxM3Test.Text));
            labelM3Param1Value.Text = textBoxM3Test.Text;
            keyValue = "pcM3TypeOrder";
            sendResult = await ccService.Send(keyValue, short.Parse(textBoxTypeOrder.Text));
            labelM3Param1Value.Text = textBoxM3Test.Text;
            //send quote, speed
            var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

            if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
            {
                ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
                ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
                objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[2] + "\\" + comboBoxM3PrgName_st2.Text + config.Extensions[0]);
                if (objPoints != null)
                {
                    List<string> keys = new List<string>()
                    {
                        "pcM3AutoQuoteSt2",
                        "pcM3AutoSpeedSt2"
                    };

                    List<object> values = new List<object>()
                    {
                        new short[] {0, (short)objPoints.Points[0].Q1, (short)objPoints.Points[0].Q2, (short)objPoints.Points[0].Q3, (short)objPoints.Points[0].Q4},
                        new short[] {0,  (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
                    };

                    var sendResults = await ccService.Send(keys, values);
                    bool allsent = true;
                    foreach (var result in sendResults)
                    {
                        if (result.Value.OpcResult)
                        {
                        }
                        else
                        {
                            AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padprintExt, "error sending " + result.Value.NodeString + " station 2");
                        }
                        allsent = allsent & result.Value.OpcResult;
                    }
                    if (allsent) AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padprintExt, "program sent succesfully " + "station 2");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padprintExt, "verify program file " + "station 2");
            }
        }
        private async void comboBoxM4PrgName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //send recipe (todo: waiting mysql)
            string keyValue = "pcM4Param1";
            var sendResult = await ccService.Send(keyValue, short.Parse(textBoxM4Test.Text));
            labelM4Param1Value.Text = textBoxM4Test.Text;

            string keyToSend = "pcM4ProgramName";

            var readResult = await ccService.Send(keyToSend, "");
            if (readResult.OpcResult)
            {

            }
            else
            {

            }

            keyToSend = "pcM4ProgramName";

            readResult = await ccService.Send(keyToSend, comboBoxM4PrgName.Text);
            if (readResult.OpcResult)
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.normal, Machine.padLaser, "program sent succesfully");
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.high, Machine.padLaser, "error sending " + readResult.NodeString);
            }
        }
        private async void checkBoxM1Inclusion_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM1Inclusion";
                chkValue = (checkBoxM1Inclusion.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM1Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM1Inclusion.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.trimmer, "trimmer offline");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.trimmer, "system offline");
            }
        }
        private async void checkBoxM2Inclusion_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM2Inclusion";
                chkValue = (checkBoxM2Inclusion.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM2Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM2Inclusion.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padprintInt, "padprint int offline");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padprintInt, "system offline");
            }
        }
        private async void checkBoxM3Inclusion_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM3Inclusion";
                chkValue = (checkBoxM3Inclusion.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM3Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM3Inclusion.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padprintExt, "padprint ext offline");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padprintExt, "system offline");
            }
        }
        private async void checkBoxM4Inclusion_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM4Inclusion";
                chkValue = (checkBoxM4Inclusion.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM4Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM4Inclusion.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padLaser, "padlaser offline");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padLaser, "system offline");
            }
        }
        private async void checkBoxM5Inclusion_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM5Inclusion";
                chkValue = (checkBoxM5Inclusion.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM5Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM5Inclusion.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.manipulator, "manipulator offline");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.manipulator, "system offline");
            }
        }
        private async void checkBoxM6Inclusion_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM6Inclusion";
                chkValue = (checkBoxM6Inclusion.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM6Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM6Inclusion.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.oven, "oven offline");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.oven, "system offline");
            }
        }

        private void checkBoxM1Param1_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM1Param1.CheckState == CheckState.Checked) ? true : false;

            checkBoxM1Param1.ImageIndex = (chkValue) ? 0 : 1;
            labelM1Param1.Text = (chkValue) ? "on" : "off";
        }

        private void checkBoxM2Param1_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM2Param1.CheckState == CheckState.Checked) ? true : false;

            checkBoxM2Param1.ImageIndex = (chkValue) ? 0 : 1;
            labelM2Param1.Text = (chkValue) ? "on" : "off";
        }

        private void checkBoxM3Param1_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM3Param1.CheckState == CheckState.Checked) ? true : false;

            checkBoxM3Param1.ImageIndex = (chkValue) ? 0 : 1;
            labelM3Param1.Text = (chkValue) ? "on" : "off";
        }

        private void checkBoxM4Param1_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM4Param1.CheckState == CheckState.Checked) ? true : false;

            checkBoxM4Param1.ImageIndex = (chkValue) ? 0 : 1;
            labelM4Param1.Text = (chkValue) ? "on" : "off";
        }

        private void checkBoxM5Param1_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM5Param1.CheckState == CheckState.Checked) ? true : false;

            checkBoxM5Param1.ImageIndex = (chkValue) ? 0 : 1;
            labelM5Param1.Text = (chkValue) ? "on" : "off";
        }

        private void checkBoxM6Param1_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM6Param1.CheckState == CheckState.Checked) ? true : false;

            checkBoxM6Param1.ImageIndex = (chkValue) ? 0 : 1;
            labelM6Param1.Text = (chkValue) ? "on" : "off";
        }

        private void tabPageT0_3_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;
            Pen myPen = new Pen(Color.Black);

            g = e.Graphics;

            if (ccService.ClientIsConnected) myPen = new Pen(Color.FromArgb(107, 227, 162));
            
            myPen.Width = 10;

            int lineHeight = 80;
            int groupboxXShift = groupBoxKeepAliveFromPlc.Location.X;
            int groupboxYShift = groupBoxKeepAliveFromPlc.Location.Y;
            g.DrawLine(myPen, pictureBoxM1PLCNode.Location.X + pictureBoxM1PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM1PLCNode.Location.Y - pictureBoxM1PLCNode.Height + groupboxYShift, pictureBoxM1PLCNode.Location.X + pictureBoxM1PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM1PLCNode.Location.Y - pictureBoxM1PLCNode.Height + lineHeight + groupboxYShift);
            g.DrawLine(myPen, pictureBoxM2PLCNode.Location.X + pictureBoxM2PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM2PLCNode.Location.Y - pictureBoxM2PLCNode.Height + groupboxYShift, pictureBoxM2PLCNode.Location.X + pictureBoxM2PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM2PLCNode.Location.Y - pictureBoxM2PLCNode.Height + lineHeight + groupboxYShift);
            g.DrawLine(myPen, pictureBoxM3PLCNode.Location.X + pictureBoxM3PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM3PLCNode.Location.Y - pictureBoxM3PLCNode.Height + groupboxYShift, pictureBoxM3PLCNode.Location.X + pictureBoxM3PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM3PLCNode.Location.Y - pictureBoxM3PLCNode.Height + lineHeight + groupboxYShift);
            g.DrawLine(myPen, pictureBoxM4PLCNode.Location.X + pictureBoxM4PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM4PLCNode.Location.Y - pictureBoxM4PLCNode.Height + groupboxYShift, pictureBoxM4PLCNode.Location.X + pictureBoxM4PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM4PLCNode.Location.Y - pictureBoxM4PLCNode.Height + lineHeight + groupboxYShift);
            g.DrawLine(myPen, pictureBoxM5PLCNode.Location.X + pictureBoxM5PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM5PLCNode.Location.Y - pictureBoxM5PLCNode.Height + groupboxYShift, pictureBoxM5PLCNode.Location.X + pictureBoxM5PLCNode.Size.Width / 2 + groupboxXShift, pictureBoxM5PLCNode.Location.Y - pictureBoxM5PLCNode.Height + lineHeight + groupboxYShift);

            g.DrawLine(myPen, pictureBoxM1PLCNode.Location.X + groupboxXShift, pictureBoxM1PLCNode.Location.Y - lineHeight + groupboxYShift, pictureBoxM5PLCNode.Location.X + pictureBoxM1PLCNode.Width + groupboxXShift, pictureBoxM1PLCNode.Location.Y - lineHeight + groupboxYShift);
            g.DrawLine(myPen, pictureBoxIOTNode.Location.X + pictureBoxIOTNode.Width / 2 + groupboxXShift, pictureBoxIOTNode.Location.Y + pictureBoxIOTNode.Height + groupboxYShift, pictureBoxIOTNode.Location.X + pictureBoxIOTNode.Width / 2 + groupboxXShift, pictureBoxIOTNode.Location.Y + pictureBoxIOTNode.Height + lineHeight - 10 + groupboxYShift);

        }
    }
}