using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using UMMBusinessLayer;
using System.Security.Cryptography;
using System.Text;
using System.Web.SessionState;
using System.Reflection;
using SubSonic;
using System.Net;
using System.IO;

public partial class Loginpage : System.Web.UI.Page
{
    UMMBusinessLayer.LoginPage LPobj = new UMMBusinessLayer.LoginPage();
    crypto crypt = new crypto();
    string userName, password, salt;
    string otp;
    string hpassd;
    protected void Page_PreInit(object sender, EventArgs e)
    {
        Page.Theme = ""; //default theme
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Popup"] != null)
        {
            if (Session["Popup"].ToString().Trim() == "Show")
            {
                ShowPopup();
            }
        }

        if (!IsPostBack)
        {
            //Add for encription of hash key run time Date 26102023
            string guid = Guid.NewGuid().ToString("N").Substring(0, 16);
            Session["Key"] = guid;

        }

    }
    protected void imgLogin_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(uname.Value))
        {
            userName = uname.Value;
        }

        if (!string.IsNullOrEmpty(hash.Value))
        {

            password = hash.Value;
        }
        if (!string.IsNullOrEmpty(saltval.Value))
        {
            salt = saltval.Value;
        }

        if (!string.IsNullOrEmpty(hpass.Value))
        {
            hpassd = hpass.Value;
            hpassd = DecryptStringAES(hpassd.ToString().Trim(), Convert.ToString(Session["Key"]));
            if (Convert.ToString(hash.Value) != Convert.ToString(hpassd))
            {
                Response.Redirect("~/Loginpage.aspx?invalid");
            }
        }

        //Process login
        ClearSession();
        Login();
        

    }
    protected void btnVerify_Click(object sender, EventArgs e)
    {
        VerifyOtp();
    }


    protected void ShowPopup()
    {
        Session["Popup"] = null;
        String script = String.Format("$('#dvEnNo').fadeIn(1000);$('#dvEnroll').fadeIn(500);");
        Anthem.Manager.IncludePageScripts = true;
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSearchOffDiv", script, true);
    }

    private void VerifyOtp()
    {
        try
        {
            string otp = R_txtOtp.Text.Trim();

            using (DataSet Ds = GetOtp(Session["UserID"].ToString().Trim()).GetDataSet())
            {
                if (Ds.Tables.Count > 0 && Ds.Tables[0].Rows.Count > 0)
                {
                    string dbotp = Convert.ToString(Ds.Tables[0].Rows[0]["otp"]);
                    if (dbotp.ToString().Trim() == otp.ToString().Trim())
                    {
                        Response.Redirect("~/Modules.aspx");
                        ViewState["icount"] = 1;
                    }
                    else
                    {
                        ShowPopup();
                    }
                }
            }
        }
        catch (Exception Err)
        {

        }
    }
    protected void SendOtp_User(string mobileno, string otp)
    {
        try
        {
            string msg = "Please enter the Otp for login: " + otp;
            //Create a new web request for the URL.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.instaalerts.zone/SendSMS/sendmsg.php?uname=CRSUJIND&pass=Crsu@123&send=IUMSAP&dest=" + mobileno + "&msg=" + msg);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        }
        catch (Exception ex) { }
    }

    public StoredProcedure GetRoles(string UserId)
    {
        SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("Sp_GetRoles", DataService.GetInstance("IUMSNXG"), "");
        sp.Command.AddParameter("@UserId", UserId, DbType.String);

        return sp;
    }
    public StoredProcedure InsertOtp(string UserId, string Otp)
    {
        SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("SP_InsertOtp", DataService.GetInstance("IUMSNXG"), "");
        sp.Command.AddParameter("@loginname", UserId, DbType.String);
        sp.Command.AddParameter("@otp", Otp, DbType.String);
        return sp;
    }
    public StoredProcedure GetOtp(string UserId)
    {
        SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("SP_GetOtp", DataService.GetInstance("IUMSNXG"), "");
        sp.Command.AddParameter("@loginname", UserId, DbType.String);

        return sp;
    }

    private string RandomNumber()
    {
        Random generator = new Random();
        String Num = generator.Next(0, 1000000).ToString("D6");
        return Num;
    }
    public static string CreatePasswordHash12(string pwd, string salt)
    {
        //string saltAndPwd = String.Concat(pwd, salt);
        string hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(pwd, "sha1");
        return hashedPwd;
    }

    private void ClearSession()
    {
        Session["ModuleID"] = "";
        Session["UserID"] = "";
        Session["LoginName"] = "";
        Session["RoleLevelID"] = "";
        Session["LocationID"] = "";
        Session["LocationName"] = "";
        Session["MappedAlias"] = "";
        Session["DDOID"] = "";
        Session["DDO"] = "";
        Session["SalFinId"] = "";
        Session["SYear"] = "";
        Session["EYear"] = "";
        Session["ModIDForUC"] = "";
        Session["AcctFinId"] = "";
        Session["LRSFinId"] = "";
        Session["PTime"] = "";
        Session["CTime"] = "";
        Session["LOGID"] = "";
        Session["SchemeID"] = "";//for maintaing schemeid
        Session["UserPages"] = "";
        Session["empID"] = "";
        Session["cmodule"] = "";
        Session["pwdhash"] = "";
        Session["Popup"] = "";
    }


    private void Login()
    {
        try
        {
            DataSet dsrights = new DataSet();
            dsrights = CheckRights();

            string UserID = "";
            string EmpID = "";
            ArrayList arr_ModuleID = new ArrayList();

            if (dsrights.Tables.Count > 0 && dsrights.Tables[0].Rows.Count > 0)
            {
                UserID = dsrights.Tables[0].Rows[0]["pk_userId"].ToString();
                EmpID = dsrights.Tables[0].Rows[0]["fk_empId"].ToString();
                Session["UserID"] = dsrights.Tables[0].Rows[0]["pk_userId"].ToString();
                //Session["LoginName"] = dsrights.Tables[0].Rows[0]["name"].ToString();
                Session["LoginName"] = dsrights.Tables[0].Rows[0]["loginname"].ToString();
                Session["empID"] = dsrights.Tables[0].Rows[0]["fk_empId"].ToString();
                if (dsrights.Tables[2].Rows.Count > 0)
                {
                    Session["SalFinId"] = dsrights.Tables[2].Rows[0]["pk_finid"].ToString();
                    Session["SYear"] = dsrights.Tables[2].Rows[0]["date1"].ToString();
                    Session["EYear"] = dsrights.Tables[2].Rows[0]["date2"].ToString();
                }

                if (dsrights.Tables[4].Rows.Count > 0)
                {
                    Session["AcctFinId"] = dsrights.Tables[4].Rows[0]["pk_finid"].ToString();
                }

                if (dsrights.Tables[5].Rows.Count > 0)
                {
                    Session["LRSFinId"] = dsrights.Tables[5].Rows[0]["pk_finyearid"].ToString();
                    Session["SchemeID"] = "";//for maintaing schemeid
                }
                //Check if this user is already logged in or Not
                //If Logged in then destroy that loogged user session [ Added By Rajeev Ranjan On 01 Aug 2007 ]
                InsertUserLoginTrack(UserID);

                if (dsrights.Tables[3].Rows.Count > 0)
                {
                    Session["ModuleID"] = (ArrayList)arr_ModuleID;
                    Session["MappedAlias"] = dsrights.Tables[3].Rows[0]["mappedalias"].ToString();
                    Session["RoleLevelID"] = dsrights.Tables[3].Rows[0]["fk_rolelevelId"].ToString();

                    Session["UserPages"] = (DataTable)dsrights.Tables[5];//rights table

                    string userid = Session["UserID"].ToString();
                    string ip = Request.Params["REMOTE_ADDR"].ToString();

                    int i = 0;
                    LPobj.useridvalue = userid;
                    LPobj.UserIPvalue = ip;
                    LPobj.Pwdhash = hash.Value;
                    DataSet dsl = LPobj.UserAuthenticationLogs();
                    if (dsl.Tables[0].Rows.Count > 0)
                    {
                        Session["LOGID"] = dsl.Tables[1].Rows[0]["pk_ulogid"].ToString();
                        Session["PTime"] = dsl.Tables[0].Rows[0]["logintime"].ToString();
                        Session["CTime"] = dsl.Tables[2].Rows[0]["logintime"].ToString();
                        //Session["pwdhash"] = dsl.Tables[2].Rows[0]["pwdhash"].ToString();
                    }

                    string guid = Guid.NewGuid().ToString();
                    Session["AuthToken"] = guid;
                    // now create a new cookie with this guid value
                    Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                    //Regenerate SessionID after successful login
                    regenerateId();

                    using (DataSet Ds = GetRoles(Convert.ToString(Session["UserID"])).GetDataSet())
                    {
                        if (Ds.Tables.Count > 0 && Ds.Tables[0].Rows.Count > 0)
                        {
                            int roleid = Convert.ToInt32(Ds.Tables[0].Rows[0]["fk_roleId"]);
                            if (roleid == 7)
                            {
                                otp = RandomNumber();

                                Session["UserID"] = Ds.Tables[0].Rows[0]["pk_userId"].ToString();
                                string mobileNo = Ds.Tables[0].Rows[0]["mobileno"].ToString();
                                InsertOtp(Convert.ToString(Session["UserID"]), otp).GetDataSet();
                                Session["Popup"] = "Show";


                                SendOtp_User(mobileNo, otp);

                            }
                            else
                            {
                                Response.Redirect("~/Modules.aspx");
                                ViewState["icount"] = 1;
                            }
                        }
                    }
                }
                dsrights.Dispose();
                dsrights.Clear();

            }
            else
            {
                Response.Redirect("~/Loginpage.aspx?invalid");
            }
        }
        catch (Exception ex)
        {

        }
    }

    //Generate Neww Sessionid after successfull login to Resolve Session Fixation
    void regenerateId()
    {
        System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
        string oldId = manager.GetSessionID(Context);
        string newId = manager.CreateSessionID(Context);
        bool isAdd = false, isRedir = false;
        manager.SaveSessionID(Context, newId, out isRedir, out isAdd);
        HttpApplication ctx = (HttpApplication)HttpContext.Current.ApplicationInstance;
        HttpModuleCollection mods = ctx.Modules;
        System.Web.SessionState.SessionStateModule ssm = (SessionStateModule)mods.Get("Session");
        System.Reflection.FieldInfo[] fields = ssm.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        SessionStateStoreProviderBase store = null;
        System.Reflection.FieldInfo rqIdField = null, rqLockIdField = null, rqStateNotFoundField = null;
        foreach (System.Reflection.FieldInfo field in fields)
        {
            if (field.Name.Equals("_store")) store = (SessionStateStoreProviderBase)field.GetValue(ssm);
            if (field.Name.Equals("_rqId")) rqIdField = field;
            if (field.Name.Equals("_rqLockId")) rqLockIdField = field;
            if (field.Name.Equals("_rqSessionStateNotFound")) rqStateNotFoundField = field;
        }
        object lockId = rqLockIdField.GetValue(ssm);
        if ((lockId != null) && (oldId != null)) store.ReleaseItemExclusive(Context, oldId, lockId);
        rqStateNotFoundField.SetValue(ssm, true);
        rqIdField.SetValue(ssm, newId);
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

    public static string CreatePasswordHash(string pwd, string salt)
    {
        string saltAndPwd = salt + pwd;//String.Concat(salt,pwd);
        string hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "sha1");

        return hashedPwd;
    }

    private bool Uservalidate()
    {
        try
        {
            LPobj.loginidvalue = userName;
            LPobj.Pwdhash = hash.Value;
            DataTable dt = LPobj.UserValidation();
            if (dt.Rows.Count > 0)
            {


                //string salt = dt.Rows[0]["Salt"].ToString();//Create Salt of Size 8

                // string hashpass = CreatePasswordHash(password, salt);
                string Randomsalt = "";
                Randomsalt = password.Substring(0, 6) + password.Substring(46, 6);
                password = password.Substring(6, 40);

                if (password.ToUpper() == CreatePasswordHash(dt.Rows[0]["password"].ToString(), Randomsalt).ToUpper())
                {
                    password = dt.Rows[0]["password"].ToString();
                    return true;
                }
                else
                {

                    Session.Clear();
                    Session.Abandon();

                    Session.Clear();
                    Session.Abandon();
                    Session.RemoveAll();

                    if (Request.Cookies["ASP.NET_SessionId"] != null)
                    {
                        Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                        Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                    }

                    if (Request.Cookies["AuthToken"] != null)
                    {
                        Response.Cookies["AuthToken"].Value = string.Empty;
                        Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
                    }
                    return false;
                }
            }
            else
            {

            }
            dt.Dispose();
            dt.Clear();
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }

    }

    private DataSet CheckRights()
    {
        DataSet ds_rights = new DataSet();
        try
        {
            if (Uservalidate())
            {

                LPobj.loginidvalue = userName;
                LPobj.passwordvalue = password;//crypt.Encrypt(CommonFunction.ReturnTextifNotBlank(R_txtPass));
                ds_rights = LPobj.UserAuthentication();
            }
            else
            {
                Session.Clear();
                Session.Abandon();

                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();

                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                }

                if (Request.Cookies["AuthToken"] != null)
                {
                    Response.Cookies["AuthToken"].Value = string.Empty;
                    Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
                }

                Response.Redirect("~/Loginpage.aspx?invalid");
            }
        }
        catch (Exception ex)
        {
            Session.Clear();
            Session.Abandon();

            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }

        }
        return ds_rights;
    }

    protected void InsertUserLoginTrack(string UserID)
    {
        try
        {
            LPobj.loginidvalue = UserID.ToString().Trim();
            LPobj.UserSession = Session.SessionID.ToString().Trim();
            LPobj.InsertUserLoginTrack();
        }
        catch
        {
            throw;
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

}
