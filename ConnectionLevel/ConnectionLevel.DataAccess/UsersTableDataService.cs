using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLevel.Models
{
    public class UsersTableDataService
    {
        private readonly string _connectionString;
        public UsersTableDataService()
        {
            _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\оралове\Source\Repos\ConnectionLevel\ConnectionLevel\ConnectionLevel.DataAccess\Database.mdf;Integrated Security=True";
        }
        public List<User> GetAll()
        {
            var data = new List<User>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.Open();
                    command.CommandText = "select * from Users";

                    var dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        int id = (int)dataReader["Id"];
                        string login = dataReader["Login"].ToString();
                        string password = dataReader["Password"].ToString();
                        data.Add(new User { Id = id, Login = login, Password = password });
                    }
                    dataReader.Close();
                }
                catch (SqlException exception)
                {
                    //ToDo obrabotka
                    throw;
                }
                catch (Exception exception)
                {
                    //ToDo obrabotka
                    throw;
                }
            }
            return data;
        }

        public void Add(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.Open();
                    command.CommandText = $"insert into Users values('{user.Login}', '{user.Password}')";
                    var affectedRows = command.ExecuteNonQuery();

                    if (affectedRows < 1) throw new Exception("Вставка не удалась");
                }
                catch (SqlException exception)
                {
                    //ToDo obrabotka
                    throw;
                }
                catch (Exception exception)
                {
                    //ToDo obrabotka
                    throw;
                }
            }
        }

        public void DeleteById(int id)
        {

        }

        public void Update(User user)
        {

        }
    }
}
