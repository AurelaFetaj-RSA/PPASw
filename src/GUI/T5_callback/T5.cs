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
        private async void buttonM5TranslatorFwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5TranslatorFwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5TranslatorBwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5TranslatorBwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5ClampFwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5ClampFwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5ClampBwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5ClampBwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5ClampOpening_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5ClampOpening";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5ClampClosing_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5ClampClosing";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5CWRotation_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5CWRotation";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5CCWRotation_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5CCWRotation";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5NoRotation_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5NoRotation";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5V1ExtFwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5V1ExtFwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5V1ExtBwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5V1ExtBwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5V2ExtFwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5V2ExtFwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void buttonM5V2ExtBwd_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM5V2ExtBwd";
            var sendResult = await ccService.Send(keyToSend, true);
            if (sendResult.OpcResult)
            {

            }
            else
            {

            }
        }

        private async void checkBoxM5TranslationBelt_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM5StartStopTranslationBelt";
                chkValue = (checkBoxM5TranslationBelt.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM5TranslationBelt.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM5TranslationBelt.ImageIndex = 2;
                }
            }
            else
            {

            }
        }

    

        private async void checkBoxM5ExitBelt3_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM5StartStopExitBelt3";
                chkValue = (checkBoxM5ExitBelt3.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM5ExitBelt3.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM5ExitBelt3.ImageIndex = 2;
                }
            }
            else
            {

            }
        }

        private async void checkBoxM5ExitBelt1_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcM5StartStopExitBelt1";
                chkValue = (checkBoxM5ExitBelt1.CheckState == CheckState.Checked) ? true : false;

                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxM5ExitBelt1.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxM5ExitBelt1.ImageIndex = 2;
                }
            }
            else
            {

            }
        }    

        //private async void checkBoxM5ExitBelt2_CheckStateChanged(object sender, EventArgs e)
        //{
        //    if (ccService.ClientIsConnected)
        //    {
        //        string keyToSend = null;
        //        bool chkValue = false;

        //        keyToSend = "pcM5StartStopExitBelt2";
        //        chkValue = (checkBoxM5ExitBelt2.CheckState == CheckState.Checked) ? true : false;

        //        var sendResult = await ccService.Send(keyToSend, chkValue);

        //        if (sendResult.OpcResult)
        //        {
        //            checkBoxM5ExitBelt2.ImageIndex = (chkValue) ? 0 : 1;
        //        }
        //        else
        //        {
        //            checkBoxM5ExitBelt2.ImageIndex = 2;
        //        }
        //    }
        //    else
        //    {

        //    }
        //}
    }
}