using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Product_Brand
{
	public partial class CreateProductBrand : System.Web.UI.Page
	{
		protected void Add_Click(object sender, EventArgs e)
		{
			string brandname = inputProductBrandname.Value;

			string query = @"
					IF NOT EXISTS (SELECT 1 FROM ProductBrand WHERE productBrandName = @ProductBrandName)
					BEGIN
						INSERT INTO ProductBrand (productBrandName, createdTime, updatedTime)
						VALUES (@ProductBrandName, GETDATE(), GETDATE())
					END
					ELSE
					BEGIN
						THROW 50000, 'Nama merek produk tersebut sudah terdaftar, silahkan gunakan nama yang lain.', 1;
					END";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@ProductBrandName", brandname),
			};

			try
			{
				CommonFunction commonFunction = new CommonFunction();
				commonFunction.Query(query, parameters);
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