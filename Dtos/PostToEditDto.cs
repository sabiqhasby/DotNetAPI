namespace DotnetAPI.Models
{
   public partial class PostToEditDto
   {
      public int PostId { get; set; }
      public string PostTitle { get; set; } = String.Empty;
      public string PostContent { get; set; } = String.Empty;

   }
}