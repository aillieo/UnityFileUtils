// -----------------------------------------------------------------------
// <copyright file="FileUtils.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;

    public static partial class FileUtils
    {
        private static readonly char[] invalidFileNameChars;
        private static readonly char[] invalidPathChars;

        static FileUtils()
        {
            invalidFileNameChars = Path.GetInvalidFileNameChars();
            invalidPathChars = Path.GetInvalidPathChars();
        }

        public static string GetPersistentPath(string folder)
        {
#if UNITY_EDITOR
            return Path.Combine(Application.dataPath, "..", folder);
#else
            return Path.Combine(Application.persistentDataPath, folder);
#endif
        }

        public static string GetCleanPathStr(string rawPath)
        {
            if (string.IsNullOrWhiteSpace(rawPath))
            {
                return rawPath;
            }

            foreach (var ch in invalidPathChars)
            {
                rawPath = rawPath.Replace(ch, '_');
            }

            rawPath.Replace("\\", "/");
            return rawPath;
        }

        public static string GetCleanFileName(string rawFileName)
        {
            if (string.IsNullOrWhiteSpace(rawFileName))
            {
                return rawFileName;
            }

            foreach (var ch in invalidFileNameChars)
            {
                rawFileName = rawFileName.Replace(ch, '_');
            }

            return rawFileName;
        }

        public static void EnsureDirectory(string path)
        {
            // path = GetCleanPathStr(path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static bool ExistFileOrDirectory(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        public static string MakeUniqueNumberSuffix(string path, int existIndex = 0)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
            {
                return path;
            }

            string dir = fileInfo.DirectoryName;
            string fileName = Path.GetFileNameWithoutExtension(path);
            string fileExt = fileInfo.Extension;

            for (int i = existIndex + 1; ; ++i)
            {
                if (!File.Exists(path))
                {
                    return path;
                }

                path = Path.Combine(dir, $"{fileName} {i}{fileExt}");
            }
        }

        internal static int TryFindNumberSuffix(string path)
        {
            Regex regex = new Regex(@".+ ([\d]+)");
            Match match = regex.Match(Path.GetFileNameWithoutExtension(path));
            if (match.Success)
            {
                var numberGroup = match.Groups[1];
                if (numberGroup.Success && int.TryParse(numberGroup.Value, out int num))
                {
                    return num;
                }
            }

            return 0;
        }

        public static string UnifyDirectorySeparator(string path)
        {
            return path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        }

        public static void ClearDirectory(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            foreach (FileInfo f in directory.EnumerateFiles())
            {
                f.Delete();
            }

            foreach (DirectoryInfo d in directory.EnumerateDirectories())
            {
                d.Delete(true);
            }
        }

        public static void DeleteDirectory(string path)
        {
            ClearDirectory(path);
            Directory.Delete(path);
        }

        public static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
                }
            }
        }

        public static long GetDirectorySize(DirectoryInfo directoryInfo, bool recursive = true)
        {
            if (directoryInfo == null)
            {
                return 0;
            }

            if (!directoryInfo.Exists)
            {
                return 0;
            }

            long totalBytes = 0;
            foreach (var fileInfo in directoryInfo.GetFiles())
            {
                Interlocked.Add(ref totalBytes, fileInfo.Length);
            }

            if (recursive)
            {
                Parallel.ForEach(
                    directoryInfo.GetDirectories(),
                    subDirectory => Interlocked.Add(ref totalBytes, GetDirectorySize(subDirectory, true)));
            }

            return totalBytes;
        }

        public static long GetDirectorySize(string path, bool recursive = true)
        {
            if (string.IsNullOrEmpty(path))
            {
                return 0;
            }

            return GetDirectorySize(new DirectoryInfo(path), recursive);
        }

        public static string[] Split(string path)
        {
            return path.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string AddFilenameSuffix(string path, string suffix)
        {
            return $"{Path.GetDirectoryName(path)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(path)}{suffix}{Path.GetExtension(path)}";
        }

        public static void MakeWritable(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                {
                    var fileInfo = new FileInfo(file);
                    fileInfo.IsReadOnly = false;
                }
            }

            if (File.Exists(path))
            {
                var fileInfo = new FileInfo(path);
                fileInfo.IsReadOnly = false;
            }
        }

        public static string AddDateSuffix(string filename)
        {
            string dateStr = DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            return AddFilenameSuffix(filename, dateStr);
        }

        public static string AddTimeSuffix(string filename)
        {
            string timeStr = DateTime.Now.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
            return AddFilenameSuffix(filename, timeStr);
        }
    }
}
