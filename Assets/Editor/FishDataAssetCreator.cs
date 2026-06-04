using UnityEditor;
using UnityEngine;

public static class FishDataAssetCreator
{
    private const string TargetFolder = "Assets/_Project/ScriptableObjects/Fish";

    [MenuItem("Tools/Fishing/Create Default Fish Data")]
    public static void CreateDefaultFishData()
    {
        EnsureFolder(TargetFolder);

        CreateOrUpdateFish("Fish_Corvina", "fish_corvina", "Corvina", FishRarity.Common, 5f, 1);
        CreateOrUpdateFish("Fish_Albacora", "fish_albacora", "Albacora", FishRarity.Common, 4f, 1);
        CreateOrUpdateFish("Fish_Dorado", "fish_dorado", "Dorado", FishRarity.Uncommon, 2f, 2);
        CreateOrUpdateFish("Fish_Picudo", "fish_picudo", "Picudo", FishRarity.Rare, 1f, 3);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Default FishData assets are ready in {TargetFolder}.");
    }

    private static void CreateOrUpdateFish(string assetName, string itemId, string displayName, FishRarity rarity, float catchWeight, int sellPrice)
    {
        string assetPath = $"{TargetFolder}/{assetName}.asset";
        FishData fishData = AssetDatabase.LoadAssetAtPath<FishData>(assetPath);

        if (fishData == null)
        {
            fishData = ScriptableObject.CreateInstance<FishData>();
            AssetDatabase.CreateAsset(fishData, assetPath);
        }

        SerializedObject serializedFish = new SerializedObject(fishData);
        SetString(serializedFish, "itemId", itemId);
        SetString(serializedFish, "displayName", displayName);
        SetEnum(serializedFish, "rarity", (int)rarity);
        SetFloat(serializedFish, "catchWeight", catchWeight);
        SetInt(serializedFish, "sellPrice", sellPrice);
        serializedFish.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(fishData);
    }

    private static void EnsureFolder(string folderPath)
    {
        string[] parts = folderPath.Split('/');
        string currentPath = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string nextPath = $"{currentPath}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, parts[i]);
            }

            currentPath = nextPath;
        }
    }

    private static void SetString(SerializedObject serializedObject, string propertyName, string value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property != null)
        {
            property.stringValue = value;
        }
    }

    private static void SetEnum(SerializedObject serializedObject, string propertyName, int value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property != null)
        {
            property.enumValueIndex = value;
        }
    }

    private static void SetFloat(SerializedObject serializedObject, string propertyName, float value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property != null)
        {
            property.floatValue = value;
        }
    }

    private static void SetInt(SerializedObject serializedObject, string propertyName, int value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property != null)
        {
            property.intValue = value;
        }
    }
}
