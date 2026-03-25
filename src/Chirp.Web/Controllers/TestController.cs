using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Chirp.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string Get(string userInput)
        {
            // INTENTIONALLY VULNERABLE - Command Injection
            Process.Start("bash", "-c \"echo " + userInput + "\"");

            return "Hello";
        }
    }
}