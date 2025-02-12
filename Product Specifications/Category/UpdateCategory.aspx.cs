using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Category
{
	public partial class UpdateCategory : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadCategoryData();
			}
		}
		protected void LoadCategoryData()
		{
			int categoryId = Convert.ToInt32(Request.QueryString["CategoryID"]);

			CommonFunction dbCon = new CommonFunction();
			string query = "SELECT * FROM Category WHERE categoryId = @CategoryID";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@CategoryID", categoryId)
			};

			var dt = dbCon.Fetch(query, parameters);

			if (dt.Rows.Count > 0)
			{
				DataRow row = dt.Rows[0];
				inputCategoryname.Value = row["categoryName"].ToString();
				inputDescription.Value = row["description"].ToString();
			}
		}
		protected void Update_Click(object sender, EventArgs e)
		{
			int categoryId = Convert.ToInt32(Request.QueryString["CategoryID"]);
			string categoryName = inputCategoryname.Value;
			string description = inputDescription.Value;

			string checkQuery = "SELECT * FROM Category WHERE categoryName = @Categoryname AND categoryId != @CategoryID";

			SqlParameter[] checkParameters = new SqlParameter[]
			{
				new SqlParameter("@Categoryname", categoryName),
				new SqlParameter("@CategoryID", categoryId)
			};

			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(checkQuery, checkParameters);

			if (dt.Rows.Count > 0)
			{
				lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> Nama kategori tersebut sudah terdaftar, silahkan gunakan nama yang lain.";
				lblErrorMessage.Visible = true;

				inputCategoryname.Attributes.Add("class", "form-control input-error");
			}
			else
			{
				string updateQuery = "UPDATE Category SET categoryName = @Categoryname, description = @Description, updatedTime = GETDATE() WHERE CategoryID = @CategoryID";

				SqlParameter[] parameters = new SqlParameter[]
				{
					new SqlParameter("@Categoryname", categoryName),
					new SqlParameter("@Description", description),
					new SqlParameter("@CategoryID", categoryId)
				};

				try
				{
					dbCon.Query(updateQuery, parameters);
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
}