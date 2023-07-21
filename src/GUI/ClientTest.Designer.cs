namespace GUI
{
    partial class ClientTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.connectBtn = new System.Windows.Forms.Button();
            this.hostTxtbox = new System.Windows.Forms.TextBox();
            this.portTxtbox = new System.Windows.Forms.TextBox();
            this.userTxtbox = new System.Windows.Forms.TextBox();
            this.pwdTxtbox = new System.Windows.Forms.TextBox();
            this.schemeTxtbox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.AddressLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxLogTxtbox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.jobAltoBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(193, 44);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(75, 31);
            this.connectBtn.TabIndex = 0;
            this.connectBtn.Text = "Connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // hostTxtbox
            // 
            this.hostTxtbox.Location = new System.Drawing.Point(82, 16);
            this.hostTxtbox.Name = "hostTxtbox";
            this.hostTxtbox.Size = new System.Drawing.Size(100, 20);
            this.hostTxtbox.TabIndex = 1;
            // 
            // portTxtbox
            // 
            this.portTxtbox.Location = new System.Drawing.Point(82, 69);
            this.portTxtbox.Name = "portTxtbox";
            this.portTxtbox.Size = new System.Drawing.Size(100, 20);
            this.portTxtbox.TabIndex = 2;
            // 
            // userTxtbox
            // 
            this.userTxtbox.Location = new System.Drawing.Point(82, 135);
            this.userTxtbox.Name = "userTxtbox";
            this.userTxtbox.Size = new System.Drawing.Size(100, 20);
            this.userTxtbox.TabIndex = 3;
            // 
            // pwdTxtbox
            // 
            this.pwdTxtbox.Location = new System.Drawing.Point(82, 161);
            this.pwdTxtbox.Name = "pwdTxtbox";
            this.pwdTxtbox.Size = new System.Drawing.Size(100, 20);
            this.pwdTxtbox.TabIndex = 4;
            // 
            // schemeTxtbox
            // 
            this.schemeTxtbox.Location = new System.Drawing.Point(82, 42);
            this.schemeTxtbox.Name = "schemeTxtbox";
            this.schemeTxtbox.Size = new System.Drawing.Size(100, 20);
            this.schemeTxtbox.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(82, 95);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Make URI";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // AddressLabel
            // 
            this.AddressLabel.AutoSize = true;
            this.AddressLabel.Location = new System.Drawing.Point(13, 196);
            this.AddressLabel.Name = "AddressLabel";
            this.AddressLabel.Size = new System.Drawing.Size(38, 13);
            this.AddressLabel.TabIndex = 7;
            this.AddressLabel.Text = "http://";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "host";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "scheme";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "user";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "password";
            // 
            // textBoxLogTxtbox
            // 
            this.textBoxLogTxtbox.Location = new System.Drawing.Point(448, 16);
            this.textBoxLogTxtbox.Multiline = true;
            this.textBoxLogTxtbox.Name = "textBoxLogTxtbox";
            this.textBoxLogTxtbox.Size = new System.Drawing.Size(354, 593);
            this.textBoxLogTxtbox.TabIndex = 13;
            this.textBoxLogTxtbox.TextChanged += new System.EventHandler(this.textBoxLogTxtbox_TextChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(13, 226);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(104, 34);
            this.button2.TabIndex = 14;
            this.button2.Text = "Read Folder Name";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // jobAltoBtn
            // 
            this.jobAltoBtn.Location = new System.Drawing.Point(16, 278);
            this.jobAltoBtn.Name = "jobAltoBtn";
            this.jobAltoBtn.Size = new System.Drawing.Size(75, 23);
            this.jobAltoBtn.TabIndex = 15;
            this.jobAltoBtn.Text = "Write Jog Alto";
            this.jobAltoBtn.UseVisualStyleBackColor = true;
            this.jobAltoBtn.Click += new System.EventHandler(this.jobAltoBtn_Click);
            // 
            // ClientTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 621);
            this.Controls.Add(this.jobAltoBtn);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBoxLogTxtbox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AddressLabel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.schemeTxtbox);
            this.Controls.Add(this.pwdTxtbox);
            this.Controls.Add(this.userTxtbox);
            this.Controls.Add(this.portTxtbox);
            this.Controls.Add(this.hostTxtbox);
            this.Controls.Add(this.connectBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ClientTest";
            this.Text = "ClientTest";
            this.Load += new System.EventHandler(this.ClientTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.TextBox hostTxtbox;
        private System.Windows.Forms.TextBox portTxtbox;
        private System.Windows.Forms.TextBox userTxtbox;
        private System.Windows.Forms.TextBox pwdTxtbox;
        private System.Windows.Forms.TextBox schemeTxtbox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label AddressLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxLogTxtbox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button jobAltoBtn;
    }
}