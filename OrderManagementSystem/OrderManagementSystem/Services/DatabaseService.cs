using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;

namespace OrderManagementSystem.Services
{
	public class DatabaseService
	{
		private readonly string connectionString;

		public DatabaseService()
		{
			var connSettings = ConfigurationManager.ConnectionStrings["MySQLConnection"];
			if (connSettings == null)
				throw new Exception("Connection string 'MySQLConnection' not found in App.config");

			connectionString = connSettings.ConnectionString;
		}

		/// <summary>
		/// الحصول على اتصال جديد بقاعدة البيانات
		/// </summary>
		public MySqlConnection GetConnection()
		{
			return new MySqlConnection(connectionString);
		}

		/// <summary>
		/// اختبار الاتصال بقاعدة البيانات
		/// </summary>
		public bool TestConnection()
		{
			try
			{
				using (var connection = GetConnection())
				{
					connection.Open();
					return true;
				}
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// تنفيذ استعلام SELECT وإرجاع النتائج على شكل DataTable
		/// </summary>
		public DataTable ExecuteQuery(string query, params MySqlParameter[] parameters)
		{
			var dt = new DataTable();
			using (var connection = GetConnection())
			using (var cmd = new MySqlCommand(query, connection))
			{
				if (parameters != null)
					cmd.Parameters.AddRange(parameters);

				using (var adapter = new MySqlDataAdapter(cmd))
				{
					adapter.Fill(dt);
				}
			}
			return dt;
		}

		/// <summary>
		/// تنفيذ استعلام INSERT, UPDATE, DELETE وإرجاع عدد الصفوف المتأثرة
		/// </summary>
		public int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
		{
			using (var connection = GetConnection())
			using (var cmd = new MySqlCommand(query, connection))
			{
				if (parameters != null)
					cmd.Parameters.AddRange(parameters);

				connection.Open();
				return cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// تنفيذ استعلام INSERT مع إرجاع الـ LAST_INSERT_ID()
		/// </summary>
		public int ExecuteInsertReturnId(string query, params MySqlParameter[] parameters)
		{
			using (var connection = GetConnection())
			using (var cmd = new MySqlCommand(query + "; SELECT LAST_INSERT_ID();", connection))
			{
				if (parameters != null)
					cmd.Parameters.AddRange(parameters);

				connection.Open();
				return Convert.ToInt32(cmd.ExecuteScalar());
			}
		}
	}
}
