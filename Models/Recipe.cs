namespace MyRecipes.Api.Models;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public int PreparationTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;  // ← הוסף את זה
    public string? ImageUrl { get; set; }
    
    // קשר למשתמש
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
