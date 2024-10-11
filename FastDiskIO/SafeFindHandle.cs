// CPOL license agreement notice:
// SafeFindHandle was modified in October 2024
// from the original version by wilsone8 with the following changes:
// - use LibraryImport instead of DllImport

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
