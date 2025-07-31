using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class PlayerItemValidator
{
    private static readonly string[] ValidTypes = { "Common", "Rare", "Epic", "Legendary" };
    private const float MinPrice = 1f;
    private const float MaxPrice = 1000f;
    private const string ItemsFolder = "Assets/Items";

    [MenuItem("Testing/Validate Player Items")]
    public static void ValidateAllPlayerItems()
    {
        var errors = new List<string>();
        var playerItems = LoadAllPlayerItems();

        Debug.Log($"Found {playerItems.Length} PlayerItem assets to validate");

        foreach (var item in playerItems)
        {
            ValidatePlayerItem(item, errors);
        }

        DisplayResults(errors);
    }

    [MenuItem("Testing/Auto-Fix Player Items")]
    public static void AutoFixPlayerItems()
    {
        var playerItems = LoadAllPlayerItems();
        var fixedCount = 0;

        foreach (var item in playerItems)
        {
            if (AutoFixPlayerItem(item))
            {
                fixedCount++;
                EditorUtility.SetDirty(item);
            }
        }

        if (fixedCount > 0)
        {
            AssetDatabase.SaveAssets();
            Debug.Log($"Auto-fixed {fixedCount} PlayerItems");
        }
        else
        {
            Debug.Log("No items needed fixing");
        }

        ValidateAllPlayerItems();
    }

    private static PlayerItem[] LoadAllPlayerItems()
    {
        if (!Directory.Exists(ItemsFolder))
        {
            Debug.LogError($"Items folder not found: {ItemsFolder}");
            return new PlayerItem[0];
        }

        var assetPaths = Directory.GetFiles(ItemsFolder, "*.asset")
            .Where(path => Path.GetFileName(path).StartsWith("PlayerItem_"))
            .ToArray();

        var items = new List<PlayerItem>();
        foreach (var path in assetPaths)
        {
            var item = AssetDatabase.LoadAssetAtPath<PlayerItem>(path);
            if (item != null)
            {
                items.Add(item);
            }
        }

        return items.ToArray();
    }

    private static void ValidatePlayerItem(PlayerItem item, List<string> errors)
    {
        var itemName = item.name;

        if (!ValidTypes.Contains(item.Type))
        {
            errors.Add($"{itemName}: Invalid Type '{item.Type}'. Must be one of: {string.Join(", ", ValidTypes)}");
        }

        var expectedName = $"PlayerItem_{item.Type}";
        if (itemName != expectedName)
        {
            errors.Add($"{itemName}: Name should be '{expectedName}' based on Type '{item.Type}'");
        }

        if (item.Price < MinPrice || item.Price > MaxPrice)
        {
            errors.Add($"{itemName}: Price {item.Price} is out of range [{MinPrice}, {MaxPrice}]");
        }
    }

    private static void DisplayResults(List<string> errors)
    {
        if (errors.Count == 0)
        {
            Debug.Log("<color=green>✓ All PlayerItems are valid!</color>");
            return;
        }

        Debug.LogError($"<color=red>✗ Found {errors.Count} validation errors:</color>");
        foreach (var error in errors)
        {
            Debug.LogError($"  • {error}");
        }
    }

    private static bool AutoFixPlayerItem(PlayerItem item)
    {
        bool wasFixed = false;

        if (item.Type == "Lehendary")
        {
            Debug.Log($"Auto-fixing: {item.name} Type 'Lehendary' → 'Legendary'");
            item.Type = "Legendary";
            wasFixed = true;
        }

        if (item.Price < MinPrice)
        {
            Debug.Log($"Auto-fixing: {item.name} Price {item.Price} → {MinPrice}");
            item.Price = MinPrice;
            wasFixed = true;
        }
        else if (item.Price > MaxPrice)
        {
            Debug.Log($"Auto-fixing: {item.name} Price {item.Price} → {MaxPrice}");
            item.Price = MaxPrice;
            wasFixed = true;
        }

        return wasFixed;
    }

    [MenuItem("Testing/Load and Validate")]
    public static void LoadAndValidateFromTestScript()
    {
        Debug.Log("Running validation from menu instead of TestScript...");
        ValidateAllPlayerItems();
    }
}