using Microsoft.EntityFrameworkCore;
using MyRecipes.Api.Data;
using MyRecipes.Api.Services;


var builder = WebApplication.CreateBuilder(args);


// הוספת DbContext עם SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=./myrecipes.db"));


// הוספת AuthService
builder.Services.AddScoped<AuthService>();

// הוספת JWT Authentication
var key = builder.Configuration["Jwt:Key"] ?? "default-secret-key-change-this";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(key)
        )
    };
});

// הוספת Authorization
builder.Services.AddAuthorization();



// CORS לפיתוח (נצמצם בהמשך למקור הקליינט)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", p =>
        p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

// OpenAPI נשאיר אם נוח לך
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("AllowClient");

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ✔️ נקודת בריאות
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// ✔️ נקודת גרסה
app.MapGet("/version", () => Results.Ok(new { version = "0.1.0" }));

// (אופציונלי) להשאיר את WeatherForecast לדוגמה
var summaries = new[]
{
    "Freezing","Bracing","Chilly","Cool","Mild","Warm","Balmy","Hot","Sweltering","Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        )
    ).ToArray();
    return forecast;
});

app.Run();


record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
