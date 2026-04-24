using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text;
using Transferencia.Application.Interfaces;
using Transferencia.Infrastructure.Clients;
using Transferencia.Infrastructure.Persistence;
using Transferencia.Infrastructure.Repositories;
using Transferencia.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration["Jwt:Key"];

builder.Services.AddControllers();

builder.Services.AddSingleton<IJwtService, JwtService>(provider => new JwtService(key));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = false,
               ValidateAudience = false,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))
           };
       });

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("Transferencia.Application")));

builder.Services.AddHttpClient<IContaCorrenteClient, ContaCorrenteClient>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Services:ContaCorrente:BaseUrl"]
        ?? throw new InvalidOperationException("BaseUrl 'Services:ContaCorrente:BaseUrl' not found."));
});

builder.Services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton(new DbConnectionFactory(
    builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
));

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.Run();