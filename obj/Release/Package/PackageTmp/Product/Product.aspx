<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Product.aspx.cs" Inherits="Unipro_Store.Product.Product" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-sm-12">
            <asp:LinkButton runat="server" ID="create" OnClick="create_Click" CssClass="btn btn-primary" Style="background-color: #386bba; border-color: #386bba; color: white; font-size: 15px"> 
                Add New Product<i class="fas fa-plus-circle" style="margin-left:10px"></i>
            </asp:LinkButton>
        </div>
        <br>
        <br>
        <br>
        <div class="col-md-12">
            <div>
                <div class="row mb-3 align-items-center">
                    <div class="col-md-5 d-flex align-items-center">
                        <div class="input-group">
                            <input type="search" id="searchKeyword" class="form-control search-with-icon" placeholder="Search keyword" aria-controls="tabelProduct">
                        </div>
                    </div>
                    <div class="col-md-2 d-flex align-items-center">
                        <label for="filterStock" style="margin-right: 10px; margin-top: 6px">Stok:</label>
                        <select id="filterStock" class="form-control">
                            <option value="">All</option>
                            <option value="empty">0</option>
                            <option value="belowOrEqual3"><= 3</option>
                            <option value="belowOrEqual5"><= 5</option>
                            <option value="above10">>= 10</option>
                        </select>
                    </div>
                    <div class="col-md-3 d-flex align-items-center">
                        <label for="filterSubcategory" style="margin-right: 10px; margin-top: 6px"">Subkategori:</label>
                        <select id="filterSubcategory" class="form-control" runat="server"></select>
                    </div>
                    <div class="col-md-2 d-flex align-items-center">
                        <label for="filterPhoneBrand" style="margin-right: 10px; margin-top: 6px"">Brand:</label>
                        <select id="filterPhoneBrand" class="form-control" runat="server"></select>
                    </div>
                </div>

                <asp:Literal ID="ltStockNotification" runat="server"></asp:Literal>

                <!-- Tabel Product -->
                <table id="tabelProduct" class="table table-bordered table-striped" style="width: 100%">
                    <thead>
                        <tr>
                            <th>No</th>
                            <th>Kode</th>
                            <th>Nama Produk</th>
                            <th class="hidden-column">Merek Ponsel</th>
                            <th>Stok</th>
                            <th style="width: 78px;">Harga Beli</th>
                            <th style="width: 78px;">Harga Jual</th>
                            <th>Gambar</th>
                            <th style="width: 100px;">Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Literal ID="ltTableRows" runat="server"></asp:Literal>
                    </tbody>
                </table>
                <!-- Penutup Tabel Product -->

                <!-- Modal Konfirmasi Penghapusan -->
                <div class="modal fade" id="deleteModal" tabindex="-1" role="dialog" aria-labelledby="deleteModalLabel" aria-hidden="true">
                    <div class="modal-dialog modal-dialog-centered" role="document">
                        <div class="modal-content" style="border-radius: 20px; padding: 20px;">
                            <div class="modal-header" style="border-bottom: none; padding-bottom: 12px; padding-top: 12px;">
                                <h5 class="modal-title" id="deleteModalLabel" style="font-weight: bold; font-size: 32px">Konfirmasi Hapus?</h5>
                            </div>
                            <div class="modal-body" style="padding-top: 0px;">
                                <p style="font-size: 17px; margin-bottom: 0px">Data produk yang telah dihapus tidak dapat dipulihkan kembali.</p>
                                <asp:HiddenField ID="hfProductIDToDelete" runat="server" />
                            </div>
                            <div class="modal-footer" style="border-top: none; justify-content: space-between; padding-top: 0px;">
                                <button type="button" class="btn btn-secondary btn-lg" data-dismiss="modal" style="width: 45%; border-radius: 10px; font-weight: bold;">Batal</button>
                                <asp:Button ID="btnConfirmDelete" runat="server" CssClass="btn btn-danger btn-lg" Text="Hapus" OnClick="delete_Click" Style="width: 45%; border-radius: 10px; font-weight: bold;" />
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Penutup Modal Konfirmasi Penghapusan -->
            </div>
        </div>
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script>
        $(document).ready(function () {
            var table = $('#tabelProduct').DataTable({
                "responsive": true,
                "lengthChange": false,
                "autoWidth": false,
                "dom": 'rt' +
                    '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
                "columnDefs": [
                    { "orderable": false, "targets": [7, 8] }
                ]
            });

            // Filter berdasarkan Keyword
            $('#searchKeyword').on('keyup', function () {
                table.search(this.value).draw();
            });

            // Event untuk melihat produk dengan stok habis (0)
            $('#viewOutOfStock').on('click', function (e) {
                e.preventDefault();

                $('select[id$=filterSubcategory]').val(''); 
                $('select[id$=filterPhoneBrand]').val('');
                $('select[id$=filterStock]').val('');  

                table.column(2).search('').draw();
                table.column(3).search('').draw();
                table.column(4).search('^0$', true, false).draw();
            });

            // Event untuk melihat produk dengan stok rendah (1-3)
            $('#viewLowStock').on('click', function (e) {
                e.preventDefault();

                $('select[id$=filterSubcategory]').val(''); 
                $('select[id$=filterPhoneBrand]').val('');
                $('select[id$=filterStock]').val('');

                table.column(2).search('').draw();
                table.column(3).search('').draw();
                table.column(4).search('^[1-3]$', true, false).draw(); 
            });

            // Filter berdasarkan stok
            $('#filterStock').on('change', function () {
                var filterValue = $(this).val();

                table.column(4).search('');

                switch (filterValue) {
                    case 'empty':
                        table.column(4).search('^0$', true, false).draw();
                        break;
                    case 'belowOrEqual3':
                        table.column(4).search('^[0-3]$', true, false).draw();
                        break;
                    case 'belowOrEqual5':
                        table.column(4).search('^[0-5]$', true, false).draw();
                        break;
                    case 'above10':
                        table.column(4).search('^[1-9][0-9]+|10$', true, false).draw();
                        break;
                    default:
                        table.draw();
                }
            });

            // Filter berdasarkan Subkategori
            $('select[id$=filterSubcategory]').on('change', function () {
                var selectedSubcategory = this.value;
                if (selectedSubcategory) {
                    table.column(2).search(selectedSubcategory).draw();
                } else {
                    table.column(2).search('').draw();
                }
            });

            // Filter berdasarkan Brand Ponsel
            $('select[id$=filterPhoneBrand]').on('change', function () {
                var selectedPhoneBrand = this.value;
                if (selectedPhoneBrand) {
                    table.column(3).search(selectedPhoneBrand).draw();
                } else {
                    table.column(3).search('').draw();
                }
            });
        });
    </script>
    <!-- Modal Delete Product -->
    <script>
        function showDeleteModal(productId) {
            $('#<%= hfProductIDToDelete.ClientID %>').val(productId);
            $('#deleteModal').modal('show');
        }
    </script>

</asp:Content>