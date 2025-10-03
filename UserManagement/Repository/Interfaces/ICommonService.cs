namespace UserManagement.Repository.Interfaces
{
    public interface ICommonService
    {
        public Task<string> HashPasswordAsync(string password);
    }
}
