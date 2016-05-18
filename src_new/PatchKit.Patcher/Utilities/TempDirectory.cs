using System.IO;

namespace PatchKit.Patcher.Utilities
{
    class TempDirectory
    {
        public static string Create()
        {
            return Create("");
        }

        public static string Create(string prefix)
        {
            return Create(Path.GetTempPath(), prefix);
        }

        public static string Create(string baseDirectory, string prefix)
        {
            int iteration = 0;
            string tempDirectory;
            do
            {
                tempDirectory = Path.Combine(baseDirectory, prefix + Path.GetRandomFileName());
            } while (iteration++ < 1000 && Directory.Exists(tempDirectory));

            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
