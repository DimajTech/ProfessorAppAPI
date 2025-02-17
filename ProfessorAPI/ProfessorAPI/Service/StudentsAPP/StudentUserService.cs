using ProfessorAPI.DTO;

namespace ProfessorAPI.Service.StudentsAPP
{
    public class StudentUserService
    {
        private readonly IConfiguration _configuration;
        private static string STUDENT_APP_URL;
        public StudentUserService(IConfiguration configuration) { 
            _configuration = configuration;
            STUDENT_APP_URL = _configuration["EnvironmentVariables:STUDENT_APP_URL"];
        }

        public static void UpdateProfessor(UpdateProfessorRequestDTO professor)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{STUDENT_APP_URL}/api/User/");
                var putTask = client.PutAsJsonAsync("UpdateUser/", professor);
                putTask.Wait();

                var result = putTask.Result;
            }
        }
    }
}
