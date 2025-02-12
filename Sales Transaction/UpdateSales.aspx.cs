using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;
using System.Globalization;

namespace Unipro_Store.Sales_Transaction
{
	public partial class UpdateSales : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();

		public string salesId
		{
			get { return (string)(ViewState["salesId"] ?? -1); }
			set { ViewState["salesId"] = value; }
		}

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
			if (Request.QueryString["salesId"] != null)
			{
				salesId = Request.QueryString["salesId"];
			}

			if (!IsPostBack)
			{
				ViewState["SelectedProducts"] = new List<Product>();
				GetTableData(); 
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

		public void GetTableData()
		{
			string query = @"
							SELECT M.mediaName AS salesMedia, ST.salesDate, STD.productId, STD.quantity AS Kuantitas, 
								   STD.discount AS Diskon, STD.subtotal, p.sellPrice AS Harga, p.image, ST.DiscountedPrice, 
								   CONCAT(sc.subCategoryName, ' ', pb.productBrandName, ' ', pm.productModelName, ' ', pbr.phoneBrandName, ' ', pt.phoneTypeName, ' ', pc.colorName) AS NamaProduk
							FROM SalesTransaction ST
							JOIN Media M ON ST.mediaId = M.mediaId
							JOIN SalesTransactionDetail STD ON ST.salesId = STD.salesId
							JOIN Product P ON STD.productId = P.productId
							JOIN SubCategory SC ON P.subCategoryId = SC.subCategoryId
							JOIN ProductModel PM ON P.productModelId = PM.productModelId
							JOIN ProductBrand PB ON PM.productBrandId = PB.productBrandId
							JOIN PhoneType PT ON P.phoneTypeId = PT.phoneTypeId
							JOIN PhoneBrand PBR ON PT.phoneBrandId = PBR.phoneBrandId
							JOIN ProductColor PC ON P.colorId = PC.colorId
							WHERE ST.salesId = @SalesId";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@SalesId", salesId)
			};

			DataTable dt = dbCon.Fetch(query, parameters);

			if (dt.Rows.Count > 0)
			{
				List<Product> selectedProducts = new List<Product>();

				foreach (DataRow dr in dt.Rows)
				{
					string media = dr["salesMedia"].ToString();
					inputMedia.Value = media;

					Product product = new Product
					{
						ProductId = dr["productId"].ToString(),
						NamaProduk = dr["NamaProduk"].ToString(),
						Harga = Convert.ToDecimal(dr["Harga"]),
						Kuantitas = Convert.ToInt32(dr["Kuantitas"]),
						Diskon = Convert.ToDecimal(dr["Diskon"]),
						Subtotal = Convert.ToDecimal(dr["subtotal"]),
						ImageBase64 = dr["image"] != DBNull.Value ? Convert.ToBase64String((byte[])dr["image"]) : string.Empty
					};

					selectedProducts.Add(product);
				}

				ViewState["SelectedProducts"] = selectedProducts;

				rptProducts.DataSource = selectedProducts;
				rptProducts.DataBind();
			}
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


		protected void Update_Click(object sender, EventArgs e)
		{
			string userId = Session["UserId"].ToString();
			DateTime updatedTime = DateTime.Now;
			string mediaName = inputMedia.Value;
			int mediaId = GetMediaIdByName(mediaName);

			// Mengambil daftar produk yang tersimpan sebelum update
			List<Product> selectedProducts = ViewState["SelectedProducts"] as List<Product>;

			// Ambil daftar produk dari database (sebelum diubah) untuk melakukan pengecekan perbandingan
			string queryExistingProducts = @"
									SELECT productId, quantity 
									FROM SalesTransactionDetail 
									WHERE salesId = @salesId";

			SqlParameter[] parametersExistingProducts = new SqlParameter[]
			{
				new SqlParameter("@salesId", salesId)
			};

			DataTable dtExistingProducts = dbCon.Fetch(queryExistingProducts, parametersExistingProducts);
			List<string> existingProductIds = dtExistingProducts.AsEnumerable().Select(row => row["productId"].ToString()).ToList();

			// Simpan daftar produk yang baru (setelah update)
			List<string> updatedProductIds = new List<string>();

			if (selectedProducts != null && selectedProducts.Count > 0)
			{
				Dictionary<string, int> availableStockMap = new Dictionary<string, int>();
				Dictionary<string, int> originalStockMap = new Dictionary<string, int>();

				List<string> insufficientStockProducts = new List<string>(); 

				// Loop melalui tiap item di Repeater untuk update qty, diskon, subtotal, dan simpan produk yang baru
				foreach (RepeaterItem item in rptProducts.Items)
				{
					HiddenField hfProductId = (HiddenField)item.FindControl("hfProductId");
					TextBox txtQty = (TextBox)item.FindControl("txtQty");
					TextBox txtDiscount = (TextBox)item.FindControl("txtDisc");

					if (hfProductId != null && txtQty != null && txtDiscount != null)
					{
						string productId = hfProductId.Value;
						updatedProductIds.Add(productId); 

						int newQty = Convert.ToInt32(txtQty.Text);
						decimal discount;
						if (!decimal.TryParse(txtDiscount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out discount))
						{
							discount = 0;
						}

						// Update produk di ViewState
						Product productToUpdate = selectedProducts.FirstOrDefault(p => p.ProductId == productId);
						if (productToUpdate != null)
						{
							decimal hargaSatuan = productToUpdate.Harga;
							decimal subtotal = hargaSatuan * newQty * (1 - (discount / 100));

							int oldQty = dtExistingProducts.AsEnumerable()
										  .Where(row => row["productId"].ToString() == productId)
										  .Select(row => Convert.ToInt32(row["quantity"]))
										  .FirstOrDefault();

							// Ambil stok tersedia untuk produk ini
							if (!availableStockMap.ContainsKey(productId))
							{
								string queryCheckStock = "SELECT stock FROM Product WHERE productId = @productId";
								SqlParameter[] parametersCheckStock = new SqlParameter[]
								{
									new SqlParameter("@productId", productId)
								};

								DataTable stockData = dbCon.Fetch(queryCheckStock, parametersCheckStock);
								int availableStock = Convert.ToInt32(stockData.Rows[0]["stock"]);

								availableStockMap[productId] = availableStock;
								originalStockMap[productId] = availableStock + oldQty;
							}

							int availableStockForProduct = availableStockMap[productId];
							int originalStockForProduct = originalStockMap[productId];

							// Periksa apakah stok cukup setelah perubahan kuantitas
							int qtyDifference = newQty - oldQty;
							if (availableStockForProduct < qtyDifference)
							{
								insufficientStockProducts.Add($"{productToUpdate.ProductId} (Stok tersedia: {originalStockForProduct})");
							}
							else
							{
								productToUpdate.Kuantitas = newQty;
								productToUpdate.Diskon = discount;
								productToUpdate.Subtotal = subtotal;

								availableStockMap[productId] -= qtyDifference;
							}
						}
					}
				}

				// Jika ada produk yang stoknya tidak mencukupi, tampilkan pesan peringatan
				if (insufficientStockProducts.Count > 0)
				{
					string errorMessage = "Stok produk berikut tidak mencukupi:<br /><br />" +
						string.Join("<br />", insufficientStockProducts.Select((product, index) => $"{index + 1}. {product}"));

					ScriptManager.RegisterStartupScript(this, this.GetType(), "showWarningToast", $"launchWarningToast('{errorMessage}');", true);
					return;
				}

				foreach (var product in selectedProducts)
				{
					int oldQty = dtExistingProducts.AsEnumerable()
									  .Where(row => row["productId"].ToString() == product.ProductId)
									  .Select(row => Convert.ToInt32(row["quantity"]))
									  .FirstOrDefault();

					int qtyDifference = product.Kuantitas - oldQty;

					if (qtyDifference != 0) // Update stok jika ada perbedaan
					{
						string queryUpdateProductStock = @"
														UPDATE Product 
														SET stock = stock - @quantity
														WHERE productId = @productId";

						SqlParameter[] parametersUpdateProductStock = new SqlParameter[]
						{
							new SqlParameter("@quantity", qtyDifference),
							new SqlParameter("@productId", product.ProductId)
						};

						dbCon.Fetch(queryUpdateProductStock, parametersUpdateProductStock);
					}
				}

				// Hitung total item, harga, diskon transaksi, dan harga setelah diskon
				int totalItem = selectedProducts.Sum(p => p.Kuantitas);
				decimal totalPrice = selectedProducts.Sum(p => p.Harga * p.Kuantitas);
				decimal transactionDiscount = selectedProducts.Sum(p => (p.Harga * p.Kuantitas) * (p.Diskon / 100));
				decimal discountedPrice = totalPrice - transactionDiscount;

				// Query untuk update data di tabel SalesTransaction
				string querySalesTransaction = @"
											UPDATE SalesTransaction
											SET mediaId = @mediaId, totalItem = @totalItem, totalPrice = @totalPrice, 
												transactionDiscount = @transactionDiscount, discountedPrice = @discountedPrice, 
												updatedTime = @updatedTime
											WHERE salesId = @salesId";

				SqlParameter[] parametersSalesTransaction = new SqlParameter[]
				{
					new SqlParameter("@salesId", salesId),
					new SqlParameter("@mediaId", mediaId),
					new SqlParameter("@totalItem", totalItem),
					new SqlParameter("@totalPrice", totalPrice),
					new SqlParameter("@transactionDiscount", transactionDiscount),
					new SqlParameter("@discountedPrice", discountedPrice),
					new SqlParameter("@updatedTime", updatedTime)
				};

				dbCon.Fetch(querySalesTransaction, parametersSalesTransaction);

				// Update atau tambahkan detail transaksi penjualan
				foreach (var product in selectedProducts)
				{
					string querySalesTransactionDetail = @"
														IF EXISTS (SELECT 1 FROM SalesTransactionDetail WHERE salesId = @salesId AND productId = @productId)
														BEGIN
															UPDATE SalesTransactionDetail
															SET quantity = @quantity, discount = @discount, subtotal = @subtotal, updatedTime = @updatedTime
															WHERE salesId = @salesId AND productId = @productId
														END
														ELSE
														BEGIN
															INSERT INTO SalesTransactionDetail (salesId, productId, quantity, discount, subtotal, createdTime, updatedTime)
															VALUES (@salesId, @productId, @quantity, @discount, @subtotal, @createdTime, @updatedTime)
														END";

					SqlParameter[] parametersSalesTransactionDetail = new SqlParameter[]
					{
						new SqlParameter("@salesId", salesId),
						new SqlParameter("@productId", product.ProductId),
						new SqlParameter("@quantity", product.Kuantitas),
						new SqlParameter("@discount", product.Diskon),
						new SqlParameter("@subtotal", product.Subtotal),
						new SqlParameter("@createdTime", updatedTime),
						new SqlParameter("@updatedTime", updatedTime)
					};

					dbCon.Fetch(querySalesTransactionDetail, parametersSalesTransactionDetail);
				}

				// Hapus Produk yang Tidak Ada di Daftar Baru
				var deletedProductIds = existingProductIds.Except(updatedProductIds).ToList();
				foreach (DataRow row in dtExistingProducts.Rows)
				{
					string deletedProductId = row["productId"].ToString();
					int qty = Convert.ToInt32(row["quantity"]);
					if (deletedProductIds.Contains(deletedProductId))
					{
						string queryDeleteProduct = @"
													DELETE FROM SalesTransactionDetail
													WHERE salesId = @salesId AND productId = @productId";

						SqlParameter[] parametersDeleteProduct = new SqlParameter[]
						{
							new SqlParameter("@salesId", salesId),
							new SqlParameter("@productId", deletedProductId)
						};

						dbCon.Fetch(queryDeleteProduct, parametersDeleteProduct);

						// Kembalikan stok produk yang dihapus sesuai dengan kuantitas yang ada
						string queryUpdateProductStockForDeleted = @"
																UPDATE Product
																SET stock = stock + @quantity
																WHERE productId = @productId";

						SqlParameter[] parametersUpdateProductStockForDeleted = new SqlParameter[]
						{
							new SqlParameter("@quantity", qty),
							new SqlParameter("@productId", deletedProductId)
						};
						dbCon.Fetch(queryUpdateProductStockForDeleted, parametersUpdateProductStockForDeleted);
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