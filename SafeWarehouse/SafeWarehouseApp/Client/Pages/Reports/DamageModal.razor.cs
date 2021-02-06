using System;
using System.IO;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SafeWarehouseApp.Client.Extensions;
using SafeWarehouseApp.Shared.Models;
using File = SafeWarehouseApp.Shared.Models.File;

namespace SafeWarehouseApp.Client.Pages.Reports
{
    partial class DamageModal
    {
        [CascadingParameter] public BlazoredModalInstance Modal { get; set; } = default!;
        [Parameter] public Damage Damage { get; set; } = new();
        private EditContext DamageContext { get; set; } = default!;
        private DamageDetail CurrentDamageDetail { get; set; } = new();
        private EditContext DamageDetailContext { get; set; } = default!;
        private bool ShowDamageDetailForm { get; set; }

        protected override void OnInitialized()
        {
            DamageContext = new EditContext(Damage);
            DamageDetailContext = new EditContext(CurrentDamageDetail);
        }

        private async Task Close() => await Modal.CloseAsync(ModalResult.Ok(true));
        private async Task Cancel() => await Modal.CancelAsync();

        private async Task OnValidSubmit()
        {
            await Modal.CloseAsync(ModalResult.Ok(Damage));
        }

        private void OnEditDamageDetailClick(DamageDetail damageDetail)
        {
            CurrentDamageDetail = damageDetail;
            DamageDetailContext = new EditContext(CurrentDamageDetail);
            ShowDamageDetailForm = true;
        }
        
        private void OnDeleteDamageDetailClick(DamageDetail damageDetail)
        {
            Damage.Details.Remove(damageDetail);
            StateHasChanged();
        }

        private void OnAddDamageDetailClick()
        {
            CurrentDamageDetail = new DamageDetail();
            DamageDetailContext = new EditContext(CurrentDamageDetail);
            ShowDamageDetailForm = true;
        }

        private void OnCancelAddDamageDetailClick()
        {
            ShowDamageDetailForm = false;
        }
        
        private void OnValidDetailSubmit()
        {
            Damage.Details.Add(CurrentDamageDetail);
            ShowDamageDetailForm = false;
        }
        
        private async Task OnFileChanged(InputFileChangeEventArgs e)
        {
            var resizedImage = await e.File.RequestImageFileAsync(e.File.ContentType, 1024, 1024);

            CurrentDamageDetail.Picture = new File
            {
                Data = await resizedImage.ReadStreamAsync(),
                ContentType = e.File.ContentType,
                FileName = Path.GetFileName(e.File.Name)
            };
        }
    }
}