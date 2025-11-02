namespace MyRecipes.Api.Models;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty; // נשמור כטקסט מופרד בפסיקים
    public string Instructions { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty; // תגיות מופרדות בפסיקים
    public int PreparationTime { get; set; } // בדקות
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // קשר למשתמש
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
