using ImageRepositoryW22.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ImageRepositoryW22.ImageRepository.Repositories;
using ImageRepositoryW22.Repositories.UserRepository;
using ImageRepositoryW22.Utilities.PasswordUtilities;
using ImageRepositoryW22.Utilities.OCRWrapper;

namespace ImageRepositoryW22
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private IServiceProvider _provider { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlite("Data Source=app.db;");
            });
            services.AddControllers();
            services.AddScoped<IImageRepository, ImageRepository.Repositories.ImageRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IPasswordUtilities, PasswordUtilities>();
            services.AddSingleton<IOCRWrapper, IronTesseractWrapper>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = OnTokenValidated
                };
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ImageRepositoryW22", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                logger.LogInformation("Using development settings.");
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ImageRepositoryW22 v1"));
            }

            if (Configuration["Jwt:Key"] == "SecretKeyYouShouldChange")
            {
                logger.LogWarning("For security you should change the JWT secret key in appsettings.json.");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            _provider = app.ApplicationServices;
        }
        private async Task OnTokenValidated(TokenValidatedContext context)
        {
            //Make sure the user in the claim is still a valid user (ex. hasn't deleted their account and is using an old token).
            var id = context.Principal.Claims.FirstOrDefault(claim => claim.Type == "id").Value;
            using (var scope = _provider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var exists = await repository.UserExists(Guid.Parse(id));

                if (!exists)
                {
                    context.Fail("Invalid username in token.");
                }
            }
            
            return;
        }
    }
}
