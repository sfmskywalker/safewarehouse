using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace SafeWarehouseApp.Client.Services
{
    public class FileDownloader
    {
        private readonly IJSRuntime _jsRuntime;
        private IJSObjectReference? _module;
        
        public FileDownloader(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task DownloadFromUrlAsync(DownloadFromUrlOptions options) => await (await GetModuleAsync()).InvokeVoidAsync("downloadFromUrl", options);
        public async Task DownloadFromBytesAsync(DownloadFromBytesOptions options) => await (await GetModuleAsync()).InvokeVoidAsync("downloadFromBytes", options);
        
        private async ValueTask<IJSObjectReference> GetModuleAsync() => _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/fileDownloader.js");
    }

    public record DownloadFromUrlOptions(Uri Url, string FileName);
    public record DownloadFromBytesOptions(byte[] Content, string FileName, string ContentType);
}