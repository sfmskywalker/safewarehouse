using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace SafeWarehouseApp.Client.Services
{
    public class Printer
    {
        private readonly IJSRuntime _jsRuntime;
        private IJSObjectReference? _module;
        
        public Printer(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task PrintAsync() => await (await GetModuleAsync()).InvokeVoidAsync("print");

        private async ValueTask<IJSObjectReference> GetModuleAsync() => _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/printer.js");
    }
}