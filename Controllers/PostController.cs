using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
   [Authorize]
   [ApiController]
   [Route("[controller]")]
   public class PostController : ControllerBase
   {
      private readonly DataContextDapper _dapper;
      public PostController(IConfiguration config)
      {
         _dapper = new DataContextDapper(config);
      }

      [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
      public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
      {
         string sql = @"
            EXEC TutorialAppSchema.spPosts_Get";
         string parameters = "";

         if (postId != 0)
         {
            parameters += ", @PostId=" + postId.ToString();
         }
         if (userId != 0)
         {
            parameters += ", @UserId=" + userId.ToString();
         }
         if (searchParam.ToLower() != "none")
         {
            parameters += ", @SearchValue='" + searchParam.ToString() + "'";
         }

         if (parameters.Length > 0)
         {
            sql += parameters.Substring(1); //hapus koma di awal
         }
         Console.WriteLine(sql);
         return _dapper.LoadData<Post>(sql);
      }

      [HttpGet("MyPosts")]
      public ActionResult<IEnumerable<Post>> GetMyPost()
      {
         string sql = @"
        EXEC TutorialAppSchema.spPosts_Get
        @UserId = " + this.User.FindFirst("userId")?.Value;

         IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);

         if (posts.Any())
         {
            return Ok(posts); // Kembalikan hasil post jika ada
         }

         return NotFound(new
         {
            Message = "Post belum dibuat"
         });
      }

      [HttpPut("UpsertPost")]
      public IActionResult UpsertPost(Post postToUpsert)
      {
         string sql = @"
            EXEC TutorialAppSchema.spPosts_Upsert
            @UserId =" + this.User.FindFirst("userId")?.Value +
            ", @PostTitle = '" + postToUpsert.PostTitle +
            "', @PostContent = '" + postToUpsert.PostContent + "'";

         if (postToUpsert.PostId > 0)
         {
            sql += ", @PostId = " + postToUpsert.PostId;
         }

         bool success = _dapper.ExecuteSql(sql);
         if (success)
         {
            return Ok(postToUpsert);
         }
         throw new Exception("Failed to upsert Post!");
      }


      [HttpDelete("Post/{postId}")]
      public IActionResult DeletePost(int postId)
      {
         string sql = @"
            EXEC TutorialAppSchema.spPosts_Delete 
            @PostId = " + postId +
            ", @UserId = " + this.User.FindFirst("userId")?.Value;

         bool success = _dapper.ExecuteSql(sql);
         if (success)
         {
            return Ok("deleted");
         }
         throw new Exception("Failed to Delete Post!");
      }
   }
}