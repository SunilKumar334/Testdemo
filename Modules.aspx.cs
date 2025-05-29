using System;
using System.Data;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using UMMBusinessLayer;
using SubSonic;
using System.Text.RegularExpressions;
using System.Web;
using DataAccessLayer;

public partial class Modules : System.Web.UI.Page
{
    UMMMaster LPobj = new UMMMaster();

    #region DataLayer
    DataAccess DAobj = new DataAccess();
    ArrayList names = new ArrayList(); ArrayList types = new ArrayList(); ArrayList values = new ArrayList();
    //string _userid, _fk_locid;

    public string userid { get; set; }
    public string fk_locid { get; set; }
    public string logintype { get; set; }
    public DataSet GetImages()
    {
        names.Clear(); values.Clear(); types.Clear();
        names.Add("@UserID"); types.Add(SqlDbType.VarChar); values.Add(userid);
        names.Add("@fk_locid"); types.Add(SqlDbType.VarChar); values.Add(fk_locid);
        names.Add("@logintype"); types.Add(SqlDbType.VarChar); values.Add(logintype);
        return DAobj.GetDataSet("UM_SP_ModuleImages_Get_Loginwise", values, names, types);
    }
    #endregion
    protected void Page_Init()
    {
        Label lbl;
        lbl = (Label)Master.FindControl("lblCheck");
        lbl.Visible = false;
        lbl.Text = "";

    }

    private DataSet getemployeemodulepagelist(int moduleid)
    {
        DataAccessLayer.DataAccess DAobj = new DataAccessLayer.DataAccess();
        ArrayList names = new ArrayList();
        ArrayList values = new ArrayList();
        ArrayList types = new ArrayList();
        names.Add("@pk_moduleid"); values.Add(moduleid); types.Add(SqlDbType.VarChar);
        return DAobj.GetDataSet("getpagelistbymodule", values, names, types);
    }
    public static StoredProcedure GetCollegeDetail(string fk_userid)
    {
        SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("Get_College_Detail", DataService.GetInstance("IUMSNXG"), "");
        sp.Command.AddParameter("@fk_userid", fk_userid, DbType.String);
        sp.CommandTimeout = 10000;
        return sp;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            bool IsDetUpdate = false;
            DataSet ds;

            bool IsUpdated = false;
            DataSet ds1;
            //ViewState["pk_sid_enroll"] = Convert.ToInt32(e.CommandArgument);
            using (ds = GetCollegeDetail(Convert.ToString(Session["UserID"])).GetDataSet())
            {
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Session["pk_collegeid"] = Convert.ToInt32(ds.Tables[0].Rows[0]["pk_collegeid"]);
                    txtMobile.Text = ds.Tables[0].Rows[0]["mobile"].ToString();
                    txtEmail.Text = ds.Tables[0].Rows[0]["email"].ToString();
                    txtContact.Text = ds.Tables[0].Rows[0]["phone"].ToString();
                    txtPrincName.Text = ds.Tables[0].Rows[0]["PrincipalName"].ToString();

                    IsDetUpdate = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsDetailUpdate"]);
                    lblCollege.Text = ds.Tables[0].Rows[0]["College"].ToString();
                }
            }
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                if (IsDetUpdate == false)
                {
                    //dvClose.Visible = false;
                    String script = String.Format("$('#dvEnNoEnroll').fadeIn(1000);$('#dvEnrollment').fadeIn(500);");
                    Anthem.Manager.IncludePageScripts = true;
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSearchOffDiv", script, true);
                }

            }

            //02-03-2023 Update Branch 
            populatebranch();
            string userID = Session["UserID"].ToString().Trim();
            int roleID = GetRole(userID);

            if (roleID == 21 || roleID == 22)
            {
                using (ds1 = Get_BranchUser_Detail(Convert.ToString(Session["UserID"])).GetDataSet())
                {
                    if (ds1.Tables[0].Rows[0]["isUpdate"].ToString() == "0")
                    {
                        String script = String.Format("$('#dvBranchUpdate').fadeIn(1000);$('#dvBranch').fadeIn(500);");
                        Anthem.Manager.IncludePageScripts = true;
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSearchOffDiv", script, true);

                        txtName.Text = ds1.Tables[1].Rows[0]["name"].ToString();
                        Textmobile.Text = ds1.Tables[1].Rows[0]["mobileno"].ToString();
                        Textemail.Text = ds1.Tables[1].Rows[0]["email"].ToString();
                        ViewState["fk_roleid"] = Convert.ToInt32(ds1.Tables[1].Rows[0]["fk_roleid"]);
                        ViewState["fk_locid"] = Convert.ToString(ds1.Tables[1].Rows[0]["fk_locid"]);
                    }
                }
            }
        }


        //Added by Balbhadra for employee login start
        if (Session["empid"] != null)
        {
            if (Request.UrlReferrer != null && Session["empid"].ToString() != "")
            {
                DataSet ds = getemployeemodulepagelist(99);
                DataRow[] dr = ds.Tables[0].Select("webpagename = '" + System.IO.Path.GetFileName(Request.UrlReferrer.LocalPath).ToString().ToLower() + "'");
                if (dr.Length > 0)
                {

                }
                else
                {
                    ClearSession();
                    //ClientMessaging("You are being redirected To Employee Login  Portal !!");

                    //Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", "alert('You are being redirected To Employee Login  Portal !!'); window.location='" + Request.ApplicationPath + "Loginpage_Emp.aspx'; ", true);
                    Response.Redirect("Loginpage_Emp.aspx?i=1");
                    return;
                }
            }

            //if (Session["empid"] != "" && (Request.UrlReferrer != null && !Request.UrlReferrer.ToString().ToLower().Contains("loginpage_emp")))
            //{

            //}
        }


        //Added by Balbhadra for employee login end


        Menu tr;
        Label lbl;
        //ImageButton imgbtn;
        tr = (Menu)Master.FindControl("MainMenu");
        lbl = (Label)Master.FindControl("lblCheck");
        //imgbtn = (ImageButton)Master.FindControl("hplBtn");
        Master.FindControl("modulediv").Visible = false;

        //imgbtn.Visible = false;
        tr.Visible = false;
        lbl.Text = "";
        Session["modulename"] = "";
        Session["moduleimage"] = "";


        if (!IsPostBack)
        {

            PopulateLocation();
            if (Session["LoginName"].ToString() == "vc" || Session["LoginName"].ToString() == "VC")
                Response.Redirect("Modulesv.aspx");
            Bind_Grid();
            DataList1.Focus();
            BindRTIApplication();
        }
        getMsg();

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
        Session["ChangePassword"] = "";
        Session.Clear();
        Session.Abandon();
        Session.RemoveAll();
    }

    void PopulateLocation()
    {
        string st = Session["LocationID"].ToString().Trim();
        LPobj.useridvalue = Session["UserID"].ToString().Trim();
        DataSet ds = LPobj.Getlocation();
        if (ds.Tables[0].Rows.Count > 0)
        {
            ddlloc.DataSource = ds.Tables[0];
            ddlloc.DataTextField = "locname";
            ddlloc.DataValueField = "pk_locid";
            ddlloc.DataBind();
            ddlloc.Items.Insert(0, new ListItem("-- Select Location --", ""));

            if (ds.Tables[1].Rows[0]["fk_defaultlocation"].ToString() != "")
            {
                if (Session["LocationID"].ToString() == "")
                {
                    ddlloc.SelectedValue = ds.Tables[1].Rows[0]["fk_defaultlocation"].ToString();
                }
                else
                {
                    ddlloc.SelectedValue = Session["LocationID"].ToString();
                }
                SetSessionId();
                Bind_Grid();
            }
            else
            {
                ddlloc.SelectedIndex = 0;
            }
        }
        else
        {
            ddlloc.Items.Insert(0, new ListItem("-- Select Location --", ""));
        }
        ds.Dispose();
        ds.Clear();
    }


    void Bind_Grid()
    {
        crypto cpt = new crypto();
        string host = HttpContext.Current.Request.Url.GetComponents(UriComponents.HostAndPort, UriFormat.Unescaped); ;

        //Response.Write("<script>alert(" + host + ");</script>");

        if (host.Contains("backoffice"))
        {
            logintype = "Back_Office";
        }
        else
        {
            logintype = "Front_Office";
        }

        //LPobj.useridvalue = Session["UserID"].ToString();
        //LPobj.fk_locid = Session["LocationID"].ToString();
        //DataSet ds = LPobj.GetImages();

        userid = Session["UserID"].ToString();
        fk_locid = Session["LocationID"].ToString();

        DataSet ds = GetImages();
        ds.Tables[0].Columns.Add("path");
        ds.Tables[0].Columns.Add("enc");
        int rnum = (ds.Tables[0].Rows.Count) + 1;
        //DataRow dr = ds.Tables[0].NewRow();
        //dr["moduleImage"] = "hrms.png";
        //dr["modulename"] = "Employee Portal";
        //dr["pk_moduleId"] = "999";
        //dr["path"] = "http://182.18.173.148/ssp/index.aspx";
        //ds.Tables[0].Rows.Add(dr);

        //dr = ds.Tables[0].NewRow();
        //dr["moduleImage"] = "StudentPortal.png";
        //dr["modulename"] = "Student Portal";
        //dr["pk_moduleId"] = "991";
        //dr["path"] = "http://182.18.173.148/ssp/index.aspx";
        //ds.Tables[0].Rows.Add(dr);

        for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
        {
            string pkid = ds.Tables[0].Rows[x]["pk_moduleId"].ToString();
            string encd = cpt.EncodeString(Convert.ToInt32(pkid));
            ds.Tables[0].Rows[x]["enc"] = encd;
        }


        DataView dv1 = ds.Tables[0].DefaultView;
        //dv1.Sort = "modulename";
        dv1.RowFilter = "moduletype = 1";
        if (dv1.Count > 0)
        {
            div1.Visible = true;
            lblType1.Text = dv1[0]["ModuleTypeName"].ToString();
            DataList1.DataSource = dv1;
            DataList1.DataBind();
        }
        else
        {
            div1.Visible = false;
            lblType1.Text = "";
        }

        DataView dv2 = ds.Tables[0].DefaultView;
        //dv2.Sort = "modulename";
        dv2.RowFilter = "moduletype = 2";
        if (dv2.Count > 0)
        {
            div2.Visible = true;
            lblType2.Text = dv2[0]["ModuleTypeName"].ToString();
            DataList2.DataSource = dv2;
            DataList2.DataBind();
        }
        else
        {
            div2.Visible = false;
            lblType2.Text = "";
        }

        DataView dv3 = ds.Tables[0].DefaultView;
        //dv3.Sort = "modulename";
        dv3.RowFilter = "moduletype = 3";
        if (dv3.Count > 0)
        {
            div3.Visible = true;
            lblType3.Text = dv3[0]["ModuleTypeName"].ToString();
            DataList3.DataSource = dv3;
            DataList3.DataBind();
        }
        else
        {
            div3.Visible = false;
            lblType3.Text = "";
        }

        DataView dv4 = ds.Tables[0].DefaultView;
        //dv4.Sort = "modulename";
        dv4.RowFilter = "moduletype = 4";
        if (dv4.Count > 0)
        {
            div4.Visible = true;
            lblType4.Text = dv4[0]["ModuleTypeName"].ToString();
            DataList4.DataSource = dv4;
            DataList4.DataBind();
        }
        else
        {
            div4.Visible = false;
            lblType4.Text = "";
        }

        ds.Dispose();
        ds.Clear();
        dv1.Dispose();
        dv2.Dispose();
        dv3.Dispose();
        dv4.Dispose();

    }

    private void getMsg()
    {
        CommonMessaging RMobj = new CommonMessaging();
        CommonFunction cObj = new CommonFunction();
        CommonCode.Common objComm = new CommonCode.Common();
        RMobj.DDOID = Session["DDOID"].ToString();
        RMobj.Locid = Session["LocationID"].ToString();
        DataSet ds = RMobj.GetMessage();
        if (ds.Tables[0].Rows.Count > 0)
        {
            popupWindow.Visible = true;
            popupWindow.Title = "<h5>Quick Message</h5>";
            if (ds.Tables[0].Rows[0]["messages"].ToString().Trim() != "")
                popupWindow.Message = "<h5>" + ds.Tables[0].Rows[0]["messages"].ToString() + "</h5>";
            else
                popupWindow.Visible = false;
        }
        else
        {
            popupWindow.Visible = false;
            //popupWindow.Title = "<h5>Quick Message</h5>";
            //popupWindow.Message = "<h5>Sorry, No Message Available.</h5>";
            //popupWindow.Width = System.Web.UI.WebControls.Unit.Pixel(200);
            //popupWindow.Height = System.Web.UI.WebControls.Unit.Pixel(100);
            //popupWindow.HideAfter = 1200;
        }

        ds.Dispose();
        ds.Clear();
    }

    void SetSessionId()
    {
        LPobj.fk_locid = ddlloc.SelectedValue.ToString().Trim();
        DataRow dr = LPobj.GetDDOidOn_Locid();
        if (dr != null)
        {
            Session["DDOID"] = dr["pk_ddoid"].ToString().Trim();
            Session["DDO"] = dr["description"].ToString().Trim();
            Session["LocationName"] = ddlloc.SelectedItem.Text.ToString().Trim();
            Session["LocationID"] = ddlloc.SelectedValue.ToString().Trim();
            Session["OfficeType"] = dr["officetype"].ToString().Trim();
        }
        else
        {
            Session["DDOID"] = "";
            Session["DDO"] = "";
            Session["LocationName"] = "";
            Session["LocationID"] = "";
            Session["OfficeType"] = "";

            //ClientMessaging("Please select a location !");
            Anthem.Manager.IncludePageScripts = true;
            ddlloc.Focus();
        }
        Session["LocationName"] = ddlloc.SelectedItem.Text.ToString().Trim();
        Session["LocationID"] = ddlloc.SelectedValue.ToString().Trim();
    }
    protected void ClientMessaging(string msg)
    {
        String script = String.Format("alert('{0}');", msg);
        Anthem.Manager.IncludePageScripts = true;
        Page.ClientScript.RegisterStartupScript(this.GetType(), "errMsg", script, true);
    }
    protected void ddlloc_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetSessionId();
        Bind_Grid();
    }

    protected string getModulePath(object argPath, object argModuleId)
    {
        string lPath = "";

        if (argPath != null && argPath.ToString().Trim() != "")
        {
            lPath = argPath.ToString();
        }
        else
        {
            lPath = "UMM/Admin_Home.aspx?MID=" + argModuleId.ToString();
        }

        return lPath;
    }

    protected string getModuletarget(object argPath)
    {
        string ltaget = "";

        if (argPath != null && argPath.ToString().Trim() != "")
        {
            ltaget = "_blank";
        }
        else
        {
            ltaget = "_self";
        }

        return ltaget;
    }

    private void BindRTIApplication()
    {
        try
        {
            DataSet ds = new DataSet();
            using (ds = SearchRepRTIType().GetDataSet())
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    rptCustomers.DataSource = ds.Tables[0];
                    rptCustomers.DataBind();
                    ClientScript.RegisterStartupScript(this.GetType(), "Popup", "ShowPopup();", true);
                }
                else
                {
                    rptCustomers.DataSource = null;
                }
            }
        }
        catch (Exception Err)
        {

        }

    }
    public StoredProcedure SearchRepRTIType()
    {
        StoredProcedure sp = new StoredProcedure("RTI_ApplicationNew_Repor_Modules", DataService.GetInstance("IUMSNXG"), "");
        sp.Command.AddParameter("@userid", Session["UserID"].ToString(), DbType.String);
        sp.Command.AddParameter("@flag", "rtiformodule", DbType.String);
        return sp;
    }


    private bool validation()
    {
        if (txtMobile.Text.Trim() == "")
        {

            ClientMessaging("Mobile number is required!");
            txtMobile.Focus();

            return false;
        }
        if (txtMobile.Text.Trim() != "" && txtMobile.Text.Trim().Length < 10)
        {
            ClientMessaging("Mobile number should be 10 digit!");
            txtMobile.Focus();

            return false;
        }
        if (txtEmail.Text.Trim() == "")
        {
            ClientMessaging("Email is required!");
            txtEmail.Focus();
            return false;
        }
        if (txtEmail.Text.Trim() != "")
        {
            if (!IsValidEmailId(txtEmail.Text))
            {
                ClientMessaging("Email is not valid!");
                txtEmail.Focus();

                return false;
            }
        }



        return true;
    }
    private bool IsValidEmailId(string InputEmail)
    {
        //Regex To validate Email Address
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(InputEmail);
        if (match.Success)
            return true;
        else
            return false;
    }

    protected void btn_Classroll_Click(object sender, EventArgs e)
    {
        //string userid = Session["UserID"].ToString();
        //string ipaddress = HttpContext.Current.Request.UserHostAddress;
        if (validation() == false)
        {
            String script1 = String.Format("$('#dvEnNoEnroll').fadeIn(1000);$('#dvEnrollment').fadeIn(500);");
            Anthem.Manager.IncludePageScripts = true;
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSearchOffDiv", script1, true);
            return;
        }

        DataSet ds = UpdateInsertEnrollment(Convert.ToInt32(Session["pk_collegeid"]), txtMobile.Text, txtEmail.Text.ToString(), txtContact.Text, txtPrincName.Text).GetDataSet();

        if (ds.Tables[0].Rows.Count > 0 && Convert.ToString(ds.Tables[0].Rows[0]["CValue"]) != "0")
        {
            //Session["pk_sid"] = Convert.ToString(ds.Tables[0].Rows[0]["CValue"]).Trim();
            ClientMessaging("Record Update Successfully!");
        }
        else
        {
            ClientMessaging("Record can not be updated!");
        }

    }
    public static StoredProcedure UpdateInsertEnrollment(int pk_collegeid, string mobile, string email, string contact, string name)
    {
        SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("Acd_College_Detail_Update", DataService.GetInstance("IUMSNXG"), "");
        sp.Command.AddParameter("@pk_collegeid", pk_collegeid, DbType.Int32);
        sp.Command.AddParameter("@mobile", mobile, DbType.String);
        sp.Command.AddParameter("@email", email, DbType.String);
        sp.Command.AddParameter("@contact", contact, DbType.String);
        sp.Command.AddParameter("@name", name, DbType.String);
        sp.CommandTimeout = 10000;
        return sp;
    }

    //Change Daya Asawa
    //03-22-2023 Update Branch
    protected void populatebranch()
    {
        try
        {
            using (DataSet ds = Get_Branch().GetDataSet())
            {
                D_ddlbranch.DataSource = ds;
                D_ddlbranch.DataTextField = "description";
                D_ddlbranch.DataValueField = "pk_branchid";
                D_ddlbranch.DataBind();
                D_ddlbranch.Items.Insert(0, new ListItem("--Select Branch--", "0"));
            }
        }
        catch (Exception Err)
        {

        }

    }

    protected void btnupdate_Click(object sender, EventArgs e)
    {

        if (validate())
        {
            string name = txtName.Text.ToString();
            string mobile = Textmobile.Text.ToUpper();
            string email = Textemail.Text.ToString();
            int fk_branchid = Convert.ToInt32(D_ddlbranch.SelectedValue.ToString());
            string userid = Convert.ToString(Session["Userid"]);
            int fk_roleid = Convert.ToInt32(ViewState["fk_roleid"].ToString());
            string fk_locid = ViewState["fk_locid"].ToString();

            int i = UpdateInsertBranch(name, mobile, email, fk_branchid, userid, fk_roleid, fk_locid).Execute();

            if (i > 0)
            {

                ClientMessaging("Record Update Successfully!");
            }
            else
            {
                ClientMessaging("Record can not be updated!");
            }

        }
        else
        {
            populatebranch();
            string userID = Session["UserID"].ToString().Trim();
            int roleID = GetRole(userID);
            DataSet ds1 = new DataSet();
            if (roleID == 21 || roleID == 22)
            {
                using (ds1 = Get_BranchUser_Detail(Convert.ToString(Session["UserID"])).GetDataSet())
                {
                    if (ds1.Tables[0].Rows[0]["isUpdate"].ToString() == "0")
                    {
                        String script = String.Format("$('#dvBranchUpdate').fadeIn(1000);$('#dvBranch').fadeIn(500);");
                        Anthem.Manager.IncludePageScripts = true;
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSearchOffDiv", script, true);

                        txtName.Text = ds1.Tables[1].Rows[0]["name"].ToString();
                        Textmobile.Text = ds1.Tables[1].Rows[0]["mobileno"].ToString();
                        Textemail.Text = ds1.Tables[1].Rows[0]["email"].ToString();
                        ViewState["fk_roleid"] = Convert.ToInt32(ds1.Tables[1].Rows[0]["fk_roleid"]);
                        ViewState["fk_locid"] = Convert.ToString(ds1.Tables[1].Rows[0]["fk_locid"]);
                    }
                }
            }
        }
    }

    protected int GetRole(string userID)
    {
        DataSet ds = new DataSet();
        ds = GetRoleID(userID).GetDataSet();
        int roleID = Convert.ToInt32(ds.Tables[0].Rows[0]["fk_roleId"].ToString());
        return roleID;
    }

    public bool validate()
    {
        if (txtName.Text.Trim() == "")
        {

            ClientMessaging("Name is required!");
            txtName.Focus();
            return false;
        }

        if (Textmobile.Text.Trim() == "")
        {

            ClientMessaging("mobile number is required!");
            Textmobile.Focus();

            return false;
        }
        if (Textmobile.Text.Trim() != "" && Textmobile.Text.Trim().Length < 10)
        {
            ClientMessaging("Mobile number should be 10 digit!");
            Textmobile.Focus();

            return false;
        }
        if (Textemail.Text.Trim() == "")
        {
            ClientMessaging("Email is required!");
            Textemail.Focus();
            return false;
        }
        if (Textemail.Text.Trim() != "")
        {
            if (!IsValidEmailId(Textemail.Text))
            {
                ClientMessaging("Email is not valid!");
                Textemail.Focus();

                return false;
            }
        }

        if (D_ddlbranch.SelectedIndex == 0)
        {

            ClientMessaging("Branch Name is required!");
            D_ddlbranch.Focus();

            return false;
        }

        return true;
    }
  
    public static StoredProcedure UpdateInsertBranch(string name, string mobile, string email, int fk_branchid, string userid, int fk_roleid, string fk_locid)
    {
        SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("ACD_Update_Branch_Login", DataService.GetInstance("IUMSNXG"), "");
        sp.Command.AddParameter("@name", name, DbType.String);
        sp.Command.AddParameter("@mobile", mobile, DbType.String);
        sp.Command.AddParameter("@email", email, DbType.String);
        sp.Command.AddParameter("@fk_branchid", fk_branchid, DbType.Int32);
        sp.Command.AddParameter("@userid", userid, DbType.String);
        sp.Command.AddParameter("@fk_roleid", fk_roleid, DbType.Int32);
        sp.Command.AddParameter("@fk_locid", fk_locid, DbType.String);
        sp.CommandTimeout = 10000;
        return sp;
    }
    public static StoredProcedure Get_Branch()
    {
        SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("Get_Branch_Login", DataService.GetInstance("IUMSNXG"), "");
        return sp;
    }

    public StoredProcedure GetRoleID(string userID)
    {
        SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("GET_UserRoleId", DataService.GetInstance("IUMSNXG"), "");
        sp.Command.AddParameter("@pk_userId", userID, DbType.String);
        return sp;
    }

    public static StoredProcedure Get_BranchUser_Detail(string fk_userid)
    {
        SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("Get_BranchUser_Detail", DataService.GetInstance("IUMSNXG"), "");
        sp.Command.AddParameter("@fk_userid", fk_userid, DbType.String);
        sp.CommandTimeout = 10000;
        return sp;
    }

    //03-22-2023 Update Branch
}
