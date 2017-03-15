using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySQLLite
{
    public partial class Form1 : Form
    {
        static String DB_NAME = "tablename";
        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Clear();
            ArrayList tables = MySQL.GetTables(DB_NAME);
            foreach (String name in tables)
            {
                comboBox1.Items.Add(name);
            }

            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.RowTemplate.Height = 50;

            toolStripStatusLabel1.Text = "";

            String SQL = "SELECT DISTINCT a.product_sn, a.*, b.* FROM StationTestReport AS a, DevFinalTestReport AS b WHERE a.device_pcb_sn LIKE b.device_pcb_sn";
            textBox1.Text = SQL;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            int w = this.Width - 25;
            button1.Left = w - button1.Width + 5;
            comboBox1.Width = w - button1.Width - 10;

            dataGridView1.Width = w;
            dataGridView1.Height = this.Height - dataGridView1.Top - statusStrip1.Height - 45;

            if (textBox1.Visible)
            {
                button2.Left = button1.Left;
                textBox1.Width = w - button2.Width-5;
                textBox1.Top = dataGridView1.Top + dataGridView1.Height - 45;
                button2.Top = textBox1.Top;
                dataGridView1.Height -= (45 + 5);
            }
      
        }

        private void freshData(String tableName, String sql)
        {
            toolStripStatusLabel1.Text = sql;

            MySQLData data = MySQL.GetData(DB_NAME, tableName, sql);

            if (data.E.Message.Length > 0)
            {
                toolStripStatusLabel1.Text = data.E.Message;
                return;
            }
            dataGridView1.ColumnCount = data.Columns.Count;
            for (int i = 0; i < data.Columns.Count; i++)
            {
                dataGridView1.Columns[i].Name = (String)data.Columns[i];
            }

            dataGridView1.Rows.Clear();
            foreach (Dictionary<String, String> dict in data.Datas)
            {
                ArrayList itemList = new ArrayList();

                foreach (String title in data.Columns)
                {
                    itemList.Add(dict[title]);
                }
                dataGridView1.Rows.Add(itemList.ToArray());
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.HeaderCell.Value = (row.Index + 1).ToString();
            }

          
            /*
            DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
            buttonColumn.Width = 200;
            buttonColumn.HeaderText = "按o鈕s";
            buttonColumn.Name = "Status Request";
            buttonColumn.Text = "Request Status";
            buttonColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(buttonColumn);
            // Add a CellClick handler to handle clicks in the button column.
            //dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
            */

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //{ "debug":1, "mode":"1", "sql":"SELECT * FROM devagingtestreport", "table":"devagingtestreport"}
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("請先選擇一個資料表", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            String tableName = comboBox1.SelectedItem.ToString();
            String sql = "SELECT * FROM " + tableName;
            
            freshData(tableName, sql);

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            comboBox1_SelectedIndexChanged(comboBox1, null);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

      

        private void tablesAsCSVFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("請先選擇一個資料表", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CSV File|*.csv";
            saveFileDialog1.Title = "Save an CSV File";
            saveFileDialog1.FileName = comboBox1.SelectedItem.ToString() +" "+DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            saveFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName == "")
            {               
                return;
            }


            string delimiter = ",";
            string fullFilename = saveFileDialog1.FileName;
            StreamWriter csvStreamWriter = new StreamWriter(fullFilename, false, System.Text.Encoding.UTF8);
            //output header data
            string strHeader = "";
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                strHeader += dataGridView1.Columns[i].HeaderText + delimiter;
            }
            csvStreamWriter.WriteLine(strHeader);

            //output rows data
            for (int j = 0; j < dataGridView1.Rows.Count; j++)
            {
                string strRowValue = "";

                for (int k = 0; k < dataGridView1.Columns.Count; k++)
                {
                    strRowValue += dataGridView1.Rows[j].Cells[k].Value + delimiter;

                }
                csvStreamWriter.WriteLine(strRowValue);
            }

            csvStreamWriter.Close();

            MessageBox.Show("儲存完畢", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SQLToolStripMenuItem.Checked = !SQLToolStripMenuItem.Checked;
            textBox1.Visible = SQLToolStripMenuItem.Checked;
            button2.Visible = textBox1.Visible;
            Form1_SizeChanged(this, null);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            freshData("StationTestReport", textBox1.Text);
        }
    }
}
