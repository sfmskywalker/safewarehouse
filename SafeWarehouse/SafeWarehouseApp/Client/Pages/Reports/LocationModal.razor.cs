using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Client.Shared;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Pages.Reports
{
    partial class LocationModal
    {
        [CascadingParameter] public BlazoredModalInstance Modal { get; set; } = default!;
        [Parameter] public Location Location { get; set; } = new();
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        [Inject] private IModalService ModalService { get; set; } = default!;
        private EditContext LocationContext { get; set; } = default!;
        private IDictionary<string, DamageType> DamageTypeLookup { get; set; } = new Dictionary<string, DamageType>();

        protected override void OnInitialized()
        {
            LocationContext = new EditContext(Location);
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                DamageTypeLookup = (await DbContext.DamageTypes.GetAllAsync()).ToDictionary(x => x.Id);
                StateHasChanged();
            }
        }

        private async Task OnValidSubmit() => await Modal.CloseAsync();

        private async Task OnAddDamageClick()
        {
            var damage = new Damage
            {
                Id = Guid.NewGuid().ToString("N"),
                DamageTypeId = DamageTypeLookup.FirstOrDefault().Key,
                Number = Location.Damages.Count + 1
            };
            
            Location.Damages.Add(damage);
            await OnEditDamageClick(damage);
        }
        
        private async Task OnEditDamageClick(Damage damage)
        {
            var modalParameters = new ModalParameters();
            modalParameters.Add(nameof(DamageModal.Damage), damage);
            var reference = ModalService.Show<DamageModal>("Schade", modalParameters);
            await reference.Result;
        }
        
        private void OnDeleteDamageClick(Damage damage)
        {
            Location.Damages.Remove(damage);
            Location.UpdateDamageNumbers();
            StateHasChanged();
        }
        
        private async Task OnDeleteLocationClick()
        {
            var modalParameters = new ModalParameters();
            modalParameters.Add(nameof(ConfirmationModal.Text), "Weet je zeker dat je deze locatie wilt verwijderen?");
            var reference = ModalService.Show<ConfirmationModal>("Locatie verwijderen", modalParameters);
            var result = await reference.Result;

            if (result.Cancelled)
                return;

            await Modal.CloseAsync(ModalResult.Ok("Delete"));
        }
    }
}