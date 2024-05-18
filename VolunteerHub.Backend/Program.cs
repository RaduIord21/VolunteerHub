using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataAccessLayer.Repositories;
using VolunteerHub.DataModels.Models;
using AutoMapper;
using VolunteerHub.Backend.Infrastructure;
using VolunteerHub.Backend.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using VolunteerHub.Backend.Data;
using System.Text.Json.Serialization;
using VolunteerHub.Backend.Services.Interfaces;
using VolunteerHub.Backend.Services.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<VolunteerHubContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VolunteerHub.Backend")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//!!builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDefaultIdentity<User>(
    options =>
    { 
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.Password.RequiredUniqueChars = 1;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<VolunteerHubContext>()
    .AddDefaultTokenProviders();
    


//https://www.c-sharpcorner.com/article/jwt-json-web-token-authentication-in-asp-net-core/
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin",
            authBuilder =>
            {
                authBuilder.RequireRole("Admin");
            });
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "VolunteerHub", Version = "v1" });
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<OrganizationManager>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IProjectStatsRepository, ProjectStatsRepository>();
builder.Services.AddScoped<IUserStatsRepository, UserStatsRepository>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IUserTaskRepository, UserTaskRepository>();
builder.Services.AddScoped<IUserOrganizationRepository, UserOrganizationRepository>();

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddCors(
    options =>
    {

        options.AddPolicy("AllowSpecificOrigin", builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
    }
    );
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

//dotnet ef dbcontext scaffold "Data Source=DESKTOP-EDQ5MCB;Initial Catalog=VolunteerHub;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False" Microsoft.EntityFrameworkCore.SqlServer -o Models



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<VolunteerHubContext>();
    context.Database.Migrate();
    // requires using Microsoft.Extensions.Configuration;
    // Set password with the Secret Manager tool.
    // dotnet user-secrets set SeedUserPW <pw>

    var testUserPw = builder.Configuration.GetValue<string>("SeedUserPW");
    //await SeedData.Initialize(services, testUserPw);
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowSpecificOrigin");
    app.UseSwagger();
    app.UseSwaggerUI();
    // To serve the Swagger UI at the app's root
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
    app.UseRouting();
    app.UseHttpsRedirection();
    app.UseHttpLogging(); 
    //app.UseMiddleware<JwtMiddleware>();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.UseCors("AllowSpecificOrigin");
    app.Run();
}


