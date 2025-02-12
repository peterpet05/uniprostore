using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;
using System.Collections;
using System.IO;
using System.Configuration;

namespace Unipro_Store.Product
{
	public partial class Product : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				GetTableData();
				LoadSubCategory();
				LoadPhoneBrand();
				LoadProducts();
			}
		}

		private void LoadSubCategory()
		{
			string query = @"
							SELECT DISTINCT s.subcategoryName 
							FROM Subcategory s
							JOIN Product p ON s.subcategoryId = p.subcategoryId";

			DataTable dtSubCategories = dbCon.Fetch(query);

			filterSubcategory.Items.Clear();
			filterSubcategory.Items.Add(new ListItem("All", ""));

			foreach (DataRow row in dtSubCategories.Rows)
			{
				filterSubcategory.Items.Add(new ListItem(row["subcategoryName"].ToString(), row["subcategoryName"].ToString()));
			}
		}

		private void LoadPhoneBrand()
		{
			string query = @"
							SELECT DISTINCT pb.phoneBrandName 
							FROM PhoneBrand pb
							JOIN PhoneType pt ON pb.phoneBrandId = pt.phoneBrandId
							JOIN Product p ON pt.phoneTypeId = p.phoneTypeId";

			DataTable dtPhoneBrands = dbCon.Fetch(query);

			filterPhoneBrand.Items.Clear();
			filterPhoneBrand.Items.Add(new ListItem("All", ""));

			foreach (DataRow row in dtPhoneBrands.Rows)
			{
				filterPhoneBrand.Items.Add(new ListItem(row["phoneBrandName"].ToString(), row["phoneBrandName"].ToString()));
			}
		}

		private void LoadProducts()
		{
				string query = @"
						SELECT 
							p.ProductId, 
							CONCAT(sc.SubCategoryName, ' ', pb.ProductBrandName, ' ', pm.ProductModelName, ' ', 
										  phb.PhoneBrandName, ' ', pt.PhoneTypeName, ' ', pc.ColorName) AS NamaProduk
						FROM Product p
						JOIN SubCategory sc ON p.SubCategoryId = sc.SubCategoryId
						JOIN ProductModel pm ON p.ProductModelId = pm.ProductModelId
						JOIN ProductBrand pb ON pm.ProductBrandId = pb.ProductBrandId
						JOIN PhoneType pt ON p.PhoneTypeId = pt.PhoneTypeId
						JOIN PhoneBrand phb ON pt.PhoneBrandId = phb.PhoneBrandId
						JOIN ProductColor pc ON p.ColorID = pc.ColorID 
						ORDER BY sc.SubCategoryName";

				var commonFunction = new Unipro_Store.Models.Function.CommonFunction();
				DataTable dt = commonFunction.Fetch(query);

				// Periksa apakah data tersedia
				if (dt.Rows.Count > 0)
				{
					ddlProduct.DataSource = dt;
					ddlProduct.DataTextField = "NamaProduk";  
					ddlProduct.DataValueField = "ProductId";  
					ddlProduct.DataBind();

					ddlProduct.Items.Insert(0, new ListItem("Pilih Produk", ""));
				}
				else
				{
					// Jika tidak ada data
					ddlProduct.Items.Clear();
					ddlProduct.Items.Insert(0, new ListItem("Tidak ada data produk", ""));
				}
		}

		public void GetTableData()
		{
			DataTable dt = dbCon.Fetch(@"SELECT 
						ROW_NUMBER() OVER(ORDER BY CONCAT(sc.subCategoryName, ' ', pb.productBrandName, ' ', pm.productModelName, ' ', phb.phoneBrandName, ' ', pt.phoneTypeName, ' ', pc.colorName) ASC) AS [No], 
						p.productId AS [IDProduk], 
						CONCAT(sc.subCategoryName, ' ', pb.productBrandName, ' ', pm.productModelName, ' ', phb.phoneBrandName, ' ', pt.phoneTypeName, ' ', pc.colorName) AS [NamaProduk],
						phb.phoneBrandName AS [MerekPonsel],
						p.stock AS [Stok], 
						p.buyPrice AS [HargaBeli], 
						p.sellPrice AS [HargaJual], 
						p.image AS [Gambar]  
					FROM 
						Product p
						JOIN SubCategory sc ON p.subCategoryId = sc.subCategoryId
						JOIN ProductModel pm ON p.productModelId = pm.productModelId
						JOIN ProductBrand pb ON pm.productBrandId = pb.productBrandId
						JOIN PhoneType pt ON p.phoneTypeId = pt.phoneTypeId
						JOIN PhoneBrand phb ON pt.phoneBrandId = phb.phoneBrandId
						JOIN ProductColor pc ON p.colorID = pc.colorID
					ORDER BY NamaProduk ASC;");

			StringBuilder sb = new StringBuilder();
			int lowStockCount = 0;
			int outOfStockCount = 0;

			foreach (DataRow row in dt.Rows)
			{
				int stock = Convert.ToInt32(row["Stok"]);

				sb.Append("<tr>");
				sb.Append("<td>" + row["No"] + "</td>");
				sb.Append("<td>" + row["IDProduk"] + "</td>");
				sb.Append("<td>" + row["NamaProduk"] + "</td>");
				sb.Append("<td class='hidden-column'>" + row["MerekPonsel"] + "</td>");
				sb.Append("<td>" + stock + "</td>");
				sb.Append("<td>Rp " + string.Format("{0:N0}", row["HargaBeli"]) + "</td>");
				sb.Append("<td>Rp " + string.Format("{0:N0}", row["HargaJual"]) + "</td>");

				if (row["Gambar"] != DBNull.Value)
				{
					byte[] imageBytes = (byte[])row["Gambar"];
					string imgBase64 = Convert.ToBase64String(imageBytes);
					string imgSrc = $"data:image/png;base64,{imgBase64}";
					sb.Append("<td><img src='" + imgSrc + "' style='height:50px;'/></td>");
				}
				else
				{
					sb.Append("<td>No image</td>");
				}

				sb.Append("<td>");
				sb.Append("<a href='CreateVariation.aspx?ProductID=" + row["IDProduk"] + "' class='btn btn-sm' style='background-color: #579c48; color: white;'><i class='fas fa-plus'></i></a> ");
				sb.Append("<a href='UpdateProduct.aspx?ProductID=" + row["IDProduk"] + "' class='btn btn-sm' style='background-color: #386bba; color: white;'><i class='fas fa-edit'></i></a> ");
				sb.Append("<a href='javascript:void(0);' onclick='showDeleteModal(\"" + row["IDProduk"] + "\");' class='btn btn-sm' style='background-color: #ff4c4c; color: white;'><i class='fas fa-trash-alt'></i></a>");
				sb.Append("</td>");
				sb.Append("</tr>");

				if (stock == 0)
				{
					outOfStockCount++;
				}
				else if (stock <= 3)
				{
					lowStockCount++;
				}
			}

			ltTableRows.Text = sb.ToString();

			StringBuilder notificationSb = new StringBuilder();
			if (outOfStockCount > 0)
			{
				notificationSb.Append($"<div class='alert alert-danger alert-dismissible fade show' role='alert' " +
						  $"style='background-color: #f8d7da; color: #721c24; border-color: #f5c6cb;'>" +
						  $"<i class='fas fa-exclamation-triangle' style='margin-right: 10px;'></i>" +
						  $"Terdapat {outOfStockCount} produk yang stoknya habis  -  " +
						  $"<strong><a href='#' id='viewOutOfStock' style='text-decoration: underline; color: #721c24; text-decoration: none;'>Lihat produk</a></strong>" +
						  $"<button type='button' class='close' data-dismiss='alert' aria-label='Close'>" +
						  $"<span aria-hidden='true'>&times;</span>" +
						  $"</button>" +
						  $"</div>");
			}

			if (lowStockCount > 0)
			{
				notificationSb.Append($"<div class='alert alert-warning alert-dismissible fade show' role='alert' " +
					  $"style='background-color: #fff3cd; color: #856404; border-color: #ffeeba;'>" +
					  $"<i class='fas fa-exclamation-circle' style='margin-right: 10px;'></i>" + 
					  $"Terdapat {lowStockCount} produk dengan stok rendah - " +
					  $"<strong><a href='#' id='viewLowStock' style='color: #856404; text-decoration: none;'>Lihat produk</a></strong>" +
					  $"<button type='button' class='close' data-dismiss='alert' aria-label='Close'>" +
					  $"<span aria-hidden='true'>&times;</span>" +
					  $"</button>" +
					  $"</div>");

			}

			ltStockNotification.Text = notificationSb.ToString();
		}

		protected void create_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Product/CreateProduct.aspx");
		}

		private string GetCodeShortForm(string name)
		{
			return string.Concat(name.Split(' ').Select(word => word[0]));
		}

		private string GetPhoneTypeShortForm(string name)
		{
			var words = name.Split(' ');
			var prefix = string.Empty;
			var suffix = string.Empty;
			string digits = string.Empty;

			foreach (var word in words)
			{
				if (word.Any(char.IsDigit))
				{
					if (word.All(char.IsDigit))
					{
						digits = word;
					}
					else
					{
						digits = new string(word.Where(char.IsDigit).ToArray());
						prefix += char.ToUpper(word[0]);
					}
				}
				else if (string.IsNullOrEmpty(digits))
				{
					prefix += char.ToUpper(word[0]);
				}
				else
				{
					suffix += char.ToUpper(word[0]);
				}
			}
			return prefix + digits + suffix;
		}

		public static string GetColorShortForm(string colorName)
		{
			colorName = colorName.Trim().ToUpper();

			if (colorName.Length < 3)
			{
				return colorName;
			}
			return colorName.Substring(0, 3);
		}

		private int GetNextProductNumber()
		{
			int lastNumber = 0;
			string query = "SELECT MAX(CAST(RIGHT(productId, 4) AS INT)) AS LastNumber FROM Product WHERE ISNUMERIC(RIGHT(productId, 4)) = 1";

			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(query);

			if (dt.Rows.Count > 0 && dt.Rows[0]["LastNumber"] != DBNull.Value)
			{
				lastNumber = Convert.ToInt32(dt.Rows[0]["LastNumber"]);
			}
			return lastNumber + 1;
		}

		private string GenerateUniqueProductCode(string subCategoryName, string productBrandName, string productModelName, string phoneBrandName, string phoneTypeName, string colorName, int nextNumber)
		{
			string shortSubCategoryName = GetCodeShortForm(subCategoryName);
			string shortProductBrandName = GetCodeShortForm(productBrandName);
			string shortProductModelName = GetCodeShortForm(productModelName);
			string shortPhoneBrandName = GetCodeShortForm(phoneBrandName);
			string shortPhoneTypeName = GetPhoneTypeShortForm(phoneTypeName);
			string shortProductColorName = GetColorShortForm(colorName);

			string shortProductCode = $"{shortSubCategoryName}-{shortProductBrandName}-{shortProductModelName}-{shortPhoneBrandName}{shortPhoneTypeName}-{shortProductColorName}-{nextNumber:D4}";
			return shortProductCode;
		}

		private int GetIdFromDatabase(string tableName, string idColumn, string nameColumn, string value)
		{
			string query = $"SELECT {idColumn} FROM {tableName} WHERE {nameColumn} = @Value";
			CommonFunction dbCon = new CommonFunction();
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@Value", value)
			};
			DataTable dt = dbCon.Fetch(query, parameters);
			return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0][idColumn]) : -1;
		}

		private byte[] ReadImageFromFolder(string imageName)
		{
			string filePath = Server.MapPath("~/Content/Images/" + imageName);
			if (File.Exists(filePath))
			{
				return File.ReadAllBytes(filePath);
			}
			return null;
		}

		protected void importCsv_Click(object sender, EventArgs e)
		{
			try
			{
				if (!inputCsv.HasFile)
				{
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorToast",
						"launchErrorToast('Silahkan unggah file CSV!');", true);
					return;
				}

				string fileExtension = Path.GetExtension(inputCsv.FileName);
				if (fileExtension.ToLower() != ".csv")
				{
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorToast",
						"launchErrorToast('Hanya file dengan format .csv yang didukung!');", true);
					return;
				}

				List<string[]> csvData = new List<string[]>();
				using (var reader = new StreamReader(inputCsv.PostedFile.InputStream))
				{
					while (!reader.EndOfStream)
					{
						var line = reader.ReadLine();
						csvData.Add(line.Split(',')); // Pisahkan data berdasarkan koma
					}
				}

				// Proses setiap baris, lewati header
				for (int i = 1; i < csvData.Count; i++)
				{
					var row = csvData[i];
					if (row.Length < 11) continue;

					string subCategoryName = row[0].Trim();
					string productBrandName = row[1].Trim();
					string productModelName = row[2].Trim();
					string phoneBrandName = row[3].Trim();
					string phoneTypeName = row[4].Trim();
					string colorName = row[5].Trim();
					int stock = int.Parse(row[6].Trim());
					decimal buyPrice = decimal.Parse(row[7].Trim());
					decimal sellPrice = decimal.Parse(row[8].Trim());
					string description = row.Length > 9 ? row[9].Trim() : "";
					string imageName = row[10].Trim();

					// Dapatkan ID dari database
					int subCategoryId = GetIdFromDatabase("SubCategory", "subCategoryId", "subCategoryName", subCategoryName);
					int productModelId = GetIdFromDatabase("ProductModel", "productModelId", "productModelName", productModelName);
					int phoneTypeId = GetIdFromDatabase("PhoneType", "phoneTypeId", "phoneTypeName", phoneTypeName);
					int colorId = GetIdFromDatabase("ProductColor", "colorId", "colorName", colorName);

					if (subCategoryId == -1 || productModelId == -1 || phoneTypeId == -1 || colorId == -1)
						continue; // Lewati jika data tidak valid

					int nextNumber = GetNextProductNumber();
					string productId = GenerateUniqueProductCode(
						subCategoryName, productBrandName, productModelName,
						phoneBrandName, phoneTypeName, colorName, nextNumber);

					byte[] imageBytes = ReadImageFromFolder(imageName);

					// Simpan data ke database
					string query = @"
								IF NOT EXISTS (
									SELECT 1 FROM Product 
									WHERE subCategoryId = @SubCategoryId 
									  AND productModelId = @ProductModelId 
									  AND phoneTypeId = @PhoneTypeId 
									  AND colorId = @ColorId
								)
								BEGIN
									INSERT INTO Product (
										productId, subCategoryId, productModelId, phoneTypeId, colorId, 
										stock, buyPrice, sellPrice, description, image, createdTime, updatedTime
									)
									VALUES (
										@ProductId, @SubCategoryId, @ProductModelId, @PhoneTypeId, @ColorId, 
										@Stock, @BuyPrice, @SellPrice, @Description, @Image, GETDATE(), GETDATE()
									)
								END";

					SqlParameter[] parameters = new SqlParameter[]
					{
						new SqlParameter("@ProductId", productId),
						new SqlParameter("@SubCategoryId", subCategoryId),
						new SqlParameter("@ProductModelId", productModelId),
						new SqlParameter("@PhoneTypeId", phoneTypeId),
						new SqlParameter("@ColorId", colorId),
						new SqlParameter("@Stock", stock),
						new SqlParameter("@BuyPrice", buyPrice),
						new SqlParameter("@SellPrice", sellPrice),
						new SqlParameter("@Description", description),
						new SqlParameter("@Image", imageBytes ?? (object)DBNull.Value)
					};

					CommonFunction dbCon = new CommonFunction();
					dbCon.Query(query, parameters);
				}

				ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccessToast", @"
					launchSuccessToast('Import Data CSV Berhasil');
					setTimeout(function() {
						window.location.href = window.location.href; // Refresh halaman
					}, 1500);", true);
				}
			catch (Exception ex)
			{
				
			}
		}

		protected void btnCalculate_Click(object sender, EventArgs e)
		{
			try
			{
				// Ambil input dari DropDownList dan input tanggal
				string productId = ddlProduct.SelectedValue;
				string startDateStr = inputStartDate.Attributes["value"];
				string endDateStr = inputEndDate.Attributes["value"];

				if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
				{
					lblMessage.Text = "<i class='fas fa-exclamation-circle'></i>  Silahkan pilih produk terlebih dahulu!";
					lblMessage.ForeColor = System.Drawing.Color.Red;
					return;
				}

				// Konversi tanggal
				DateTime startDate = DateTime.Parse(startDateStr);
				DateTime endDate = DateTime.Parse(endDateStr);

				if (startDate > endDate)
				{
					lblMessage.Text = "<i class='fas fa-exclamation-circle'></i>  Tanggal mulai tidak boleh lebih besar dari tanggal berakhir!";
					lblMessage.ForeColor = System.Drawing.Color.Red;
					return;
				}

				int maxLeadTime = 3;
				int avgLeadTime = 2;
				int totalDays = (endDate - startDate).Days + 1;

				string stockQuery = "SELECT stock FROM Product WHERE productId = @ProductId";
				string query = @"
								SELECT 
							AVG(DailySales) AS AvgDailySales,
							MAX(DailySales) AS MaxDailySales,
							COUNT(DISTINCT CAST(SaleDate AS DATE)) AS TotalDays,
							SUM(TotalQuantity) AS TotalStock
							FROM (
								SELECT 
									CAST(st.salesDate AS DATE) AS SaleDate,
									SUM(std.Quantity) AS DailySales,
									SUM(std.Quantity) AS TotalQuantity
								FROM SalesTransactionDetail std
								JOIN SalesTransaction st ON std.salesId = st.salesId
								WHERE std.ProductId = @ProductId AND st.salesDate BETWEEN @StartDate AND @EndDate
								GROUP BY CAST(st.salesDate AS DATE)
							) AS SalesPerDay;
							";

				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["UniproStoreCS"].ConnectionString))
				{
					int currentStock = 0;
					using (SqlCommand cmd = new SqlCommand(stockQuery, con))
					{
						cmd.Parameters.AddWithValue("@ProductId", productId);
						con.Open();
						object stockResult = cmd.ExecuteScalar();
						currentStock = stockResult != null ? Convert.ToInt32(stockResult) : 0;
						con.Close();
					}

					using (SqlCommand cmd = new SqlCommand(query, con))
					{
						cmd.Parameters.AddWithValue("@ProductId", productId);
						cmd.Parameters.AddWithValue("@StartDate", startDate);
						cmd.Parameters.AddWithValue("@EndDate", endDate);

						con.Open();
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								// Ambil hasil dari query
								double avgDailySales = reader["AvgDailySales"] != DBNull.Value ? Convert.ToDouble(reader["AvgDailySales"]) : 0;
								double maxDailySales = reader["MaxDailySales"] != DBNull.Value ? Convert.ToDouble(reader["MaxDailySales"]) : 0;

								// Hitung Safety Stock dan Reorder Point
								double safetyStock = (maxDailySales * maxLeadTime) - (avgDailySales * avgLeadTime);
								double reorderPoint = (avgDailySales * avgLeadTime) + safetyStock;

								txtCurrentStock.Text = currentStock.ToString();
								txtCountedDays.Text = totalDays.ToString();
								txtMaximalSold.Text = maxDailySales.ToString();
								txtAverageSold.Text = avgDailySales.ToString();
								txtMaximalLeadTime.Text = maxLeadTime.ToString();
								txtAverageLeadTime.Text = avgLeadTime.ToString();
								txtSafetyStock.Text = safetyStock.ToString();
								txtReorderPoint.Text = reorderPoint.ToString();

								resultSection.Style["display"] = "block";
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		protected void delete_Click(object sender, EventArgs e)
		{
			string productId = hfProductIDToDelete.Value;

			// Query untuk mengecek apakah ada SalesTransaction atau PurchaseTransaction terkait dengan produk
			string checkTransactionQuery = @"
				SELECT 1 FROM SalesTransactionDetail WHERE ProductID = @ProductID
				UNION ALL
				SELECT 1 FROM PurchaseTransactionDetail WHERE ProductID = @ProductID";

			SqlParameter[] parametersForCheck = new SqlParameter[]
			{
				new SqlParameter("@ProductID", productId)
			};

			try
			{
				DataTable dt = dbCon.Fetch(checkTransactionQuery, parametersForCheck);

				if (dt.Rows.Count > 0)
				{
					// Menampilkan warning toast jika ada transaksi penjualan atau pembelian terkait
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showWarningToast",
						"launchWarningToast('Produk tidak dapat dihapus karena terkait dengan transaksi!');", true);
				}
				else
				{
					string deleteProductQuery = "DELETE FROM Product WHERE ProductID = @ProductID";
					SqlParameter[] parametersForDelete = new SqlParameter[]
					{
						new SqlParameter("@ProductID", productId)
					};

					dbCon.Query(deleteProductQuery, parametersForDelete);

					GetTableData();

					ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccessToast", "launchSuccessToast('Produk berhasil dihapus');", true);
					ScriptManager.RegisterStartupScript(this, this.GetType(), "hideModal", "$('#deleteModal').modal('hide');", true);
				}
			}
			catch (Exception ex)
			{
				Response.Write("Error: " + ex.Message);
			}
		}
	}
}