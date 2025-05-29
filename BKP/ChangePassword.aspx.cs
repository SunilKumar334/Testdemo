using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using UMMBusinessLayer;
using UMM_CHILDCLASSES;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using SubSonic;

public partial class UMM_ChangePassword : System.Web.UI.Page
{

    UserAuthorization UAobj = new UserAuthorization();
    crypto crypt = new crypto();
    UMM_Child_UserSmtp_Dtls obj = new UMM_Child_UserSmtp_Dtls();

    protected void Page_Load(object sender, EventArgs e)
    {

        this.Form.DefaultButton = this.btnSave.UniqueID;

        UAobj.useridvalue = Session["UserID"].ToString();

        DataRow dr = UAobj.GetPassword();
        if (dr == null)
        {
            Anthem.Manager.IncludePageScripts = true;
            String script = String.Format("alert('{0}');", "Cannot change your password contact to your administrator!");
            Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);
            //ViewState["Pass"] = dr["Password"].ToString();
        }
        else
        {
            username.Value = dr["loginname"].ToString();
        }
        if (!Page.IsPostBack)
        {
            Random Rand = new Random();
            string finalKey = Guid.NewGuid().ToString("N").Substring(0,16);
            int randomKey = Rand.Next(100000000, 999999999);
            int randomKeyval = Rand.Next(100, 999);

            string randomsalt = Convert.ToString(randomKey) + Convert.ToString(randomKeyval);

            Session["Randomsalt"] = randomsalt;

            Session["Key"] = finalKey;
            Setpolicy();
        }
    }
    private void Setpolicy()
    {
        DataSet ds = obj.GetPasswordPolicies();
        if (ds.Tables[0].Rows.Count > 0)
        {
            MinLength.Value = ds.Tables[0].Rows[0]["MinLength"].ToString();
            MinUpperCaseChar.Value = ds.Tables[0].Rows[0]["MinUpperCaseChar"].ToString();
            MinNumericChar.Value = ds.Tables[0].Rows[0]["MinNumericChar"].ToString();
            MinSpecialChar.Value = ds.Tables[0].Rows[0]["MinSpecialChar"].ToString();
            Allow_Pass_equal_userid.Value = ds.Tables[0].Rows[0]["Allow_Pass_equal_userid"].ToString();
        }
    }

    public static string DecryptStringAES(string cipherText, string Key)
    {

        var keybytes = Encoding.UTF8.GetBytes(Key);
        var iv = Encoding.UTF8.GetBytes(Key);

        //var keybytes = Encoding.UTF8.GetBytes("1234567890123456");
        //var iv = Encoding.UTF8.GetBytes("1234567890123456");

        var encrypted = Convert.FromBase64String(cipherText);
        var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
        return string.Format(decriptedFromJavascript);
    }

    private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
    {
        // Check arguments.  
        if (cipherText == null || cipherText.Length <= 0)
        {
            throw new ArgumentNullException("cipherText");
        }
        if (key == null || key.Length <= 0)
        {
            throw new ArgumentNullException("key");
        }
        if (iv == null || iv.Length <= 0)
        {
            throw new ArgumentNullException("key");
        }

        // Declare the string used to hold  
        // the decrypted text.  
        string plaintext = null;

        // Create an RijndaelManaged object  
        // with the specified key and IV.  
        using (var rijAlg = new RijndaelManaged())
        {
            //Settings  
            rijAlg.Mode = CipherMode.CBC;
            rijAlg.Padding = PaddingMode.PKCS7;
            rijAlg.FeedbackSize = 128;
           

            rijAlg.Key = key;
            rijAlg.IV = iv;

            // Create a decrytor to perform the stream transform.  
            var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

            try
            {
                // Create the streams used for decryption.  
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {

                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream  
                            // and place them in a string.  
                            plaintext = srDecrypt.ReadToEnd();

                        }

                    }
                }
            }
            catch
            {
                plaintext = "keyError";
            }
        }

        return plaintext;
    }  


     //static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
     //   {
     //       // Check arguments.
     //       if (cipherText == null || cipherText.Length <= 0)
     //           throw new ArgumentNullException("cipherText");
     //       if (Key == null || Key.Length <= 0)
     //           throw new ArgumentNullException("Key");
     //       if (IV == null || IV.Length <= 0)
     //           throw new ArgumentNullException("IV");

     //       // Declare the string used to hold
     //       // the decrypted text.
     //       string plaintext = null;

     //       // Create an Aes object
     //       // with the specified key and IV.
     //       using (Aes aesAlg = Aes.Create())
     //       {
     //           aesAlg.Key = Key;
     //           aesAlg.IV = IV;

     //           // Create a decryptor to perform the stream transform.
     //           ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

     //           // Create the streams used for decryption.
     //           using (MemoryStream msDecrypt = new MemoryStream(cipherText))
     //           {
     //               using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
     //               {
     //                   using (StreamReader srDecrypt = new StreamReader(csDecrypt))
     //                   {

     //                       // Read the decrypted bytes from the decrypting stream
     //                       // and place them in a string.
     //                       plaintext = srDecrypt.ReadToEnd();
     //                   }
     //               }
     //           }

     //       }

     //       return plaintext;

     //   }
    

    Boolean Check_Password_policies()
    {

        Boolean flag = false;
        DataSet ds = obj.GetPasswordPolicies();
        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {

            //Check Min. Length Policy
            if (R_txtRPassword.Text.Trim().Length >= Convert.ToInt32(ds.Tables[0].Rows[0]["MinLength"]))
            {
                flag = true;
            }
            else
            {
                String script = String.Format("alert('{0}');", "New password should have minimum " + Convert.ToInt32(ds.Tables[0].Rows[0]["MinLength"]) + " Characters");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);
                return false;
            }
            if (Convert.ToBoolean(ds.Tables[0].Rows[0]["MinNumericChar"]) == true)
            {
                flag = false;
                //Check minimum number of Numeric values in password
                char[] s = R_txtRPassword.Text.ToCharArray();


                for (int x = 0; x < s.Length; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        if (s[x].ToString() == y.ToString())
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (flag == false)
                {
                    String script = String.Format("alert('{0}');", "New password should Contain Numeric Value");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);
                    return false;
                }
            }

            //Check Special Character
            if (Convert.ToBoolean(ds.Tables[0].Rows[0]["MinSpecialChar"]) == true)
            {
                flag = false;
                string sss = @"~'!@#$%^&*()-+={[}]|\\:;\'<,>.?/\\""";
                char[] t = sss.ToCharArray();
                int indexOf = R_txtRPassword.Text.Trim().IndexOfAny(t);
                if (indexOf == -1)
                {
                    flag = false;
                }
                else
                    flag = true;

                if (flag == false)
                {
                    String script = String.Format("alert('{0}');", "New password should Contain Special Character");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);
                    return false;
                }
            }


            //Check uppercase
            if (Convert.ToBoolean(ds.Tables[0].Rows[0]["MinUpperCaseChar"]) == true)
            {
                flag = false;
                foreach (char c in R_txtRPassword.Text)
                {
                    if (char.IsUpper(c))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag == false)
                {
                    String script = String.Format("alert('{0}');", "New password should Contain Upper Case Character");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);
                    return false;
                }
            }


            ////Check userid=password or not
            if (Convert.ToBoolean(ds.Tables[0].Rows[0]["Allow_pass_equal_userid"]) == true)
            {
                flag = false;
                if (Session["Userid"].ToString() == R_txtRPassword.Text.Trim())
                {
                    flag = true;
                }
                else
                {
                    String script = String.Format("alert('{0}');", "New password should be equal to user id");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);
                    return false;
                }
            }



        }
        return flag;
    }

    public static string CreatePasswordHash(string pwd, string salt)
    {
        string saltAndPwd = pwd  + salt;//String.Concat(salt,pwd);
        string hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "sha1");

        return hashedPwd;
    }

    protected bool CheckValid()
    {
         string checkpassowrdpolicy = "^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*+,-])(?=.{8,})";

        //if (R_txtOldPassword.Text.Trim() == "")
        //{
        //    Anthem.Manager.IncludePageScripts = true;
        //    String script = String.Format("alert('{0}');", "Old Password is required!");
        //    Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);
        //    R_txtPwd.Focus();
        //    return false;
        //}
        //if (R_txtPwd.Text.Trim() == "")
        //{
        //    Anthem.Manager.IncludePageScripts = true;
        //    String script = String.Format("alert('{0}');", "New Password is required!");
        //    Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);
        //    R_txtPwd.Focus();
        //    return false;
        //}

        ////string NewPass = DecryptStringAES(R_txtPwd.Text.Trim(), Convert.ToString(Session["Key"]));
        ////string oldPass = DecryptStringAES(R_txtOldPassword.Text.Trim(), Convert.ToString(Session["Key"]));
        ////string confirmPass = DecryptStringAES(R_txtRPassword.Text.Trim(), Convert.ToString(Session["Key"]));


        ////if (!Regex.IsMatch(NewPass, checkpassowrdpolicy))
        ////{
        ////    Anthem.Manager.IncludePageScripts = true;
        ////    String script = String.Format("alert('{0}');", "Invalid passowrd !");
        ////    Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);
        ////    R_txtPwd.Focus();
        ////    return false;
        ////}

        

        //if (R_txtRPassword.Text.Trim() == "")
        //{
        //    Anthem.Manager.IncludePageScripts = true;
        //    String script = String.Format("alert('{0}');", "Confirm New Password is required!");
        //    Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);
        //    R_txtRPassword.Focus();
        //    return false;
        //}

        if (hdnewpass.Value != hdconfirmPass.Value)
        {
            Anthem.Manager.IncludePageScripts = true;
            String script = String.Format("alert('{0}');", "Password & Confirm Password must be same !");
            Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);
            R_txtRPassword.Text = "";
            R_txtRPassword.Focus();
            return false;
        }

        //if (NewPass.Trim() != confirmPass.Trim())
        //{
        //    Anthem.Manager.IncludePageScripts = true;
        //    String script = String.Format("alert('{0}');", "Password & Confirm Password must be same !");
        //    Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);
        //    R_txtRPassword.Text = "";
        //    R_txtRPassword.Focus();
        //    return false;
        //}

        DataRow dr = UAobj.GetPassword();
        if (!dr.IsNull("password"))
        {
            string salt = Convert.ToString(Session["Randomsalt"]);
            string pwd = hdpwd.Value;
            if (pwd.ToString().ToUpper() == Convert.ToString(CreatePasswordHash (dr["password"].ToString(),salt)))
            {
                return true;
            }
            else
            {
                Anthem.Manager.IncludePageScripts = true;
                String script = String.Format("alert('{0}');", "Old password entered was incorrect");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);
                R_txtOldPassword.Focus();
                return false;
            }
        }
        return false;
    }

    private static string CreateSalt(int size)
    {
        //Generate a cryptographic random number.
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        byte[] buff = new byte[size];
        rng.GetBytes(buff);
        // Return a Base64 string representation of the random number.
        return Convert.ToBase64String(buff);
    }

    //public static string CreatePasswordHash(string pwd, string salt)
    //{
    //    string saltAndPwd = String.Concat(pwd, salt);
    //    string hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "sha1");
    //    return hashedPwd;
    //}


    protected void btnSave_Click(object sender, EventArgs e)
    {
       
      //  if (CheckValid() == true)
       // {
            //if (Check_Password_policies() == true)
            //{
           // int queryreturn = 0;
            string useridvalue = Session["UserID"].ToString();
            string newpwd = hash.Value;
            string newsalt = Convert.ToString(Session["Randomsalt"]);
            //string Plain_Password = R_txtRPassword.Text.Trim();  //DecryptStringAES(R_txtPwd.Text.Trim(), Convert.ToString(Session["Key"]));
        string Plain_Password = DecryptStringAES(R_txtPwd.Text.Trim(), Convert.ToString(Session["Key"]));
        string UserIPvalue = HttpContext.Current.Request.UserHostAddress;
        int result = SMS_SP_ChangePwd(Plain_Password, useridvalue).Execute();
      
            //queryreturn = UAobj.ChangePassword();
           
            
            if (result > 0)
            {
                Anthem.Manager.IncludePageScripts = true;
                String script = String.Format("alert('{0}');", "Password changed successfully");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);

            }
            //}
            //else
            //{
            //    Anthem.Manager.IncludePageScripts = true;
            //    String script = String.Format("alert('{0}');", "New Password does not match with policy");
            //    Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);
            //}
       // }

        //Random Rand = new Random();
        //int randomKey = Rand.Next(100000000, 999999999);
        //int randomKeyval = Rand.Next(100, 999);
        //string randomsalt = Convert.ToString(randomKey) + Convert.ToString(randomKeyval);
        //Session["Randomsalt"] = randomsalt;
    }


    public static StoredProcedure SMS_SP_ChangePwd(string Plain_Password, string userid)
    {
        SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("SMS_SP_ChangePwd", DataService.GetInstance("IUMSNXG"), "");
        sp.Command.AddParameter("@Plain_Password", Plain_Password, DbType.String);
        sp.Command.AddParameter("@userid", userid, DbType.String);
        return sp;
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        clear();
    }
    private void clear()
    {
        R_txtOldPassword.Text = "";
        R_txtPwd.Text = "";
        R_txtRPassword.Text = "";
    }

}
