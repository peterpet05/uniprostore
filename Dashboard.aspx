<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Unipro_Store.WebForm2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <!-- Widget Total Produk -->
        <div class="col-lg-3 col-6">
            <div class="small-box" style="background-color: #062e5f">
                <div class="inner">
                    <h3 style="color: rgba(255, 255, 255, 0.8)">
                        <asp:Literal ID="lblTotalProducts" runat="server" /></h3>
                    <p style="color: rgba(255, 255, 255, 0.8)">Total Produk</p>
                </div>
                <div class="icon">
                    <i style="color: rgba(255, 255, 255, 0.3)" class="fas fa-boxes"></i>
                </div>
                <a style="background-color: rgba(255, 255, 255, 0.15)" href="Product/Product.aspx" class="small-box-footer">Tampilkan<i class="fas fa-arrow-circle-right" style="margin-left: 7px"></i></a>
            </div>
        </div>
        <!-- Widget Total Supplier -->
        <div class="col-lg-3 col-6">
            <div class="small-box" style="background-color: #1161a0">
                <div class="inner">
                    <h3 style="color: rgba(255, 255, 255, 0.8)">
                        <asp:Literal ID="lblTotalSuppliers" runat="server" /></h3>
                    <p style="color: rgba(255, 255, 255, 0.8)">Total Supplier</p>
                </div>
                <div class="icon">
                    <i style="color: rgba(255, 255, 255, 0.3)" class="fas fa-truck"></i>
                </div>
                <a style="background-color: rgba(255, 255, 255, 0.15)" href="Supplier/Supplier.aspx" class="small-box-footer">Tampilkan<i class="fas fa-arrow-circle-right" style="margin-left: 7px"></i></a>
            </div>
        </div>
        <!-- Widget Total Transaksi Penjualan -->
        <div class="col-lg-3 col-6">
            <div class="small-box" style="background-color: #338bc4">
                <div class="inner">
                    <h3 style="color: rgba(255, 255, 255, 0.8)">
                        <asp:Literal ID="lblTotalSales" runat="server" /></h3>
                    <p style="color: rgba(255, 255, 255, 0.8)">Total Penjualan</p>
                </div>
                <div class="icon">
                    <i style="color: rgba(255, 255, 255, 0.3)" class="fas fa-download"></i>
                </div>
                <a style="background-color: rgba(255, 255, 255, 0.15)" href="Sales Transaction/Sales.aspx" class="small-box-footer">Tampilkan<i class="fas fa-arrow-circle-right" style="margin-left: 7px"></i></a>
            </div>
        </div>
        <!-- Widget Total Transaksi Pembelian -->
        <div class="col-lg-3 col-6">
            <div class="small-box" style="background-color: #49afcc">
                <div class="inner">
                    <h3 style="color: rgba(255, 255, 255, 0.8)">
                        <asp:Literal ID="lblTotalPurchases" runat="server" /></h3>
                    <p style="color: rgba(255, 255, 255, 0.8)">Total Pembelian</p>
                </div>
                <div class="icon">
                    <i style="color: rgba(255, 255, 255, 0.3)" class="fas fa-upload"></i>
                </div>
                <a style="background-color: rgba(255, 255, 255, 0.15)" href="Purchase Transaction/Purchase.aspx" class="small-box-footer">Tampilkan<i class="fas fa-arrow-circle-right" style="margin-left: 7px"></i></a>
            </div>
        </div>
    </div>

    <div class="row">
        <!-- Widget Top 5 Produk Terlaris -->
        <div class="col-sm-6">
            <div class="card" style="height:305px">
                <div class="card-header">
                    <h3 class="card-title" style="font-weight: bold; padding-top: 8px; padding-bottom: 8px">Top 5 Produk Terlaris</h3>
                    <div class="card-tools">
                        <div class="input-group" style="margin-top: 0.1px">
                            <select id="quarterPicker" name="quarterPicker" class="form-control" style="max-width: 100px;" onchange="quarterChanged()">
                                <option value="Q1">Q1</option>
                                <option value="Q2">Q2</option>
                                <option value="Q3">Q3</option>
                                <option value="Q4">Q4</option>
                            </select>
                            <input type="number" id="yearPicker" name="yearPicker" class="form-control" value="2024" min="2018" max="2050" style="max-width: 100px;" onchange="quarterChanged()" />
                        </div>
                    </div>
                </div>
                <div class="card-body d-flex justify-content-center align-items-center" style="padding-bottom: 15px; min-height: 200px;">
                    <div class="row">
                        <!-- Donut Chart -->
                        <div class="col-md-5-5">
                            <div class="chart-responsive">
                                <canvas id="topProductsChart" height="169"></canvas>
                            </div>
                        </div>
                        <!-- Legend -->
                        <div class="col-md-6-5">
                            <ul class="chart-legend clearfix">
                                <asp:Repeater ID="rptTopProducts" runat="server">
                                    <ItemTemplate>
                                        <li class="d-flex align-items-center mb-1-5">
                                            <div class="circle" style="background-color: <%# Eval("Color") %>"></div>
                                            <span class="ml-2"><%# Eval("NamaProduk") %> (<%# Eval("TotalTerjual") %>)</span>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </div>
                    </div>
                    <asp:Label ID="lblNoData" runat="server" Text="Tidak ada transaksi " Visible="False" CssClass="no-data-message" />
                </div>
            </div>
        </div>
        <!-- Widget Statistik Media Penjualan -->
        <div class="col-sm-6">
            <div class="card">
                <div class="card-header border-0">
                    <h3 class="card-title" style="font-weight: bold; padding-top: 8px; padding-bottom: 8px">Statistik Media Penjualan</h3>
                    <div class="card-tools">
                        <input type="month" id="monthPicker" name="monthPicker" class="form-control" style="max-width: 200px;" onchange="fetchTransactions()"/>
                        <asp:HiddenField ID="lastSelectedMonth" runat="server" />
                    </div>
                </div>
                <div class="card-body table-responsive p-0">
                    <table class="table table-striped table-valign-middle">
                        <tbody>
                            <asp:Repeater ID="rptSalesMedia" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td style="height:60px">
                                            <img src="<%# ResolveUrl(GetIcon(Eval("salesMedia").ToString())) %>" style="width: 25px; height: 25px; margin-right: 7px" />
                                            <%# Eval("salesMedia") %>
                                        </td>
                                        <td><%# FormatPercentage(Eval("percentage")) %>%</td>
                                        <td>
                                            <small class="<%# GetTrendClass(Eval("trend").ToString()) %>">
                                                <i class="fas <%# GetTrendIcon(Eval("trend").ToString()) %> mr-1"></i><%# FormatPercentage(Eval("trend")) %>%
                                            </small>
                                        </td>
                                        <td><%# Eval("totalTransactions") %> Transaksi</td>
                                        <td>
                                            <asp:HiddenField ID="hfTotalTransactions" Value='<%# Eval("totalTransactions") %>' runat="server" />
                                            <a href='<%# GetSalesTransactionUrl(Eval("salesMedia").ToString(), ViewState["SelectedMonth"].ToString()) %>' class="text-muted">
                                                <i class="fas fa-search"></i>
                                            </a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <!-- Widget Total Omset Harian -->
        <div class="col-sm-12">
            <div class="card">
                <div class="card-header">
                    <h3 class="card-title" style="font-weight: bold; padding-top: 8px; padding-bottom: 8px">Total Omset Harian</h3>
                    <div class="card-tools">
                        <input type="month" id="monthPicker2" name="monthPicker2" class="form-control" style="max-width: 200px;" onchange="monthChanged2()"/>
                        <asp:HiddenField ID="lastSelectedMonth2" runat="server" />
                    </div>
                </div>
                <div class="card-body" style="height: 300px;">
                    <canvas id="omsetChart" style="max-height: 100%; max-width: 100%;"></canvas>
                </div>
            </div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/chart.js@3.9.1/dist/chart.min.js"></script>
    <!-- Month Picker Widget Statistik Media Penjualan -->
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            var storedMonth = document.getElementById('<%= lastSelectedMonth.ClientID %>').value;

            if (!storedMonth) {
                var today = new Date();
                var year = today.getFullYear();
                var month = ('0' + (today.getMonth() + 1)).slice(-2);
                storedMonth = year + '-' + month;
                document.getElementById('monthPicker').value = storedMonth;
            } else {
                document.getElementById('monthPicker').value = storedMonth;
            }

            fetchTransactions(false);
        });

        function fetchTransactions(shouldPostBack = true) {
            var month = document.getElementById('monthPicker').value;
            var lastSelectedMonth = document.getElementById('<%= lastSelectedMonth.ClientID %>').value;

            if (month && month !== lastSelectedMonth) {
                document.getElementById('<%= lastSelectedMonth.ClientID %>').value = month;

                if (shouldPostBack) {
                    __doPostBack('MonthPicker', month);
                }
            }
        }
    </script>

    <!-- Load Donut Chart dan Quater + Year Picker Widget Top 5 Produk Terlaris -->
    <script>
        window.onload = function () {
            Chart.defaults.global.legend.display = false;

            var ctx = document.getElementById('topProductsChart').getContext('2d');
            var topProductsChart = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: productNames,
                    datasets: [{
                        data: totalSales,
                        backgroundColor: ['#062e5f', '#1161a0', '#0252a8', '#49afcc', '#338bc4'],
                        borderColor: ['#ffffff'],
                        borderWidth: 2
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    cutoutPercentage: 70,
                    plugins: {
                        legend: {
                            display: false,
                        },
                        tooltip: {
                            enabled: true
                        }
                    }
                }
            });
        };

        function quarterChanged() {
            var selectedQuarter = document.getElementById('quarterPicker').value;
            var selectedYear = document.getElementById('yearPicker').value;

            __doPostBack('QuarterPicker', selectedQuarter + ',' + selectedYear);
        }

        function monthChanged() {
            var selectedMonth = document.getElementById('monthPicker').value;

            __doPostBack('MonthPicker', selectedMonth);
        }
    </script>
    
     <!-- Load Bar Chart dan Month Picker Widget Total Omset Harian -->
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            Chart.defaults.global.legend.display = false;
            Chart.defaults.global.tooltips.callbacks.title = function () { };

            var ctx = document.getElementById('omsetChart').getContext('2d');
            var omsetChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: days,
                    datasets: [{

                        data: totalOmset,
                        backgroundColor: '#1161a0',
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        xAxes: [{
                            scaleLabel: {
                                display: true,
                                labelString: 'Tanggal'
                            },
                            grid: {
                                display: true
                            },
                            ticks: {
                                autoSkip: false
                            }
                        }],
                        yAxes: [{
                            scaleLabel: {
                                display: true,
                                labelString: 'Indonesia Rupiah (IDR)'
                            },
                            grid: {
                                display: true,
                                color: "black"
                            },
                            ticks: {
                                beginAtZero: true,
                                callback: function (value) {
                                    return value + 'K';
                                }
                            }
                        }]
                    },
                    tooltips: {
                        callbacks: {
                            label: function (tooltipItem, data) {
                                var index = tooltipItem.index;
                                return 'Total Omset: Rp ' + totalOmsetRaw[index];
                            }
                        }
                    }
                },
            });
        });

        function monthChanged2() {
            var selectedMonth2 = document.getElementById('monthPicker2').value;
            __doPostBack('MonthPicker2', selectedMonth2);
        }
    </script>

</asp:Content>