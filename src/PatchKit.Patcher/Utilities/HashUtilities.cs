using System.Data.HashFunction;
using System.IO;
using System.Linq;
using System.Text;

namespace PatchKit.Patcher.Utilities
{
    internal static class HashUtilities
    {
        const ulong HashSeed = 42;

        public static string ComputeStringHash(string str)
        {
            return string.Concat(new xxHash(HashSeed).ComputeHash(Encoding.UTF8.GetBytes(str)).Select(b => b.ToString("X2")));
        }

        public static string ComputeFileHash(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return string.Concat(new xxHash(HashSeed).ComputeHash(fileStream).Select(b => b.ToString("X2")).Reverse()).ToLower().TrimStart('0');
            }
        }
    }
}
