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
        private EditContext DamageContext { get; set; } = default!;
        private ICollection<DamageType> DamageTypes { get; set; } = new List<DamageType>();
        private IDictionary<string, File> DamagePictureFiles { get; set; } = new Dictionary<string, File>();
        private IDictionary<string, Material> MaterialLookup { get; set; } = new Dictionary<string, Material>();

        protected override async Task OnInitializedAsync()
        {
            DamageContext = new EditContext(Damage);
            await LoadDamagePicturesAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                DamageTypes = (await DbContext.DamageTypes.GetAllAsync()).OrderBy(x => x.Title).ToList();
                MaterialLookup = (await DbContext.Materials.GetAllAsync()).ToDictionary(x => x.Id);
                StateHasChanged();
            }
        }

        private async Task LoadDamagePicturesAsync()
        {
            foreach (var damagePicture in Damage.Pictures.Where(x => x.PictureId is not null and not "")) 
                DamagePictureFiles[damagePicture.PictureId!] = await DbContext.Files.GetAsync(damagePicture.PictureId);
        }

        private async Task OnValidSubmit()
        {
            await Modal.CloseAsync(ModalResult.Ok(Damage));
        }
        
        private async Task OnAddMaterialClick()
        {
            var requiredMaterial = new RequiredMaterial
            {
                MaterialId = MaterialLookup.Keys.FirstOrDefault()!
            };

            await OnEditMaterialClick(requiredMaterial);
            var existingRequiredMaterial = Damage.RequiredMaterials.FirstOrDefault(x => x.MaterialId == requiredMaterial.MaterialId);

            // If we already have an entry with the specified material, simply increase its quantity with the selected amount.
            if (existingRequiredMaterial != null)
            {
                existingRequiredMaterial.Quantity += requiredMaterial.Quantity;
                requiredMaterial = existingRequiredMaterial;
            }
            else
                Damage.RequiredMaterials.Add(requiredMaterial);

            if (requiredMaterial.Quantity <= 0) 
                Damage.RequiredMaterials.Remove(requiredMaterial);
        }
        
        private async Task OnEditMaterialClick(RequiredMaterial requiredMaterial)
        {
            var modalParameters = new ModalParameters();
            modalParameters.Add(nameof(RequiredMaterialModal.RequiredMaterial), requiredMaterial);
            var reference = ModalService.Show<RequiredMaterialModal>("Benodigd materiaal", modalParameters);
            await reference.Result;
        }
        
        private void OnDeleteMaterialClick(RequiredMaterial requiredMaterial)
        {
            Damage.RequiredMaterials.Remove(requiredMaterial);
            StateHasChanged();
        }
        
        private async Task OnAddPictureClick()
        {
            var damagePicture = new DamagePicture
            {
                Number = Damage.Pictures.Count + 1
            };
            Damage.Pictures.Add(damagePicture);
            await OnEditPictureClick(damagePicture);
        }
        
        private async Task OnEditPictureClick(DamagePicture damagePicture)
        {
            var modalParameters = new ModalParameters();
            modalParameters.Add(nameof(DamagePictureModal.DamagePicture), damagePicture);
            var reference = ModalService.Show<DamagePictureModal>("Schade foto", modalParameters);
            await reference.Result;
            await LoadDamagePicturesAsync();
        }
        
        private void OnDeletePictureClick(DamagePicture damagePicture)
        {
            Damage.Pictures.Remove(damagePicture);
            Damage.UpdatePictureNumbers();
            StateHasChanged();
        }
    }
}