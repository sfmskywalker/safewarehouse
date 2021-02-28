using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Pages.DamageTypes
{
    partial class Edit
    {
        [Parameter] public string? Id { private get; set; }
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        private DamageType DamageType { get; set; } = new();
        private EditContext DamageTypeContext { get; set; } = default!;
        private bool HasRendered { get; set; }

        protected override void OnInitialized()
        {
            DamageTypeContext = new EditContext(DamageType);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (HasRendered) 
                await LoadDamageTypeAsync();
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadDamageTypeAsync();
                HasRendered = true;
                StateHasChanged();
            }
        }
        
        private async Task LoadDamageTypeAsync()
        {
            if (Id is null or "")
                return;
            
            var material = await DbContext.DamageTypes.GetAsync(Id);
            SetDamageType(material);
        }
        
        private void SetDamageType(DamageType damageType)
        {
            DamageTypeContext = new EditContext(damageType);
            DamageType = damageType;
        }

        private async Task SaveChangesAsync() => await DbContext.DamageTypes.PutAsync(DamageType);

        private async Task OnValidSubmit()
        {
            if (DamageType.Id is null or "")
            {
                DamageType.Id = Guid.NewGuid().ToString();
            }

            await SaveChangesAsync();
            NavigationManager.NavigateTo("damage-types");
        }
    }
}