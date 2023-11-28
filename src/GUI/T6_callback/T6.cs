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

namespace GUI
{
    public partial class FormApp : Form
    {
        private async void buttonM6LFLampsON_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM6LFLampsON";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM6LFLampsOFF_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM6LFLampsOFF";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM6RGLampsON_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM6RGLampsON";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM6RGLampsOFF_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM6RGLampsOFF";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void numericUpDownM6OnPercentage_ValueChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcM6ONPercentage";

                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownM6OnPercentage.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
        }

        private async void checkBoxM6Aspiration_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM6StartStopSuction";
                chkValue = (checkBoxM6Aspiration.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM6Aspiration.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM6Aspiration.ImageIndex = 2;
                }
            }
            else
            {

            }
        }
    }
}