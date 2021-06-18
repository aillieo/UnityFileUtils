using AillieoUtils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Tests : MonoBehaviour
{
    private void OnEnable()
    {
        string path = "folder/folder2/aa3_.txt";
        var fi = FileUtils.Creator.GetFileInfoForCreationInDataPath(path, FileUtils.Creator.CreateOnFileExistBehaviour.RenameNewFile);
//        fi.Create();

#if UNITY_EDITOR
        EditorFileUtils.OpenFolder(fi.FullName);
#endif
        
        Debug.LogError(fi.FullName);
        
        
        
        var fi2 = FileUtils.Creator.GetFileInfoForCreationInDataPath(FileUtils.AddTimeSuffix(path), FileUtils.Creator.CreateOnFileExistBehaviour.RenameNewFile);
        // fi2.Create();
#if UNITY_EDITOR
        // EditorFileUtils.OpenFolder(fi2.FullName);
#endif
        Debug.LogError(fi2.FullName);
        
        Debug.LogError($"{path} || {FileUtils.AddTimeSuffix(path)}");
        
    }




    public string testFind = "aa3 2.txt";
    [ContextMenu("Find")]
    public void Find()
    {
        //Debug.LogError($"{testFind} found {FileUtils.TryFindNumberSuffix(testFind)}");
    }
}
