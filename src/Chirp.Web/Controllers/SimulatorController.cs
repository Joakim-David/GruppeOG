using Chirp.Services;
using Chirp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Chirp.Core;
using System.Text.Json;

/// <summary>
/// API controller for the Minitwit simulator interface.
/// Provides REST endpoints for the simulator to interact with the Chirp application,
/// including message retrieval, user registration, follow relationships, and progress tracking.
/// All endpoints (except /register and /latest) require Basic authentication.
/// </summary>
/// <remarks>
/// This controller implements the Minitwit API specification for simulator integration.
/// Authentication is handled via a hardcoded Basic auth header.
/// The controller maintains a global 'latest' counter to track simulator request sequence.
/// </remarks>
[ApiController]
[Route("")]
public class SimulatorController : ControllerBase
{
    /// <summary>
    /// Global counter tracking the most recent 'latest' value received from the simulator.
    /// Used to track simulator progress through test sequences.
    /// </summary>
    private static int _latest = 0;
    
    /// <summary>
    /// Expected Authorization header value for simulator authentication.
    /// Format: "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh"
    /// </summary>
    private const string SimulatorAuth = "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh";

    /// <summary>
    /// Validates the Authorization header against the expected simulator credentials.
    /// </summary>
    /// <param name="authHeader">The Authorization header value from the request.</param>
    /// <returns>True if the header matches the expected value, false otherwise.</returns>
    private bool IsAuthorized(string authHeader)
        => authHeader == SimulatorAuth;

    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    private readonly UserManager<Author> _userManager;
    private readonly IUserStore<Author> _userStore;
    private readonly IUserEmailStore<Author> _emailStore;

    /// <summary>
    /// Initializes a new instance of the SimulatorController.
    /// </summary>
    /// <param name="cheepService">Service for cheep (message) operations.</param>
    /// <param name="authorService">Service for author operations including follow relationships.</param>
    /// <param name="userManager">ASP.NET Core Identity UserManager for user operations.</param>
    /// <param name="userStore">ASP.NET Core Identity UserStore for user persistence.</param>
    public SimulatorController(ICheepService cheepService, IAuthorService authorService, UserManager<Author> userManager, IUserStore<Author> userStore)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
    }

    /// <summary>
    /// Retrieves recent public messages from the platform.
    /// GET /msgs
    /// </summary>
    /// <param name="auth">Authorization header (required).</param>
    /// <param name="no">Maximum number of messages to return (default: 100).</param>
    /// <param name="latest">Optional value to update the global latest counter.</param>
    /// <returns>
    /// 200 OK with array of message objects containing content, pub_date, and user.
    /// 403 Forbidden if authorization fails.
    /// </returns>
    /// <remarks>
    /// Returns messages in the format:
    /// [
    ///   {
    ///     "content": "message text",
    ///     "pub_date": "2024-02-24 14:30:00",
    ///     "user": "username"
    ///   }
    /// ]
    /// </remarks>
    [HttpGet("msgs")]
    public async Task<IActionResult> GetMessages(
        [FromHeader(Name = "Authorization")] string auth,
        [FromQuery] int no = 100,
        [FromQuery] int? latest = null)
    {
        if (!IsAuthorized(auth)) 
            return StatusCode(403, new { status = 403, error_msg = "You are not authorized to use this resource!" });
        
        if (latest.HasValue) 
            _latest = latest.Value;
        
        var cheeps = await _cheepService.GetNLatestCheeps(null, no);
        
        var response = cheeps.Select(c => new 
        {
            content = c.Text,
            pub_date = c.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"),
            user = c.Author.Name
        });
        
        return Ok(response);
    }

    [HttpGet("msgs/{username}")]
    [HttpPost("msgs/{username}")]
    public async Task<IActionResult> MessagesPerUser(
        string username, 
        [FromHeader(Name = "Authorization")] string auth,  
        [FromQuery] int no = 100,
        [FromQuery] int? latest = null)
    {
        if (!IsAuthorized(auth)) 
            return StatusCode(403, new { status = 403, error_msg = "You are not authorized to use this resource!" });
        
        if (latest.HasValue) 
            _latest = latest.Value;
        
        if (Request.Method == "GET")
        {
            try
            {
                // Get cheeps from the user
                var author = await _authorService.GetAuthorByName(username);
                if (author == null) 
                    return NotFound();
                
                var cheeps = await _cheepService.GetNLatestCheeps(username, no);
                
                var response = cheeps.Select(c => new
                {
                    content = c.Text,
                    pub_date = c.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    user = c.Author.Name
                });
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(404, ex);
            }
        }
        else // POST
        {
            try
            {
                var requestData = await JsonSerializer.DeserializeAsync<JsonElement>(Request.Body);
                string content = requestData.GetProperty("content").GetString()!;
                
                await _cheepService.CreateCheepForUser(username, content);
                
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                return StatusCode(500, new { status = 500, error_msg = ex.Message });
            }
        }
    }

    /// <summary>
    /// Registers a new user account on the platform.
    /// POST /register
    /// </summary>
    /// <param name="request">JSON object containing username, email, and pwd fields.</param>
    /// <param name="latest">Optional value to update the global latest counter.</param>
    /// <returns>
    /// 204 No Content on successful registration.
    /// 400 Bad Request if username is taken, email is invalid, or required fields are missing.
    /// 500 Internal Server Error for unexpected errors.
    /// </returns>
    /// <remarks>
    /// Expected request body format:
    /// {
    ///   "username": "string",
    ///   "email": "string",
    ///   "pwd": "string"
    /// }
    /// This endpoint does NOT require authentication.
    /// </remarks>
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

            if (result.Succeeded)
            {
                // Auto-confirm email for API registrations
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.ConfirmEmailAsync(user, token);
            }
            
            if (!result.Succeeded)
            {
                // Check if it's a "username already taken" error
                var usernameTakenError = result.Errors.FirstOrDefault(e => e.Code == "DupligcateUserName");
                if (usernameTakenError != null)
                {
                    return StatusCode(400, new { status = 400, error_msg = "Username already taken" });
                }
                
                // For other errors, return a generic message
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

    /// <summary>
    /// Retrieves the current value of the global latest counter.
    /// GET /latest
    /// </summary>
    /// <returns>
    /// 200 OK with JSON object containing the latest value.
    /// </returns>
    /// <remarks>
    /// Returns: { "latest": integer }
    /// This endpoint does NOT require authentication.
    /// Used by the simulator to track its progress through test sequences.
    /// </remarks>
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest()
    {
        return Ok(new { latest = _latest });
    }

    /// <summary>
    /// Retrieves the list of users that a given user is following.
    /// GET /fllws/{username}
    /// POST /fllws/{username}
    /// </summary>
    /// <param name="username">The username whose following list to retrieve.</param>
    /// <param name="auth">Authorization header (required).</param>
    /// <param name="no">Maximum number of follows to return (default: 100).</param>
    /// <param name="latest">Optional value to update the global latest counter.</param>
    /// <returns>
    /// 200 OK with JSON object containing array of followed usernames.
    /// 400 Bad Request if user operation fails.
    /// 403 Forbidden if authorization fails.
    /// 500 Internal Server Error for unexpected errors.
    /// </returns>
    /// <remarks>
    /// Returns format:
    /// {
    ///   "follows": ["username1", "username2", ...]
    /// }
    /// Note: This endpoint handles both GET and POST for /fllws/{username}.
    /// The POST variant for follow/unfollow actions is not yet implemented.
    /// </remarks>
    [HttpGet("fllws/{username}")]
    [HttpPost("fllws/{username}")]
    public async Task<IActionResult> Follow(
        string username,
        [FromHeader(Name = "Authorization")] string auth,
        [FromQuery] int no = 100,
        [FromQuery] int? latest = null)
    {
        if (!IsAuthorized(auth)) return StatusCode(403, new { status = 403, error_msg = "You are not authorized..." });
        if (latest.HasValue) _latest = latest.Value;

        if (Request.Method == "POST")
        {
            try
            {
                var requestData = await JsonSerializer.DeserializeAsync<JsonElement>(Request.Body);

                if (requestData.TryGetProperty("follow", out JsonElement followElement))
                {
                    string targetUser = followElement.GetString()!;
                    await _authorService.FollowUser(username, targetUser);
                    return StatusCode(204);
                }

                if (requestData.TryGetProperty("unfollow", out JsonElement unfollowElement))
                {
                    string targetUser = unfollowElement.GetString()!;
                    await _authorService.UnfollowUser(username, targetUser);
                    return StatusCode(204);
                }

                return StatusCode(400,
                    new { status = 400, error_msg = "Missing 'follow' or 'unfollow' in request body" });
            }
            catch (InvalidOperationException)
            {
                return StatusCode(404);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                return StatusCode(500, new { status = 500, error_msg = ex.Message });
            }
        }
        else //GET
        {
            try
            {
                List<AuthorDTO> following = await _authorService.GetFollowing(username);
                var followNames = following
                    .Take(no)
                    .Select(a => a.Name)  
                    .ToList();

                return Ok(new { follows = followNames });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(400, new { status = 400, error_msg = ex.Message });
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Exception: {ex}");
                return StatusCode(500, new { status = 500, error_msg = ex.Message });
            }
        }
    }

    /// <summary>
    /// Creates a new instance of the Author entity using reflection.
    /// Required for ASP.NET Core Identity user registration.
    /// </summary>
    /// <returns>A new Author instance.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if Author class cannot be instantiated (e.g., if it's abstract or lacks a parameterless constructor).
    /// </exception>
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

    /// <summary>
    /// Retrieves the email store from the UserManager.
    /// Required for setting user email during registration.
    /// </summary>
    /// <returns>An IUserEmailStore instance for Author entities.</returns>
    /// <exception cref="NotSupportedException">
    /// Thrown if the configured user store does not support email operations.
    /// </exception>
    private IUserEmailStore<Author> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<Author>)_userStore;
    }

}