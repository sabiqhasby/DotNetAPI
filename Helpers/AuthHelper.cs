using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DotnetAPI.Helpers
{
   public class AuthHelper
   {
      private readonly IConfiguration _config;
      private readonly DataContextDapper _dapper;

      public AuthHelper(IConfiguration config)
      {
         _config = config;
         _dapper = new DataContextDapper(config);

      }


      public byte[] GetPasswordHash(string password, byte[] passwordSalt)
      {
         string passwordSaltPlusString = _config.GetSection("AppSettings: PasswordKey").Value +
             Convert.ToBase64String(passwordSalt);

         return KeyDerivation.Pbkdf2(
                 password: password,
                 salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                 prf: KeyDerivationPrf.HMACSHA256,
                 iterationCount: 10000,
                 numBytesRequested: 256 / 8
             );
      }

      public string CreateToken(int userId)
      {
         //buat claim
         Claim[] claims = new Claim[] {
            new Claim("userId", userId.ToString())
         };

         //ambil secret token dari appsetting
         string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

         //convert tokenKeyString ke SymetricSecurityKey
         SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                 Encoding.UTF8.GetBytes(
                     tokenKeyString != null ? tokenKeyString : ""
                 )
             );

         //buat tandatangan credential menggunakan Hmac512
         SigningCredentials credentials = new SigningCredentials(
            tokenKey,
            SecurityAlgorithms.HmacSha512Signature
          );

         //buat isi token
         SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
         {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.Now.AddDays(1) //24 hours
         };


         JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

         SecurityToken token = tokenHandler.CreateToken(descriptor);

         return tokenHandler.WriteToken(token);

      }

      public bool setPassword(UserForLoginDto userForSetPassword)
      {

         byte[] passwordSalt = new byte[128 / 8];
         using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
         {
            rng.GetNonZeroBytes(passwordSalt);
         }

         byte[] passwordHash = GetPasswordHash(userForSetPassword.Password, passwordSalt);
         string sqlAddAuth = @"
                        INSERT INTO TutorialAppSchema.Auth (
                            [Email],
                            [PasswordHash],
                            [PasswordSalt]
                        ) VALUES (
                            '" + userForSetPassword.Email +
                 "', @PasswordHash, @PasswordSalt)";

         List<SqlParameter> sqlParameters = new List<SqlParameter>();

         SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
         passwordSaltParameter.Value = passwordSalt;

         SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
         passwordHashParameter.Value = passwordHash;

         sqlParameters.Add(passwordSaltParameter);
         sqlParameters.Add(passwordHashParameter);

         return _dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters);


      }


   }
}