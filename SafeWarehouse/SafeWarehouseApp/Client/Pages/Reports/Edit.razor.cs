using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Pages.Reports
{
    partial class Edit
    {
        [Parameter] public string Id { private get; set; } = default!;
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private IModalService ModalService { get; set; } = default!;
        [Inject] private Cloner Cloner { get; set; } = default!;
        private Report Report { get; set; } = new();
        private bool HasRendered { get; set; }
        private IDictionary<string, DamageType> DamageTypes { get; set; } = new Dictionary<string, DamageType>();
        private IJSObjectReference DesignerModule { get; set; } = default!;
        private static Func<string, int, int, int, int, Task> _updateDamageSpriteAsync = default!;

        [JSInvokable]
        public static async Task UpdateDamageSpriteAsyncCaller(string id, int left, int top, int width, int height) => await _updateDamageSpriteAsync.Invoke(id, left, top, width, height);

        protected override void OnInitialized()
        {
            _updateDamageSpriteAsync = UpdateDamageSpriteAsync;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (HasRendered)
                Report = await DbContext.Reports.GetAsync(Id);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Report = await DbContext.Reports.GetAsync(Id);
                DamageTypes = (await DbContext.DamageTypes.GetAllAsync()).ToDictionary(x => x.Id);
                await InitializeDesignerAsync();
                HasRendered = true;
                StateHasChanged();
            }
        }

        private async Task InitializeDesignerAsync()
        {
            DesignerModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/reportDamageDesigner.js");
            await DesignerModule.InvokeVoidAsync("initialize");
        }

        private async Task UpdateDamageSpriteAsync(string id, int left, int top, int width, int height)
        {
            var damage = Report.Damages.First(x => x.Id == id);
            damage.Left = left;
            damage.Top = top;
            damage.Width = width;
            damage.Height = height;
            await SaveChangesAsync();
        }

        private string GetDamageTypeText(string? damageTypeId) => damageTypeId != null && DamageTypes.ContainsKey(damageTypeId) ? DamageTypes[damageTypeId].Title : "";

        private async Task SaveChangesAsync() => await DbContext.Reports.PutAsync(Report);

        private async Task OnAddDamageClick()
        {
            var damage = new Damage
            {
                Id = Guid.NewGuid().ToString("N"),
                Number = Report.Damages.Count + 1,
                Left = 150,
                Top = 150,
                Width = 100,
                Height = 100
            };

            var modalParameters = new ModalParameters();
            modalParameters.Add(nameof(DamageModal.Damage), damage);
            var reference = ModalService.Show<DamageModal>("Nieuwe schade", modalParameters);
            var result = await reference.Result;

            if (result.Cancelled)
                return;

            damage = (Damage) result.Data;
            Report.Damages.Add(damage);
            await SaveChangesAsync();
        }

        private async Task OnEditDamageClick(Damage damage)
        {
            var clone = Cloner.Clone(damage);
            var modalParameters = new ModalParameters();
            modalParameters.Add(nameof(DamageModal.Damage), clone);
            var reference = ModalService.Show<DamageModal>("Bewerk schade", modalParameters);
            var result = await reference.Result;

            if (result.Cancelled)
                return;

            clone = (Damage) result.Data;
            Cloner.Update(damage, clone);
            await SaveChangesAsync();
        }

        private async Task OnDeleteDamageClick(Damage damage)
        {
            Report.Damages.Remove(damage);

            var index = 0;

            foreach (var d in Report.Damages)
                d.Number = ++index;

            await SaveChangesAsync();
        }
    }
}