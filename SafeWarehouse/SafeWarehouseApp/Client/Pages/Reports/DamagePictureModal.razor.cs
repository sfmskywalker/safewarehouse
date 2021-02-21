using System;
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
        private File? Picture { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DamageContext = new EditContext(DamagePicture);
            Picture = DamagePicture.PictureId is not null and not "" ? await DbContext.Files.GetAsync(DamagePicture.PictureId) : default;
        }

        private async Task OnValidSubmit() => await Modal.CloseAsync();

        private async Task OnFileChanged(InputFileChangeEventArgs e)
        {
            var resizedImage = await e.File.RequestImageFileAsync(e.File.ContentType, 1024, 1024);

            var picture = new File
            {
                Id = Guid.NewGuid().ToString("N"),
                Data = await resizedImage.ReadStreamAsync(),
                ContentType = e.File.ContentType,
                FileName = Path.GetFileName(e.File.Name)
            };

            await DbContext.Files.PutAsync(picture);

            if (DamagePicture.PictureId is not null and not "")
                await DbContext.Files.DeleteAsync(DamagePicture.PictureId);

            DamagePicture.PictureId = picture.Id;
            Picture = picture;
        }
    }
}