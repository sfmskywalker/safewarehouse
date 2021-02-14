using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SafeWarehouseApp.Client.Extensions;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Shared.Models;
using File = SafeWarehouseApp.Shared.Models.File;

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
        private EditContext ReportContext { get; set; } = default!;
        private bool HasRendered { get; set; }
        private string CurrentTab { get; set; } = "Designer";
        private IDictionary<string, DamageType> DamageTypes { get; set; } = new Dictionary<string, DamageType>();
        private IJSObjectReference DesignerModule { get; set; } = default!;
        private static Func<string, int, int, int, int, Task> _updateDamageSpriteAsync = default!;

        [JSInvokable]
        public static async Task UpdateDamageSpriteAsyncCaller(string id, int left, int top, int width, int height) => await _updateDamageSpriteAsync.Invoke(id, left, top, width, height);

        protected override void OnInitialized()
        {
            SetReport(Report);
            _updateDamageSpriteAsync = UpdateDamageSpriteAsync;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (HasRendered)
                await LoadReportAsync(Id);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadReportAsync(Id);
                DamageTypes = (await DbContext.DamageTypes.GetAllAsync()).ToDictionary(x => x.Id);
                await InitializeDesignerAsync();
                HasRendered = true;
                StateHasChanged();
            }
        }

        private async Task LoadReportAsync(string id)
        {
            var report = await DbContext.Reports.GetAsync(id);
            SetReport(report);
        }

        private void SetReport(Report report)
        {
            if (ReportContext != null!)
                ReportContext.OnFieldChanged -= OnReportFieldChanged;

            ReportContext = new EditContext(report);
            ReportContext.OnFieldChanged += OnReportFieldChanged;
            Report = report;
        }

        private async Task InitializeDesignerAsync()
        {
            DesignerModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/reportDamageDesigner.js");
            await DesignerModule.InvokeVoidAsync("initialize");
        }

        private async Task UpdateDamageSpriteAsync(string id, int left, int top, int width, int height)
        {
            var damage = Report.Locations.First(x => x.Id == id);
            damage.Left = left;
            damage.Top = top;
            damage.Width = width;
            damage.Height = height;
            await SaveChangesAsync();
        }

        private string GetDamageTypeText(string? damageTypeId) => damageTypeId != null && DamageTypes.ContainsKey(damageTypeId) ? DamageTypes[damageTypeId].Title : "";

        private async Task SaveChangesAsync() => await DbContext.Reports.PutAsync(Report);

        private async Task BeginAddDamage(int left = 150, int top = 150)
        {
            var location = new Location
            {
                Id = Guid.NewGuid().ToString("N"),
                Number = Report.Locations.Count + 1,
                Left = left,
                Top = top,
                Width = 100,
                Height = 100
            };

            var modalParameters = new ModalParameters();
            modalParameters.Add(nameof(LocationModal.Location), location);
            var reference = ModalService.Show<LocationModal>("Nieuwe locatie", modalParameters);
            var result = await reference.Result;

            if (result.Cancelled)
                return;

            location = (Location) result.Data;
            Report.Locations.Add(location);
            await SaveChangesAsync();
        }

        private void ChangeTab(string tab)
        {
            CurrentTab = tab;
        }

        private async Task OnCanvasDoubleClick(MouseEventArgs args)
        {
            await BeginAddDamage((int) (args.OffsetX - 50), (int) (args.OffsetY - 50));
        }

        private DateTime _lastTap = DateTime.MinValue;

        private async Task OnTouchEnd(TouchEventArgs args)
        {
            var now = DateTime.Now;
            var delta = now - _lastTap;

            _lastTap = DateTime.Now;

            if (delta < TimeSpan.FromMilliseconds(600) && delta > TimeSpan.Zero)
                await BeginAddDamage((int) args.ChangedTouches[0].ClientX, (int) args.ChangedTouches[0].ClientY);
        }

        private Task OnAddDamageClick() => BeginAddDamage();

        private async Task OnEditLocationClick(Location location)
        {
            var clone = Cloner.Clone(location);
            var modalParameters = new ModalParameters();
            modalParameters.Add(nameof(LocationModal.Location), clone);
            var reference = ModalService.Show<LocationModal>("Bewerk locatie", modalParameters);
            var result = await reference.Result;

            if (result.Cancelled)
                return;

            clone = (Location?) result.Data;

            if (clone == null)
                Report.Locations.Remove(location);
            else
                Cloner.Update(clone, location);
            
            await SaveChangesAsync();
        }

        private async Task OnDeleteLocationClick(Location location)
        {
            Report.Locations.Remove(location);

            var index = 0;

            foreach (var d in Report.Locations)
                d.Number = ++index;

            await SaveChangesAsync();
        }

        private async void OnReportFieldChanged(object? sender, FieldChangedEventArgs e) => await SaveChangesAsync();

        private async Task OnGeneralPhotoChanged(InputFileChangeEventArgs args)
        {
            var resizedImage = await args.File.RequestImageFileAsync(args.File.ContentType, 1024, 1024);

            Report.Photo = new File
            {
                Data = await resizedImage.ReadStreamAsync(),
                ContentType = args.File.ContentType,
                FileName = Path.GetFileName(args.File.Name)
            };
        }
    }
}