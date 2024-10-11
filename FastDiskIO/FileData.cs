// CPOL license agreement notice:
// FileData was modified in October 2024
// from the original version by wilsone8 with the following changes:
// - documentation comments were modified, fixed or removed in some places
// - WIN32_FIND_DATA was changed to a struct and is being passed by ref

namespace FastDiskIO;

/// <summary>
/// Contains information about a file returned by
/// <see cref="FastDirectoryEnumeration"/>.
/// </summary>
[Serializable]
public class FileData
{
    /// <summary>
    /// Attributes of the file.
    /// </summary>
    public readonly FileAttributes Attributes;

    public DateTime CreationTime => CreationTimeUtc.ToLocalTime();

    public readonly DateTime CreationTimeUtc;

    public DateTime LastAccessTime => LastAccessTimeUtc.ToLocalTime();

    public readonly DateTime LastAccessTimeUtc;

    public DateTime LastWriteTime => LastWriteTimeUtc.ToLocalTime();

    public readonly DateTime LastWriteTimeUtc;

    /// <summary>
    /// Size of the file in bytes
    /// </summary>
    public readonly long Size;

    /// <summary>
    /// Name of the file
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Full path to the file
    /// </summary>
    public readonly string Path;

    public override string ToString()
    {
        return Name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileData"/> class.
    /// </summary>
    /// <param name="dir">The directory that the file is stored at</param>
    /// <param name="findData">WIN32_FIND_DATA structure that this
    /// object wraps.</param>
    internal FileData(string dir, ref WIN32_FIND_DATA findData)
    {
        Attributes = findData.dwFileAttributes;

        CreationTimeUtc = ConvertDateTime(
            findData.ftCreationTime_dwHighDateTime,
            findData.ftCreationTime_dwLowDateTime);

        LastAccessTimeUtc = ConvertDateTime(
            findData.ftLastAccessTime_dwHighDateTime,
            findData.ftLastAccessTime_dwLowDateTime);

        LastWriteTimeUtc = ConvertDateTime(
            findData.ftLastWriteTime_dwHighDateTime,
            findData.ftLastWriteTime_dwLowDateTime);

        Size = CombineHighLowInts(
            findData.nFileSizeHigh,
            findData.nFileSizeLow);

        Name = findData.GetFileName();
        Path = System.IO.Path.Combine(dir, Name);
    }

    private static long CombineHighLowInts(uint high, uint low)
    {
        return (long)high << 0x20 | low;
    }

    private static DateTime ConvertDateTime(uint high, uint low)
    {
        long fileTime = CombineHighLowInts(high, low);
        return DateTime.FromFileTimeUtc(fileTime);
    }
}
