using BusinessLayer.Interface;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLoggers.Interface;
using NLoggers.Services;
using RepositoryLayer;
using RepositoryLayer.Interface;
using RepositoryLayer.Services;
using RepositoryLayer.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotes_EFCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<FundooContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Fundoonotes")));

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("THIS_IS_MY_KEY_TO_GENERATE_TOKEN")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            services.AddSwaggerGen(
               setup =>
               {
                   // Include 'SecurityScheme' to use JWT Authentication
                   var jwtSecurityScheme = new OpenApiSecurityScheme
                   {
                       Scheme = "bearer",
                       BearerFormat = "JWT",
                       Name = "JWT Authentication",
                       In = ParameterLocation.Header,
                       Type = SecuritySchemeType.Http,
                       Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
                       Reference = new OpenApiReference
                       {
                           Id = JwtBearerDefaults.AuthenticationScheme,
                           Type = ReferenceType.SecurityScheme,
                       },
                   };
                   setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                   setup.AddSecurityRequirement(new OpenApiSecurityRequirement
               {
                    { jwtSecurityScheme, Array.Empty<string>() },
               });
               });
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddTransient<IUserRL, UserRL>();
            services.AddTransient<IUserBL, UserBL>();
            services.AddTransient<INoteRL, NoteRL>();
            services.AddTransient<INoteBL, NoteBL>();
            services.AddTransient<ILabelRL, LabelRL>();
            services.AddTransient<ILabelBL, LabelBL>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My FundooNotes API");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}