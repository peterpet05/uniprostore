<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SalesReport.aspx.cs" Inherits="Unipro_Store.Report.SalesReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Bagian Filter Laporan -->
    <div class="card card-info">
        <div class="card-header text-center" style="background-color: white; border-bottom: none;">
            <p style="font-size: 18px; font-weight: 600; margin-top: 0px; margin-bottom: 5px; color: #6c757d">Filter Laporan</p>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-sm-3">
                    <div class="icon-box">
                        <i class="fas fa-tags"></i>
                        <p style="margin-bottom: 10px">Subkategori</p>
                        <asp:DropDownList ID="ddlSubcategory" runat="server" CssClass="form-control dropdown-list" />
                    </div>
                </div>
                <div class="col-sm-3">
                    <div class="icon-box">
                        <i class="fas fa-globe"></i>
                        <p style="margin-bottom: 10px">Merek</p>
                        <asp:DropDownList ID="ddlProductBrand" runat="server" CssClass="form-control dropdown-list" />
                    </div>
                </div>
                <div class="col-sm-3">
                    <div class="icon-box">
                        <i class="fas fa-store"></i>
                        <p style="margin-bottom: 10px">Media</p>
                        <asp:DropDownList ID="ddlSalesMedia" runat="server" CssClass="form-control dropdown-list" />
                    </div>
                </div>
                <div class="col-sm-3">
                    <div class="icon-box">
                        <i class="fas fa-id-card-alt"></i>
                        <p style="margin-bottom: 10px">Pencatat</p>
                        <asp:DropDownList ID="ddlUser" runat="server" CssClass="form-control dropdown-list" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Penutup Filter Laporan -->

    <!-- Bagian Rentang Waktu -->
    <div class="card card-info">
        <div class="card-header text-center" style="background-color: white; border-bottom: none;">
            <p style="font-size: 18px; font-weight: 600; margin-top: 0px; margin-bottom: 5px; color: #6c757d">Rentang Waktu Laporan</p>
        </div>
        <div class="card-body" style="padding-bottom:0px">
            <div class="form-group row">
                <label for="inputStartDate" class="col-sm-1-5 col-form-label">Tanggal Mulai</label>
                <div class="col-sm-10-5">
                    <input type="datetime-local" class="form-control" id="inputStartDate" runat="server" required="required" />
                </div>
            </div>
            <div class="form-group row">
                <label for="inputEndDate" class="col-sm-1-5 col-form-label">Tanggal Berakhir</label>
                <div class="col-sm-10-5">
                    <input type="datetime-local" class="form-control" id="inputEndDate" runat="server" required="required" />
                </div>
            </div>
        </div>
        <div class="card-footer d-flex justify-content-end" style="padding-bottom:20px">
            <asp:Button Text="Tampilkan Laporan" runat="server" class="btn btn-primary" ID="Show" OnClick="Show_Click" />
        </div>
    </div>
    <!-- Penutup Rentang Waktu -->
</asp:Content>
