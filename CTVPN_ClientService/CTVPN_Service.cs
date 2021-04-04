using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Xml;

namespace CTVPN_ClientService
{
    public partial class CTVPN_Service : ServiceBase
    {
        public CTVPN_Service()
        {
            InitializeComponent();
        }

        private Timer serviceTimer;

        private int intervalMin = int.Parse(ConfigurationManager.AppSettings["checkinInterval"].ToString());

        string batchExecutionPath = ConfigurationManager.AppSettings["ExecutionPath"].ToString();

        private void serviceTimer_TimerCallback(object state)
        {
            //set the Timer timeout (run time)
            serviceTimer.Change(Timeout.Infinite, Timeout.Infinite);

            DoSomeWork();

            //set the service timer callback interval (hr, min, sec)
            serviceTimer.Change(new TimeSpan(0, intervalMin, 0), new TimeSpan(1, 0, 0));
        }

        protected override void OnStart(string[] args)
        {
            TimerCallback timerCallback = new TimerCallback(serviceTimer_TimerCallback);
            serviceTimer = new Timer(timerCallback);

            // (hr, min, sec)
            serviceTimer.Change(new TimeSpan(0, 0, 1), new TimeSpan(0, intervalMin, 0));
        }

        protected override void OnStop()
        {
            //When the service is told to stop, dispose of the timer object 
            //and stop the service then dispose of any extra resources

            serviceTimer.Dispose();
            //tried this but in debugging it created an exception when the 
            //service controller told the service to stop, so I went back to
            //using the debugging version of killing the service process.

            //this.Stop();
            //this.Dispose();

            //left over from debugging:
            //http://blogs.msdn.com/irenak/archive/2005/12/19/505429.aspx

            Process[] processes = Process.GetProcessesByName("CTVPN_Service.exe");
            foreach (Process process in processes)
            {
                process.Kill();
            }
            Process[] processes2 = Process.GetProcessesByName("CTVPN_Service.vhost.exe");
            foreach (Process process in processes2)
            {
                process.Kill();
            }
        }

        private void DoSomeWork()
        {
            try
            {
                ReplaceRasphone();

                WebServiceCheckIn();

                bool timeToConnect = ConnectOrDisconnect();

                if (timeToConnect == true)
                {
                    string command = CreateConnectCommand();
                    CreateBatchFile();
                    RunBatchFile(command);
                    DeleteBatchFile();
                }
                else
                {
                    string command = CreateDisconnectCommand();
                    CreateBatchFile();
                    RunBatchFile(command);
                    DeleteBatchFile();
                }
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry("VPN Connection process error: " + ex.Message,
                System.Diagnostics.EventLogEntryType.Error);
            }
        }

        protected bool ConnectOrDisconnect()
        {
            bool timeToRun = false;

            try
            {
                SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["VPNClientsConnectionString"].ConnectionString.ToString());
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 connect FROM VPNClients ORDER BY id ASC", cnx);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                        timeToRun = bool.Parse(rdr[0].ToString());
                }
                rdr.Close(); rdr.Dispose();
                cmd.Dispose();
                cnx.Close(); cnx.Dispose();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Could not get connection state record from the database: " + ex.ToString());
            }

            return timeToRun;
        }

        private string CreateConnectCommand()
        {
            string batchString = "";

            try
            {
                string name = "";
                string user = "";
                string password = "";

                SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["VPNClientsConnectionString"].ConnectionString.ToString());
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 cnxName, cnxUser, cnxPassword FROM VPNClients", cnx);
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
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Cannot retrieve connection configuration values from the database: " + ex.Message,
                System.Diagnostics.EventLogEntryType.Error);
            }

            return batchString;
        }

        private string CreateDisconnectCommand()
        {
            string batchString = "";
            string name = "";

            try
            {
                SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["VPNClientsConnectionString"].ConnectionString.ToString());
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 cnxName FROM VPNClients", cnx);
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
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Cannot retrieve disconnection configuration values from the database: " + ex.Message,
                System.Diagnostics.EventLogEntryType.Error);
            }

            return batchString;
        }

        private void CreateBatchFile()
        {
            try
            {
                StreamWriter writer = new StreamWriter(batchExecutionPath + "temp.bat");
                writer.WriteLine("");
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry("Could not create the batch file in directory: " + batchExecutionPath.ToString() + " ::: " + ex.Message,
                System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private void RunBatchFile(string command)
        {
            try
            {
                StreamWriter writer = new StreamWriter(batchExecutionPath + "temp.bat");
                writer.WriteLine(command);
                writer.Flush();
                writer.Close();
                writer.Dispose();

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = batchExecutionPath.ToString() + "temp.bat";
                startInfo.UseShellExecute = true;
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = Process.Start(startInfo);
                p.WaitForExit();
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry("Could not run the batch file in directory: " + batchExecutionPath.ToString() + " ::: " + ex.Message,
                System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private void DeleteBatchFile()
        {
            try
            {
                File.Delete(batchExecutionPath + "temp.bat");
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry("Could not delete the batch file in directory: " + batchExecutionPath.ToString() + " ::: " + ex.Message,
                System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private void WebServiceCheckIn()
        {
            try
            {
                string id = "";
                string cnState = "";
                int cnxState = 0; //false

                SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["VPNClientsConnectionString"].ConnectionString.ToString());
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 guid FROM VPNClients ORDER BY id ASC", cnx);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                        id = rdr[0].ToString();
                }
                rdr.Close(); rdr.Dispose();
                cmd.Dispose();

                CTVPN_ClientService.WebReference.State cns = new CTVPN_ClientService.WebReference.State();
                cnState = cns.Response(id).ToString();

                if (cnState == "True")
                    cnxState = 1;

                SqlCommand cmd2 = new SqlCommand("UPDATE VPNClients SET connect = '" + cnxState + "'", cnx);
                cmd2.ExecuteNonQuery();
                cmd2.Dispose();

                cnx.Close(); cnx.Dispose();
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry("Cannot get state from web service: " + ex.Message,
                System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private void ReplaceRasphone()
        {
            string source = "C:\\Program Files\\Company Name\\CTVPN Client\\Rasphone\\rasphone.pbk";
            string destination = "C:\\Documents and Settings\\All Users\\Application Data\\Microsoft\\Network\\Connections\\Pbk\\rasphone.pbk";

            File.Copy(source, destination, true);
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
