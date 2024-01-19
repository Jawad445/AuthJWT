using Auth_Jwt;
using Auth_Jwt.Infrastructure;
using Auth_Jwt.Infrastructure.Security;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.Win32;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

//Adding Connection String for sql lite
builder.Services.AddTransient<IDbConnection>(
            db => new SqliteConnection(
                builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEntityFrameworkSqlite().AddDbContext<DataContext>();

builder.Services.AddAuthorization();

//Swagger Configurations for authentication using bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x => { 
        x.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme 
                    {
                        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "bearer"
                    });

                var requirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                };

                x.AddSecurityRequirement(requirement);
                x.SwaggerDoc("v1",new OpenApiInfo { Title = "Expenses API", Version = "v1" });
                x.CustomSchemaIds(y => y.FullName);
                x.DocInclusionPredicate( (version, apiDescription) => true);
            });

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));
builder.Services.Configure<PasswordHasherSettings>(builder.Configuration.GetSection(nameof(PasswordHasherSettings)));

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddCors();
builder.Services.AddJwt();


var app = builder.Build();

app.UseMiddlewares();

app.UseCors(builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("Token-Expired"));

app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });

app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "JWT API V1");
    });

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapPost("/login", (Login.Command command) =>
{
});

app.MapPost("/register", (Register.Command command) =>
{

});

app.MapPost("/refreshtoken", (ExchangeRefreshToken.Command command) =>
{

});


app.MapGet("/User", (HttpContext httpContext) =>
{
    return "Hello World";
}).WithName("GetUser").WithOpenApi();

app.Run();