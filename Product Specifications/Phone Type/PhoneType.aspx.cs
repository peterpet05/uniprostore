using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Phone_Type
{
	public partial class PhoneType : System.Web.UI.Page
	{
		CommonFunction dbCon = new CommonFunction();
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				GetTableData();
				LoadPhoneBrand();
			}
		}

		private void LoadPhoneBrand()
		{
			string query = @"
						   SELECT DISTINCT b.phoneBrandName 
						   FROM PhoneBrand b
						   JOIN PhoneType t ON b.phoneBrandId = t.phoneBrandId";

			DataTable dtPhoneBrands = dbCon.Fetch(query);

			filterPhoneBrand.Items.Clear();
			filterPhoneBrand.Items.Add(new ListItem("All", ""));

			foreach (DataRow row in dtPhoneBrands.Rows)
			{
				filterPhoneBrand.Items.Add(new ListItem(row["phoneBrandName"].ToString(), row["phoneBrandName"].ToString()));
			}
		}

		public void GetTableData()
		{
			DataTable dt = new DataTable();
			dt = dbCon.Fetch("SELECT ROW_NUMBER() OVER(ORDER BY t.phoneTypeName ASC) AS [No], t.phoneTypeId AS [ID], t.phoneTypeName AS [PhoneTypename], b.phoneBrandName AS [PhoneBrandname] " +
							 "FROM PhoneType t JOIN PhoneBrand b ON t.phoneBrandId = b.phoneBrandId");

			StringBuilder sb = new StringBuilder();
			foreach (DataRow row in dt.Rows)
			{
				sb.Append("<tr>");
				sb.Append("<td>" + row["No"] + "</td>");
				sb.Append("<td class='hidden-column'>" + row["PhoneBrandname"] + "</td>");
				sb.Append("<td>" + row["phoneTypeName"] + "</td>");
				sb.Append("<td>");
				sb.Append("<a href='UpdatePhoneType.aspx?PhoneTypeID=" + row["ID"] + "' class='btn btn-sm' style='background-color: #386bba; color: white;'><i class='fas fa-edit'></i></a> ");
				sb.Append("<a href='javascript:void(0);' onclick='showDeleteModal(" + row["ID"] + ");' class='btn btn-sm' style='background-color: #ff4c4c; color: white;'><i class='fas fa-trash-alt'></i></a>");
				sb.Append("</td>");
				sb.Append("</tr>");
			}

			ltTableRows.Text = sb.ToString();
		}

		protected void create_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Product Specifications/Phone Type/CreatePhoneType.aspx");
		}

		protected void delete_Click(object sender, EventArgs e)
		{
			int phoneTypeId = Convert.ToInt32(hfPhoneTypeIDToDelete.Value);

			// Query untuk mengecek apakah ada produk terkait dengan tipe ponsel ini
			string checkProductQuery = "SELECT 1 FROM Product WHERE PhoneTypeID = @PhoneTypeID";

			SqlParameter[] parametersForCheck = new SqlParameter[]
			{
				new SqlParameter("@PhoneTypeID", phoneTypeId)
			};

			try
			{
				DataTable dt = dbCon.Fetch(checkProductQuery, parametersForCheck);

				if (dt.Rows.Count > 0)
				{
					// Menampilkan warning toast jika ada produk terkait
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showWarningToast", "launchWarningToast('Silahkan hapus seluruh data produk terkait terlebih dahulu!');", true);
				}
				else
				{
					// Jika tidak ada produk terkait, lanjutkan penghapusan tipe ponsel
					string deletePhoneTypeQuery = "DELETE FROM PhoneType WHERE PhoneTypeID = @PhoneTypeID";
					SqlParameter[] parametersForDelete = new SqlParameter[]
					{
						new SqlParameter("@PhoneTypeID", phoneTypeId)
					};

					dbCon.Query(deletePhoneTypeQuery, parametersForDelete);

					GetTableData();
					// Menampilkan success toast
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccessToast", "launchSuccessToast('Tipe ponsel berhasil dihapus');", true);
					// Menyembunyikan modal setelah penghapusan
					ScriptManager.RegisterStartupScript(this, this.GetType(), "hideModal", "$('#deleteModal').modal('hide');", true);
				}
			}
			catch (Exception ex)
			{
				Response.Write("Error: " + ex.Message);
			}
		}
	}
}