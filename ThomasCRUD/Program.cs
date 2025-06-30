using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ThomasCRUD.Data;
using ThomasCRUD.Repository;

var builder = WebApplication.CreateBuilder(args);

// Configura o Entity Framework Core para usar o SQL Server com a connection string do appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona suporte para descoberta de endpoints (usado pelo Swagger)
builder.Services.AddEndpointsApiExplorer();

// Configura o Swagger(documentação da API) com autenticação via JWT
builder.Services.AddSwaggerGen(c =>
{
    // Define o título e versão da documentação Swagger
    c.SwaggerDoc("v1", new() { Title = "ThomasCRUD", Version = "v1" });

    // Adiciona definição de segurança para autenticação Bearer (JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o seu token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Recupera informações do JWT do appsettings.json
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Configura a autenticação com JWT Bearer
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
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Debug.WriteLine("Falha na autenticação: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Debug.WriteLine("Token validado com sucesso para: " + context.Principal.Identity.Name);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Debug.WriteLine("Desafio de autenticação: " + context.ErrorDescription);
                return Task.CompletedTask;
            }
        };
    });

// Adiciona suporte a controllers com JSON e ignora ciclos de referência (evita loop de serialização)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Injeta dependências para os repositórios personalizados
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ILogradouroRepository, LogradouroRepository>();

// Configura o ASP.NET Identity para gerenciamento de usuários e papéis com EF Core
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Injeta um serviço customizado de envio de e-mails (pode ser simulado)
builder.Services.AddScoped<IEmailSender<IdentityUser>, EmailSender>();



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
