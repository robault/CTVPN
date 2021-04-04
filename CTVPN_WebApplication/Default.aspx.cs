using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace CTVPN_WebApplication
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void createClientButton_Click(object sender, EventArgs e)
        {
            string guid = EncryptString(Guid.NewGuid().ToString(), "<encryped_name>");
            string name = clientNameTextBox.Text.ToString();
            bool connect = false;
            DateTime lastcheckin = DateTime.Parse("2000/01/01 12:00:00 AM");
            string user = EncryptString(userNameTextBox.Text.ToString(), "<encryped_name>");
            string password = EncryptString(passwordTextBox.Text.ToString(), "<encryped_name>");
            string connectionName = EncryptString("ctvpn", "<encryped_name>");

            try
            {
                SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["VPNClientsConnectionString"].ConnectionString.ToString());
                cnx.Open();
                SqlCommand cmd = new SqlCommand("spVPNClients_CreateClient", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@guid", guid));
                cmd.Parameters.Add(new SqlParameter("@name", name));
                cmd.Parameters.Add(new SqlParameter("@connect", connect));
                cmd.Parameters.Add(new SqlParameter("@lastcheckin", lastcheckin));
                cmd.Parameters.Add(new SqlParameter("@user", user));
                cmd.Parameters.Add(new SqlParameter("@password", password));
                cmd.Parameters.Add(new SqlParameter("@cnxName", connectionName));
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cnx.Close();
                cnx.Dispose();

                string createSql = "VALUES (1, '" + guid + "', '" + name + "', 0, '2000101', 'qitfxsjJzms6VITjwkWcKg==', '" + user + "', '" + password + "')";
                sqlTextBox.Text = createSql.ToString();
            }
            catch (Exception ex)
            {
                errorLabel.Text = "<h3>The client could not be created:</h3>" + "\n" + ex.ToString() + "\n" + "\n" + "<h4>Try closing this page, restarting the SQL service, then try again.</h4>";
            }

            clientNameTextBox.Text = "";
            userNameTextBox.Text = "";
            passwordTextBox.Text = "";
            GridView1.DataBind();
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
