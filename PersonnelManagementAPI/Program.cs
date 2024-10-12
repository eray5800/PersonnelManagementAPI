using BAL.CompanyServices;
using BAL.EmailServices;
using BAL.EmployeeServices;
using BAL.RoleServices;
using DAL.Core;
using DAL.Core.IConfiguration;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    x.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<PersonnelManagementDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MssqlConnectionString"));
});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});





builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());


builder.Services.AddIdentity<Employee, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<PersonnelManagementDBContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero // Varsayýlan 5 dakika clock skew'u devre dýþý býrakmak için
    };

    // Token oluþturulurken ExpireMinutes ayarýný kullanarak token süresini belirleyin
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Burada token oluþturma sýrasýnda Expiration süresini ayarlayabilirsiniz
            var expireMinutes = int.Parse(builder.Configuration["Jwt:ExpireMinutes"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Token'i burada response'a ekleyebilirsiniz veya manuel olarak bir yerde kullanabilirsiniz.
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<CompanyRequestService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<EmailService>();



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Employee>>();
    var roleService = new RoleService(userManager, roleManager);
    await roleService.SeedRolesAsync();

    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    string adminEmail = config["AdminUser:Email"];
    string adminPassword = config["AdminUser:Password"];

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var newUser = new Employee { UserName = "adminUser", Email = adminEmail, EmailConfirmed = true, IsActive = true };
        var createResult = await userManager.CreateAsync(newUser, adminPassword);
        if (createResult.Succeeded)
        {
            await roleService.AssignRoleAsync(newUser.Id.ToString(), "SystemAdministrator");
        }
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
