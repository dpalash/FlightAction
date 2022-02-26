using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSharpFunctionalExtensions;
using Framework.Utility.Interfaces;
using Serilog;

namespace Framework.Utility
{
    public class DirectoryUtility : IDirectoryUtility
    {
        private const string SystemVolumeInformation = "System Volume Information";

        private readonly ILogger _logger;

        public DirectoryUtility(ILogger logger)
        {
            _logger = logger;
        }

        public Maybe<IEnumerable<string>> GetAllFilesInDirectory(string dir, List<string> extensions = null, string searchPattern = "*.*")
        {
            if (Directory.Exists(dir))
            {
                List<string> files;

                if (extensions != null)
                    files = Directory.GetFiles(dir, searchPattern, SearchOption.AllDirectories)
                        .Where(file => extensions
                        .Any(e => string.Compare(Path.GetExtension(file), e, StringComparison.InvariantCultureIgnoreCase) == 0))
                        .ToList();
                else
                    files = Directory.GetFiles(dir, searchPattern, SearchOption.AllDirectories).ToList();

                if (files.Any())
                    return files;
            }

            return null;
        }

        /// <summary>
        /// Returns the names of files in a specified directories that match the specified patterns using LINQ
        /// </summary>
        /// <param name="srcDirs">The directories to search</param>
        /// <param name="searchPatterns">the list of search patterns</param>
        /// <param name="searchOption"></param>
        /// <returns>The list of files that match the specified pattern</returns>
        public static string[] GetFilesUsingLinq(string[] srcDirs, string[] searchPatterns, SearchOption searchOption = SearchOption.AllDirectories)
        {
            var r = from dir in srcDirs
                    from searchPattern in searchPatterns
                    from f in Directory.GetFiles(dir, searchPattern, searchOption)
                    select f;

            return r.ToArray();
        }

        public DirectoryInfo CreateFolderIfNotExistAsync(string folderNameWithPath)
        {
            if (string.IsNullOrWhiteSpace(folderNameWithPath))
                throw new ArgumentException("Path is null or empty.", nameof(folderNameWithPath));

            DirectoryInfo createdDirectory;

            try
            {
                createdDirectory = Directory.CreateDirectory(folderNameWithPath);
            }
            catch (Exception ex)
            {
                createdDirectory = null;
                _logger.Error(ex, $"Failed to create directory from method:{nameof(CreateFolderIfNotExistAsync)}");
            }

            return createdDirectory;
        }

        public Result Move(string sourcePath, string targetPath)
        {
            try
            {

                Directory.Move(sourcePath, targetPath);
                return Result.Success();
            }
            catch (Exception e)
            {
                return Result.Failure(e.Message);
            }
        }

        public void CopyDirectoryStructure(DirectoryInfo source, DirectoryInfo target)
        {
            if (!source.Exists)
                return;

            Directory.CreateDirectory(target.FullName);

            foreach (var fi in source.GetFiles())
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);

            foreach (var diSourceSubDir in source.GetDirectories())
                CopyDirectoryStructure(diSourceSubDir, target.CreateSubdirectory(diSourceSubDir.Name));
        }


        public Result DeleteFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Result.Failure($"{nameof(path)} is null/empty");

            if (!File.Exists(path))
                return Result.Success($"[{path}] is already deleted");

            try
            {
                File.Delete(path);
                return Result.Success();
            }
            catch (Exception e)
            {
                return Result.Failure(e.Message);
            }
        }

        public void DeleteFilesByPattern(string directoryPath, string searchPattern)
        {
            foreach (var filePath in Directory.EnumerateFiles(directoryPath, searchPattern))
            {
                File.Delete(filePath);
            }
        }

        public void DeleteEmptyFoldersFromDirectory(string directoryPath)
        {
            if (directoryPath.Contains(SystemVolumeInformation))
                return;

            var directories = Directory.GetDirectories(directoryPath);

            if (!directories.Any())
                return;

            foreach (var dir in directories)
            {
                DeleteEmptyFoldersFromDirectory(dir);

                if (!dir.Contains(SystemVolumeInformation) && Directory.GetFiles(dir).Length == 0 && Directory.GetDirectories(dir).Length == 0)
                    Directory.Delete(dir, false);
            }
        }
    }
}
