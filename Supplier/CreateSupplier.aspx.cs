using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Supplier
{
	public partial class CreateSupplier : System.Web.UI.Page
	{
		protected void Add_Click(object sender, EventArgs e)
		{
			string suppliername = inputSuppliername.Value;
			string address = inputAddress.Value;
			string city = inputCity.Value;
			string fullAddress = address + ", " + city;
			string contactnumber = inputContactnumber.Value;

			string query = @"
					IF NOT EXISTS (SELECT 1 FROM Supplier WHERE suppliername = @SupplierName)
					BEGIN
						INSERT INTO Supplier (supplierName, address, contactNumber, createdTime, updatedTime)
						VALUES (@SupplierName, @Address, @ContactNumber, GETDATE(), GETDATE())
					END
					ELSE
					BEGIN
						THROW 50000, 'Nama pemasok tersebut sudah terdaftar, silahkan gunakan nama yang lain.', 1;
					END";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@SupplierName", suppliername),
				new SqlParameter("@Address", fullAddress),
				new SqlParameter("@ContactNumber", contactnumber)
			};

			try
			{
				CommonFunction commonFunction = new CommonFunction();
				commonFunction.Query(query, parameters);
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