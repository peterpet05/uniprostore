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
using Unipro_Store.Product_Model;
using System.IO;

namespace Unipro_Store.Purchase_Transaction
{
	public partial class CreatePurchase : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();

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

				LoadSuppliers();
				inputSupplier.Attributes.Add("required", "required");
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
			DataTable dtSuppliers = dbCon.Fetch("SELECT supplierId, supplierName FROM Supplier WHERE isActive = 1");

			inputSupplier.DataSource = dtSuppliers;
			inputSupplier.DataValueField = "supplierId";
			inputSupplier.DataTextField = "supplierName";
			inputSupplier.DataBind();
			inputSupplier.Items.Insert(0, new ListItem("Pilih supplier", ""));
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

		private string GenerateUniquePurchaseId()
		{
			string identifier = DateTime.Now.ToString("ddMMyyyyHHmmss");
			return "PTR" + identifier;
		}

		protected void Add_Click(object sender, EventArgs e)
		{
			string purchaseId = GenerateUniquePurchaseId();
			DateTime purchaseDate = DateTime.Now.Date;

			List<Product> selectedProducts = ViewState["SelectedProducts"] as List<Product>;

			if (selectedProducts != null && selectedProducts.Count > 0)
			{
				// Loop melalui tiap item di Repeater untuk update qty dan subtotal
				foreach (RepeaterItem item in rptProducts.Items)
				{
					HiddenField hfProductId = (HiddenField)item.FindControl("hfProductId");
					TextBox txtQty = (TextBox)item.FindControl("txtQty");
					TextBox price = (TextBox)item.FindControl("txtPrice");
					Label lblSubtotal = (Label)item.FindControl("product-subtotal");

					if (hfProductId != null && txtQty != null)
					{
						string productId = hfProductId.Value;
						int qty = Convert.ToInt32(txtQty.Text);
						decimal hargaSatuan = Convert.ToInt32(price.Text);
						//decimal hargaSatuan = selectedProducts.First(p => p.ProductId == productId).Harga;
						decimal subtotal = hargaSatuan * qty;

						// Temukan produk dalam list dan update nilai qty dan subtotal
						Product productToUpdate = selectedProducts.FirstOrDefault(p => p.ProductId == productId);
						if (productToUpdate != null)
						{
							productToUpdate.Kuantitas = qty;
							productToUpdate.Subtotal = subtotal;
							productToUpdate.Harga = hargaSatuan;
						}
					}
				}

				// Hitung total item dan total harga
				int totalItem = selectedProducts.Sum(p => p.Kuantitas);
				decimal totalPrice = selectedProducts.Sum(p => p.Harga * p.Kuantitas);
		

				DateTime createdTime = DateTime.Now;
				DateTime updatedTime = DateTime.Now;
				byte[] fileData = null;
				if (inputInvoice.HasFile)
				{
					string fileExtension = Path.GetExtension(inputInvoice.FileName).ToLower();
					string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

					if (!allowedExtensions.Contains(fileExtension))
					{
						ScriptManager.RegisterStartupScript(this, this.GetType(), "showWarningToast",
							"launchWarningToast('Faktur harus diupload dalam format JPG atau PNG!');", true);
						return;
					}

					using (BinaryReader br = new BinaryReader(inputInvoice.PostedFile.InputStream))
					{
						fileData = br.ReadBytes(inputInvoice.PostedFile.ContentLength);
					}
				}
				else
				{
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showWarningToast",
						"launchWarningToast('Silakan unggah faktur pembelian terlebih dahulu!');", true);
					return;
				}

				// Query untuk insert data ke dalam tabel PurchaseTransaction
				string queryPurchaseTransaction = @"
								INSERT INTO PurchaseTransaction (purchaseId, supplierId, purchaseDate, totalItem, totalPrice, createdTime, updatedTime, invoiceImage)
								VALUES (@purchaseId, @supplierId, @purchaseDate, @totalItem, @totalPrice, @createdTime, @updatedTime, @invoiceImage)";

				SqlParameter[] parametersPurchaseTransaction = new SqlParameter[]
				{
					new SqlParameter("@purchaseId", purchaseId),
					new SqlParameter("@supplierId", supplierId),
					new SqlParameter("@purchaseDate", purchaseDate),
					new SqlParameter("@totalItem", totalItem),
					new SqlParameter("@totalPrice", totalPrice),
					new SqlParameter("@createdTime", createdTime),
					new SqlParameter("@updatedTime", updatedTime),
					new SqlParameter("@invoiceImage", fileData ?? (object)DBNull.Value)
				};

				dbCon.Fetch(queryPurchaseTransaction, parametersPurchaseTransaction);

				if (selectedProducts != null && selectedProducts.Count > 0)
				{
					foreach (var product in selectedProducts)
					{
						// Query untuk insert data ke dalam tabel PurchaseTransactionDetail
						string queryPurchaseTransactionDetail = @"
								INSERT INTO PurchaseTransactionDetail 
								(purchaseId, productId, quantity, buyPrice, subtotal, createdTime, updatedTime)
								VALUES 
								(@purchaseId, @productId, @quantity, @buyPrice, @subtotal, @createdTime, @updatedTime)";

						SqlParameter[] parametersPurchaseTransactionDetail = new SqlParameter[]
						{
							new SqlParameter("@purchaseId", purchaseId),
							new SqlParameter("@productId", product.ProductId),
							new SqlParameter("@quantity", product.Kuantitas),
							new SqlParameter("@buyPrice", product.Harga),
							new SqlParameter("@subtotal", product.Subtotal),
							new SqlParameter("@createdTime", createdTime),
							new SqlParameter("@updatedTime", updatedTime)
						};

						dbCon.Fetch(queryPurchaseTransactionDetail, parametersPurchaseTransactionDetail);

						// Query untuk update stok produk setelah pencatatan berhasil
						string queryUpdateProductStock = @"
							UPDATE Product 
							SET stock = stock + @quantity
							WHERE productId = @productId";

						SqlParameter[] parametersUpdateProductStock = new SqlParameter[]
						{
							new SqlParameter("@quantity", product.Kuantitas),
							new SqlParameter("@productId", product.ProductId)
						};

						dbCon.Fetch(queryUpdateProductStock, parametersUpdateProductStock);
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