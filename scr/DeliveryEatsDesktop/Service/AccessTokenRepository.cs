namespace desktop.Service
{
    public class AccessTokenRepository : IAccessTokenRepository
    {
        private string _token;

        public void AddAccessToken(string token)
        {
            _token = token;
        }

        public void DeleteAccessToken()
        {
            _token = null;
        }

        public string? GetAccessToken() => _token;

        public void UpdateAccessToken(string token)
        {
            _token = token;
        }
    }
}
