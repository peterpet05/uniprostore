<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UpdateProduct.aspx.cs" Inherits="Unipro_Store.Product.UpdateProduct" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card card-info">
        <!-- Bagian Ikon Produk dan Label -->
        <div class="card-header text-center" style="background-color: white; border-bottom: none;">
            <i class="fas fa-boxes" style="font-size: 58px; margin-top: 20px; color: #6c757d"></i>
            <p style="font-size: 19px; font-weight: 600; margin-top: 10px; margin-bottom: 0px; color: #6c757d">Detail Produk</p>
        </div>
        <!-- Bagian Form Input -->
        <div class="card-body">
            <div class="form-group row">
                <label for="inputCategory" class="col-sm-1-25 col-form-label">Kategori</label>
                <div class="col-sm-4-75">
                    <asp:DropDownList ID="inputCategory" runat="server" CssClass="form-control" OnSelectedIndexChanged="inputCategory_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </div>
                <label for="inputSubCategory" class="col-sm-1-25 col-form-label">Subkategori</label>
                <div class="col-sm-4-75">
                    <asp:DropDownList ID="inputSubCategory" runat="server" CssClass="form-control" OnSelectedIndexChanged="inputSubCategory_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </div>
            </div>
            <div class="form-group row">
                <label for="inputProductBrand" class="col-sm-1-25 col-form-label">Merek</label>
                <div class="col-sm-4-75">
                    <asp:DropDownList ID="inputProductBrand" runat="server" CssClass="form-control" OnSelectedIndexChanged="inputProductBrand_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </div>
                <label for="inputProductModel" class="col-sm-1-25 col-form-label">Model</label>
                <div class="col-sm-4-75">
                    <asp:DropDownList ID="inputProductModel" runat="server" CssClass="form-control" OnSelectedIndexChanged="inputProductModel_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </div>
            </div>
            <div class="form-group row">
                <label for="inputPhoneBrand" class="col-sm-1-25 col-form-label">Brand Ponsel</label>
                <div class="col-sm-4-75">
                    <asp:DropDownList ID="inputPhoneBrand" runat="server" CssClass="form-control" OnSelectedIndexChanged="inputPhoneBrand_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </div>
                <label for="inputPhoneType" class="col-sm-1-25 col-form-label">Tipe Ponsel</label>
                <div class="col-sm-4-75">
                    <asp:DropDownList ID="inputPhoneType" runat="server" CssClass="form-control" OnSelectedIndexChanged="inputPhoneType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </div>
            </div>
            <div class="form-group row">
                <label for="inputProductColor" class="col-sm-1-25 col-form-label">Warna</label>
                <div class="col-sm-4-75">
                    <asp:DropDownList ID="inputProductColor" runat="server" CssClass="form-control" OnSelectedIndexChanged="inputProductColor_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </div>
                <label for="inputStock" class="col-sm-1-25 col-form-label">Stok</label>
                <div class="col-sm-4-75">
                    <input type="number" class="form-control" id="inputStock" placeholder="Jumlah" runat="server" required="required" min="0" max="99">
                </div>
            </div>
            <div class="form-group row">
                <label for="inputBuyPrice" class="col-sm-1-25 col-form-label">Harga Beli</label>
                <div class="col-sm-4-75">
                    <input type="text" class="form-control" id="inputBuyPrice" placeholder="Masukkan harga beli" runat="server" required="required" pattern="\d*" title="Hanya angka yang diperbolehkan">
                </div>
                <label for="inputSellPrice" class="col-sm-1-25 col-form-label">Harga Jual</label>
                <div class="col-sm-4-75">
                    <input type="text" class="form-control" id="inputSellPrice" placeholder="Masukkan harga jual" runat="server" required="required" pattern="\d*" title="Hanya angka yang diperbolehkan">
                </div>
            </div>
            <div class="form-group row">
                <label for="inputDescription" class="col-sm-1-25 col-form-label">Deskripsi</label>
                <div class="col-sm-10-75">
                    <textarea class="form-control" id="inputDescription" placeholder="Masukkan deskripsi produk" runat="server" rows="2"></textarea>
                </div>
            </div>
            <div class="form-group row">
                <label for="inputImage" class="col-sm-1-25 col-form-label">Gambar</label>
                <div class="col-sm-10-75">
                        <div class="custom-file">
                            <asp:FileUpload type="file" ID="inputImage" runat="server" CssClass="custom-file-input" />
                            <label id="fileNameLabel" class="custom-file-label" for="inputImage">Uploaded image</label>
                            <asp:Label ID="lblErrorMessage" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                            <asp:HiddenField ID="hfImageFileName" runat="server" />
                        </div>
                </div>
            </div>
        </div>
        <!-- Penutup Form Input -->
        <div class="card-footer d-flex justify-content-end" style="padding-top: 0px">
            <asp:Button Text="Batal" runat="server" class="btn btn-secondary" ID="Cancel" OnClientClick="event.preventDefault(); window.location='Product.aspx';" />
            <asp:Button Text="Simpan" runat="server" class="btn btn-primary ml-2" ID="Update" OnClick="Update_Click"/>
        </div>
        <!-- Penutup Card Keseluruhan -->
    </div>

    <!-- Menampilkan nama gambar yang dipilih -->
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            var input = document.getElementById('<%= inputImage.ClientID %>');
            var fileNameLabel = document.getElementById('fileNameLabel');

            input.addEventListener('change', function () {
                if (input.files && input.files[0]) {
                    fileNameLabel.textContent = input.files[0].name;
                } else {
                    fileNameLabel.textContent = "Uploaded image";
                }
            });
        });
    </script>

</asp:Content>