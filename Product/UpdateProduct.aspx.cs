using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unipro_Store.Category;
using Unipro_Store.Phone_Brand;
using Unipro_Store.Phone_Type;
using Unipro_Store.Product_Brand;
using Unipro_Store.Product_Color;
using Unipro_Store.Subcategory;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Product
{
	public partial class UpdateProduct : System.Web.UI.Page
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
			DataTable dtProductColors = dbCon.Fetch("SELECT colorId, colorName FROM ProductColor");

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
				inputStock.Value = row["stock"].ToString();
				inputBuyPrice.Value = Convert.ToDecimal(row["buyPrice"]).ToString("0");
				inputSellPrice.Value = Convert.ToDecimal(row["sellPrice"]).ToString("0");
				inputDescription.Value = row["description"].ToString();
			}
		}

		protected void Update_Click(object sender, EventArgs e)
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

						hfImageFileName.Value = inputImage.FileName;
					}
					else
					{
						lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> Silahkan unggah file gambar dalam format png/jpg/jpeg.";
						lblErrorMessage.CssClass = "text-danger";
						lblErrorMessage.Visible = true;
						return;
					}
				}

				string checkQuery = @"
									SELECT 1 
									FROM Product 
									WHERE subCategoryId = @SubCategoryId 
									  AND productModelId = @ProductModelId 
									  AND phoneTypeId = @PhoneTypeId 
									  AND colorId = @ColorId 
									  AND productId <> @ProductId"; 

				SqlParameter[] checkParams = new SqlParameter[]
				{
					new SqlParameter("@SubCategoryId", subCategoryId),
					new SqlParameter("@ProductModelId", productModelId),
					new SqlParameter("@PhoneTypeId", phoneTypeId),
					new SqlParameter("@ColorId", colorId),
					new SqlParameter("@ProductId", productId)
				};

				CommonFunction dbCon = new CommonFunction();
				DataTable dt = dbCon.Fetch(checkQuery, checkParams);

				if (dt.Rows.Count > 0)
				{
					lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> Kombinasi produk tersebut sudah ada. Silahkan periksa kembali variasi produk.";
					lblErrorMessage.CssClass = "text-danger";
					lblErrorMessage.Visible = true;
					return;
				}

				string query = @"
								UPDATE Product
								SET subCategoryId = @SubCategoryId,
									productModelId = @ProductModelId,
									phoneTypeId = @PhoneTypeId,
									colorId = @ColorId,
									stock = @Stock,
									buyPrice = @BuyPrice,
									sellPrice = @SellPrice,
									description = @Description,
									updatedTime = GETDATE()" +
											(imageBytes != null ? ", image = @Image" : "") + @"
								WHERE productId = @ProductId";

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
					new SqlParameter("@Description", description)
				};

				if (imageBytes != null)
				{
					var imageParam = new SqlParameter("@Image", SqlDbType.VarBinary) { Value = imageBytes };
					parameters = parameters.Concat(new[] { imageParam }).ToArray();
				}

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
	}
}