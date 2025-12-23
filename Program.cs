using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Clients;
using SpeakingPractice.Api.Infrastructure.Persistence;
using SpeakingPractice.Api.Middleware;
using SpeakingPractice.Api.Options;
using SpeakingPractice.Api.Repositories;
using SpeakingPractice.Api.Services;
using SpeakingPractice.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Speaking Practice API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
        .UseSnakeCaseNamingConvention();
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection(MinioOptions.SectionName));
builder.Services.Configure<WhisperOptions>(builder.Configuration.GetSection(WhisperOptions.SectionName));
builder.Services.Configure<LlamaOptions>(builder.Configuration.GetSection(LlamaOptions.SectionName));
builder.Services.Configure<LanguageToolOptions>(builder.Configuration.GetSection(LanguageToolOptions.SectionName));
builder.Services.Configure<PayOsOptions>(builder.Configuration.GetSection(PayOsOptions.SectionName));

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                 ?? throw new InvalidOperationException("Jwt configuration missing");

if (string.IsNullOrWhiteSpace(jwtOptions.Key) || jwtOptions.Key.Length < 16)
{
    throw new InvalidOperationException("JWT Key must be at least 16 characters long (128 bits) for HS256 algorithm");
}

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));

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
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = signingKey
        };

        // Allow JWT to be read from the accessToken cookie for browser-based clients
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (string.IsNullOrEmpty(context.Token) &&
                    context.Request.Cookies.TryGetValue("accessToken", out var token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddHttpClient<IWhisperClient, WhisperClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<WhisperOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});
builder.Services.AddHttpClient<ILlamaClient, LlamaClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<LlamaOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    // Increase timeout for grammar correction which can take 12+ seconds
    client.Timeout = TimeSpan.FromSeconds(60);
});
builder.Services.AddHttpClient<ILanguageToolClient, LanguageToolClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<LanguageToolOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

builder.Services.AddHttpClient("PayOS", (sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<PayOsOptions>>().Value;
    if (string.IsNullOrWhiteSpace(options.BaseUrl))
    {
        throw new InvalidOperationException("PayOS BaseUrl is not configured");
    }

    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

    // Auth headers giống như bạn đang dùng trong Postman
    if (!string.IsNullOrWhiteSpace(options.ClientId))
    {
        client.DefaultRequestHeaders.Add("x-client-id", options.ClientId);
    }

    if (!string.IsNullOrWhiteSpace(options.ApiKey))
    {
        client.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
    }
});

builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<MinioOptions>>().Value;
    
    // Extract hostname:port from endpoint URL
    // MinIO client expects only hostname:port, not full URL
    var endpoint = options.Endpoint;
    var useSsl = options.WithSsl;
    
    // Parse endpoint - MinIO expects format: hostname:port
    if (Uri.TryCreate(endpoint, UriKind.Absolute, out var uri))
    {
        var host = uri.Host;
        var port = uri.Port;
        
        // If port is -1 (not specified), use default
        if (port <= 0)
        {
            port = uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) ? 443 : 9000;
        }
        
        endpoint = $"{host}:{port}";
        useSsl = uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase);
    }
    else
    {
        // Try to parse manually if URI parsing fails
        endpoint = endpoint.Trim();
        
        if (endpoint.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
        {
            endpoint = endpoint.Substring(7); // Remove "http://"
            useSsl = false;
        }
        else if (endpoint.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            endpoint = endpoint.Substring(8); // Remove "https://"
            useSsl = true;
        }
        
        // Ensure endpoint has port if not present
        if (!endpoint.Contains(':'))
        {
            endpoint = $"{endpoint}:{(useSsl ? 443 : 9000)}";
        }
    }
    
    var client = new MinioClient()
        .WithEndpoint(endpoint)
        .WithCredentials(options.AccessKey, options.SecretKey);

    if (useSsl)
    {
        client = client.WithSSL();
    }

    return client.Build();
});

builder.Services.AddScoped<IMinioClientWrapper, MinioClientWrapper>();
builder.Services.AddScoped<ISpeakingSessionRepository, SpeakingSessionRepository>();
builder.Services.AddScoped<IRecordingRepository, RecordingRepository>();
builder.Services.AddScoped<IAnalysisResultRepository, AnalysisResultRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IUserDraftRepository, UserDraftRepository>();
builder.Services.AddScoped<IUserProgressRepository, UserProgressRepository>();
builder.Services.AddScoped<IVocabularyRepository, VocabularyRepository>();
builder.Services.AddScoped<IUserVocabularyRepository, UserVocabularyRepository>();
builder.Services.AddScoped<IAchievementRepository, AchievementRepository>();
builder.Services.AddScoped<IUserAchievementRepository, UserAchievementRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IApiUsageLogRepository, ApiUsageLogRepository>();
builder.Services.AddScoped<ISpeakingSessionService, SpeakingSessionService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IContentGenerationService, ContentGenerationService>();
builder.Services.AddScoped<IRefinementService, RefinementService>();
builder.Services.AddScoped<IDraftService, DraftService>();
builder.Services.AddScoped<IStreakService, StreakService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandling();
app.UseCors("AllowLocalhost3000");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed IELTS topics if --seed argument is provided
if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await SpeakingPractice.Api.DataSeed.IELTSTopicsSeeder.SeedAsync(context);
}

app.Run();
