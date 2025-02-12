using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Subcategory
{
	public partial class UpdateSubcategory : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadSubCategoryData();
			}
		}
		protected void LoadCategories(string selectedCategoryName)
		{
			CommonFunction dbCon = new CommonFunction();
			string query = "SELECT categoryId, categoryName FROM Category";

			DataTable dt = dbCon.Fetch(query);
			inputCategory.Items.Clear();

			foreach (DataRow row in dt.Rows)
			{
				ListItem item = new ListItem(row["categoryName"].ToString(), row["categoryId"].ToString());
				inputCategory.Items.Add(item);
			}

			if (!string.IsNullOrEmpty(selectedCategoryName))
			{
				inputCategory.Value = inputCategory.Items.FindByText(selectedCategoryName)?.Value;
			}
		}
		protected void LoadSubCategoryData()
		{
			int subCategoryId = Convert.ToInt32(Request.QueryString["SubCategoryID"]);

			CommonFunction dbCon = new CommonFunction();
			string query = @"
							 SELECT sc.subCategoryName, sc.description, c.categoryName 
							 FROM SubCategory sc 
							 JOIN Category c ON sc.categoryId = c.categoryId 
							 WHERE sc.subCategoryId = @SubCategoryID";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@SubCategoryID", subCategoryId)
			};

			var dt = dbCon.Fetch(query, parameters);

			if (dt.Rows.Count > 0)
			{
				DataRow row = dt.Rows[0];

				inputSubcategoryname.Value = row["subCategoryName"].ToString();
				inputDescription.Value = row["description"].ToString();
				string selectedCategoryName = row["categoryName"].ToString();
				LoadCategories(selectedCategoryName);
			}
		}
		protected void Update_Click(object sender, EventArgs e)
		{
			int subCategoryId = Convert.ToInt32(Request.QueryString["SubCategoryID"]);
			string subCategoryName = inputSubcategoryname.Value;
			string description = inputDescription.Value;
			int selectedCategoryId = Convert.ToInt32(inputCategory.Value);

			string checkQuery = @"
								SELECT * FROM SubCategory 
								WHERE subCategoryName = @SubCategoryName 
								AND categoryId = @CategoryID 
								AND subCategoryId != @SubCategoryID";

			SqlParameter[] checkParameters = new SqlParameter[]
			{
				new SqlParameter("@SubCategoryName", subCategoryName),
				new SqlParameter("@CategoryID", selectedCategoryId),
				new SqlParameter("@SubCategoryID", subCategoryId)
			};

			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(checkQuery, checkParameters);

			if (dt.Rows.Count > 0)
			{
				lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> Nama subkategori tersebut sudah terdaftar dalam kategori ini, silahkan gunakan nama yang lain.";
				lblErrorMessage.Visible = true;

				inputSubcategoryname.Attributes.Add("class", "form-control input-error");
			}
			else
			{
				string updateQuery = "UPDATE SubCategory SET subCategoryName = @SubCategoryName, description = @Description, categoryId = @CategoryID, updatedTime = GETDATE() " +
									 "WHERE subCategoryId = @SubCategoryID";

				SqlParameter[] parameters = new SqlParameter[]
				{
					new SqlParameter("@SubCategoryName", subCategoryName),
					new SqlParameter("@Description", description),
					new SqlParameter("@CategoryID", selectedCategoryId), 
					new SqlParameter("@SubCategoryID", subCategoryId)
				};

				try
				{
					dbCon.Query(updateQuery, parameters);
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
}