using Microsoft.AspNetCore.Mvc;
using MyRecipes.Api.Data;
using MyRecipes.Api.Models;
using MyRecipes.Api.Services;

namespace MyRecipes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;
    
    public AuthController(AppDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }
    
    // POST: api/auth/register
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || 
            string.IsNullOrWhiteSpace(request.Email) || 
            string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Username, email ו-password הם חובה");
        
        // בדוק אם המשתמש כבר קיים
        var existingUser = _context.Users.FirstOrDefault(u => u.Username == request.Username);
        if (existingUser != null)
            return BadRequest("Username זה כבר בשימוש");
        
        // צור משתמש חדש
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _authService.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Users.Add(user);
        _context.SaveChanges();
        
        return Ok(new { message = "הרשמה בוצעה בהצלחה", userId = user.Id });
    }
    
    // POST: api/auth/login
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || 
            string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Username ו-password הם חובה");
        
        // חפש משתמש
        var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
        if (user == null)
            return Unauthorized("Username או password לא נכון");
        
        // בדוק סיסמה
        if (!_authService.VerifyPassword(request.Password, user.PasswordHash))
            return Unauthorized("Username או password לא נכון");
        
        // יצור JWT token
        var token = _authService.GenerateToken(user);
        
        return Ok(new { token, userId = user.Id, username = user.Username });
    }
}

// DTO Classes
public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
