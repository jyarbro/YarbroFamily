using App.Data;
using App.Data.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Nrrdio.Utilities.Web;
using Nrrdio.Utilities.Web.Models.Options;

// Scopes:
// Transient: created each time they are requested. This lifetime works best for lightweight, stateless services.
// Scoped: created once per request.
// Singleton: created the first time they are requested (or when ConfigureServices is run if you specify an instance there) and then every subsequent request will use the same instance.
//
// Notes:
// services.Configure<OptionsModel>(Configuration.GetSection("OptionsSection"));

namespace App {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<DataContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<AppUserService>();
            services.AddScoped<HomeService>();

            services.Configure<GzipWebClientOptions>((options) => {
                Configuration.GetSection(GzipWebClientOptions.Section).Bind(options);
            });

            services.AddScoped<GzipWebClient>();

            services.Configure<CookiePolicyOptions>(options => {
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.Secure = CookieSecurePolicy.Always;
                options.HandleSameSiteCookieCompatibility();
            });

            services
                .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));

            services.AddAuthorization(options => {
                options.AddPolicy("Admin", policy => policy.RequireClaim("wids", new[] { "cf1c38e5-3621-4004-a7cb-879624dced7c" }));
                options.AddPolicy("Parent", policy => policy.RequireClaim("wids", new[] { "9b895d92-2cd3-44c7-9d02-a6ac2d5ea5c3" }));
                options.FallbackPolicy = options.DefaultPolicy;
            });

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDistributedMemoryCache();

            services.AddSession();

            services
                .AddRazorPages(options => {
                    options.Conventions.AuthorizeAreaFolder("Admin", "/", "Admin");
                })
                .AddMvcOptions(options => {
                    var policyRequireAuthenticatedUser = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    options.Filters.Add(new AuthorizeFilter(policyRequireAuthenticatedUser));
                })
                .AddMicrosoftIdentityUI();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext dataContext) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else {
                app.UseExceptionHandler("/Error");
                app.UseHsts();

                dataContext.Database.Migrate();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints => {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
