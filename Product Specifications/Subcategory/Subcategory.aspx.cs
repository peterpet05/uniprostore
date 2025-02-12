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

namespace Unipro_Store.Subcategory
{
	public partial class Subcategory : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				GetTableData();
				LoadCategory();
			}
		}

		private void LoadCategory()
		{
			string query = @"
						   SELECT DISTINCT c.categoryName 
						   FROM Category c
						   JOIN Subcategory s ON c.categoryId = s.categoryId";

			DataTable dtCategories = dbCon.Fetch(query);

			filterCategory.Items.Clear();
			filterCategory.Items.Add(new ListItem("All", ""));

			foreach (DataRow row in dtCategories.Rows)
			{
				filterCategory.Items.Add(new ListItem(row["categoryName"].ToString(), row["categoryName"].ToString()));
			}
		}

		public void GetTableData()
		{
			DataTable dt = new DataTable();
			dt = dbCon.Fetch("SELECT ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS [No], s.subCategoryId AS [ID], s.subCategoryName AS [Subcategoryname], s.description AS [Description], c.categoryName AS [Categoryname] " +
							 "FROM Subcategory s JOIN Category c ON s.categoryId = c.categoryId");

			StringBuilder sb = new StringBuilder();
			foreach (DataRow row in dt.Rows)
			{
				sb.Append("<tr>");
				sb.Append("<td>" + row["No"] + "</td>");
				sb.Append("<td class='hidden-column'>" + row["Categoryname"] + "</td>"); 
				sb.Append("<td>" + row["Subcategoryname"] + "</td>");
				sb.Append("<td>" + row["Description"] + "</td>");
				sb.Append("<td>");
				sb.Append("<a href='UpdateSubcategory.aspx?SubcategoryID=" + row["ID"] + "' class='btn btn-sm' style='background-color: #386bba; color: white;'><i class='fas fa-edit'></i></a> ");
				sb.Append("<a href='javascript:void(0);' onclick='showDeleteModal(" + row["ID"] + ");' class='btn btn-sm' style='background-color: #ff4c4c; color: white;'><i class='fas fa-trash-alt'></i></a>");
				sb.Append("</td>");
				sb.Append("</tr>");
			}

			ltTableRows.Text = sb.ToString();
		}

		protected void create_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Product Specifications/Subcategory/CreateSubcategory.aspx");
		}

		protected void delete_Click(object sender, EventArgs e)
		{
			int subCategoryId = Convert.ToInt32(hfSubcategoryIDToDelete.Value);

			// Query untuk mengecek apakah ada produk terkait dengan subkategori ini
			string checkProductQuery = "SELECT 1 FROM Product WHERE SubcategoryID = @SubcategoryID";

			SqlParameter[] parametersForCheck = new SqlParameter[]
			{
				new SqlParameter("@SubcategoryID", subCategoryId)
			};

			try
			{
				DataTable dt = dbCon.Fetch(checkProductQuery, parametersForCheck);

				if (dt.Rows.Count > 0)
				{
					// Menampilkan warning toast jika ada produk terkait
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showWarningToast", "launchWarningToast('Silahkan hapus seluruh data produk terkait terlebih dahulu!');", true);
				}
				else
				{
					// Jika tidak ada produk terkait, lanjutkan penghapusan subkategori
					string deleteSubcategoryQuery = "DELETE FROM Subcategory WHERE SubcategoryID = @SubcategoryID";
					SqlParameter[] parametersForDelete = new SqlParameter[]
					{
						new SqlParameter("@SubcategoryID", subCategoryId)
					};

					dbCon.Query(deleteSubcategoryQuery, parametersForDelete);

					GetTableData();
					// Menampilkan success toast
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccessToast", "launchSuccessToast('Subkategori berhasil dihapus');", true);
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