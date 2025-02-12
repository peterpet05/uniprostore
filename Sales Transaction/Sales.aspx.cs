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
using System.Web.Security;

namespace Unipro_Store.Sales_Transaction
{
	public partial class Sales : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				GetTableData();
				LoadMedias();
				LoadUsers();
			}
		}

		private void LoadMedias()
		{
			DataTable dtMedias = dbCon.Fetch("SELECT mediaId, mediaName FROM Media");

			filterMedia.Items.Clear();
			filterMedia.Items.Add(new ListItem("All", ""));

			foreach (DataRow row in dtMedias.Rows)
			{
				filterMedia.Items.Add(new ListItem(row["mediaName"].ToString(), row["mediaName"].ToString()));
			}
		}

		private void LoadUsers()
		{
			string query = @"
						   SELECT DISTINCT u.fullName 
						   FROM Users u
						   JOIN SalesTransaction s ON u.userId = s.userId";

			DataTable dtUsers = dbCon.Fetch(query);

			filterUser.Items.Clear();
			filterUser.Items.Add(new ListItem("All", ""));

			foreach (DataRow row in dtUsers.Rows)
			{
				filterUser.Items.Add(new ListItem(row["fullName"].ToString(), row["fullName"].ToString()));
			}
		}

		public void GetTableData()
		{
			string role = Session["Role"]?.ToString();

			string media = Request.QueryString["media"];
			string month = Request.QueryString["month"];

			string query = @"
                SELECT ROW_NUMBER() OVER(ORDER BY s.createdTime DESC) AS [No], 
                       s.salesId AS [ID], 
                       s.salesDate AS [Date], 
                       m.mediaName AS [Media], 
                       s.totalItem AS [TotalProduk], 
                       s.totalPrice AS [TotalHarga], 
                       s.transactionDiscount AS [Diskon], 
                       s.discountedPrice AS [TotalDiskon], 
                       u.fullname AS [Pencatat] 
                FROM SalesTransaction s 
                JOIN Users u ON s.userId = u.userId
                JOIN Media m ON s.mediaId = m.mediaId
                WHERE 1 = 1";

			List<SqlParameter> parameters = new List<SqlParameter>();

			if (!string.IsNullOrEmpty(media))
			{
				query += " AND s.mediaId = @media";
				parameters.Add(new SqlParameter("@media", media));
			}

			if (!string.IsNullOrEmpty(month))
			{
				query += " AND FORMAT(s.salesDate, 'yyyy-MM') = @month";
				parameters.Add(new SqlParameter("@month", month));
			}

			DataTable dt = dbCon.Fetch(query, parameters.ToArray());

			StringBuilder sb = new StringBuilder();
			foreach (DataRow row in dt.Rows)
			{
				sb.Append("<tr>");
				sb.Append("<td>" + row["No"] + "</td>");
				sb.Append("<td>" + row["ID"] + "</td>");
				DateTime salesDate = Convert.ToDateTime(row["Date"]);
				sb.Append("<td>" + salesDate.ToString("dd/MM/yyyy") + "</td>");
				sb.Append("<td>" + row["Media"] + "</td>");
				sb.Append("<td>" + row["TotalProduk"] + "</td>");
				sb.Append("<td>" + String.Format("Rp {0:N0}", row["TotalHarga"]) + "</td>");
				sb.Append("<td>" + String.Format("Rp {0:N0}", row["Diskon"]) + "</td>");
				sb.Append("<td>" + String.Format("Rp {0:N0}", row["TotalDiskon"]) + "</td>");
				sb.Append("<td>" + row["Pencatat"] + "</td>");
				sb.Append("<td>");
				sb.Append("<a href='ViewSales.aspx?SalesID=" + row["ID"] + "' class='btn btn-sm' style='background-color: #579c48; color: white;'><i class='fas fa-eye'></i></a> ");
				if (role == "Owner")
				{
					sb.Append("<a href='UpdateSales.aspx?SalesID=" + row["ID"] + "' class='btn btn-sm' style='background-color: #386bba; color: white;'><i class='fas fa-edit'></i></a> ");
					sb.Append("<a href='javascript:void(0);' onclick='showDeleteModal(\"" + row["ID"] + "\");' class='btn btn-sm' style='background-color: #ff4c4c; color: white;'><i class='fas fa-trash-alt'></i></a>");
				}
				sb.Append("</td>");
				sb.Append("</tr>");
			}

			ltTableRows.Text = sb.ToString();
		}

		protected void create_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Sales Transaction/CreateSales.aspx");
		}

		protected void delete_Click(object sender, EventArgs e)
		{
			string salesId = hfSalesTransactionIDToDelete.Value;

			// Ambil daftar produk dari transaksi sebelum dihapus
			string queryGetProducts = @"
				SELECT productId, quantity 
				FROM SalesTransactionDetail 
				WHERE SalesID = @SalesID";

			SqlParameter[] parametersGetProducts = new SqlParameter[]
			{
				new SqlParameter("@SalesID", salesId)
			};

			DataTable dtProducts = dbCon.Fetch(queryGetProducts, parametersGetProducts);

			foreach (DataRow row in dtProducts.Rows)
			{
				string productId = row["productId"].ToString();
				int quantity = Convert.ToInt32(row["quantity"]);

				// Kembalikan stok produk
				string queryUpdateStock = @"
					UPDATE Product 
					SET stock = stock + @quantity 
					WHERE productId = @productId";

				SqlParameter[] parametersUpdateStock = new SqlParameter[]
				{
					new SqlParameter("@quantity", quantity),
					new SqlParameter("@productId", productId)
				};

				dbCon.Fetch(queryUpdateStock, parametersUpdateStock);
			}

			string queryDeleteTransaction = "DELETE FROM SalesTransaction WHERE SalesID = @SalesID";

			SqlParameter[] parametersDeleteTransaction = new SqlParameter[]
			{
				new SqlParameter("@SalesID", salesId)
			};

			dbCon.Query(queryDeleteTransaction, parametersDeleteTransaction);

			GetTableData();

			// Menampilkan success toast
			ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccessToast", "launchSuccessToast('Transaksi berhasil dihapus');", true);
			// Menyembunyikan modal setelah penghapusan
			ScriptManager.RegisterStartupScript(this, this.GetType(), "hideModal", "$('#deleteModal').modal('hide');", true);
		}
	}
}