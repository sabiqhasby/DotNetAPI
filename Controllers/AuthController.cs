using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DotnetAPI.Controllers
{
   [Authorize]
   [ApiController]
   [Route("[controller]")]
   public class AuthController : ControllerBase
   {

      private readonly DataContextDapper _dapper;
      //private readonly IConfiguration _config;

      private readonly AuthHelper _authHelper;

      public AuthController(IConfiguration config)
      {
         _dapper = new DataContextDapper(config);
         //_config = config;
         _authHelper = new AuthHelper(config);
      }

      [AllowAnonymous]
      [HttpPost("Register")]
      public IActionResult Register(UserForRegistrationDto userRegistration)
      {
         if (userRegistration.Password == userRegistration.PasswordConfirm)
         {
            string sqlCheckUserExist = @"
                    SELECT * FROM TutorialAppSchema.Auth WHERE Email = '" +
                userRegistration.Email + "'";

            IEnumerable<string> existUser = _dapper.LoadData<string>(sqlCheckUserExist);

            //Buat jika user belum terdaftar
            //passwordSalt dibuat menggunakan RandomNumberGenerator untuk menghasilkan byte acak.
            //Salt ini akan digabungkan dengan password sebelum dilakukan hashing.
            if (!existUser.Any())
            {
               UserForLoginDto userForSetPassword = new UserForLoginDto()
               {
                  Email = userRegistration.Email,
                  Password = userRegistration.Password,
               };
               bool success = _authHelper.setPassword(userForSetPassword);
               if (success)
               {
                  string sqlAddUser = @"
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
                             @Gender, 1
                         )";

                  var parameters = new
                  {

                     userRegistration.Email,
                     userRegistration.Gender
                  };

                  List<SqlParameter> sqlUserParameters = new List<SqlParameter>();

                  SqlParameter firstNameParameter = new SqlParameter("@FirstName", SqlDbType.NVarChar);
                  firstNameParameter.Value = userRegistration.FirstName;

                  SqlParameter lastNameParameter = new SqlParameter("@LastName", SqlDbType.NVarChar);
                  lastNameParameter.Value = userRegistration.LastName;

                  SqlParameter emailParameter = new SqlParameter("@Email", SqlDbType.NVarChar);
                  emailParameter.Value = userRegistration.Email;

                  SqlParameter genderParameter = new SqlParameter("@Gender", SqlDbType.NVarChar);
                  genderParameter.Value = userRegistration.Gender;

                  sqlUserParameters.Add(firstNameParameter);
                  sqlUserParameters.Add(lastNameParameter);
                  sqlUserParameters.Add(emailParameter);
                  sqlUserParameters.Add(genderParameter);


                  if (_dapper.ExecuteSqlWithParameters(sqlAddUser, sqlUserParameters))
                  {

                     return Ok();
                  }
                  throw new Exception("Failed to add user");

               }
               throw new Exception("Failed to register user");

            }
            throw new Exception("User with this email already exists!");
         }
         return BadRequest("Passwords do not match!");
      }


      [HttpPut("ResetPassword")]
      public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
      {
         if (_authHelper.setPassword(userForSetPassword))
         {
            return Ok();
         }
         throw new Exception("Failed to update Password!");
      }

      [AllowAnonymous]
      [HttpPost("Login")]
      public async Task<IActionResult> Login(UserForLoginDto userLogin)
      {
         string sqlForHashAndSalt = @"
               SELECT
                [PasswordHash],
                [PasswordSalt] 
               FROM TutorialAppSchema.Auth WHERE Email = @Email";

         //UserForLoginConfirmationDto userForLoginConfirm = _dapper
         //    .LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

         var parameters = new
         {
            userLogin.Email,
         };

         UserForLoginConfirmationDto? userForLoginConfirm = await _dapper
             .LoadDataSingleAsync<UserForLoginConfirmationDto>(sqlForHashAndSalt, parameters);

         if (userForLoginConfirm == null)
         {
            return NotFound("User with email does not exist");
         }

         byte[] passwordHash = _authHelper.GetPasswordHash(userLogin.Password, userForLoginConfirm.PasswordSalt);

         //if (passwordHash == userForLoginConfirm.PasswordHash) { } // WONT WORK, karena memori tersimpan di berbeda tempat

         //for (int i = 0; i < passwordHash.Length; i++)
         //{
         //    if (passwordHash[i] != userForLoginConfirm.PasswordHash[i])
         //    {
         //        return StatusCode(401, "Incorect Passsword!");
         //    }
         //}

         //BANDINGKAN JIKA TIDAK SAMA MAKA...401
         if (!passwordHash.SequenceEqual(userForLoginConfirm.PasswordHash))
         {
            return StatusCode(401, "Incorrect Password!");
         }

         string userIdSql = "SELECT * FROM TutorialAppSchema.Users WHERE Email = @Email";

         int userId = await _dapper.LoadDataSingleAsync<int>(userIdSql, new { userLogin.Email });

         return Ok(new Dictionary<string, string>{
            { "token", _authHelper.CreateToken(userId)}
         });
      }

      [HttpGet("RefreshToken")]
      public async Task<IActionResult> RefreshToken()
      {
         string userId = User.FindFirst("userId")?.Value + "";

         string userIdSql = "SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = @UserId";

         int userIdFromDb = await _dapper.LoadDataSingleAsync<int>(userIdSql, new { userId });

         return Ok(new Dictionary<string, string>{
            { "token", _authHelper.CreateToken(userIdFromDb)}
         });
      }


   }
}