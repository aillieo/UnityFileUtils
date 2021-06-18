using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AillieoUtils
{
    public static partial class FileUtils
    {
        public static class Creator
        {
            public enum CreateOnFileExistBehaviour
            {
                Exception,
                Ignore,
                Overwrite,
                RenameNewFile,
            }

            public static FileInfo GetFileInfoForCreation(string path, CreateOnFileExistBehaviour behaviour)
            {
                FileInfo file = new FileInfo(path);
                EnsureDirectory(file.DirectoryName);
                if (file.Exists)
                {
                    switch (behaviour)
                    {
                        case CreateOnFileExistBehaviour.Exception:
                            throw new IOException($"file already exists {path}");
                        case CreateOnFileExistBehaviour.Ignore:
                            return null;
                        case CreateOnFileExistBehaviour.Overwrite:
                            file.Attributes = FileAttributes.Normal;
                            file.Delete();
                            break;
                        case CreateOnFileExistBehaviour.RenameNewFile:
                            return GetFileInfoForCreationWithIncreasingNumberSuffix(path);
                    }
                }
                return file;
            }

            public static FileStream CreateFile(string path, CreateOnFileExistBehaviour behaviour)
            {
                return GetFileInfoForCreation(path, behaviour)?.Create();
            }

            public static FileInfo GetFileInfoForCreationInPersistentDataPath(string path, CreateOnFileExistBehaviour behaviour = CreateOnFileExistBehaviour.Exception)
            {
                path = Path.Combine(Application.persistentDataPath, GetCleanPathStr(path));
                return GetFileInfoForCreation(path, behaviour);
            }

            public static FileInfo GetFileInfoForCreationInDataPath(string path, CreateOnFileExistBehaviour behaviour = CreateOnFileExistBehaviour.Exception)
            {
                path = Path.Combine(Application.dataPath, GetCleanPathStr(path));
                return GetFileInfoForCreation(path, behaviour);
            }

            public static FileInfo GetFileInfoForCreationInLogPath(string path, CreateOnFileExistBehaviour behaviour = CreateOnFileExistBehaviour.RenameNewFile)
            {
                path = Path.Combine(Application.consoleLogPath, GetCleanPathStr(path));
                return GetFileInfoForCreation(path, behaviour);
            }

            public static FileStream CreateFileInPersistentDataPath(string path, CreateOnFileExistBehaviour behaviour = CreateOnFileExistBehaviour.Exception)
            {
                return GetFileInfoForCreationInPersistentDataPath(path, behaviour)?.Create();
            }

            public static FileStream CreateFileInDataPath(string path, CreateOnFileExistBehaviour behaviour = CreateOnFileExistBehaviour.Exception)
            {
                return GetFileInfoForCreationInDataPath(path, behaviour)?.Create();
            }

            public static FileStream CreateFileInLogPath(string path, CreateOnFileExistBehaviour behaviour = CreateOnFileExistBehaviour.RenameNewFile)
            {
                return GetFileInfoForCreationInLogPath(path, behaviour)?.Create();
            }

            public static FileInfo GetFileInfoForCreationWithIncreasingNumberSuffix(string path)
            {
                path = MakeUniqueNumberSuffix(path);
                return new FileInfo(path);
            }

            public static FileStream CreateFileWithIncreasingNumberSuffix(string path)
            {
                return GetFileInfoForCreationWithIncreasingNumberSuffix(path)?.Create();
            }
        }
    }
}
