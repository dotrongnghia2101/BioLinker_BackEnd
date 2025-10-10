using BioLinker.Data;
using BioLinker.DTO.UserDTO;
using BioLinker.Enities;
using BioLinker.Respository.BioPageRepo;
using BioLinker.Respository.ClickRepo;
using BioLinker.Respository.LinkRepo;
using BioLinker.Respository.TemplateRepo;
using BioLinker.Respository.UserRepo;
using BioLinker.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// ==================== CAU HINH DATABASE MYSQL ====================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

//==================== CAU HINH SERVICE ====================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<IBackgroundService, BioLinker.Service.BackgroundService>();
builder.Services.AddScoped<IStyleService, StyleService>();
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<IStyleSettingsService, StyleSettingsService>();
builder.Services.AddScoped<IBioPageService, BioPageService>();
builder.Services.AddScoped<ITemplateDetailService, TemplateDetailService>();
builder.Services.AddScoped<IStaticLinkService, StaticLinkService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IEmailVerificationService, EmailVerificationService>();
builder.Services.AddScoped<ICountTemplateClickedRepository, CountTemplateClickedRepository>();
builder.Services.AddScoped<ICountTemplateClickedService, CountTemplateClickedService>();

//==================== CAU HINH REPOSITORY ====================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<IBackgroundRepository, BackgroundRepository>();
builder.Services.AddScoped<IStyleRepository, StyleRepository>();
builder.Services.AddScoped<IContentRepository, ContentRepository>();
builder.Services.AddScoped<IStyleSettingsRepository, StyleSettingsRepository>();
builder.Services.AddScoped<IBioPageRepository, BioPageRepository>();
builder.Services.AddScoped<ITemplateDetailRepository, TemplateDetailRepository>();
builder.Services.AddScoped<IStaticLinkRepository, StaticLinkRepository>();
builder.Services.AddScoped<ICountBioClickedRepository, CountBioClickedRepository>();
builder.Services.AddScoped<ICountBioClickedService, CountBioClickedService>();
// ==================== AUTHENTICATION (JWT + FACEBOOK) ====================

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Facebook";
})
// Cookie dùng để giữ session OAuth (bắt buộc cho Facebook)
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookieOptions =>
{
    cookieOptions.Cookie.Name = ".BioLinker.Auth";
    cookieOptions.Cookie.HttpOnly = true;  
        //  Bắt buộc cho Render vì Facebook yêu cầu HTTPS thật
        cookieOptions.Cookie.SameSite = SameSiteMode.None;
        cookieOptions.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
// FACEBOOK OAUTH CONFIG
.AddFacebook("Facebook", options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    options.CallbackPath = "/api/auth/signin-facebook"; // ⚠ phải khớp với Meta Developer
    options.SaveTokens = true;

    //  Scope và Fields để Facebook trả về đủ dữ liệu (email, avatar)
    options.Scope.Add("email");
    options.Scope.Add("public_profile");
    options.Fields.Add("id");
    options.Fields.Add("name");
    options.Fields.Add("email");
    options.Fields.Add("picture");

    //  Sự kiện Facebook (đọc avatar và sửa redirect HTTPS)
    options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
    {
        OnCreatingTicket = context =>
        {
            try
            {
                // Đảm bảo có using System.Text.Json
                using var userJson = JsonDocument.Parse(context.User.GetRawText());
                var root = userJson.RootElement;

                string? pictureUrl = null;

                if (root.TryGetProperty("picture", out var pictureProp) &&
                    pictureProp.TryGetProperty("data", out var dataProp) &&
                    dataProp.TryGetProperty("url", out var urlProp))
                {
                    pictureUrl = urlProp.GetString();
                }

                if (!string.IsNullOrEmpty(pictureUrl))
                {
                    var identity = (ClaimsIdentity)context.Principal.Identity!;
                    identity.AddClaim(new Claim("picture", pictureUrl));
                }
            }
            catch
            {
                // bỏ qua lỗi parse JSON
            }

            return Task.CompletedTask;
        },

        //  Fix redirect HTTP → HTTPS khi chạy trên Render
        OnRedirectToAuthorizationEndpoint = context =>
        {
            var httpsUrl = context.RedirectUri.Replace("http://", "https://");
            context.Response.Redirect(httpsUrl);
            return Task.CompletedTask;
        }
    };
})

// JWT CHO API
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
            var token = context.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
                context.Token = token;
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
             .WithOrigins(
            "https://biolinker.io.vn",
            "http://localhost:3000",      // FE local test (React)
             "https://localhost:3000") // FE domain
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

app.Use((context, next) =>
{
    if (context.Request.Headers.ContainsKey("X-Forwarded-Proto") &&
        context.Request.Headers["X-Forwarded-Proto"] == "https")
    {
        context.Request.Scheme = "https";
    }
    return next();
});

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BioLinker API v1");
    c.DocumentTitle = "BioLinker Swagger UI";
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseCookiePolicy();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
