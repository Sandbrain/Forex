using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Forex.Forms
{
    public partial class SessionForm : Form
    {
        public SessionForm()
        {
            InitializeComponent();
            cbAccountType.SelectedItem = cbAccountType.Items[0];
            buttonConnect.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string URL { get; set; }
        public string AccountType { get; set; }
        public bool Cancel = false;        

        private void buttonConnect_Click(object sender, EventArgs e)
        {              
            this.UserName = textBoxUserName.Text;
            this.Password = textBoxPassword.Text;
            this.URL = textBoxURL.Text;
            this.AccountType = cbAccountType.SelectedItem.ToString();             
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Cancel = true;
            this.Close();
        }       
    }
}
