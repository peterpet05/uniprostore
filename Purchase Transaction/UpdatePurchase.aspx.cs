using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;
using System.IO;

namespace Unipro_Store.Purchase_Transaction
{
	public partial class UpdatePurchase : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();

		public string purchaseId
		{
			get { return (string)(ViewState["purchaseId"] ?? -1); }
			set { ViewState["purchaseId"] = value; }
		}

		public Int32 supplierId
		{
			get { return (Int32)(ViewState["supplierId"] ?? -1); }
			set { ViewState["supplierId"] = value; }
		}

		[Serializable]
		public class Product
		{
			public string ProductId { get; set; }
			public string NamaProduk { get; set; }
			public decimal Harga { get; set; }
			public int Kuantitas { get; set; }
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
			if (Request.QueryString["purchaseId"] != null)
			{
				purchaseId = Request.QueryString["purchaseId"];
			}

			if (!IsPostBack)
			{
				ViewState["SelectedProducts"] = new List<Product>();
				LoadSuppliers();
				inputSupplier.Attributes.Add("required", "required");
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

		private void LoadSuppliers()
		{
			CommonFunction dbCon = new CommonFunction();
			DataTable dtSuppliers = dbCon.Fetch("SELECT supplierId, supplierName FROM Supplier");

			inputSupplier.DataSource = dtSuppliers;
			inputSupplier.DataValueField = "supplierId";
			inputSupplier.DataTextField = "supplierName";
			inputSupplier.DataBind();
			inputSupplier.Items.Insert(0, new ListItem("Pilih supplier", ""));
		}

		public void GetTableData()
		{
			string query = @"SELECT PT.purchaseDate, PTD.productId, PTD.quantity AS Kuantitas, 
							PTD.subtotal, PTD.buyPrice AS Harga, P.image, PT.totalPrice, S.supplierId, S.supplierName,
							CONCAT(SC.subCategoryName, ' ', PB.productBrandName, ' ', PM.productModelName, ' ', PBR.phoneBrandName, ' ', PTY.phoneTypeName, ' ', PC.colorName) AS NamaProduk
							FROM PurchaseTransaction PT
							JOIN PurchaseTransactionDetail PTD ON PT.purchaseId = PTD.purchaseId
							JOIN Supplier S ON PT.supplierId = S.supplierId
							JOIN Product P ON PTD.productId = P.productId
							JOIN SubCategory SC ON P.subCategoryId = SC.subCategoryId
							JOIN ProductModel PM ON P.productModelId = PM.productModelId
							JOIN ProductBrand PB ON PM.productBrandId = PB.productBrandId
							JOIN PhoneType PTY ON P.phoneTypeId = PTY.phoneTypeId
							JOIN PhoneBrand PBR ON PTY.phoneBrandId = PBR.phoneBrandId
							JOIN ProductColor PC ON P.colorId = PC.colorId
							WHERE PT.purchaseId = @PurchaseId";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@PurchaseId", purchaseId)
			};

			DataTable dt = dbCon.Fetch(query, parameters);
			if (dt.Rows.Count > 0)
			{
				List<Product> selectedProducts = new List<Product>();

				foreach (DataRow dr in dt.Rows)
				{
					string supplierId = dr["supplierId"].ToString();
					inputSupplier.SelectedValue = supplierId;

					Product product = new Product
					{
						ProductId = dr["productId"].ToString(),
						NamaProduk = dr["NamaProduk"].ToString(),
						Harga = Convert.ToDecimal(dr["Harga"]),
						Kuantitas = Convert.ToInt32(dr["Kuantitas"]),
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
                               p.buyPrice, p.image
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
					decimal hargaBeli = Convert.ToDecimal(productRow["buyPrice"]);

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
						Harga = hargaBeli,
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

		protected void inputSupplier_SelectedIndexChanged(object sender, EventArgs e)
		{
			string selectedSupplier = inputSupplier.SelectedValue;

			if (!string.IsNullOrEmpty(selectedSupplier))
			{
				supplierId = Convert.ToInt32(selectedSupplier);
			}
		}

		protected void Update_Click(object sender, EventArgs e)
		{
			DateTime updatedTime = DateTime.Now;
			string supplierName = inputSupplier.SelectedValue;

			if (string.IsNullOrEmpty(inputSupplier.SelectedValue))
			{
				supplierId = Convert.ToInt32(ViewState["supplierId"]);
			}
			else
			{
				supplierId = Convert.ToInt32(inputSupplier.SelectedValue);
				ViewState["supplierId"] = supplierId; 
			}

			// Mengambil daftar produk yang tersimpan sebelum update
			List<Product> selectedProducts = ViewState["SelectedProducts"] as List<Product>;

			// Ambil daftar produk dari database (sebelum diubah) untuk melakukan pengecekan perbandingan
			string queryExistingProducts = @"
										SELECT productId, quantity 
										FROM PurchaseTransactionDetail 
										WHERE purchaseId = @purchaseId";

			SqlParameter[] parametersExistingProducts = new SqlParameter[]
			{
				new SqlParameter("@purchaseId", purchaseId)
			};

			DataTable dtExistingProducts = dbCon.Fetch(queryExistingProducts, parametersExistingProducts);
			List<string> existingProductIds = dtExistingProducts.AsEnumerable().Select(row => row["productId"].ToString()).ToList();

			// Simpan daftar produk yang baru (setelah update)
			List<string> updatedProductIds = new List<string>();

			if (selectedProducts != null && selectedProducts.Count > 0)
			{
				// Loop melalui tiap item di Repeater untuk update qty, diskon, subtotal, dan simpan produk yang baru
				foreach (RepeaterItem item in rptProducts.Items)
				{
					HiddenField hfProductId = (HiddenField)item.FindControl("hfProductId");
					TextBox txtQty = (TextBox)item.FindControl("txtQty");
					TextBox price = (TextBox)item.FindControl("txtPrice");

					if (hfProductId != null && txtQty != null)
					{
						string productId = hfProductId.Value;
						updatedProductIds.Add(productId); // Tambahkan produk ke daftar produk yang baru

						int newQty = Convert.ToInt32(txtQty.Text);


						// Update produk di ViewState
						Product productToUpdate = selectedProducts.FirstOrDefault(p => p.ProductId == productId);
						if (productToUpdate != null)
						{
							decimal hargaSatuan = Convert.ToInt32(price.Text);
							decimal subtotal = hargaSatuan * newQty;

							int oldQty = dtExistingProducts.AsEnumerable()
										  .Where(row => row["productId"].ToString() == productId)
										  .Select(row => Convert.ToInt32(row["quantity"]))
										  .FirstOrDefault();

							// Perbarui produk di ViewState
							productToUpdate.Kuantitas = newQty;
							productToUpdate.Subtotal = subtotal;
							productToUpdate.Harga = hargaSatuan;

							// Cek apakah ada perubahan kuantitas
							if (newQty != oldQty)
							{
								int qtyDifference = newQty - oldQty;

								// Update stok hanya jika ada perbedaan kuantitas
								string queryUpdateProductStock = @"
																UPDATE Product 
																SET stock = stock + @quantity
																WHERE productId = @productId";

								SqlParameter[] parametersUpdateProductStock = new SqlParameter[]
								{
									new SqlParameter("@quantity", qtyDifference),
									new SqlParameter("@productId", productId)
								};

								dbCon.Fetch(queryUpdateProductStock, parametersUpdateProductStock);
							}
						}
					}
				}

				// Hitung total item, harga, diskon transaksi, dan harga setelah diskon
				int totalItem = selectedProducts.Sum(p => p.Kuantitas);
				decimal totalPrice = selectedProducts.Sum(p => p.Harga * p.Kuantitas);
				byte[] invoiceFileData = null;
				bool isInvoiceUpdated = inputInvoice.HasFile;

				if (isInvoiceUpdated)
				{
					string fileExtension = Path.GetExtension(inputInvoice.FileName).ToLower();
					string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

					if (!allowedExtensions.Contains(fileExtension))
					{
						ScriptManager.RegisterStartupScript(this, this.GetType(), "showWarningToast",
							"launchWarningToast('Faktur harus dalam format JPG, PNG, atau PDF!');", true);
						return;
					}

					using (BinaryReader br = new BinaryReader(inputInvoice.PostedFile.InputStream))
					{
						invoiceFileData = br.ReadBytes(inputInvoice.PostedFile.ContentLength);
					}
				}

				// Query untuk update data di tabel PurchaseTransaction
				string queryPurchaseTransaction = @"
												UPDATE PurchaseTransaction
												SET supplierId = @supplierId, totalItem = @totalItem, totalPrice = @totalPrice, updatedTime = @updatedTime, 
													invoiceImage = CASE 
														WHEN @invoiceImage IS NOT NULL THEN CONVERT(varbinary(max), @invoiceImage)
														ELSE invoiceImage 
													END
												WHERE purchaseId = @purchaseId";

				SqlParameter[] parametersPurchaseTransaction = new SqlParameter[]
				{
					new SqlParameter("@purchaseId", purchaseId),
					new SqlParameter("@supplierId", supplierId),
					new SqlParameter("@totalItem", totalItem),
					new SqlParameter("@totalPrice", totalPrice),
					new SqlParameter("@updatedTime", updatedTime),
					new SqlParameter("@invoiceImage", isInvoiceUpdated ? invoiceFileData : (object)DBNull.Value)
				};

				dbCon.Fetch(queryPurchaseTransaction, parametersPurchaseTransaction);

				// Update atau tambahkan detail transaksi produk
				foreach (var product in selectedProducts)
				{
					string queryPurchaseTransactionDetail = @"
														IF EXISTS (SELECT 1 FROM PurchaseTransactionDetail WHERE purchaseId = @purchaseId AND productId = @productId)
														BEGIN
															UPDATE PurchaseTransactionDetail
															SET quantity = @quantity, subtotal = @subtotal, updatedTime = @updatedTime, buyPrice = @buyPrice
															WHERE purchaseId = @purchaseId AND productId = @productId
														END
														ELSE
														BEGIN
															INSERT INTO PurchaseTransactionDetail (purchaseId, productId, quantity, subtotal, createdTime, updatedTime)
															VALUES (@purchaseId, @productId, @quantity, @subtotal, @createdTime, @updatedTime)
														END";

					SqlParameter[] parametersPurchaseTransactionDetail = new SqlParameter[]
					{
						new SqlParameter("@purchaseId", purchaseId),
						new SqlParameter("@productId", product.ProductId),
						new SqlParameter("@quantity", product.Kuantitas),
						new SqlParameter("@subtotal", product.Subtotal),
						new SqlParameter("@buyPrice", product.Harga), 
						new SqlParameter("@createdTime", updatedTime),
						new SqlParameter("@updatedTime", updatedTime)
					};

					dbCon.Fetch(queryPurchaseTransactionDetail, parametersPurchaseTransactionDetail);
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
													DELETE FROM PurchaseTransactionDetail
													WHERE purchaseId = @purchaseId AND productId = @productId";

						SqlParameter[] parametersDeleteProduct = new SqlParameter[]
						{
							new SqlParameter("@purchaseId", purchaseId),
							new SqlParameter("@productId", deletedProductId)
						};

						dbCon.Fetch(queryDeleteProduct, parametersDeleteProduct);

						// Kembalikan stok produk yang dihapus sesuai dengan kuantitas yang ada
						string queryUpdateProductStockForDeleted = @"
																UPDATE Product 
																SET stock = stock - @quantity
																WHERE productId = @productId";

						SqlParameter[] parametersUpdateProductStockForDeleted = new SqlParameter[]
						{
							new SqlParameter("@quantity", qty),
							new SqlParameter("@productId", deletedProductId)
						};

						dbCon.Fetch(queryUpdateProductStockForDeleted, parametersUpdateProductStockForDeleted);
					}
				}
				Response.Redirect("Purchase.aspx");
			}
			else
			{
				ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorToast", "launchErrorToast('Belum ada produk yang ditambahkan');", true);
			}
		}
	}
}