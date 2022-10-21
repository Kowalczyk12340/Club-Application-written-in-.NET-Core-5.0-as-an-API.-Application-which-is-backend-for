using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ClubsAPI.Services.Interfaces;
using ClubsAPI.Services;
using FluentValidation;
using ClubsAPI.DTOs;
using ClubsAPI.Validations;
using ClubsAPI.Seeders;
using System.Threading.Tasks;

namespace ClubsAPI
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<ApplicationDataContext>(options =>
      options.UseSqlServer(Configuration.GetConnectionString("Database"),
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

      services.AddAutoMapper(typeof(Startup));
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
                          Encoding.UTF8.GetBytes(Configuration["keyjwt"])),
              ClockSkew = TimeSpan.Zero
            };
          });

      services.AddAuthorization(options =>
      {
        options.AddPolicy("IsAdmin", policy => policy.RequireClaim("role", "admin"));
      });
      services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
      services.AddSingleton<IClock, SystemClock>(x => SystemClock.Instance);
      services.AddScoped<ErrorHandlingMiddleware>();
      services.AddScoped<RequestTimeMiddleware>();
      services.AddScoped<ClubSeeder>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ClubSeeder seed)
    {
      seed.Seed();
      app.UseResponseCaching();
      if (env.IsDevelopment())
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
    }
  }
}