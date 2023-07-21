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
    public partial class FormMain : Form
    {
        public void AddMessageToDT(string command, string value, DataGridView dtLst)
        {
            int maxGLMessages = 200;
            //check messages counter: eventually clear text buffer
            if (dtLst.Rows.Count >= maxGLMessages)
            {
                dtLst.Rows.Clear();
                dtLst.Refresh();
            }            

            //command
            Label lbCommand = new Label();
            lbCommand.Text = command;

            Label lbValue = new Label();
            lbValue.Text = value;

            //datetime
            Label lbTime = new Label();
            lbTime.Text = DateTime.Now.ToString();

            dtLst.Rows.Insert(0, lbTime.Text, lbCommand.Text, lbValue.Text);            
            dtLst.Rows[0].Height = 24;

            dtLst.Refresh();
        }
    }
}