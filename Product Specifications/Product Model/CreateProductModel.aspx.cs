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
	public partial class CreateProductModel : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadProductBrands();
			}
		}
		private void LoadProductBrands()
		{
			CommonFunction dbCon = new CommonFunction();
			DataTable dtProductBrands = dbCon.Fetch("SELECT productBrandId, productBrandName FROM ProductBrand");

			foreach (DataRow row in dtProductBrands.Rows)
			{
				inputProductBrand.Items.Add(new ListItem(row["productBrandName"].ToString(), row["productBrandId"].ToString()));
			}
		}
		protected void Add_Click(object sender, EventArgs e)
		{
			string productmodelname = inputProductModelname.Value;
			string productBrandId = inputProductBrand.Value;

			string query = @"
						   IF NOT EXISTS (SELECT 1 FROM ProductModel WHERE productModelName = @ProductModelName AND productBrandId = @ProductBrandId)
						   BEGIN
							   INSERT INTO ProductModel (productBrandId, productModelName, createdTime, updatedTime)
							   VALUES (@ProductBrandId, @ProductModelName, GETDATE(), GETDATE())
						   END
						   ELSE
						   BEGIN
							   THROW 50000, 'Nama model produk tersebut sudah terdaftar dalam merek ini, silahkan gunakan nama yang lain.', 1;
						   END";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@ProductBrandId", productBrandId),
				new SqlParameter("@ProductModelName", productmodelname),
			};

			try
			{
				CommonFunction commonFunction = new CommonFunction();
				commonFunction.Query(query, parameters);
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