using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Product_Brand
{
	public partial class UpdateProductBrand : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadProductBrandData();
			}
		}
		protected void LoadProductBrandData()
		{
			int productBrandId = Convert.ToInt32(Request.QueryString["ProductBrandID"]);

			CommonFunction dbCon = new CommonFunction();
			string query = "SELECT * FROM ProductBrand WHERE productBrandId = @ProductBrandID";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@ProductBrandID", productBrandId)
			};

			var dt = dbCon.Fetch(query, parameters);

			if (dt.Rows.Count > 0)
			{
				DataRow row = dt.Rows[0];
				inputProductBrandname.Value = row["productBrandName"].ToString();
			}
		}
		protected void Update_Click(object sender, EventArgs e)
		{
			int productBrandId = Convert.ToInt32(Request.QueryString["ProductBrandID"]);
			string productBrandName = inputProductBrandname.Value;

			string checkQuery = "SELECT * FROM ProductBrand WHERE productBrandName = @ProductBrandname AND productBrandId != @ProductBrandID";

			SqlParameter[] checkParameters = new SqlParameter[]
			{
				new SqlParameter("@ProductBrandname", productBrandName),
				new SqlParameter("@ProductBrandID", productBrandId)
			};

			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(checkQuery, checkParameters);

			if (dt.Rows.Count > 0)
			{
				lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> Nama merek produk tersebut sudah terdaftar, silahkan gunakan nama yang lain.";
				lblErrorMessage.Visible = true;

				inputProductBrandname.Attributes.Add("class", "form-control input-error");
			}
			else
			{
				string updateQuery = "UPDATE ProductBrand SET productBrandName = @Productbrandname, updatedTime = GETDATE() WHERE ProductBrandID = @ProductBrandID";

				SqlParameter[] parameters = new SqlParameter[]
				{
					new SqlParameter("@Productbrandname", productBrandName),
					new SqlParameter("@ProductBrandID", productBrandId)
				};

				try
				{
					dbCon.Query(updateQuery, parameters);
					Response.Redirect("ProductBrand.aspx");
				}
				catch (Exception ex)
				{
					lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> " + ex.Message;
					lblErrorMessage.Visible = true;

					inputProductBrandname.Attributes.Add("class", "form-control input-error");
				}
			}
		}
	}
}