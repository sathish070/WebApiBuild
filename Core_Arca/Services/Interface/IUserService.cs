namespace Core_Arca.Services.Interface
{
    public interface IUserService
    {
        bool Authenticate(string username, string password);
    }
}
