using System;
using System.Data.SqlClient;
using System.Data;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Users
{
	public partial class UpdateUser : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadUserData();
			}
		}

		protected void LoadUserData()
		{
			int userId = Convert.ToInt32(Request.QueryString["UserID"]);

			CommonFunction dbCon = new CommonFunction();
			
			string query = @"
                SELECT u.*, r.roleName 
                FROM Users u
                JOIN Roles r ON u.roleId = r.roleId
                WHERE u.userId = @UserID";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@UserID", userId)
			};

			var dt = dbCon.Fetch(query, parameters);

			if (dt.Rows.Count > 0)
			{
				DataRow row = dt.Rows[0];
				inputFullname.Value = row["fullName"].ToString();
				inputUsername.Value = row["username"].ToString();
				hiddenPassword.Value = row["Password"].ToString();
				string role = row["roleName"].ToString();

				if (role == "Owner")
				{
					ownerRole.Checked = true;
				}
				else if (role == "Admin")
				{
					adminRole.Checked = true;
				}

				bool isActive = Convert.ToBoolean(row["isActive"]);
				isActiveDropdown.SelectedValue = isActive ? "Aktif" : "Inaktif";
			}
		}

		protected void Update_Click(object sender, EventArgs e)
		{
			int userId = Convert.ToInt32(Request.QueryString["UserID"]);
			string fullName = inputFullname.Value;
			string username = inputUsername.Value;
			string password = string.IsNullOrEmpty(inputPassword.Value) ? hiddenPassword.Value : inputPassword.Value;
			string selectedRoleName = ownerRole.Checked ? "Owner" : "Admin";
			bool isActive = isActiveDropdown.SelectedValue == "Aktif";
			int roleId = GetRoleId(selectedRoleName);

			string checkQuery = "SELECT * FROM Users WHERE username = @Username AND userId != @UserID";

			SqlParameter[] checkParameters = new SqlParameter[]
			{
				new SqlParameter("@Username", username),
				new SqlParameter("@UserID", userId)
			};

			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(checkQuery, checkParameters);

			if (dt.Rows.Count > 0)
			{
				lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> Pengguna dengan username tersebut sudah terdaftar, silakan gunakan username lain.";
				lblErrorMessage.Visible = true;

				inputUsername.Attributes.Add("class", "form-control input-error");
			}
			else
			{
				string updateQuery = "UPDATE Users SET fullName = @FullName, username = @Username, password = @Password, roleId = @RoleId, updatedTime = GETDATE(), isActive = @IsActive WHERE UserID = @UserID";

				SqlParameter[] parameters = new SqlParameter[]
				{
					new SqlParameter("@FullName", fullName),
					new SqlParameter("@Username", username),
					new SqlParameter("@Password", password),
					new SqlParameter("@RoleId", roleId),
					new SqlParameter("@IsActive", isActive),
					new SqlParameter("@UserID", userId)
				};

				try
				{
					dbCon.Query(updateQuery, parameters);
					Response.Redirect("Users.aspx"); 
				}
				catch (Exception ex)
				{
					lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> " + ex.Message;
					lblErrorMessage.Visible = true;

					inputUsername.Attributes.Add("class", "form-control input-error");
				}
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