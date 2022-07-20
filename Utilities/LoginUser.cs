namespace JwtAPi.Utilities
{
    public class LoginUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class PostRefreshToken
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
