using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SafeWarehouseApp.Client.Extensions;
using SafeWarehouseApp.Client.Services;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Client.Pages.Reports
{
    partial class Print
    {
        [Parameter] public string Id { get; set; } = default!;
        [Inject] private SafeWarehouseContext DbContext { get; set; } = default!;
        [Inject] private Printer Printer { get; set; } = default!;
        private Report Report { get; set; } = new();
        private IDictionary<string, DamageType> DamageTypes { get; set; } = new Dictionary<string, DamageType>();
        private IDictionary<string, Customer> Customers { get; set; } = new Dictionary<string, Customer>();
        private Customer Customer => Report.CustomerId is not null && Customers.ContainsKey(Report.CustomerId) ? Customers[Report.CustomerId] : new Customer();
        private File Schematic { get; set; } = new()!;
        private File? Photo { get; set; }
        private DateTime ReportDate => DateTime.Now;
        private PrintReportModel Model { get; set; } = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadReportAsync(Id);
                DamageTypes = (await DbContext.DamageTypes.GetAllAsync()).ToDictionary(x => x.Id);
                Customers = (await DbContext.Customers.GetAllAsync()).ToDictionary(x => x.Id);
                Model = await CreateModelAsync();
                StateHasChanged();
                await Printer.PrintAsync();
            }
        }

        private async Task<PrintReportModel> CreateModelAsync()
        {
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

            var model = new PrintReportModel
            {
                Report = Report,
                Customer = customer,
                ReportDate = DateTime.Now,
                Schematic = schematic,
                Locations = await Task.WhenAll(locations.Select(async location => new PrintReportLocationModel
                {
                    Location = location,
                    Damages = await Task.WhenAll(location.Damages.Select(async damage => new PrintReportDamageModel
                    {
                        Damage = damage,
                        DamageType = DamageTypes.GetEntry(damage.DamageTypeId),
                        RequiredMaterials = damage.RequiredMaterials.Select(x => materials.GetEntry(x.MaterialId)).Where(x => x != null).Select(x => x!).ToList(),
                        Pictures = await Task.WhenAll(damage.Pictures.Select(async damagePicture => new PrintReportDamagePictureModel
                        {
                            DamagePicture = damagePicture,
                            Picture = await GetFileAsync(damagePicture.PictureId)
                        }).ToList())
                    }).ToList())
                }).ToList()),
                RequiredMaterials = requiredMaterials.Select(requiredMaterial => new PrintReportRequiredMaterialModel
                {
                    Material = materials.GetEntry(requiredMaterial.MaterialId),
                    Quantity = requiredMaterial.Quantity
                }).ToList()
            };

            return model;
        }

        private async Task LoadReportAsync(string id)
        {
            var report = await DbContext.Reports.GetAsync(id);
            await SetReportAsync(report);
        }
        
        private async Task SetReportAsync(Report report)
        {
            Report = report;
            Schematic = await DbContext.Files.GetAsync(Report.SchematicPhotoId);

            if (Report.PhotoId is not null and not "")
                Photo = await DbContext.Files.GetAsync(Report.PhotoId);
        }
        
        private async Task<string?> GetFileDataAsync(string? fileId)
        {
            var file = await GetFileAsync(fileId);
            return file?.GetImageDataUrl();
        }
        
        private async Task<File?> GetFileAsync(string? fileId) => fileId is not null and not "" ? await DbContext.Files.GetAsync(fileId) : default;
    }

    public record PrintReportModel
    {
        public Report Report { get; set; } = new();
        public Customer Customer { get; set; } = new();
        public DateTime ReportDate { get; set; }
        public File Schematic { get; set; } = new();
        public IList<PrintReportLocationModel> Locations { get; set; } = new List<PrintReportLocationModel>();
        public IList<PrintReportRequiredMaterialModel> RequiredMaterials { get; set; } = new List<PrintReportRequiredMaterialModel>();

    }

    public class PrintReportRequiredMaterialModel
    {
        public Material? Material { get; set; }
        public int Quantity { get; set; }
    }

    public record PrintReportLocationModel
    {
        public Location Location { get; set; } = new();
        public IList<PrintReportDamageModel> Damages { get; set; } = new List<PrintReportDamageModel>();
    }

    public record PrintReportDamageModel
    {
        public Damage Damage { get; set; } = new();
        public DamageType? DamageType { get; set; }
        public IList<Material> RequiredMaterials { get; set; } = new List<Material>();
        public IList<PrintReportDamagePictureModel> Pictures { get; set; } = new List<PrintReportDamagePictureModel>();
    }

    public record PrintReportDamagePictureModel
    {
        public DamagePicture DamagePicture { get; set; } = new();
        public File? Picture { get; set; }
    }
}