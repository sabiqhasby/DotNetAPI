namespace DotnetAPI.Models
{
   public partial class Post
   {
      public int PostId { get; set; }
      public int UserId { get; set; }
      public string PostTitle { get; set; } = String.Empty;
      public string PostContent { get; set; } = String.Empty;
      public DateTime PostCreated { get; set; }
      public DateTime PostUpdated { get; set; }
   }
}