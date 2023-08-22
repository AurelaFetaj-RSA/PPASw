using RSACommon.Configuration;
using RSACommon.Points;
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
        //private async void checkBoxM1Inclusion_CheckStateChanged(object sender, EventArgs e)
        //{
        //    string keyToSend = null;
        //    bool chkValue = false;

        //    checkBoxM1Inclusion.Text = ""; //todo: to remove once initialized
        //    keyToSend = "pcM1Inclusion";
        //    chkValue = (checkBoxM1Inclusion.CheckState == CheckState.Checked) ? true : false;

        //    var sendResult = await ccService.Send(keyToSend, chkValue);
        //    if (sendResult.OpcResult)
        //    {
        //        checkBoxM1Inclusion.BackgroundImage = (chkValue) ? imageListMIncEsc.Images[0] : imageListMIncEsc.Images[1];
        //    }
        //    else checkBoxM1Inclusion.BackgroundImage = imageListMIncEsc.Images[2];
        //}

        //private async void checkBoxM2Inclusion_CheckStateChanged(object sender, EventArgs e)
        //{
        //    string keyToSend = null;
        //    bool chkValue = false;

        //    checkBoxM2Inclusion.Text = ""; //todo: to remove once initialized
        //    keyToSend = "pcM2Inclusion";
        //    chkValue = (checkBoxM2Inclusion.CheckState == CheckState.Checked) ? true : false;

        //    var sendResult = await ccService.Send(keyToSend, chkValue);
        //    if (sendResult.OpcResult)
        //    {
        //        checkBoxM2Inclusion.BackgroundImage = (chkValue) ? imageListMIncEsc.Images[0] : imageListMIncEsc.Images[1];
        //    }
        //    else checkBoxM2Inclusion.BackgroundImage = imageListMIncEsc.Images[2];
        //}

        //private async void checkBoxM3Inclusion_CheckStateChanged(object sender, EventArgs e)
        //{
        //    string keyToSend = null;
        //    bool chkValue = false;

        //    checkBoxM3Inclusion.Text = ""; //todo: to remove once initialized
        //    keyToSend = "pcM3Inclusion";
        //    chkValue = (checkBoxM3Inclusion.CheckState == CheckState.Checked) ? true : false;

        //    var sendResult = await ccService.Send(keyToSend, chkValue);
        //    if (sendResult.OpcResult)
        //    {
        //        checkBoxM3Inclusion.BackgroundImage = (chkValue) ? imageListMIncEsc.Images[0] : imageListMIncEsc.Images[1];
        //    }
        //    else checkBoxM3Inclusion.BackgroundImage = imageListMIncEsc.Images[2];
        //}

        //private async void checkBoxM4Inclusion_CheckStateChanged(object sender, EventArgs e)
        //{
        //    string keyToSend = null;
        //    bool chkValue = false;

        //    checkBoxM4Inclusion.Text = ""; //todo: to remove once initialized
        //    keyToSend = "pcM4Inclusion";
        //    chkValue = (checkBoxM4Inclusion.CheckState == CheckState.Checked) ? true : false;

        //    var sendResult = await ccService.Send(keyToSend, chkValue);
        //    if (sendResult.OpcResult)
        //    {
        //        checkBoxM4Inclusion.BackgroundImage = (chkValue) ? imageListMIncEsc.Images[0] : imageListMIncEsc.Images[1];
        //    }
        //    else checkBoxM4Inclusion.BackgroundImage = imageListMIncEsc.Images[2];
        //}

        //private async void checkBoxM5Inclusion_CheckStateChanged(object sender, EventArgs e)
        //{
        //    string keyToSend = null;
        //    bool chkValue = false;

        //    checkBoxM5Inclusion.Text = ""; //todo: to remove once initialized
        //    keyToSend = "pcM5Inclusion";
        //    chkValue = (checkBoxM5Inclusion.CheckState == CheckState.Checked) ? true : false;

        //    var sendResult = await ccService.Send(keyToSend, chkValue);
        //    if (sendResult.OpcResult)
        //    {
        //        checkBoxM5Inclusion.BackgroundImage = (chkValue) ? imageListMIncEsc.Images[0] : imageListMIncEsc.Images[1];
        //    }
        //    else checkBoxM5Inclusion.BackgroundImage = imageListMIncEsc.Images[2];
        //}

        //private async void checkBoxM6Inclusion_CheckStateChanged(object sender, EventArgs e)
        //{
        //    string keyToSend = null;
        //    bool chkValue = false;

        //    checkBoxM6Inclusion.Text = ""; //todo: to remove once initialized
        //    keyToSend = "pcM6Inclusion";
        //    chkValue = (checkBoxM6Inclusion.CheckState == CheckState.Checked) ? true : false;

        //    var sendResult = await ccService.Send(keyToSend, chkValue);
        //    if (sendResult.OpcResult)
        //    {
        //        checkBoxM6Inclusion.BackgroundImage = (chkValue) ? imageListMIncEsc.Images[0] : imageListMIncEsc.Images[1];
        //    }
        //    else checkBoxM6Inclusion.BackgroundImage = imageListMIncEsc.Images[2];
        //}


        //private async void comboBoxM2PrgName_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    //todo: send recipe (todo: waiting mysql integration)
        //    string keyValue = "pcM2Param1";
        //    var sendResult = await ccService.Send(keyValue, short.Parse(textBoxM2Test.Text));
        //    if (sendResult.OpcResult)
        //    {
        //        labelM2Param1Value.Text = textBoxM2Test.Text;
        //    }
        //    else
        //    {
        //        //todo manage log/user error
        //    }

        //    //send quote, speed
        //    var dummyS = myCore.FindPerType(typeof(ReadProgramsService));

        //    if (dummyS != null && dummyS.Count > 0 && dummyS[0] is ReadProgramsService progRS)
        //    {
        //        ReadProgramsConfiguration config = progRS.Configuration as ReadProgramsConfiguration;
        //        ConcretePointsContainer<PointAxis> objPoints = new ConcretePointsContainer<PointAxis>("xxxx");
        //        objPoints = (ConcretePointsContainer<PointAxis>)await progRS.LoadProgramByNameAsync<PointAxis>(config.ProgramsPath[1] + "\\" + comboBoxM2PrgName.Text + config.Extensions[0]);
        //        if (objPoints != null)
        //        {
        //            List<string> keys = new List<string>()
        //            {
        //                "pcM2AutoQuote",
        //                "pcM2AutoSpeed"
        //            };

        //            List<object> values = new List<object>()
        //            {
        //                new short[] {0, (short)objPoints.Points[0].Q1, (short)objPoints.Points[0].Q2, (short)objPoints.Points[0].Q3, (short)objPoints.Points[0].Q4},
        //                new short[] {0, (short)objPoints.Points[0].V1, (short)objPoints.Points[0].V2, (short)objPoints.Points[0].V3, (short)objPoints.Points[0].V4}
        //            };


        //            var readResult = await ccService.Send(keys, values);
        //        }
        //    }
        //}
    }
}