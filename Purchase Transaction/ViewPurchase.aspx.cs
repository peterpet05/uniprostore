using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Purchase_Transaction
{
	public partial class ViewPurchase : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();

		public string purchaseId
		{
			get { return (string)(ViewState["purchaseId"] ?? -1); }
			set { ViewState["purchaseId"] = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request.QueryString["purchaseId"] != null)
			{
				purchaseId = Request.QueryString["purchaseId"];
			}
			GetTableData();
		}

		public void GetTableData()
		{
			string query = @"SELECT PT.purchaseDate, PTD.productId, PTD.quantity AS Kuantitas, 
							PTD.subtotal, PTD.buyPrice AS Harga, P.image, PT.totalPrice, S.supplierName,
							CONCAT(SC.subCategoryName, ' ', PB.productBrandName, ' ', PM.productModelName, ' ', PBR.phoneBrandName, ' ', PTY.phoneTypeName, ' ', PC.colorName) AS NamaProduk
							FROM PurchaseTransaction PT
							JOIN PurchaseTransactionDetail PTD ON PT.purchaseId = PTD.purchaseId
							JOIN Supplier S ON PT.supplierId = S.supplierId
							JOIN Product P ON PTD.productId = P.productId
							JOIN SubCategory SC ON P.subCategoryId = SC.subCategoryId
							JOIN ProductModel PM ON P.productModelId = PM.productModelId
							JOIN ProductBrand PB ON PM.productBrandId = PB.productBrandId
							JOIN PhoneType PTY ON P.phoneTypeId = PTY.phoneTypeId
							JOIN PhoneBrand PBR ON PTY.phoneBrandId = PBR.phoneBrandId
							JOIN ProductColor PC ON P.colorId = PC.colorId
							WHERE PT.purchaseId = @PurchaseId";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@PurchaseId", purchaseId)
			};

			DataTable dt = dbCon.Fetch(query, parameters);
			if (dt.Rows.Count > 0)
			{
				foreach (DataRow dr in dt.Rows)
				{
					supplierName.Text = dr["supplierName"].ToString();
					purchaseDate.Text = Convert.ToDateTime(dr["purchaseDate"]).ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
					rptProducts.DataSource = dt;
					rptProducts.DataBind();
					ltlGrandTotal.Text = String.Format("Rp {0:N0}", Convert.ToDecimal(dr["totalPrice"]));

					if (dr["Image"] != DBNull.Value)
					{
						byte[] imageBytes = (byte[])dr["Image"];
						string imgBase64 = Convert.ToBase64String(imageBytes);
					}
				}
			}
		}

		protected void btnFaktur_Click(object sender, EventArgs e)
		{
			string query = @"SELECT invoiceImage FROM PurchaseTransaction WHERE purchaseId = @PurchaseId";

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@PurchaseId", purchaseId)
			};

			DataTable dt = dbCon.Fetch(query, parameters);

			if (dt.Rows.Count > 0 && dt.Rows[0]["invoiceImage"] != DBNull.Value)
			{
				byte[] imageBytes = (byte[])dt.Rows[0]["invoiceImage"];
				string imgBase64 = Convert.ToBase64String(imageBytes);

				imgNota.ImageUrl = $"data:image/png;base64,{imgBase64}";

				ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
					"$('#notaModal').modal('show');", true);
			}
			else
			{
				ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorToast",
					"launchErrorToast('Faktur transaksi pembelian ini belum diupload!');", true);
			}
		}

	}
}