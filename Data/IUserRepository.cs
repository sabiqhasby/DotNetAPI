using DotnetAPI.Models;

namespace DotnetAPI.Data
{
    public interface IUserRepository
    {
        public bool SaveChanges();
        public void AddEntity<T>(T entityToAdd);
        public void RemoveEntity<T>(T entityToRemove);

        public IEnumerable<User> GetUsers();
        public IEnumerable<UserSalary> GetSalaries();
        public IEnumerable<UserJobInfo> GetJobInfos();

        public User GetSingleUser(int userId);
        public UserSalary GetSingleSalary(int userId);
        public UserJobInfo GetSingleJobInfo(int userId);




    }
}
