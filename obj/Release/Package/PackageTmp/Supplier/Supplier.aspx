<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Supplier.aspx.cs" Inherits="Unipro_Store.Supplier.Supplier" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-sm-12">
            <asp:LinkButton runat="server" ID="create" OnClick="create_Click" CssClass="btn btn-primary" Style="background-color: #386bba; border-color: #386bba; color: white; font-size: 15px"> 
                Add New Supplier<i class="fas fa-plus-circle" style="margin-left:10px"></i>
            </asp:LinkButton>
        </div>
        <br>
        <br>
        <br>
        <div class="col-md-12">
            <div>
                <div class="row mb-3 align-items-center">
                    <div class="col-md-9 d-flex align-items-center">
                        <div class="input-group">
                            <input type="search" id="searchKeyword" class="form-control search-with-icon" placeholder="Search keyword" aria-controls="tabelSupplier">
                        </div>
                    </div>
                    <div class="col-md-3 d-flex align-items-center">
                        <label for="filterCity" style="margin-right: 10px; margin-top: 6px"">Kota:</label>
                        <select id="filterCity" class="form-control" runat="server"></select>
                    </div>
                </div>

                <!-- Tabel Supplier -->
                <table id="tabelSupplier" class="table table-bordered table-striped" style="width: 100%">
                    <thead>
                        <tr>
                            <th>No</th>
                            <th>Nama Supplier</th>
                            <th>Alamat</th>
                            <th>Kontak</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Literal ID="ltTableRows" runat="server"></asp:Literal>
                    </tbody>
                </table>
                <!-- Penutup Tabel Supplier -->

                <!-- Modal Konfirmasi Penghapusan -->
                <div class="modal fade" id="deleteModal" tabindex="-1" role="dialog" aria-labelledby="deleteModalLabel" aria-hidden="true">
                    <div class="modal-dialog modal-dialog-centered" role="document">
                        <div class="modal-content" style="border-radius: 20px; padding: 20px;">
                            <div class="modal-header" style="border-bottom: none; padding-bottom: 12px; padding-top: 12px;">
                                <h5 class="modal-title" id="deleteModalLabel" style="font-weight: bold; font-size: 32px">Konfirmasi Hapus?</h5>
                            </div>
                            <div class="modal-body" style="padding-top: 0px;">
                                <p style="font-size: 17px; margin-bottom: 0px">Data supplier yang telah dihapus tidak dapat dipulihkan kembali.</p>
                                <asp:HiddenField ID="hfSupplierIDToDelete" runat="server" />
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
            var table = $('#tabelSupplier').DataTable({
                "responsive": true,
                "lengthChange": false,
                "autoWidth": false,
                "dom": 'rt' +
                    '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
                "columnDefs": [
                    { "orderable": false, "targets": [5] }
                ]
            });

            // Filter Berdasarkan Kota
            $('select[id$=filterCity]').on('change', function () {
                var selectedCity = this.value;
                if (selectedCity) {
                    table.column(2).search(selectedCity).draw();
                } else {
                    table.column(2).search('').draw();
                }
            });

            // Filter berdasarkan Keyword
            $('#searchKeyword').on('keyup', function () {
                table.search(this.value).draw();
            });
        });
    </script>
    <!-- Modal Delete Supplier -->
    <script>
        function showDeleteModal(supplierId) {
            $('#<%= hfSupplierIDToDelete.ClientID %>').val(supplierId);
            $('#deleteModal').modal('show');
        }
    </script>

</asp:Content>