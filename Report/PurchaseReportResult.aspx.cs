using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Report
{
	public partial class PurchaseReportResult : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				string subcategory = Session["Subcategory"] as string;
				string productBrand = Session["ProductBrand"] as string;
				string phone = Session["Phone"] as string;
				string supplier = Session["Supplier"] as string;
				string startDate = Session["StartDate"].ToString();
				string endDate = Session["EndDate"].ToString();
				string startDateFormatted = Convert.ToDateTime(Session["StartDate"]).ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
				string endDateFormatted = Convert.ToDateTime(Session["EndDate"]).ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));

				ClientScript.RegisterStartupScript(this.GetType(), "SetDateRange", $@"
					var startDate = '{startDateFormatted}';
					var endDate = '{endDateFormatted}';
				", true);

				LoadFilteredReport(subcategory, productBrand, phone, supplier, startDate, endDate);
			}
		}

		private void LoadFilteredReport(string subcategory, string productBrand, string phone, string supplier, string startDate, string endDate)
		{
			string query = @"
						SELECT PT.purchaseId AS [KodeTransaksi],
							   PT.purchaseDate AS [Tanggal],
							   PT.createdTime AS [Waktu],
							   CONCAT(SC.subCategoryName, ' ', PB.productBrandName, ' ', PM.productModelName, ' ', PHB.phoneBrandName, ' ', PTY.phoneTypeName, ' ', PC.colorName) AS [NamaProduk],
							   PTD.quantity AS [Kuantitas],
							   PTD.subtotal AS [TotalHarga],
							   CONCAT(PHB.phoneBrandName, ' ', PTY.phoneTypeName) AS [Phone],
							   CASE 
								   WHEN S.supplierName = 'PT Digital Gemilang Selindo' THEN 'DIGISEL'
								   ELSE S.supplierName 
							   END AS [Supplier]
						FROM PurchaseTransaction PT
						JOIN Supplier S ON PT.supplierId = S.supplierId
						JOIN PurchaseTransactionDetail PTD ON PT.purchaseId = PTD.purchaseId
						JOIN Product P ON PTD.productId = P.productId
						JOIN SubCategory SC ON P.subCategoryId = SC.subCategoryId
						JOIN ProductModel PM ON P.productModelId = PM.productModelId
						JOIN ProductBrand PB ON PM.productBrandId = PB.productBrandId
						JOIN PhoneType PTY ON P.phoneTypeId = PTY.phoneTypeId
						JOIN PhoneBrand PHB ON PTY.phoneBrandId = PHB.phoneBrandId
						JOIN ProductColor PC ON P.colorId = PC.colorId
						WHERE S.isActive = 1";

			List<SqlParameter> parameters = new List<SqlParameter>();

			if (!string.IsNullOrEmpty(subcategory) && subcategory != "All")
			{
				query += " AND SC.subCategoryName = @Subcategory";
				parameters.Add(new SqlParameter("@Subcategory", subcategory));
			}

			if (!string.IsNullOrEmpty(productBrand) && productBrand != "All")
			{
				query += " AND PB.productBrandName = @ProductBrand";
				parameters.Add(new SqlParameter("@ProductBrand", productBrand));
			}

			if (!string.IsNullOrEmpty(phone) && phone != "All")
			{
				query += " AND CONCAT(PHB.phoneBrandName, ' ', PTY.phoneTypeName) = @Phone";
				parameters.Add(new SqlParameter("@Phone", phone));
			}

			if (!string.IsNullOrEmpty(supplier) && supplier != "All")
			{
				query += " AND S.supplierName = @Supplier";
				parameters.Add(new SqlParameter("@Supplier", supplier));
			}

			if (!string.IsNullOrEmpty(startDate))
			{
				query += " AND PT.createdTime >= @StartDate";
				parameters.Add(new SqlParameter("@StartDate", Convert.ToDateTime(startDate)));
			}

			if (!string.IsNullOrEmpty(endDate))
			{
				query += " AND PT.createdTime <= @EndDate";
				parameters.Add(new SqlParameter("@EndDate", Convert.ToDateTime(endDate)));
			}

			query += " ORDER BY PT.purchaseDate";

			DataTable dtPurchaseReport = dbCon.Fetch(query, parameters.ToArray());

			if (dtPurchaseReport.Rows.Count > 0)
			{
				rptPurchaseReport.DataSource = dtPurchaseReport;
				rptPurchaseReport.DataBind();
			}
			else
			{
				rptPurchaseReport.DataSource = null;
				rptPurchaseReport.DataBind();
			}
		}
	}
}