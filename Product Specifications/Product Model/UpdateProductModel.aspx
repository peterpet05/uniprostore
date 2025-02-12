<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UpdateProductModel.aspx.cs" Inherits="Unipro_Store.Product_Model.UpdateProductModel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card card-info">
        <!-- Bagian Ikon Model Produk dan Label -->
        <div class="card-header text-center" style="background-color: white; border-bottom: none;">
            <i class="fas fa-window-restore" style="font-size: 58px; margin-top: 20px; color: #6c757d"></i>
            <p style="font-size: 19px; font-weight: 600; margin-top: 10px; margin-bottom: 0px; color: #6c757d">Detail Model Produk</p>
        </div>
        <!-- Bagian Form Input -->
        <div class="card-body">
            <div class="form-group row">
                <label for="inputProductBrand" class="col-sm-2 col-form-label">Merek Produk</label>
                <div class="col-sm-10">
                    <select class="form-control" id="inputProductBrand" runat="server" required>
                        <option value="" selected>Pilih merek produk</option>
                    </select>
                </div>
            </div>
            <div class="form-group row">
                <label for="inputProductModelname" class="col-sm-2 col-form-label">Nama Model</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="inputProductModelname" placeholder="Masukkan nama model produk" runat="server" required="required">
                    <asp:Label ID="lblErrorMessage" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                </div>
            </div>
        </div>
        <!-- Penutup Form Input -->
        <div class="card-footer d-flex justify-content-end" style="padding-top: 0px">
            <asp:Button Text="Batal" runat="server" class="btn btn-secondary" ID="Cancel" OnClientClick="event.preventDefault(); window.location='ProductModel.aspx';" />
            <asp:Button Text="Simpan" runat="server" class="btn btn-primary ml-2" ID="Update" OnClick="Update_Click" />
        </div>
        <!-- Penutup Card Keseluruhan -->
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <!-- Menghapus border merah saat mulai mengetikkan nama model produk baru -->
    <script>
        $(document).ready(function () {
            $('#<%=inputProductModelname.ClientID%>').on('input', function () {
                $(this).removeClass('input-error');
            });
        });
    </script>

</asp:Content>