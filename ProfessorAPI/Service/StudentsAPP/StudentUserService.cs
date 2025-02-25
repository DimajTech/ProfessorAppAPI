using ProfessorAPI.DTO;
using ProfessorAPI.Models;
using System.Text.Json;

namespace ProfessorAPI.Service.StudentsAPP
{
    public class StudentUserService
    {
        private readonly IConfiguration _configuration;
        private readonly string STUDENT_APP_URL;
        public StudentUserService(IConfiguration configuration) { 
            _configuration = configuration;
            STUDENT_APP_URL = _configuration["EnvironmentVariables:STUDENT_APP_URL"];
        }

        public async Task UpdateProfessor(User newValues)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{STUDENT_APP_URL}/User/");
                    var response = await client.PutAsJsonAsync("UpdateProfessor", newValues);
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
