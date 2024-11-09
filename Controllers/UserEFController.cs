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
    //private readonly DataContextEF _entityFramework;
    private readonly IUserRepository _userRepository;
    private readonly Mapper _mapper;
    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        //_entityFramework = new DataContextEF(config);

        _userRepository = userRepository;

        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserToAddDto, User>();
            cfg.CreateMap<UserSalary, UserSalary>().ReverseMap();
            cfg.CreateMap<UserJobInfo, UserJobInfo>().ReverseMap();
        }));
        _userRepository = userRepository;
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        //LOAD DATA USERS
        IEnumerable<User> users = _userRepository.GetUsers();
        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        //User? user = _entityFramework.Users.Where(u => u.UserId == userId)
        //   .FirstOrDefault();

        //if (user != null)
        //{
        //    return user;
        //}
        //throw new Exception("Failed to Get User");

        return _userRepository.GetSingleUser(userId);

    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        //User? userDb = _entityFramework.Users.Where(u => u.UserId == user.UserId)
        //   .FirstOrDefault();
        User? userDb = _userRepository.GetSingleUser(user.UserId);

        if (userDb != null)
        {
            userDb.Active = user.Active;
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Gender = user.Gender;

            //bool success = _entityFramework.SaveChanges() > 0;
            bool success = _userRepository.SaveChanges();
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

        //_entityFramework.Add(userDb);
        _userRepository.AddEntity(userDb);

        //bool success = _entityFramework.SaveChanges() > 0;
        bool success = _userRepository.SaveChanges();
        if (success)
        {
            return Ok();
        }
        throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        //User? userDb = _entityFramework.Users
        //   .Where(u => u.UserId == userId)
        //   .FirstOrDefault();

        User? userDb = _userRepository.GetSingleUser(userId);

        if (userDb != null)
        {
            //_entityFramework.Users.Remove(userDb);
            _userRepository.RemoveEntity(userDb);
            //bool success = _entityFramework.SaveChanges() > 0;
            bool success = _userRepository.SaveChanges();
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
        //IEnumerable<UserSalary> userSalaries = _entityFramework.UserSalary.ToList();
        IEnumerable<UserSalary> userSalaries = _userRepository.GetSalaries();
        return userSalaries;
    }

    [HttpGet("UserSalary/{userId}")]
    public UserSalary GetSingleUserSalary(int userId)
    {
        //UserSalary? userSalary = _entityFramework.UserSalary.Where(u => u.UserId == userId)
        //   .FirstOrDefault();
        //if (userSalary != null)
        //{
        //    return userSalary;
        //}
        //throw new Exception("Failed to get UserSalary");
        return _userRepository.GetSingleSalary(userId);
    }

    [HttpPut("UserSalary")]
    public IActionResult UpdateUserSalary(UserSalary userSalary)
    {
        //UserSalary? dbUserSalary = _entityFramework.UserSalary
        //    .Where(u => u.UserId == userSalary.UserId)
        //    .FirstOrDefault();

        //UserSalary? dbUserSalary = _entityFramework.UserSalary
        //    .SingleOrDefault(u => u.UserId == userSalary.UserId);

        UserSalary? dbUserSalary = _userRepository.GetSingleSalary(userSalary.UserId);

        if (dbUserSalary == null)
        {
            return NotFound("UserSalary not found.");
        }


        //dbUserSalary.Salary = userSalary.Salary;
        _mapper.Map(userSalary, dbUserSalary);

        bool success = _userRepository.SaveChanges();
        if (success)
        {
            return Ok();
        }
        throw new Exception("Failed to Update UserSalary");


    }

    [HttpPost("UserSalary")]
    public IActionResult AddUserSalary(UserSalary userSalary)
    {
        //_entityFramework.Add(userSalary);
        _userRepository.AddEntity(userSalary);
        bool success = _userRepository.SaveChanges();
        if (success)
        {
            return Ok();
        }
        throw new Exception("Failed to Insert UserSalary");
    }

    [HttpDelete("UserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        //UserSalary? userSalary = _entityFramework.UserSalary
        //    .Where(u => u.UserId == userId)
        //    .FirstOrDefault(); 

        UserSalary? userSalary = _userRepository.GetSingleSalary(userId);
        if (userSalary != null)
        {
            //_entityFramework.Remove(userSalary);
            _userRepository.RemoveEntity(userSalary);
            bool success = _userRepository.SaveChanges();
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
        //IEnumerable<UserJobInfo> userJobInfos = _entityFramework.UserJobInfo.ToList();
        IEnumerable<UserJobInfo> userJobInfos = _userRepository.GetJobInfos();
        return userJobInfos;
    }

    [HttpGet("UserJobInfo/{userId}")]
    public UserJobInfo GetUserJobsSingle(int userId)
    {
        //UserJobInfo? dbUserJobInfo = _entityFramework.UserJobInfo
        //    .Where(u => u.UserId == userId)
        //    .FirstOrDefault(); 
        //if (dbUserJobInfo != null)
        //{
        //    return dbUserJobInfo;
        //}
        //throw new Exception("Failed to Get UserJobInfo");

        return _userRepository.GetSingleJobInfo(userId);

    }

    [HttpPut("UserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
    {
        //UserJobInfo? userJob = _entityFramework.UserJobInfo
        //    .Where(u => u.UserId == userJobInfo.UserId)
        //    .FirstOrDefault();
        //UserJobInfo? userJob = _entityFramework.UserJobInfo.SingleOrDefault(u => u.UserId == userJobInfo.UserId);

        UserJobInfo? userJob = _userRepository.GetSingleJobInfo(userJobInfo.UserId);

        if (userJob != null)
        {
            //userJob.JobTitle = userJobInfo.JobTitle;
            //userJob.Department = userJobInfo.Department;

            _mapper.Map(userJobInfo, userJob);

            bool success = _userRepository.SaveChanges();
            if (success) { return Ok(userJob); }
            throw new Exception("Failed to Edit UserJobInfo");
        }
        throw new Exception("Failed to Edit UserJobInfo");
    }

    [HttpPost("UserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
    {
        //_entityFramework.Add(userJobInfo);
        _userRepository.AddEntity(userJobInfo);
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
        //UserJobInfo? userJob = _entityFramework.UserJobInfo.SingleOrDefault(u => u.UserId == userId);

        UserJobInfo? userJob = _userRepository.GetSingleJobInfo(userId);

        if (userJob != null)
        {
            //_entityFramework.Remove(userJob);
            _userRepository.RemoveEntity(userJob);
            bool success = _userRepository.SaveChanges();
            if (success) { return Ok(); }
            throw new Exception("Failed to Delete UserJobInfo");
        }

        throw new Exception("Deleting...Something when wrong in UserJobInfo!");

    }



}
