using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;
using Unipro_Store.Category;

namespace Unipro_Store.Report
{
	public partial class SalesReport : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadSubCategories();
				LoadProductBrands();
				LoadSalesMedias();
				LoadUsers();
			}
		}

		private void LoadSubCategories()
		{
			CommonFunction dbCon = new CommonFunction();
			DataTable dtSubCategories = dbCon.Fetch("SELECT subCategoryName FROM SubCategory");

			ddlSubcategory.DataSource = dtSubCategories;
			ddlSubcategory.DataTextField = "subCategoryName";
			ddlSubcategory.DataBind();
			ddlSubcategory.Items.Insert(0, new ListItem("All", "All"));
			ddlSubcategory.SelectedValue = "All";
		}

		private void LoadProductBrands()
		{
			CommonFunction dbCon = new CommonFunction();
			DataTable dtProductBrands = dbCon.Fetch("SELECT productBrandName FROM ProductBrand");

			ddlProductBrand.DataSource = dtProductBrands;
			ddlProductBrand.DataTextField = "productBrandName";
			ddlProductBrand.DataBind();
			ddlProductBrand.Items.Insert(0, new ListItem("All", "All"));
			ddlProductBrand.SelectedValue = "All";
		}

		private void LoadSalesMedias()
		{
			CommonFunction dbCon = new CommonFunction();
			DataTable dtSalesMedias = dbCon.Fetch("SELECT DISTINCT M.mediaName FROM SalesTransaction ST JOIN Media M ON ST.mediaId = M.mediaId");

			ddlSalesMedia.DataSource = dtSalesMedias;
			ddlSalesMedia.DataTextField = "mediaName";
			ddlSalesMedia.DataBind();
			ddlSalesMedia.Items.Insert(0, new ListItem("All", "All"));
			ddlSalesMedia.SelectedValue = "All";
		}

		private void LoadUsers()
		{
			CommonFunction dbCon = new CommonFunction();
			DataTable dtUsers = dbCon.Fetch("SELECT fullName FROM Users");

			ddlUser.DataSource = dtUsers;
			ddlUser.DataTextField = "fullName";
			ddlUser.DataBind();
			ddlUser.Items.Insert(0, new ListItem("All", "All"));
			ddlUser.SelectedValue = "All";
		}

		protected void Show_Click(object sender, EventArgs e)
		{
			DateTime startDate;
			DateTime endDate;

			if (DateTime.TryParse(inputStartDate.Value, out startDate) && DateTime.TryParse(inputEndDate.Value, out endDate))
			{
				if (startDate > endDate)
				{
					ScriptManager.RegisterStartupScript(this, this.GetType(), "showWarningToast",
						"launchWarningToast('Tanggal mulai harus lebih kecil atau sama dengan tanggal akhir.');", true);
					return; 
				}
			}
			else
			{
				ScriptManager.RegisterStartupScript(this, this.GetType(), "showErrorToast",
					"launchErrorToast('Tanggal mulai dan tanggal akhir tidak valid.');", true);
				return;
			}

			Session["Subcategory"] = ddlSubcategory.SelectedValue;
			Session["ProductBrand"] = ddlProductBrand.SelectedValue;
			Session["SalesMedia"] = ddlSalesMedia.SelectedValue;
			Session["User"] = ddlUser.SelectedValue;
			Session["StartDate"] = inputStartDate.Value;
			Session["EndDate"] = inputEndDate.Value;

			Response.Redirect("SalesReportResult.aspx");
		}
	}
}