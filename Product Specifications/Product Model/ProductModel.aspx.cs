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

namespace Unipro_Store.Product_Model
{
	public partial class ProductModel : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				GetTableData();
				LoadProductBrand();
			}
		}

		private void LoadProductBrand()
		{
			string query = @"
						   SELECT DISTINCT b.productBrandName 
						   FROM ProductBrand b
						   JOIN ProductModel m ON b.productBrandId = m.productBrandId";

			DataTable dtProductBrands = dbCon.Fetch(query);

			filterProductBrand.Items.Clear();
			filterProductBrand.Items.Add(new ListItem("All", ""));

			foreach (DataRow row in dtProductBrands.Rows)
			{
				filterProductBrand.Items.Add(new ListItem(row["productBrandName"].ToString(), row["productBrandName"].ToString()));
			}
		}

		public void GetTableData()
		{
			DataTable dt = new DataTable();
			dt = dbCon.Fetch("SELECT ROW_NUMBER() OVER(ORDER BY m.productModelName ASC) AS [No], m.productModelId AS [ID], m.productModelName AS [ProductModelname], b.productBrandName AS [ProductBrandname] " +
							 "FROM ProductModel m JOIN ProductBrand b ON m.productBrandId = b.productBrandId");

			StringBuilder sb = new StringBuilder();
			foreach (DataRow row in dt.Rows)
			{
				sb.Append("<tr>");
				sb.Append("<td>" + row["No"] + "</td>");
				sb.Append("<td class='hidden-column'>" + row["ProductBrandname"] + "</td>");
				sb.Append("<td>" + row["productModelName"] + "</td>");
				sb.Append("<td>");
				sb.Append("<a href='UpdateProductModel.aspx?ProductModelID=" + row["ID"] + "' class='btn btn-sm' style='background-color: #386bba; color: white;'><i class='fas fa-edit'></i></a> ");
				sb.Append("<a href='javascript:void(0);' onclick='showDeleteModal(" + row["ID"] + ");' class='btn btn-sm' style='background-color: #ff4c4c; color: white;'><i class='fas fa-trash-alt'></i></a>");
				sb.Append("</td>");
				sb.Append("</tr>");
			}

			ltTableRows.Text = sb.ToString();
		}

		protected void create_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Product Specifications/Product Model/CreateProductModel.aspx");
		}

		protected void delete_Click(object sender, EventArgs e)
		{
			int productModelId = Convert.ToInt32(hfProductModelIDToDelete.Value);

			// Query untuk mengecek apakah ada produk terkait dengan model produk ini
			string checkProductQuery = "SELECT 1 FROM Product WHERE ProductModelID = @ProductModelID";

			SqlParameter[] parametersForCheck = new SqlParameter[]
			{
				new SqlParameter("@ProductModelID", productModelId)
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
					// Jika tidak ada produk terkait, lanjutkan penghapusan model produk
					string deleteProductModelQuery = "DELETE FROM ProductModel WHERE ProductModelID = @ProductModelID";
					SqlParameter[] parametersForDelete = new SqlParameter[]
					{
						new SqlParameter("@ProductModelID", productModelId)
					};

					dbCon.Query(deleteProductModelQuery, parametersForDelete);

					GetTableData();
					// Menampilkan success toast
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccessToast", "launchSuccessToast('Model produk berhasil dihapus');", true);
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