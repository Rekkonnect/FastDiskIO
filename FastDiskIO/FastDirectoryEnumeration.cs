// CPOL license agreement notice:
// FastDirectoryEnumeration was modified in October 2024
// from the original version by wilsone8 with the following changes:
// - use LibraryImport instead of DllImport
// - documentation comments were modified, fixed or removed in some places
// - the FileEnumerator was adjusted to avoid the extra DirectoryInfo.GetDirectories call
// - additional file counting-related methods were added
// - obsolete APIs involving code security were removed or adjusted accordingly

using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace FastDiskIO;

/// <summary>
/// Provides methods for quick file enumerations, including an allocation-friendlier
/// enumeration of file and directory counts matching the specified pattern.
/// </summary>
public static partial class FastDirectoryEnumeration
{
    /// <summary>
    /// Gets <see cref="FileData"/> for all the files in a directory that 
    /// match a specific filter, optionally including all sub directories.
    /// </summary>
    /// <param name="path">The path to search.</param>
    /// <param name="searchPattern">The search string to match against files in the path.</param>
    /// <param name="searchOption">
    /// One of the SearchOption values that specifies whether the search 
    /// operation should include all subdirectories or only the current directory.
    /// </param>
    /// <returns>An object that implements <see cref="IEnumerable{FileData}"/> and 
    /// allows you to enumerate the files in the given directory.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="path"/> is a null reference (Nothing in VB)
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="searchPattern"/> is a null reference (Nothing in VB)
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="searchOption"/> is not one of the valid values of the
    /// <see cref="SearchOption"/> enumeration.
    /// </exception>
    public static IEnumerable<FileData> EnumerateFiles(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        VerifyArguments(path, searchPattern, searchOption);

        string fullPath = Path.GetFullPath(path);

        return new FileEnumerable(fullPath, searchPattern, searchOption);
    }

    private static void VerifyArguments(string path, string searchPattern, SearchOption searchOption)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(searchPattern);

        switch (searchOption)
        {
            case SearchOption.TopDirectoryOnly:
            case SearchOption.AllDirectories:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(searchOption));
        }
    }

    /// <summary>
    /// Gets <see cref="FileData"/> for all the files in a directory that match a 
    /// specific filter, using the specified <see cref="SearchOption"/>.
    /// </summary>
    /// <param name="path">The path to search.</param>
    /// <param name="searchPattern">The search string to match against files in the path.</param>
    /// <param name="searchOption">
    /// The <see cref="SearchOption"/> to use for the enumeration of files.
    /// </param>
    /// <returns>An array of <see cref="FileData"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="searchPattern"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="searchOption"/> is not one of the defined values in
    /// <see cref="SearchOption"/>.
    /// </exception>
    public static FileData[] GetFiles(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return EnumerateFiles(path, searchPattern, searchOption)
            .ToArray();
    }

    /// <summary>
    /// Gets the total file and subdirectory counts of all the files and subdirectories
    /// in a directory that match a specific filter, using the specified
    /// <see cref="SearchOption"/>.
    /// </summary>
    /// <param name="path">The path to search.</param>
    /// <param name="searchPattern">The search string to match against files in the path.</param>
    /// <param name="searchOption">
    /// The <see cref="SearchOption"/> to use for the enumeration of files.
    /// </param>
    /// <returns>
    /// An <see cref="EnumeratedFileCount"/> representing the number of files
    /// and subdirectories that matched the specified filters.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="searchPattern"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="searchOption"/> is not one of the defined values in
    /// <see cref="SearchOption"/>.
    /// </exception>
    public static EnumeratedFileCount GetEnumeratedFileCount(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        VerifyArguments(path, searchPattern, searchOption);

        string fullPath = Path.GetFullPath(path);

        var enumerable = new FileEnumerable(fullPath, searchPattern, searchOption);
        var enumerator = enumerable.GetEnumerator();
        enumerator.EnumerateToEnd();
        return enumerator.EnumeratedFileCount;
    }

    /// <summary>
    /// Gets the total file count of all the files in a directory that match a 
    /// specific filter, using the specified <see cref="SearchOption"/>.
    /// </summary>
    /// <param name="path">The path to search.</param>
    /// <param name="searchPattern">The search string to match against files in the path.</param>
    /// <param name="searchOption">
    /// The <see cref="SearchOption"/> to use for the enumeration of files.
    /// </param>
    /// <returns>The number of files that matched the specified filters.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="searchPattern"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="searchOption"/> is not one of the defined values in
    /// <see cref="SearchOption"/>.
    /// </exception>
    public static int GetFileCount(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return GetEnumeratedFileCount(path, searchPattern, searchOption)
            .Files;
    }

    /// <summary>
    /// Gets the total file count of all the files in a directory that match a 
    /// specific filter, using the specified <see cref="SearchOption"/>.
    /// </summary>
    /// <param name="path">The path to search.</param>
    /// <param name="searchPattern">The search string to match against files in the path.</param>
    /// <param name="searchOption">
    /// The <see cref="SearchOption"/> to use for the enumeration of files.
    /// </param>
    /// <returns>The number of files that matched the specified filters.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="searchPattern"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="searchOption"/> is not one of the defined values in
    /// <see cref="SearchOption"/>.
    /// </exception>
    public static int GetDirectoryCount(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return GetEnumeratedFileCount(path, searchPattern, searchOption)
            .Directories;
    }

    private class FileEnumerable : IEnumerable<FileData>
    {
        private readonly string _path;
        private readonly string _filter;
        private readonly SearchOption _searchOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEnumerable"/> class.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">The search string to match against files in the path.</param>
        /// <param name="searchOption">
        /// One of the SearchOption values that specifies whether the search 
        /// operation should include all subdirectories or only the current directory.
        /// </param>
        public FileEnumerable(string path, string searchPattern, SearchOption searchOption)
        {
            _path = path;
            _filter = searchPattern;
            _searchOption = searchOption;
        }

        public FileEnumerator GetEnumerator()
        {
            return new FileEnumerator(_path, _filter, _searchOption);
        }

        IEnumerator<FileData> IEnumerable<FileData>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [SuppressUnmanagedCodeSecurity]
    private partial class FileEnumerator : IEnumerator<FileData>
    {
        [LibraryImport(
            "kernel32.dll",
            EntryPoint = "FindFirstFileW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16)]
        private static partial SafeFindHandle FindFirstFile(
            string fileName,
            ref WIN32_FIND_DATA data);

        [LibraryImport(
            "kernel32.dll",
            EntryPoint = "FindNextFileW",
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool FindNextFile(
            SafeFindHandle hndFindFile,
            ref WIN32_FIND_DATA lpFindFileData);

        /// <summary>
        /// Hold context information about where we current are in the directory search.
        /// </summary>
        private class SearchContext(string path)
        {
            public readonly string Path = path;
            public Stack<string>? SubdirectoriesToProcess;
            public bool FailedFindNext;
        }

        public int FileCount { get; private set; }
        public int DirectoryCount { get; private set; }

        public EnumeratedFileCount EnumeratedFileCount => new(FileCount, DirectoryCount);

        private string _path;
        private readonly string _searchPattern;
        private readonly SearchOption _searchOption;
        private readonly Stack<SearchContext>? _contextStack;
        private SearchContext _currentContext;

        private SafeFindHandle? _hndFindFile;
        private WIN32_FIND_DATA _winFindData;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEnumerator"/> class.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">The search string to match against files in the path.</param>
        /// <param name="searchOption">
        /// One of the SearchOption values that specifies whether the search 
        /// operation should include all subdirectories or only the current directory.
        /// </param>
        public unsafe FileEnumerator(string path, string searchPattern, SearchOption searchOption)
        {
            _path = path;
            _searchPattern = searchPattern;
            _searchOption = searchOption;
            _currentContext = new SearchContext(path);

            if (_searchOption == SearchOption.AllDirectories)
            {
                _contextStack = new Stack<SearchContext>();
            }

            _winFindData = new();
        }

        public void EnumerateToEnd()
        {
            Reset();
            while (MoveNext())
            {
            }
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public FileData Current => new(_path, ref _winFindData);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (_hndFindFile != null)
            {
                _hndFindFile.Dispose();
            }
        }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            bool success = false;

            // If the handle is null, this is first call to MoveNext in the current 
            // directory. In that case, start a new search.
            if (!_currentContext.FailedFindNext)
            {
                if (_hndFindFile is null)
                {
                    // Not honored by the runtime, I guess we just keep that in place just in case this works
#pragma warning disable SYSLIB0003 // Type or member is obsolete
                    new FileIOPermission(FileIOPermissionAccess.PathDiscovery, _path).Demand();
#pragma warning restore SYSLIB0003 // Type or member is obsolete

                    string searchPath = Path.Combine(_path, _searchPattern);
                    _hndFindFile = FindFirstFile(searchPath, ref _winFindData);
                    success = !_hndFindFile.IsInvalid;
                }
                else
                {
                    // Otherwise, find the next item.
                    success = FindNextFile(_hndFindFile, ref _winFindData);
                }
            }

            // If we have any file or folder with this search pattern, success is true
            if (success)
            {
                if (_winFindData.dwFileAttributes.HasFlag(FileAttributes.Directory))
                {
                    if (_searchOption is SearchOption.AllDirectories)
                    {
                        var directoryFileName = _winFindData.GetFileName();
                        if (directoryFileName is not "." and not "..")
                        {
                            DirectoryCount++;
                            _currentContext.SubdirectoriesToProcess ??= new();
                            var fullDirectoryName = Path.Combine(_path, directoryFileName);
                            _currentContext.SubdirectoriesToProcess.Push(fullDirectoryName);
                        }
                    }
                    return MoveNext();
                }

                FileCount++;
                return true;
            }

            _currentContext.FailedFindNext = true;

            if (_searchOption is SearchOption.AllDirectories)
            {
                Debug.Assert(_contextStack is not null, "We should have the context stack here");

                if (_currentContext.SubdirectoriesToProcess is { Count: > 0 })
                {
                    string subDir = _currentContext.SubdirectoriesToProcess.Pop();

                    _contextStack.Push(_currentContext);
                    _path = subDir;
                    _hndFindFile = null;
                    _currentContext = new SearchContext(_path);
                    return MoveNext();
                }

                // If there are no more files in this directory and we are 
                // in a sub directory, pop back up to the parent directory and
                // continue the search from there.
                if (_contextStack.Count > 0)
                {
                    _currentContext = _contextStack.Pop();
                    _path = _currentContext.Path;
                    if (_hndFindFile is not null)
                    {
                        _hndFindFile.Close();
                        _hndFindFile = null;
                    }

                    return MoveNext();
                }
            }

            return success;
        }

        public void Reset()
        {
            _hndFindFile = null;
            FileCount = 0;
            DirectoryCount = 0;
        }
    }
}
