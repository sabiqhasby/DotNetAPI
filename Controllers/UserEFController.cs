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
            cfg.CreateMap<UserSalary, UserSalary>().ReverseMap();
            cfg.CreateMap<UserJobInfo, UserJobInfo>().ReverseMap();
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

    [HttpGet("UserSalary")]
    public IEnumerable<UserSalary> GetUserSalary()
    {
        IEnumerable<UserSalary> userSalaries = _entityFramework.UserSalary.ToList();
        return userSalaries;
    }

    [HttpGet("UserSalary/{userId}")]
    public UserSalary GetSingleUserSalary(int userId)
    {
        UserSalary? userSalary = _entityFramework.UserSalary.Where(u => u.UserId == userId)
           .FirstOrDefault();
        if (userSalary != null)
        {
            return userSalary;
        }
        throw new Exception("Failed to get UserSalary");
    }

    [HttpPut("UserSalary")]
    public IActionResult UpdateUserSalary(UserSalary userSalary)
    {
        //UserSalary? dbUserSalary = _entityFramework.UserSalary
        //    .Where(u => u.UserId == userSalary.UserId)
        //    .FirstOrDefault();
        UserSalary? dbUserSalary = _entityFramework.UserSalary
            .SingleOrDefault(u => u.UserId == userSalary.UserId);

        if (dbUserSalary == null)
        {
            return NotFound("UserSalary not found.");
        }


        //dbUserSalary.Salary = userSalary.Salary;
        _mapper.Map(userSalary, dbUserSalary);

        bool success = _entityFramework.SaveChanges() > 0;
        if (success)
        {
            return Ok();
        }
        throw new Exception("Failed to Update UserSalary");


    }

    [HttpPost("UserSalary")]
    public IActionResult AddUserSalary(UserSalary userSalary)
    {
        _entityFramework.Add(userSalary);
        bool success = _entityFramework.SaveChanges() > 0;
        if (success)
        {
            return Ok();
        }
        throw new Exception("Failed to Insert UserSalary");
    }

    [HttpDelete("UserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        UserSalary? userSalary = _entityFramework.UserSalary
            .Where(u => u.UserId == userId)
            .FirstOrDefault();
        if (userSalary != null)
        {
            _entityFramework.Remove(userSalary);
            bool success = _entityFramework.SaveChanges() > 0;
            if (success)
            {
                return Ok(userSalary);
            }
            throw new Exception("Failed to Delete UserSalary");
        }
        throw new Exception("Deleting UserSalary... Ups something went wrong!");
    }

    [HttpGet("UserJobInfo")]
    public IEnumerable<UserJobInfo> GetUserJobs()
    {
        IEnumerable<UserJobInfo> userJobInfos = _entityFramework.UserJobInfo.ToList();
        return userJobInfos;
    }

    [HttpGet("UserJobInfo/{userId}")]
    public UserJobInfo GetUserJobsSingle(int userId)
    {
        UserJobInfo? dbUserJobInfo = _entityFramework.UserJobInfo
            .Where(u => u.UserId == userId)
            .FirstOrDefault();

        if (dbUserJobInfo != null)
        {
            return dbUserJobInfo;
        }
        throw new Exception("Failed to Get UserJobInfo");
    }

    [HttpPut("UserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
    {
        //UserJobInfo? userJob = _entityFramework.UserJobInfo
        //    .Where(u => u.UserId == userJobInfo.UserId)
        //    .FirstOrDefault();
        UserJobInfo? userJob = _entityFramework.UserJobInfo.SingleOrDefault(u => u.UserId == userJobInfo.UserId);


        if (userJob != null)
        {
            //userJob.JobTitle = userJobInfo.JobTitle;
            //userJob.Department = userJobInfo.Department;

            _mapper.Map(userJobInfo, userJob);

            bool success = _entityFramework.SaveChanges() > 0;
            if (success) { return Ok(userJob); }
            throw new Exception("Failed to Edit UserJobInfo");
        }
        throw new Exception("Failed to Edit UserJobInfo");
    }

    [HttpPost("UserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
    {
        _entityFramework.Add(userJobInfo);
        bool success = _entityFramework.SaveChanges() > 0;
        if (success) { return Ok(userJobInfo); }
        throw new Exception("Failed to insert UserJobInfo");
    }

    [HttpDelete("UserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        //UserJobInfo? userJob = _entityFramework.UserJobInfo
        //    .Where(u => u.UserId == userId)
        //    .FirstOrDefault();

        //Cari terlebih dahulu, jika tidak di temukan return null.
        UserJobInfo? userJob = _entityFramework.UserJobInfo.SingleOrDefault(u => u.UserId == userId);

        if (userJob != null)
        {
            _entityFramework.Remove(userJob);
            bool success = _entityFramework.SaveChanges() > 0;
            if (success) { return Ok(); }
            throw new Exception("Failed to Delete UserJobInfo");
        }

        throw new Exception("Deleting...Something when wrong in UserJobInfo!");

    }



}
