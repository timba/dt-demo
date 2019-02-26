using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using DTDemo.DealProcessing;
using DTDemo.DealProcessing.Csv;
using DTDemo.Server.Controllers;
using DTDemo.Server.Hubs;

namespace DTDemo.Server
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ui";
            });

            services.AddSingleton<IDealRecordService>(cnt => 
                new DealRecordService(cnt.GetService<IRecordParser>(), true));

            services.AddTransient<IDealRecordStatAccumulator, DealRecordStatAccumulator>();

            services.AddSingleton<DealsDataControllerConfig>(new DealsDataControllerConfig(
                // Codepage 28591 corresponds to 8-bit ASCII based character set ISO/IEC 8859-1 (Western European)
                csvFileEncoding: Encoding.GetEncoding(28591), 
                clientBufferSize: 500
            ));

            services.AddSingleton<IRecordParser, RecordParser>();
            services.AddSingleton<IParser[]>(new IParser[] {
                new InitialParser(','),
                new GenericParser(','),
                new StringParser(','),
                new QuoteParser(','),
                new NewlineParser(',')
            });

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<DealsHub>("/dealshub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "../ui";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
