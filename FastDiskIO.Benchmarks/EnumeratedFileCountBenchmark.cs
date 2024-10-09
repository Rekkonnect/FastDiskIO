#define FAST_BENCHMARK

using BenchmarkDotNet.Attributes;

namespace FastDiskIO.Benchmarks;

[IterationTime(150)]
[MemoryDiagnoser]
public class EnumeratedFileCountBenchmark
{
    private const string DirectoryPath = @"E:\test\dummy-files\";

#if FAST_BENCHMARK
    [Params(1000)]
    public int FileCount { get; set; }
    [Params(300)]
    public int DirectoryCount { get; set; }
    [Params(1)]
    public int DirectoryDepth { get; set; }
#else
    [Params(30, 100, 1000)]
    public int FileCount { get; set; }
    [Params(5, 50, 300)]
    public int DirectoryCount { get; set; }
    [Params(0, 1, 2)]
    public int DirectoryDepth { get; set; }
#endif

    public int ExpectedTotalFiles
    {
        get
        {
            return FileCount * ExpectedTotalDirectories;
        }
    }

    public int ExpectedTotalDirectories
    {
        get
        {
            int sum = 0;
            for (int i = 0; i <= DirectoryDepth; i++)
            {
                sum += Power(DirectoryCount, i);
            }
            return sum;
        }
    }

    private static int Power(int @base, int exponent)
    {
        switch (exponent)
        {
            case 0:
                return 1;
            case 1:
                return @base;
            case 2:
                return @base * @base;
        }

        int result = 1;
        for (int i = 0; i < exponent; i++)
        {
            result *= @base;
        }
        return result;
    }

#if !FAST_BENCHMARK
    [GlobalSetup]
    public void Setup()
    {
        var expected = ExpectedTotalFiles;
        const int maxFiles = 350_000;
        if (expected > maxFiles)
        {
            throw new Exception(
                $"""
                The expected file count {expected} is greater than the maximum allowed {maxFiles}
                This error is expected, and is placed to avoid cluttering the disk.
                """);
        }

        Console.WriteLine("Clearing the target dummy file directory");

        // Delete the entire directory to start anew
        Directory.Delete(DirectoryPath, true);

        Console.WriteLine("Creating the files to be enumerated, this may take a while...");

        var createdDirectories = new List<DirectoryInfo>();

        CreateDirectories();

        void CreateDirectories()
        {
            CreateDirectoriesDepth("", 0);
        }

        void CreateDirectoriesDepth(string baseDirectory, int depth)
        {
            CreateDirectoryRelatedPath(baseDirectory);

            if (depth >= DirectoryDepth)
                return;

            for (int i = 0; i < DirectoryCount; i++)
            {
                var nextPath = Path.Combine(baseDirectory, i.ToString());
                CreateDirectoriesDepth(nextPath, depth + 1);
            }
        }

        void CreateDirectoryRelatedPath(string path)
        {
            CreateDirectory(Path.Combine(DirectoryPath, path));
        }

        void CreateDirectory(string path)
        {
            var directory = new DirectoryInfo(path);
            directory.Create();
            createdDirectories.Add(directory);
            CreateFiles(directory);
        }

        void CreateFiles(DirectoryInfo directory)
        {
            int fileCount = FileCount;
            for (int i = 0; i < fileCount; i++)
            {
                var path = Path.Combine(directory.FullName, $"{i}.txt");
                var file = CreateFile(path);
            }
        }

        static FileInfo CreateFile(string path)
        {
            var file = new FileInfo(path);
            file.Create().Close();
            return file;
        }
    }
#endif

    [Benchmark]
    public int GetFileCount()
    {
        var count = FastDirectoryEnumeration.GetFileCount(
            DirectoryPath, "*", SearchOption.AllDirectories);
        return EnsureCorrectFileCount(count);
    }

    [Benchmark]
    public int GetFiles()
    {
        var count = FastDirectoryEnumeration.GetFiles(
            DirectoryPath, "*", SearchOption.AllDirectories)
            .Length;
        return EnsureCorrectFileCount(count);
    }

    [Benchmark]
    public int EnumerateFiles()
    {
        var count = FastDirectoryEnumeration.EnumerateFiles(
            DirectoryPath, "*", SearchOption.AllDirectories)
            .Count();
        return EnsureCorrectFileCount(count);
    }

    [Benchmark]
    public int Directory_GetFiles()
    {
        var count = Directory.GetFiles(
            DirectoryPath, "*", SearchOption.AllDirectories)
            .Length;
        return EnsureCorrectFileCount(count);
    }

    [Benchmark(Baseline = true)]
    public int Directory_EnumerateFiles()
    {
        var count = Directory.EnumerateFiles(
            DirectoryPath, "*", SearchOption.AllDirectories)
            .Count();
        return EnsureCorrectFileCount(count);
    }

    private int EnsureCorrectFileCount(int count)
    {
        if (count != ExpectedTotalFiles)
        {
            throw new Exception(
                $"""
                The expected file count {ExpectedTotalFiles} does not match the actual count {count}
                This error is unexpected, and indicates a bug in the library.
                """);
        }
        return count;
    }
}
