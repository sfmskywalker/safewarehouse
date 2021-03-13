using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HandlebarsDotNet;
using IronPdf;
using iText.Html2pdf;

namespace SafeWarehouseApp.Client.Services
{
    public class PdfGenerator
    {
        public async Task<PdfDocument> GeneratePdfAsync(string template, object model, CancellationToken cancellationToken = default)
        {
            // var rs = new LocalReporting().UseBinary(JsReportBinary.GetBinary()).AsUtility().Create();
            //
            // var request = new RenderRequest
            // {
            //     Template = new Template
            //     {
            //         Recipe = Recipe.ChromePdf,
            //         Engine = Engine.Handlebars,
            //         Content = template,
            //         
            //     },
            //     Data = model
            // };
            //
            // var report = await rs.RenderAsync(request, cancellationToken);
            // return new PdfDocument(report.Content, report.Meta);
            
            var inputTemplate = template;
            var compiledTemplate = Handlebars.Compile(inputTemplate);
            var html = compiledTemplate(model);
            
            var outputStream = new MemoryStream();
            HtmlConverter.ConvertToPdf(html, outputStream, new ConverterProperties().SetBaseUri("https://localhost:5001/"));
            
            outputStream = new MemoryStream(outputStream.ToArray());
            
            return new PdfDocument(outputStream);
        }
    }

    public record PdfDocument(Stream Content);
}