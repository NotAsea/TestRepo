using Cysharp.Text;

namespace TestRepo.Util.Tools;

public readonly record struct PropertySelect(string Name, string Value);

public static class FileUtil
{
    private static FileStream OpenFileForWrite(string filePath, bool deleteOldFile = true)
    {
        var path = Path.GetFullPath(filePath); // in case pass relative path, or else it still returns original path;
        var folder =
            Path.GetDirectoryName(path) ?? throw new DirectoryNotFoundException(nameof(path));
        if (!Directory.Exists(folder))
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
        await writer
            .WriteLineAsync(
                ZString.Join(',', props.Gen().Select(new SelectPropertyName()).ToList())
            )
            .ConfigureAwait(false);
        foreach (var d in data)
        {
            var prop = propertyToWrite(d);
            await writer
                .WriteLineAsync(
                    ZString.Join(',', prop.Gen().Select(new SelectPropertyValue()).ToList())
                )
                .ConfigureAwait(false);
        }
    }
}

file readonly struct SelectPropertyName : IStructFunction<PropertySelect, string>
{
    public string Invoke(PropertySelect arg) => arg.Name;
}

file readonly struct SelectPropertyValue : IStructFunction<PropertySelect, string>
{
    public string Invoke(PropertySelect arg) => arg.Value;
}
