using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Database_API_ver1
{
    public partial class frmMain : Form
    {

        public SqlConnection conn = null;
        public String DBName = "";
        public String ServerName = "";
        public String User = "";
        public String Password = "";
        String[] Command = new String[200];
        int nCommand = 0;
        String saveDec = "";
        SqlDataAdapter daKQ = null;
        DataTable dtKQ = null;

        public frmMain()
        {
            InitializeComponent();
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            DBName = txtDBName.Text.Trim();
            ServerName = txtServerName.Text.Trim();
            User = txtUser.Text.Trim();
            Password = txtPassword.Text.Trim();


            Connect myConnect = new Connect(ServerName, DBName, User, Password);
            conn = myConnect.getConnect();
            if (conn != null)
            {
                MessageBox.Show("Đã kết nối thành công");
                panel1.Show();
                panel2.Enabled = false;
                saveInfo();
            }
            else
            {
                MessageBox.Show("Kết nối thất bại");
                panel1.Hide();
                panel2.Enabled = true;
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            loadInfo();
            txtDBName.Text = "master";
            panel1.Hide();
        }


        bool isInWord(int index, String command)
        {
            for (char i = 'a'; i < 'z'; i++)
            {
                if (command[index - 1] == i || command[index] == i - 32)
                    return true;
            }

            if (index + 2 < command.Length)
            {
                for (char i = 'a'; i < 'z'; i++)
                {
                    if (command[index + 2] == i || command[index] == i - 32)
                        return true;
                }
            }
            return false;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {

            String sql = txtCommand.Text;
            toCommand(sql);
            for (int i = 0; i < nCommand; i++)
            {
                execute(Command[i]);
            }

            MessageBox.Show("Đã execute xong");

        }

        int takeGoPos(String command)
        {
            for (int i = 0; i < command.Length - 2; i++)
                if (command.Substring(i, 2).ToLower().Equals("go") && !isInWord(i, command))
                    return i;
            return -1;
        }

        void toCommand(String command)
        {
            Command = new String[200];
            nCommand = 0;
            while (command.Length > 0)
            {
                int i = takeGoPos(command);
                if (i != -1)
                {

                    Command[nCommand] = command.Substring(0, i);


                    command = command.Substring(i + 2, command.Length - Command[nCommand].Length - "go".Length);
                    nCommand++;
                }
                else
                {
                    Command[nCommand] = command;
                    nCommand++;
                    break;
                }
            }

        }



        void execute(String commandline)
        {
            String sql = commandline;

            try
            {
                daKQ =
                       new SqlDataAdapter(sql, conn);
                dtKQ = new DataTable();
                daKQ.Fill(dtKQ);
                dgvKQ.DataSource = dtKQ;

                return;
            }
            catch { }


            try
            {

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                return;
            }
            catch { }


        }

        private void button1_Click(object sender, EventArgs e)
        {

            conn.Close();
            conn = null;
            txtPassword.ResetText();
            txtDBName.Text = "master";

            dgvKQ.DataSource = null;
            txtCommand.ResetText();
            panel1.Hide();
            panel2.Enabled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
          
            saveFileDialog1.Title = "Save chema Files";

            saveFileDialog1.DefaultExt = "sql";
            saveFileDialog1.Filter = "SQL file (*.sql)|*.sql|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                saveDec = saveFileDialog1.FileName;
                String sql = txtCommand.Text;
                toCommand(sql);
                saveChema();
                MessageBox.Show("Đã lưu chema thành công");
            }


        }


        private void saveChema()
        {

            String filepath = saveDec;// đường dẫn của file muốn tạo
            FileStream fs = new FileStream(filepath, FileMode.Create);//Tạo file mới tên là test.txt            
            StreamWriter sWriter = new StreamWriter(fs, Encoding.UTF8);//fs là 1 FileStream 
            for(int i = 0; i<nCommand; i++)         
                sWriter.WriteLine(Command[i]);
            sWriter.Flush();
            fs.Close();
        }


        private void saveInfo()
        {
            String filepath = "config.inf";// đường dẫn của file muốn tạo
            FileStream fs = new FileStream(filepath, FileMode.Create);//Tạo file mới tên là test.txt            
            StreamWriter sWriter = new StreamWriter(fs, Encoding.UTF8);//fs là 1 FileStream 
            sWriter.WriteLine(ServerName);
            sWriter.WriteLine(User);
            sWriter.Flush();
            fs.Close();
        }


        private void loadInfo()
        {
            string[] lines = File.ReadAllLines("config.inf");
            int nline = 0;
            foreach (string s in lines)
            {
                if(nline ==0)
                {
                    txtServerName.Text = s;
                    nline++;
                }
                else
                {
                    if(nline==1)
                    {
                        txtUser.Text = s;
                        nline++;
                    }
                }
           
            }

        }

    }
}
