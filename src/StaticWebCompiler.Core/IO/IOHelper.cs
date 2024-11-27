namespace StaticWebCompiler.IO;

internal static class IOHelper
{
    internal static void CopyDirectory(string sourceDir, string destinationDir)
    {
        // Ensure the source directory exists
        if (!Directory.Exists(sourceDir))
        {
            throw new DirectoryNotFoundException($"Source directory does not exist: {sourceDir}");
        }

        // Create the destination directory if it doesn't exist
        Directory.CreateDirectory(destinationDir);

        // Copy all files in the current directory
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(file);
            string destinationFile = Path.Combine(destinationDir, fileName);
            File.Copy(file, destinationFile, overwrite: true);
        }

        // Copy all subdirectories
        foreach (var subdirectory in Directory.GetDirectories(sourceDir))
        {
            string directoryName = Path.GetFileName(subdirectory);
            string destinationSubdirectory = Path.Combine(destinationDir, directoryName);
            CopyDirectory(subdirectory, destinationSubdirectory);
        }
    }
}
