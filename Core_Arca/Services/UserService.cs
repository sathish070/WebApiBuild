using Core_Arca.Data;
using Core_Arca.Helpers;
using Core_Arca.Services.Interface;

namespace ScantronInterfaceBuild.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Authenticate(string username, string password)
        {
            var users = GetUsersForBuildType();
            string encryptedPassword = EncryptionHelper.Encrypt(password);
            var user =users.SingleOrDefault(x => x.username == username && x.password == encryptedPassword);
            return user != null;
        }

        private IEnumerable<User> GetUsersForBuildType()
        {
            return _configuration.GetSection($"Credentials").Get<List<User>>();
        }
    }
}
