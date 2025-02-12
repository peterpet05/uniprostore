using static Unipro_Store.Models.Function;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System;
using System.Web.Security;

namespace Unipro_Store
{
	public partial class LoginPage : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			lblErrorMessage.Visible = false;
			divErrorContainer.Attributes["class"] = "error-container";
		}

		protected void btnLogin_Click(object sender, EventArgs e)
		{
			string username = txtUsername.Text;
			string password = txtPassword.Text;
			DateTime login = DateTime.Now;

			string[] result = ValidateUser(username, password);

			if (result != null)
			{
				string userId = result[0];     
				string role = result[1];       
				string fullName = result[2];  

				Session["UserId"] = userId;   
				Session["Username"] = username;
				Session["Role"] = role;
				Session["FullName"] = fullName;

				CommonFunction dbCon = new CommonFunction();
				string updateQuery = "UPDATE Users SET lastLogin = @LastLogin WHERE userId = @UserId";

				SqlParameter[] parameters = new SqlParameter[]
				{
					new SqlParameter("@LastLogin", login),
					new SqlParameter("@UserId", userId),
				};

				var dt = dbCon.Fetch(updateQuery, parameters);

				Response.Redirect("~/Dashboard.aspx");
			}
			else
			{
				lblErrorMessage.Text = "<i class='fa fa-exclamation-circle'></i> Invalid username or password.";
				lblErrorMessage.Visible = true;
				divErrorContainer.Attributes["class"] = "error-container-visible";
			}
		}

		private string[] ValidateUser(string username, string password)
		{
			CommonFunction commonFunction = new CommonFunction();

			string query = "SELECT u.userId, r.roleName, u.fullName " +
				   "FROM Users u " +
				   "JOIN Roles r ON u.roleId = r.roleId " +
				   "WHERE u.username = @Username AND u.password = @Password AND u.isActive = 1";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@Username", username),
				new SqlParameter("@Password", password)
			};

			DataTable dt = commonFunction.Fetch(query, parameters);

			if (dt.Rows.Count > 0)
			{
				return new string[] { dt.Rows[0]["userId"].ToString(), dt.Rows[0]["roleName"].ToString(), dt.Rows[0]["fullName"].ToString() };
			}
			else
			{
				return null;
			}
		}
	}
}