using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Users
{
	public partial class CreateUser : System.Web.UI.Page
	{
		protected void Add_Click(object sender, EventArgs e)
		{
			string fullname = inputFullname.Value;
			string username = inputUsername.Value;
			string password = inputPassword.Value;
			string selectedRoleName = ownerRole.Checked ? "Owner" : "Admin";

			int roleId = GetRoleId(selectedRoleName);

			string query = @"
					IF NOT EXISTS (SELECT 1 FROM Users WHERE username = @Username)
					BEGIN
						INSERT INTO Users (fullName, username, password, roleId, createdTime, updatedTime)
						VALUES (@FullName, @Username, @Password, @RoleId, GETDATE(), GETDATE())
					END
					ELSE
					BEGIN
						THROW 50000, 'Pengguna dengan username tersebut sudah terdaftar, silahkan gunakan username lain.', 1;
					END";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@FullName", fullname),
				new SqlParameter("@Username", username),
				new SqlParameter("@Password", password),
				new SqlParameter("@RoleId", roleId)
			};

			try
			{
				CommonFunction commonFunction = new CommonFunction();
				commonFunction.Query(query, parameters);
				Response.Redirect("Users.aspx");
			}
			catch (Exception ex)
			{
				lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> " + ex.Message;
				lblErrorMessage.Visible = true;

				inputUsername.Attributes.Add("class", "form-control input-error");
			}
		}

		private int GetRoleId(string roleName)
		{
			CommonFunction commonFunction = new CommonFunction();
			string query = "SELECT roleId FROM Roles WHERE roleName = @RoleName";
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@RoleName", roleName)
			};
			DataTable dt = commonFunction.Fetch(query, parameters);

			if (dt.Rows.Count > 0)
			{
				return Convert.ToInt32(dt.Rows[0]["roleId"]);
			}
			else
			{
				throw new Exception("Role tidak ditemukan.");
			}
		}
	}
}