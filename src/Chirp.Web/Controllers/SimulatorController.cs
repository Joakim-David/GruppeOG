using Chirp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;
using System.Text.Json;

// Controllers/SimulatorController.cs
[ApiController]
[Route("")]
public class SimulatorController : ControllerBase
{
    private static int _latest = 0;
    private const string SimulatorAuth = "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh";

    private bool IsAuthorized(string authHeader)
        => authHeader == SimulatorAuth;

    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    private readonly UserManager<Author> _userManager;
    private readonly IUserStore<Author> _userStore;
    private readonly IUserEmailStore<Author> _emailStore;

    public SimulatorController(ICheepService cheepService, IAuthorService authorService, UserManager<Author> userManager, IUserStore<Author> userStore)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
    }

    [HttpGet("msgs")]
    public async Task<IActionResult> GetMessages(
        [FromHeader(Name = "Authorization")] string auth,
        [FromQuery] int no = 100,
        [FromQuery] int? latest = null)
    {
        if (!IsAuthorized(auth)) return StatusCode(403, new { status = 403, error_msg = "You are not authorized..." });

        if (latest.HasValue) _latest = latest.Value;
        
        var cheeps = await _cheepService.GetNLatestCheeps(no);
        return StatusCode(200, cheeps);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] JsonElement request, [FromQuery] int? latest = null)
    {
        if (latest.HasValue) _latest = latest.Value;

        try
        {
            string username = request.GetProperty("username").GetString()!;
            string email = request.GetProperty("email").GetString()!;
            string pwd = request.GetProperty("pwd").GetString()!;
            
            var user = CreateUser();
            await _userStore.SetUserNameAsync(user, username, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, email, CancellationToken.None);
            
            var result = await _userManager.CreateAsync(user, pwd);
            
            if (!result.Succeeded)
            {
                var usernameTakenError = result.Errors.FirstOrDefault(e => e.Code == "DuplicateUserName");
                if (usernameTakenError != null)
                {
                    return StatusCode(400, new { status = 400, error_msg = "Username already taken" });
                }
                
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return StatusCode(400, new { status = 400, error_msg = errors });
            }
            
            return StatusCode(204);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Exception: {ex}");
            return StatusCode(500, new { status = 500, error_msg = ex.Message });
        }
    }


    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest()
    {
       return Ok(new { latest = _latest });
    }

    [HttpGet("msgs/{username}")]
    [HttpPost("msgs/{username}")]
    public async Task<IActionResult> MessagesPerUser(
        string username, 
        [FromHeader(Name = "authorization")] string auth,  
        [FromQuery] int no = 100,
        [FromQuery] int? latest = null)
    {
        if (!IsAuthorized(auth)) return StatusCode(403, new { status = 403, error_msg = "You are not authorized..." });
        if (latest.HasValue) _latest = latest.Value;
        if (Request.Method == "GET")
        {
            // Check if the user from argument exists
            var author = await _authorService.GetAuthorByName(username);
            if (author == null) 
                return StatusCode(404, new { status = 404, error_msg = "Author not found." });
            
            // Get cheeps from the user - reuse the existing method from cheepservice
            var cheeps = await _cheepService.GetUserTimelineCheeps(username, username, 1);

            var filteredMsgs = cheeps.Take(no).Select(c => new
            {
                content = c.Text,
                pub_date = c.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"),
                user = c.Author.Name
            });
            return Ok(filteredMsgs);
        }
        else
        {
            try
            {
                var requestData = await JsonSerializer.DeserializeAsync<JsonElement>(Request.Body);
                string content = requestData.GetProperty("content").GetString();

                await _cheepService.CreateCheepForUser(username, content);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, error_msg = ex.Message });
            }
        }
    }

    private Author CreateUser()
    {
        try
        {
            return Activator.CreateInstance<Author>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(Author)}'. " +
                $"Ensure that '{nameof(Author)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }

    private IUserEmailStore<Author> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<Author>)_userStore;
    }

}