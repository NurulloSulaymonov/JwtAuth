using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Helpers;
using WebApi.Services.Account;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();

builder.Logging.AddSerilog(logger);

builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<UserService>();
//connection to sqlite database
builder.Services.AddDbContext<DataContext>(config=>config.UseSqlite("Data Source=blog.db"));

//control identity tables in sqlite database
builder.Services.AddIdentity<User, IdentityRole>(config =>
    {
        config.Password.RequiredLength = 4;
        config.Password.RequireDigit = false; // must have at least one digit
        config.Password.RequireNonAlphanumeric = false; // must have at least one non-alphanumeric character
        config.Password.RequireUppercase = false; // must have at least one uppercase character
        config.Password.RequireLowercase = false;  // must have at least one lowercase character
    })
    //for registering usermanager and signinmanger
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
 builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        }).AddJwtBearer(o =>
        {
            var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]);
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                ValidAudience = builder.Configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sample web API",
                Version = "v1",
                Description = "Sample API Services.",
                Contact = new OpenApiContact
                {
                    Name = "Khayriddin Sakhiev."
                },
            });
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
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
                    new string[] {}
                }
            });
        });

var app = builder.Build();

await app.Services.Initialize();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();