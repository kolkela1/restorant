using MySql.Data.MySqlClient;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services
{
	public class AuthService
	{
		private DatabaseService dbService;

		public AuthService()
		{
			dbService = new DatabaseService();
		}

		public User Login(string username, string password)
		{
			User user = null;

			using (var connection = dbService.GetConnection())
			{
				connection.Open();
				string query = "SELECT * FROM Users WHERE Username = @username AND Password = @password AND IsActive = 1";

				using (var cmd = new MySqlCommand(query, connection))
				{
					cmd.Parameters.AddWithValue("@username", username);
					cmd.Parameters.AddWithValue("@password", password);

					using (var reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							user = new User
							{
								UserID = Convert.ToInt32(reader["UserID"]),
								Username = reader["Username"].ToString(),
								FullName = reader["FullName"].ToString(),
								UserType = reader["UserType"].ToString(),
								Phone = reader["Phone"]?.ToString(),
								Email = reader["Email"]?.ToString()
							};
						}
					}
				}
			}

			return user;
		}

		public bool AddUser(User user, string password)
		{
			using (var connection = dbService.GetConnection())
			{
				connection.Open();
				string query = @"INSERT INTO Users (Username, Password, FullName, Phone, Email, UserType, IsActive) 
                               VALUES (@username, @password, @fullname, @phone, @email, @usertype, 1)";

				using (var cmd = new MySqlCommand(query, connection))
				{
					cmd.Parameters.AddWithValue("@username", user.Username);
					cmd.Parameters.AddWithValue("@password", password);
					cmd.Parameters.AddWithValue("@fullname", user.FullName);
					cmd.Parameters.AddWithValue("@phone", user.Phone ?? "");
					cmd.Parameters.AddWithValue("@email", user.Email ?? "");
					cmd.Parameters.AddWithValue("@usertype", user.UserType);

					return cmd.ExecuteNonQuery() > 0;
				}
			}
		}

		public bool DeleteUser(int userId)
		{
			using (var connection = dbService.GetConnection())
			{
				connection.Open();
				string query = "DELETE FROM Users WHERE UserID = @userid";

				using (var cmd = new MySqlCommand(query, connection))
				{
					cmd.Parameters.AddWithValue("@userid", userId);
					return cmd.ExecuteNonQuery() > 0;
				}
			}
		}

		public List<User> GetAllUsers()
		{
			var users = new List<User>();

			using (var connection = dbService.GetConnection())
			{
				connection.Open();
				string query = "SELECT * FROM Users WHERE IsActive = 1 ORDER BY UserType, FullName";

				using (var cmd = new MySqlCommand(query, connection))
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						users.Add(new User
						{
							UserID = Convert.ToInt32(reader["UserID"]),
							Username = reader["Username"].ToString(),
							FullName = reader["FullName"].ToString(),
							UserType = reader["UserType"].ToString(),
							Phone = reader["Phone"]?.ToString(),
							Email = reader["Email"]?.ToString(),
							CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
						});
					}
				}
			}

			return users;
		}
	}
}