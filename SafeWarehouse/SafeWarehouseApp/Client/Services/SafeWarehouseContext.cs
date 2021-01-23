using System.Threading.Tasks;
using Microsoft.JSInterop;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Services
{
    public class SafeWarehouseContext
    {
        private readonly IJSRuntime _jsRuntime;
        private IJSObjectReference? _module;
        
        public SafeWarehouseContext(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            Materials = new Store<Material>("materials", GetModuleAsync);
            DamageTypes = new Store<DamageType>("damageTypes", GetModuleAsync);
            Reports = new Store<Report>("reports", GetModuleAsync);
        }

        public Store<Material> Materials { get;  }
        public Store<DamageType> DamageTypes { get; }
        public Store<Report> Reports { get; }
        
        private async ValueTask<IJSObjectReference> GetModuleAsync() => _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/dbContext.js");
    }
}