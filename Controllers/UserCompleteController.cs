using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DotnetAPI.Controllers;


[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
   private readonly DataContextDapper _dapper;
   public UserCompleteController(IConfiguration config)
   {
      _dapper = new DataContextDapper(config);
   }

   [HttpGet("TestConnection")]
   public DateTime TestConnection()
   {
      return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
   }


   [HttpGet("GetUsers/{userId}")]
   public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
   {
      string sql = "EXEC TutorialAppSchema.spUsers_Get";
      string parameters = "";

      if (userId != 0)
      {
         parameters += ", @UserId =" + userId.ToString();
      }
      if (isActive)
      {
         parameters += ", @Active =" + isActive.ToString();
      }

      sql += parameters.Substring(1); //, parameters.Length);

      //LOAD DATA USERS
      IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
      return users;
   }

   [HttpPut("UpsertUser")]
   public IActionResult UpsertUser(UserComplete user)
   {

      string sql = @"
         EXEC TutorialAppSchema.spUser_Upsert
            @FirstName = '" + user.FirstName +
            "', @LastName = '" + user.LastName +
            "', @Email = '" + user.Email +
            "', @Gender = '" + user.Gender +
            "', @JobTitle = '" + user.JobTitle +
            "', @Department = '" + user.Department +
            "', @Salary = " + user.Salary.ToString(CultureInfo.InvariantCulture) +
            ", @Active = '" + user.Active +
            "', @UserId = " + user.UserId;
      //Console.WriteLine(sql);

      //jalankan sql
      bool success = _dapper.ExecuteSql(sql);
      if (success)
      {
         return Ok();
      }
      throw new Exception("Failed to Update User");
   }



   [HttpDelete("DeleteUser/{userId}")]
   public IActionResult DeleteUser(int userId)
   {

      string sql = @"
         TutorialAppSchema.sp_User_Delete
         @UserId = " + userId.ToString();

      bool success = _dapper.ExecuteSql(sql);
      if (success)
      {
         return Ok();
      }
      throw new Exception("Failed to Delete User");

   }


}



