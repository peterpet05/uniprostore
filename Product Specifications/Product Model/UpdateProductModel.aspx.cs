using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Product_Model
{
	public partial class UpdateProductModel : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadProductModelData();
			}
		}
		protected void LoadProductBrands(string selectedProductBrandName)
		{
			CommonFunction dbCon = new CommonFunction();
			string query = "SELECT productBrandId, productBrandName FROM ProductBrand";

			DataTable dt = dbCon.Fetch(query);
			inputProductBrand.Items.Clear();

			foreach (DataRow row in dt.Rows)
			{
				ListItem item = new ListItem(row["productBrandName"].ToString(), row["productBrandId"].ToString());
				inputProductBrand.Items.Add(item);
			}

			if (!string.IsNullOrEmpty(selectedProductBrandName))
			{
				inputProductBrand.Value = inputProductBrand.Items.FindByText(selectedProductBrandName)?.Value;
			}
		}
		protected void LoadProductModelData()
		{
			int productModelId = Convert.ToInt32(Request.QueryString["ProductModelID"]);

			CommonFunction dbCon = new CommonFunction();
			string query = @"
							 SELECT m.productModelName, b.productBrandName 
							 FROM ProductModel m 
							 JOIN ProductBrand b ON m.productBrandId = b.productBrandId 
							 WHERE m.productModelId = @ProductModelID";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@ProductModelID", productModelId)
			};

			var dt = dbCon.Fetch(query, parameters);

			if (dt.Rows.Count > 0)
			{
				DataRow row = dt.Rows[0];

				inputProductModelname.Value = row["productModelName"].ToString();
				string selectedProductBrandName = row["productBrandName"].ToString();
				LoadProductBrands(selectedProductBrandName);
			}
		}
		protected void Update_Click(object sender, EventArgs e)
		{
			int productModelId = Convert.ToInt32(Request.QueryString["ProductModelID"]);
			string productModelName = inputProductModelname.Value;
			int selectedProductBrandId = Convert.ToInt32(inputProductBrand.Value);

			string checkQuery = @"
								SELECT * FROM ProductModel 
								WHERE productModelName = @ProductModelName 
								AND productBrandId = @ProductBrandID 
								AND productModelId != @ProductModelID";

			SqlParameter[] checkParameters = new SqlParameter[]
			{
				new SqlParameter("@ProductModelName", productModelName),
				new SqlParameter("@ProductBrandID", selectedProductBrandId),
				new SqlParameter("@ProductModelID", productModelId)
			};

			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(checkQuery, checkParameters);

			if (dt.Rows.Count > 0)
			{
				lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> Nama model produk tersebut sudah terdaftar dalam merek ini, silahkan gunakan nama yang lain.";
				lblErrorMessage.Visible = true;

				inputProductModelname.Attributes.Add("class", "form-control input-error");
			}
			else
			{
				string updateQuery = "UPDATE ProductModel SET productModelName = @ProductModelName, productBrandId = @ProductBrandID, updatedTime = GETDATE() " +
									 "WHERE productModelId = @ProductModelID";

				SqlParameter[] parameters = new SqlParameter[]
				{
					new SqlParameter("@ProductModelName", productModelName),
					new SqlParameter("@ProductBrandID", selectedProductBrandId),
					new SqlParameter("@ProductModelID", productModelId)
				};

				try
				{
					dbCon.Query(updateQuery, parameters);
					Response.Redirect("ProductModel.aspx");
				}
				catch (Exception ex)
				{
					lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> " + ex.Message;
					lblErrorMessage.Visible = true;

					inputProductModelname.Attributes.Add("class", "form-control input-error");
				}
			}
		}
	}
}