using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace FastDiskIO;

/// <summary>
/// Wraps a FindFirstFile handle.
/// </summary>
public sealed partial class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool FindClose(IntPtr handle);

    public SafeFindHandle()
        : base(true)
    {
    }

    protected override bool ReleaseHandle()
    {
        return FindClose(handle);
    }
}
