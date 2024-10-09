using NUnit.Framework;

namespace FastDiskIO.Tests;

public class EnumeratedFileCountTests
{
    private const string _baseDirectoryPath = @".\dummy\";

    [Test]
    public void Test()
    {
        var createdDirectories = new List<DirectoryInfo>();
        var createdFiles = new List<FileInfo>();

        ClearDirectories();
        CreateDirectories();
        CreateAllFiles();

        var enumeratedCount = FastDirectoryEnumeration.GetEnumeratedFileCount(
            _baseDirectoryPath, searchOption: SearchOption.AllDirectories);

        Assert.Multiple(() =>
        {
            Assert.That(enumeratedCount.Files, Is.EqualTo(createdFiles.Count));
            Assert.That(enumeratedCount.Directories, Is.EqualTo(createdDirectories.Count));
        });

        void ClearDirectories()
        {
            Directory.Delete(_baseDirectoryPath, recursive: true);
        }

        void CreateDirectories()
        {
            CreateDirectory("a");
            CreateDirectory("a", "01");
            CreateDirectory("a", "01", "i");
            CreateDirectory("a", "01", "ii");
            CreateDirectory("a", "02");
            CreateDirectory("a", "03");
            CreateDirectory("b");
            CreateDirectory("c");
        }

        void CreateDirectory(params string[] paths)
        {
            var directory = Directory.CreateDirectory(
                Path.Combine([_baseDirectoryPath, .. paths]));
            createdDirectories.Add(directory);
        }

        void CreateAllFiles()
        {
            foreach (var directory in createdDirectories)
            {
                CreateFiles(directory);
            }
        }

        void CreateFiles(DirectoryInfo directory)
        {
            const int fileCount = 7;
            for (int i = 0; i < fileCount; i++)
            {
                var path = Path.Combine(directory.FullName, $"file{i}.txt");
                var file = CreateFile(path);
                createdFiles.Add(file);
            }
        }

        static FileInfo CreateFile(string path)
        {
            var file = new FileInfo(path);
            file.Create().Close();
            return file;
        }
    }
}
