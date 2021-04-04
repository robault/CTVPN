using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web;
using System.Collections;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace CTVPN_WebService
{
    [WebService(Namespace = "http://<web_service_ip_or_domain_name>/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class State : System.Web.Services.WebService
    {
        [WebMethod]
        public bool Response(string id)
        {
            bool connect = false;

            try
            {
                string test = DecryptString(id, "<encryped_name>");

                if (GuidTryParse(test) == true)
                {
                    SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["VPNClientsConnectionString"].ConnectionString.ToString());
                    cnx.Open();
                    SqlCommand cmd = new SqlCommand("SELECT connect FROM VPNClients WHERE guid= '" + id.ToString() + "'; UPDATE VPNClients SET lastcheckin = '" + DateTime.Now.ToString() + "' WHERE guid= '" + id.ToString() + "'", cnx);
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    while (rdr.Read())
                    {
                        if (rdr[0].ToString() == "True")
                            connect = true;
                    }
                    cmd.Dispose();
                    cnx.Close();
                    cnx.Dispose();
                }
                else
                {
                    //
                }
            }
            catch
            {
                //
            }
            return connect;
        }

        private bool GuidTryParse(string s)
        {
            if (s != null)
            {
                Regex format = new Regex(
                    "^[A-Fa-f0-9]{32}$|" +
                    "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
                    "^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$");
                Match match = format.Match(s);
                if (match.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
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
