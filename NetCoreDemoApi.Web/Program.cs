using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NetCoreDemoApi.Repositories;
using NetCoreDemoApi.Repositories.SqlLite;
using NetCoreDemoApi.Services;
using Serilog;
using System.Text;

try
{
    var builder = WebApplication.CreateBuilder(args);

    //builder.Host.UseSerilog();  // Add SeriLog
    builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

    builder.Services.AddControllers();// Add services to the container.

    Log.Information("Starting web application");

    //Jwt configuration starts here
    var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
    var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     .AddJwtBearer(options =>
     {
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidateLifetime = true,
             ValidateIssuerSigningKey = true,
             ValidIssuer = jwtIssuer,
             ValidAudience = jwtIssuer,
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
         };
     });
    //Jwt configuration ends here

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    //  Repositories
    builder.Services.AddTransient<IClientRepository, ClienteRepository>();
    builder.Services.AddTransient<IUserRepository, UserRepository>();

    //  services
    builder.Services.AddTransient<IClientService, ClientService>();
    builder.Services.AddTransient<AuthService>();

    var app = builder.Build();

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
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
