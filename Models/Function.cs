using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Unipro_Store.Models
{
	public class Function
	{
		public class CommonFunction
		{
			SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["UniproStoreCS"].ConnectionString);

			public void Query(string query, SqlParameter[] parameters = null)
			{
				if (con.State == ConnectionState.Closed)
				{
					con.Open();
				}

				using (SqlCommand cmd = new SqlCommand(query, con))
				{
					if (parameters != null)
					{
						cmd.Parameters.AddRange(parameters);
					}

					cmd.ExecuteNonQuery();
				}

				con.Close();
			}

			public DataTable Fetch(string query, SqlParameter[] parameters = null)
			{
				DataTable dt = new DataTable();
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["UniproStoreCS"].ConnectionString))
				{
					using (SqlCommand cmd = new SqlCommand(query, con))
					{
						if (parameters != null)
						{
							cmd.Parameters.AddRange(parameters);
						}

						con.Open();
						using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
						{
							sda.Fill(dt);
						}
					}
				}
				return dt;
			}

		}
	}
}