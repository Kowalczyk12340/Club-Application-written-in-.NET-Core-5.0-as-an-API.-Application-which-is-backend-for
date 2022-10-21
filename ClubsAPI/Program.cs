using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ClubsAPI.Data;
using ClubsAPI.Filters;
using ClubsAPI.Helpers;
using ClubsAPI.Middlewares;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NodaTime;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Cors;
using ClubsAPI.Services.Interfaces;
using ClubsAPI.Services;
using FluentValidation;
using ClubsAPI.DTOs;
using ClubsAPI.Validations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubsAPI.Seeders;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseNLog();

var config = builder.Configuration;
var services = builder.Services;

services.AddDbContext<ApplicationDataContext>(options =>
      options.UseSqlServer(config.GetConnectionString("Database"),
      sqlOptions => sqlOptions.UseNetTopologySuite()));
services.AddScoped<ApplicationDataContext>();
services.AddCors(options =>
{
  options.AddPolicy(
          "CorsPolicy",
          builder => builder.WithOrigins("http://localhost:4200")
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials()
          .WithExposedHeaders(new string[] { "totalAmountOfRecords" }));
});

services.AddAutoMapper(typeof(Program));
services.AddSingleton(provider => new MapperConfiguration(config =>
{
  var geometryFactory = provider.GetRequiredService<GeometryFactory>();
  config.AddProfile(new AutoMapperProfiles(geometryFactory));
}).CreateMapper());

services.AddSingleton<GeometryFactory>(NtsGeometryServices
    .Instance.CreateGeometryFactory(srid: 4326));

services.AddScoped<IFileStorageService, AzureStorageService>();
services.AddScoped<IFileStorageService, InAppStorageService>();
services.AddHttpContextAccessor();
services.AddScoped<IValidator<UserCredentials>, UserCredentialsValidator>();
services.AddScoped<IPasswordHasher<IdentityUser>, PasswordHasher<IdentityUser>>();
services.AddScoped<IClubLeaguesService, ClubLeaguesService>();
services.AddScoped<IClubsService, ClubsService>();
services.AddScoped<ICoachesService, CoachesService>();
services.AddScoped<INationalitiesService, NationalitiesService>();
services.AddScoped<IPlayersService, PlayersService>();
services.AddScoped<IRatingsService, RatingsService>();
services.AddControllers(options =>
{
  options.Filters.Add(typeof(MyExceptionFilter));
});

services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "ClubsAPI", Version = "v1" });
});

services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDataContext>()
    .AddDefaultTokenProviders();

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(config["keyjwt"])),
        ClockSkew = TimeSpan.Zero
      };
    });

services.AddAuthorization(options =>
{
  options.AddPolicy("IsAdmin", policy => policy.RequireClaim("role", "admin"));
});
services.AddApplicationInsightsTelemetry(config["APPINSIGHTS_CONNECTIONSTRING"]);
services.AddSingleton<IClock, SystemClock>(x => SystemClock.Instance);
services.AddScoped<ErrorHandlingMiddleware>();
services.AddScoped<RequestTimeMiddleware>();
services.AddScoped<IClubSeeder, ClubSeeder>();

var app = builder.Build();

await SeedDatabase();

app.UseResponseCaching();
if (builder.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseSwagger();
  app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClubsAPI v1"));
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();

app.UseEndpoints(endpoints =>
{
  endpoints.MapControllers();
});

app.Run();

async Task SeedDatabase()
{
  using var scope = app.Services.CreateScope();
  var dbSeeder = scope.ServiceProvider.GetRequiredService<IClubSeeder>();
  await dbSeeder.Seed();
}