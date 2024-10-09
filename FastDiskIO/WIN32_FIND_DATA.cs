using System.Runtime.InteropServices;
using System.Text;

namespace FastDiskIO;

/// <summary>
/// Contains information about the file that is found 
/// by the FindFirstFile or FindNextFile functions.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
[BestFitMapping(false)]
internal unsafe struct WIN32_FIND_DATA
{
    public const int MaxFileName = 260;
    public const int MaxAlternateFileName = 14;

    public FileAttributes dwFileAttributes;
    public uint ftCreationTime_dwLowDateTime;
    public uint ftCreationTime_dwHighDateTime;
    public uint ftLastAccessTime_dwLowDateTime;
    public uint ftLastAccessTime_dwHighDateTime;
    public uint ftLastWriteTime_dwLowDateTime;
    public uint ftLastWriteTime_dwHighDateTime;
    public uint nFileSizeHigh;
    public uint nFileSizeLow;
    public int dwReserved0;
    public int dwReserved1;

    public fixed char cFileName[MaxFileName];
    public fixed char cAlternateFileName[MaxAlternateFileName];

    public string GetFileName()
    {
        fixed (char* cFileName = this.cFileName)
        {
            return GetString(cFileName, MaxFileName);
        }
    }

    private static string GetString(Span<byte> buffer)
    {
        Span<char> chars = stackalloc char[buffer.Length];
        Encoding.Unicode.GetChars(buffer, chars);
        int len = chars.IndexOf('\0');
        if (len >= 0)
        {
            chars = chars[..len];
        }
        return new string(chars);
    }
    private static string GetString(char* buffer, int maxLength)
    {
        var span = new Span<char>(buffer, maxLength);
        return GetString(span);
    }
    private static string GetString(Span<char> chars)
    {
        int len = chars.IndexOf('\0');
        if (len >= 0)
        {
            chars = chars[..len];
        }
        return new string(chars);
    }

    public override string ToString()
    {
        return "File name: " + GetFileName();
    }
}
