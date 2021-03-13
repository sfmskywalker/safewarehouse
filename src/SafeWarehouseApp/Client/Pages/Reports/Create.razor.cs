using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SafeWarehouseApp.Client.Extensions;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Shared.Models;
using File = SafeWarehouseApp.Shared.Models.File;

namespace SafeWarehouseApp.Client.Pages.Reports
{
    partial class Create
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;

        private async Task OnFileChanged(InputFileChangeEventArgs e)
        {
            var image = await e.File.ReadStreamAsync();
            var resizedImage = await e.File.RequestImageFileAsync(e.File.ContentType, 600, 600);
            var fileName = Path.GetFileName(e.File.Name);

            var originalSchematic = new File
            {
                Id = Guid.NewGuid().ToString("N"),
                Data = image,
                ContentType = e.File.ContentType,
                FileName = fileName
            };

            var resizedSchematic = new File
            {
                Id = Guid.NewGuid().ToString("N"),
                Data = await resizedImage.ReadStreamAsync(),
                ContentType = e.File.ContentType,
                FileName = $"{Path.GetFileNameWithoutExtension(fileName)}-resized{Path.GetExtension(fileName)}"
            };
            
            var report = new Report
            {
                Id = Guid.NewGuid().ToString("N"),
                Date = DateTime.Now,
                OriginalSchematicId = originalSchematic.Id,
                SchematicPhotoId = resizedSchematic.Id,
            };

            await DbContext.Files.PutAsync(originalSchematic);
            await DbContext.Files.PutAsync(resizedSchematic);
            await DbContext.Reports.PutAsync(report);
            
            NavigationManager.NavigateTo($"reports/{report.Id}");
        }
    }
}