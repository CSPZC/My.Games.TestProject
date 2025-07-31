using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class TestScript
{
    [MenuItem("Testing/Load and Validate")]
    public static void LoadAndValidate()
    {
        const string objectsFolder = "Assets/Items";
        var filePaths = Directory.GetFiles(objectsFolder, "*.asset");
        var files = filePaths.Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(PlayerItem)));

        Debug.Log($"Files loaded:\n{string.Join("\n", files)}");
        
        // Validation goes here
    }
}
