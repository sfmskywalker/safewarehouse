using System.IO;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SafeWarehouseApp.Client.Extensions;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Shared.Models;
using File = SafeWarehouseApp.Shared.Models.File;

namespace SafeWarehouseApp.Client.Pages.Reports
{
    partial class DamagePictureModal
    {
        [CascadingParameter] public BlazoredModalInstance Modal { get; set; } = default!;
        [Parameter] public DamagePicture DamagePicture { get; set; } = new();
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        private EditContext DamageContext { get; set; } = default!;

        protected override void OnInitialized()
        {
            DamageContext = new EditContext(DamagePicture);
        }

        private async Task Close() => await Modal.CloseAsync(ModalResult.Ok(true));
        private async Task Cancel() => await Modal.CancelAsync();

        private async Task OnValidSubmit()
        {
            await Modal.CloseAsync(ModalResult.Ok(DamagePicture));
        }
        
        private async Task OnFileChanged(InputFileChangeEventArgs e)
        {
            var resizedImage = await e.File.RequestImageFileAsync(e.File.ContentType, 1024, 1024);

            DamagePicture.Picture = new File
            {
                Data = await resizedImage.ReadStreamAsync(),
                ContentType = e.File.ContentType,
                FileName = Path.GetFileName(e.File.Name)
            };
        }
    }
}