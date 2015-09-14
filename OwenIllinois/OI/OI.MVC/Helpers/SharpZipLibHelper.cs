using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace OI.MVC.Helpers
{
    internal static class SharpZipLibHelper
    {
        public static void ZipFolder(string folderPath, ZipOutputStream zipStream)
        {
            var path = !folderPath.EndsWith("\\") ? string.Concat(folderPath, "\\") : folderPath;
            ZipFolder(path, path, zipStream);
        }

        private static void ZipFolder(string rootFolder, string currentFolder,
            ZipOutputStream zipStream)
        {
            var subFolders = Directory.GetDirectories(currentFolder);
            foreach (var folder in subFolders)
            {
                ZipFolder(rootFolder, folder, zipStream);
            }
            var relativePath = string.Concat(currentFolder.Substring(rootFolder.Length), "/");
            if (relativePath.Length > 1)
            {
                var dirEntry = new ZipEntry(relativePath);
                dirEntry.DateTime = DateTime.Now;
            }
            foreach (var file in Directory.GetFiles(currentFolder))
            {
                AddFileToZip(zipStream, relativePath, file);
            }
        }

        private static void AddFileToZip(ZipOutputStream zStream, string relativePath, string file)
        {
            byte[] buffer = new byte[4096];
            var fileRelativePath = string.Concat((relativePath.Length > 1 ? relativePath : string.Empty), Path.GetFileName(file));

            var entry = new ZipEntry(fileRelativePath) {DateTime = DateTime.Now};
            zStream.PutNextEntry(entry);

            using (FileStream fs = File.OpenRead(file))
            {
                int sourceBytes;
                do
                {
                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
                    zStream.Write(buffer, 0, sourceBytes);
                } while (sourceBytes > 0);
            }
        }
    }
}