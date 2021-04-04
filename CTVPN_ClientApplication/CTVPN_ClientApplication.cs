using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml;

namespace CTVPN_ClientApplication
{
    public partial class CTVPN_ClientApplication : Form
    {
        public CTVPN_ClientApplication()
        {
            InitializeComponent();
            messagelabel.Text = "";
            GetFormFieldDataFromDatabase();
        }

        string batchExecutionPath = ConfigurationManager.AppSettings["ExecutionPath"].ToString();

        private void GetFormFieldDataFromDatabase()
        {
            //string batchString = "";

            SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["VPNClientsConnectionString"].ConnectionString.ToString());
            cnx.Open();
            SqlCommand cmd = new SqlCommand("SELECT TOP (1) cnxName, cnxUser, cnxPassword FROM VPNclients", cnx);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                if (!rdr.IsDBNull(0))
                {
                    connectionNameTextBox.Text = DecryptString(rdr[0].ToString(), "<encryped_name>");
                    userNameTextBox.Text = DecryptString(rdr[1].ToString(), "<encryped_name>");
                    passwordTextBox.Text = DecryptString(rdr[2].ToString(), "<encryped_name>");
                }
            }

            rdr.Close(); rdr.Dispose();
            cmd.Dispose();
            cnx.Close(); cnx.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string name = EncryptString(connectionNameTextBox.Text.ToString(), "<encryped_name>");
            string user = EncryptString(userNameTextBox.Text.ToString(), "<encryped_name>");
            string password = EncryptString(passwordTextBox.Text.ToString(), "<encryped_name>");

            SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["VPNClientsConnectionString"].ConnectionString.ToString());
            cnx.Open();
            SqlCommand cmd = new SqlCommand("UPDATE VPNclients SET cnxName = @cnxName, cnxUser = @cnxUser, cnxPassword = @cnxPassword", cnx);
            cmd.Parameters.Add(new SqlParameter("@cnxName", name));
            cmd.Parameters.Add(new SqlParameter("@cnxUser", user));
            cmd.Parameters.Add(new SqlParameter("@cnxPassword", password));
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cnx.Close(); cnx.Dispose();

            messagelabel.Text = "Configuration saved.";
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //item = Open
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //item = Exit
            Application.Exit();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            //create connect command string
            string name = "";
            string user = "";
            string password = "";
            string batchString = "";

            SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["VPNClientsConnectionString"].ConnectionString.ToString());
            cnx.Open();
            SqlCommand cmd = new SqlCommand("SELECT TOP (1) cnxName, cnxUser, cnxPassword FROM VPNclients", cnx);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                if (!rdr.IsDBNull(0))
                {
                    name = DecryptString(rdr[0].ToString(), "<encryped_name>");
                    user = DecryptString(rdr[1].ToString(), "<encryped_name>");
                    password = DecryptString(rdr[2].ToString(), "<encryped_name>");
                }
            }

            batchString = "rasdial " + "\"" + name + "\"" + " " + user + " " + password;

            rdr.Close(); rdr.Dispose();
            cmd.Dispose();
            cnx.Close(); cnx.Dispose();

            //create the batch file and write the string to it
            StreamWriter writer = new StreamWriter(batchExecutionPath + "temp.bat");
            writer.WriteLine("@ECHO OFF");
            writer.WriteLine(batchString);
            writer.Flush();
            writer.Close();
            writer.Dispose();

            //run it
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = batchExecutionPath.ToString() + "temp.bat";
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process p = Process.Start(startInfo);
            p.WaitForExit();

            //delete it
            File.Delete(batchExecutionPath + "temp.bat");
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            //create the disconnect command string
            string batchString = "";
            string name = "";

            SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["VPNClientsConnectionString"].ConnectionString.ToString());
            cnx.Open();
            SqlCommand cmd = new SqlCommand("SELECT TOP (1) cnxName FROM VPNclients", cnx);
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                if (!rdr.IsDBNull(0))
                {
                    name = DecryptString(rdr[0].ToString(), "<encryped_name>");
                }
            }

            batchString = "rasdial " + "\"" + name + "\" /DISCONNECT";

            rdr.Close(); rdr.Dispose();
            cmd.Dispose();
            cnx.Close(); cnx.Dispose();

            //create the batch file and write the string to it
            StreamWriter writer = new StreamWriter(batchExecutionPath + "temp.bat");
            writer.WriteLine("@ECHO OFF");
            writer.WriteLine(batchString);
            writer.Flush();
            writer.Close();
            writer.Dispose();

            //run it
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = batchExecutionPath.ToString() + "temp.bat";
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process p = Process.Start(startInfo);
            p.WaitForExit();

            //delete it
            File.Delete(batchExecutionPath + "temp.bat");
        }

        private string EncryptString(string InputText, string Password)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();
            RijndaelCipher.Padding = PaddingMode.PKCS7;
            byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(InputText);
            byte[] Salt = Encoding.ASCII.GetBytes("<encryption_salt>");
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(PlainText, 0, PlainText.Length);
            cryptoStream.FlushFinalBlock();
            byte[] CipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string EncryptedData = Convert.ToBase64String(CipherBytes);
            return EncryptedData;
        }

        private string DecryptString(string InputText, string Password)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();
            RijndaelCipher.Padding = PaddingMode.PKCS7;
            byte[] EncryptedData = Convert.FromBase64String(InputText);
            byte[] Salt = Encoding.ASCII.GetBytes("<encryption_salt>");
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream(EncryptedData);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
            byte[] PlainText = new byte[EncryptedData.Length];
            int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
            memoryStream.Close();
            cryptoStream.Close();
            string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
            return DecryptedData;
        }
    }
}