using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Phone_Type
{
	public partial class UpdatePhoneType : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadPhoneTypeData();
			}
		}
		protected void LoadPhoneBrands(string selectedPhoneBrandName)
		{
			CommonFunction dbCon = new CommonFunction();
			string query = "SELECT phoneBrandId, phoneBrandName FROM PhoneBrand";

			DataTable dt = dbCon.Fetch(query);
			inputPhoneBrand.Items.Clear();

			foreach (DataRow row in dt.Rows)
			{
				ListItem item = new ListItem(row["phoneBrandName"].ToString(), row["phoneBrandId"].ToString());
				inputPhoneBrand.Items.Add(item);
			}

			if (!string.IsNullOrEmpty(selectedPhoneBrandName))
			{
				inputPhoneBrand.Value = inputPhoneBrand.Items.FindByText(selectedPhoneBrandName)?.Value;
			}
		}
		protected void LoadPhoneTypeData()
		{
			int phoneTypeId = Convert.ToInt32(Request.QueryString["PhoneTypeID"]);

			CommonFunction dbCon = new CommonFunction();
			string query = @"
							 SELECT t.phoneTypeName, b.phoneBrandName 
							 FROM PhoneType t 
							 JOIN PhoneBrand b ON t.phoneBrandId = b.phoneBrandId 
							 WHERE t.phoneTypeId = @PhoneTypeID";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@PhoneTypeID", phoneTypeId)
			};

			var dt = dbCon.Fetch(query, parameters);

			if (dt.Rows.Count > 0)
			{
				DataRow row = dt.Rows[0];

				inputPhoneTypename.Value = row["phoneTypeName"].ToString();
				string selectedPhoneBrandName = row["phoneBrandName"].ToString();
				LoadPhoneBrands(selectedPhoneBrandName);
			}
		}
		protected void Update_Click(object sender, EventArgs e)
		{
			int phoneTypeId = Convert.ToInt32(Request.QueryString["PhoneTypeID"]);
			string phoneTypeName = inputPhoneTypename.Value;
			int selectedPhoneBrandId = Convert.ToInt32(inputPhoneBrand.Value);

			string checkQuery = @"
								SELECT * FROM PhoneType 
								WHERE phoneTypeName = @PhoneTypeName 
								AND phoneBrandId = @PhoneBrandID 
								AND phoneTypeId != @PhoneTypeID";

			SqlParameter[] checkParameters = new SqlParameter[]
			{
				new SqlParameter("@PhoneTypeName", phoneTypeName),
				new SqlParameter("@PhoneBrandID", selectedPhoneBrandId),
				new SqlParameter("@PhoneTypeID", phoneTypeId)
			};

			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(checkQuery, checkParameters);

			if (dt.Rows.Count > 0)
			{
				lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> Nama tipe ponsel tersebut sudah terdaftar dalam brand ini, silahkan gunakan nama yang lain.";
				lblErrorMessage.Visible = true;

				inputPhoneTypename.Attributes.Add("class", "form-control input-error");
			}
			else
			{
				string updateQuery = "UPDATE PhoneType SET phoneTypeName = @PhoneTypeName, phoneBrandId = @PhoneBrandID, updatedTime = GETDATE() " +
									 "WHERE phoneTypeId = @PhoneTypeID";

				SqlParameter[] parameters = new SqlParameter[]
				{
					new SqlParameter("@PhoneTypeName", phoneTypeName),
					new SqlParameter("@PhoneBrandID", selectedPhoneBrandId),
					new SqlParameter("@PhoneTypeID", phoneTypeId)
				};

				try
				{
					dbCon.Query(updateQuery, parameters);
					Response.Redirect("PhoneType.aspx");
				}
				catch (Exception ex)
				{
					lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> " + ex.Message;
					lblErrorMessage.Visible = true;

					inputPhoneTypename.Attributes.Add("class", "form-control input-error");
				}
			}
		}
	}
}