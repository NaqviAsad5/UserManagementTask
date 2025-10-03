using System.Security.Cryptography;
using System.Text;
using UserManagement.Repository.Interfaces;

namespace UserManagement.Repository.Services
{
    public class CommonService : ICommonService
    {
        public async Task<string> HashPasswordAsync(string password)
        {
            return await Task.Run(() =>
            {
                using (var hmac = new HMACSHA256())
                {
                    var passwordBytes = Encoding.UTF8.GetBytes(password);
                    var hashedBytes = hmac.ComputeHash(passwordBytes);
                    return Convert.ToBase64String(hashedBytes);
                }
            });
        }
    }

}
