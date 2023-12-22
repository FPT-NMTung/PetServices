using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetServices.Mapper;
using PetServices.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PetServicesContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DbConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5, // Số lần thử lại tối đa
            maxRetryDelay: TimeSpan.FromSeconds(30), // Thời gian chờ tối đa giữa các lần thử lại
            errorNumbersToAdd: null // Danh sách mã lỗi cụ thể để thử lại (nếu cần)
        )
    );
});

builder.Services.AddCors();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    };

    c.AddSecurityRequirement(securityRequirement);
});

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new Mapping());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManaOnly", policy => policy.RequireRole("MANAGER"));
    options.AddPolicy("CusOnly", policy => policy.RequireRole("CUSTOMER"));
    options.AddPolicy("PartnerOnly", policy => policy.RequireRole("PARTNER"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("ADMIN"));
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
    });
}
app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod();
});


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
