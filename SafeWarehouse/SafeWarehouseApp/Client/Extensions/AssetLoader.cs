using System.IO;
using System.Reflection;

namespace SafeWarehouseApp.Client.Extensions
{
    public static class AssetLoader
    {
        public static Stream ReadAssetStream(string name)
        {
            var fileName = $"SafeWarehouseApp.Client.Assets.{name}";
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(fileName)!;
        }
    }
}