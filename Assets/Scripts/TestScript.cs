using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

public static class TestScript
{
    private static readonly string[] ValidTypes = { "Common", "Rare", "Epic", "Legendary" };
    private const float MinPrice = 1f;
    private const float MaxPrice = 1000f;

    [MenuItem("Testing/Load and Validate")]
    public static void LoadAndValidate()
    {
        const string objectsFolder = "Assets/Items";
        var filePaths = Directory.GetFiles(objectsFolder, "*.asset");
        var files = filePaths.Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(PlayerItem)))
                            .Where(asset => asset != null)
                            .Cast<PlayerItem>()
                            .ToArray();

        UnityEngine.Debug.Log($"Files loaded:\n{string.Join("\n", files.Select(f => f.name))}");

        ValidatePlayerItems(files);
    }

    private static void ValidatePlayerItems(PlayerItem[] items)
    {
        var errors = new List<string>();

        foreach (var item in items)
        {
            ValidateItem(item, errors);
        }

        if (errors.Count == 0)
        {
            UnityEngine.Debug.Log("<color=green>✓ Validation passed! All PlayerItems are valid.</color>");
        }
        else
        {
            UnityEngine.Debug.LogError($"<color=red>✗ Validation failed! Found {errors.Count} errors:</color>");
            foreach (var error in errors)
            {
                UnityEngine.Debug.LogError($"  • {error}");
            }
        }
    }

    private static void ValidateItem(PlayerItem item, List<string> errors)
    {
        if (!ValidTypes.Contains(item.Type))
        {
            errors.Add($"{item.name}: Invalid Type '{item.Type}'. Expected: {string.Join(", ", ValidTypes)}");
        }

        var expectedName = $"PlayerItem_{item.Type}";
        if (item.name != expectedName)
        {
            errors.Add($"{item.name}: Name mismatch. Expected '{expectedName}' for Type '{item.Type}'");
        }

        if (item.Price < MinPrice || item.Price > MaxPrice)
        {
            errors.Add($"{item.name}: Price {item.Price} out of range [{MinPrice}-{MaxPrice}]");
        }
    }
}