using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PSK.Server.Data;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;
using System.Text;
using PSK.Server.Services;
using PSK.Server.Middlewares;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using PSK.Server.Interceptors;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()

    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    }); builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<MetadataSchemaFilter>();

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<ILabelService, LabelService>();

builder.Services.AddHttpContextAccessor();

//Logging Interceptor
builder.Services.Configure<LoggingInterceptorOptions>(
    builder.Configuration.GetSection("LoggingInterceptor"));

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.Register(c =>
    {
        var sp = c.Resolve<IServiceProvider>();
        var options = sp.GetRequiredService<IOptions<LoggingInterceptorOptions>>().Value;
        var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..")); //log to Kredditoriai/psk.log
        var fullPath = Path.Combine(projectRoot, options.LogFilePath);

        return new LoggingInterceptor(fullPath, httpContextAccessor, options.Enabled);
    }).AsSelf().InstancePerDependency();

    containerBuilder.RegisterAssemblyTypes(typeof(Program).Assembly)
        .Where(t => t.Name.EndsWith("Service"))
        .AsImplementedInterfaces()
        .EnableInterfaceInterceptors()
        .InterceptedBy(typeof(LoggingInterceptor));
});


builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

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
        ValidIssuer = "Issuer",
        ValidAudience = "Audience",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecretSecret123...SecretAAAAAAAAAAA"))
    };
});

builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);

TypeAdapterConfig<Guid?, Guid?>.NewConfig()
    .MapWith(src => src == Guid.Empty ? (Guid?)null : src);


var app = builder.Build();
app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();


app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.MapFallbackToFile("/index.html");

await app.RunAsync();
