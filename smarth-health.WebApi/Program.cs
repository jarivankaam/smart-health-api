using Microsoft.Data.SqlClient;
using smarth_health.WebApi.Repositories;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load User Secrets (in Development)
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

var logger = LoggerFactory.Create(logging => logging.AddConsole()).CreateLogger<Program>();

var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);

builder.Services.AddAuthorization();

// Identity API with Dapper Stores
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddDapperStores(options =>
    {
        options.ConnectionString = sqlConnectionString;
    });

// Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ✅ Register direct DB connection
builder.Services.AddScoped<IDbConnection>(sp =>
{
    logger.LogInformation("🔗 Attempting to create a database connection...");
    return new SqlConnection(sqlConnectionString);
});

// ✅ Register repositories
builder.Services.AddScoped<ITimelineRepository, TimelineRepository>();
builder.Services.AddScoped<ITimeLineItemRepository, TimeLineItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Map default Identity endpoints under /auth.
app.MapGroup("/auth")
    .MapIdentityApi<IdentityUser>();

// Custom logout endpoint.
app.MapPost("/auth/logout",
    async (SignInManager<IdentityUser> signInManager, [FromBody] object empty) =>
    {
        if (empty != null)
        {
            await signInManager.SignOutAsync();
            return Results.Ok();
        }
        return Results.Unauthorized();
    })
.RequireAuthorization();

app.MapGet("/", () => $"The API is up. Connection string found: {(sqlConnectionStringFound ? "Yes" : "No")}");

app.Use(async (context, next) =>
{
    Console.WriteLine($"📌 Incoming request: {context.Request.Method} {context.Request.Path}");
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();