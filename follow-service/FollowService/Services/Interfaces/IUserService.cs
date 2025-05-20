namespace Services.Interfaces
{
    public interface IUserService
    {
        Task AddUser(string id);
        Task DeleteUser(string id);
    }
}
