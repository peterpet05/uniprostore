<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SalesReportResult.aspx.cs" Inherits="Unipro_Store.Report.SalesReportResult" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row" style="margin-top: -15px">
        <div class="col-sm-12">
            <p style="margin-bottom: 25px">Menampilkan hasil laporan berdasarkan filter dan rentang waktu yang diterapkan</p>
            <!-- Tabel Hasil Laporan Penjualan -->
            <div class="card-body table-responsive p-0" style="height: 420px;">
                <table class="table table-bordered table-striped table-head-fixed text-nowrap" id="tabelSalesReportResult">
                    <thead>
                        <tr>
                            <th>Tanggal</th>
                            <th>Kode Transaksi</th>
                            <th>Media</th>
                            <th>Nama Produk</th>
                            <th>Kuantitas</th>
                            <th>Total Harga</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptSalesReport" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><%# string.Format(new System.Globalization.CultureInfo("id-ID"), "{0:dd/MM/yyyy}", Eval("Tanggal")) %></td>
                                    <td><%# Eval("KodeTransaksi") %></td>
                                    <td><%# Eval("Media") %></td>
                                    <td><%# Eval("NamaProduk") %></td>
                                    <td><%# Eval("Kuantitas") %></td>
                                    <td><%# Eval("TotalHarga", "Rp {0:N0}") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
            <!-- Penutup Tabel Hasil Laporan Penjualan -->
            <div class="d-flex justify-content-end mt-3">
                <button id="downloadExcel" class="btn btn-secondary">Download .xlsx</button>
                <button id="downloadPdf" class="btn btn-primary ml-2">Download .pdf</button>
            </div>
        </div>
    </div>

    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf-autotable/3.5.24/jspdf.plugin.autotable.min.js"></script>
    <script src="https://unpkg.com/xlsx-populate/browser/xlsx-populate.min.js"></script>

    <!-- Download as PDF dan Download as XLSX -->
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            document.getElementById("downloadPdf").addEventListener("click", function (event) {
                event.preventDefault(); 

                const { jsPDF } = window.jspdf;
                var doc = new jsPDF('landscape', 'mm', 'a4'); 
           
                startDate = String(startDate);
                endDate = String(endDate);

                getBase64ImageFromURL('/Admin LTE/dist/img/Head.png', function (headerBase64) {
                    // Tambahkan gambar header full (x, y, width, height) menyesuaikan dengan A4 landscape
                    doc.addImage(headerBase64, 'PNG', 0, 0, 297, 40); 

                    doc.setFont('helvetica', 'bold');
                    doc.setFontSize(15); 
                    doc.text('LAPORAN PENJUALAN PRODUK', 148.5, 54, { align: 'center' });

                    doc.setFont('helvetica', 'normal');
                    doc.setFontSize(10); 
                    doc.text(`Periode ${startDate} s/d ${endDate}`, 148.5, 61, { align: 'center' });

                    var table = document.getElementById('tabelSalesReportResult');
                    var totalHarga = 0;

                    // Loop melalui setiap baris tabel untuk menghitung total harga 
                    for (var i = 1, row; row = table.rows[i]; i++) {
                        var harga = parseFloat(row.cells[5].innerText.replace(/[^\d,-]/g, '').replace(',', '.')); 
                        if (!isNaN(harga)) {
                            totalHarga += harga; 
                        }
                    }

                    var formattedTotalHarga = `Rp ${totalHarga.toLocaleString('id-ID')}`;

                    doc.autoTable({
                        html: '#tabelSalesReportResult',
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
                        didDrawPage: function (data) {
                        }
                    });

                    var finalY = doc.previousAutoTable.finalY || 80; 

                    doc.setFillColor(3, 14, 46); 
                    doc.rect(14, finalY + 5, 269, 10, 'F'); 

                    doc.setFontSize(11);
                    doc.setFont('helvetica', 'bold'); 
                    doc.setTextColor(255, 255, 255); 
                    doc.text('Total pembayaran seluruh produk terjual: ' + formattedTotalHarga, 148.5, finalY + 11, { align: 'center' }); // Teks di tengah

                    doc.save('LaporanPenjualan.pdf');
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

        document.getElementById("downloadExcel").addEventListener("click", function (event) {
            event.preventDefault(); 

            XlsxPopulate.fromBlankAsync()
                .then(workbook => {
                    const sheet = workbook.sheet(0);

                    sheet.cell("A1").value("UNIPRO STORE").style({
                        bold: true,
                        fontSize: 22,
                        horizontalAlignment: 'center',
                        verticalAlignment: 'center',
                        fontColor: "FFFFFF", 
                        fill: "030e2e" 
                    });
                    sheet.cell("A2").value("Laporan Penjualan Produk").style({
                        fontSize: 14,
                        horizontalAlignment: 'center',
                        verticalAlignment: 'center',
                        fontColor: "FFFFFF", 
                        fill: "030e2e" 
                    });
                    sheet.cell("A3").value(`Periode ${startDate} s/d ${endDate}`).style({
                        fontSize: 14,
                        horizontalAlignment: 'center',
                        verticalAlignment: 'center',
                        fontColor: "FFFFFF", 
                        fill: "030e2e" 
                    });

                    sheet.range("A1:F1").merged(true); 
                    sheet.range("A2:F2").merged(true); 
                    sheet.range("A3:F3").merged(true); 

                    sheet.row(1).height(30); 
                    sheet.row(2).height(25); 
                    sheet.row(3).height(20); 
                    sheet.row(5).height(30); 

                    const headers = ['Tanggal', 'Kode Transaksi', 'Media', 'Nama Produk', 'Kuantitas', 'Total Harga'];
                    for (let i = 0; i < headers.length; i++) {
                        const headerCell = sheet.cell(5, i + 1).value(headers[i]);
                        headerCell.style({
                            bold: true,
                            fontColor: "FFFFFF", 
                            fill: "000033", 
                            horizontalAlignment: 'center',
                            verticalAlignment: 'center',
                            border: true
                        });
                    }

                    sheet.column("A").width(20); // Tanggal
                    sheet.column("B").width(30); // Kode Transaksi
                    sheet.column("C").width(25); // Media
                    sheet.column("D").width(75); // Nama Produk
                    sheet.column("E").width(15); // Kuantitas
                    sheet.column("F").width(25); // Total Harga

                    var table = document.getElementById('tabelSalesReportResult');
                    var tableRows = table.getElementsByTagName('tr');
                    var rowIndex = 6; 

                    for (var i = 1; i < tableRows.length; i++) { 
                        var cells = tableRows[i].getElementsByTagName('td');
                        for (var j = 0; j < cells.length; j++) {
                            var cellRef = sheet.cell(rowIndex, j + 1); 

                            if (j === 3) { 
                                cellRef.style({
                                    horizontalAlignment: 'left', // Rata kiri untuk kolom Nama Produk
                                    verticalAlignment: 'center',
                                    border: true 
                                });
                            } else {
                                cellRef.style({
                                    horizontalAlignment: 'center', // Rata tengah untuk kolom lainnya
                                    verticalAlignment: 'center',
                                    border: true 
                                });
                            }

                            cellRef.value(cells[j].innerText); 
                        }
                        sheet.row(rowIndex).height(30); 
                        rowIndex++;
                    }

                    return workbook.outputAsync("blob");
                })
                .then(function (blob) {
                    // Buat link untuk mendownload file
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement("a");
                    a.href = url;
                    a.download = "LaporanPenjualan.xlsx";
                    document.body.appendChild(a);
                    a.click();
                    document.body.removeChild(a);
                });
        });
    </script>
</asp:Content>