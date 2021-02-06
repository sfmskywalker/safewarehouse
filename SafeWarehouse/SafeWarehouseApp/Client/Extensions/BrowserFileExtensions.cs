using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace SafeWarehouseApp.Client.Extensions
{
    public static class BrowserFileExtensions
    {
        private const long MaxFileSize = 104857600;
        
        public static async Task<string> ReadStreamAsync(this IBrowserFile file)
        {
            var buffer = new byte[file.Size];
            await file.OpenReadStream(MaxFileSize).ReadAsync(buffer);
            return Convert.ToBase64String(buffer);
        }
    }
}