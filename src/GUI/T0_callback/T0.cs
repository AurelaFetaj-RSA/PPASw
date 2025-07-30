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
using RSAPoints.ConcretePoints;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace GUI
{
    public partial class FormApp : Form
    {
        private async void checkBoxM1Inclusion_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM1Inclusion";
                chkValue = (checkBoxM1Inclusion.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);
                M1inc = (chkValue == true) ? "1" : "0";
                if (sendResult.OpcResult)
                {
                    checkBoxM1Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM1Inclusion.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.trimmer, "refiladora sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
   
        private async void checkBoxM2Inclusion_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM2Inclusion";
                chkValue = (checkBoxM2Inclusion.CheckState == CheckState.Checked) ? true : false;
                M2inc = (chkValue == true) ? "1" : "0";
                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM2Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM2Inclusion.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padprintInt, "padprint int sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private async void checkBoxM3Inclusion_CheckStateChanged(object sender, EventArgs e)
        {
            if (M3InLine == "0") return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM3Inclusion";
                chkValue = (checkBoxM3Inclusion.CheckState == CheckState.Checked) ? true : false;
                M3inc = (chkValue == true) ? "1" : "0";
                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM3Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM3Inclusion.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padprintExt, "padprint ext sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private async void checkBoxM4Inclusion_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM4Inclusion";
                chkValue = (checkBoxM4Inclusion.CheckState == CheckState.Checked) ? true : false;
                M3inc = (chkValue == true) ? "1" : "0";
                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM4Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM4Inclusion.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padLaser, "padlaser sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
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
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.manipulator, "manipulador sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
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
                M6inc = (chkValue == true) ? "1" : "0";
                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM6Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM6Inclusion.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.oven, "horno sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private void checkBoxM1Param1_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM1Param1.CheckState == CheckState.Checked) ? true : false;

            checkBoxM1Param1.ImageIndex = (chkValue) ? 0 : 1;
            checkBoxM1Param1.Text = (chkValue) ? "refiladora on" : "refiladora off";
        }
        private void checkBoxM2Param1_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM2Param1.CheckState == CheckState.Checked) ? true : false;

            checkBoxM2Param1.ImageIndex = (chkValue) ? 0 : 1;
            checkBoxM2Param1.Text = (chkValue) ? "tamp. int. on" : "tamp. int. off";
        }
        private void checkBoxM3Param1_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM3Param1.CheckState == CheckState.Checked) ? true : false;

            checkBoxM3Param1.ImageIndex = (chkValue) ? 0 : 1;
            checkBoxM3Param1.Text = (chkValue) ? "tamp. ext. on" : "tamp. ext. off";
        }
        private void checkBoxM4Param1_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM4Param1.CheckState == CheckState.Checked) ? true : false;

            checkBoxM4Param1.ImageIndex = (chkValue) ? 0 : 1;
            checkBoxM4Param1.Text = (chkValue) ? "laser on" : "laser off";
        }
        private void checkBoxM5Param1_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM5Param1.CheckState == CheckState.Checked) ? true : false;

            checkBoxM5Param1.ImageIndex = (chkValue) ? 0 : 1;
            checkBoxM5Param1.Text = (chkValue) ? "horno on" : "horno off";
        }
        private void checkBoxM6Param1_CheckStateChanged(object sender, EventArgs e)
        {
            bool chkValue = false;

            chkValue = (checkBoxM6Param1.CheckState == CheckState.Checked) ? true : false;

            checkBoxM6Param1.ImageIndex = (chkValue) ? 0 : 1;
            checkBoxM6Param1.Text = (chkValue) ? "manip. on" : "manip. off";
        }
        private void tabPageT0_3_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;
            Pen myPen = new Pen(Color.Black);

            g = e.Graphics;

            if (ccService != null) if (ccService.ClientIsConnected) myPen = new Pen(Color.FromArgb(107, 227, 162));
            
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
        private async void checkBoxM1Start_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                bool state = false;
                //send start/stop 
                state = (checkBoxM1Start.CheckState == CheckState.Checked) ? true : false;
                string keyValue = "pcM1StartStop";
                var sendResult = await ccService.Send(keyValue, state);
                if (sendResult.OpcResult)
                {
                    if (state) checkBoxM1Start.ImageIndex = 1;
                    else checkBoxM1Start.ImageIndex = 0;
                }
                else
                {
                    checkBoxM1Start.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.trimmer, "refiladora sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private async void checkBoxM2Start_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                bool state = false;
                //send start/stop 
                state = (checkBoxM2Start.CheckState == CheckState.Checked) ? true : false;
                string keyValue = "pcM2StartStop";
                var sendResult = await ccService.Send(keyValue, state);
                if (sendResult.OpcResult)
                {
                    if (state) checkBoxM2Start.ImageIndex = 1;
                    else checkBoxM2Start.ImageIndex = 0;
                }
                else
                {
                    checkBoxM2Start.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padprintInt, "padprint int sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private async void checkBoxM3Start_CheckStateChanged(object sender, EventArgs e)
        {
            if (M3InLine == "0") return;
            if (ccService.ClientIsConnected)
            {
                bool state = false;
                //send start/stop 
                state = (checkBoxM3Start.CheckState == CheckState.Checked) ? true : false;
                string keyValue = "pcM3StartStop";
                var sendResult = await ccService.Send(keyValue, state);
                if (sendResult.OpcResult)
                {
                    if (state) checkBoxM3Start.ImageIndex = 1;
                    else checkBoxM3Start.ImageIndex = 0;
                }
                else
                {
                    checkBoxM3Start.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padprintExt, "padprint ext sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private async void checkBoxM4Start_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                bool state = false;
                //send start/stop 
                state = (checkBoxM4Start.CheckState == CheckState.Checked) ? true : false;
                string keyValue = "pcM4StartStop";
                var sendResult = await ccService.Send(keyValue, state);
                if (sendResult.OpcResult)
                {
                    if (state) checkBoxM4Start.ImageIndex = 1;
                    else checkBoxM4Start.ImageIndex = 0;
                }
                else
                {
                    checkBoxM4Start.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padLaser, "padlaser sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private async void checkBoxM5Start_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                bool state = false;
                //send start/stop 
                state = (checkBoxM5Start.CheckState == CheckState.Checked) ? true : false;
                string keyValue = "pcM5StartStop";
                var sendResult = await ccService.Send(keyValue, state);
                if (sendResult.OpcResult)
                {
                    if (state) checkBoxM5Start.ImageIndex = 1;
                    else checkBoxM5Start.ImageIndex = 0;
                }
                else
                {
                    checkBoxM5Start.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.manipulator, "manipulador sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private async void checkBoxM6Start_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                bool state = false;
                //send start/stop 
                state = (checkBoxM6Start.CheckState == CheckState.Checked) ? true : false;
                string keyValue = "pcM6StartStop";
                var sendResult = await ccService.Send(keyValue, state);
                if (sendResult.OpcResult)
                {
                    if (state) checkBoxM6Start.ImageIndex = 1;
                    else checkBoxM6Start.ImageIndex = 0;
                }
                else
                {
                    checkBoxM6Start.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.oven, "oven sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private async void checkBoxM1Pause_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                bool state = false;
                //send start/stop 
                state = (checkBoxM1Pause.CheckState == CheckState.Checked) ? true : false;
                string keyValue = "pcM1Pause";
                var sendResult = await ccService.Send(keyValue, state);
                if (sendResult.OpcResult)
                {
                    if (state) checkBoxM1Pause.ImageIndex = 4;
                    else checkBoxM1Pause.ImageIndex = 3;
                }
                else
                {
                    checkBoxM1Pause.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.trimmer, "refiladora sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }

        }
        private async void checkBoxM2Pause_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                bool state = false;
                //send start/stop 
                state = (checkBoxM2Pause.CheckState == CheckState.Checked) ? true : false;
                string keyValue = "pcM2Pause";
                var sendResult = await ccService.Send(keyValue, state);
                if (sendResult.OpcResult)
                {
                    if (state) checkBoxM2Pause.ImageIndex = 4;
                    else checkBoxM2Pause.ImageIndex = 3;
                }
                else
                {
                    checkBoxM2Pause.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padprintInt, "padprint int sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private async void checkBoxM4Pause_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                bool state = false;
                //send start/stop 
                state = (checkBoxM4Pause.CheckState == CheckState.Checked) ? true : false;
                string keyValue = "pcM4Pause";
                var sendResult = await ccService.Send(keyValue, state);
                if (sendResult.OpcResult)
                {
                    if (state) checkBoxM4Pause.ImageIndex = 4;
                    else checkBoxM4Pause.ImageIndex = 3;
                }
                else
                {
                    checkBoxM4Pause.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padLaser, "padlaser sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private async void checkBoxM3Pause_CheckStateChanged(object sender, EventArgs e)
        {
            if (M3InLine == "0") return;
            if (ccService.ClientIsConnected)
            {
                bool state = false;
                //send start/stop 
                state = (checkBoxM3Pause.CheckState == CheckState.Checked) ? true : false;
                string keyValue = "pcM3Pause";
                var sendResult = await ccService.Send(keyValue, state);
                if (sendResult.OpcResult)
                {
                    if (state) checkBoxM3Pause.ImageIndex = 4;
                    else checkBoxM3Pause.ImageIndex = 3;
                }
                else
                {
                    checkBoxM3Pause.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padprintExt, "padprint ext sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private async void checkBoxM5Pause_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                bool state = false;
                //send start/stop 
                state = (checkBoxM5Pause.CheckState == CheckState.Checked) ? true : false;
                string keyValue = "pcM5Pause";
                var sendResult = await ccService.Send(keyValue, state);
                if (sendResult.OpcResult)
                {
                    if (state) checkBoxM5Pause.ImageIndex = 4;
                    else checkBoxM5Pause.ImageIndex = 3;
                }
                else
                {
                    checkBoxM5Pause.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.manipulator, "manipulador sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private async void checkBoxM6Pause_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                bool state = false;
                //send start/stop 
                state = (checkBoxM6Pause.CheckState == CheckState.Checked) ? true : false;
                string keyValue = "pcM6Pause";
                var sendResult = await ccService.Send(keyValue, state);
                if (sendResult.OpcResult)
                {
                    if (state) checkBoxM6Pause.ImageIndex = 4;
                    else checkBoxM6Pause.ImageIndex = 3;
                }
                else
                {
                    checkBoxM6Pause.ImageIndex = 2;
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.oven, "oven sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private async void buttonM1Reset_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyValue = "pcM1Reset";
                var sendResult = await ccService.Send(keyValue, true);

                if ((sendResult == null) || (sendResult.OpcResult == false))
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.trimmer, "refiladora sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private async void buttonM2Reset_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyValue = "pcM2Reset";
                var sendResult = await ccService.Send(keyValue, true);

                if ((sendResult == null) || (sendResult.OpcResult == false))
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padprintInt, "padprint int sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }
        private async void buttonM4Reset_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyValue = "pcM4Reset";
                var sendResult = await ccService.Send(keyValue, true);

                if ((sendResult == null) || (sendResult.OpcResult == false))
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padLaser, "padlaser sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }

        private async void buttonM6Reset_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyValue = "pcM6Reset";
                var sendResult = await ccService.Send(keyValue, true);

                if ((sendResult == null) || (sendResult.OpcResult == false))
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.oven, "horno sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }

        private async void buttonM5Reset_Click(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyValue = "pcM5Reset";
                var sendResult = await ccService.Send(keyValue, true);

                if ((sendResult == null) || (sendResult.OpcResult == false))
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.manipulator, "manipulador sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }

        private async void buttonM3Reset_Click(object sender, EventArgs e)
        {
            if (M3InLine == "0") return;
            if (ccService.ClientIsConnected)
            {
                string keyValue = "pcM3Reset";
                var sendResult = await ccService.Send(keyValue, true);

                if ((sendResult == null) || (sendResult.OpcResult == false))
                {
                    AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.padprintExt, "padprint ext sin conexión");
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }

        private async void checkBoxStartStop_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                bool state = false;
                string keyValue = "";
                //send start/stop 
                state = (checkBoxStartStop.CheckState == CheckState.Checked) ? true : false;

                if (state)
                {
                    keyValue = "pcM1StartStop";
                    var readResult = await ccService.Send(keyValue, true);
                    keyValue = "pcM2StartStop";
                    readResult = await ccService.Send(keyValue, true);
                    keyValue = "pcM3StartStop";
                    readResult = await ccService.Send(keyValue, true);
                    keyValue = "pcM4StartStop";
                    readResult = await ccService.Send(keyValue, true);
                    keyValue = "pcM5StartStop";
                    readResult = await ccService.Send(keyValue, true);
                    keyValue = "pcM6StartStop";
                    readResult = await ccService.Send(keyValue, true);
                }
                else
                {
                    keyValue = "pcM1StartStop";
                    var readResult = await ccService.Send(keyValue, false);
                    keyValue = "pcM2StartStop";
                    readResult = await ccService.Send(keyValue, false);
                    keyValue = "pcM3StartStop";
                    readResult = await ccService.Send(keyValue, false);
                    keyValue = "pcM4StartStop";
                    readResult = await ccService.Send(keyValue, false);
                    keyValue = "pcM5StartStop";
                    readResult = await ccService.Send(keyValue, false);
                    keyValue = "pcM6StartStop";
                    readResult = await ccService.Send(keyValue, false);
                }

                if (state)
                {
                    checkBoxStartStop.ImageIndex = 1;
                    checkBoxStartStop.Text = "STOP";
                }
                else
                {
                    checkBoxStartStop.ImageIndex = 0;
                    checkBoxStartStop.Text = "START";
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }

        private async void checkBoxPause_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                bool state = false;
                string keyValue = "";
                //send start/stop 
                state = (checkBoxPause.CheckState == CheckState.Checked) ? true : false;

                if (state)
                {
                    keyValue = "pcM1Pause";
                    var readResult = await ccService.Send(keyValue, true);
                    keyValue = "pcM2Pause";
                    readResult = await ccService.Send(keyValue, true);
                    keyValue = "pcM3Pause";
                    readResult = await ccService.Send(keyValue, true);
                    keyValue = "pcM4Pause";
                    readResult = await ccService.Send(keyValue, true);
                    keyValue = "pcM5Pause";
                    readResult = await ccService.Send(keyValue, true);
                    keyValue = "pcM6Pause";
                    readResult = await ccService.Send(keyValue, true);

                    checkBoxPause.Image = imageListStartStop.Images[3];
                    checkBoxPause.Text = "IN PAUSE";
                }
                else
                {
                    keyValue = "pcM1Pause";
                    var readResult = await ccService.Send(keyValue, false);
                    keyValue = "pcM2Pause";
                    readResult = await ccService.Send(keyValue, false);
                    keyValue = "pcM3Pause";
                    readResult = await ccService.Send(keyValue, false);
                    keyValue = "pcM4Pause";
                    readResult = await ccService.Send(keyValue, false);
                    keyValue = "pcM5Pause";
                    readResult = await ccService.Send(keyValue, false);
                    keyValue = "pcM6Pause";
                    readResult = await ccService.Send(keyValue, false);
                    checkBoxPause.Image = imageListStartStop.Images[2];
                    checkBoxPause.Text = "PAUSE";
                }
            }
            else
            {
                AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "sistema sin conexión");
            }
        }




    }
}