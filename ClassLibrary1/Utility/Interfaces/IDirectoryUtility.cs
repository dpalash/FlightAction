using System.Collections.Generic;
using System.IO;
using CSharpFunctionalExtensions;

namespace Framework.Utility.Interfaces
{
    public interface IDirectoryUtility
    {
        Maybe<IEnumerable<string>> GetAllFilesInDirectory(string dir, List<string> extensions = null, string searchPattern = "*.*");
        DirectoryInfo CreateFolderIfNotExistAsync(string folderNameWithPath);
        Result Move(string sourcePath, string targetPath);
        void CopyDirectoryStructure(DirectoryInfo source, DirectoryInfo target);
        Result DeleteFile(string path);
        void DeleteFilesByPattern(string directoryPath, string searchPattern);
        void DeleteEmptyFoldersFromDirectory(string directoryPath);
    }
}