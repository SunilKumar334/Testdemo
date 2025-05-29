<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Loginpage.aspx.cs" Inherits="Loginpage" ViewStateEncryptionMode="Always" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <title>HPU :: Himachal Pradesh University :: UNIVERSITY ADMIN LOGIN</title>
    <script src="include/sha1.js" type="text/javascript"></script>
    <script src="include/utf8.js" type="text/javascript"></script>
    <script src="include/aes.js" type="text/javascript"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/crypto-js/3.1.2/rollups/aes.js"></script>
    <meta content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" name="viewport" />
    <script src="include/jquery.1.9.1.min.js"></script>
    <link href="include/bootstrap/bootstrap.css" rel="stylesheet" />
    <link href="include/bootstrap/font-awesome.min.css" rel="stylesheet" />
    <link href="include/bootstrap/style_outside-page.css" rel="stylesheet" />
    <script src="include/bootstrap/bootstrap.js"></script>
    <style>
        #captcha {
            width: 73%;
            margin-top: 5px;
        }
        .form-control.underline-input {color:#fff;}
    </style>
    <script type="text/javascript">
        // Broadcast that you're opening a page.
        localStorage.openpages = Date.now();
        //var onLocalStorageEvent = function (e) {
        //    if (e.key == "openpages") {
        //        // Listen if anybody else is opening the same page!
        //        localStorage.page_available = Date.now();
        //    }
        //    if (e.key == "page_available") {
        //        alert("Application is already running in another Tab.");
        //        open(location, '_self').close();
        //        close();
        //    }
        //};
        window.addEventListener('storage', onLocalStorageEvent, false);
    </script>
    <style type="text/css">
        .modal {
            position: fixed;
            top: 0;
            left: 0;
            background-color: black;
            z-index: 99;
            opacity: 0.8;
            filter: alpha(opacity=80);
            -moz-opacity: 0.8;
            min-height: 100%;
            width: 100%;
        }

        .loading {
            font-family: Arial;
            font-size: 10pt;
            border: 5px solid #67CFF5;
            width: 200px;
            height: 100px;
            display: none;
            position: fixed;
            background-color: White;
            z-index: 999;
        }

        .FixedHeader {
            position: absolute;
            font-weight: bold;
        }
    </style>

</head>
<script>
    function Check() {
        f.username.focus();
        var index = document.URL.lastIndexOf('?');
        //alert(parseInt(index));
        if (parseInt(index) > -1) {
            alert('Either you are not Authorized or Your Username/Password is invalid !!!');
            window.location.href = "Loginpage.aspx";
        }
    }
    function burstCache() {
        createCaptcha();
        if (!navigator.onLine) {
            document.body.innerHTML = 'Offline Viewing not Allowed...pls be online.';
            window.location = 'AppError.aspx';
        }

    }
</script>
<style>
    canvas {
        /*prevent interaction with the canvas*/
        pointer-events: none;
    }
</style>
<script type="text/javascript">
    function onsubmitclick() {
        debugger;
        if (validateCaptcha() == true) {
            //alert(document.getElementById("f"));
            var f = document.getElementById("f");
            if (f.username.value == "" || f.upassword.value == "") {
                alert("User Name and Password can not be blank !!!")
                if (f.username.value == "")
                    f.username.focus();
                else if (f.upassword.value == "")
                    f.upassword.focus();
                else

                    f.username.focus();
                return false;
            }


            var hashpass = Sha1.hash(f.upassword.value);
            //var salt = "" + Math.floor(Math.random() * 1111111111118 + 10000);
            var salt = "" + Math.floor(Math.random() * 899999999999 + 100000000000);
            var salt1, salt2;
            salt1 = salt.substring(0, 6);
            salt2 = salt.substring(6, 12);
            var Saltedhash = Sha1.hash(salt + hashpass);
            var Saltedhash = salt1 + Saltedhash + salt2;
            f.saltval.value = ''; //  Math.random().toString(36).substr(2, 16);
            f.hash.value = Saltedhash; //  Sha1.hash(f.salt.value + hashpass); //Saltedhash
            f.upassword.value = "";
            f.uname.value = f.username.value;
            try
            {
                 var key = CryptoJS.enc.Utf8.parse('<%=Session["Key"]%>');
                var iv = CryptoJS.enc.Utf8.parse('<%=Session["Key"]%>');
                f.hpass.value = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(f.hash.value), key,
                     {
                         keySize: 128 / 8,
                         iv: iv,
                         mode: CryptoJS.mode.CBC,
                         padding: CryptoJS.pad.Pkcs7
                     });
            }
            catch (err) {
                //alert(err);
            }

            }


    }



    var code;
    function createCaptcha() {
        //clear the contents of captcha div first 
        document.getElementById('captcha').innerHTML = "";
        var charsArray =
        "0123456789";
        var lengthOtp = 6;
        var captcha = [];
        for (var i = 0; i < lengthOtp; i++) {
            //below code will not allow Repetition of Characters
            var index = Math.floor(Math.random() * charsArray.length + 1); //get the next character from the array
            if (captcha.indexOf(charsArray[index]) == -1)
                captcha.push(charsArray[index]);
            else i--;
        }
        var canv = document.createElement("canvas");
        canv.id = "captcha";
        canv.width = 100;
        canv.height = 50;
        var ctx = canv.getContext("2d");
        ctx.font = "25px Georgia";
        ctx.strokeText(captcha.join(""), 0, 30);
        //storing captcha so that can validate you can save it somewhere else according to your specific requirements
        code = captcha.join("");
        document.getElementById("captcha").appendChild(canv); // adds the canvas to the body element
    }
    function validateCaptcha() {
        //event.preventDefault();
        //debugger
        if (document.getElementById("cpatchaTextBox").value == code) {

            return true;
        } else {

            f.username.value = "";
            f.upassword.value = "";
            alert("Invalid Captcha. try Again");


            return false;
            //createCaptcha();
        }
    }


    function DisableBackButton() {
        window.history.forward()
    }
    DisableBackButton();
    window.onload = DisableBackButton;
    window.onpageshow = function (evt) { if (evt.persisted) DisableBackButton() }
    window.onunload = function () { void (0) }
</script>
<body onload="burstCache();">
    <form runat="server" name="f" id="f" autocomplete="off">
        <div class="limiter">
            <div class="container-login100">
                <div class="wrap-login100">
                    <div class="login100-form validate-form text-center">
                        <div class="login-logo-right">
                            <a href="#">
                                <img class="img-fluid mx-auto" src="<%=Page.ResolveUrl("~/img/logo.png")%>"></a>
                        </div>
                        <span class="login100-form-title sub p-b-43">Enter Login Information
                        </span>
                        <%--<span class="login100-form-title sub text-center">Enter Login Information
                        </span>--%>
                        <div class="wrap-input100">
                            <input name="txtusername" type="text" maxlength="40" autocomplete="off" id="username" tabindex="0" class="input100" placeholder="Username">
                        </div>

                        <div class="wrap-input100">
                            <input name="txtpassword" type="password" maxlength="40" id="upassword" autocomplete="off" class="input100" placeholder="Password">
                           
                        </div>

                        <div class="form-group wrap-input100">
                            <div class="row">
                                <div class="col-md-5 col-xs-4">
                                    <div style="padding: 0 10px; background: #fff; border-radius: 10px; height: 45px;" id="captcha"></div>
                                </div>
                                <div class="col-md-2 col-xs-2">
                                    <div class="refresh-btn">
                                        <i id="imgCaptcha" style="font-size: 16px; color: #fff; margin-top: 7px;" onclick="createCaptcha()" class="fa fa-fw fa-refresh"></i>
                                    </div>
                                </div>
                                <div class="col-md-5 col-xs-6" style="margin-top: 7px;">
                                    <input type="text" id="cpatchaTextBox" class="form-control underline-input" />
                                </div>
                            </div>
                        </div>

                        <div class="form-group text-left ">
                            <Anthem:Button ID="btnLogin" runat="server" Style="margin: 10px 0 15px 0;" Text="Login" OnClick="imgLogin_Click"
                                AutoUpdateAfterCallBack="true" OnClientClick="return onsubmitclick();"
                                TextDuringCallBack="Wait..." CssClass="login100-form-btn" SkinID="none" EnableCallBack="false" />

                            <input type="hidden" runat="server" name="hash" id="hash" readonly autocomplete="off" />
                            <input type="hidden" runat="server" name="uname" id="uname" readonly autocomplete="off" />
                            <input type="hidden" runat="server" name="saltval" id="saltval" autocomplete="off" />
                            <input type="hidden" runat="server" name="hpass" id="hpass" autocomplete="off" />

                        </div>


                    </div>

                    <div class="login100-more" style="background-image: url('img/login-bg.jpg');">
                        <div class="back-to-home">
                                <%--<a href="#" class="txt1"><i class="fa fa-home" aria-hidden="true"></i> Back to Home </a>--%>
                            </div>
                        <div class="login-logo-left text-left">
                            <div class="leftlogin">
                                <h1 class="welcome-login-head colo">Welcome To HPU <br /><span style="color: #ffc107;font-weight: 500;">Admin Portal</span></h1>
                                <%--<h4>After single login admin can access below activities.</h4>
                                <ul class="list-group service-provide prov">
                                    <li><i class="fa fa-fw fa-caret-right"></i> HRMS & Payroll</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Pension Management</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Convocation</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Recruitment</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Asset & Estate Mgmt.</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Financial Accounts</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Bill Tracking System</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Research Mgmt. System</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> OSES</li> 
                                    <li><i class="fa fa-fw fa-caret-right"></i> Placement</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> User Management</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Store and Purchase</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Pre-Examination</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Grievance Mgmt.</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Health Center</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Budget Mgmt.</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Guest House</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Pre-Admission</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Pension Portal</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Legal Section</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> PF</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Student Alumni</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Exams</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Admin Activity</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Letter/Correspondence</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Admission & Academics</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Employee Portal</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Student Portal</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Tour & Travel</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> RTI</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Examination & Results</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> File Movement</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Miscellaneous Fee</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Conference</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> VC & Admin office</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Establishment</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Hostel Management</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> University Resource Center</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Transport and Fleet</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Leave Management</li>
                                    <li><i class="fa fa-fw fa-caret-right"></i> Digital Document Circulation (DDC)</li> 
                                     
                                </ul>--%>
                            </div>
                        </div>
                        <div class="footer1">
                            <a class="text-center" href="#">
                                <img src="img/iti.png"></a>
                            <%--<img class="pull-right" src="img/becil2.png">--%>
                        </div>
                    </div>

                </div>
            </div>
        </div>


         

        <!--divShow-->
        <div id="dvEnNo" class="white_content-new-1" style="display: none">
            <div class="popupboxouter">
                <div class="popupbox">
                    <div onclick="$('#rdbType').find('input:radio').each(function (i) { this.checked = false;});
                        $('#dvEnNo').fadeOut(1000);"
                        class="close-1">
                        X
                    </div>
                    <div style="display: block" id="dvEnroll">
                        <div class="table-responsive no-padding">
                            <table border="0" style="width: 100%; text-align: left;" class="table">
                                <tr>
                                    <td class="vtext">Enter OTP</td>
                                    <td class="colon">:</td>
                                    <td>
                                        <Anthem:TextBox ID="R_txtOtp" runat="server" SkinID="textboxlong" MaxLength="6" AutoUpdateAfterCallBack="True" />
                                    </td>
                                    <td colspan="3"></td>
                                </tr>
                                <tr>
                                    <td colspan="2"></td>

                                    <td colspan="2">
                                        <Anthem:Button ID="btnVerify" AutoUpdateAfterCallBack="true" runat="server" OnClick="btnVerify_Click"
                                            Text="VERIFY" PreCallBackFunction="btnSave_PreCallBack" TextDuringCallBack="WAIT..." />&nbsp;&nbsp;
                                    </td>
                                    <td colspan="2"></td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>


        </div>

        <!--end-->


    </form>



   <%-- <script src="include/jquery.js"></script>
    <script src="include/jquery.backstretch.min.js"></script>

    <script>
        $.backstretch("App_Themes/skyblue/Images/admin-bg.jpg", { speed: 500 });
    </script>--%>


</body>
</html>

