using BioLinker.Data;
using BioLinker.DTO;
using BioLinker.Enities;
using BioLinker.Repository;
using BioLinker.Respository;
using BioLinker.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// ==================== CAU HINH DATABASE ====================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(connectionString));

//==================== CAU HINH SERVICE ====================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

//==================== CAU HINH REPOSITORY ====================
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ==================== CAU HINH AUTHENTICATION (JWT + FACEBOOK) ====================
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // default la JWT
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
//{
//    var jwtConfig = builder.Configuration.GetSection("Jwt");
//    var key = Encoding.UTF8.GetBytes(jwtConfig["Key"]!);

//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = false,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtConfig["Issuer"],
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        RoleClaimType = ClaimTypes.Role
//    };

//    options.Events = new JwtBearerEvents
//    {
//        OnMessageReceived = context =>
//        {
//            var token = context.Request.Cookies["jwt"];
//            if (!string.IsNullOrEmpty(token))
//            {
//                context.Token = token;
//            }
//            return Task.CompletedTask;
//        }
//    };
//})
//.AddCookie("Cookies") // them cookie de giu session khi login bang facebook
//.AddFacebook("Facebook", options =>
//{
//    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
//    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
//    options.CallbackPath = "/signin-facebook";
//    options.SaveTokens = true;
//    options.Scope.Add("email");
//    options.Fields.Add("name");
//    options.Fields.Add("email");
//});
builder.Services.AddAuthentication(options =>
{
    // Khi chưa xác định thì mặc định dùng JWT cho API
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

    // Nếu có challenge từ Facebook thì nó sẽ override
    options.DefaultChallengeScheme = "Facebook";
})
// Cookie (rất quan trọng cho OAuth correlation)
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookieOptions =>
{
    cookieOptions.Cookie.Name = ".BioLinker.Auth";
    cookieOptions.Cookie.HttpOnly = true;
    //cookieOptions.Cookie.SameSite = SameSiteMode.None; // bắt buộc cho redirect OAuth
    //cookieOptions.Cookie.SecurePolicy = CookieSecurePolicy.Always; // chỉ chạy HTTPS
    cookieOptions.Cookie.SameSite = SameSiteMode.Lax;
    cookieOptions.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
})
// Facebook OAuth
.AddFacebook("Facebook", options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // phải chỉ rõ
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    options.CallbackPath = "/signin-facebook"; // phải khớp Facebook Dev Console
    options.SaveTokens = true;
    options.Scope.Add("email");
    options.Fields.Add("name");
    options.Fields.Add("email");
})
// JWT cho API
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions =>
{
    var jwtConfig = builder.Configuration.GetSection("Jwt");
    var key = Encoding.UTF8.GetBytes(jwtConfig["Key"]!);

    jwtOptions.SaveToken = true;
    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RoleClaimType = ClaimTypes.Role
    };

    jwtOptions.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Nếu token được lưu trong cookie "jwt"
            var token = context.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

//==================== CAU HINH CORS ==================== (cross-origin resource sharing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
              .WithOrigins("https://localhost:7168", "https://biolinker.com") // domain duoc phep
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


//==================== CAU HINH GOOGLE ==================== 
builder.Services.Configure<GoogleAuthSettings>(
    builder.Configuration.GetSection("GoogleAuthSettings"));

//==================== CAU HINH SWAGGER ==================== 
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Biolinker API",
        Version = "v1",
        Description = "API documentation for Biolinker system"
    });

    // them cau hinh JWT va hien Authorize
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your JWT token}'"
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
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BioLinker API v1");
    c.DocumentTitle = "BioLinker Swagger UI";
});

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseCookiePolicy();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
