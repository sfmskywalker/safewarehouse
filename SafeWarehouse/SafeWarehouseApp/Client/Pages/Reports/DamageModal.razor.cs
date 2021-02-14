using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Pages.Reports
{
    partial class DamageModal
    {
        [CascadingParameter] public BlazoredModalInstance Modal { get; set; } = default!;
        [Parameter] public Damage Damage { get; set; } = new();
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        [Inject] private IModalService ModalService { get; set; } = default!;
        [Inject] private Cloner Cloner { get; set; } = default!;
        private EditContext DamageContext { get; set; } = default!;
        private ICollection<Material> Materials { get; set; } = new List<Material>();
        private ICollection<DamageType> DamageTypes { get; set; } = new List<DamageType>();

        protected override void OnInitialized()
        {
            DamageContext = new EditContext(Damage);
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
        
        private async Task OnAddPictureClick()
        {
            var damagePicture = new DamagePicture();
            var modalParameters = new ModalParameters();
            modalParameters.Add(nameof(DamagePictureModal.DamagePicture), damagePicture);
            var reference = ModalService.Show<DamagePictureModal>("Nieuwe schade foto", modalParameters);
            var result = await reference.Result;

            if (result.Cancelled)
                return;

            damagePicture = (DamagePicture) result.Data;
            Damage.Pictures.Add(damagePicture);
        }
        
        private async Task OnEditPictureClick(DamagePicture damagePicture)
        {
            var clone = Cloner.Clone(damagePicture);
            var modalParameters = new ModalParameters();
            modalParameters.Add(nameof(DamagePictureModal.DamagePicture), clone);
            var reference = ModalService.Show<DamagePictureModal>("Bewerk schade foto", modalParameters);
            var result = await reference.Result;

            if (result.Cancelled)
                return;

            clone = (DamagePicture) result.Data;
            Cloner.Update(damagePicture, clone);
        }
        
        private void OnDeletePictureClick(DamagePicture damagePicture)
        {
            Damage.Pictures.Remove(damagePicture);
            StateHasChanged();
        }
    }
}