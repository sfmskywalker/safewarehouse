using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Blazored.Modal;
using SafeWarehouseApp.Client.Services;

namespace SafeWarehouseApp.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            var services = builder.Services;
            
            builder.RootComponents.Add<App>("#app");

            services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            services.AddScoped<SafeWarehouseContext>();
            services.AddBlazoredModal();
            services.AddAutoMapper(x => x.AddMaps(typeof(Program)));
            services.AddScoped<PdfGenerator>();
            services.AddScoped<Printer>();
            services.AddScoped<FileDownloader>();

            await builder.Build().RunAsync();
        }
    }
}
