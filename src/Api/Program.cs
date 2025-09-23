using Application.Commands;
using Application.Queries;
using Domain.Configuration;
using Domain.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fullstack Backend API", Version = "v1" });
    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Test => User admin Password admin"
    };
    c.AddSecurityDefinition("bearerAuth", jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, new string[] { } }
    });
});

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("FullstackDemoDb"), 
    ServiceLifetime.Singleton);

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepositorySync<>));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(CreateUserCommand).Assembly,
    typeof(GetUsersQuery).Assembly));

var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt.GetValue<string>("Key"));
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
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt.GetValue<string>("Issuer"),
        ValidAudience = jwt.GetValue<string>("Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IRequestHandler<AuthUserCommand, string>, DevAuthUserHandler>();
}
else
{
    builder.Services.AddScoped<IRequestHandler<AuthUserCommand, string>, AuthUserHandler>();
}

builder.Services.Configure<DevelopmentUserOptions>
    (builder.Configuration.GetSection(DevelopmentUserOptions.DevelopmentUser));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Users.Any())
    {
        var pw = Convert.ToHexString(System.Security.Cryptography.SHA256.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes("admin")));
        db.Users.Add(new Domain.Entities.User("admin", "admin@example.com", pw));
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseCors("AllowAngular");   
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();