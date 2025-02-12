<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UpdateCategory.aspx.cs" Inherits="Unipro_Store.Category.UpdateCategory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card card-info">
        <!-- Bagian Ikon Category dan Label -->
        <div class="card-header text-center" style="background-color: white; border-bottom: none;">
            <i class="fas fa-tag" style="font-size: 58px; margin-top: 20px; color: #6c757d"></i>
            <p style="font-size: 19px; font-weight: 600; margin-top: 10px; margin-bottom: 0px; color: #6c757d">Detail Kategori</p>
        </div>
        <!-- Bagian Form Input -->
        <div class="card-body">
            <div class="form-group row">
                <label for="inputCategoryname" class="col-sm-2 col-form-label">Nama kategori</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="inputCategoryname" placeholder="Masukkan nama kategori" runat="server" required="required">
                    <asp:Label ID="lblErrorMessage" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                </div>
            </div>
            <div class="form-group row">
            <label for="inputDescription" class="col-sm-2 col-form-label">Deskripsi</label>
                <div class="col-sm-10">
                    <textarea class="form-control" id="inputDescription" placeholder="Masukkan deskripsi kategori" runat="server" rows="7"></textarea>
                </div>
            </div>
        </div>
        <!-- Penutup Form Input -->
        <div class="card-footer d-flex justify-content-end" style="padding-top: 0px">
            <asp:Button Text="Batal" runat="server" class="btn btn-secondary" ID="Cancel" OnClientClick="event.preventDefault(); window.location='Category.aspx';" />
            <asp:Button Text="Simpan" runat="server" class="btn btn-primary ml-2" ID="Update" OnClick="Update_Click" />
        </div>
        <!-- Penutup Card Keseluruhan -->
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <!-- Menghapus border merah saat mulai mengetikkan nama kategori baru -->
    <script>
        $(document).ready(function () {
            $('#<%=inputCategoryname.ClientID%>').on('input', function () {
                $(this).removeClass('input-error');
            });
        });
    </script>

</asp:Content>