<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UpdateSales.aspx.cs" Inherits="Unipro_Store.Sales_Transaction.UpdateSales" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card card-info">
        <!-- Bagian Ikon Penjualan dan Label -->
        <div class="card-header text-center" style="background-color: white; border-bottom: none;">
            <i class="fas fa-download" style="font-size: 58px; margin-top: 20px; color: #6c757d"></i>
            <p style="font-size: 19px; font-weight: 600; margin-top: 10px; margin-bottom: 0px; color: #6c757d">Detail Transaksi</p>
        </div>
        <!-- Bagian Form Input -->
        <div class="card-body">
            <div class="form-group row">
                <label for="inputMedia" class="col-sm-1-5 col-form-label">Media Penjualan</label>
                <div class="col-sm-3">
                    <select id="inputMedia" class="form-control" runat="server" required>
                        <option value="" disabled selected>Pilih media</option>
                        <option>Tokopedia</option>
                        <option>Shopee</option>
                        <option>WhatsApp</option>
                        <option>Toko Offline</option>
                    </select>
                </div>
                <div class="col-sm-2"></div>
                <label for="productSearchBar" class="col-sm-1-5 col-form-label">Cari Produk</label>
                <div class="col-sm-4" style="position: relative; padding: 0; margin-left: -7px">
                    <div class="input-group">
                        <input type="text" id="searchProduct" placeholder="Masukkan kode atau nama produk" class="form-control">
                        <div class="input-group-append">
                            <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-primary" OnClick="btnAdd_Click" CausesValidation="false" Style="border-top-right-radius: 5px; border-bottom-right-radius: 5px;">
                                <i class="fas fa-plus"></i>
                            </asp:LinkButton>
                        </div>
                        <asp:HiddenField ID="hiddenProductId" runat="server" />
                        <asp:HiddenField ID="hiddenProductIdToRemove" runat="server" />
                    </div>
                    <div id="suggestions" class="list-group"></div>
                </div>
            </div>
            <!-- Tabel detail transaksi -->
            <table id="tabelSalesTransactionDetail" class="table table-bordered table-striped" style="width: 100%; padding-right: 0px">
                <thead>
                    <tr>
                        <th>Kode</th>
                        <th>Gambar</th>
                        <th>Nama Produk</th>
                        <th>Harga Satuan</th>
                        <th>Qty</th>
                        <th>Diskon (%)</th>
                        <th>Subtotal</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptProducts" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("ProductId") %></td>
                                <td>
                                    <img src='<%# !string.IsNullOrEmpty(Eval("ImageBase64").ToString()) ? "data:image/png;base64," + Eval("ImageBase64") : "No image" %>' width="50" height="50" alt="Image" />
                                </td>
                                <td><%# Eval("NamaProduk") %></td>
                                <td class="product-price" data-price='<%# Eval("Harga") %>'><%# String.Format("Rp {0:N0}", Eval("Harga")) %></td>
                                <td>
                                    <asp:TextBox ID="txtQty" runat="server" CssClass="form-control qty-input"
                                        Text='<%# Eval("Kuantitas") != null && Convert.ToInt32(Eval("Kuantitas")) > 0 ? Eval("Kuantitas") : "1" %>'
                                        TextMode="Number" Min="1" Max="99" Step="1" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtDisc" runat="server" CssClass="form-control discount-input" TextMode="Number" Min="0" Max="100" Step="0.01" Style="width: 65px;" Text='<%# String.Format("{0:0.##}", Eval("Diskon")) %>' />
                                </td>
                                <td class="product-subtotal">
                                    <%# String.Format("Rp {0:N0}", (Convert.ToDecimal(Eval("Harga")) * Convert.ToDecimal(Eval("Kuantitas")) * (1 - (Convert.ToDecimal(Eval("Diskon")) / 100)))) %>
                                </td>
                                <td>
                                    <button class='btn btn-danger' onclick='removeProduct("<%# Eval("ProductId") %>")'><i class='fas fa-trash-alt'></i></button>
                                </td>
                                <asp:HiddenField ID="hfProductId" Value='<%# Eval("ProductId") %>' runat="server" />
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
                <strong style="font-size: 25px"><i class="fas fa-coins" style="margin-right: 10px"></i><span id="grand-total" style="font-size: 30px">0</span></strong>
            </div>
            <!-- Tombol Batal dan Simpan di sebelah kanan -->
            <div class="ml-auto">
                <asp:Button Text="Batal" runat="server" class="btn btn-secondary" ID="Cancel" OnClientClick="event.preventDefault(); window.location='Sales.aspx';" />
                <asp:Button Text="Simpan" runat="server" class="btn btn-primary ml-2" ID="Update" OnClick="Update_Click" />
            </div>
        </div>
        <!-- Penutup Card Footer -->
    </div>
    <!-- Penutup Card Keseluruhan -->

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <!-- Menampilkan dropdown produk -->
    <script>
        $(document).ready(function () {
            var hiddenProductId = '<%= hiddenProductId.ClientID %>';
            $("#searchProduct").on("input", function () {
                var searchTerm = $(this).val();

                if (searchTerm.length > 0) {
                    $.ajax({
                        type: "POST",
                        url: "CreateSales.aspx/GetProductSuggestions",
                        data: JSON.stringify({ term: searchTerm }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            var suggestions = response.d;
                            var suggestionsList = $("#suggestions");
                            suggestionsList.empty();

                            $.each(suggestions, function (index, product) {
                                suggestionsList.append('<a href="#" class="list-group-item list-group-item-action" data-id="' + product.ProductId + '">' + product.NamaProduk + '</a>');
                            });

                            $("#suggestions").on("click", ".list-group-item", function (e) {
                                e.preventDefault();
                                var selectedProduct = $(this).text();
                                var selectedProductId = $(this).data("id");

                                $("#searchProduct").val(selectedProduct);
                                var hiddenProductField = $("#" + hiddenProductId);
                                hiddenProductField.val(selectedProductId);

                                $("#suggestions").empty();
                            });
                        },
                    });
                } else {
                    $("#suggestions").empty();
                }
            });
        });
    </script>
    <!-- Menghapus produk dari keranjang -->
    <script type="text/javascript">
        function removeProduct(productId) {
            document.getElementById('<%= hiddenProductIdToRemove.ClientID %>').value = productId;
            __doPostBack('btnRemove', '');
        }
    </script>
    <!-- Menghitung subtotal, grandtotal, dan validasi field kuantitas serta diskon -->
    <script type="text/javascript">
        $(document).ready(function () {

            // Fungsi untuk menghitung dan memperbarui subtotal
            function calculateSubtotal(row) {
                var price = parseFloat(row.find('.product-price').data('price'));
                var qty = parseFloat(row.find('.qty-input').val());
                var discount = parseFloat(row.find('.discount-input').val());

                if (isNaN(qty) || qty < 1) qty = 1;
                if (isNaN(discount) || discount < 0) discount = 0;
                if (discount > 100) discount = 100;

                var subtotal = (price * qty) * (1 - (discount / 100));

                row.find('.product-subtotal').text(subtotal.toLocaleString('id-ID', { style: 'currency', currency: 'IDR', minimumFractionDigits: 0 }));
            }

            // Fungsi untuk menghitung dan memperbarui grand total
            function updateGrandTotal() {
                var grandTotal = 0;

                $('.product-subtotal').each(function () {
                    var subtotal = parseFloat($(this).text().replace(/[^\d,-]/g, '').replace(',', '.'));
                    if (!isNaN(subtotal)) {
                        grandTotal += subtotal;
                    }
                });

                $('#grand-total').text(grandTotal.toLocaleString('id-ID', { style: 'currency', currency: 'IDR', minimumFractionDigits: 0 }));
            }

            // Event ketika kuantitas atau diskon berubah
            $(document).on('input', '.qty-input, .discount-input', function () {
                var row = $(this).closest('tr');

                calculateSubtotal(row);
                updateGrandTotal();
            });

            // Event saat blur pada kuantitas
            $(document).on('blur', '.qty-input', function () {
                var qty = parseInt($(this).val());

                if (qty < 1 || isNaN(qty)) {
                    $(this).val(1);
                } else if (qty > 99) {
                    $(this).val(99);
                }

                $(this).trigger('input');
            });

            // Event saat blur pada diskon
            $(document).on('blur', '.discount-input', function () {
                var discount = $(this).val();

                if (discount === '' || isNaN(parseFloat(discount))) {
                    $(this).val(0);
                } else if (parseFloat(discount) > 100) {
                    $(this).val(100);
                } else if (parseFloat(discount) < 0) {
                    $(this).val(0);
                }

                $(this).trigger('input');
            });

            $('.qty-input, .discount-input').each(function () {
                if ($(this).hasClass('qty-input') && ($(this).val() === '' || isNaN(parseInt($(this).val())))) {
                    $(this).val(1);
                }

                if ($(this).hasClass('discount-input') && ($(this).val() === '' || isNaN(parseFloat($(this).val())))) {
                    $(this).val(0);
                }
                $(this).trigger('input');
            });
        });
    </script>
</asp:Content>