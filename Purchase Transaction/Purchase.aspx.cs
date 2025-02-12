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

namespace Unipro_Store.Purchase_Transaction
{
	public partial class Purchase : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				GetTableData();
				LoadSuppliers();
			}
		}

		private void LoadSuppliers()
		{
			string query = @"
						   SELECT DISTINCT s.supplierName 
						   FROM Supplier s
						   JOIN PurchaseTransaction p ON s.supplierId = p.supplierId";

			DataTable dtSupplier = dbCon.Fetch(query);

			filterSupplier.Items.Clear();
			filterSupplier.Items.Add(new ListItem("All", ""));

			foreach (DataRow row in dtSupplier.Rows)
			{
				filterSupplier.Items.Add(new ListItem(row["supplierName"].ToString(), row["supplierName"].ToString()));
			}
		}

		public void GetTableData()
		{
			string role = Session["Role"]?.ToString();

			DataTable dt = new DataTable();
			dt = dbCon.Fetch("SELECT ROW_NUMBER() OVER(ORDER BY p.createdTime DESC) AS [No], p.purchaseId AS [ID], p.purchaseDate AS [Date], p.totalItem AS [TotalProduk], p.totalPrice AS [TotalHarga], s.supplierName AS [Supplier] FROM PurchaseTransaction p JOIN Supplier s ON p.supplierId = s.supplierId");

			StringBuilder sb = new StringBuilder();
			foreach (DataRow row in dt.Rows)
			{
				sb.Append("<tr>");
				sb.Append("<td>" + row["No"] + "</td>");
				sb.Append("<td>" + row["ID"] + "</td>");
				DateTime purchaseDate = Convert.ToDateTime(row["Date"]);
				sb.Append("<td>" + purchaseDate.ToString("dd/MM/yyyy") + "</td>");
				sb.Append("<td>" + row["Supplier"] + "</td>");
				sb.Append("<td>" + row["TotalProduk"] + "</td>");
				sb.Append("<td>" + String.Format("Rp {0:N0}", row["TotalHarga"]) + "</td>");
				sb.Append("<td>");
				sb.Append("<a href='ViewPurchase.aspx?PurchaseID=" + row["ID"] + "' class='btn btn-sm' style='background-color: #579c48; color: white;'><i class='fas fa-eye'></i></a> ");
				if (role == "Owner")
				{
					sb.Append("<a href='UpdatePurchase.aspx?PurchaseID=" + row["ID"] + "' class='btn btn-sm' style='background-color: #386bba; color: white;'><i class='fas fa-edit'></i></a> ");
					sb.Append("<a href='javascript:void(0);' onclick='showDeleteModal(\"" + row["ID"] + "\");' class='btn btn-sm' style='background-color: #ff4c4c; color: white;'><i class='fas fa-trash-alt'></i></a>");
				}
				sb.Append("</td>");
				sb.Append("</tr>");
			}

			ltTableRows.Text = sb.ToString();
		}

		protected void create_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Purchase Transaction/CreatePurchase.aspx");
		}

		protected void delete_Click(object sender, EventArgs e)
		{
			string purchaseId = hfPurchaseTransactionIDToDelete.Value;

			// Ambil daftar produk dari transaksi sebelum dihapus
			string queryGetProducts = @"
				SELECT productId, quantity 
				FROM PurchaseTransactionDetail 
				WHERE PurchaseID = @PurchaseID";

			SqlParameter[] parametersGetProducts = new SqlParameter[]
			{
				new SqlParameter("@PurchaseID", purchaseId)
			};

			DataTable dtProducts = dbCon.Fetch(queryGetProducts, parametersGetProducts);

			foreach (DataRow row in dtProducts.Rows)
			{
				string productId = row["productId"].ToString();
				int quantity = Convert.ToInt32(row["quantity"]);

				// Kembalikan stok produk
				string queryUpdateStock = @"
					UPDATE Product 
					SET stock = stock - @quantity 
					WHERE productId = @productId";

				SqlParameter[] parametersUpdateStock = new SqlParameter[]
				{
					new SqlParameter("@quantity", quantity),
					new SqlParameter("@productId", productId)
				};

				dbCon.Fetch(queryUpdateStock, parametersUpdateStock);
			}

			string queryDeleteTransaction = "DELETE FROM PurchaseTransaction WHERE PurchaseID = @PurchaseID";

			SqlParameter[] parametersDeleteTransaction = new SqlParameter[]
			{
				new SqlParameter("@PurchaseID", purchaseId)
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