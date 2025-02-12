using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Category
{
	public partial class CreateCategory : System.Web.UI.Page
	{
		protected void Add_Click(object sender, EventArgs e)
		{
			string categoryname = inputCategoryname.Value;
			string description = inputDescription.Value;

			string query = @"
					IF NOT EXISTS (SELECT 1 FROM Category WHERE categoryname = @CategoryName)
					BEGIN
						INSERT INTO Category (categoryName, description, createdTime, updatedTime)
						VALUES (@CategoryName, @Description, GETDATE(), GETDATE())
					END
					ELSE
					BEGIN
						THROW 50000, 'Nama kategori tersebut sudah terdaftar, silahkan gunakan nama yang lain.', 1;
					END";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@CategoryName", categoryname),
				new SqlParameter("@Description", description),
			};

			try
			{
				CommonFunction commonFunction = new CommonFunction();
				commonFunction.Query(query, parameters);
				Response.Redirect("Category.aspx");
			}
			catch (Exception ex)
			{
				lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> " + ex.Message;
				lblErrorMessage.Visible = true;

				inputCategoryname.Attributes.Add("class", "form-control input-error");
			}
		}
	}
}