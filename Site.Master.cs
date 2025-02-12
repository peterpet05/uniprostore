using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI;

namespace Unipro_Store
{
	public partial class Site1 : System.Web.UI.MasterPage
	{
		public string NotificationsCount { get; set; }
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["Username"] == null)
			{
				Response.Redirect("~/LoginPage.aspx");
			}

			string role = Session["Role"] as string;

			// Menyembunyikan menu tertentu jika role bukan Owner
			if (role != "Owner")
			{
				usersNavLink.Visible = false;
				laporanpenjualanNavLink.Visible = false;
				laporanpembelianNavLink.Visible = false;
				laporanheader.Visible = false;
			}

			SetActivePage();
			BindNotifications();

			if (!IsPostBack)
			{
				string pageName = System.IO.Path.GetFileName(Request.Path).ToLower();

				var pageHeaders = new Dictionary<string, string>
				{
					{ "dashboard.aspx", "Dashboard" },
					{ "users.aspx", "User" },
					{ "createuser.aspx", "Tambah User" },
					{ "updateuser.aspx", "Edit User" },
					{ "supplier.aspx", "Supplier" },
					{ "createsupplier.aspx", "Tambah Supplier" },
					{ "updatesupplier.aspx", "Edit Supplier" },
					{ "product.aspx", "Produk" },
					{ "createproduct.aspx", "Tambah Produk" },
					{ "createvariation.aspx", "Tambah Variasi Produk" },
					{ "updateproduct.aspx", "Edit Produk" },
					{ "category.aspx", "Kategori" },
					{ "createcategory.aspx", "Tambah Kategori" },
					{ "updatecategory.aspx", "Edit Kategori" },
					{ "subcategory.aspx", "Subkategori" },
					{ "createsubcategory.aspx", "Tambah Subkategori" },
					{ "updatesubcategory.aspx", "Edit Subkategori" },
					{ "productbrand.aspx", "Merek Produk" },
					{ "createproductbrand.aspx", "Tambah Merek Produk" },
					{ "updateproductbrand.aspx", "Edit Merek Produk" },
					{ "productmodel.aspx", "Model Produk" },
					{ "createproductmodel.aspx", "Tambah Model Produk" },
					{ "updateproductmodel.aspx", "Edit Model Produk" },
					{ "phonebrand.aspx", "Brand Ponsel" },
					{ "createphonebrand.aspx", "Tambah Brand Ponsel" },
					{ "updatephonebrand.aspx", "Edit Brand Ponsel" },
					{ "phonetype.aspx", "Tipe Ponsel" },
					{ "createphonetype.aspx", "Tambah Tipe Ponsel" },
					{ "updatephonetype.aspx", "Edit Tipe Ponsel" },
					{ "productcolor.aspx", "Warna Produk" },
					{ "createproductcolor.aspx", "Tambah Warna Produk" },
					{ "updateproductcolor.aspx", "Edit Warna Produk" },
					{ "sales.aspx", "Daftar Penjualan" },
					{ "viewsales.aspx", "Detail Transaksi Penjualan" },
					{ "createsales.aspx", "Tambah Transaksi Penjualan" },
					{ "updatesales.aspx", "Edit Transaksi Penjualan" },
					{ "purchase.aspx", "Daftar Pembelian" },
					{ "viewpurchase.aspx", "Detail Transaksi Pembelian" },
					{ "createpurchase.aspx", "Tambah Transaksi Pembelian" },
					{ "updatepurchase.aspx", "Edit Transaksi Pembelian" },
					{ "salesreport.aspx", "Laporan Penjualan" },
					{ "salesreportresult.aspx", "Laporan Penjualan" },
					{ "purchasereport.aspx", "Laporan Pembelian" },
					{ "purchasereportresult.aspx", "Laporan Pembelian" }
				};

				if (pageHeaders.ContainsKey(pageName))
				{
					PageHeader.Text = pageHeaders[pageName];
				}
			}
		}

		private void BindNotifications()
		{
			Unipro_Store.Models.Function.CommonFunction commonFunction = new Unipro_Store.Models.Function.CommonFunction();

			// Query untuk mengambil produk dengan stok kurang dari atau sama dengan 3
			string query = @"
						SELECT CONCAT(sc.subCategoryName, ' ', pb.productBrandName, ' ', pm.productModelName, ' ', phb.phoneBrandName, ' ', pt.phoneTypeName) AS productName, p.stock 
						FROM Product p
						JOIN Subcategory sc ON p.subCategoryId = sc.subCategoryId
						JOIN ProductModel pm ON p.productModelId = pm.productModelId
						JOIN ProductBrand pb ON pm.productBrandId = pb.productBrandId
						JOIN PhoneType pt ON p.phoneTypeId = pt.phoneTypeId
						JOIN PhoneBrand phb ON pt.phoneBrandId = phb.phoneBrandId
						WHERE p.stock <= 3
						ORDER BY p.stock ASC";

			DataTable dt = commonFunction.Fetch(query);

			rptNotifications.DataSource = dt;
			rptNotifications.DataBind();

			NotificationsCount = dt.Rows.Count.ToString();
		}

		private void SetActivePage()
		{
			string pageName = System.IO.Path.GetFileName(Request.Path).ToLower();

			if (pageName.Equals("dashboard.aspx", StringComparison.OrdinalIgnoreCase))
			{
				dashboardNavLink.Attributes["class"] = "nav-link active";
			}
			else if (pageName.Equals("users.aspx", StringComparison.OrdinalIgnoreCase) || pageName.Equals("createuser.aspx", StringComparison.OrdinalIgnoreCase) || pageName.Equals("updateuser.aspx", StringComparison.OrdinalIgnoreCase))
			{
				usersNavLink.Attributes["class"] = "nav-link active";
			}
			else if (pageName.Equals("supplier.aspx", StringComparison.OrdinalIgnoreCase) || pageName.Equals("createsupplier.aspx", StringComparison.OrdinalIgnoreCase) || pageName.Equals("updatesupplier.aspx", StringComparison.OrdinalIgnoreCase))
			{
				supplierNavLink.Attributes["class"] = "nav-link active";
			}
			else if (pageName.Equals("product.aspx", StringComparison.OrdinalIgnoreCase) || pageName.Equals("createproduct.aspx", StringComparison.OrdinalIgnoreCase) || pageName.Equals("createvariation.aspx", StringComparison.OrdinalIgnoreCase) || pageName.Equals("updateproduct.aspx", StringComparison.OrdinalIgnoreCase))
			{
				produkNavLink.Attributes["class"] = "nav-link active";
			}
			else if (pageName.Equals("sales.aspx", StringComparison.OrdinalIgnoreCase) || pageName.Equals("createsales.aspx", StringComparison.OrdinalIgnoreCase) || pageName.Equals("viewsales.aspx", StringComparison.OrdinalIgnoreCase) || pageName.Equals("updatesales.aspx", StringComparison.OrdinalIgnoreCase))
			{
				penjualanNavLink.Attributes["class"] = "nav-link active";
			}
			else if (pageName.Equals("purchase.aspx", StringComparison.OrdinalIgnoreCase) || pageName.Equals("createpurchase.aspx", StringComparison.OrdinalIgnoreCase) || pageName.Equals("viewpurchase.aspx", StringComparison.OrdinalIgnoreCase) || pageName.Equals("updatepurchase.aspx", StringComparison.OrdinalIgnoreCase))
			{
				pembelianNavLink.Attributes["class"] = "nav-link active";
			}
			else if (pageName.Equals("salesreport.aspx", StringComparison.OrdinalIgnoreCase) || pageName.Equals("salesreportresult.aspx", StringComparison.OrdinalIgnoreCase))
			{
				laporanpenjualanNavLink.Attributes["class"] = "nav-link active";
			}
			else if (pageName.Equals("purchasereport.aspx", StringComparison.OrdinalIgnoreCase) || pageName.Equals("purchasereportresult.aspx", StringComparison.OrdinalIgnoreCase))
			{
				laporanpembelianNavLink.Attributes["class"] = "nav-link active";
			}
			else
			{
				spesifikasiNavLink.Attributes["class"] = "nav-link active";
				spesifikasiNavItem.Attributes["class"] += " menu-open";

				if (pageName.Equals("category.aspx") || pageName.Equals("createcategory.aspx") || pageName.Equals("updatecategory.aspx"))
				{
					kategoriNavLink.Attributes["class"] = "nav-link active-spesifikasi";
				}
				else if (pageName.Equals("subcategory.aspx") || pageName.Equals("createsubcategory.aspx") || pageName.Equals("updatesubcategory.aspx"))
				{
					subkategoriNavLink.Attributes["class"] = "nav-link active-spesifikasi";
				}
				else if (pageName.Equals("productbrand.aspx") || pageName.Equals("createproductbrand.aspx") || pageName.Equals("updateproductbrand.aspx"))
				{
					merekprodukNavLink.Attributes["class"] = "nav-link active-spesifikasi";
				}
				else if (pageName.Equals("productmodel.aspx") || pageName.Equals("createproductmodel.aspx") || pageName.Equals("updateproductmodel.aspx"))
				{
					modelprodukNavLink.Attributes["class"] = "nav-link active-spesifikasi";
				}
				else if (pageName.Equals("phonebrand.aspx") || pageName.Equals("createphonebrand.aspx") || pageName.Equals("updatephonebrand.aspx"))
				{
					merekponselNavLink.Attributes["class"] = "nav-link active-spesifikasi";
				}
				else if (pageName.Equals("phonetype.aspx") || pageName.Equals("createphonetype.aspx") || pageName.Equals("updatephonetype.aspx"))
				{
					tipeponselNavLink.Attributes["class"] = "nav-link active-spesifikasi";
				}
				else if (pageName.Equals("productcolor.aspx") || pageName.Equals("createproductcolor.aspx") || pageName.Equals("updateproductcolor.aspx"))
				{
					warnaprodukNavLink.Attributes["class"] = "nav-link active-spesifikasi";
				}
			}
		}

		protected void btnLogout_Click(object sender, EventArgs e)
		{
			Session.Clear();
			Response.Redirect("~/LoginPage.aspx");
		}
	}
}