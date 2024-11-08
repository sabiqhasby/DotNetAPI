using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;


[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
   private readonly DataContextDapper _dapper;
   public UserController(IConfiguration config)
   {
      _dapper = new DataContextDapper(config);
   }

   [HttpGet("TestConnection")]
   public DateTime TestConnection()
   {
      return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
   }


   [HttpGet("GetUsers")]
   //public IActionResult Test()
   public IEnumerable<User> GetUsers()
   {
      //string[] responseArray = new string[]
      //{
      //   "test1",
      //   "test2",
      //};
      //return responseArray;

      string sql = @"
         SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
         FROM TutorialAppSchema.Users";

      //LOAD DATA USERS
      IEnumerable<User> users = _dapper.LoadData<User>(sql);
      return users;
   }

   [HttpGet("GetSingleUser/{userId}")]
   public User GetSingleUser(int userId)
   {
      string sql = @"
         SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
         FROM TutorialAppSchema.Users
         WHERE UserId = " + userId.ToString();

      User user = _dapper.LoadDataSingle<User>(sql);
      return user;
   }

   [HttpPut("EditUser")]
   public async Task<IActionResult> EditUser(User user)
   {
      //string sql = @"
      //   UPDATE TutorialAppSchema.Users
      //         SET [FirstName] = '" + user.FirstName +
      //         "', [LastName] = '" + user.LastName +
      //         "', [Email] = '" + user.Email +
      //         "', [Gender] = '" + user.Gender +
      //         "', [Active] = '" + user.Active +
      //   "' WHERE UserId = " + user.UserId;

      string sql = @"
         UPDATE TutorialAppSchema.Users
         SET 
            [FirstName] = @FirstName,
            [LastName] = @LastName,
            [Email] = @Email,
            [Gender] = @Gender,
            [Active] = @Active
         WHERE 
            UserId = @UserId";
      var parameters = new
      {
         user.FirstName,
         user.LastName,
         user.Email,
         user.Gender,
         user.Active,
         user.UserId
      };
      //jalankan sql
      bool success = await _dapper.ExecuteSqlAsync(sql, parameters);
      if (success)
      {
         return Ok();
      }
      throw new Exception("Failed to Update User");
   }

   [HttpPost("AddUser")]
   public async Task<IActionResult> AddUser(UserToAddDto user)
   {
      string sql = @"
         INSERT INTO TutorialAppSchema.Users(
             [FirstName],
             [LastName],
             [Email],
             [Gender],
             [Active] 
         ) VALUES (
             @FirstName,
             @LastName,
             @Email,
             @Gender,
             @Active 
         )
      ";
      var parameters = new
      {
         user.FirstName,
         user.LastName,
         user.Email,
         user.Gender,
         user.Active,
      };
      bool success = await _dapper.ExecuteSqlAsync(sql, parameters);
      if (success)
      {
         return Ok();
      }
      throw new Exception("Failed to Add User");
   }

   [HttpDelete("DeleteUser/{userId}")]
   public IActionResult DeleteUser(int userId)
   {

      string sql = @"
         DELETE FROM TutorialAppSchema.Users
         WHERE UserId = " + userId.ToString();

      bool success = _dapper.ExecuteSql(sql);
      if (success)
      {
         return Ok();
      }
      throw new Exception("Failed to Delete User");

   }

}



