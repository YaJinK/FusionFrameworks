using System.IO.Compression;
using UnityEditor;

namespace Fusion.Frameworks.Utilities
{
    public class ZipUtility
    {
        public static void Compress(string folder, string output)
        {
            ZipFile.CreateFromDirectory(folder, output, CompressionLevel.Optimal, true);
        }

        public static void Extract(string source, string target)
        {
            ZipFile.ExtractToDirectory(source, target, true);
        }
    }
}
