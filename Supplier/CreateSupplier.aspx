<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateSupplier.aspx.cs" Inherits="Unipro_Store.Supplier.CreateSupplier" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card card-info">
        <!-- Bagian Ikon Supplier dan Label -->
        <div class="card-header text-center" style="background-color: white; border-bottom: none;">
            <i class="fas fa-truck" style="font-size: 58px; margin-top: 20px; color: #6c757d"></i>
            <p style="font-size: 19px; font-weight: 600; margin-top: 10px; margin-bottom: 0px; color: #6c757d">Detail Pemasok</p>
        </div>
        <!-- Bagian Form Input -->
        <div class="card-body">
            <div class="form-group row">
                <label for="inputSuppliername" class="col-sm-2 col-form-label">Nama supplier</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="inputSuppliername" placeholder="Masukkan nama supplier" runat="server" required="required">
                    <asp:Label ID="lblErrorMessage" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                </div>
            </div>
            <div class="form-group row">
                <label for="inputAddress" class="col-sm-2 col-form-label">Alamat</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="inputAddress" placeholder="Masukkan alamat supplier" runat="server" required="required">
                </div>
            </div>
            <div class="form-group row">
                <label for="inputCity" class="col-sm-2 col-form-label">Kota</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="inputCity" placeholder="Masukkan kota alamat supplier" runat="server" required="required">
                </div>
            </div>
            <div class="form-group row">
                <label for="inputContactnumber" class="col-sm-2 col-form-label">Kontak</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="inputContactnumber" placeholder="Masukkan kontak supplier"
                        pattern="^0\d{9,14}$" title="Nomor harus dimulai dengan 0 dan terdiri dari 10-15 angka"
                        runat="server" required="required" minlength="10" maxlength="15">
                </div>
            </div>
        </div>
        <!-- Penutup Form Input -->
        <div class="card-footer d-flex justify-content-end" style="padding-top: 0px">
            <asp:Button Text="Batal" runat="server" class="btn btn-secondary" ID="Cancel" OnClientClick="event.preventDefault(); window.location='Supplier.aspx';" />
            <asp:Button Text="Simpan" runat="server" class="btn btn-primary ml-2" ID="Add" OnClick="Add_Click" />
        </div>
        <!-- Penutup Card Keseluruhan -->
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <!-- Menghapus border merah saat mulai mengetikkan nama supplier baru -->
    <script>
        $(document).ready(function () {
            $('#<%=inputSuppliername.ClientID%>').on('input', function () {
                $(this).removeClass('input-error');
            });
        });
    </script>

</asp:Content>