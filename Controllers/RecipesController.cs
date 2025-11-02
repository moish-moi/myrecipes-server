using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyRecipes.Api.Data;
using MyRecipes.Api.Models;
using System.Security.Claims;

namespace MyRecipes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // חייב להיות מחובר
public class RecipesController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public RecipesController(AppDbContext context)
    {
        _context = context;
    }
    
    // GET: api/recipes - קבל את כל המתכונים של המשתמש
    [HttpGet]
    public IActionResult GetMyRecipes()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var recipes = _context.Recipes
            .Where(r => r.UserId == userId)
            .ToList();
        
        return Ok(recipes);
    }
    
    // GET: api/recipes/{id} - קבל מתכון ספציפי
    [HttpGet("{id}")]
    public IActionResult GetRecipe(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var recipe = _context.Recipes.FirstOrDefault(r => r.Id == id && r.UserId == userId);
        if (recipe == null)
            return NotFound("המתכון לא נמצא");
        
        return Ok(recipe);
    }
    
    // POST: api/recipes - הוספת מתכון חדש
    [HttpPost]
    public IActionResult CreateRecipe([FromBody] CreateRecipeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest("שם המתכון הוא חובה");
        
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var recipe = new Recipe
        {
            Title = request.Title,
            Ingredients = request.Ingredients ?? string.Empty,
            Instructions = request.Instructions ?? string.Empty,
            Tags = request.Tags ?? string.Empty,
            PreparationTime = request.PreparationTime,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Recipes.Add(recipe);
        _context.SaveChanges();
        
        return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
    }
    
    // PUT: api/recipes/{id} - עדכון מתכון
    [HttpPut("{id}")]
    public IActionResult UpdateRecipe(int id, [FromBody] CreateRecipeRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var recipe = _context.Recipes.FirstOrDefault(r => r.Id == id && r.UserId == userId);
        if (recipe == null)
            return NotFound("המתכון לא נמצא");
        
        recipe.Title = request.Title ?? recipe.Title;
        recipe.Ingredients = request.Ingredients ?? recipe.Ingredients;
        recipe.Instructions = request.Instructions ?? recipe.Instructions;
        recipe.Tags = request.Tags ?? recipe.Tags;
        recipe.PreparationTime = request.PreparationTime > 0 ? request.PreparationTime : recipe.PreparationTime;
        
        _context.Recipes.Update(recipe);
        _context.SaveChanges();
        
        return Ok(recipe);
    }
    
    
    // DELETE: api/recipes/{id} - מחיקת מתכון
    [HttpDelete("{id}")]
    public IActionResult DeleteRecipe(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var recipe = _context.Recipes.FirstOrDefault(r => r.Id == id && r.UserId == userId);
        if (recipe == null)
            return NotFound("המתכון לא נמצא");
        
        _context.Recipes.Remove(recipe);
        _context.SaveChanges();
        
        return Ok(new { message = "המתכון נמחק בהצלחה" });
    }
    
    // GET: api/recipes/search/{tag} - חיפוש לפי תגית
    [HttpGet("search/{tag}")]
    public IActionResult SearchByTag(string tag)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var recipes = _context.Recipes
            .Where(r => r.UserId == userId && r.Tags.Contains(tag))
            .ToList();
        
        return Ok(recipes);
    }
}

// DTO for creating/updating recipes
public class CreateRecipeRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Ingredients { get; set; }
    public string? Instructions { get; set; }
    public string? Tags { get; set; }
    public int PreparationTime { get; set; }
}
