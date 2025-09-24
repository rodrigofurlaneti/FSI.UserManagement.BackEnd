namespace Domain.Configuration
{
    public class DevelopmentUserOptions
    {
        public const string DevelopmentUser = "DevelopmentUser";
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
