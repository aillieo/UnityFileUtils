// -----------------------------------------------------------------------
// <copyright file="EditorFileUtils.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public static class EditorFileUtils
    {
        public static string AssetPathToFilePath(string assetPath)
        {
            assetPath = FileUtils.GetCleanPathStr(assetPath);
            return $"{Application.dataPath}/../{assetPath}";
        }

        public static string FilePathToAssetPath(string filePath)
        {
            filePath = FileUtils.GetCleanPathStr(filePath);
            if (!filePath.StartsWith(Application.dataPath))
            {
                return null;
            }

            return $"Assets/{filePath.Replace(Application.dataPath, string.Empty)}";
        }

        public static void OpenFolder(string folder)
        {
            EditorUtility.RevealInFinder(folder);
        }

        public static bool IsReadOnly(UnityEngine.Object asset)
        {
            if (!AssetDatabase.IsOpenForEdit(asset, StatusQueryOptions.UseCachedIfPossible))
            {
                return false;
            }

            string assetPath = AssetDatabase.GetAssetPath(asset);
            if (!string.IsNullOrEmpty(assetPath))
            {
                return (File.GetAttributes(assetPath) & FileAttributes.ReadOnly) != 0;
            }

            return false;
        }
    }
}
