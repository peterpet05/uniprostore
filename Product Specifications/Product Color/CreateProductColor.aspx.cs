using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Product_Color
{
	public partial class CreateProductColor : System.Web.UI.Page
	{
		protected void Add_Click(object sender, EventArgs e)
		{
			string colorname = inputProductColorname.Value;

			string query = @"
					IF NOT EXISTS (SELECT 1 FROM ProductColor WHERE colorName = @ColorName)
					BEGIN
						INSERT INTO ProductColor (colorName, createdTime, updatedTime)
						VALUES (@ColorName, GETDATE(), GETDATE())
					END
					ELSE
					BEGIN
						THROW 50000, 'Nama warna produk tersebut sudah terdaftar, silahkan gunakan nama yang lain.', 1;
					END";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@ColorName", colorname),
			};

			try
			{
				CommonFunction commonFunction = new CommonFunction();
				commonFunction.Query(query, parameters);
				Response.Redirect("ProductColor.aspx");
			}
			catch (Exception ex)
			{
				lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> " + ex.Message;
				lblErrorMessage.Visible = true;

				inputProductColorname.Attributes.Add("class", "form-control input-error");
			}
		}
	}
}