using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Pages.Reports
{
    partial class RequiredMaterialModal
    {
        [CascadingParameter] public BlazoredModalInstance Modal { get; set; } = default!;
        [Parameter] public RequiredMaterial RequiredMaterial { get; set; } = new();
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        private EditContext RequiredMaterialContext { get; set; } = default!;
        private ICollection<Material> Materials { get; set; } = new List<Material>();

        protected override void OnInitialized()
        {
            RequiredMaterialContext = new EditContext(RequiredMaterial);
        }

        protected override void OnParametersSet()
        {
            if (RequiredMaterial.MaterialId is null or "")
                RequiredMaterial.MaterialId = Materials.FirstOrDefault()?.Id!;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Materials = (await DbContext.Materials.GetAllAsync()).OrderBy(x => x.Name).ToList();
                StateHasChanged();
            }
        }
        
        private async Task OnValidSubmit() => await Modal.CloseAsync();
    }
}