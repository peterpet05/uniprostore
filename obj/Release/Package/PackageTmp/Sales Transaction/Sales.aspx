<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Sales.aspx.cs" Inherits="Unipro_Store.Sales_Transaction.Sales" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-sm-12">
            <asp:LinkButton runat="server" ID="create" OnClick="create_Click" CssClass="btn btn-primary" Style="background-color: #386bba; border-color: #386bba; color: white; font-size: 15px"> 
                Add New Transaction<i class="fas fa-plus-circle" style="margin-left:10px"></i>
            </asp:LinkButton>
        </div>
        <br>
        <br>
        <br>
        <div class="col-md-12">
            <div>
                <div class="row mb-3 align-items-center">
                    <div class="col-md-6 d-flex align-items-center">
                        <div class="input-group">
                            <input type="search" id="searchKeyword" class="form-control search-with-icon" placeholder="Search keyword" aria-controls="tabelSalesTransaction">
                        </div>
                    </div>
                    <div class="col-md-3 d-flex align-items-center">
                        <label for="filterMedia" style="margin-right: 10px; margin-top: 6px"">Media:</label>
                        <select id="filterMedia" class="form-control" runat="server">
                        </select>
                    </div>
                    <div class="col-md-3 d-flex align-items-center">
                        <label for="filterUser" style="margin-right: 10px; margin-top: 6px"">Pencatat:</label>
                        <select id="filterUser" class="form-control" runat="server">
                        </select>
                    </div>
                </div>
                <!-- Tabel Sales Transaction -->
                <table id="tabelSalesTransaction" class="table table-bordered table-striped" style="width: 100%">
                    <thead>
                        <tr>
                            <th>No</th>
                            <th>Kode</th>
                            <th>Tanggal</th>
                            <th>Media</th>
                            <th>Total Produk</th>
                            <th>Total Harga</th>
                            <th>Diskon</th>
                            <th>Total Bayar</th>
                            <th>Pencatat</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Literal ID="ltTableRows" runat="server"></asp:Literal>
                    </tbody>
                </table>
                <!-- Penutup Tabel Sales Transaction-->

                <!-- Modal Konfirmasi Penghapusan -->
                <div class="modal fade" id="deleteModal" tabindex="-1" role="dialog" aria-labelledby="deleteModalLabel" aria-hidden="true">
                    <div class="modal-dialog modal-dialog-centered" role="document">
                        <div class="modal-content" style="border-radius: 20px; padding: 20px;">
                            <div class="modal-header" style="border-bottom: none; padding-bottom: 12px; padding-top: 12px;">
                                <h5 class="modal-title" id="deleteModalLabel" style="font-weight: bold; font-size: 32px">Konfirmasi Hapus?</h5>
                            </div>
                            <div class="modal-body" style="padding-top: 0px;">
                                <p style="font-size: 17px; margin-bottom: 0px">Data transaksi yang telah dihapus tidak dapat dipulihkan kembali.</p>
                                <asp:HiddenField ID="hfSalesTransactionIDToDelete" runat="server" />
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
            var table = $('#tabelSalesTransaction').DataTable({
                "responsive": true,
                "lengthChange": false,
                "autoWidth": false,
                "dom": 'rt' +
                    '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
                "columnDefs": [
                    { "orderable": false, "targets": [9] }
                ]
            });

            // Filter Berdasarkan Media
            $('select[id$=filterMedia]').on('change', function () {
                var selectedMedia = this.value;
                if (selectedMedia) {
                    table.column(3).search(selectedMedia).draw();
                } else {
                    table.column(3).search('').draw();
                }
            });

            // Filter Berdasarkan User
            $('select[id$=filterUser]').on('change', function () {
                var selectedUser = this.value;
                if (selectedUser) {
                    table.column(8).search(selectedUser).draw();
                } else {
                    table.column(8).search('').draw();
                }
            });

            // Filter berdasarkan Keyword
            $('#searchKeyword').on('keyup', function () {
                table.search(this.value).draw();
            });
        });
    </script>
    <!-- Modal Delete Sales Transaction -->
    <script>
        function showDeleteModal(salesId) {
            $('#<%= hfSalesTransactionIDToDelete.ClientID %>').val(salesId);
            $('#deleteModal').modal('show');
        }
    </script>

</asp:Content>