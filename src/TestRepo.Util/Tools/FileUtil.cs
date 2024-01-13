namespace TestRepo.Util.Tools;

public readonly record struct PropertySelect(string Name, string Value);

public static class FileUtil
{
    private static FileStream OpenFileForWrite(string filePath, bool deleteOldFile = true)
    {
        var path = Path.GetFullPath(filePath); // in case pass relative path, or else it still return original path;
        var folder = Path.GetDirectoryName(path);
        if (Directory.Exists(folder))
            Directory.CreateDirectory(folder);
        if (deleteOldFile && File.Exists(path))
            File.Delete(path);
        return File.OpenWrite(path);
    }

    public static async Task WriteToFile<T>(
        string filePath,
        IReadOnlyList<T> data,
        Func<T, PropertySelect[]> propertyToWrite,
        bool deleteOldFile = true
    )
    {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Count == 0)
            throw new ArgumentNullException(nameof(data), "Collection must contain data");
        var props = propertyToWrite(data[0]);
        await using var file = OpenFileForWrite(filePath, deleteOldFile);
        await using var writer = new StreamWriter(file);
        await writer.WriteLineAsync(string.Join(',', props.Select(p => p.Name)));
        foreach (var d in data)
        {
            var prop = propertyToWrite(d);
            await writer.WriteLineAsync(string.Join(',', prop.Select(p => p.Value)));
        }
    }
}
