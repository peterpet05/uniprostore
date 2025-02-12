using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Users
{
	public partial class Users : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				GetTableData();
				LoadRoles();
			}
		}

		private void LoadRoles()
		{
			DataTable dtRoles = dbCon.Fetch("SELECT roleId, roleName FROM Roles");

			filterRole.Items.Clear();
			filterRole.Items.Add(new ListItem("All", ""));

			foreach (DataRow row in dtRoles.Rows)
			{
				filterRole.Items.Add(new ListItem(row["roleName"].ToString(), row["roleName"].ToString()));
			}
		}


		public void GetTableData()
		{
			DataTable dt = new DataTable();
			dt = dbCon.Fetch(@"
							SELECT 
								ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS [No], 
								u.userId AS [ID], 
								u.fullName AS [Fullname], 
								u.username AS [Username], 
								u.password AS [Password], 
								r.roleName AS [Role], 
								u.createdTime AS [Time], 
								u.isActive AS [Status],
								u.lastLogin AS [LastLogin]
							FROM 
								Users u
							JOIN 
								Roles r ON u.roleId = r.roleId
						");

			StringBuilder sb = new StringBuilder();
			foreach (DataRow row in dt.Rows)
			{
				sb.Append("<tr>");
				sb.Append("<td>" + row["No"] + "</td>");
				sb.Append("<td>" + row["Fullname"] + "</td>");
				sb.Append("<td>" + row["Username"] + "</td>");
				sb.Append("<td>" + row["Password"] + "</td>");
				sb.Append("<td>" + row["Role"] + "</td>");
				sb.Append("<td>" + row["Time"] + "</td>");
				bool isActive = Convert.ToBoolean(row["Status"]);
				sb.Append("<td>" + (isActive ? "Aktif" : "Inaktif") + "</td>");
				sb.Append("<td>" + row["LastLogin"] + "</td>");
				sb.Append("<td>");
				sb.Append("<a href='UpdateUser.aspx?UserID=" + row["ID"] + "' class='btn btn-sm' style='background-color: #386bba; color: white;'><i class='fas fa-edit'></i></a> ");
				sb.Append("<a href='javascript:void(0);' onclick='showDeleteModal(" + row["ID"] + ");' class='btn btn-sm' style='background-color: #ff4c4c; color: white;'><i class='fas fa-trash-alt'></i></a>");
				sb.Append("</td>");
				sb.Append("</tr>");
			}

			ltTableRows.Text = sb.ToString();
		}

		protected void create_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Users/CreateUser.aspx");
		}

		protected void delete_Click(object sender, EventArgs e)
		{
			int userId = Convert.ToInt32(hfUserIDToDelete.Value);

			// Query untuk mengecek apakah ada SalesTransaction yang dicatat oleh user ini
			string checkSalesTransactionQuery = "SELECT 1 FROM SalesTransaction WHERE UserID = @UserID";

			SqlParameter[] parametersForCheck = new SqlParameter[]
			{
				new SqlParameter("@UserID", userId)
			};

			try
			{
				DataTable dt = dbCon.Fetch(checkSalesTransactionQuery, parametersForCheck);

				if (dt.Rows.Count > 0)
				{
					// Menampilkan warning toast jika ada transaksi penjualan terkait
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showWarningToast", "launchWarningToast('User tidak dapat dihapus karena terkait dengan pencatatan transaksi');", true);
				}
				else
				{
					string deleteUserQuery = "DELETE FROM Users WHERE UserID = @UserID";
					SqlParameter[] parametersForDelete = new SqlParameter[]
					{
						new SqlParameter("@UserID", userId)
					};

					dbCon.Query(deleteUserQuery, parametersForDelete);

					GetTableData();

					// Menampilkan success toast
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccessToast", "launchSuccessToast('User berhasil dihapus');", true);
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