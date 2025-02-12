using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Subcategory
{
	public partial class CreateSubcategory : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadCategories();
			}
		}
		private void LoadCategories()
		{
			CommonFunction dbCon = new CommonFunction();
			DataTable dtCategories = dbCon.Fetch("SELECT categoryId, categoryName FROM Category");

			foreach (DataRow row in dtCategories.Rows)
			{
				inputCategory.Items.Add(new ListItem(row["categoryName"].ToString(), row["categoryId"].ToString()));
			}
		}
		protected void Add_Click(object sender, EventArgs e)
		{
			string subcategoryname = inputSubcategoryname.Value;
			string description = inputDescription.Value;
			string categoryId = inputCategory.Value;  

			string query = @"
						   IF NOT EXISTS (SELECT 1 FROM Subcategory WHERE subCategoryName = @SubcategoryName AND categoryId = @CategoryId)
						   BEGIN
							   INSERT INTO Subcategory (categoryId, subCategoryName, description, createdTime, updatedTime)
							   VALUES (@CategoryId, @SubcategoryName, @Description, GETDATE(), GETDATE())
						   END
						   ELSE
						   BEGIN
							   THROW 50000, 'Nama subkategori tersebut sudah terdaftar dalam kategori ini, silahkan gunakan nama yang lain.', 1;
						   END";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@CategoryId", categoryId),
				new SqlParameter("@SubcategoryName", subcategoryname),
				new SqlParameter("@Description", description),
			};

			try
			{
				CommonFunction commonFunction = new CommonFunction();
				commonFunction.Query(query, parameters);
				Response.Redirect("Subcategory.aspx");
			}
			catch (Exception ex)
			{
				lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> " + ex.Message;
				lblErrorMessage.Visible = true;

				inputSubcategoryname.Attributes.Add("class", "form-control input-error");
			}
		}
	}
}