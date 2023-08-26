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

                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM2ManualQuote.Value.ToString()));

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
            //if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM1Inclusion";
                chkValue = (checkBoxM2WorkingBelt.CheckState == CheckState.Checked) ? true : false;

                //var sendResult = await ccService.Send(keyToSend, chkValue);

                //if (sendResult.OpcResult)
                {
                    checkBoxM2WorkingBelt.ImageIndex = (chkValue) ? 0 : 1;
                }
                //else
                //{
                //    checkBoxM2WorkingBelt.ImageIndex = 2;
                    //AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.trimmer, "trimmer offline");
                //}
            }
            //else
            //{
                //AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "system offline");
            //}
        }

        private void checkBoxM2ExitBelt_CheckStateChanged(object sender, EventArgs e)
        {
            //if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM1Inclusion";
                chkValue = (checkBoxM2ExitBelt.CheckState == CheckState.Checked) ? true : false;

                //var sendResult = await ccService.Send(keyToSend, chkValue);

                //if (sendResult.OpcResult)
                {
                    checkBoxM2ExitBelt.ImageIndex = (chkValue) ? 2 : 3;
                }
                //else
                //{
                //    checkBoxM2WorkingBelt.ImageIndex = 2;
                //AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.trimmer, "trimmer offline");
                //}
            }
            //else
            //{
            //AddMessageToDataGridOnTop(DateTime.Now, Priority.critical, Machine.line, "system offline");
            //}
        }
    }
}