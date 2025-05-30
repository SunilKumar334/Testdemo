<%@ Page Language="C#" MasterPageFile="~/UMM/MasterPage.master" AutoEventWireup="true"
    CodeFile="ChangePassword.aspx.cs" Inherits="UMM_ChangePassword"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    

    <style type="text/css">
        .numberlist {
            width: 450px;
            margin-top: 16px;

        }

            .numberlist ol {
                counter-reset: li;
                list-style: none;
                *list-style: decimal;
                font: 15px trebuchet ms,lucida sans;
                padding: 0;
                margin-bottom: 4em;
            }

                .numberlist ol li {
                    font-size: 13px;
                }

            .numberlist a {
                position: relative;
                display: block;
                padding: .1em .4em .4em 2em;
                *padding: .4em;
                margin: .5em 0;
                background: #fff;
                color: #444;
                text-decoration: none;
                -moz-border-radius: .3em;
                -webkit-border-radius: .3em;
                border-radius: .3em;
            }

                .numberlist a:before {
                    content: counter(li);
                    counter-increment: li;
                    position: absolute;
                    left: -1.3em;
                    top: 50%;
                    margin-top: -1.3em;
                    background: #87ceeb;
                    height: 20px;
                    width: 20px;
                    line-height: 20px;
                    border: .3em solid #fff;
                    text-align: center;
                    font-weight: 700;
                    -moz-border-radius: 2em;
                    -webkit-border-radius: 2em;
                    border-radius: 2em;
                    color: #fff;
                }
    </style>
    <script src="../include/sha1.js" type="text/javascript"></script>
    <script src="../include/aes.js" type="text/javascript"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/crypto-js/3.1.2/rollups/aes.js"></script>
    <%--   <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/crypto-js/3.1.2/rollups/aes.js"></script>--%>

    <script src="../include/utf8.js" type="text/javascript"></script>
    <script language="javascript">
        function PasswordCheck() {
            var pass = document.getElementById("ctl00_ContentPlaceHolder1_R_txtPwd");
            var pass1 = document.getElementById("ctl00_ContentPlaceHolder1_R_txtRPassword");
            if (pass.value != pass1.value && pass1.value != "") {
                alert("Password & Confirm Password must be same");
                pass1.value = "";
                pass1.focus();
            }
        }
        function onsubmitclick() {
            debugger;
            if (document.getElementById('ctl00_ContentPlaceHolder1_R_txtOldPassword').value == "") {
                alert("Old Password is required!")
                document.getElementById('ctl00_ContentPlaceHolder1_R_txtOldPassword').focus();
                return false;
            }
            if (document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').value == "") {
                document.getElementById('ctl00_ContentPlaceHolder1_hash').value = null;
                document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').focus();
                alert("New Password is required!")
                return false;
            }
            if (document.getElementById('ctl00_ContentPlaceHolder1_R_txtRPassword').value == "") {
                alert("Confirm New Password is required!")
                document.getElementById('ctl00_ContentPlaceHolder1_R_txtRPassword').focus();
                return false;
            }


            if (analyzePassword() == false)
            {
                alert("invalid password !")
                return false;
            }

            var password_strength = document.getElementById("password_strength");
            if (password_strength.innerHTML == "Weak") {
                alert("invalid password !")
                return false;
            }

            var hashpass = Sha1.hash(document.getElementById('ctl00_ContentPlaceHolder1_R_txtOldPassword').value);
            var hash = Sha1.hash(document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').value);

       
            var salt = '<%=Session["Randomsalt"]%>' //"" + Math.floor(Math.random() * 1111111111118 + 10000);
            var hashOldPwd = Sha1.hash(hashpass + salt);

            //document.getElementById('ctl00_ContentPlaceHolder1_hdSalt').value = salt;
            document.getElementById('ctl00_ContentPlaceHolder1_hdpwd').value = hashOldPwd; // hashpass;
            document.getElementById('ctl00_ContentPlaceHolder1_hash').value = Sha1.hash(document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').value);
            document.getElementById('ctl00_ContentPlaceHolder1_hdOldPass').value = hashOldPwd //Sha1.hash(document.getElementById('ctl00_ContentPlaceHolder1_R_txtOldPassword').value);
            document.getElementById('ctl00_ContentPlaceHolder1_hdnewpass').value = Sha1.hash(document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').value);
            document.getElementById('ctl00_ContentPlaceHolder1_hdconfirmPass').value = Sha1.hash(document.getElementById('ctl00_ContentPlaceHolder1_R_txtRPassword').value);

            //document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').value = ""; 
           // document.getElementById('ctl00_ContentPlaceHolder1_R_txtRPassword').value = ""; 
          //  document.getElementById('ctl00_ContentPlaceHolder1_R_txtOldPassword').value = ""; //
            // debugger;
            try
            {
                const message = document.getElementById("p01");
                var key = CryptoJS.enc.Utf8.parse('<%=Session["Key"]%>');
                var iv = CryptoJS.enc.Utf8.parse('<%=Session["Key"]%>');

         

                //var key = CryptoJS.enc.Utf8.parse('1234567890123456');
                //var iv = CryptoJS.enc.Utf8.parse('1234567890123456');

                var encryptedNewPass = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').value), key,
                    {
                        keySize: 128 / 8,
                        iv: iv,
                        mode: CryptoJS.mode.CBC,
                        padding: CryptoJS.pad.Pkcs7
                    });
                var encryptedoldPass = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(document.getElementById('ctl00_ContentPlaceHolder1_R_txtOldPassword').value), key,
                    {
                        keySize: 128 / 8,
                        iv: iv,
                        mode: CryptoJS.mode.CBC,
                        padding: CryptoJS.pad.Pkcs7
                    });

                var encryptedconfirmPass = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(document.getElementById('ctl00_ContentPlaceHolder1_R_txtRPassword').value), key,
                  {
                      keySize: 128 / 8,
                      iv: iv,
                      mode: CryptoJS.mode.CBC,
                      padding: CryptoJS.pad.Pkcs7
                  });
            }
            catch (err) {
                message.innerHTML = "Input is " + err;
            }

            debugger;
            //var descrypt = CryptoJS.AES.decrypt(encryptedlogin.toString(), key.toString(), {
            //    iv: iv.toString(),
            //    mode: CryptoJS.mode.CBC,
            //    padding: CryptoJS.pad.Pkcs7
            //}).toString();
            document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').value = encryptedNewPass; //Sha1.hash(document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').value);
            document.getElementById('ctl00_ContentPlaceHolder1_R_txtRPassword').value = encryptedoldPass; //Sha1.hash(document.getElementById('ctl00_ContentPlaceHolder1_R_txtRPassword').value);
            document.getElementById('ctl00_ContentPlaceHolder1_R_txtOldPassword').value = encryptedconfirmPass; //  Sha1.hash(document.getElementById('ctl00_ContentPlaceHolder1_R_txtOldPassword').value);
           // document.getElementById('ctl00_ContentPlaceHolder1_hfEncrptedPass').value = encryptedlogin.toString();

        }
        function setval() {

            if (document.getElementById('ctl00_ContentPlaceHolder1_R_txtOldPassword').value == "") {
                alert("Old Password is required!")
                document.getElementById('ctl00_ContentPlaceHolder1_R_txtOldPassword').focus();
                return false;
            }
            if (document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').value == "") {
                document.getElementById('ctl00_ContentPlaceHolder1_hash').value = null;
                document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').focus();
                alert("New Password is required!")
                return false;
            }
            if (document.getElementById('ctl00_ContentPlaceHolder1_R_txtRPassword').value == "") {
                alert("Confirm New Password is required!")
                document.getElementById('ctl00_ContentPlaceHolder1_R_txtRPassword').focus();
                return false;
            }

            //document.getElementById('ctl00_ContentPlaceHolder1_hash').value = Sha1.hash(document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').value);
            //document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').value = '';
            //document.getElementById('ctl00_ContentPlaceHolder1_R_txtOldPassword').value = '';
            //document.getElementById('ctl00_ContentPlaceHolder1_R_txtRPassword').value = '';
        }
        //function checkpolicy() {

        //    var pass = document.getElementById("ctl00_ContentPlaceHolder1_R_txtPwd");
        //    var Str = document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').value;
        //    var len = document.getElementById('ctl00_ContentPlaceHolder1_MinLength').value;
        //    if (document.getElementById('ctl00_ContentPlaceHolder1_Allow_Pass_equal_userid').value == "True") {
        //        if (document.getElementById('ctl00_ContentPlaceHolder1_username').value == Str) {
        //            alert("Login Name & Password can not be same");
        //            pass.value = "";
        //            pass.focus();
        //            return false;
        //        }

        //    }
        //    if (Str.length < len) {
        //        alert("New password should have minimum " + len + " Characters");
        //        pass.value = "";
        //        pass.focus();
        //        return false;
        //    }
        //    if (document.getElementById('ctl00_ContentPlaceHolder1_MinNumericChar').value == "True") {
        //        if (!/\d/.test(Str)) //For one Numeric Case
        //        {
        //            alert("New password should Contain a Numeric Value");
        //            pass.value = "";
        //            pass.focus();
        //            return false;
        //        }
        //    }
        //    if (document.getElementById('ctl00_ContentPlaceHolder1_MinUpperCaseChar').value == "True") {
        //        if (!/[A-Z]/.test(Str)) //For one Upper Case
        //        {
        //            alert("New password should Contain a Upper Case Character");
        //            pass.value = "";
        //            pass.focus();
        //            return false;
        //        }
        //    }

        //    if (document.getElementById('ctl00_ContentPlaceHolder1_MinSpecialChar').value == "True") {
        //        if (!/[-!@$%^&*()_+|~=`{}\[\]:";'<>?,.\/]/.test(Str)) {
        //            alert("New password should Contain Special Character");
        //            pass.value = "";
        //            pass.focus();
        //            return false;
        //        }
        //    }

        //}


        //function analyzePassword() {
        //    var password_strength = document.getElementById("password_strength");
        //    var Str = document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').value;

        //    //TextBox left blank.
        //    if (Str.length == 0) {
        //        password_strength.innerHTML = "";
        //        return;
        //    }

        //    //Regular Expressions.
        //    var regex = new Array();
        //    regex.push("[A-Z]"); //Uppercase Alphabet.
        //    regex.push("[a-z]"); //Lowercase Alphabet.
        //    regex.push("[0-9]"); //Digit.
        //    regex.push("[$@$!%*#?&]");
        //    regex.push("(?=.{8,})")
        //    //Special Character.

        //    //var regex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,})");
        //    var passed = 0;

        //    //Validate for each Regular Expression.
        //    for (var i = 0; i < regex.length; i++) {
        //        if (new RegExp(regex[i]).test(Str)) {
        //            passed++;
        //        }
        //    }

        //    //Validate for length of Password.
        //    if (passed > 2 && Str.length > 8) {
        //        passed++;
        //    }

        //    alert(passed);

        //    //Display status.
        //    var color = "";
        //    var strength = "";
        //    switch (passed) {
        //        case 0:
        //        case 1:
        //            strength = "Weak";
        //            color = "red";
        //            break;
        //        case 2:
        //            strength = "Good";
        //            color = "darkorange";
        //            break;
        //        case 3:
        //        case 4:
        //            strength = "Strong";
        //            color = "green";
        //            break;
        //        case 5:
        //            strength = "Very Strong";
        //            color = "darkgreen";
        //            break;
        //    }
        //    password_strength.innerHTML = strength;
        //    password_strength.style.color = color;
        //}


        function analyzePassword() {

            var strongRegex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*\+,-])(?=.{8,})");
          //  var strongRegex = new RegExp("/^(?=.*\d)(?=.*[!@#$%^&*])(?=.*[a-z])(?=.*[A-Z]).{8,}$/");
           // var strongRegex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*\+,-\<=>\?\_`{|}~[\])(?=.{8,})");
            var Str = document.getElementById('ctl00_ContentPlaceHolder1_R_txtPwd').value;
            var password_strength = document.getElementById("password_strength");
            var color = "";
            var strength = "";
            if (strongRegex.test(Str)) {
                strength = "Strong";
                color = "darkgreen";
                password_strength.innerHTML = strength;
                password_strength.style.color = color;
                return true;
            }
            else {

                strength = "Weak";
                color = "red";

                password_strength.innerHTML = strength;
                password_strength.style.color = color;
                return false;


            }
        }

    </script>

    <table border="0" class="secContent" style="width: 100%;">
        <tr>
            <td align="center" class="tableheading" colspan="3">Change Password
            </td>
        </tr>
        <tr>
            <td class="tdgap" colspan="3"></td>
        </tr>
        <tr>
            <td>
                <input runat="server" type="hidden" name="MinLength" id="MinLength" readonly autocomplete="off" />
                <input runat="server" type="hidden" name="MinUpperCaseChar" id="MinUpperCaseChar"
                    readonly autocomplete="off" />
                <input runat="server" type="hidden" name="MinNumericChar" id="MinNumericChar" readonly
                    autocomplete="off" />
                <input runat="server" type="hidden" name="MinSpecialChar" id="MinSpecialChar" readonly
                    autocomplete="off" />
                <input runat="server" type="hidden" name="Allow_Pass_equal_userid" id="Allow_Pass_equal_userid"
                    readonly autocomplete="off" />
                <input runat="server" type="hidden" name="username" id="username" readonly autocomplete="off" />
                <input runat="server" type="hidden" name="hash" id="hash" readonly autocomplete="off" />
                <Anthem:HiddenField ID="hdSalt" runat="server" />
                <Anthem:HiddenField ID="hdpwd" runat="server" />
                <Anthem:HiddenField  ID ="hdOldPass" runat="server" /> 
                <Anthem:HiddenField  ID ="hdnewpass" runat="server" /> 
                <Anthem:HiddenField  ID ="hdconfirmPass" runat="server" /> 
                <Anthem:HiddenField  ID ="hfEncrptedPass" runat="server" /> 


            </td>
        </tr>
        <tr>
            <td id="lblOldPassword" align="left" class="vtext" style="width: 162px">Old Password
            </td>
            <td class="colon" align="left">:
            </td>
            <td class="required" align="left">
                <Anthem:TextBox ID="R_txtOldPassword" AutoUpdateAfterCallBack="true" runat="server"
                    CssClass="textbox" TextMode="Password"></Anthem:TextBox>*
            </td>
        </tr>
        <tr>
            <td id="lblPwd" align="left" class="vtext" style="width: 162px">New Password
            </td>
            <td align="left" class="colon">:
            </td>
            <td align="left" class="required">
                <Anthem:TextBox ID="R_txtPwd" runat="server" AutoUpdateAfterCallBack="true" CssClass="textbox"
                    TextMode="Password" onkeyup="analyzePassword();" MaxLength="15"></Anthem:TextBox>*

                <span id="password_strength"></span>
            </td>
        </tr>
        <tr>
            <td id="lblRPassword" align="left" class="vtext" style="width: 162px">Confirm New Password
            </td>
            <td class="colon" align="left">:
            </td>
            <td class="required" align="left">
                <Anthem:TextBox ID="R_txtRPassword" AutoUpdateAfterCallBack="true" runat="server"
                    CssClass="textbox" TextMode="Password" onblur="PasswordCheck();" MaxLength="15"></Anthem:TextBox>
                *
            </td>
        </tr>
        <tr>
            <td style="width: 162px; height: 21px;"></td>
            <td style="width: 100px; height: 21px;">&nbsp;
            </td>
            <td style="width: 200px; height: 21px;">

                <div class="numberlist">
                    <ol>
                        <li class="facebook"><a href="javascript:void(0)">string must contain at least 1 lowercase alphabetical character.</a></li>
                        <li class="twitter"><a href="javascript:void(0)">string must contain at least 1 uppercase alphabetical character.</a></li>
                        <li class="rss"><a href="javascript:void(0)">string must contain at least 1 numeric character.</a></li>
                        <li class="digg"><a href="javascript:void(0)">string must contain at least one special character in following mentioned character <span style="color: red">! @ # $ % ^ & * + , -</span></a></li>

                    </ol>
                </div>




            </td>
        </tr>
        <tr>
            <td></td>
            <td>&nbsp;
            </td>
            <td>
                <%--OnClientClick="return setval();"--%>
                <Anthem:Button ID="btnSave" runat="server" CssClass="button" Text="Change Password"
                    OnClientClick="return  onsubmitclick(); analyzePassword()" Width="157px" AutoUpdateAfterCallBack="true" TextDuringCallBack="WAIT..." OnClick="btnSave_Click" />
                <Anthem:Button ID="btnCancel" runat="server" CssClass="button" Text="Reset" OnClick="btnCancel_Click"
                    AutoUpdateAfterCallBack="true" />
                <p id="p01"></p>
            </td>
        </tr>
    </table>
</asp:Content>
