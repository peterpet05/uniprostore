using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Product
{
	public partial class CreateVariation : System.Web.UI.Page
	{
		public string productId
		{
			get { return ViewState["productId"]?.ToString() ?? string.Empty; }
			set { ViewState["productId"] = value; }
		}

		public Int32 categoryId
		{
			get { return (Int32)(ViewState["categoryId"] ?? -1); }
			set { ViewState["categoryId"] = value; }
		}

		public Int32 subCategoryId
		{
			get { return (Int32)(ViewState["subCategoryId"] ?? -1); }
			set { ViewState["subCategoryId"] = value; }
		}

		public Int32 productColorId
		{
			get { return (Int32)(ViewState["productColorId"] ?? -1); }
			set { ViewState["productColorId"] = value; }
		}

		public Int32 productBrandId
		{
			get { return (Int32)(ViewState["productBrandId"] ?? -1); }
			set { ViewState["productBrandId"] = value; }
		}

		public Int32 productModelId
		{
			get { return (Int32)(ViewState["productModelId"] ?? -1); }
			set { ViewState["productModelId"] = value; }
		}

		public Int32 phoneBrandId
		{
			get { return (Int32)(ViewState["phoneBrandId"] ?? -1); }
			set { ViewState["phoneBrandId"] = value; }
		}

		public Int32 phoneTypeId
		{
			get { return (Int32)(ViewState["phoneTypeId"] ?? -1); }
			set { ViewState["phoneTypeId"] = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadCategories();
				LoadProductBrands();
				LoadPhoneBrands();
				LoadProductColors();

				if (Request.QueryString["productId"] != null)
				{
					productId = Request.QueryString["productId"];
					LoadProductDetails(productId);
				}

				inputCategory.Attributes.Add("required", "required");
				inputSubCategory.Attributes.Add("required", "required");
				inputProductBrand.Attributes.Add("required", "required");
				inputProductModel.Attributes.Add("required", "required");
				inputPhoneBrand.Attributes.Add("required", "required");
				inputPhoneType.Attributes.Add("required", "required");
				inputProductColor.Attributes.Add("required", "required");
			}
		}

		private void DisableDropdowns()
		{
			inputCategory.Enabled = false;
			inputSubCategory.Enabled = false;
			inputProductBrand.Enabled = false;
			inputProductModel.Enabled = false;
			inputPhoneBrand.Enabled = false;
			inputPhoneType.Enabled = false;
			inputProductColor.Enabled = false;
		}

		private void LoadCategories()
		{
			CommonFunction dbCon = new CommonFunction();
			DataTable dtCategories = dbCon.Fetch("SELECT categoryId, categoryName FROM Category");

			inputCategory.DataSource = dtCategories;
			inputCategory.DataValueField = "categoryId";
			inputCategory.DataTextField = "categoryName";
			inputCategory.DataBind();
			inputCategory.Items.Insert(0, new ListItem("Pilih kategori", ""));
		}

		private void LoadSubCategories()
		{
			if (categoryId == -1) return;

			CommonFunction dbCon = new CommonFunction();
			DataTable dtSubCategories = dbCon.Fetch($"SELECT subCategoryId, subCategoryName FROM SubCategory WHERE categoryId = {categoryId}");

			inputSubCategory.DataSource = dtSubCategories;
			inputSubCategory.DataValueField = "subCategoryId";
			inputSubCategory.DataTextField = "subCategoryName";
			inputSubCategory.DataBind();
			inputSubCategory.Items.Insert(0, new ListItem("Pilih subkategori", ""));
		}

		private void LoadProductBrands()
		{
			CommonFunction dbCon = new CommonFunction();
			DataTable dtProductBrands = dbCon.Fetch("SELECT productBrandId, productBrandName FROM ProductBrand");

			inputProductBrand.DataSource = dtProductBrands;
			inputProductBrand.DataValueField = "productBrandId";
			inputProductBrand.DataTextField = "productBrandName";
			inputProductBrand.DataBind();
			inputProductBrand.Items.Insert(0, new ListItem("Pilih merek produk", ""));
		}

		private void LoadProductModels()
		{
			if (productBrandId == -1) return;

			CommonFunction dbCon = new CommonFunction();
			DataTable dtProductModels = dbCon.Fetch($"SELECT productModelId, productModelName FROM ProductModel WHERE productBrandId = {productBrandId} ORDER BY productModelName");

			inputProductModel.DataSource = dtProductModels;
			inputProductModel.DataValueField = "productModelId";
			inputProductModel.DataTextField = "productModelName";
			inputProductModel.DataBind();
			inputProductModel.Items.Insert(0, new ListItem("Pilih model produk", ""));
		}

		private void LoadPhoneBrands()
		{
			CommonFunction dbCon = new CommonFunction();
			DataTable dtPhoneBrands = dbCon.Fetch("SELECT phoneBrandId, phoneBrandName FROM PhoneBrand");

			inputPhoneBrand.DataSource = dtPhoneBrands;
			inputPhoneBrand.DataValueField = "phoneBrandId";
			inputPhoneBrand.DataTextField = "phoneBrandName";
			inputPhoneBrand.DataBind();
			inputPhoneBrand.Items.Insert(0, new ListItem("Pilih brand ponsel", ""));
		}

		private void LoadPhoneTypes()
		{
			if (phoneBrandId == -1) return;

			CommonFunction dbCon = new CommonFunction();
			DataTable dtPhoneTypes = dbCon.Fetch($"SELECT phoneTypeId, phoneTypeName FROM PhoneType WHERE phoneBrandId = {phoneBrandId} ORDER BY phoneTypeName");

			inputPhoneType.DataSource = dtPhoneTypes;
			inputPhoneType.DataValueField = "phoneTypeId";
			inputPhoneType.DataTextField = "phoneTypeName";
			inputPhoneType.DataBind();
			inputPhoneType.Items.Insert(0, new ListItem("Pilih tipe ponsel", ""));
		}

		private void LoadProductColors()
		{
			CommonFunction dbCon = new CommonFunction();
			DataTable dtProductColors = dbCon.Fetch("SELECT colorId, colorName FROM ProductColor ORDER BY colorName");

			inputProductColor.DataSource = dtProductColors;
			inputProductColor.DataValueField = "colorId";
			inputProductColor.DataTextField = "colorName";
			inputProductColor.DataBind();
			inputProductColor.Items.Insert(0, new ListItem("Pilih warna produk", ""));
		}

		private void LoadProductDetails(string productId)
		{
			string query = @"
							SELECT p.*, 
								   sc.categoryId, 
								   pm.productBrandId, 
								   pt.phoneBrandId, 
								   sc.subCategoryId, 
								   pm.productModelId, 
								   pt.phoneTypeId, 
								   pc.colorId 
							FROM Product p
							JOIN SubCategory sc ON p.subCategoryId = sc.subCategoryId
							JOIN ProductModel pm ON p.productModelId = pm.productModelId
							JOIN PhoneType pt ON p.phoneTypeId = pt.phoneTypeId
							JOIN ProductColor pc ON p.colorId = pc.colorId
							WHERE p.productId = @ProductId";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@ProductId", productId)
			};

			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(query, parameters);

			if (dt.Rows.Count > 0)
			{
				DataRow row = dt.Rows[0];

				categoryId = Convert.ToInt32(row["categoryId"]);
				inputCategory.SelectedValue = categoryId.ToString();
				LoadSubCategories();
				inputSubCategory.SelectedValue = row["subCategoryId"].ToString();

				productBrandId = Convert.ToInt32(row["productBrandId"]);
				inputProductBrand.SelectedValue = productBrandId.ToString();
				LoadProductModels();
				inputProductModel.SelectedValue = row["productModelId"].ToString();

				phoneBrandId = Convert.ToInt32(row["phoneBrandId"]);
				inputPhoneBrand.SelectedValue = phoneBrandId.ToString();
				LoadPhoneTypes();
				inputPhoneType.SelectedValue = row["phoneTypeId"].ToString();

				inputProductColor.SelectedValue = row["colorId"].ToString();

				DisableDropdowns();
			}
		}

		protected void inputCategory_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (int.TryParse(inputCategory.SelectedValue, out int selectedCategoryId))
			{
				categoryId = selectedCategoryId;
				LoadSubCategories();
				inputSubCategory.Enabled = true;
			}
			else
			{
				inputSubCategory.SelectedIndex = 0;
				inputSubCategory.Enabled = false;
			}
		}

		protected void inputProductBrand_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (int.TryParse(inputProductBrand.SelectedValue, out int selectedProductBrandId))
			{
				productBrandId = selectedProductBrandId;
				LoadProductModels();
				inputProductModel.Enabled = true;
			}
			else
			{
				inputProductModel.SelectedIndex = 0;
				inputProductModel.Enabled = false;
			}
		}

		protected void inputProductModel_SelectedIndexChanged(object sender, EventArgs e)
		{
			string selectedProductModel = inputProductModel.SelectedValue;

			if (!string.IsNullOrEmpty(selectedProductModel))
			{
				productModelId = Convert.ToInt32(selectedProductModel);
			}
		}


		protected void inputPhoneBrand_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (int.TryParse(inputPhoneBrand.SelectedValue, out int selectedPhoneBrandId))
			{
				phoneBrandId = selectedPhoneBrandId;
				LoadPhoneTypes();
				inputPhoneType.Enabled = true;
			}
			else
			{
				inputPhoneType.SelectedIndex = 0;
				inputPhoneType.Enabled = false;
			}
		}

		protected void inputSubCategory_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (int.TryParse(inputSubCategory.SelectedValue, out int selectedSubCategoryId))
			{
				subCategoryId = selectedSubCategoryId;
			}
		}

		protected void inputPhoneType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (int.TryParse(inputPhoneType.SelectedValue, out int selectedPhoneTypeId))
			{
				phoneTypeId = selectedPhoneTypeId;
			}
		}

		protected void inputProductColor_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (int.TryParse(inputProductColor.SelectedValue, out int selectedColorId))
			{
				productColorId = selectedColorId;
			}
		}

		protected void AddColor_Click(object sender, EventArgs e)
		{
			inputProductColor.Enabled = true;

			inputProductModel.Enabled = false;
			inputProductBrand.Enabled = false;
			inputPhoneType.Enabled = false;
			inputPhoneBrand.Enabled = false;
		}

		protected void AddProductModel_Click(object sender, EventArgs e)
		{
			inputProductModel.Enabled = true;
			inputProductBrand.Enabled = true;

			inputProductColor.Enabled = false;
			inputPhoneType.Enabled = false;
			inputPhoneBrand.Enabled = false;
		}

		protected void AddPhoneType_Click(object sender, EventArgs e)
		{
			inputPhoneType.Enabled = true;
			inputPhoneBrand.Enabled = true;

			inputProductColor.Enabled = false;
			inputProductModel.Enabled = false;
			inputProductBrand.Enabled = false;
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

		protected void Add_Click(object sender, EventArgs e)
		{
			try
			{
				string stock = inputStock.Value;
				string buyPrice = inputBuyPrice.Value;
				string sellPrice = inputSellPrice.Value;
				string description = inputDescription.Value;
				string subCategoryId = inputSubCategory.SelectedValue;
				string productModelId = inputProductModel.SelectedValue;
				string phoneTypeId = inputPhoneType.SelectedValue;
				string colorId = inputProductColor.SelectedValue;

				// Mengambil nama-nama untuk pembuatan kode produk
				string subCategoryName = inputSubCategory.SelectedItem.Text;
				string productBrandName = inputProductBrand.SelectedItem.Text;
				string productModelName = inputProductModel.SelectedItem.Text;
				string phoneBrandName = inputPhoneBrand.SelectedItem.Text;
				string phoneTypeName = inputPhoneType.SelectedItem.Text;
				string colorName = inputProductColor.SelectedItem.Text;

				// Mendapatkan nomor urut unik berdasarkan kombinasi produk
				int nextNumber = GetNextProductNumber();

				// Membuat kode produk berdasarkan nama dan nomor urut
				string productId = GenerateUniqueProductCode(subCategoryName, productBrandName, productModelName, phoneBrandName, phoneTypeName, colorName, nextNumber);

				// Membaca file gambar
				byte[] imageBytes = null;
				if (inputImage.HasFile)
				{
					string fileType = inputImage.PostedFile.ContentType;
					if (fileType == "image/png" || fileType == "image/jpeg" || fileType == "image/jpg")
					{
						using (BinaryReader br = new BinaryReader(inputImage.PostedFile.InputStream))
						{
							imageBytes = br.ReadBytes(inputImage.PostedFile.ContentLength);
						}
					}
					else
					{
						lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> Silahkan unggah file gambar dalam format png/jpg/jpeg.";
						lblErrorMessage.CssClass = "text-danger";
						lblErrorMessage.Visible = true;
						return;
					}
				}
				else
				{
					lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> Silahkan unggah file gambar.";
					lblErrorMessage.CssClass = "text-danger";
					lblErrorMessage.Visible = true;
					return;
				}

				string query = @"
								IF NOT EXISTS (
									SELECT 1 
									FROM Product 
									WHERE subCategoryId = @SubCategoryId 
									  AND productModelId = @ProductModelId 
									  AND phoneTypeId = @PhoneTypeId 
									  AND colorId = @ColorId
								)
								BEGIN
									INSERT INTO Product (
										productId, subCategoryId, productModelId, phoneTypeId, colorId, stock, buyPrice, sellPrice, description, image, createdTime, updatedTime
									)
									VALUES (
										@ProductId, @SubCategoryId, @ProductModelId, @PhoneTypeId, @ColorId, @Stock, @BuyPrice, @SellPrice, @Description, @Image, GETDATE(), GETDATE()
									)
								END
								ELSE
								BEGIN
									THROW 50000, 'Produk dengan variasi tersebut sudah terdaftar, silahkan periksa variasi produk kembali.', 1;
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
					new SqlParameter("@Image", imageBytes)
				};

				Unipro_Store.Models.Function.CommonFunction dbCon = new Unipro_Store.Models.Function.CommonFunction();
				dbCon.Query(query, parameters);

				Response.Redirect("Product.aspx");
			}
			catch (SqlException ex)
			{
				lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i>" + ex.Message;
				lblErrorMessage.CssClass = "text-danger";
				lblErrorMessage.Visible = true;
			}
			catch (Exception ex)
			{
				lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i>" + ex.Message;
				lblErrorMessage.CssClass = "text-danger";
				lblErrorMessage.Visible = true;
			}
		}
	}
}