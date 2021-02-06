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
        private Report Report { get; } = CreateReport();
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        private string? SelectedImageUrl => Report.File?.GetImageDataUrl();

        private async Task OnFileChanged(InputFileChangeEventArgs e)
        {
            var resizedImage = await e.File.RequestImageFileAsync(e.File.ContentType, 1024, 1024);

            Report.File = new File
            {
                Data = await resizedImage.ReadStreamAsync(),
                ContentType = e.File.ContentType,
                FileName = Path.GetFileName(e.File.Name)
            };
        }

        private static Report CreateReport() => new()
        {
            Id = Guid.NewGuid().ToString("N"),
            Title = "Concept",
            Date = DateTime.Now
        };

        private async Task OnNextButtonClick()
        {
            if (Report.File.FileName == null!)
                return;
            
            await DbContext.Reports.PutAsync(Report);
            NavigationManager.NavigateTo($"report/{Report.Id}");
        }
    }
}