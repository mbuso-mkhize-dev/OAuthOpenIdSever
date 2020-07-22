using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OAuthOpenIdServer.ApplicationLogic.Boundaries.ApplicationLogic.Services;
using OAuthOpenIdServer.ApplicationLogic.Boundaries.EntityFramework.Repositories;
using OAuthOpenIdServer.ApplicationLogic.Entities.ApplicationSettings;
using OAuthOpenIdServer.ApplicationLogic.Services;
using OAuthOpenIdServer.EntityFramework;
using OAuthOpenIdServer.EntityFramework.Core.Models;
using OAuthOpenIdServer.EntityFramework.Repositories.Base;
using OAuthOpenIdServer.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OAuthOpenIdServer
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
            services.AddControllersWithViews();

            services.AddDbContext<ApplicationDatabaseContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

                //options.UseOpenIddict();
            });

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<ApplicationDatabaseContext>()
            .AddDefaultTokenProviders();

            var authSettings = Configuration.GetSection("Auth");

            services.Configure<AuthSettingsEntity>(authSettings);

            string authIssuer = authSettings["Issuer"];
            string authAudienceId = authSettings["Audience:Id"];
            string authAudienceSecret = authSettings["Audience:Secret"];

            if (!long.TryParse(authSettings["AccessToken:LifetimeInSeconds"], out long accessTokenLifetimeInSeconds))
            {
                accessTokenLifetimeInSeconds = 60 * 60 * 24; // 1 day
            }

            if (!long.TryParse(authSettings["RefreshToken:LifetimeInSeconds"], out long refreshTokenLifetimeInSeconds))
            {
                refreshTokenLifetimeInSeconds = 60 * 60 * 24 * 90; // 90 days
            }

            // services.AddAuthentication(x =>
            // {
            //     x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //     x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            //     x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            // })
            //.AddOAuthValidation();

         

            var issuerSigningKey = new SymmetricSecurityKey(Base64UrlEncoder.DecodeBytes(authAudienceSecret));

            //// Configure Identity to use the same JWT claims as OpenIddict instead
            //// of the legacy WS-Federation claims it uses by default (ClaimTypes),
            //// which saves you from doing the mapping in your authorization controller.
            //services.Configure<IdentityOptions>(options =>
            //{
            //    options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
            //    options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
            //    options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            //});


            services.AddIdentityServer()
                .AddAspNetIdentity<User>()
                //.AddConfigurationStore(options =>
                //{
                //    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                //        sql => sql.MigrationsAssembly(assembly));
                //})
                //.AddOperationalStore(options =>
                //{
                //    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                //        sql => sql.MigrationsAssembly(assembly));
                //})
                //.AddSigningCredential(certificate);
                .AddInMemoryApiResources(ConfigurationHelper.GetApis())
                .AddInMemoryIdentityResources(ConfigurationHelper.GetIdentityResources())
                .AddInMemoryApiScopes(ConfigurationHelper.ApiScopes)
                .AddInMemoryClients(ConfigurationHelper.GetClients(authSettings["Client:Id"], authSettings["Client:Secret"]))
                .AddDeveloperSigningCredential();

            services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();

            services.AddScoped<IUserService, UserService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Campus Life Public API", Version = "v1" });

                //c.OperationFilter<FormFileOperationFilter>();

                //c.IncludeXmlComments(xmlPath);
            });

            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {

                        context.Response.Headers.Add("Application-Error", error.Error.Message);
                        // CORS
                        context.Response.Headers.Add("access-control-expose-headers", "Application-Error");
                        await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                    }
                });
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();
            app.UseCors("AllowAll");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //InitializeAsync(app.ApplicationServices, CancellationToken.None).GetAwaiter().GetResult();

            app.UseIdentityServer();
        }

        private async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken)
        {
            // Create a new service scope to ensure the database context is correctly disposed when this methods returns.
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDatabaseContext>();
                await context.Database.EnsureCreatedAsync();
            }
        }
    }
}