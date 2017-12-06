using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OffertTemplateTool.DAL.Context;
using OffertTemplateTool.DAL.Models;
using OffertTemplateTool.DAL.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OffertTemplateTool.Connectors;
using DinkToPdf.Contracts;
using DinkToPdf;
using OffertTemplateTool.TemplateService;

namespace OffertTemplateTool
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
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddAzureAd(options => Configuration.Bind("AzureAd", options))
            .AddCookie();

            DataBaseContext.ConnectionString = Configuration.GetConnectionString("DataBaseContext");
            services.AddDbContext<DataBaseContext>();

            services.AddScoped<IRepository<Offers>, OfferRepository>();
            services.AddScoped<IRepository<Users>, UsersRepository>();
            services.AddScoped<IRepository<Settings>, SettingsRepository>();
            services.AddScoped<IRepository<Estimates>, EstimateRepository>();
            services.AddScoped<IRepository<EstimateLines>, EstimateLinesRepository>();
            services.AddScoped<IRepository<EstimateConnects>, EstimateConnectsRepository>();
            services.AddScoped<IConnector, WeFactConnector>();
            services.AddScoped<ITemplateService, TemplateServiceClass>();

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
