using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DotnetAPI.Helpers
{
   public class AuthHelper
   {
      private readonly IConfiguration _config;
      public AuthHelper(IConfiguration config)
      {
         _config = config;
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
   }
}