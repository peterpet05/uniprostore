using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Product_Color
{
	public partial class UpdateProductColor : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadProductColorData();
			}
		}
		protected void LoadProductColorData()
		{
			int colorId = Convert.ToInt32(Request.QueryString["ColorID"]);

			CommonFunction dbCon = new CommonFunction();
			string query = "SELECT * FROM ProductColor WHERE colorId = @ColorID";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@ColorID", colorId)
			};

			var dt = dbCon.Fetch(query, parameters);

			if (dt.Rows.Count > 0)
			{
				DataRow row = dt.Rows[0];
				inputProductColorname.Value = row["colorName"].ToString();
			}
		}
		protected void Update_Click(object sender, EventArgs e)
		{
			int colorId = Convert.ToInt32(Request.QueryString["colorID"]);
			string colorName = inputProductColorname.Value;

			string checkQuery = "SELECT * FROM ProductColor WHERE colorName = @Colorname AND colorId != @ColorID";

			SqlParameter[] checkParameters = new SqlParameter[]
			{
				new SqlParameter("@Colorname", colorName),
				new SqlParameter("@ColorID", colorId)
			};

			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(checkQuery, checkParameters);

			if (dt.Rows.Count > 0)
			{
				lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> Nama warna produk tersebut sudah terdaftar, silahkan gunakan nama yang lain.";
				lblErrorMessage.Visible = true;

				inputProductColorname.Attributes.Add("class", "form-control input-error");
			}
			else
			{
				string updateQuery = "UPDATE ProductColor SET colorName = @Colorname, updatedTime = GETDATE() WHERE ColorID = @ColorID";

				SqlParameter[] parameters = new SqlParameter[]
				{
					new SqlParameter("@Colorname", colorName),
					new SqlParameter("@ColorID", colorId)
				};

				try
				{
					dbCon.Query(updateQuery, parameters);
					Response.Redirect("ProductColor.aspx");
				}
				catch (Exception ex)
				{
					lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> " + ex.Message;
					lblErrorMessage.Visible = true;

					inputProductColorname.Attributes.Add("class", "form-control input-error");
				}
			}
		}
	}
}