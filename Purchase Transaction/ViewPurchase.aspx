<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewPurchase.aspx.cs" Inherits="Unipro_Store.Purchase_Transaction.ViewPurchase" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card card-info">
        <!-- Bagian Ikon Pembelian dan Label -->
        <div class="card-header text-center" style="background-color: white; border-bottom: none;">
            <i class="fas fa-upload" style="font-size: 58px; margin-top: 20px; color: #6c757d"></i>
            <p style="font-size: 19px; font-weight: 600; margin-top: 10px; margin-bottom: 0px; color: #6c757d">Detail Transaksi</p>
        </div>
        <!-- Bagian Form Input -->
        <div class="card-body">
            <div class="form-group row">
                <label for="supplierName" class="col-sm-1-5 col-form-label">Nama Supplier</label>
                <div class="col-sm-3">
                    <asp:TextBox ID="supplierName" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>
                <div class="col-sm-3"></div>
                <label for="purchaseDate" class="col-sm-1-5 col-form-label">Tanggal Transaksi</label>
                <div class="col-sm-3">
                    <asp:TextBox ID="purchaseDate" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>
            </div>
            <!-- Modal Tampilkan Faktur -->
            <div class="modal fade" id="notaModal" tabindex="-1" aria-labelledby="notaModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                        </div>
                        <div class="modal-body text-center">
                            <asp:Image ID="imgNota" runat="server" CssClass="img-fluid" />
                        </div>
                        <div class="modal-footer">
                        </div>
                    </div>
                </div>
            </div>
            <!-- Tabel detail transaksi -->
            <table id="tabelPurchaseTransactionDetail" class="table table-bordered table-striped" style="width: 100%; padding-right: 0px">
                <thead>
                    <tr>
                        <th>Kode</th>
                        <th>Gambar</th>
                        <th>Nama Produk</th>
                        <th>Harga Satuan</th>
                        <th>Qty</th>
                        <th>Subtotal</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptProducts" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("ProductId") %></td>
                                <td>
                                    <img src='<%# Eval("Image") != DBNull.Value ? "data:image/png;base64," + Convert.ToBase64String((byte[])Eval("Image")) : "No image" %>' width="50" height="50" alt="Image" />
                                </td>
                                <td><%# Eval("NamaProduk") %></td>
                                <td class="product-price" data-price='<%# Eval("Harga") %>'><%# String.Format("Rp {0:N0}", Eval("Harga")) %></td>
                                <td><%# Eval("Kuantitas") %></td>
                                <td class="product-subtotal">
                                    <%# String.Format("Rp {0:N0}", (Convert.ToDecimal(Eval("Harga")) * Convert.ToDecimal(Eval("Kuantitas")))) %>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
            <!-- Penutup tabel detail transaksi -->
        </div>
        <!-- Penutup Form Input -->
        <!-- Card Footer -->
        <div class="card-footer d-flex justify-content-between align-items-center">
            <!-- Kolom Total Transaksi di sebelah kiri -->
            <div class="total-transaction d-flex flex-column align-items-start">
                <div class="d-flex align-items-center">
                    <span style="font-size: 15px">Total Transaksi</span>
                </div>
                <strong style="font-size: 25px">
                    <i class="fas fa-coins" style="margin-right: 10px"></i>
                    <asp:Literal ID="ltlGrandTotal" runat="server"></asp:Literal>
                </strong>
            </div>
            <!-- Tombol Kembali di sebelah kanan -->
            <div class="ml-auto">
                <div>
                    <asp:Button Text="Kembali" runat="server" class="btn btn-secondary" ID="Cancel" OnClientClick="event.preventDefault(); window.location='Purchase.aspx';" />
                    <asp:Button CssClass="btn btn-secondary" runat="server" Text="Lihat Faktur" ID="btnFaktur" Style="background-color: #386bba; border-color: #386bba; color: white; font-size: 15px; margin-right: 10px;" OnClick="btnFaktur_Click" />
                </div>
            </div>
        </div>
        <!-- Penutup Card Footer -->
    </div>
    <!-- Penutup Card Keseluruhan -->
</asp:Content>
