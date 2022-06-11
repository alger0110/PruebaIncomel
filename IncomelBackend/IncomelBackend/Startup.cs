using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Repository;
using Microsoft.EntityFrameworkCore;
using Services;
using System.Security.Claims;

namespace IncomelBackend
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

            #region Cors
            services.AddCors(opt => opt.AddPolicy("ApiCorsPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
            }));
            #endregion

            #region Controllers
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            #endregion

            #region Configure
            services.Configure<TokenManagement>(Configuration.GetSection("tokenManagement"));
            services.Configure<EmailConfig>(Configuration.GetSection("emailConfig"));
            services.Configure<FrontendConfig>(Configuration.GetSection("frontEndConfig"));
            services.Configure<TemplateEmailsConfig>(Configuration.GetSection("templateEmails"));
            #endregion

            #region JWTBearer
            var token = Configuration.GetSection("tokenManagement").Get<TokenManagement>();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                    ValidIssuer = token.Issuer,
                    ValidAudience = token.Audience,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
            #endregion

            #region Context
            services.AddDbContext<IncomelDBContext>(opt => opt.UseNpgsql(Configuration.GetConnectionString("Incomel")));
            #endregion

            #region Swagger
            AddSwagger(services);
            #endregion

            #region Transient
            services.AddTransient<IUserService, UserRepository>();
            services.AddTransient<IAuthenticationService, AuthenticationRepository>();
            services.AddTransient<IEmailService, EmailRepository>();
            services.AddTransient<IEmployeeService, EmployeeRepository>();
            #endregion

            #region Policies
            services.AddAuthorization(opt =>
            {
                #region EmployeeControler
                opt.AddPolicy("Employees", poli => poli.RequireClaim(ClaimTypes.Role, "Employees"));
                opt.AddPolicy("CEmployee", poli => poli.RequireClaim("Employees", "1"));
                opt.AddPolicy("REmployee", poli => poli.RequireClaim("Employees", "2"));
                opt.AddPolicy("UEmployee", poli => poli.RequireClaim("Employees", "3"));
                opt.AddPolicy("DEmployee", poli => poli.RequireClaim("Employees", "4"));
                #endregion
            });
            #endregion

        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                var groupName = "v1";

                options.SwaggerDoc(groupName, new OpenApiInfo
                {
                    Title = $"Incomel Backend API {groupName}",
                    Version = groupName,
                    Description = "Incomel Backend API",
                    Contact = new OpenApiContact
                    {
                        Name = "Incomel",
                        Email = string.Empty,
                        Url = new Uri("https://foo.com/"),
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(opt => opt.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Foo API V1");
            });

            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            string cS = Configuration.GetConnectionString("Logs");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.PostgreSQL(cS,"IncomelLogs", needAutoCreateTable: true)
                .MinimumLevel.Error()
                .CreateLogger();
        }
    }
}
