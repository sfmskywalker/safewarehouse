using System.Text;

namespace SafeWarehouseApp.Shared.Extensions
{
    public static class Helpers
    {
        public static string IntToLetters(int value)
        {
            var result = new StringBuilder(value / 26 + 1);
            
            while (--value >= 0)
            {
                result.Insert(0, (char) ('A' + value % 26));
                value /= 26;
            }
            
            return result.ToString();
        }
    }
}