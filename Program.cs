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
        //Add jwt authentication to swagger
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token. The 'Bearer' prefix is added automatically."
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

//Inject DI of database
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnection")));

//Creating a variable to store our JwtSettings in appsettings (We can use this to quick access it)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

//Creating a variable to store our secret key
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

// Configuring JWT authentication schemes
services.AddAuthentication(options =>
{
    //
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
using (var scope = app.Services.CreateScope())
{
    // Get the database context to interact with the database
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!context.Users.Any(u => u.Role == "Admin"))
    {
        // Creating the default Admin user
        var admin = new User
        {
            FullName = "Admin",
            Email = "Admin@gmail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("Admin123"), //Password is hashed
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

