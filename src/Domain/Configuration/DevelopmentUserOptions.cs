namespace Domain.Configuration
{
    public class DevelopmentUserOptions
    {
        public const string DevelopmentUser = "DevelopmentUser"; 
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
