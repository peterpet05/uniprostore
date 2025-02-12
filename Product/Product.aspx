<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Product.aspx.cs" Inherits="Unipro_Store.Product.Product" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
       <div class="col-sm-12 d-flex justify-content-between" style="margin-bottom:20px">
            <div>
                <asp:LinkButton runat="server" ID="create" OnClick="create_Click" CssClass="btn btn-primary" 
                    Style="background-color: #386bba; border-color: #386bba; color: white; font-size: 15px; margin-right: 10px;"> 
                    Add New Product<i class="fas fa-plus-circle" style="margin-left:10px"></i>
                </asp:LinkButton>
                <asp:LinkButton runat="server" ID="importCsv" CssClass="btn btn-primary" 
                    Style="background-color: #386bba; border-color: #386bba; color: white; font-size: 15px; margin-right: 10px;"
                    OnClientClick="showModal(); return false;"> 
                    Import from .CSV<i class="fas fa-file-upload" style="margin-left:10px"></i>
                </asp:LinkButton>
                <!-- Modal Import CSV -->
                <div id="csvModal" class="modal fade" tabindex="-1" role="dialog" aria-hidden="true">
                    <div class="modal-dialog modal-dialog-centered" role="document">
                        <div class="modal-content" style="border-radius: 20px; padding: 20px;">
                            <div class="modal-header" style="border-bottom: none; padding-bottom: 0px; padding-top: 0px;">
                                <h5 class="modal-title" style="font-weight: bold; font-size: 30px">Import Data from CSV</h5>
                            </div>

                            <div class="modal-body">
                                <asp:FileUpload ID="inputCsv" runat="server" CssClass="form-control" />
                            </div>
                            <div class="modal-footer" style="justify-content: space-between; padding-bottom: 0px; padding-top: 0px">
                                <button type="button" class="btn btn-secondary btn-lg" data-dismiss="modal" style="width: 45%; border-radius: 10px; font-weight: bold;">Batal</button>
                                <asp:Button runat="server" ID="uploadCsv" OnClick="importCsv_Click" Text="Upload CSV" CssClass="btn btn-success btn-lg" Style="width: 45%; border-radius: 10px; font-weight: bold; background-color: #579c48; border-color: #579c48"/>
                            </div>
                        </div>
                    </div>
                </div>
                <asp:ScriptManager runat="server" ID="ScriptManager"></asp:ScriptManager>
                <asp:LinkButton runat="server" CssClass="btn btn-primary" 
                    Style="background-color: #386bba; border-color: #386bba; color: white; font-size: 15px;"
                    OnClientClick="showCountModal(); return false;"> 
                    Count SS & ROP<i class="fas fa-calculator" style="margin-left:10px"></i>
                </asp:LinkButton>
                <!-- Modal Count SS + ROP -->
                <div class="modal fade" id="ssRopModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
                  <div class="modal-dialog modal-lg">
                    <div class="modal-content" style="border-radius: 20px; padding: 20px 20px 5px 20px;">
                      <div class="modal-header" style="border-bottom: none; padding-bottom: 12px; padding-top: 12px;">
                        <h5 class="modal-title" style="font-weight: bold; font-size: 30px">Hitung Safety Stock dan Reorder Point</h5>
                      </div>
                      <div class="modal-body">
                        <!-- UpdatePanel untuk Partial Postback -->
                        <asp:UpdatePanel runat="server" ID="updPanel" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="form-group row">
                                    <label for="ddlProduct" class="col-sm-1-5 col-form-label">Produk</label>
                                    <div class="col-sm-10-5">
                                        <asp:DropDownList ID="ddlProduct" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label for="inputStartDate" class="col-sm-1-5 col-form-label">Periode</label>
                                    <div class="col-sm-5" style="padding-left:0px; padding-right:0px">
                                        <input type="date" id="inputStartDate" runat="server" class="form-control" required="required" />
                                    </div>
                                    <label for="inputEndDate" class="col-sm-0-5 col-form-label">→</label>
                                    <div class="col-sm-5" style="padding-left:0px;">
                                        <input type="date" id="inputEndDate" runat="server" class="form-control" required="required" />
                                    </div>
                                </div>
                                <div id="resultSection" runat="server" style="display: none; padding-top:20px">
                                    <div class="form-group row">
                                        <div class="col-sm-6">
                                            <label for="txtCurrentStock">Stok tersedia (unit)</label>
                                            <asp:TextBox ID="txtCurrentStock" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-6">
                                            <label for="txtCountedDays">Total hari dihitung (hari)</label>
                                            <asp:TextBox ID="txtCountedDays" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <div class="col-sm-6">
                                            <label for="txtMaximalSold">Maksimal penjualan produk per hari (unit)</label>
                                            <asp:TextBox ID="txtMaximalSold" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-6">
                                            <label for="txtAverageSold">Rata-rata penjualan produk per hari (unit)</label>
                                            <asp:TextBox ID="txtAverageSold" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <div class="col-sm-6">
                                            <label for="txtMaximalLeadTime">Maksimal lead time (hari)</label>
                                            <asp:TextBox ID="txtMaximalLeadTime" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-6">
                                            <label for="txtAverageLeadTime">Rata-rata lead time (hari)</label>
                                            <asp:TextBox ID="txtAverageLeadTime" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <div class="col-sm-6">
                                            <label for="txtSafetyStock">Safety stock (unit)</label>
                                            <asp:TextBox ID="txtSafetyStock" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-6">
                                            <label for="txtReorderPoint">Reorder point (unit)</label>
                                            <asp:TextBox ID="txtReorderPoint" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                         <div class="modal-footer d-flex justify-content-between align-items-center" style="padding: 5px 0px 0px 0px; margin-right:-5px">
                            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" style="margin-bottom: 0;"></asp:Label>
                            <div>
                                <button type="button" class="btn btn-secondary" data-dismiss="modal" style="margin-right:10px">Kembali</button>
                                <asp:Button ID="btnCalculate" runat="server" CssClass="btn btn-primary" Text="Hitung" OnClick="btnCalculate_Click" />
                            </div>
                        </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                      </div>
                    </div>
                  </div>
                </div>
            </div>
            <button id="downloadPdf" class="btn btn-primary" 
                style="background-color: #386bba; border-color: #386bba; color: white; font-size: 15px"> 
                Download Data<i class="fas fa-file-download" style="margin-left:10px"></i>
            </button>
        </div>
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
                            <th style="width: 85px;">± Harga Beli</th>
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
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf-autotable/3.5.24/jspdf.plugin.autotable.min.js"></script>
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
    <!-- Modal Show Import -->
    <script>
        function showModal() {
            $('#csvModal').modal('show');
        }
    </script>
    <!-- Modal Show Count SS + ROP -->
    <script>
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            $('#ssRopModal').modal('show');
        });

        function showCountModal() {
            $('#ssRopModal').modal('show');
        }
    </script>
    <!-- Download Data -->
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            document.getElementById("downloadPdf").addEventListener("click", function (event) {
                event.preventDefault();

                if ($.fn.dataTable.isDataTable('#tabelProduct')) {
                    $('#tabelProduct').DataTable().page.len(-1).draw();
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
                    doc.text('REKAPITULASI DATA PRODUK', 148.5, 54, { align: 'center' });

                    doc.setFont('helvetica', 'normal');
                    doc.setFontSize(10);
                    doc.text(`Diunduh pada: ${getCurrentDate()}`, 148.5, 61, { align: 'center' });

                    const table = document.getElementById('tabelProduct');
                    const headers = Array.from(table.querySelectorAll('thead th'))
                        .filter((_, index) => index !== 3 && index !== 7 && index !== 8) // Menghapus kolom Merek Ponsel, Gambar, dan Actions
                        .map(th => th.textContent.trim());
                    const rows = Array.from(table.querySelectorAll('tbody tr')).map(tr => {
                        return Array.from(tr.querySelectorAll('td'))
                            .filter((_, index) => index !== 3 && index !== 7 && index !== 8) // Menghapus kolom Merek Ponsel, Gambar, dan Actions
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
                    });

                    doc.save('ProductData.pdf');
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