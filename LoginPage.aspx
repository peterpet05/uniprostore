<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginPage.aspx.cs" Inherits="Unipro_Store.LoginPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />

    <link href="https://fonts.googleapis.com/css?family=Lato:300,400,700&display=swap" rel="stylesheet" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" />
    <link rel="stylesheet" href="/Login Page/css/style.css" />
    <link href="<%= ResolveUrl("~/Content/custom-style.css") %>" rel="stylesheet" />

</head>
<body>
    <form id="form1" runat="server">
        <section class="ftco-section">
            <div class="container">
                <div class="row justify-content-center">
                    <div class="col-md-8 col-lg-5">
                        <div class="login-wrap p-4 p-md-5" style="background-color: whitesmoke;">
                            <div class="d-flex align-items-center justify-content-center">
                                <img src="Login Page/images/Logo.png" style="width: 100px; height: 100px;" />
                            </div>
                            <h3 class="text-center" style="font-weight: bold; margin: 12px 0px 0px 0px;">Welcome Back</h3>
                            <p class="text-center" style="font-size: 15px; margin-top: -5px; font-family: Roboto, sans-serif;">Please log in to continue</p>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="text-danger" />
                            <asp:Panel runat="server" DefaultButton="btnLogin">
                                <div class="form-group">
                                    <label for="username" style="margin-left:1px;">Username</label>
                                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control rounded-left" placeholder="Input Username" required="required" style="height: 35px;"></asp:TextBox>
                                </div>
                                <div class="form-group" style="margin-bottom:35px; position: relative">
                                    <label for="password">Password</label>
                                    <div class="input-group">
                                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Input Password" required="required"></asp:TextBox>
                                        <div class="input-group-append">
                                            <span toggle="#txtPassword" class="input-group-text">
                                                <i class="fas fa-eye toggle-password" style="cursor: pointer;"></i>
                                            </span>
                                        </div>
                                    </div>
                                    <p style="font-size:9px; margin: 3.5px 0px 0px 1px;">It must be a combination of minimum 8 letters, numbers, and symbols</p>
                                    <div id="divErrorContainer" runat="server" class="error-container">
                                        <asp:Label ID="lblErrorMessage" runat="server" CssClass="text-danger" Visible="false">
                                            <i class="fa fa-exclamation-circle"></i> Invalid username or password.
                                        </asp:Label>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Button ID="btnLogin" runat="server" CssClass="form-control btn btn-primary rounded submit px-3" Text="Log In" OnClick="btnLogin_Click" />
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </form>

    <script src='<%= ResolveUrl("~/Login Page/js/jquery.min.js") %>'></script>
    <script src='<%= ResolveUrl("~/Login Page/js/popper.js") %>'></script>
    <script src='<%= ResolveUrl("~/Login Page/js/bootstrap.min.js") %>'></script>
    <script src='<%= ResolveUrl("~/Login Page/js/main.js") %>'></script>
    <script>
        $(document).ready(function () {
            $(".toggle-password").click(function () {
                var input = $("#<%= txtPassword.ClientID %>");

                if (input.attr("type") === "password") {
                    input.attr("type", "text"); 
                    $(this).removeClass("fas fa-eye").addClass("fas fa-eye-slash"); 
                } else {
                    input.attr("type", "password"); 
                    $(this).removeClass("fas fa-eye-slash").addClass("fas fa-eye");
                }
            });
        });
    </script>

</body>
</html>