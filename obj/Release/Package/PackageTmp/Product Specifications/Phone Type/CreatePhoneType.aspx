<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreatePhoneType.aspx.cs" Inherits="Unipro_Store.Phone_Type.CreatePhoneType" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card card-info">
        <!-- Bagian Ikon Tipe Ponsel dan Label -->
        <div class="card-header text-center" style="background-color: white; border-bottom: none;">
            <i class="fas fa-mobile" style="font-size: 58px; margin-top: 20px; color: #6c757d"></i>
            <p style="font-size: 19px; font-weight: 600; margin-top: 10px; margin-bottom: 0px; color: #6c757d">Detail Tipe Ponsel</p>
        </div>
        <!-- Bagian Form Input -->
        <div class="card-body">
            <div class="form-group row">
                <label for="inputPhoneBrand" class="col-sm-2 col-form-label">Brand Ponsel</label>
                <div class="col-sm-10">
                    <select class="form-control" id="inputPhoneBrand" runat="server" required>
                        <option value="" selected>Pilih brand ponsel</option>
                    </select>
                </div>
            </div>
            <div class="form-group row">
                <label for="inputPhoneTypename" class="col-sm-2 col-form-label">Nama Tipe</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="inputPhoneTypename" placeholder="Masukkan nama tipe ponsel" runat="server" required="required">
                    <asp:Label ID="lblErrorMessage" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                </div>
            </div>
        </div>
        <!-- Penutup Form Input -->
        <div class="card-footer d-flex justify-content-end" style="padding-top: 0px">
            <asp:Button Text="Batal" runat="server" class="btn btn-secondary" ID="Cancel" OnClientClick="event.preventDefault(); window.location='PhoneType.aspx';" />
            <asp:Button Text="Simpan" runat="server" class="btn btn-primary ml-2" ID="Add" OnClick="Add_Click" />
        </div>
        <!-- Penutup Card Keseluruhan -->
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <!-- Menghapus border merah saat mulai mengetikkan nama tipe ponsel baru -->
    <script>
        $(document).ready(function () {
            $('#<%=inputPhoneTypename.ClientID%>').on('input', function () {
                $(this).removeClass('input-error');
            });
        });
    </script>

</asp:Content>