using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using SkyCommerce.Data.Configuration;
using SkyCommerce.Data.Context;
using SkyCommerce.Site.Configure;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;

namespace SkyCommerce.Site
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
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            services.AddHttpClient();


            //serve para o aspnetcore nao associar os schemas xml ao nome default das claims
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            if (Debugger.IsAttached)
            {
                IdentityModelEventSource.ShowPII = true;
            }

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookius";
                options.DefaultChallengeScheme = "oidc";
            }).AddCookie("Cookius")
              .AddOpenIdConnect("oidc", options =>
              {
                  options.Authority = "https://localhost:5001";
                  options.ClientId = "b23dcc22b84d4e5790b5aa76080e7fb0";
                  options.ClientSecret = "72c109c367f44fc1b1abd7ff5f636875";
                  options.ResponseType = "code";
                  options.Scope.Add("profile");
                  options.Scope.Add("openid");
                  options.SaveTokens = true;
                  options.GetClaimsFromUserInfoEndpoint = true;

                  options.TokenValidationParameters = new TokenValidationParameters()
                  {
                      NameClaimType = "name",
                      RoleClaimType = "role"
                  };
              });



            // Dbcontext config
            services.ConfigureProviderForContext<SkyContext>(DetectDatabase);

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/conta/entrar");
            });
            services.ConfigureSkyCommerce();
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

            // Definindo a cultura padrão: pt-BR
            var supportedCultures = new[] { new CultureInfo("pt-BR") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(culture: "pt-BR", uiCulture: "pt-BR"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// it's just a tuple. Returns 2 parameters.
        /// Trying to improve readability at ConfigureServices
        /// </summary>
        private (DatabaseType, string) DetectDatabase => (
            Configuration.GetValue<DatabaseType>("ApplicationSettings:DatabaseType"),
            Configuration.GetConnectionString("DefaultConnection"));
    }
}
