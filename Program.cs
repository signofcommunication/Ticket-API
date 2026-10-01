using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Ticket_API.Data;
using Ticket_API.Models;
using Ticket_API.OpenApi;
using Ticket_API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddSingleton<TokenService>();

// 400 validasi model -> envelope terstruktur.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .SelectMany(e => e.Value!.Errors.Select(err => new ApiError(
                Code: "validation",
                Message: string.IsNullOrWhiteSpace(err.ErrorMessage)
                    ? $"Field {e.Key} tidak valid."
                    : err.ErrorMessage,
                Field: e.Key)))
            .ToList();

        var payload = ApiResponse<object?>.Fail(
            "Validasi gagal.",
            errors,
            context.HttpContext.TraceIdentifier);
        return new BadRequestObjectResult(payload);
    };
});

var jwt = builder.Configuration.GetSection("Jwt");
var jwtKey = jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1),
        };
        // 401/403 dari JWT -> envelope terstruktur, bukan body kosong.
        var envelopeOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var payload = ApiResponse<object?>.Fail(
                    "Unauthorized. Token tidak ada atau tidak valid.",
                    [new ApiError("unauthorized", "Butuh Bearer token dari login.")],
                    context.HttpContext.TraceIdentifier);
                return context.Response.WriteAsync(JsonSerializer.Serialize(payload, envelopeOptions));
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var payload = ApiResponse<object?>.Fail(
                    "Forbidden. Anda tidak punya akses.",
                    [new ApiError("forbidden", "Role tidak diizinkan.")],
                    context.HttpContext.TraceIdentifier);
                return context.Response.WriteAsync(JsonSerializer.Serialize(payload, envelopeOptions));
            },
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddProblemDetails();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("Backoffice", policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// 500 global -> envelope terstruktur.
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var feature = context.Features.Get<IExceptionHandlerFeature>();
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogError(feature?.Error, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);

    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    context.Response.ContentType = "application/json";
    var payload = ApiResponse<object?>.Fail(
        "Terjadi kesalahan internal.",
        [new ApiError("internal_error", "Coba lagi beberapa saat.")],
        context.TraceIdentifier);
    await context.Response.WriteAsJsonAsync(payload);
}));

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Ticket API")
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseCors("Backoffice");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await AppDbContext.EnsureCreatedAndSeededAsync(app.Services, CancellationToken.None);

app.Run();
