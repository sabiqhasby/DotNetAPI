using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContextEF _entityFramework;

        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Add(entityToAdd);
            }
        }

        public void RemoveEntity<T>(T entityToRemove)
        {
            if (entityToRemove != null)
            {
                _entityFramework.Remove(entityToRemove);
            }
        }
        //public IEnumerable<User> GetUsers()
        //{
        //    //LOAD DATA USERS
        //    IEnumerable<User> users = _entityFramework.Users.ToList();
        //    return users;
        //} 
        
        public async Task<IEnumerable<User>> GetUsers()
        {
            //LOAD DATA USERS
            return await _entityFramework.Users.ToListAsync();
        }

        public IEnumerable<UserSalary> GetSalaries()
        {
            //LOAD DATA USERS
            IEnumerable<UserSalary> userSalaries = _entityFramework.UserSalary.ToList();
            return userSalaries;
        }

        public IEnumerable<UserJobInfo> GetJobInfos()
        {
            //LOAD DATA USERS
            IEnumerable<UserJobInfo> userJobInfos = _entityFramework.UserJobInfo.ToList();
            return userJobInfos;
        }

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
        public UserSalary GetSingleSalary(int userId)
        {
            UserSalary? userSalary = _entityFramework.UserSalary.Where(u => u.UserId == userId)
               .FirstOrDefault();

            if (userSalary != null)
            {
                return userSalary;
            }
            throw new Exception("Failed to Get User");

        }
        public UserJobInfo GetSingleJobInfo(int userId)
        {
            UserJobInfo? userJobInfo = _entityFramework.UserJobInfo.Where(u => u.UserId == userId)
               .FirstOrDefault();

            if (userJobInfo != null)
            {
                return userJobInfo;
            }
            throw new Exception("Failed to Get User");

        }


    }
}
