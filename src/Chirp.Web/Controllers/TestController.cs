using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Chirp.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string Get(string userInput)
        {
            // ❌ INTENTIONALLY VULNERABLE (SQL Injection)
            string query = "SELECT * FROM Users WHERE Name = '" + userInput + "'";

            using (var connection = new SqlConnection("Server=localhost;Database=Test;User Id=sa;Password=Password123;"))
            {
                connection.Open();
                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();
            }

            return "Done";
        }
    }
}