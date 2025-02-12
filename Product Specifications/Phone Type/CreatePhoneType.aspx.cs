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
	public partial class CreatePhoneType : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadPhoneBrands();
			}
		}
		private void LoadPhoneBrands()
		{
			CommonFunction dbCon = new CommonFunction();
			DataTable dtPhoneBrands = dbCon.Fetch("SELECT phoneBrandId, phoneBrandName FROM PhoneBrand");

			foreach (DataRow row in dtPhoneBrands.Rows)
			{
				inputPhoneBrand.Items.Add(new ListItem(row["phoneBrandName"].ToString(), row["phoneBrandId"].ToString()));
			}
		}
		protected void Add_Click(object sender, EventArgs e)
		{
			string phonetypename = inputPhoneTypename.Value;
			string phoneBrandId = inputPhoneBrand.Value;

			string query = @"
						   IF NOT EXISTS (SELECT 1 FROM PhoneType WHERE phoneTypeName = @PhoneTypeName AND phoneBrandId = @PhoneBrandId)
						   BEGIN
							   INSERT INTO PhoneType (phoneBrandId, phoneTypeName, createdTime, updatedTime)
							   VALUES (@PhoneBrandId, @PhoneTypeName, GETDATE(), GETDATE())
						   END
						   ELSE
						   BEGIN
							   THROW 50000, 'Nama tipe ponsel tersebut sudah terdaftar dalam brand ini, silahkan gunakan nama yang lain.', 1;
						   END";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@PhoneBrandId", phoneBrandId),
				new SqlParameter("@PhoneTypeName", phonetypename),
			};

			try
			{
				CommonFunction commonFunction = new CommonFunction();
				commonFunction.Query(query, parameters);
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