using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MsSqlQueryExecutor
{
    public partial class MainForm : Form
    {
        private List<ServerEntity> servers = new List<ServerEntity>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void addNewServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectionStringDialogBox csdb = new ConnectionStringDialogBox();

            if (csdb.ShowDialog() == DialogResult.OK)
            {
                ServerEntity server = new ServerEntity(csdb.ConnectionString);

                TreeNode serverNode = tree.Nodes.Add(server.ServerName);

                foreach (string dbname in server.DatabaseNames)
                {
                    serverNode.Nodes.Add(dbname);
                }

                serverNode.ExpandAll();

                servers.Add(server);
            }
        }

        private void clearServerListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tree.Nodes.Clear();
            servers.Clear();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textResult.Clear();

            if (tree.Nodes.Count > 0)
            {
                for (int i = 0; i < tree.Nodes.Count; i++)
                {
                    TreeNode serverNode = tree.Nodes[i];

                    if (serverNode.Nodes.Count > 0)
                    {
                        for (int j = 0; j < serverNode.Nodes.Count; j++)
                        {
                            TreeNode databaseNode = serverNode.Nodes[j];

                            if (databaseNode.Checked || serverNode.Checked)
                            {
                                ServerEntity server = servers[i];

                                textResult.Text += server.ExecuteText(databaseNode.Text, script.Text) + "\r\n";
                            }
                        }
                    }
                }
            }
        }
    }
}
