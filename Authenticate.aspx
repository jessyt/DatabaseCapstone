<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Authenticate.aspx.vb" Inherits="Login_Authenticate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Login</title>
    <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.0/jquery.min.js"></script>
    <script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js"></script>
    <link href="../Style/Main.css" rel="stylesheet" />
    <style>
    </style>
    <script type="text/javascript" src="jquery.js"></script>
</head>
<body>

    

    <form>
       <div class="col-md-4 col-md-offset-3">
            <div class="modal-dialog">
                <div id="loginmodal" class="loginmodal-container">
                <h1>IT Service Tracker</h1>
                
                    <div class="form-group">
                        
                            <label for="txt_username">Email address</label>
                            <input id="txt_username" type="email" class="form-control" placeholder="Email" />
                        </div>
                        <div class="form-group">
                            <label for="txt_password">Password</label>
                            <input id="txt_password" type="password" class="form-control" placeholder="Password" />
                        </div>
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chk_RememberMe" class="checkbox_check"/>
                                Remember Me
                            </label>
                        </div>
                        <button type="button" id="btn-login" class="btn btn-default">Login</button>
                    
                    <button type="button" id="btn-newuser" class="btn btn-link">Register New User</button>
                    </div>
                <div id="newusermodal" class="newusermodal-container">
                <h1>Register A New User</h1>
                
                    <div class="form-group">
                        
                           <label for="txt_NUfirstname">First Name</label>
                            <input id="txt_NUfirstname" type="text" class="form-control" placeholder="FirstName" />
                        
                        <label for="txt_NUlastname">Last Name</label>
                            <input id="txt_NUlastname" type="text" class="form-control" placeholder="LastName" />

                        <label for="txt_NUgradyear"> Graduation Year</label>
                            <input id="txt_NUgradyear" type="text" class="form-control" placeholder="Graduation Year" />

                        <label for="txt_NUusername">Email address</label>
                            <input id="txt_NUusername" type="email" class="form-control" placeholder="Email" />
                        </div>
                        <div class="form-group">
                            <label for="txt_NUpassword">Password</label>
                            <input id="txt_NUpassword" type="password" class="form-control" placeholder="Password" />
                        </div>
                        
                        <button type="button" id="btn-NUregister" class="btn btn-default">Register</button>
                   <button type="button" id="btn-NUlogin" class="btn btn-default">Log In</button>
                    </div>
                </div>
               

           </div>



    </form>

    <script type="text/javascript">
        $(document).ready(

            function () {
                $('#newusermodal').hide();

                $("#txt_password").keyup(function (event) {
                    if (event.keyCode == 13) {
                        $("#btn-login").trigger("click");
                    }
                });
                $('#btn-NUlogin').on('click', function () {
                   
                    $('#newusermodal').hide();
                    $('#loginmodal').show();
                })

                $('#btn-login').on('click', function () {
                    Login();
                })
                $('#btn-newuser').on('click', function () {
                    $('#loginmodal').hide();
                    $('#newusermodal').show();
                })
                $('#chk_RememberMe').on('click', function () {
                    RememberMe();
                })
                // Retrieve
                $("#txt_username").val(localStorage.getItem("username"));
                if(localStorage.getItem("username") != null)
                {
                    $('#chk_RememberMe').prop('checked', true);
                }
            }
            ) //End docready
        function RememberMe(){
            var username = $("#txt_username").val();
            if (username != "") {
            if (typeof (Storage) !== "undefined") {
                // Store
                if ($('input.checkbox_check').is(':checked')) {
                    localStorage.setItem("username", username);
                } else {
                    localStorage.removeItem("username");
                }
            } else {
                alert("Sorry, your browser does not support cookies");
            }
            } else {
                $('#chk_RememberMe').prop('checked', false);
                localStorage.removeItem("username");
            }
        }
        function Login() {
            var username = $("#txt_username").val();
            var password = $("#txt_password").val();

            jsonData = [], wrap = {};
            jsonData.push({
                "Username": username,
                "Password": password
            });
            wrap.dataList = jsonData;

            $.ajax({
                'type': "POST",
                'url': "../../web-services/wsAuthenticate.asmx/AuthenticateUser",
                'data': JSON.stringify(wrap),
                'contentType': "application/json; charset=utf-8",
                'dataType': "json",
                'beforeSend': function () {

                },
                'success': function (data) {
                    if (data.d.indexOf("major.error") > -1) {
                        // Redirect
                        location.href = 'error-handler.aspx';
                    } else {
                        if (data.d.indexOf("warn:") > -1) {
                            // Warning

                        } else {
                            if (data.d.indexOf("err:") > -1) {
                                // Error

                            } else {
                                // Success
                                myAuthenticateData = JSON.parse(data.d, function (key, value) {
                                    var type;
                                    if (value && typeof value === 'object') {
                                        type = value.type;
                                        if (typeof type === 'string' && typeof window[type] === 'function') {
                                            return new (window[type])(value);
                                        }
                                    }
                                    return value;
                                });

                                if ($.isEmptyObject(myAuthenticateData) === false) {
                                    for (var itm in myAuthenticateData) {

                                        var SM_PK = myAuthenticateData[itm].SM_PK;
                                        var Role_FK = myAuthenticateData[itm].Role_FK;
                                        if (SM_PK === -1) {
                                            alert("Invalid Username or Password")
                                            return false;
                                        }
                                        else {
                                            var nextpage = "../ContentPages/ContentPage.aspx";
                                            var winPrint = window.open(nextpage, "_self");
                                            return false;
                                        }
                                        if (itm == (myAuthenticateData.length - 1)) { break; }
                                    }
                                }

                            }
                        }
                    }
                },
                'error': function (data) {
                    alert("UpdateIOs: " + data.responseText)
                },
                'complete': function () {
                }
            });

        }
    </script>
</body>
</html>
