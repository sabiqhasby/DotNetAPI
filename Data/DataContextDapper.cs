using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DotnetAPI.Data
{
   class DataContextDapper
   {
      //private readonly IConfiguration _config;
      private readonly string _connectionString;
      public DataContextDapper(IConfiguration config)
      {
         //_config = config;
         _connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
      }


      private IDbConnection CreateConnection()
      {
         return new SqlConnection(_connectionString);
      }

      public IEnumerable<T> LoadData<T>(string sql)
      {
         IDbConnection dbConnection = CreateConnection();
         return dbConnection.Query<T>(sql);
      }
      public T LoadDataSingle<T>(string sql)
      {
         IDbConnection dbConnection = CreateConnection();
         return dbConnection.QuerySingle<T>(sql);
      }

      public bool ExecuteSql(string sql)
      {
         IDbConnection dbConnection = CreateConnection();
         return dbConnection.Execute(sql) > 0;
      }
      public int ExecuteSqlWithRowCount(string sql)
      {
         IDbConnection dbConnection = CreateConnection();
         return dbConnection.Execute(sql);
      }
      public async Task<bool> ExecuteSqlAsync(string sql, object parameters)
      {
         IDbConnection dbConnection = CreateConnection();
         return await dbConnection.ExecuteAsync(sql, parameters) > 0;
      }

      public async Task<int> ExecuteSqlWithRowCountAsync(string sql, object parameters)
      {
         IDbConnection dbConnection = CreateConnection();
         return await dbConnection.ExecuteAsync(sql, parameters);
      }

   }
}