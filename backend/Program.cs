using StudentEnrollmentAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Register Controllers and MySQL Database Service
builder.Services.AddControllers();
builder.Services.AddSingleton<DbConnection>();

// 2. Configure Universal CORS policy (Allows Netlify, Localhost, Postman, etc.)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// 3. Enable CORS middleware
app.UseCors("AllowReactApp");

// 4. Root Health-Check Endpoint (shows API status when opened in browser)
app.MapGet("/", () => Results.Ok(new
{
    status = "healthy",
    message = "Student Enrollment API is running successfully!",
    endpoints = new[] { "/api/students", "/api/enrollments" }
}));

// 5. Map Controller routes (/api/students and /api/enrollments)
app.MapControllers();

// 6. Dynamic Port binding (Uses $PORT for Cloud/Render, defaults to 5000 for Localhost)
var port = Environment.GetEnvironmentVariable("PORT");
var url = !string.IsNullOrEmpty(port) ? $"http://0.0.0.0:{port}" : "http://localhost:5000";

app.Run(url);