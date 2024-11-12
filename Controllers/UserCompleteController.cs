using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

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

   [HttpPut("EditUser")]
   public async Task<IActionResult> EditUser(User user)
   {
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

   [HttpGet("UserSalary")]
   public IEnumerable<UserSalary> GetUserSalary()
   {
      string sql = @"
         SELECT 
            [UserId],
            [Salary],
            [AvgSalary]
         FROM TutorialAppSchema.UserSalary";

      IEnumerable<UserSalary> userSalary = _dapper.LoadData<UserSalary>(sql);
      return userSalary;
   }


   [HttpPut("UserSalary")]
   public async Task<IActionResult> EditUserSalary(UserSalary userSalary)
   {
      string sql = @"
         UPDATE TutorialAppSchema.UserSalary
         SET 
            [Salary] = @Salary
         WHERE 
            UserSalary.UserId = @UserId";

      var parameters = new
      {
         userSalary.Salary,
         userSalary.UserId
      };
      //jalankan sql
      bool success = await _dapper.ExecuteSqlAsync(sql, parameters);
      if (success)
      {
         return Ok();
      }
      throw new Exception("Failed to Update UserSalary");
   }

   [HttpPost("UserSalary")]
   public async Task<IActionResult> AddUserSalary(UserSalary userSalary)
   {
      string sql = @"
         INSERT INTO TutorialAppSchema.UserSalary(
             UserId,
             Salary
         ) VALUES (
             @UserId,
             @Salary
         )";

      var parameters = new
      {
         userSalary.UserId,
         userSalary.Salary,
      };
      //jalankan sql
      bool success = await _dapper.ExecuteSqlWithRowCountAsync(sql, parameters) > 0;
      if (success)
      {
         return Ok(userSalary);
      }
      throw new Exception("Failed to Insert UserSalary");
   }

   [HttpDelete("UserSalary/{userId}")]
   public IActionResult deleteUserSalaryByUserId(int userId)
   {
      string sql = "DELETE FROM TutorialAppSchema.UserSalary WHERE UserId=" + userId.ToString();

      if (_dapper.ExecuteSql(sql))
      {
         return Ok();
      }
      throw new Exception("Failed to Delete UserSalary");
   }


   [HttpGet("UserJobInfo/{userId}")]
   public UserJobInfo GetSingleUserJobInfo(int userId)
   {
      string sql = @"
         SELECT [UserId],
               [JobTitle],
               [Department] 
         FROM TutorialAppSchema.UserJobInfo
         WHERE UserJobInfo.UserId = " + userId.ToString();
      UserJobInfo userJobInfoSingle = _dapper.LoadDataSingle<UserJobInfo>(sql);
      return userJobInfoSingle;
   }

   [HttpPut("UserJobInfo")]
   public async Task<IActionResult> UpdateUserJobInfo(UserJobInfo jobInfo)
   {
      string sql = @"
         UPDATE TutorialAppSchema.UserJobInfo
            SET [JobTitle] = @JobTitle,
               [Department] = @Department
            WHERE UserJobInfo.UserId = @UserId
      ";

      var parameters = new
      {
         jobInfo.UserId,
         jobInfo.JobTitle,
         jobInfo.Department
      };
      bool success = await _dapper.ExecuteSqlAsync(sql, parameters);
      if (success)
      {
         return Ok(jobInfo);
      }
      throw new Exception("Failed to Update UserSalary");
   }

   [HttpPost("UserJobInfo")]
   public async Task<IActionResult> AddUserJobInfo(UserJobInfo jobInfo)
   {
      string sql = @"
         INSERT INTO TutorialAppSchema.UserJobInfo(
         [UserId],
         [JobTitle],
         [Department]
         ) VALUES (@UserId, @JobTitle, @Department)
      ";

      var parameters = new
      {
         jobInfo.UserId,
         jobInfo.JobTitle,
         jobInfo.Department
      };

      bool success = await _dapper.ExecuteSqlAsync(sql, parameters);
      if (success)
      {
         return Ok(jobInfo);
      }
      throw new Exception("Failed to Add UserJobInfo");
   }

   [HttpDelete("UserJobInfo/{userId}")]
   public IActionResult DeleteUserJobInfo(int userId)
   {
      string sql = "DELETE FROM TutorialAppSchema.UserJobInfo WHERE UserJobInfo.UserId = " + userId;

      bool success = _dapper.ExecuteSql(sql);
      if (success)
      {
         return Ok();
      }
      throw new Exception("Failed to Delete UserJobInfo");
   }


}



