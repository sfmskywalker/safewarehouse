using System.IO;
using System.Threading;
using System.Threading.Tasks;
using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;

namespace SafeWarehouseApp.Client.Services
{
    public class PdfGenerator
    {
        public async Task<PdfDocument> GeneratePdfAsync(string template, object model, CancellationToken cancellationToken = default)
        {
            var rs = new LocalReporting().UseBinary(JsReportBinary.GetBinary()).AsUtility().Create();

            var request = new RenderRequest
            {
                Template = new Template
                {
                    Recipe = Recipe.ChromePdf,
                    Engine = Engine.Handlebars,
                    Content = template,
                    
                },
                Data = model
            };

            var report = await rs.RenderAsync(request, cancellationToken);
            return new PdfDocument(report.Content, report.Meta);
        }
    }

    public record PdfDocument(Stream Content, ReportMeta Meta);
}