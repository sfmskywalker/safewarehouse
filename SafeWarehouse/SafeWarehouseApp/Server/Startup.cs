using System;
using System.Net.Http;
using AutoMapper;
using Blazored.Modal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SafeWarehouseApp.Client.Services;

namespace SafeWarehouseApp.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddScoped(sp => new HttpClient {BaseAddress = new Uri("https://localhost:5001")});
            services.AddScoped<SafeWarehouseContext>();
            services.AddBlazoredModal();
            services.AddAutoMapper(x => x.AddMaps(typeof(Program)));
            services.AddSingleton<Cloner>();
            
            if (Program.HostingModel == BlazorHostingModel.Server)
            {
                services.AddServerSideBlazor(options =>
                {
                    options.DetailedErrors = !Environment.IsProduction();
                    options.JSInteropDefaultCallTimeout = TimeSpan.FromSeconds(30);
                }).AddHubOptions(options => options.MaximumReceiveMessageSize = 100 * 1024 * 1024); // 100mb
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                if (Program.HostingModel == BlazorHostingModel.WebAssembly)
                    app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            if (Program.HostingModel == BlazorHostingModel.WebAssembly)
                app.UseBlazorFrameworkFiles();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();

                if (Program.HostingModel == BlazorHostingModel.WebAssembly)
                    endpoints.MapFallbackToFile("index.html");
                else
                {
                    endpoints.MapBlazorHub();
                    endpoints.MapFallbackToPage("/_Host");
                }
            });
        }
    }
}