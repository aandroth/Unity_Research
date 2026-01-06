using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

/// <summary>
/// Editor tool to rename ItemData ScriptableObjects based on their sprite names
/// </summary>
public class ItemDataRenamer : EditorWindow
{
    private string itemsFolderPath = "Assets/Procedural_Generation/Scripts/Data/Items";
    private Vector2 scrollPosition;
    private bool showPreview = true;
    
    [MenuItem("Tools/Rename ItemData Objects")]
    public static void ShowWindow()
    {
        GetWindow<ItemDataRenamer>("ItemData Renamer");
    }
    
    [MenuItem("Assets/Rename ItemData by Sprite Name", true)]
    public static bool ValidateRenameItemData()
    {
        // Only show in menu when right-clicking on a folder or ItemData asset
        Object[] selection = Selection.objects;
        if (selection.Length != 1) return false;
        
        string path = AssetDatabase.GetAssetPath(selection[0]);
        return AssetDatabase.IsValidFolder(path) || selection[0] is ItemData;
    }
    
    [MenuItem("Assets/Rename ItemData by Sprite Name")]
    public static void RenameItemDataFromContext()
    {
        Object[] selection = Selection.objects;
        if (selection.Length != 1) return;
        
        string path = AssetDatabase.GetAssetPath(selection[0]);
        
        if (AssetDatabase.IsValidFolder(path))
        {
            RenameItemDataInFolder(path);
        }
        else if (selection[0] is ItemData)
        {
            RenameItemDataInFolder(Path.GetDirectoryName(path).Replace('\\', '/'));
        }
    }
    
    static void RenameItemDataInFolder(string folderPath)
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { folderPath });
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("No ItemData Found", "No ItemData objects found in this folder.", "OK");
            return;
        }
        
        int renamed = 0;
        int skipped = 0;
        int errors = 0;
        
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
            
            if (itemData == null || itemData.sprite == null)
            {
                skipped++;
                continue;
            }
            
            string spriteName = itemData.sprite.name;
            string cleanName = CleanSpriteNameForRename(spriteName);
            
            if (string.IsNullOrEmpty(cleanName))
            {
                skipped++;
                continue;
            }
            
            string directory = Path.GetDirectoryName(assetPath).Replace('\\', '/');
            string newAssetPath = directory + "/" + cleanName + ".asset";
            
            if (newAssetPath != assetPath && File.Exists(newAssetPath))
            {
                cleanName = GetUniqueNameStatic(directory, cleanName);
                newAssetPath = directory + "/" + cleanName + ".asset";
            }
            
            string error = AssetDatabase.RenameAsset(assetPath, cleanName);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"Failed to rename {assetPath}: {error}");
                errors++;
            }
            else
            {
                itemData.name = cleanName;
                EditorUtility.SetDirty(itemData);
                renamed++;
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog(
            "Rename Complete",
            $"Renamed: {renamed}\n" +
            (skipped > 0 ? $"Skipped: {skipped}\n" : "") +
            (errors > 0 ? $"Errors: {errors}\n" : ""),
            "OK"
        );
    }
    
    static string CleanSpriteNameForRename(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName))
            return null;
        
        string cleaned = spriteName;
        if (cleaned.StartsWith("Dungeon_Tileset_"))
        {
            cleaned = cleaned.Substring("Dungeon_Tileset_".Length);
        }
        
        if (cleaned.StartsWith("Dungeon_Tileset"))
        {
            cleaned = cleaned.Substring("Dungeon_Tileset".Length);
            while (cleaned.Length > 0 && (cleaned[0] == '_' || char.IsDigit(cleaned[0])))
            {
                cleaned = cleaned.Substring(1);
            }
        }
        
        if (string.IsNullOrEmpty(cleaned) || cleaned.All(char.IsDigit))
        {
            return cleaned;
        }
        
        cleaned = cleaned.Replace("_", " ");
        if (!string.IsNullOrEmpty(cleaned))
        {
            cleaned = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleaned);
            cleaned = cleaned.Replace(" ", "");
        }
        
        return cleaned;
    }
    
    static string GetUniqueNameStatic(string directory, string baseName)
    {
        int counter = 1;
        string uniqueName = baseName;
        
        while (File.Exists(directory + "/" + uniqueName + ".asset"))
        {
            uniqueName = baseName + "_" + counter;
            counter++;
        }
        
        return uniqueName;
    }
    
    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        EditorGUILayout.LabelField("ItemData Renamer", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Folder path
        EditorGUILayout.LabelField("Items Folder", EditorStyles.boldLabel);
        itemsFolderPath = EditorGUILayout.TextField("Folder Path", itemsFolderPath);
        
        if (GUILayout.Button("Browse Folder"))
        {
            string path = EditorUtility.OpenFolderPanel("Select Items Folder", itemsFolderPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                itemsFolderPath = "Assets" + path.Replace(Application.dataPath, "");
            }
        }
        
        EditorGUILayout.Space();
        
        // Preview option
        showPreview = EditorGUILayout.Toggle("Show Preview", showPreview);
        
        EditorGUILayout.Space();
        
        // Rename button
        if (GUILayout.Button("Rename All ItemData Objects", GUILayout.Height(30)))
        {
            RenameAllItemDataObjects();
        }
        
        EditorGUILayout.Space();
        
        // Help box
        EditorGUILayout.HelpBox(
            "This tool will rename all ItemData ScriptableObjects in the specified folder " +
            "to match their sprite names. The sprite name will be cleaned up (removing 'Dungeon_Tileset_' prefix if present).",
            MessageType.Info
        );
        
        EditorGUILayout.EndScrollView();
    }
    
    void RenameAllItemDataObjects()
    {
        if (!AssetDatabase.IsValidFolder(itemsFolderPath))
        {
            EditorUtility.DisplayDialog("Error", "Invalid folder path. Please select a valid folder.", "OK");
            return;
        }
        
        // Get all ItemData assets in the folder
        string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { itemsFolderPath });
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("No ItemData Found", "No ItemData objects found in the specified folder.", "OK");
            return;
        }
        
        int renamed = 0;
        int skipped = 0;
        int errors = 0;
        
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
            
            if (itemData == null || itemData.sprite == null)
            {
                skipped++;
                continue;
            }
            
            // Get sprite name and clean it up
            string spriteName = itemData.sprite.name;
            string cleanName = CleanSpriteName(spriteName);
            
            if (string.IsNullOrEmpty(cleanName))
            {
                skipped++;
                continue;
            }
            
            // Get the directory and create new path
            string directory = Path.GetDirectoryName(assetPath).Replace('\\', '/');
            string newAssetPath = directory + "/" + cleanName + ".asset";
            
            // Check if target name already exists (and it's not the same file)
            if (newAssetPath != assetPath && File.Exists(newAssetPath))
            {
                // Handle duplicates by appending a number
                cleanName = GetUniqueName(directory, cleanName);
                newAssetPath = directory + "/" + cleanName + ".asset";
            }
            
            // Rename the asset
            string error = AssetDatabase.RenameAsset(assetPath, cleanName);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"Failed to rename {assetPath}: {error}");
                errors++;
            }
            else
            {
                // Also update the object name
                itemData.name = cleanName;
                EditorUtility.SetDirty(itemData);
                renamed++;
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog(
            "Rename Complete",
            $"Renamed: {renamed}\n" +
            (skipped > 0 ? $"Skipped: {skipped}\n" : "") +
            (errors > 0 ? $"Errors: {errors}\n" : ""),
            "OK"
        );
    }
    
    /// <summary>
    /// Cleans up the sprite name by removing common prefixes and making it more readable
    /// </summary>
    string CleanSpriteName(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName))
            return null;
        
        // Remove "Dungeon_Tileset_" prefix if present
        string cleaned = spriteName;
        if (cleaned.StartsWith("Dungeon_Tileset_"))
        {
            cleaned = cleaned.Substring("Dungeon_Tileset_".Length);
        }
        
        // Remove "Dungeon_Tileset" prefix if present (without underscore)
        if (cleaned.StartsWith("Dungeon_Tileset"))
        {
            cleaned = cleaned.Substring("Dungeon_Tileset".Length);
            // Remove leading underscore or number if present
            while (cleaned.Length > 0 && (cleaned[0] == '_' || char.IsDigit(cleaned[0])))
            {
                cleaned = cleaned.Substring(1);
            }
        }
        
        // If it's just a number, keep it as-is (we'll let the user manually rename these)
        if (string.IsNullOrEmpty(cleaned) || cleaned.All(char.IsDigit))
        {
            return cleaned; // Return the number as-is
        }
        
        // Replace underscores with spaces for readability
        cleaned = cleaned.Replace("_", " ");
        
        // Capitalize first letter of each word
        if (!string.IsNullOrEmpty(cleaned))
        {
            cleaned = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleaned);
            // Remove spaces for filename (use PascalCase)
            cleaned = cleaned.Replace(" ", "");
            
            // Make first letter lowercase for camelCase style if preferred, or keep PascalCase
            // For now, keeping PascalCase
        }
        
        return cleaned;
    }
    
    /// <summary>
    /// Gets a unique name by appending a number if the name already exists
    /// </summary>
    string GetUniqueName(string directory, string baseName)
    {
        int counter = 1;
        string uniqueName = baseName;
        
        while (File.Exists(directory + "/" + uniqueName + ".asset"))
        {
            uniqueName = baseName + "_" + counter;
            counter++;
        }
        
        return uniqueName;
    }
}

