using Boxed.Mapping;
using DemoApiMongo.Configuration;
using DemoApiMongo.Entities.DataModels;
using DemoApiMongo.Entities.Mappers;
using DemoApiMongo.Entities.ViewModels;
using DemoApiMongo.Filter;
using DemoApiMongo.Repository;
using DemoApiMongo.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// logs entry
Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
    .WriteTo.File("log/productsLogs.txt", rollingInterval: RollingInterval.Day).CreateBootstrapLogger();
builder.Host.UseSerilog();


// DB Settings
builder.Services.Configure<ProductDBSettings>(
builder.Configuration.GetSection("ProductDatabase"));


// service implementation
builder.Services.AddSingleton<IProductRepo, ProductRepo>();
builder.Services.AddSingleton<IUserRepo, UserRepo>();

// authorization & authentication
var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x => {
x.RequireHttpsMetadata = false;
x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
});


// exception handling
//builder.Services.AddSingleton<ExceptionLoggingFilter>();
builder.Services.AddControllers(options => options.Filters.Add(typeof(ExceptionLoggingFilter))).AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//AutoMapping
builder.Services.AddAutoMapper(typeof(AutoMapping));

//BoxedMapping
builder.Services.AddTransient<IMapper<ProductDetailModel, ProductDetails>, BoxedMapping>();


// jwt settings
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
            "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
