<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Modules.aspx.cs" Inherits="Modules" MasterPageFile="~/UMM/MasterPage.master" %>

<%@ Register TagPrefix="cc1" Namespace="EeekSoft.Web" Assembly="EeekSoft.Web.PopupWin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <%-- <script src="include/jquery.min.js"></script>
    <script src="include/js/bootstrap.min.js"></script>--%>

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.2/js/bootstrap.min.js"></script>
    <link href="Exam_Monitoring/js/bootstrap.min.css" rel="stylesheet" />
    <%--   <script>
        $(window).load(function () {
            $('#myModal').modal('show');
        });
    </script>--%>
    <script language="javascript" type='text/JavaScript'>
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

    </script>
    <script type="text/javascript">
        function ShowPopup() {
            //$("#myModal .modal-title").html(title);
            //$("#myModal .modal-body").html(body);
            $("#myModal").modal('show');
        }
    </script>
    <script type="text/javascript">

        function preventBack() { window.history.forward(); }
        setTimeout("preventBack()", 0);
        window.onunload = function () { null };
    </script>
    <style>
        .module-list tr td {
            width: 20%;
        }

            .module-list tr td table tr td {
                width: 100%;
            }


        .examform123 {
            margin-top: -17px;
            margin-left: 226px;
        }
    </style>

    <%--<link rel="stylesheet" href='<%=Page.ResolveUrl("~/css/hoverbox.css")%>' type="text/css" media="screen, projection" />--%>

    <div class="moduleselectmenu">
        <h1>
            <span class="borderbottom">MODULES ASSIGNED</span>
            <div class="modulelistfild">
                <asp:DropDownList AutoPostBack="true"
                    ID="ddlloc" runat="server" OnSelectedIndexChanged="ddlloc_SelectedIndexChanged" Visible="false" />
            </div>
        </h1>
    </div>

    <%-- <div class="help-line-no">
        <center><b>Helpline : +91-9599664730, 01681-241031 (11:00 AM TO 5:00 PM (Mon - Sat))</b></center>
    </div>--%>
    <div class="module-main">
        <div id="div1" runat="server" visible="false">
            <fieldset class="fieldset-border">
                <legend>
                    <Anthem:Label ID="lblType1" runat="server" AutoUpdateAfterCallBack="True" /></legend>
                <asp:DataList ID="DataList1" CssClass="module-list" runat="server" RepeatColumns="4" Width="100%" AutoUpdateAfterCallBack="true">
                    <ItemTemplate>
                        <div class="pattern-1__single c<%# Container.ItemIndex + 1 %>">
                            <div class="pattern-1__icon">

                                <img src="img/<%# DataBinder.Eval(Container.DataItem, "moduleImage")%>" />

                            </div>
                            <div class="pattern-1__content">
                                <h3 class="pattern-1__title">
                                    <a class="m-heading" href="UMM/Admin_Home.aspx?MID=<%# DataBinder.Eval(Container.DataItem, "enc") %>">
                                        <%# DataBinder.Eval(Container.DataItem, "modulename")%> </a>
                                </h3>
                                <p class="pattern-1__text">View Module Detail</p>
                            </div>

                            <a href="UMM/Admin_Home.aspx?MID=<%# DataBinder.Eval(Container.DataItem, "enc") %>" class="thm-btn">View <i class="fa fa-angle-right"></i></a>
                            <div class="pattern-1__star-1">
                                <img src="img/feature-two-star-1.png" alt="">
                            </div>
                            <div class="pattern-1__star-2">
                                <img src="img/feature-two-star-2.png" alt="">
                            </div>
                            <div class="pattern-1__star-3">
                                <img src="img/feature-two-star-3.png" alt="">
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:DataList>
            </fieldset>
        </div>

        <div id="div2" runat="server" visible="false">
            <fieldset class="fieldset-border">
                <legend>
                    <Anthem:Label ID="lblType2" runat="server" AutoUpdateAfterCallBack="True" /></legend>
                <asp:DataList ID="DataList2" runat="server" CssClass="module-list" RepeatColumns="5" Width="100%" AutoUpdateAfterCallBack="true">
                    <ItemTemplate>
                        <div class="module_icon_main_div card c<%# Container.ItemIndex + 4 %>">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <div class="modulename">
                                            <a class="m-heading" href="UMM/Admin_Home.aspx?MID=<%# DataBinder.Eval(Container.DataItem, "enc") %>">
                                                <%# DataBinder.Eval(Container.DataItem, "modulename")%>
                                            </a>
                                            <a class="m-text" href="UMM/Admin_Home.aspx?MID=<%# DataBinder.Eval(Container.DataItem, "enc") %>">View Module Detail</a>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="moduleimage">
                                            <a href="UMM/Admin_Home.aspx?MID=<%# DataBinder.Eval(Container.DataItem, "enc") %>">
                                                <img src="img/<%# DataBinder.Eval(Container.DataItem, "moduleImage")%>" />
                                            </a>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ItemTemplate>
                </asp:DataList>
            </fieldset>
        </div>

        <div id="div3" runat="server" visible="false">
            <fieldset class="fieldset-border">
                <legend>
                    <Anthem:Label ID="lblType3" runat="server" AutoUpdateAfterCallBack="True" /></legend>
                <Anthem:DataList ID="DataList3" CssClass="module-list" runat="server" RepeatColumns="5" Width="100%" AutoUpdateAfterCallBack="true">
                    <ItemTemplate>
                        <div class="module_icon_main_div card c<%# Container.ItemIndex + 10 %>">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <div class="modulename">
                                            <a class="m-heading" href="UMM/Admin_Home.aspx?MID=<%# DataBinder.Eval(Container.DataItem, "enc") %>">
                                                <%# DataBinder.Eval(Container.DataItem, "modulename")%>
                                            </a>
                                            <a class="m-text" href="UMM/Admin_Home.aspx?MID=<%# DataBinder.Eval(Container.DataItem, "enc") %>">View Module Detail</a>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="moduleimage">
                                            <a href="UMM/Admin_Home.aspx?MID=<%# DataBinder.Eval(Container.DataItem, "enc") %>">
                                                <img src="img/<%# DataBinder.Eval(Container.DataItem, "moduleImage")%>" />
                                            </a>
                                        </div>
                                    </td>
                                </tr>

                            </table>
                    </ItemTemplate>
                </Anthem:DataList>
            </fieldset>
        </div>

        <div id="div4" runat="server" visible="false">
            <fieldset class="fieldset-border">
                <legend>
                    <Anthem:Label ID="lblType4" runat="server" AutoUpdateAfterCallBack="True" /></legend>
                <Anthem:DataList ID="DataList4" CssClass="module-list" runat="server" RepeatColumns="5" Width="100%" AutoUpdateAfterCallBack="true">
                    <ItemTemplate>
                        <div class="module_icon_main_div card c<%# Container.ItemIndex + 1 %>">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <div class="modulename">
                                            <a class="m-heading" href="UMM/Admin_Home.aspx?MID=<%# DataBinder.Eval(Container.DataItem, "enc") %>">
                                                <%# DataBinder.Eval(Container.DataItem, "modulename")%>
                                            </a>
                                            <a class="m-text" href="UMM/Admin_Home.aspx?MID=<%# DataBinder.Eval(Container.DataItem, "enc") %>">View Module Detail</a>
                                        </div>

                                    </td>
                                    <td>
                                        <div class="moduleimage">
                                            <a href="UMM/Admin_Home.aspx?MID=<%# DataBinder.Eval(Container.DataItem, "enc") %>">
                                                <img src="img/<%# DataBinder.Eval(Container.DataItem, "moduleImage")%>" />
                                            </a>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ItemTemplate>
                </Anthem:DataList>
            </fieldset>
        </div>

    </div>


    <%-- <table border="0" class="modulebody" align="center" cellpadding="0" cellspacing="0" width="100%">
       
        <tr>
     <td style="padding-top:10px; padding-left:88px;">
           <table border="0"  cellpadding="0" cellspacing="0">
                        <tr class="hoverbox">
                            <td align="center">
                            <div class="modulebg">
                            <div class="moduleimg">
                                <a href="http://182.18.173.148/ssp/index.aspx" target="_blank">
                                    <img height="10px" width="10px" src="img/hrms.png" />
                                </a>
                                </div>
                                <div class="modulename">
                                <a href="http://182.18.173.148/ssp/index.aspx" target="_blank">
                                    Employee Portal
                                </a>
                                </div>
                                </div>
                                
                            </td>
                       <%-- </tr>
                         <tr class="hoverbox">
                            <td align="center">
                            <div class="modulebg">
                            <div class="moduleimg">
                                <a href="http://182.18.173.148/ssp/index.aspx" target="_blank">
                                    <img height="10px" width="10px" src="img/StudentPortal.png" />
                                </a>
                                </div>
                                <div class="modulename">
                                <a href="http://182.18.173.148/ssp/index.aspx" target="_blank">
                                    Student Portal
                                </a>
                                </div>
                                </div>
                                
                            </td>
                        </tr>
                    </table>
        </td>
    </tr>
    </table> --%>



    <div class="modal" tabindex="-1" role="dialog" id="myModal">
        <div class="modal-dialog" role="document" style="margin-left: 200px;">
            <div class="modal-content" style="overflow-y: auto; width: 916px">
                <div class="modal-header">
                    <h4 class="modal-title">RTI Application (New)</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" style="color: red;">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body" style="max-height: 500px; overflow: auto;">
                    <Anthem:Repeater ID="rptCustomers" runat="server">
                        <HeaderTemplate>
                            <table class="table table-bordered" style="width: 100%;">
                                <tr>
                                    <th>Sr No</th>
                                    <th>Application No</th>
                                    <th>Applicant Name</th>
                                    <th>Related To Department</th>
                                    <th>RTI Subject</th>
                                    <th>Date of Application</th>
                                    <th>Application Received Date</th>
                                    <th>Forward  To Department</th>
                                    <th>Reply</th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Container.ItemIndex + 1 %></td>
                                <td>

                                    <asp:Label ID="lbl_appno" runat="server" Text='<%# Eval("appno") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="lbl_applicantname" runat="server" Text='<%# Eval("applicantname") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="lbl_rel_depart" runat="server" Text='<%# Eval("departmentname") %>' />
                                </td>

                                <td>
                                    <asp:Label ID="lbl_rtisubje" runat="server" Text='<%# Eval("rtisubj") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="lbl_appdate" runat="server" Text='<%# Eval("appdate") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="lbl_receiveddate" runat="server" Text='<%# Eval("receiveddate") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="Lbl_forwarddep" runat="server" Text='<%# Eval("Description") %>' />
                                </td>
                                <td>
                                    <a href="RTI/RTI_Application_ReplybyDept.aspx">
                                        <img src="../Images/Edit.gif" alt="" border="0"></img></a></td>

                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </Anthem:Repeater>
                </div>
                <div class="modal-footer">
                    <%-- <button type="button" class="btn btn-primary">Save changes</button>--%>
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div id="dvEnNoEnroll" class="white_content-new-1" style="display: none">
        <div class="popupboxouter">
            <div class="popupbox" style="width: 864px">
                <div onclick="$('#dvEnNoEnroll').fadeOut(1000);" class="close-1" id="dvClose" runat="server">
                    X
                </div>
                <div style="display: none; width: 100%!important; height: 400px!important" id="dvEnrollment">
                    <table class="table" style="padding: 0px; margin: 0px; border: 0px; width: 100%;">
                        <tr>
                            <td colspan="7">
                                <table class="table" style="padding: 0px; margin: 0px; border: 0px; width: 100%;">
                                    <tr>
                                        <td class="tableheading" colspan="3">College Detail Update </td>
                                        <td class="tableheading" colspan="3">
                                            <Anthem:Label ID="lblCollegeN" runat="server" SkinID="lblmessage" AutoUpdateAfterCallBack="true" />
                                        </td>

                                    </tr>
                                    <tr>
                                        <td colspan="6">
                                            <Anthem:Label ID="lblCollege" runat="server" SkinID="lblmessage" AutoUpdateAfterCallBack="true" Style="font-weight: bold; font-size: 15px; color: blue" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="vtext">Mobile
                                        </td>
                                        <td class="colon">:</td>
                                        <td class="required">


                                            <Anthem:TextBox runat="server" CssClass="form-control input-sm" ID="txtMobile" AutoUpdateAfterCallBack="true" onkeypress="return isNumberKey(event)" onkeydown="return NumCheck(this)" onpaste="event.returnValue=false"
                                                ondrop="event.returnValue=false" MaxLength="10"></Anthem:TextBox><p class="examform123">*</p>


                                        </td>

                                        <td class="vtext">Email
                                        </td>
                                        <td class="colon">:</td>
                                        <td class="required">

                                            <Anthem:TextBox ID="txtEmail" runat="server" AutoUpdateAfterCallBack="true" SkinID="textboxmedium"></Anthem:TextBox><p class="examform123">*</p>

                                        </td>

                                    </tr>
                                    <tr>
                                        <td class="vtext">Principal Name
                                        </td>
                                        <td class="colon">:</td>
                                        <td class="required">

                                            <Anthem:TextBox ID="txtPrincName" runat="server" AutoUpdateAfterCallBack="true" SkinID="textboxmedium"></Anthem:TextBox>

                                        </td>
                                        <td class="vtext">Contact No
                                        </td>
                                        <td class="colon">:</td>
                                        <td class="required">
                                            <Anthem:TextBox runat="server" CssClass="form-control input-sm" ID="txtContact" AutoUpdateAfterCallBack="true" onkeypress="return isNumberKey(event)" onkeydown="return NumCheck(this)" onpaste="event.returnValue=false"
                                                ondrop="event.returnValue=false" MaxLength="12"></Anthem:TextBox>
                                            <p class="examform123">*</p>
                                            <br />



                                        </td>
                                    </tr>

                                    <tr>
                                        <td colspan="3"></td>
                                        <td>
                                            <Anthem:Button ID="btn_Classroll" runat="server" OnClick="btn_Classroll_Click" OnClientClick="Closepopup()" AutoUpdateAfterCallBack="true" Text="UPDATE"></Anthem:Button>
                                            <Anthem:Label ID="lblMsg" runat="server" SkinID="lblmessage" AutoUpdateAfterCallBack="true" />

                                        </td>

                                    </tr>
                                </table>
                            </td>
                        </tr>

                    </table>
                </div>
            </div>
        </div>
    </div>

    <%--    Changes 01-03-2023 Update Branch--%>
    <div id="dvBranchUpdate" class="white_content-new-1" style="display: none">
        <div class="popupboxouter">
            <div class="popupbox" style="width: 864px">
                <%--        <div onclick="$('#dvBranchUpdate').fadeOut(1000);" class="close-1" id="dvClose1" runat="server">
                    X
                </div>--%>
                <div style="display: none; width: 100%!important; height: 400px!important" id="dvBranch">
                    <table class="table" style="padding: 0px; margin: 0px; border: 0px; width: 100%;">
                        <tr>
                            <td colspan="7">
                                <table class="table" style="padding: 0px; margin: 0px; border: 0px; width: 100%;">
                                    <tr>
                                        <td class="tableheading" colspan="3">Branch  Update </td>
                                        <td class="tableheading" colspan="3">
                                            <Anthem:Label ID="lblBranchN" runat="server" SkinID="lblmessage" AutoUpdateAfterCallBack="true" />
                                        </td>

                                    </tr>
                                    <tr>
                                        <td colspan="6">
                                            <Anthem:Label ID="lblBranch" runat="server" SkinID="lblmessage" AutoUpdateAfterCallBack="true" Style="font-weight: bold; font-size: 15px; color: blue" />
                                        </td>
                                    </tr>

                                    <tr>

                                        <td class="vtext">Name
                                        </td>
                                        <td class="colon">:</td>
                                        <td class="required">
                                            <Anthem:TextBox ID="txtName" runat="server" AutoUpdateAfterCallBack="true" SkinID="textboxmedium"></Anthem:TextBox>
                                        </td>

                                        <td class="vtext">Mobile
                                        </td>
                                        <td class="colon">:</td>
                                        <td class="required">
                                            <Anthem:TextBox runat="server" CssClass="form-control input-sm" ID="Textmobile" AutoUpdateAfterCallBack="true" onkeypress="return isNumberKey(event)" onkeydown="return NumCheck(this)" onpaste="event.returnValue=false"
                                                ondrop="event.returnValue=false" MaxLength="10"></Anthem:TextBox>
                                            *
                                        </td>
                                    </tr>

                                    <tr>

                                        <td class="vtext">Email
                                        </td>
                                        <td class="colon">:</td>
                                        <td class="required">
                                            <Anthem:TextBox ID="Textemail" runat="server" AutoUpdateAfterCallBack="true" SkinID="textboxmedium"></Anthem:TextBox>
                                            *

                                        </td>
                                        <td class="vtext">Branch</td>
                                        <td class="colon">:</td>
                                        <td class="required">
                                            <Anthem:DropDownList ID="D_ddlbranch" runat="server" SkinID="dropdownlong" AutoUpdateAfterCallBack="true"
                                                AutoCallBack="true" />
                                            *
                                        </td>
                                    </tr>

                                    <tr>
                                        <td colspan="3"></td>
                                        <td>
                                            <Anthem:Button ID="btnupdate" runat="server" OnClick="btnupdate_Click" OnClientClick="Closepopup()" AutoUpdateAfterCallBack="true" Text="UPDATE"></Anthem:Button>
                                            <Anthem:Label ID="LabelM" runat="server" SkinID="lblmessage" AutoUpdateAfterCallBack="true" />

                                        </td>

                                    </tr>
                                </table>
                            </td>
                        </tr>

                    </table>
                </div>
            </div>
        </div>
    </div>

    <%--    Changes 01-03-2023--%>


    <cc1:PopupWin ID="popupWindow" Visible="false" Title="Quick Message" Style="text-align: left"
        runat="server" ShowLink="false" Width="300px" Height="250px" TextColor="0, 50, 0"
        Shadow="160, 180, 140" ColorStyle="green" DockMode="BottomRight" DragDrop="true"
        HideAfter="15000" Message="No Message to show in popup"></cc1:PopupWin>
</asp:Content>
