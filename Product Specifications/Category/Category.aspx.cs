using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Category
{
	public partial class Category : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				GetTableData();
			}
		}

		public void GetTableData()
		{
			DataTable dt = new DataTable();
			dt = dbCon.Fetch("SELECT ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS [No], categoryId AS [ID], categoryName AS [Categoryname], description AS [Description] FROM Category");

			StringBuilder sb = new StringBuilder();
			foreach (DataRow row in dt.Rows)
			{
				sb.Append("<tr>");
				sb.Append("<td>" + row["No"] + "</td>");
				sb.Append("<td>" + row["Categoryname"] + "</td>");
				sb.Append("<td>" + row["Description"] + "</td>");
				sb.Append("<td>");
				sb.Append("<a href='UpdateCategory.aspx?CategoryID=" + row["ID"] + "' class='btn btn-sm' style='background-color: #386bba; color: white;'><i class='fas fa-edit'></i></a> ");
				sb.Append("<a href='javascript:void(0);' onclick='showDeleteModal(" + row["ID"] + ");' class='btn btn-sm' style='background-color: #ff4c4c; color: white;'><i class='fas fa-trash-alt'></i></a>");
				sb.Append("</td>");
				sb.Append("</tr>");
			}

			ltTableRows.Text = sb.ToString();
		}

		protected void create_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Product Specifications/Category/CreateCategory.aspx");
		}

		protected void delete_Click(object sender, EventArgs e)
		{
			int categoryId = Convert.ToInt32(hfCategoryIDToDelete.Value);
			// Query untuk mengecek apakah ada subkategori terkait
			string checkSubcategoryQuery = "SELECT 1 FROM Subcategory WHERE CategoryID = @CategoryID";

			SqlParameter[] parametersForCheck = new SqlParameter[]
			{
				new SqlParameter("@CategoryID", categoryId)
			};

			try
			{
				DataTable dt = dbCon.Fetch(checkSubcategoryQuery, parametersForCheck);

				if (dt.Rows.Count > 0)
				{
					// Menampilkan warning toast jika ada subkategori terkait
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showWarningToast", "launchWarningToast('Silahkan hapus seluruh data subkategori terkait terlebih dahulu!');", true);
				}
				else
				{
					SqlParameter[] parametersForDelete = new SqlParameter[]
					{
						new SqlParameter("@CategoryID", categoryId)
					};

					string deleteCategoryQuery = "DELETE FROM Category WHERE CategoryID = @CategoryID";
					dbCon.Query(deleteCategoryQuery, parametersForDelete);

					GetTableData();
					// Menampilkan success toast
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccessToast", "launchSuccessToast('Kategori berhasil dihapus');", true);
					// Menyembunyikan modal setelah penghapusan
					ScriptManager.RegisterStartupScript(this, this.GetType(), "hideModal", "$('#deleteModal').modal('hide');", true);
				}
			}
			catch (Exception ex)
			{
				Response.Write("Error: " + ex.Message);
			}
		}
	}
}