using ProfessorAPI.Models;

namespace ProfessorAPI.Service.AdminApp
{
    public class AdminUserService
    {
        private readonly IConfiguration _configuration;
        private readonly string ADMIN_API_URL;

        public AdminUserService(IConfiguration configuration)
        {
            _configuration = configuration;
            ADMIN_API_URL = _configuration["EnvironmentVariables:ADMIN_API_URL"];
        }

        public async Task UpdateProfessor(User newValues)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{ADMIN_API_URL}/api/user/");
                    var response = await client.PutAsJsonAsync("updateUser", newValues);
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
