using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Report
{
	public partial class SalesReportResult : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				string subcategory = Session["Subcategory"] as string;
				string productBrand = Session["ProductBrand"] as string;
				string salesMedia = Session["SalesMedia"] as string;
				string user = Session["User"] as string;
				string startDate = Session["StartDate"].ToString();
				string endDate = Session["EndDate"].ToString();
				string startDateFormatted = Convert.ToDateTime(Session["StartDate"]).ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
				string endDateFormatted = Convert.ToDateTime(Session["EndDate"]).ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));

				ClientScript.RegisterStartupScript(this.GetType(), "SetDateRange", $@"
					var startDate = '{startDateFormatted}';
					var endDate = '{endDateFormatted}';
				", true);

				LoadFilteredReport(subcategory, productBrand, salesMedia, user, startDate, endDate);
			}
		}

		private void LoadFilteredReport(string subcategory, string productBrand, string salesMedia, string user, string startDate, string endDate)
		{
			string query = @"
							SELECT ST.salesId AS [KodeTransaksi],
								   ST.salesDate AS [Tanggal],
								   ST.createdTime AS [Waktu],
								   CONCAT(SC.subCategoryName, ' ', PB.productBrandName, ' ', PM.productModelName, ' ', PHB.phoneBrandName, ' ', PT.phoneTypeName, ' ', PC.colorName) AS [NamaProduk],
								   STD.quantity AS [Kuantitas],
								   STD.subtotal AS [TotalHarga],
								   M.mediaName AS [Media],
								   U.fullName AS [Pencatat]
							FROM SalesTransaction ST
							JOIN Media M ON ST.mediaId = M.mediaId
							JOIN Users U ON ST.userId = U.userId
							JOIN SalesTransactionDetail STD ON ST.salesId = STD.salesId
							JOIN Product P ON STD.productId = P.productId
							JOIN SubCategory SC ON P.subCategoryId = SC.subCategoryId
							JOIN ProductModel PM ON P.productModelId = PM.productModelId
							JOIN ProductBrand PB ON PM.productBrandId = PB.productBrandId
							JOIN PhoneType PT ON P.phoneTypeId = PT.phoneTypeId
							JOIN PhoneBrand PHB ON PT.phoneBrandId = PHB.phoneBrandId
							JOIN ProductColor PC ON P.colorId = PC.colorId
							WHERE 1=1";

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

			if (!string.IsNullOrEmpty(salesMedia) && salesMedia != "All")
			{
				query += " AND M.mediaName = @SalesMedia";
				parameters.Add(new SqlParameter("@SalesMedia", salesMedia));
			}

			if (!string.IsNullOrEmpty(user) && user != "All")
			{
				query += " AND U.fullName = @User";
				parameters.Add(new SqlParameter("@User", user));
			}

			if (!string.IsNullOrEmpty(startDate))
			{
				query += " AND ST.createdTime >= @StartDate";
				parameters.Add(new SqlParameter("@StartDate", Convert.ToDateTime(startDate)));
			}

			if (!string.IsNullOrEmpty(endDate))
			{
				query += " AND ST.createdTime <= @EndDate";
				parameters.Add(new SqlParameter("@EndDate", Convert.ToDateTime(endDate)));
			}

			query += " ORDER BY ST.salesDate";

			DataTable dtSalesReport = dbCon.Fetch(query, parameters.ToArray());

			if (dtSalesReport.Rows.Count > 0)
			{
				rptSalesReport.DataSource = dtSalesReport;
				rptSalesReport.DataBind();
			}
			else
			{
				rptSalesReport.DataSource = null;
				rptSalesReport.DataBind();
			}
		}
	}
}