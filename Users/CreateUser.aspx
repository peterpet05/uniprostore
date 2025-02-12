<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateUser.aspx.cs" Inherits="Unipro_Store.Users.CreateUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card card-info">
        <!-- Bagian Ikon Pengguna dan Label -->
        <div class="card-header text-center" style="background-color: white; border-bottom: none;">
            <i class="fas fa-users" style="font-size: 58px; margin-top: 20px; color: #6c757d"></i>
            <p style="font-size: 19px; font-weight: 600; margin-top: 10px; margin-bottom: 0px; color: #6c757d">Detail Pengguna</p>
        </div>
        <!-- Bagian Form Input -->
        <div class="card-body">
            <div class="form-group row">
                <label for="inputFullname" class="col-sm-2 col-form-label">Nama lengkap</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="inputFullname" placeholder="Masukkan nama lengkap" runat="server" required="required">
                </div>
            </div>
            <div class="form-group row">
                <label for="inputUsername" class="col-sm-2 col-form-label">Username</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="inputUsername" placeholder="Masukkan username" runat="server" required="required">
                    <asp:Label ID="lblErrorMessage" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                </div>
            </div>
            <div class="form-group row">
                <label for="inputPassword" class="col-sm-2 col-form-label">Password</label>
                <div class="col-sm-10">
                    <div class="input-group">
                        <input type="password" class="form-control" id="inputPassword" placeholder="Masukkan password" runat="server" required="required" pattern="^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$" title="Password harus minimal 8 karakter, dengan campuran huruf dan angka.">
                        <div class="input-group-append">
                            <span toggle="#inputPassword" class="input-group-text">
                                <i class="fas fa-eye toggle-password" style="cursor: pointer;"></i>
                            </span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group row">
                <label for="inputRole" class="col-sm-2 col-form-label">Role</label>
                <div class="col-sm-10 mt-1">
                    <asp:RadioButton ID="ownerRole" runat="server" GroupName="radio1" Checked="true" Text="Owner" Style="margin-right: 80px;" />
                    <asp:RadioButton ID="adminRole" runat="server" GroupName="radio1" Text="Admin" />
                </div>
            </div>
        </div>
        <!-- Penutup Form Input -->
        <div class="card-footer d-flex justify-content-end" style="padding-top: 0px">
            <asp:Button Text="Batal" runat="server" class="btn btn-secondary" ID="Cancel" OnClientClick="event.preventDefault(); window.location='Users.aspx';" />
            <asp:Button Text="Simpan" runat="server" class="btn btn-primary ml-2" ID="Add" OnClick="Add_Click" />
        </div>
        <!-- Penutup Card Keseluruhan -->
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <!-- Menghapus border merah saat mulai mengetikkan username baru -->
    <script>
        $(document).ready(function () {
            $('#<%=inputUsername.ClientID%>').on('input', function () {
                $(this).removeClass('input-error');
            });
        });
    </script>
    <!-- Show dan Hide Password -->
    <script>
        $(document).ready(function () {
            $(".toggle-password").click(function () {
                var input = $(this).closest(".input-group").find("input");

                if (input.attr("type") === "password") {
                    input.attr("type", "text"); 
                    $(this).removeClass("fa-eye").addClass("fa-eye-slash"); 
                } else {
                    input.attr("type", "password"); 
                    $(this).removeClass("fa-eye-slash").addClass("fa-eye"); 
                }
            });
        });
    </script>

</asp:Content>