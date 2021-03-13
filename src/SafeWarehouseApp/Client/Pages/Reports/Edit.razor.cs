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
        private Report Report { get; set; } = new();
        private EditContext ReportContext { get; set; } = default!;
        private bool HasRendered { get; set; }
        private string CurrentTab { get; set; } = "Designer";
        private IDictionary<string, DamageType> DamageTypes { get; set; } = new Dictionary<string, DamageType>();
        private IDictionary<string, Customer> Customers { get; set; } = new Dictionary<string, Customer>();
        private Customer? Customer => Report.CustomerId is not null && Customers.ContainsKey(Report.CustomerId) ? Customers[Report.CustomerId] : default;
        private File Schematic { get; set; } = new()!;
        private File? Photo { get; set; }
        private IJSObjectReference DesignerModule { get; set; } = default!;
        private static Func<string, int, int, int, int, Task> _updateDamageSpriteAsync = default!;

        [JSInvokable]
        public static async Task UpdateDamageSpriteAsyncCaller(string id, int left, int top, int width, int height) => await _updateDamageSpriteAsync.Invoke(id, left, top, width, height);

        protected override async Task OnInitializedAsync()
        {
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
                Customers = (await DbContext.Customers.GetAllAsync()).ToDictionary(x => x.Id);
                await InitializeDesignerAsync();
                HasRendered = true;
                StateHasChanged();
            }
        }

        private async Task LoadReportAsync(string id)
        {
            var report = await DbContext.Reports.GetAsync(id);
            await SetReportAsync(report);
        }

        private async Task SetReportAsync(Report report)
        {
            if (ReportContext != null!)
                ReportContext.OnFieldChanged -= OnReportFieldChanged;

            ReportContext = new EditContext(report);
            ReportContext.OnFieldChanged += OnReportFieldChanged;
            Report = report;
            Schematic = await DbContext.Files.GetAsync(Report.SchematicPhotoId);

            if (Report.PhotoId is not null and not "")
                Photo = await DbContext.Files.GetAsync(Report.PhotoId);
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

        private async Task SaveChangesAsync() => await DbContext.Reports.PutAsync(Report);

        private async Task BeginAddLocation(int left = 150, int top = 150)
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

            Report.Locations.Add(location);
            await SaveChangesAsync();
            await OnEditLocationClick(location);
        }

        private void ChangeTab(string tab)
        {
            CurrentTab = tab;
        }

        private async Task<string?> GetFileDataAsync(string? fileId)
        {
            var file = fileId is not null and not "" ? await DbContext.Files.GetAsync(fileId) : default;
            return file?.GetImageDataUrl();
        }

        private async Task OnCanvasDoubleClick(MouseEventArgs args)
        {
            await BeginAddLocation((int) (args.OffsetX - 50), (int) (args.OffsetY - 50));
        }

        private DateTime _lastTap = DateTime.MinValue;

        private async Task OnTouchEnd(TouchEventArgs args)
        {
            var now = DateTime.Now;
            var delta = now - _lastTap;

            _lastTap = DateTime.Now;

            if (delta < TimeSpan.FromMilliseconds(600) && delta > TimeSpan.Zero)
                await BeginAddLocation((int) args.ChangedTouches[0].ClientX, (int) args.ChangedTouches[0].ClientY);
        }

        private Task OnAddDamageClick() => BeginAddLocation();

        private async Task OnEditLocationClick(Location location)
        {
            var modalParameters = new ModalParameters();
            modalParameters.Add(nameof(LocationModal.Location), location);
            var reference = ModalService.Show<LocationModal>("Locatie", modalParameters);
            var result = await reference.Result;
            var action = (string) result.Data;

            if (action == "Delete")
            {
                Report.Locations.Remove(location);
                Report.UpdateLocationNumbers();
            }

            await SaveChangesAsync();
        }

        private async void OnReportFieldChanged(object? sender, FieldChangedEventArgs e) => await SaveChangesAsync();

        private async Task OnGeneralPhotoChanged(InputFileChangeEventArgs args)
        {
            var resizedImage = await args.File.RequestImageFileAsync(args.File.ContentType, 1024, 1024);

            var newPhoto = new File
            {
                Id = Guid.NewGuid().ToString("N"),
                Data = await resizedImage.ReadStreamAsync(),
                ContentType = args.File.ContentType,
                FileName = Path.GetFileName(args.File.Name)
            };

            await DbContext.Files.PutAsync(newPhoto);

            if (Report.PhotoId is not null and not "")
                await DbContext.Files.DeleteAsync(Report.PhotoId);

            Report.PhotoId = newPhoto.Id;
            Photo = newPhoto;
            await SaveChangesAsync();
        }

        private async Task OnSendClick()
        {
            var template = await AssetLoader.ReadAssetStream("report.handlebars").ReadStringAsync();
            var customer = Customers.GetEntry(Report.CustomerId) ?? new Customer();
            var locations = Report.Locations;
            var schematic = await DbContext.Files.GetAsync(Report.SchematicPhotoId);
            var materials = (await DbContext.Materials.GetAllAsync()).ToDictionary(x => x.Id);

            var requiredMaterials = Report.Locations
                .SelectMany(location => location.Damages.SelectMany(damage => damage.RequiredMaterials))
                .GroupBy(x => x.MaterialId)
                .Select(x => new RequiredMaterial
                {
                    MaterialId = x.Key,
                    Quantity = x.Select(y => y.Quantity).Sum()
                }).ToList();

            var model = new
            {
                CompanyName = customer.CompanyName,
                ReportDate = DateTime.Now.ToString("dd-MM-yyyy"),
                ContactName = customer.ContactName,
                City = customer.City,
                Address = customer.Address,
                NextExaminationBefore = Report.NextExaminationBefore?.ToString("dd-MM-yyyy"),
                Remarks = Report.Remarks,
                SchematicData = schematic.GetImageDataUrl(),
                Locations = await Task.WhenAll(locations.Select(async location => new
                {
                    Number = location.Number,
                    Description = location.Description,
                    Left = location.Left,
                    Top = location.Top,
                    Width = location.Width,
                    Height = location.Height,
                    Damages = await Task.WhenAll(location.Damages.Select(async damage => new
                    {
                        Number = damage.Number,
                        DamageType = DamageTypes.GetEntry(damage.DamageTypeId)?.Title ?? "(onbekend)",
                        RequiredMaterials = damage.RequiredMaterials.Select(x => materials.GetEntry(x.MaterialId)).Where(x => x != null).Select(x => x!.Name).ToList(),
                        DamagePictures = await Task.WhenAll(damage.Pictures.Select(async damagePicture => new
                        {
                            Number = damagePicture.Number,
                            Description = damagePicture.Description,
                            PictureData = await GetFileDataAsync(damagePicture.PictureId)
                        }).ToList())
                    }).ToList())
                }).ToList()),
                RequiredMaterials = requiredMaterials.Select(requiredMaterial => new
                {
                    Material = materials.GetEntry(requiredMaterial.MaterialId)?.Name ?? "(onbekend)",
                    Quantity = requiredMaterial.Quantity
                }).ToList()
            };

            //await Printer.PrintAsync();

            // var document = await PdfGenerator.GeneratePdfAsync(template, model);
            // var content = await document.Content.ToByteArrayAsync();
            // await FileDownloader.DownloadFromBytesAsync(new DownloadFromBytesOptions(content, "VeiligMagazijn_Rapport.pdf", "application/pdf"));
        }
    }
}