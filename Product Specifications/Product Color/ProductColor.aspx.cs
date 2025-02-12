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

namespace Unipro_Store.Product_Color
{
	public partial class ProductColor : System.Web.UI.Page
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
			dt = dbCon.Fetch("SELECT ROW_NUMBER() OVER(ORDER BY colorName ASC) AS [No], colorId AS [ID], colorName AS [Productcolorname] FROM ProductColor");

			StringBuilder sb = new StringBuilder();
			foreach (DataRow row in dt.Rows)
			{
				sb.Append("<tr>");
				sb.Append("<td>" + row["No"] + "</td>");
				sb.Append("<td>" + row["Productcolorname"] + "</td>");
				sb.Append("<td>");
				sb.Append("<a href='UpdateProductColor.aspx?ColorID=" + row["ID"] + "' class='btn btn-sm' style='background-color: #386bba; color: white;'><i class='fas fa-edit'></i></a> ");
				sb.Append("<a href='javascript:void(0);' onclick='showDeleteModal(" + row["ID"] + ");' class='btn btn-sm' style='background-color: #ff4c4c; color: white;'><i class='fas fa-trash-alt'></i></a>");
				sb.Append("</td>");
				sb.Append("</tr>");
			}

			ltTableRows.Text = sb.ToString();
		}

		protected void create_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Product Specifications/Product Color/CreateProductColor.aspx");
		}

		protected void delete_Click(object sender, EventArgs e)
		{
			int colorId = Convert.ToInt32(hfProductColorIDToDelete.Value);

			// Query untuk mengecek apakah ada produk terkait dengan warna ini
			string checkProductQuery = "SELECT 1 FROM Product WHERE ColorID = @ColorID";

			SqlParameter[] parametersForCheck = new SqlParameter[]
			{
				new SqlParameter("@ColorID", colorId)
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
					// Jika tidak ada produk terkait, lanjutkan penghapusan warna produk
					string deleteColorQuery = "DELETE FROM ProductColor WHERE ColorID = @ColorID";
					SqlParameter[] parametersForDelete = new SqlParameter[]
					{
						new SqlParameter("@ColorID", colorId)
					};

					dbCon.Query(deleteColorQuery, parametersForDelete);

					GetTableData();
					// Menampilkan success toast
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccessToast", "launchSuccessToast('Warna produk berhasil dihapus');", true);
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