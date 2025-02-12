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

namespace Unipro_Store.Product_Brand
{
	public partial class ProductBrand : System.Web.UI.Page
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
			dt = dbCon.Fetch("SELECT ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS [No], productBrandId AS [ID], productBrandName AS [Productbrandname] FROM ProductBrand");

			StringBuilder sb = new StringBuilder();
			foreach (DataRow row in dt.Rows)
			{
				sb.Append("<tr>");
				sb.Append("<td>" + row["No"] + "</td>");
				sb.Append("<td>" + row["Productbrandname"] + "</td>");
				sb.Append("<td>");
				sb.Append("<a href='UpdateProductBrand.aspx?ProductBrandID=" + row["ID"] + "' class='btn btn-sm' style='background-color: #386bba; color: white;'><i class='fas fa-edit'></i></a> ");
				sb.Append("<a href='javascript:void(0);' onclick='showDeleteModal(" + row["ID"] + ");' class='btn btn-sm' style='background-color: #ff4c4c; color: white;'><i class='fas fa-trash-alt'></i></a>");
				sb.Append("</td>");
				sb.Append("</tr>");
			}

			ltTableRows.Text = sb.ToString();
		}

		protected void create_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Product Specifications/Product Brand/CreateProductBrand.aspx");
		}

		protected void delete_Click(object sender, EventArgs e)
		{
			int productBrandId = Convert.ToInt32(hfProductBrandIDToDelete.Value);
			// Query untuk mengecek apakah ada model produk terkait
			string checkProductModelQuery = "SELECT 1 FROM ProductModel WHERE ProductBrandID = @ProductBrandID";

			SqlParameter[] parametersForCheck = new SqlParameter[]
			{
				new SqlParameter("@ProductBrandID", productBrandId)
			};

			try
			{
				DataTable dt = dbCon.Fetch(checkProductModelQuery, parametersForCheck);

				if (dt.Rows.Count > 0)
				{
					// Menampilkan warning toast jika ada model produk terkait
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showWarningToast", "launchWarningToast('Silahkan hapus seluruh data model produk terkait terlebih dahulu!');", true);
				}
				else
				{
					SqlParameter[] parametersForDelete = new SqlParameter[]
					{
						new SqlParameter("@ProductBrandID", productBrandId)
					};

					string deleteProductBrandQuery = "DELETE FROM ProductBrand WHERE ProductBrandID = @ProductBrandID";
					dbCon.Query(deleteProductBrandQuery, parametersForDelete);

					GetTableData();
					// Menampilkan success toast
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccessToast", "launchSuccessToast('Merek produk berhasil dihapus');", true);
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