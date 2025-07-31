using UnityEditor;
using UnityEngine;
using System.Linq;

public class PlayerItemAssetPostprocessor : AssetPostprocessor
{
    private static readonly string[] ValidTypes = { "Common", "Rare", "Epic", "Legendary" };
    
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            if (assetPath.Contains("PlayerItem_") && assetPath.EndsWith(".asset"))
            {
                var playerItem = AssetDatabase.LoadAssetAtPath<PlayerItem>(assetPath);
                if (playerItem != null)
                {
                    ValidateAndLogErrors(playerItem, assetPath);
                }
            }
        }
    }

    private static void ValidateAndLogErrors(PlayerItem item, string assetPath)
    {
        bool hasErrors = false;

        if (!ValidTypes.Contains(item.Type))
        {
            Debug.LogError($"<color=red>PlayerItem validation failed:</color> {assetPath} has invalid Type '{item.Type}'. Expected: {string.Join(", ", ValidTypes)}", item);
            hasErrors = true;
        }

        var expectedName = $"PlayerItem_{item.Type}";
        if (item.name != expectedName)
        {
            Debug.LogError($"<color=red>PlayerItem validation failed:</color> {assetPath} should be named '{expectedName}' based on Type '{item.Type}'", item);
            hasErrors = true;
        }

        if (item.Price < 1f || item.Price > 1000f)
        {
            Debug.LogError($"<color=red>PlayerItem validation failed:</color> {assetPath} has Price {item.Price} outside valid range [1, 1000]", item);
            hasErrors = true;
        }

        if (!hasErrors)
        {
            Debug.Log($"<color=green>PlayerItem validation passed:</color> {assetPath}");
        }
    }
}