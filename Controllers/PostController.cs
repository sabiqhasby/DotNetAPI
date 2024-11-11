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

      [HttpGet("Posts")]
      public IEnumerable<Post> GetPosts()
      {
         string sql = @"
            SELECT 
            [PostId],
            [UserId],
            [PostTitle],
            [PostContent],
            [PostCreated],
            [PostUpdated] FROM TutorialAppSchema.Posts";

         return _dapper.LoadData<Post>(sql);
      }

      [HttpGet("Posts/{postId}")]
      public Post GetPostSingle(int postId)
      {
         string sql = @"
            SELECT 
            [PostId],
            [UserId],
            [PostTitle],
            [PostContent],
            [PostCreated],
            [PostUpdated] FROM TutorialAppSchema.Posts
               WHERE PostId = " + postId.ToString();

         return _dapper.LoadDataSingle<Post>(sql);
      }

      [HttpGet("PostByUser/{userId}")]
      public IEnumerable<Post> GetPostByUser(int userId)
      {
         string sql = @"
            SELECT 
            [PostId],
            [UserId],
            [PostTitle],
            [PostContent],
            [PostCreated],
            [PostUpdated] FROM TutorialAppSchema.Posts
               WHERE UserId = " + userId.ToString();

         return _dapper.LoadData<Post>(sql);
      }

      [HttpGet("MyPosts")]
      public IEnumerable<Post> GetMyPost()
      {
         string sql = @"
             SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                    WHERE UserId = " + this.User.FindFirst("userId")?.Value;

         return _dapper.LoadData<Post>(sql);
      }

      [HttpPost("Post")]
      public async Task<IActionResult> AddPost(PostToAddDto postToAdd)
      {
         string sql = @"
         INSERT INTO TutorialAppSchema.Posts(
             [UserId],
             [PostTitle],
             [PostContent],
             [PostCreated],
             [PostUpdated]
         ) VALUES (
            @UserId,
            @PostTitle,
            @PostContent,
            GETDATE(),
            GETDATE()
         )";

         object parameter = new
         {
            UserId = this.User.FindFirst("userId")?.Value, //ambil dari token
            postToAdd.PostTitle,
            postToAdd.PostContent
         };
         bool success = await _dapper.ExecuteSqlAsync(sql, parameter);
         if (success)
         {
            return Ok(postToAdd);
         }
         throw new Exception("Failed to create new Post!");
      }
      [HttpPut("Post")]
      public async Task<IActionResult> EditPost(PostToEditDto postToEdit)
      {
         string sql = @"
         UPDATE TutorialAppSchema.Posts
            SET 
             [PostTitle] = @PostTitle,
             [PostContent] = @PostContent,
             [PostUpdated] = GETDATE()
            WHERE PostId = @PostId
            AND UserId = @UserId
        ";

         object parameter = new
         {
            postToEdit.PostTitle,
            postToEdit.PostContent,
            postToEdit.PostId, //ambil dari token
            UserId = this.User.FindFirst("userId")?.Value, //ambil dari token
         };
         bool success = await _dapper.ExecuteSqlAsync(sql, parameter);
         if (success)
         {
            return Ok(postToEdit);
         }
         throw new Exception("Failed to Edit Post!");
      }

      [HttpDelete("Post/{postId}")]
      public async Task<IActionResult> DeletePost(int postId)
      {
         string sql = @"
            DELETE FROM TutorialAppSchema.Posts 
            WHERE PostId = @PostId
            AND UserId = @UserId";

         bool success = await _dapper.ExecuteSqlAsync(sql, new
         {
            PostId = postId,
            UserId = this.User.FindFirst("userId")?.Value, //ambil dari token
         });
         if (success)
         {
            return Ok("deleted");
         }
         throw new Exception("Failed to Delete Post!");
      }
   }
}