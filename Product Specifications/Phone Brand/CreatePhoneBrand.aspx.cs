using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Phone_Brand
{
	public partial class CreatePhoneBrand : System.Web.UI.Page
	{
		protected void Add_Click(object sender, EventArgs e)
		{
			string brandname = inputPhoneBrandname.Value;

			string query = @"
					IF NOT EXISTS (SELECT 1 FROM PhoneBrand WHERE phoneBrandName = @PhoneBrandName)
					BEGIN
						INSERT INTO PhoneBrand (phoneBrandName, createdTime, updatedTime)
						VALUES (@PhoneBrandName, GETDATE(), GETDATE())
					END
					ELSE
					BEGIN
						THROW 50000, 'Nama brand ponsel tersebut sudah terdaftar, silahkan gunakan nama yang lain.', 1;
					END";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@PhoneBrandName", brandname),
			};

			try
			{
				CommonFunction commonFunction = new CommonFunction();
				commonFunction.Query(query, parameters);
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