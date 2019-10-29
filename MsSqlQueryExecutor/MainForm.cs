using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Xml;

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

                                textResult.Text += string.Format("[{0}] Database:{1}\r\n{2}\r\n", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss.ffff"), databaseNode.Text, server.ExecuteText(databaseNode.Text, script.Text));
                            }
                        }
                    }
                }
            }
        }

        private void tableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();

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

                                ds.Merge(server.ExecuteTable(databaseNode.Text, script.Text));
                            }
                        }
                    }
                }
            }

            dataGridView1.DataSource = ds.Tables[0];
        }
    }
}
