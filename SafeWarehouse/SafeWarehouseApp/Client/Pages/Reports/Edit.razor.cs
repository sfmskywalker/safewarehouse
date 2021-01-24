using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            Report.Damages.Add(new Damage
            {
                Id = Guid.NewGuid().ToString("N"),
                Number = Report.Damages.Count + 1,
                Left = 100,
                Top = 100,
                Width = 50,
                Height = 50,
                Title = "Nieuwe Schade"
            });

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