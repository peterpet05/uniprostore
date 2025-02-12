using System;
using System.Data;
using Unipro_Store.Models;
using System.Web.UI;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;

namespace Unipro_Store
{
	public partial class WebForm2 : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (IsPostBack && Request["__EVENTTARGET"] == "MonthPicker")
			{
				string selectedMonth = Request["__EVENTARGUMENT"];
				if (!string.IsNullOrEmpty(selectedMonth))
				{
					ViewState["SelectedMonth"] = selectedMonth;
					lastSelectedMonth.Value = selectedMonth;
					BindSalesMediaData(selectedMonth);
				}
			}
			else if (IsPostBack && Request["__EVENTTARGET"] == "QuarterPicker")
			{
				string[] args = Request["__EVENTARGUMENT"].Split(',');
				string selectedQuarter = args[0];
				int selectedYear = int.Parse(args[1]);
				ViewState["SelectedQuarter"] = selectedQuarter;
				ViewState["SelectedYear"] = selectedYear;

				LoadTopProductsByQuarterData(selectedQuarter, selectedYear);
			}
			else if (IsPostBack && Request["__EVENTTARGET"] == "MonthPicker2")
			{
				string selectedMonth2 = Request["__EVENTARGUMENT"];
				if (!string.IsNullOrEmpty(selectedMonth2))
				{
					ViewState["SelectedMonth2"] = selectedMonth2;
					LoadOmsetData(selectedMonth2); 
				}
			}
			else if (!IsPostBack)
			{
				string currentQuarter = GetQuarterFromMonth(DateTime.Now.Month);
				int currentYear = DateTime.Now.Year;
				ViewState["SelectedQuarter"] = currentQuarter;
				ViewState["SelectedYear"] = currentYear;

				LoadTopProductsByQuarterData(currentQuarter, currentYear); 
				LoadTotals(); 

				string currentMonth = new DateTime(DateTime.Now.Year, 10, 1).ToString("yyyy-MM");
				// string currentMonth = DateTime.Now.ToString("yyyy-MM");
				ViewState["SelectedMonth"] = currentMonth;
				lastSelectedMonth.Value = ViewState["SelectedMonth"].ToString();
				BindSalesMediaData(currentMonth); 
				ViewState["SelectedMonth2"] = currentMonth;
				LoadOmsetData(currentMonth); 

			}

			if (ViewState["SelectedMonth"] != null)
			{
				string selectedMonth = ViewState["SelectedMonth"].ToString();
				ClientScript.RegisterStartupScript(this.GetType(), "setMonthPicker",
					$"document.getElementById('monthPicker').value = '{selectedMonth}';", true);
			}

			if (ViewState["SelectedMonth2"] != null)
			{
				string selectedMonth2 = ViewState["SelectedMonth2"].ToString();
				ClientScript.RegisterStartupScript(this.GetType(), "setMonthPicker2",
					$"document.getElementById('monthPicker2').value = '{selectedMonth2}';", true);
			}

			if (ViewState["SelectedQuarter"] != null && ViewState["SelectedYear"] != null)
			{
				string selectedQuarter = ViewState["SelectedQuarter"].ToString();
				int selectedYear = (int)ViewState["SelectedYear"];
				ClientScript.RegisterStartupScript(this.GetType(), "setQuarterPicker",
					$"document.getElementById('quarterPicker').value = '{selectedQuarter}';" +
					$"document.getElementById('yearPicker').value = {selectedYear};", true);
			}

			if (IsPostBack)
			{
				string selectedQuarter = ViewState["SelectedQuarter"].ToString();
				int selectedYear = (int)ViewState["SelectedYear"];
				LoadTopProductsByQuarterData(selectedQuarter, selectedYear);


				if (ViewState["SelectedMonth2"] != null)
				{
					string selectedMonth2 = ViewState["SelectedMonth2"].ToString();
					LoadOmsetData(selectedMonth2); 
				}
			}
		}

		private void LoadTotals()
		{
			// int currentMonth = DateTime.Now.Month;
			// int currentYear = DateTime.Now.Year;
			int currentMonth = 10;
			int currentYear = 2024;

			// Query untuk menghitung total dari tabel Product, Supplier, SalesTransaction, dan PurchaseTransaction
			string query = $@"
						SELECT 
						(SELECT COUNT(*) FROM Product) AS TotalProducts,
						(SELECT COUNT(*) FROM Supplier WHERE isActive = 1) AS TotalSuppliers,
						(SELECT COUNT(*) FROM SalesTransaction WHERE MONTH(SalesDate) = {currentMonth} AND YEAR(SalesDate) = {currentYear}) AS TotalSales,
						(SELECT COUNT(*) FROM PurchaseTransaction WHERE MONTH(PurchaseDate) = {currentMonth} AND YEAR(PurchaseDate) = {currentYear}) AS TotalPurchases";

			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(query);

			if (dt.Rows.Count > 0)
			{
				int totalProducts = dt.Rows[0]["TotalProducts"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["TotalProducts"]) : 0;
				int totalSuppliers = dt.Rows[0]["TotalSuppliers"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["TotalSuppliers"]) : 0;
				int totalSales = dt.Rows[0]["TotalSales"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["TotalSales"]) : 0;
				int totalPurchases = dt.Rows[0]["TotalPurchases"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["TotalPurchases"]) : 0;

				lblTotalProducts.Text = totalProducts.ToString();
				lblTotalSuppliers.Text = totalSuppliers.ToString();
				lblTotalSales.Text = totalSales.ToString();
				lblTotalPurchases.Text = totalPurchases.ToString();
			}
		}

		private string GetQuarterFromMonth(int month)
		{
			if (month >= 1 && month <= 3) return "Q1";
			else if (month >= 4 && month <= 6) return "Q2";
			else if (month >= 7 && month <= 9) return "Q3";
			else return "Q4";
		}

		private void LoadTopProductsByQuarterData(string quarter, int year)
		{
			string startMonth = "01", endMonth = "03"; 
			switch (quarter)
			{
				case "Q2": startMonth = "04"; endMonth = "06"; break;
				case "Q3": startMonth = "07"; endMonth = "09"; break;
				case "Q4": startMonth = "10"; endMonth = "12"; break;
			}

			string startDate = new DateTime(year, int.Parse(startMonth), 1).ToString("yyyy-MM-dd");
			string endDate = new DateTime(year, int.Parse(endMonth), DateTime.DaysInMonth(year, int.Parse(endMonth))).ToString("yyyy-MM-dd");

			// Query untuk produk terlaris berdasarkan range bulan dan tahun
			string query = @"
							SELECT TOP 5
								CONCAT(sc.subcategoryName, ' ', pb.productBrandName, ' ', pm.productModelName) AS NamaProduk, 
								SUM(std.quantity) AS TotalTerjual
							FROM SalesTransactionDetail std
							JOIN SalesTransaction st ON std.salesId = st.salesId
							JOIN Product p ON std.productId = p.productId
							JOIN Subcategory sc ON p.subcategoryId = sc.subcategoryId
							JOIN ProductModel pm ON p.productModelId = pm.productModelId
							JOIN ProductBrand pb ON pm.productBrandId = pb.productBrandId
							WHERE st.salesDate BETWEEN @startDate AND @endDate
							GROUP BY sc.subcategoryName, pb.productBrandName, pm.productModelName
							ORDER BY TotalTerjual DESC";

			SqlParameter[] parameters = {
				new SqlParameter("@startDate", startDate),
				new SqlParameter("@endDate", endDate)
			};

			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(query, parameters);

			dt.Columns.Add("Color", typeof(string));

			if (dt.Rows.Count == 0)
			{
				// Tampilkan pesan "Tidak ada transaksi" jika tidak ada data
				rptTopProducts.Visible = false; 
				lblNoData.Visible = true; 
			}
			else
			{
				lblNoData.Visible = false;
				rptTopProducts.Visible = true;

				List<string> productNames = new List<string>();
				List<int> totalSales = new List<int>();
				List<string> backgroundColors = new List<string> { "#062e5f", "#1161a0", "#0252a8", "#49afcc", "#338bc4" };

				for (int i = 0; i < dt.Rows.Count; i++)
				{
					dt.Rows[i]["Color"] = backgroundColors[i]; 
					productNames.Add("\"" + dt.Rows[i]["NamaProduk"].ToString() + "\"");
					totalSales.Add(Convert.ToInt32(dt.Rows[i]["TotalTerjual"]));
				}

				string productNamesJson = "[" + string.Join(",", productNames) + "]";
				string totalSalesJson = "[" + string.Join(",", totalSales) + "]";
				string backgroundColorsJson = "[" + string.Join(",", backgroundColors.Select(c => "\"" + c + "\"")) + "]";

				ClientScript.RegisterStartupScript(this.GetType(), "topProductsChartData",
					$"var productNames = {productNamesJson}; var totalSales = {totalSalesJson}; var backgroundColors = {backgroundColorsJson};", true);

				rptTopProducts.DataSource = dt;
				rptTopProducts.DataBind();
			}
		}

		private void BindSalesMediaData(string selectedMonth)
		{
			// Ubah query untuk filter berdasarkan bulan yang dipilih
			string query = @"
							SELECT m.mediaName AS salesMedia, 
								   COUNT(s.salesId) AS totalTransactions, 
								   SUM(s.totalPrice) AS totalSales
							FROM SalesTransaction s
							JOIN Media m ON s.mediaId = m.mediaId
							WHERE FORMAT(s.salesDate, 'yyyy-MM') = @selectedMonth
							GROUP BY m.mediaName
							ORDER BY totalTransactions DESC";

			SqlParameter[] parameters = {
				new SqlParameter("@selectedMonth", selectedMonth)
			};

			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(query, parameters);

			// Jika tidak ada transaksi, tambahkan semua media penjualan dengan nilai 0 transaksi
			if (dt.Rows.Count == 0)
			{
				dt = new DataTable();
				dt.Columns.Add("salesMedia", typeof(string));
				dt.Columns.Add("totalTransactions", typeof(int));
				dt.Columns.Add("totalSales", typeof(double));
				dt.Columns.Add("percentage", typeof(double));
				dt.Columns.Add("trend", typeof(string));

				string[] mediaPenjualan = { "Tokopedia", "Shopee", "Toko Offline", "WhatsApp" };

				foreach (string media in mediaPenjualan)
				{
					DataRow row = dt.NewRow();
					row["salesMedia"] = media;
					row["totalTransactions"] = 0;
					row["totalSales"] = 0.0;
					row["percentage"] = 0.0;
					row["trend"] = "-";
					dt.Rows.Add(row);
				}
			}
			else
			{
				dt.Columns.Add("percentage", typeof(double));
				dt.Columns.Add("trend", typeof(string));

				foreach (DataRow row in dt.Rows)
				{
					int totalTransactions = Convert.ToInt32(row["totalTransactions"]);

					// Hitung persentase kontribusi transaksi
					double percentage = CalculatePercentage(totalTransactions, selectedMonth);
					row["percentage"] = percentage;

					// Hitung tren kenaikan/penurunan dibandingkan bulan sebelumnya
					string trend = CalculateTrend(row["salesMedia"].ToString(), totalTransactions, selectedMonth);
					row["trend"] = trend;
				}
				string[] mediaPenjualan = { "Tokopedia", "Shopee", "Toko Offline", "WhatsApp" };
				foreach (string media in mediaPenjualan)
				{
					bool exists = dt.AsEnumerable().Any(row => row.Field<string>("salesMedia") == media);
					if (!exists)
					{
						DataRow row = dt.NewRow();
						row["salesMedia"] = media;
						row["totalTransactions"] = 0;
						row["totalSales"] = 0.0;
						row["percentage"] = 0.0;
						row["trend"] = "-";
						dt.Rows.Add(row);
					}
				}
			}

			rptSalesMedia.DataSource = dt;
			rptSalesMedia.DataBind();
		}

		private string CalculateTrend(string salesMedia, int currentTransactions, string selectedMonth)
		{
			DateTime selectedDate = DateTime.ParseExact(selectedMonth + "-01", "yyyy-MM-dd", null);
			string previousMonth = selectedDate.AddMonths(-1).ToString("yyyy-MM");

			// Query untuk mendapatkan banyaknya transaksi di bulan yang sebelum yang dipilih
			string queryPreviousMonth = @"
									SELECT COUNT(s.salesId) AS totalPreviousTransactions
									FROM SalesTransaction s
									JOIN Media m ON s.mediaId = m.mediaId
									WHERE m.mediaName = @salesMedia AND FORMAT(s.salesDate, 'yyyy-MM') = @previousMonth";

			SqlParameter[] parameters = {
				new SqlParameter("@salesMedia", salesMedia),
				new SqlParameter("@previousMonth", previousMonth)
			};

			CommonFunction dbCon = new CommonFunction();
			DataTable dtPrevious = dbCon.Fetch(queryPreviousMonth, parameters);

			int previousTransactions = dtPrevious.Rows.Count > 0 ? Convert.ToInt32(dtPrevious.Rows[0]["totalPreviousTransactions"]) : 0;

			// Hitung tren berdasarkan perbandingan transaksi bulan ini dengan bulan sebelumnya
			if (previousTransactions > 0)
			{
				double trend = ((double)(currentTransactions - previousTransactions) / previousTransactions) * 100;
				return trend.ToString("0.0"); 
			}
			return "0.0"; // Jika tidak ada data sebelumnya, anggap tidak ada perubahan (0.0%)
		}

		private double CalculatePercentage(int totalTransactions, string selectedMonth)
		{
			// Query untuk mendapatkan banyaknya transaksi di bulan yang dipilih
			string queryTotal = @"
					SELECT COUNT(salesId) AS totalAllTransactions
					FROM SalesTransaction
					WHERE FORMAT(salesDate, 'yyyy-MM') = @selectedMonth";

			SqlParameter[] parameters = {
				new SqlParameter("@selectedMonth", selectedMonth)
			};

			CommonFunction dbCon = new CommonFunction();
			DataTable totalDt = dbCon.Fetch(queryTotal, parameters);
			int totalAllTransactions = totalDt.Rows.Count > 0 ? Convert.ToInt32(totalDt.Rows[0]["totalAllTransactions"]) : 0;

			if (totalAllTransactions > 0)
			{
				return (Convert.ToDouble(totalTransactions) / totalAllTransactions) * 100;
			}
			else
			{
				return 0;
			}
		}

		protected string FormatPercentage(object percentage)
		{
			double value;

			if (double.TryParse(percentage.ToString(), out value))
			{
				if (value == 0)
				{
					return "0"; 
				}

				if (value % 1 != 0)
				{
					return value.ToString("N2");
				}
				else
				{
					return value.ToString("N0");
				}
			}
			return "0"; 
		}

		protected string GetTrendClass(string trend)
		{
			double trendValue;

			if (double.TryParse(trend, out trendValue))
			{
				if (trendValue > 0)
					return "text-success";
				else if (trendValue < 0)
					return "text-danger";
			}

			return "text-muted";
		}

		protected string GetTrendIcon(string trend)
		{
			double trendValue;

			if (double.TryParse(trend, out trendValue))
			{
				if (trendValue > 0)
					return "fa-arrow-up";
				else if (trendValue < 0)
					return "fa-arrow-down";
			}

			return "fa-minus"; 
		}

		protected string GetIcon(string salesMedia)
		{
			switch (salesMedia)
			{
				case "Tokopedia":
					return "~/Admin LTE/dist/img/Tokopedia.png"; 
				case "Shopee":
					return "~/Admin LTE/dist/img/Shopee.png";
				case "Toko Offline":
					return "~/Admin LTE/dist/img/Store.png"; 
				case "WhatsApp":
					return "~/Admin LTE/dist/img/WhatsApp.png"; 
				default:
					return "~/images/default.png"; 
			}
		}

		protected string GetSalesTransactionUrl(string salesMedia, string selectedMonth)
		{
			string query = "SELECT mediaId FROM Media WHERE mediaName = @salesMedia";
			SqlParameter[] parameters = { new SqlParameter("@salesMedia", salesMedia) };
			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(query, parameters);
			string mediaId = dt.Rows.Count > 0 ? dt.Rows[0]["mediaId"].ToString() : "";

			string url = $"/Sales Transaction/Sales.aspx?media={mediaId}&month={selectedMonth}";
			return url;
		}

		private void LoadOmsetData(string selectedMonth)
		{
			string startDate = selectedMonth + "-01";
			string endDate = DateTime.ParseExact(selectedMonth, "yyyy-MM", null)
										 .AddMonths(1)
										 .AddDays(-1)
										 .ToString("yyyy-MM-dd");

			// Query untuk mendapatkan total omset per hari
			string query = @"
						SELECT 
							DAY(salesDate) AS SalesDay,
							SUM(discountedPrice) AS TotalOmset
						FROM SalesTransaction
						WHERE salesDate BETWEEN @startDate AND @endDate
						GROUP BY DAY(salesDate)
						ORDER BY SalesDay";

			SqlParameter[] parameters = {
				new SqlParameter("@startDate", startDate),
				new SqlParameter("@endDate", endDate)
			};

			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(query, parameters);

			// Hitung jumlah hari dalam bulan yang dipilih
			int daysInMonth = DateTime.ParseExact(selectedMonth, "yyyy-MM", null).AddMonths(1).AddDays(-1).Day;

			// Inisialisasi dengan nilai 0 untuk setiap hari
			List<string> days = Enumerable.Range(1, daysInMonth).Select(d => d.ToString()).ToList();
			List<decimal> totalOmset = Enumerable.Repeat(0m, daysInMonth).ToList();
			List<decimal> totalOmsetRaw = Enumerable.Repeat(0m, daysInMonth).ToList();

			foreach (DataRow row in dt.Rows)
			{
				int day = Convert.ToInt32(row["SalesDay"]) - 1;
				if (day >= 0 && day < totalOmset.Count) 
				{
					decimal rawValue = Convert.ToDecimal(row["TotalOmset"], CultureInfo.InvariantCulture);
					totalOmsetRaw[day] = rawValue; 
					totalOmset[day] = rawValue / 1000;

				}
			}

			string daysJson = "[" + string.Join(",", days) + "]";
			string totalOmsetJson = "[" + string.Join(",", totalOmset.Select(o => o.ToString("0.##", CultureInfo.InvariantCulture))) + "]";
			string totalOmsetRawJson = "[" + string.Join(",", totalOmsetRaw.Select(o => o.ToString("0.##", CultureInfo.InvariantCulture))) + "]";

			ClientScript.RegisterStartupScript(this.GetType(), "omsetChartData",
				$"var days = {daysJson}; var totalOmset = {totalOmsetJson}; var totalOmsetRaw = {totalOmsetRawJson};", true);
		}
	}
}