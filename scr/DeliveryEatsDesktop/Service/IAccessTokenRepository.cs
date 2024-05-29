namespace desktop.Service
{
    public interface IAccessTokenRepository
    {
        public string? GetAccessToken();
        public void AddAccessToken(string token);
        public void UpdateAccessToken(string token);
        public void DeleteAccessToken();
    }
}
