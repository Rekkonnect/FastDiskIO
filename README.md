# FastDiskIO

A project providing fast disk IO operations

## Disclaimer

The contained code is targeted for Windows devices.

Source code for the fast file enumeration was originally taken from
https://www.codeproject.com/Articles/38959/A-Faster-Directory-Enumerator, authored by **wilsone8**.

The code was modified accordingly to prove that the implemented mechanism can be faster than .NET's
built-in file enumeration technique. The original post mentions being able to enumerate files over
the local network also, which has not been tested in this library. Feel free to test this out.

## Give me the juice

- Count the number of files and subdirectories recursively contained within a directory:
  - Slightly faster (~5%) and much less memory allocated (~99.5%)

## Benchmarks

A minified version of an example run of the benchmarks in [EnumeratedFileCountBenchmark.cs](https://github.com/Rekkonnect/FastDiskIO/blob/master/FastDiskIO.Benchmarks/EnumeratedFileCountBenchmark.cs)

| Method                   | Mean     | Ratio | Allocated   | Alloc Ratio |
|------------------------- |---------:|------:|------------:|------------:|
| GetFileCount             | 111.1 ms |  0.95 |    99.25 KB |       0.004 |
| Directory_EnumerateFiles | 116.6 ms |  1.00 | 25741.13 KB |       1.000 |

Evaluated directory structure:
- root/
  - 1/
    - 1/
    - 2/ ... through 300/
    - 1.txt through 1000.txt
  - 2/ ... through 300/
    - same contents as root/1/
  - 1.txt through 1000.txt

Total: 301,000 Files, 300 Folders

## Future considerations

This repo might contain more faster disk IO operations in the future covering arising needs.
Feel free to open issues ensuring that enough research was done covering the requested operation.
