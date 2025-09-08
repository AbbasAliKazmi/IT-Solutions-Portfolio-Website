using ProductAPI.Services; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProductAPI.Data;
using ProductAPI.Models;
using System.Text;
using System.IO;
using Microsoft.OpenApi.Models;   // ✅ Swagger ke liye add

var builder = WebApplication.CreateBuilder(args);

// ===================== Phase 3: Identity + JWT =====================
// 🔹 Identity DbContext (SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Identity setup (ApplicationUser + ApplicationRole)
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 🔹 JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

// ===================== Phase 1: Products API =====================
builder.Services.AddDbContext<ProductContext>(options =>
    options.UseSqlite("Data Source=products.db"));

// ===================== Phase 2: Other Setup =====================
// 🔹 CORS (Frontend Allowed)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",   // React dev server
                "http://localhost:10005", 
                "http://localhost/weburio", // agar WP ya custom port pe chal raha
                "http://localhost",        // WordPress local without port
                "http://127.0.0.1"         // alternate localhost
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// 🔹 Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SupportNonNullableReferenceTypes();

    // ✅ JWT Bearer config for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer prefix (e.g., Bearer eyJhbGciOi...)",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ✅ Email sender register
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();



var app = builder.Build();

// ===================== Phase 1 + 3: Seed Product DB + Roles + Admin =====================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        // --- Product seed ---
        var productContext = services.GetRequiredService<ProductContext>();
        DbInitializer.Initialize(productContext);

        // --- Roles seed ---
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        await SeedData.SeedRolesAsync(roleManager);

        // --- Admin user seed ---
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        await SeedData.EnsureAdminUserAsync(userManager);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Seeding error: {ex.Message}");
    }
}

// ===================== Phase 2: Swagger =====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ===================== Phase 2: Static Files =====================
app.UseStaticFiles();
var contentPath = Path.Combine(app.Environment.WebRootPath, "content");
if (!Directory.Exists(contentPath))
{
    Directory.CreateDirectory(contentPath);
}

// ===================== Phase 3: Authentication & Routing =====================
// ✅ Correct order
app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowFrontend"); // ✅ CORS here

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
