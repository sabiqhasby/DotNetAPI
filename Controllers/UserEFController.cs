using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;


[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
   private readonly DataContextEF _entityFramework;
   private readonly Mapper _mapper;
   public UserEFController(IConfiguration config)
   {
      _entityFramework = new DataContextEF(config);

      _mapper = new Mapper(new MapperConfiguration(cfg =>
      {
         cfg.CreateMap<UserToAddDto, User>();
      }));
   }

   [HttpGet("GetUsers")]
   public IEnumerable<User> GetUsers()
   {
      //LOAD DATA USERS
      IEnumerable<User> users = _entityFramework.Users.ToList();
      return users;
   }

   [HttpGet("GetSingleUser/{userId}")]
   public User GetSingleUser(int userId)
   {
      User? user = _entityFramework.Users.Where(u => u.UserId == userId)
         .FirstOrDefault();

      if (user != null)
      {
         return user;
      }
      throw new Exception("Failed to Get User");

   }

   [HttpPut("EditUser")]
   public IActionResult EditUser(User user)
   {
      User? userDb = _entityFramework.Users.Where(u => u.UserId == user.UserId)
         .FirstOrDefault();

      if (userDb != null)
      {
         userDb.Active = user.Active;
         userDb.FirstName = user.FirstName;
         userDb.LastName = user.LastName;
         userDb.Email = user.Email;
         userDb.Gender = user.Gender;

         bool success = _entityFramework.SaveChanges() > 0;
         if (success)
         {
            return Ok();
         }
         throw new Exception("Failed to Update User");
      }
      throw new Exception("Failed to Get User");
   }

   [HttpPost("AddUser")]
   public IActionResult AddUser(UserToAddDto user)
   {
      //Validation
      if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName) || string.IsNullOrWhiteSpace(user.Email))
      {
         return BadRequest("FirstName, LastName, and Email are required.");
      }

      User userDb = _mapper.Map<User>(user);

      _entityFramework.Add(userDb);
      bool success = _entityFramework.SaveChanges() > 0;
      if (success)
      {
         return Ok();
      }
      throw new Exception("Failed to Add User");


   }

   [HttpDelete("DeleteUser/{userId}")]
   public IActionResult DeleteUser(int userId)
   {
      User? userDb = _entityFramework.Users
         .Where(u => u.UserId == userId)
         .FirstOrDefault();

      if (userDb != null)
      {
         _entityFramework.Users.Remove(userDb);
         bool success = _entityFramework.SaveChanges() > 0;
         if (success)
         {
            return Ok();
         }
         throw new Exception("Failed to Delete User");

      }
      throw new Exception("Failed to Get User");
   }

}



