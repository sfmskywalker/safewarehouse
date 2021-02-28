using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SafeWarehouseApp.Client.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<byte[]> ToByteArrayAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            await using var ms = new MemoryStream();
            await stream.CopyToAsync(ms, cancellationToken);
            return ms.ToArray();
        }
        
        public static async Task<string> ReadStringAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            using var reader = new StreamReader(stream);
            var text = await reader.ReadToEndAsync();
            return text;
        }
    }
}