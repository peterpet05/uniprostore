using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Sales_Transaction
{
	public partial class CreateSales : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();

		[Serializable]
		public class Product
		{
			public string ProductId { get; set; }
			public string NamaProduk { get; set; }
			public decimal Harga { get; set; }
			public int Kuantitas { get; set; }
			public decimal Diskon { get; set; }
			public decimal Subtotal { get; set; }
			public string ImageBase64 { get; set; }
		}

		public class ProductSuggestion
		{
			public string ProductId { get; set; }
			public string NamaProduk { get; set; }
		}

		private List<Product> SelectedProducts
		{
			get
			{
				if (ViewState["SelectedProducts"] == null)
				{
					ViewState["SelectedProducts"] = new List<Product>();
				}
				return (List<Product>)ViewState["SelectedProducts"];
			}
			set
			{
				ViewState["SelectedProducts"] = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				ViewState["SelectedProducts"] = new List<Product>();
			}

			if (Request["__EVENTTARGET"] == "btnRemove")
			{
				string productIdToRemove = hiddenProductIdToRemove.Value;

				if (!string.IsNullOrEmpty(productIdToRemove))
				{
					RemoveProductFromViewState(productIdToRemove);
				}
			}
		}

		[System.Web.Services.WebMethod]
		public static List<ProductSuggestion> GetProductSuggestions(string term)
		{
			List<ProductSuggestion> suggestions = new List<ProductSuggestion>();
			Unipro_Store.Models.Function.CommonFunction dbCon = new Unipro_Store.Models.Function.CommonFunction();

			string query = @"
							SELECT p.productId, 
								   CONCAT(sc.subCategoryName, ' ', pb.productBrandName, ' ', pm.productModelName, ' ', 
										  phb.phoneBrandName, ' ', pt.phoneTypeName, ' ', pc.colorName) AS NamaProduk
							FROM Product p
							JOIN SubCategory sc ON p.subCategoryId = sc.subCategoryId
							JOIN ProductModel pm ON p.productModelId = pm.productModelId
							JOIN ProductBrand pb ON pm.productBrandId = pb.productBrandId
							JOIN PhoneType pt ON p.phoneTypeId = pt.phoneTypeId
							JOIN PhoneBrand phb ON pt.phoneBrandId = phb.phoneBrandId
							JOIN ProductColor pc ON p.colorID = pc.colorID
							WHERE p.productId LIKE @SearchText + '%' OR
							CONCAT(sc.subCategoryName, ' ', pb.productBrandName, ' ', pm.productModelName, ' ', 
								   phb.phoneBrandName, ' ', pt.phoneTypeName, ' ', pc.colorName) LIKE @SearchText + '%'";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@SearchText", term)
			};

			DataTable dt = dbCon.Fetch(query, parameters);
			foreach (DataRow row in dt.Rows)
			{
				suggestions.Add(new ProductSuggestion
				{
					ProductId = row["productId"].ToString(),
					NamaProduk = row["NamaProduk"].ToString()
				});
			}

			return suggestions;
		}

		protected void btnAdd_Click(object sender, EventArgs e)
		{
			string productId = hiddenProductId.Value;

			if (!string.IsNullOrEmpty(productId))
			{
				List<Product> selectedProducts = ViewState["SelectedProducts"] as List<Product> ?? new List<Product>();

				if (selectedProducts.Any(p => p.ProductId == productId))
				{
					return;
				}

				string query = @"
                        SELECT CONCAT(sc.subCategoryName, ' ', pb.productBrandName, ' ', pm.productModelName, ' ', 
                                      phb.phoneBrandName, ' ', pt.phoneTypeName, ' ', pc.colorName) AS NamaProduk, 
                               p.sellPrice, p.image
                        FROM Product p
                        JOIN SubCategory sc ON p.subCategoryId = sc.subCategoryId
                        JOIN ProductModel pm ON p.productModelId = pm.productModelId
                        JOIN ProductBrand pb ON pm.productBrandId = pb.productBrandId
                        JOIN PhoneType pt ON p.phoneTypeId = pt.phoneTypeId
                        JOIN PhoneBrand phb ON pt.phoneBrandId = phb.phoneBrandId
                        JOIN ProductColor pc ON p.colorID = pc.colorID
                        WHERE p.productId = @ProductId";

				SqlParameter[] parameters = new SqlParameter[]
				{
					new SqlParameter("@ProductId", productId)
				};

				DataTable dt = dbCon.Fetch(query, parameters);

				if (dt.Rows.Count > 0)
				{
					DataRow productRow = dt.Rows[0];
					string namaProduk = productRow["NamaProduk"].ToString();
					decimal hargaJual = Convert.ToDecimal(productRow["sellPrice"]);

					string imageBase64 = string.Empty;
					if (productRow["image"] != DBNull.Value)
					{
						byte[] imageBytes = (byte[])productRow["image"];
						imageBase64 = Convert.ToBase64String(imageBytes);
					}

					selectedProducts.Add(new Product
					{
						ProductId = productId,
						NamaProduk = namaProduk,
						Harga = hargaJual,
						ImageBase64 = imageBase64
					});

					ViewState["SelectedProducts"] = selectedProducts;

					BindTable();
				}
			}
		}

		private void BindTable()
		{
			List<Product> selectedProducts = ViewState["SelectedProducts"] as List<Product>;

			if (selectedProducts != null && selectedProducts.Count > 0)
			{
				rptProducts.DataSource = selectedProducts;
				rptProducts.DataBind();
			}
			else
			{
				rptProducts.DataSource = null;
				rptProducts.DataBind();
			}
		}

		private void RemoveProductFromViewState(string productId)
		{
			List<Product> selectedProducts = ViewState["SelectedProducts"] as List<Product>;

			if (selectedProducts != null)
			{
				Product productToRemove = selectedProducts.FirstOrDefault(p => p.ProductId == productId);
				if (productToRemove != null)
				{
					selectedProducts.Remove(productToRemove);

					if (selectedProducts.Count == 0)
					{
						ViewState["SelectedProducts"] = new List<Product>();
					}
					else
					{
						ViewState["SelectedProducts"] = selectedProducts;
					}
					BindTable();
				}
			}
		}

		private string GenerateUniqueSalesId()
		{
			string identifier = DateTime.Now.ToString("ddMMyyyyHHmmss");
			return "STR" + identifier;
		}

		private int GetMediaIdByName(string mediaName)
		{
			string query = "SELECT mediaId FROM Media WHERE mediaName = @mediaName";
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@mediaName", mediaName)
			};

			DataTable dt = dbCon.Fetch(query, parameters);
			if (dt.Rows.Count > 0)
			{
				return Convert.ToInt32(dt.Rows[0]["mediaId"]);
			}
			else
			{
				throw new Exception("Media tidak ditemukan.");
			}
		}

		protected void Add_Click(object sender, EventArgs e)
		{
			string userId = Session["UserId"].ToString();
			string salesId = GenerateUniqueSalesId();
			DateTime salesDate = DateTime.Now.Date;
			string mediaName = inputMedia.Value;
			int mediaId = GetMediaIdByName(mediaName);

			List<Product> selectedProducts = ViewState["SelectedProducts"] as List<Product>;

			if (selectedProducts != null && selectedProducts.Count > 0)
			{
				// Loop melalui tiap item di Repeater untuk update qty, diskon, dan subtotal
				foreach (RepeaterItem item in rptProducts.Items)
				{
					HiddenField hfProductId = (HiddenField)item.FindControl("hfProductId");
					TextBox txtQty = (TextBox)item.FindControl("txtQty");
					TextBox txtDiscount = (TextBox)item.FindControl("txtDisc");
					Label lblSubtotal = (Label)item.FindControl("product-subtotal");

					if (hfProductId != null && txtQty != null && txtDiscount != null)
					{
						string productId = hfProductId.Value;
						int qty = Convert.ToInt32(txtQty.Text);
						decimal discount;
						if (!decimal.TryParse(txtDiscount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out discount))
						{
							discount = 0;
						}
						decimal hargaSatuan = selectedProducts.First(p => p.ProductId == productId).Harga;
						decimal subtotal = hargaSatuan * qty * (1 - (discount / 100));

						// Temukan produk dalam list dan update nilai qty, diskon, dan subtotal
						Product productToUpdate = selectedProducts.FirstOrDefault(p => p.ProductId == productId);
						if (productToUpdate != null)
						{
							productToUpdate.Kuantitas = qty;
							productToUpdate.Diskon = discount;
							productToUpdate.Subtotal = subtotal;
						}
					}
				}

				// Mengecek stok di tabel produk 
				List<string> insufficientStockProducts = new List<string>();  
				foreach (var product in selectedProducts)
				{
					string queryCheckStock = "SELECT stock FROM Product WHERE productId = @productId";
					SqlParameter[] parametersCheckStock = new SqlParameter[]
					{
						new SqlParameter("@productId", product.ProductId)
					};

					DataTable stockData = dbCon.Fetch(queryCheckStock, parametersCheckStock);
					if (stockData.Rows.Count > 0)
					{
						int availableStock = Convert.ToInt32(stockData.Rows[0]["stock"]);

						if (availableStock < product.Kuantitas)
						{
							insufficientStockProducts.Add($"{product.ProductId} (Stok tersedia: {availableStock})");
						}
					}
					else
					{
						insufficientStockProducts.Add($"{product.ProductId} (Produk tidak ditemukan)");
					}
				}

				if (insufficientStockProducts.Count > 0)
				{
					string errorMessage = "Stok produk berikut tidak mencukupi:<br /><br />" +
						string.Join("<br />", insufficientStockProducts.Select((product, index) => $"{index + 1}. {product}"));

					ScriptManager.RegisterStartupScript(this, this.GetType(), "showWarningToast", $"launchWarningToast('{errorMessage}');", true);
					return; 
				}

				// Hitung total item, harga, diskon transaksi, dan harga setelah diskon
				int totalItem = selectedProducts.Sum(p => p.Kuantitas);
				decimal totalPrice = selectedProducts.Sum(p => p.Harga * p.Kuantitas);
				decimal transactionDiscount = selectedProducts.Sum(p => (p.Harga * p.Kuantitas) * (p.Diskon / 100));
				decimal discountedPrice = totalPrice - transactionDiscount;

				DateTime createdTime = DateTime.Now;
				DateTime updatedTime = DateTime.Now;

				// Query untuk insert data ke dalam tabel SalesTransaction
				string querySalesTransaction = @"
                        INSERT INTO SalesTransaction (salesId, userId, salesDate, mediaId, totalItem, totalPrice, transactionDiscount, discountedPrice, createdTime, updatedTime)
                        VALUES (@salesId, @userId, @salesDate, @mediaId, @totalItem, @totalPrice, @transactionDiscount, @discountedPrice, @createdTime, @updatedTime)";

				SqlParameter[] parametersSalesTransaction = new SqlParameter[]
				{
					new SqlParameter("@salesId", salesId),
					new SqlParameter("@userId", userId),
					new SqlParameter("@salesDate", salesDate),
					new SqlParameter("@mediaId", mediaId),
					new SqlParameter("@totalItem", totalItem),
					new SqlParameter("@totalPrice", totalPrice),
					new SqlParameter("@transactionDiscount", transactionDiscount),
					new SqlParameter("@discountedPrice", discountedPrice),
					new SqlParameter("@createdTime", createdTime),
					new SqlParameter("@updatedTime", updatedTime)
				};

				dbCon.Fetch(querySalesTransaction, parametersSalesTransaction);

				if (selectedProducts != null && selectedProducts.Count > 0)
				{
					foreach (var product in selectedProducts)
					{
						// Query untuk insert data ke dalam tabel SalesTransactionDetail
						string querySalesTransactionDetail = @"
										INSERT INTO SalesTransactionDetail 
										(salesId, productId, quantity, discount, subtotal, createdTime, updatedTime)
										VALUES 
										(@salesId, @productId, @quantity, @discount, @subtotal, @createdTime, @updatedTime)";

						SqlParameter[] parametersSalesTransactionDetail = new SqlParameter[]
						{
							new SqlParameter("@salesId", salesId),
							new SqlParameter("@productId", product.ProductId), 
							new SqlParameter("@quantity", product.Kuantitas),  
							new SqlParameter("@discount", product.Diskon),    
							new SqlParameter("@subtotal", product.Subtotal),   
							new SqlParameter("@createdTime", createdTime),
							new SqlParameter("@updatedTime", updatedTime)
						};

						dbCon.Fetch(querySalesTransactionDetail, parametersSalesTransactionDetail);

						// Query untuk update stok produk setelah pencatatan berhasil
						string queryUpdateProductStock = @"
							UPDATE Product 
							SET stock = stock - @quantity
							WHERE productId = @productId";

						SqlParameter[] parametersUpdateProductStock = new SqlParameter[]
						{
							new SqlParameter("@quantity", product.Kuantitas), 
							new SqlParameter("@productId", product.ProductId)  
						};

						dbCon.Fetch(queryUpdateProductStock, parametersUpdateProductStock);
					}
				}

				Response.Redirect("Sales.aspx");
			}
			else
			{
				ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorToast", "launchErrorToast('Belum ada produk yang ditambahkan');", true);
			}
		}
	}
}