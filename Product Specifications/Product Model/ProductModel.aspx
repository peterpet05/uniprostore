<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductModel.aspx.cs" Inherits="Unipro_Store.Product_Model.ProductModel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-sm-12 d-flex justify-content-between" style="margin-bottom:20px">
            <asp:LinkButton runat="server" ID="create" OnClick="create_Click" CssClass="btn btn-primary" 
                Style="background-color: #386bba; border-color: #386bba; color: white; font-size: 15px"> 
                Add New Product Model<i class="fas fa-plus-circle" style="margin-left:10px"></i>
            </asp:LinkButton>
            <button id="downloadPdf" class="btn btn-primary" 
                style="background-color: #386bba; border-color: #386bba; color: white; font-size: 15px"> 
                Download Data<i class="fas fa-file-download" style="margin-left:10px"></i>
            </button>
        </div>
        <div class="col-md-12">
            <div>
                <div class="row mb-3 align-items-center">
                    <div class="col-md-9 d-flex align-items-center">
                        <div class="input-group">
                            <input type="search" id="searchKeyword" class="form-control search-with-icon" placeholder="Search keyword" aria-controls="tabelProductModel">
                        </div>
                    </div>
                    <div class="col-md-3 d-flex align-items-center">
                        <label for="filterProductBrand" style="margin-right: 10px; margin-top: 6px"">Merek:</label>
                        <select id="filterProductBrand" class="form-control" runat="server"></select>
                    </div>
                </div>

                <!-- Tabel Product Model -->
                <table id="tabelProductModel" class="table table-bordered table-striped" style="width: 100%">
                    <thead>
                        <tr>
                            <th>No</th>
                            <th class="hidden-column">Merek</th>
                            <th>Nama Model</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Literal ID="ltTableRows" runat="server"></asp:Literal>
                    </tbody>
                </table>
                <!-- Penutup Tabel Product Model -->

                <!-- Modal Konfirmasi Penghapusan -->
                <div class="modal fade" id="deleteModal" tabindex="-1" role="dialog" aria-labelledby="deleteModalLabel" aria-hidden="true">
                    <div class="modal-dialog modal-dialog-centered" role="document">
                        <div class="modal-content" style="border-radius: 20px; padding: 20px;">
                            <div class="modal-header" style="border-bottom: none; padding-bottom: 12px; padding-top: 12px;">
                                <h5 class="modal-title" id="deleteModalLabel" style="font-weight: bold; font-size: 32px">Konfirmasi Hapus?</h5>
                            </div>
                            <div class="modal-body" style="padding-top: 0px;">
                                <p style="font-size: 17px; margin-bottom: 0px">Data model produk yang telah dihapus tidak dapat dipulihkan kembali.</p>
                                <asp:HiddenField ID="hfProductModelIDToDelete" runat="server" />
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
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf-autotable/3.5.24/jspdf.plugin.autotable.min.js"></script>
    <script>
        $(document).ready(function () {
            var table = $('#tabelProductModel').DataTable({
                "responsive": true,
                "lengthChange": false,
                "autoWidth": false,
                "dom": 'rt' +
                    '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
                "columnDefs": [
                    { "orderable": false, "targets": [3] }
                ]
            });

            // Filter Berdasarkan Merek
            $('select[id$=filterProductBrand]').on('change', function () {
                var selectedProductBrand = this.value;
                if (selectedProductBrand) {
                    table.column(1).search(selectedProductBrand).draw();
                } else {
                    table.column(1).search('').draw();
                }
            });

            // Filter berdasarkan Keyword
            $('#searchKeyword').on('keyup', function () {
                table.search(this.value).draw();
            });
        });
    </script>
    <!-- Modal Delete Product Model -->
    <script>
        function showDeleteModal(ProductModelId) {
            $('#<%= hfProductModelIDToDelete.ClientID %>').val(ProductModelId);
            $('#deleteModal').modal('show');
        }
    </script>
    <!-- Download Data -->
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            document.getElementById("downloadPdf").addEventListener("click", function (event) {
                event.preventDefault();

                if ($.fn.dataTable.isDataTable('#tabelProductModel')) {
                    $('#tabelProductModel').DataTable().page.len(-1).draw(); 
                }

                const { jsPDF } = window.jspdf;
                var doc = new jsPDF('landscape', 'mm', 'a4');

                const getCurrentDate = () => {
                    const today = new Date();
                    const day = today.getDate();
                    const monthNames = [
                        "Januari", "Februari", "Maret", "April", "Mei", "Juni",
                        "Juli", "Agustus", "September", "Oktober", "November", "Desember"
                    ];
                    const month = monthNames[today.getMonth()];
                    const year = today.getFullYear();
                    return `${day} ${month} ${year}`;
                };

                getBase64ImageFromURL('/Admin LTE/dist/img/Head.png', function (headerBase64) {
                    // Tambahkan gambar header full (x, y, width, height) menyesuaikan dengan A4 landscape
                    doc.addImage(headerBase64, 'PNG', 0, 0, 297, 40);

                    doc.setFont('helvetica', 'bold');
                    doc.setFontSize(15);
                    doc.text('REKAPITULASI DATA MODEL PRODUK', 148.5, 54, { align: 'center' });

                    doc.setFont('helvetica', 'normal');
                    doc.setFontSize(10);
                    doc.text(`Diunduh pada: ${getCurrentDate()}`, 148.5, 61, { align: 'center' });

                    const table = document.getElementById('tabelProductModel');
                    const headers = Array.from(table.querySelectorAll('thead th'))
                        .slice(0, -1) // Menghapus kolom terakhir ("Actions")
                        .map(th => th.textContent.trim());
                    const rows = Array.from(table.querySelectorAll('tbody tr')).map(tr => {
                        return Array.from(tr.querySelectorAll('td'))
                            .slice(0, -1) // Menghapus kolom terakhir ("Actions")
                            .map(td => td.textContent.trim());
                    });

                    doc.autoTable({
                        head: [headers],
                        body: rows,
                        startY: 70,
                        theme: 'striped',
                        headStyles: {
                            fillColor: [3, 14, 46],
                            halign: 'center',
                            lineWidth: 0.1,
                            lineColor: [169, 169, 169]
                        },
                        bodyStyles: {
                            halign: 'left',
                            lineWidth: 0.1,
                            lineColor: [169, 169, 169]
                        },
                        styles: {
                            fontSize: 10,
                            cellPadding: 3,
                            lineWidth: 0.1,
                            lineColor: [169, 169, 169]
                        },
                        columnStyles: {
                            0: { cellWidth: 15 },
                            1: { cellWidth: 'auto' },
                            2: { cellWidth: 'auto' }
                        }
                    });

                    doc.save('ProductModelData.pdf');
                });
            });
        });

        // Fungsi untuk mengonversi gambar ke Base64
        function getBase64ImageFromURL(url, callback) {
            var img = new Image();
            img.setAttribute('crossOrigin', 'anonymous');
            img.onload = function () {
                var canvas = document.createElement("canvas");
                canvas.width = this.width;
                canvas.height = this.height;
                var ctx = canvas.getContext("2d");
                ctx.drawImage(this, 0, 0);
                var dataURL = canvas.toDataURL("image/png");
                callback(dataURL);
            };
            img.src = url;
        }
    </script>

</asp:Content>