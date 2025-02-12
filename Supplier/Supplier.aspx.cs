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

namespace Unipro_Store.Supplier
{
	public partial class Supplier : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				GetTableData();
				LoadCity();
			}
		}

		private void LoadCity()
		{
			DataTable dtCity = dbCon.Fetch(@"SELECT DISTINCT RTRIM(LTRIM(SUBSTRING(address, CHARINDEX(',', address) + 1, LEN(address)))) AS city 
									         FROM Supplier");

			filterCity.Items.Clear();
			filterCity.Items.Add(new ListItem("All", ""));

			foreach (DataRow row in dtCity.Rows)
			{
				filterCity.Items.Add(new ListItem(row["city"].ToString(), row["city"].ToString()));
			}
		}

		public void GetTableData()
		{
			DataTable dt = new DataTable();
			dt = dbCon.Fetch("SELECT ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS [No], supplierId AS [ID], supplierName AS [Suppliername], address AS [Address], contactNumber AS [Contactnumber], isActive AS [Status] FROM Supplier");

			StringBuilder sb = new StringBuilder();
			foreach (DataRow row in dt.Rows)
			{
				sb.Append("<tr>");
				sb.Append("<td>" + row["No"] + "</td>");
				sb.Append("<td>" + row["Suppliername"] + "</td>");
				sb.Append("<td>" + row["Address"] + "</td>");
				sb.Append("<td>" + row["ContactNumber"] + "</td>");
				bool isActive = Convert.ToBoolean(row["Status"]);
				sb.Append("<td>" + (isActive ? "Aktif" : "Inaktif") + "</td>");
				sb.Append("<td>");
				sb.Append("<a href='UpdateSupplier.aspx?SupplierID=" + row["ID"] + "' class='btn btn-sm' style='background-color: #386bba; color: white;'><i class='fas fa-edit'></i></a> ");
				sb.Append("<a href='javascript:void(0);' onclick='showDeleteModal(" + row["ID"] + ");' class='btn btn-sm' style='background-color: #ff4c4c; color: white;'><i class='fas fa-trash-alt'></i></a>");
				sb.Append("</td>");
				sb.Append("</tr>");
			}

			ltTableRows.Text = sb.ToString();
		}

		protected void create_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Supplier/CreateSupplier.aspx");
		}

		protected void delete_Click(object sender, EventArgs e)
		{
			int supplierId = Convert.ToInt32(hfSupplierIDToDelete.Value);

			// Query untuk mengecek apakah ada PurchaseTransaction terkait dengan supplier ini
			string checkPurchaseTransactionQuery = "SELECT 1 FROM PurchaseTransaction WHERE SupplierID = @SupplierID";

			SqlParameter[] parametersForCheck = new SqlParameter[]
			{
				new SqlParameter("@SupplierID", supplierId)
			};

			try
			{
				DataTable dt = dbCon.Fetch(checkPurchaseTransactionQuery, parametersForCheck);

				if (dt.Rows.Count > 0)
				{
					// Menampilkan warning toast jika ada transaksi pembelian terkait
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showWarningToast",
						"launchWarningToast('Supplier tidak dapat dihapus karena terkait dengan transaksi pembelian');", true);
				}
				else
				{
					// Jika tidak ada transaksi pembelian, hapus supplier
					string deleteUserQuery = "DELETE FROM Supplier WHERE SupplierID = @SupplierID";
					SqlParameter[] parametersForDelete = new SqlParameter[]
					{
						new SqlParameter("@SupplierID", supplierId)
					};

					dbCon.Query(deleteUserQuery, parametersForDelete);

					GetTableData();

					// Menampilkan success toast
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccessToast", "launchSuccessToast('Supplier berhasil dihapus');", true);
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