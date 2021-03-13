using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Pages.Materials
{
    partial class Edit
    {
        [Parameter] public string? Id { private get; set; }
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        private Material Material { get; set; } = new();
        private EditContext MaterialContext { get; set; } = default!;
        private bool HasRendered { get; set; }

        protected override void OnInitialized()
        {
            MaterialContext = new EditContext(Material);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (HasRendered) 
                await LoadMaterialAsync();
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadMaterialAsync();
                HasRendered = true;
                StateHasChanged();
            }
        }
        
        private async Task LoadMaterialAsync()
        {
            if (Id is null or "")
                return;
            
            var material = await DbContext.Materials.GetAsync(Id);
            SetMaterial(material);
        }
        
        private void SetMaterial(Material material)
        {
            MaterialContext = new EditContext(material);
            Material = material;
        }

        private async Task SaveChangesAsync() => await DbContext.Materials.PutAsync(Material);

        private async Task OnValidSubmit()
        {
            if (Material.Id is null or "")
            {
                Material.Id = Guid.NewGuid().ToString();
            }

            await SaveChangesAsync();
            NavigationManager.NavigateTo("materials");
        }
    }
}