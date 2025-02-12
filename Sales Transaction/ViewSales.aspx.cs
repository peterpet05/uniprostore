using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Sales_Transaction
{
	public partial class ViewSales : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();

		public string salesId
		{
			get { return (string)(ViewState["salesId"] ?? -1); }
			set { ViewState["salesId"] = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request.QueryString["salesId"] != null)
			{
				salesId = Request.QueryString["salesId"];
			}
			GetTableData();
		}

		public void GetTableData()
		{
			string query = @"
							SELECT M.mediaName AS salesMedia, ST.salesDate, STD.productId, STD.quantity AS Kuantitas, 
								   STD.discount AS Diskon, STD.subtotal, p.sellPrice AS Harga, p.image, ST.DiscountedPrice, 
								   CONCAT(sc.subCategoryName, ' ', pb.productBrandName, ' ', pm.productModelName, ' ', pbr.phoneBrandName, ' ', pt.phoneTypeName, ' ', pc.colorName) AS NamaProduk
							FROM SalesTransaction ST
							JOIN Media M ON ST.mediaId = M.mediaId
							JOIN SalesTransactionDetail STD ON ST.salesId = STD.salesId
							JOIN Product P ON STD.productId = P.productId
							JOIN SubCategory SC ON P.subCategoryId = SC.subCategoryId
							JOIN ProductModel PM ON P.productModelId = PM.productModelId
							JOIN ProductBrand PB ON PM.productBrandId = PB.productBrandId
							JOIN PhoneType PT ON P.phoneTypeId = PT.phoneTypeId
							JOIN PhoneBrand PBR ON PT.phoneBrandId = PBR.phoneBrandId
							JOIN ProductColor PC ON P.colorId = PC.colorId
							WHERE ST.salesId = @SalesId";
			;
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@SalesId", salesId)
			};

			DataTable dt = dbCon.Fetch(query, parameters);
			if (dt.Rows.Count > 0)
			{
				foreach (DataRow dr in dt.Rows)
				{
					salesMedia.Text = dr["salesMedia"].ToString();
					salesDate.Text = Convert.ToDateTime(dr["salesDate"]).ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
					rptProducts.DataSource = dt;
					rptProducts.DataBind();
					ltlGrandTotal.Text = String.Format("Rp {0:N0}", Convert.ToDecimal(dr["DiscountedPrice"]));

					if (dr["Image"] != DBNull.Value)
					{
						byte[] imageBytes = (byte[])dr["Image"];
						string imgBase64 = Convert.ToBase64String(imageBytes);
					}
				}
			}
		}
	}
}