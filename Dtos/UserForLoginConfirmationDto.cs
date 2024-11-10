namespace DotnetAPI.Dtos
{
    public partial class UserForLoginConfirmationDto
    {
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];

    }
}
