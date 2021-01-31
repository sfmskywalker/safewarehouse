using System;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Pages.Reports
{
    partial class DamageModal
    {
        [CascadingParameter] public BlazoredModalInstance Modal { get; set; } = default!;
        [Parameter] public Damage Damage { get; set; } = new();
        private EditContext EditContext { get; set; } = default!;

        protected override void OnInitialized()
        {
            EditContext = new EditContext(Damage);
        }

        private async Task Close() => await Modal.CloseAsync(ModalResult.Ok(true));
        private async Task Cancel() => await Modal.CancelAsync();

        private async Task OnValidSubmit()
        {
            await Modal.CloseAsync(ModalResult.Ok(Damage));
        }
    }
}