using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Supplier
{
	public partial class UpdateSupplier : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadSupplierData();
			}
		}
		protected void LoadSupplierData()
		{
			int supplierId = Convert.ToInt32(Request.QueryString["SupplierID"]);

			CommonFunction dbCon = new CommonFunction();
			string query = "SELECT * FROM Supplier WHERE supplierId = @SupplierID";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@SupplierID", supplierId)
			};

			var dt = dbCon.Fetch(query, parameters);

			if (dt.Rows.Count > 0)
			{
				DataRow row = dt.Rows[0];
				inputSuppliername.Value = row["supplierName"].ToString();
				string[] addressParts = row["address"].ToString().Split(new[] { ", " }, StringSplitOptions.None);
				inputAddress.Value = addressParts[0];
				inputCity.Value = addressParts[1];
				inputContactnumber.Value = row["contactNumber"].ToString();
				bool isActive = Convert.ToBoolean(row["isActive"]);
				isActiveDropdown.SelectedValue = isActive ? "Aktif" : "Inaktif";
			}
		}
		protected void Update_Click(object sender, EventArgs e)
		{
			int supplierId = Convert.ToInt32(Request.QueryString["SupplierID"]);
			string supplierName = inputSuppliername.Value;
			string address = inputAddress.Value;
			string city = inputCity.Value;
			string fullAddress = address + ", " + city;
			string contactNumber = inputContactnumber.Value;
			bool isActive = isActiveDropdown.SelectedValue == "Aktif";

			string checkQuery = "SELECT * FROM Supplier WHERE supplierName = @Suppliername AND supplierId != @SupplierID";

			SqlParameter[] checkParameters = new SqlParameter[]
			{
				new SqlParameter("@Suppliername", supplierName),
				new SqlParameter("@SupplierID", supplierId)
			};

			CommonFunction dbCon = new CommonFunction();
			DataTable dt = dbCon.Fetch(checkQuery, checkParameters);

			if (dt.Rows.Count > 0)
			{
				lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> Nama pemasok tersebut sudah terdaftar, silahkan gunakan nama yang lain.";
				lblErrorMessage.Visible = true;

				inputSuppliername.Attributes.Add("class", "form-control input-error");
			}
			else
			{
				string updateQuery = "UPDATE Supplier SET supplierName = @Suppliername, address = @Address, contactNumber = @Contactnumber, updatedTime = GETDATE(), isActive = @IsActive WHERE SupplierID = @SupplierID";

				SqlParameter[] parameters = new SqlParameter[]
				{
					new SqlParameter("@Suppliername", supplierName),
					new SqlParameter("@Address", fullAddress),
					new SqlParameter("@ContactNumber", contactNumber),
					new SqlParameter("@IsActive", isActive),
					new SqlParameter("@SupplierID", supplierId)
				};

				try
				{
					dbCon.Query(updateQuery, parameters);
					Response.Redirect("Supplier.aspx");
				}
				catch (Exception ex)
				{
					lblErrorMessage.Text = "<i class='fas fa-exclamation-circle'></i> " + ex.Message;
					lblErrorMessage.Visible = true;

					inputSuppliername.Attributes.Add("class", "form-control input-error");
				}
			}
		}
	}
}