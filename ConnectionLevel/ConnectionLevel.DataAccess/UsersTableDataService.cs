using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ConnectionLevel.Models
{
    public class UsersTableDataService
    {
        private readonly string _connectionString;
        private readonly string _ownerName;
        private readonly DbProviderFactory _providerFactory;


        public UsersTableDataService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["mainAppConnectionString"].ConnectionString;
            _ownerName = ConfigurationManager.AppSettings["ownerName"];
            
            _providerFactory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["mainAppConnectionString"].ProviderName);
        }
        public List<User> GetAll()
        {
            var data = new List<User>();

            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.ConnectionString = _connectionString;
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
                catch (DbException exception)
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
            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                DbTransaction transaction = null;
                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    command.Transaction = transaction;
                    command.CommandText = $"insert into Users values(@login, @password)";

                    DbParameter loginParameter = command.CreateParameter();
                    loginParameter.ParameterName = "@login";
                    loginParameter.DbType = System.Data.DbType.String;
                    loginParameter.Value = user.Login;

                    DbParameter passwordParameter = command.CreateParameter();
                    passwordParameter.ParameterName = "@password";
                    passwordParameter.DbType = System.Data.DbType.String;
                    passwordParameter.Value = user.Password;

                    command.Parameters.AddRange(new DbParameter[] { loginParameter, passwordParameter });
                    var affectedRows = command.ExecuteNonQuery();

                    if (affectedRows < 1) throw new Exception("Вставка не удалась");

                    transaction.Commit();
                }
                catch (DbException exception)
                {
                    transaction?.Rollback();
                    //ToDo obrabotka
                    throw;
                }
                catch (Exception exception)
                {
                    transaction?.Rollback();
                    //ToDo obrabotka
                    throw;
                }
                finally
                {
                    transaction?.Dispose();
                }
            }
        }

        public void DeleteById(int id)
        {

        }

        public void Update(User user)
        {

        }

        public void ExecuteTransaction(DbConnection connection, params DbCommand[] commands)
        {
            DbTransaction transaction = null;
            transaction = connection.BeginTransaction();
            for(int i = 0; i < commands.Length; i++)
            {
                commands[i].Transaction = transaction;
            }
            for(int i = 0; i < commands.Length; i++)
            {
                try
                {
                    var affectedRows = commands[i].ExecuteNonQuery();
                    if (affectedRows < 1) throw new Exception("Вставка не удалась");
                }
                catch (DbException exception)
                {
                    transaction?.Rollback();
                    //ToDo obrabotka
                    throw;
                }
                catch (Exception exception)
                {
                    transaction?.Rollback();
                    //ToDo obrabotka
                    throw;
                }
            }            
            transaction.Commit();
        }
    }
}
