using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MsSqlQueryExecutor
{
    public partial class ConnectionStringDialogBox : Form
    {
        public ConnectionStringDialogBox()
        {
            InitializeComponent();
        }

        public string ConnectionString
        {
            get
            {
                return connectionString.Text;
            }
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
