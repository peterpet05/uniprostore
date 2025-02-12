<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UpdatePhoneBrand.aspx.cs" Inherits="Unipro_Store.Phone_Brand.UpdatePhoneBrand" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card card-info">
        <!-- Bagian Ikon Brand Ponsel dan Label -->
        <div class="card-header text-center" style="background-color: white; border-bottom: none;">
            <i class="fas fa-trademark" style="font-size: 58px; margin-top: 20px; color: #6c757d"></i>
            <p style="font-size: 19px; font-weight: 600; margin-top: 10px; margin-bottom: 0px; color: #6c757d">Detail Brand Ponsel</p>
        </div>
        <!-- Bagian Form Input -->
        <div class="card-body">
            <div class="form-group row">
                <label for="inputPhoneBrandname" class="col-sm-2 col-form-label">Nama Brand</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="inputPhoneBrandname" placeholder="Masukkan nama brand" runat="server" required="required">
                    <asp:Label ID="lblErrorMessage" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                </div>
            </div>
        </div>
        <!-- Penutup Form Input -->
        <div class="card-footer d-flex justify-content-end" style="padding-top: 0px">
            <asp:Button Text="Batal" runat="server" class="btn btn-secondary" ID="Cancel" OnClientClick="event.preventDefault(); window.location='PhoneBrand.aspx';" />
            <asp:Button Text="Simpan" runat="server" class="btn btn-primary ml-2" ID="Update" OnClick="Update_Click" />
        </div>
        <!-- Penutup Card Keseluruhan -->
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <!-- Menghapus border merah saat mulai mengetikkan nama brand ponsel baru -->
    <script>
        $(document).ready(function () {
            $('#<%=inputPhoneBrandname.ClientID%>').on('input', function () {
                $(this).removeClass('input-error');
            });
        });
    </script>

</asp:Content>