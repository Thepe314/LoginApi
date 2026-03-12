using System.Security.Cryptography;
using System.Text;
using LoginApi.Data;
using LoginApi.Models;
using LoginApi.Repositories;
using LoginApi.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;


var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>
    {
        // Configure Swagger to support JWT Bearer authentication:
        // This allows you to enter a JWT token in the Swagger UI “Authorize” button,
        // which Swagger will then send in the Authorization header on API requests.
        // Effectively, it enables testing protected endpoints with your JWT token.
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token."
        });

         c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
    }
);

//Inject DI of Repo
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<EncryptionService>();

//Inject DI of database
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnection")));

// var Encryptedkey = new byte[32];
// RandomNumberGenerator.Fill(Encryptedkey);
// Console.WriteLine("NEW KEY: " + Convert.ToBase64String(Encryptedkey));

var testKey = builder.Configuration["EncryptionSettings:Key"];
var keyBytes = Convert.FromBase64String(testKey);
Console.WriteLine($"Key length: {keyBytes.Length} bytes");

//Creating a variable to store our JwtSettings in appsettings (We can use this to quick access it)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

//Creating a variable to store our secret key
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

// Configuring JWT authentication schemes
services.AddAuthentication(options =>
{
    
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    
}).AddJwtBearer(options =>
{
    // These are the parameters that are validated when an incoming JWT token is received
    options.TokenValidationParameters= new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

// Seed an Admin user on startup if one doesn't already exist
//Creating a temp scope that will be discarded after use
using (var scope = app.Services.CreateScope())
{
    // Get the database context to interact with the database
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
     var encryption = scope.ServiceProvider.GetRequiredService<EncryptionService>();

    //Check if there any user with role of admin if yes, give message if no,create new user.
    if (!context.Users.Any(u => u.Role == "Admin"))
    {
        // Creating the default Admin user
        var admin = new User
        {
            FullName = "Admin",
            Email = "Admin@gmail.com",
            Password = encryption.Encrypt("Admin123"), //Password is encrypted
            Role = "Admin"
        };
        //Save admin to database
        context.Users.Add(admin);
        context.SaveChanges();

        Console.WriteLine("Admin seeded successfully ");
    }
    else
    {   //Admin already exists, no need to seed it
        Console.WriteLine("Admin already exists");
    }
}

app.UseSwagger();

app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();


app.Run();

