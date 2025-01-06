using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RecargaApi.Data;
using System.IdentityModel.Tokens.Jwt;
using RecargaApi.middlewares;
using RecargaApi.Filters;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<RecargaApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RecargaApiContext") ?? throw new InvalidOperationException("Connection string 'RecargaApiContext' not found.")));

// Add services to the container.

// Configuraci�n de autenticacion JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "http://localhost:8000", // URL del Sistema de Autenticación
        ValidAudience = "http://localhost:8000",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("django-insecure-+8+ig6g-!0a9di86)6#j4r48)qr)mi5(mezuhi*o=z_2+gqfi0")) // Reemplaza con tu clave secreta
    };

    // Configurar para leer el token desde una cookie
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Leer el token desde la cookie llamada "access"
            var token = context.Request.Cookies["access"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
                Console.WriteLine($"Token: {token}");

                // Validar el formato del token JWT
                try
                {
                    var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                    Console.WriteLine($"Decoded Token: {decodedToken}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al decodificar el token: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("No se encontró el token en la cookie.");
            }
            return Task.CompletedTask;
        },

    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<FileUploadOperationFilter>();
});

var app = builder.Build();

app.UseCors(options =>
{
    options.WithOrigins(
        "http://localhost:8000",
        "http://localhost:5173",
        "http://localhost:5174",
        "http://localhost:8001"
    )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();


app.UseAuthentication(); // Agregar autenticaci�n
app.UseMiddleware<RoleBasedAuthorizationMiddleware>(); // Agregar middleware de autorizaci�n
app.UseAuthorization();

app.MapControllers();

app.Run();
