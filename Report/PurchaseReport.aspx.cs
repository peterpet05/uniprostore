using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unipro_Store.Models.Function;

namespace Unipro_Store.Report
{
	public partial class PurchaseReport : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadSubCategories();
				LoadProductBrands();
				LoadPhones();
				LoadSuppliers();
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

		private void LoadPhones()
		{
			CommonFunction dbCon = new CommonFunction();
			DataTable dtPhones = dbCon.Fetch("SELECT CONCAT(phb.phoneBrandName, ' ', pt.phoneTypeName) AS [NamaPonsel] FROM PhoneBrand phb INNER JOIN PhoneType pt ON phb.phoneBrandId = pt.phoneBrandId");

			ddlPhone.DataSource = dtPhones;
			ddlPhone.DataTextField = "NamaPonsel";
			ddlPhone.DataBind();
			ddlPhone.Items.Insert(0, new ListItem("All", "All"));
			ddlPhone.SelectedValue = "All";
		}

		private void LoadSuppliers()
		{
			CommonFunction dbCon = new CommonFunction();
			DataTable dtSuppliers = dbCon.Fetch("SELECT supplierName FROM Supplier WHERE isActive = 1");

			ddlSupplier.DataSource = dtSuppliers;
			ddlSupplier.DataTextField = "supplierName";
			ddlSupplier.DataBind();
			ddlSupplier.Items.Insert(0, new ListItem("All", "All"));
			ddlSupplier.SelectedValue = "All";
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
			Session["Phone"] = ddlPhone.SelectedValue;
			Session["Supplier"] = ddlSupplier.SelectedValue;
			Session["StartDate"] = inputStartDate.Value;
			Session["EndDate"] = inputEndDate.Value;

			Response.Redirect("PurchaseReportResult.aspx");
		}
	}
}