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
using System.Data.SqlClient;
using System.Xml;
using UMMBusinessLayer;
using System.Text;
using SubSonic;

public partial class MasterPage : System.Web.UI.MasterPage
{
    CommonFunction cobj = new CommonFunction();
    UMMBusinessLayer.UMMMaster UMobj = new UMMBusinessLayer.UMMMaster();
    UserAuthorization UAobj = new UserAuthorization();

    string RelUrl;
    string PageName;
    string FileDisplayName = "";
    string FileRealName = "", filename = "";

    protected void Page_Init()
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        Response.Cache.SetNoStore();
        Page.ViewStateUserKey = Session.SessionID;
        RelUrl = Request.ServerVariables["Script_Name"];
        PageName = RelUrl.Substring(RelUrl.LastIndexOf('/') + 1);
       // this.Page.UICulture = Session["language"].ToString();
        if (Session["ModuleID"] == null || Session["UserID"] == null || Session["RoleLevelID"] == null || Session["LoginName"] == "" )
        {
            Response.Redirect(Page.ResolveUrl("~/Logout.aspx?state=invalid"));
        }
        if (Session["ModuleID"].ToString() == "" || Session["UserID"].ToString() == "" || Session["RoleLevelID"].ToString() == "" || Session["LoginName"].ToString() == "")
        {
            Response.Redirect(Page.ResolveUrl("~/Logout.aspx?state=invalid"));
        }
        AutherizationCheck();
    }

    //By Anand on:July 11,2012
    void AutherizationCheck()
    {
        UMM_CHILDCLASSES.UMM_Child_UserSmtp_Dtls objj = new UMM_CHILDCLASSES.UMM_Child_UserSmtp_Dtls();
        if (Session["MappedAlias"].ToString() != "S")//If Logged user is not Software Manager,So we have to fetch page rights
        {
            string str = Request.RawUrl;

            int n = str.LastIndexOf('/');
            string pagename = str.Substring(n + 1, str.Length - n - 1);


            pagename = str.Substring(n + 1, str.Length - n - 1);
            if (pagename.Contains("?"))
            {
                string[] ss = pagename.Split('?');
                pagename = ss[0];

            }

            DataTable dtRights = (DataTable)Session["UserPages"];
            int chkflag = 0;



            //Pages not that are not entered in database but...opened as popup without any rights
            if (dtRights.Rows.Count > 0 && pagename.ToLower() != "Modules.aspx".ToLower() && pagename.ToLower() != "default.aspx".ToLower()
                && pagename.ToLower() != "ChangePassword.aspx".ToLower()
                && pagename.ToLower() != "admin_home.aspx" && pagename.ToLower() != "Acct_Stndbillpostqstring.aspx".ToLower()
                && pagename.ToLower().Contains("Salary_Posting.aspx".ToLower()) == true && pagename.ToLower() != "ASST_Issue_against_req_search.aspx".ToLower()
                && pagename.ToLower() != "ASST_req_search.aspx".ToLower() && pagename.ToLower() != "ASST_Issue_search.aspx".ToLower()
                && pagename.ToLower() != "ASST_Return_against_req_search.aspx".ToLower()
                && pagename.ToLower() != "ASST_Maintainence_Request_Detail.aspx".ToLower()
                && pagename.ToLower() != "ASST_Return_against_Maintenance_search.aspx".ToLower()
                && pagename.ToLower() != "ASST_Return_against_Maintenance_search.aspx".ToLower()
                && pagename.Contains("Acct_Treasury_SalaryBill_Posting.aspx".ToLower()) == true)
            {
                int fk_webpageid = 0;
                for (int a = 0; a < dtRights.Rows.Count; a++)
                {
                    if (pagename == dtRights.Rows[a]["webpagename"].ToString())
                    {
                        chkflag = 0;
                        fk_webpageid = Convert.ToInt32(dtRights.Rows[a]["fk_webpageid"]);
                        ViewState["hlpwpgid"] = fk_webpageid.ToString();
                        break;
                    }
                    else
                    {
                        chkflag = 1;
                        continue;
                    }
                }
                if (chkflag == 1)
                {
                    Response.Redirect(Page.ResolveUrl("~/Logout.aspx?state=invalid"));
                }
                if (fk_webpageid != 0)
                {

                    DataSet dsb = objj.GetBreadCumbsOn_Pageid(fk_webpageid);
                    StringBuilder sb = new StringBuilder("");
                    int z = dsb.Tables[0].Rows.Count;
                    for (int r = 0; r < dsb.Tables[0].Rows.Count; r++)
                    {
                        if (r == z - 1)
                            sb.Append(@"<strong>" + dsb.Tables[0].Rows[r]["menucaption"].ToString() + "</Strong>");
                        else
                            sb.Append("<span>" + dsb.Tables[0].Rows[r]["menucaption"].ToString() + "</span>&nbsp;>&nbsp;");
                    }
                    lrtBC.Text = sb.ToString();

                    dsb.Dispose();
                    dsb.Clear();
                }
            }

            //dtRights.Dispose();
            //dtRights.Clear();
        }
        else
        {
            string type = "";
            string str = Request.RawUrl;
            string pagename = "";
            int n = str.LastIndexOf('/');

            pagename = str.Substring(n + 1, str.Length - n - 1);
            //if (pagename.Contains("?"))
            //{
            //    string[] ss = pagename.Split('?');
            //    pagename = ss[0];
            //    type = ss[1];
            //}

            int mmid;
            if (Session["ModuleID"] == null || Session["UserID"] == null || Session["RoleLevelID"] == null || Session["LoginName"] == "" || Session["TempModID"] == null)
            {
                Response.Redirect(Page.ResolveUrl("~/Logout.aspx?state=invalid"));
            }
            string sssss = Session["TempModID"].ToString();
            if (Session["TempModID"].ToString() != "")
            {
               
                mmid = Convert.ToInt32(Session["TempModID"]);
                DataSet dsm = objj.GetBreadCumbsOn_Pagename_Moduleid(pagename, mmid);
               
                if (dsm.Tables[0].Rows.Count != 0)
                {
                    StringBuilder sb = new StringBuilder("");
                    int z = dsm.Tables[0].Rows.Count;
                    for (int r = 0; r < dsm.Tables[0].Rows.Count; r++)
                    {
                        if (r == z - 1)
                            sb.Append(@"<strong>" + dsm.Tables[0].Rows[r]["menucaption"].ToString() + "</Strong>");
                        else
                            sb.Append("<span>" + dsm.Tables[0].Rows[r]["menucaption"].ToString() + "</span>&nbsp;>&nbsp;");

                    }
                    lrtBC.Text = sb.ToString();

                    dsm.Dispose();
                    dsm.Clear();
                }
            }
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Header.DataBind();
        RelUrl = Request.ServerVariables["Script_Name"];
        PageName = RelUrl.Substring(RelUrl.LastIndexOf('/') + 1);
        string s = UMobj.pageoperationname_value;
        try
        {
            PopulateXMLMenu();
            if (!IsPostBack)
            {
                FillNotice_type();
                // lblCampus.Text = Session["LocationName"].ToString();
                lblLoginName.Text = Session["LoginName"].ToString();
                lblPLogin.Text = "LLT." + " - " + Session["PTime"].ToString();
                lblCLogin.Text = "CLT." + " - " + Session["CTime"].ToString();
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }

    private void PopulateXMLMenu()
    {
        DataSet ds = new DataSet();
        string ModID = "";

        if (Request.QueryString["MID"] != null)
        {

            crypto cpt = new crypto();
            string arr1 = cpt.DecodeString(Request.QueryString["MID"].ToString());

            ModID = arr1;


            Session["TempModID"] = ModID;
        }

        UMobj.useridvalue = Session["UserID"].ToString();
        UMobj.moduleIDvalue = Session["TempModID"].ToString();
        UMobj.fk_locid = Session["LocationID"].ToString();
        ds = UMobj.GetWebPagesOnUserID();
        ds.DataSetName = "Menus";

        ds.Tables[0].TableName = "Menu";
        DataRelation relation = new DataRelation("ParentChild",
                ds.Tables["Menu"].Columns["pk_webpageId"],
                ds.Tables["Menu"].Columns["parentId"],
                true);

        relation.Nested = true;
        ds.Relations.Add(relation);

        if (ds.Tables[0].Rows.Count > 0)
        {
            XmlDataSource x = new XmlDataSource();
            x.Data = ds.GetXml();

            XmlDataSource1.Data = ds.GetXml();
            MainMenu.DataSource = XmlDataSource1;
            MainMenu.DataBind();
            lblCheck.Visible = true;
            lblCheck.Text = "";
        }
        else
        {
            MainMenu.Visible = false;
        }

        ds.Dispose();
        ds.Clear();
    }

    /// <summary>
    /// Handles the DataBound event of the MainMenu control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void MainMenu_DataBound(object sender, EventArgs e)
    {
        for (int i = 0; i < this.MainMenu.Items.Count; i++)
        {

            MainMenu.Items[i].Selectable = false;

        }

    }

    protected void rptNotietype_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            LinkButton file = e.Item.FindControl("LnkBtnDownload") as LinkButton;
            HtmlGenericControl x = (HtmlGenericControl)e.Item.FindControl("Downimg");
            string ss = file.CommandArgument.ToString();
            if (ss == "")
            {
                file.Visible = false;
                x.Visible = false;
            }
            else
            {
                file.Visible = true;
                x.Visible = true;
            }
        }
    }

    protected void rptNotietype_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        
        if (e.CommandName.ToString().Trim() == "Page")
        {
            filename = e.CommandArgument.ToString();
            if (filename != "")
            {
                LinkButton lblbtn = (LinkButton)e.Item.FindControl("LnkBtnDownload");
                getFileName(filename);
            }
        }
    }
    public string ReturnPath()
    {
        try
        {
            string host = HttpContext.Current.Request.Url.Host;
            DataSet dsFilepath = new DataSet();
            dsFilepath.ReadXml(HttpContext.Current.Server.MapPath("~/IO_Config_Exam.xml"));
            foreach (DataRow dr in dsFilepath.Tables[0].Rows)
            {
                if (host == dr["Server_Ip"].ToString().Trim())
                {
                    return dr["http_Add"].ToString().Trim();

                }
            }
            return "";
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    protected void getFileName(string FileName)
    {

        string FileUrl = ReturnPath();

        if (FileName.Contains("/"))
        {
            FileDisplayName = FileName.Substring(0, FileName.IndexOf("/"));

            FileRealName = ReturnPath() + "/Notifications/" + FileName.Substring(FileName.IndexOf("/") + 1);

        }
        FileUrl = FileUrl + FileName;




        StringBuilder sb = new StringBuilder();
        sb.Append("<script type = 'text/javascript'>");
        sb.Append("window.open('");
        sb.Append(FileRealName);
        sb.Append("');");
        sb.Append("</script>");
        Page.ClientScript.RegisterStartupScript(this.GetType(),
                "script", sb.ToString());



        //   Response.Write("<script>window.open('" + FileRealName + "','_blank');</script>");

        // return "<a target='_blank' href=" + FileRealName + ">" + FileDisplayName + "</a>";

    }

    private void FillNotice_type()
    {
        DataSet Dsa = FillNoticetype().GetDataSet();

        rptNotietype.DataSource = Dsa.Tables[0];
        rptNotietype.DataBind();

       

    }
    public static StoredProcedure FillNoticetype()
    {
        SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("Portal_CMS_Notification_SP", DataService.GetInstance("IUMSNXG"), "");
        sp.Command.AddParameter("@Mode", 5, DbType.Int32);
        sp.CommandTimeout = 10000;
        return sp;
    }

    //public virtual DataSet GetBreadCumbsOn_Pagename_Moduleid(string Pagename, int Moduleid)
    //{
    //    this.ClearAll();
    //    ArrNames.Add("@webpagename");
    //    ArrValues.Add(Pagename);
    //    ArrTypes.Add(SqlDbType.VarChar);

    //    ArrNames.Add("@Fk_moduleid");
    //    ArrValues.Add(Moduleid);
    //    ArrTypes.Add(SqlDbType.Int);

    //    DataSet dss = IUMSNXG.SP.SMS_SP_Executor("UM_Sp_GetBreadCumbs_onPagename_Moduleid", ArrValues, ArrNames, ArrTypes).GetDataSet();
    //    return dss;
    //}

    //protected void hplBtn_Click(object sender, ImageClickEventArgs e)
    //{

    //    string script = string.Format("ShowSearchmaster();", "");
    //    Anthem.Manager.IncludePageScripts = true;
    //    Page.ClientScript.RegisterStartupScript(this.GetType(), "errmsg", script, true); string str = Request.RawUrl;

    //    int n = str.LastIndexOf('/');
    //    string pagename = str.Substring(n + 1, str.Length - n - 1);
    //    if (Session["TempModID"].ToString() != "")
    //    {
    //        int moduleid = Convert.ToInt32(Session["TempModID"]);
    //        IDataReader rdr = IUMSNXG1.SPs.HlpEng_sp_GethelpContent(moduleid, pagename).GetReader();
    //        DataTable dt = new DataTable();
    //        dt.Load(rdr);
    //        if (dt.Rows.Count != 0)
    //            ltrlHelp.Text = dt.Rows[0]["HelpContent"].ToString();
    //    }
    //    else
    //    {
    //        ltrlHelp.Text = "";
    //    }

    //}
}
