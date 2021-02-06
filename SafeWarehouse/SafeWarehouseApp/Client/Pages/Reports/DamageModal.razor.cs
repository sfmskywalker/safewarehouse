using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    partial class DamageModal
    {
        [CascadingParameter] public BlazoredModalInstance Modal { get; set; } = default!;
        [Parameter] public Damage Damage { get; set; } = new();
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        private EditContext DamageContext { get; set; } = default!;
        private DamageDetail CurrentDamageDetail { get; set; } = new();
        private EditContext DamageDetailContext { get; set; } = default!;
        private bool ShowDamageDetailForm { get; set; }
        private ICollection<Material> Materials { get; set; } = new List<Material>();
        private ICollection<DamageType> DamageTypes { get; set; } = new List<DamageType>();

        protected override void OnInitialized()
        {
            DamageContext = new EditContext(Damage);
            DamageDetailContext = new EditContext(CurrentDamageDetail);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Materials = (await DbContext.Materials.GetAllAsync()).OrderBy(x => x.Name).ToList();
                DamageTypes = (await DbContext.DamageTypes.GetAllAsync()).OrderBy(x => x.Title).ToList();
                StateHasChanged();
            }
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
            if(CurrentDamageDetail.Id == null!)
            {
                CurrentDamageDetail.Id = Guid.NewGuid().ToString("N");
                Damage.Details.Add(CurrentDamageDetail);
            }
            
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