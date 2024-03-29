// -----------------------------------------------------------------------
// <copyright file="Roller.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils
{
    using System;
    using System.IO;
    using System.Linq;
    using UnityEngine;

    public static partial class FileUtils
    {
        public static class Roller
        {
            [Serializable]
            public struct RollConfig
            {
                [Flags]
                public enum Strategy
                {
                    None = 0,
                    FileSizeLimit = 1,
                    FileCountLimit = 2,
                }

                public Strategy strategy;
                public long maxBytes;
                public int maxFileCount;
                public Func<FileInfo, bool> filter;
                public Comparison<FileInfo> comparer;

                public static RollConfig Default = new RollConfig() { strategy = Strategy.FileSizeLimit | Strategy.FileCountLimit, maxBytes = 1024, maxFileCount = 10, comparer = DefaultComparer, filter = DefaultFilter };
                public static readonly Comparison<FileInfo> DefaultComparer = (f1, f2) => f1.LastWriteTime.CompareTo(f2.LastWriteTime);
                public static readonly Func<FileInfo, bool> DefaultFilter = f => true;
            }

            public static FileInfo FileRoll(string directory)
            {
                return FileRoll(directory, RollConfig.Default);
            }

            public static FileInfo FileRoll(string path, RollConfig config)
            {
                FileInfo fileInfo = new FileInfo(path);
                EnsureDirectory(fileInfo.DirectoryName);
                var filter = config.filter != null ? config.filter : RollConfig.DefaultFilter;
                var list = fileInfo.Directory.EnumerateFiles().Where(filter).ToList();
                var comparer = config.comparer != null ? config.comparer : RollConfig.DefaultComparer;
                list.Sort(comparer);
                int totalCount = list.Count;
                long totalBytes = GetDirectorySize(fileInfo.DirectoryName);
                int maxFileCount = Mathf.Max(config.maxFileCount, 0);
                long maxBytes = Math.Max(config.maxBytes, 0);
                if ((config.strategy & RollConfig.Strategy.FileCountLimit) > 0)
                {
                    while (totalCount > maxFileCount && list.Count > 0)
                    {
                        FileInfo toRemove = list[list.Count - 1];
                        list.RemoveAt(list.Count - 1);
                        totalCount -= 1;
                        totalBytes -= toRemove.Length;
                    }
                }

                if ((config.strategy & RollConfig.Strategy.FileSizeLimit) > 0)
                {
                    while (totalBytes > maxBytes && list.Count > 0)
                    {
                        FileInfo toRemove = list[list.Count - 1];
                        list.RemoveAt(list.Count - 1);
                        totalCount -= 1;
                        totalBytes -= toRemove.Length;
                    }
                }

                return Creator.GetFileInfoForCreation(path, Creator.CreateOnFileExistBehaviour.RenameNewFile);
            }
        }
    }
}
