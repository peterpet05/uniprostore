using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Phone_Brand
{
	public partial class UpdatePhoneBrand : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadPhoneBrandData();
			}
		}
		protected void LoadPhoneBrandData()
		{
			int phoneBrandId = Convert.ToInt32(Request.QueryString["PhoneBrandID"]);

			CommonFunction dbCon = new CommonFunction();
			string query = "SELECT * FROM PhoneBrand WHERE phoneBrandId = @PhoneBrandID";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@PhoneBrandID", phoneBrandId)
			};

			var dt = dbCon.Fetch(query, parameters);

			if (dt.Rows.Count > 0)
			{
				DataRow row = dt.Rows[0];
				inputPhoneBrandname.Value = row["phoneBrandName"].ToString();
			}
		}
		protected void Update_Click(object sender, EventArgs e)
		{
			int phoneBrandId = Convert.ToInt32(Request.QueryString["PhoneBrandID"]);
			string phoneBrandName = inputPhoneBrandname.Value;

			string checkQuery = "SELECT * FROM PhoneBrand WHERE phoneBrandName = @PhoneBrandname AND phoneBrandId != @PhoneBrandID";

			SqlParameter[] checkParameters = new SqlParameter[]
			{
				new SqlParameter("@PhoneBrandname", phoneBrandName),
				new SqlParameter("@PhoneBrandID", phoneBrandId)
			};

			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(checkQuery, checkParameters);

			if (dt.Rows.Count > 0)
			{
				lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> Nama brand ponsel tersebut sudah terdaftar, silahkan gunakan nama yang lain.";
				lblErrorMessage.Visible = true;

				inputPhoneBrandname.Attributes.Add("class", "form-control input-error");
			}
			else
			{
				string updateQuery = "UPDATE PhoneBrand SET phoneBrandName = @Phonebrandname, updatedTime = GETDATE() WHERE PhoneBrandID = @PhoneBrandID";

				SqlParameter[] parameters = new SqlParameter[]
				{
					new SqlParameter("@Phonebrandname", phoneBrandName),
					new SqlParameter("@PhoneBrandID", phoneBrandId)
				};

				try
				{
					dbCon.Query(updateQuery, parameters);
					Response.Redirect("PhoneBrand.aspx");
				}
				catch (Exception ex)
				{
					lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> " + ex.Message;
					lblErrorMessage.Visible = true;

					inputPhoneBrandname.Attributes.Add("class", "form-control input-error");
				}
			}
		}
	}
}