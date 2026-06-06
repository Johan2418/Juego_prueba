using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDesignUpdater : MonoBehaviour
{
    [MenuItem("Map/Update Map Design with Sprites")]
    public static void UpdateMapDesign()
    {
        // Load sprites
        Sprite waterSprite = LoadSpriteFromPath("Assets/Sprites/Cute_Fantasy_Free/Tiles/Water_Tile.png");
        Sprite beachSprite = LoadSpriteFromPath("Assets/Sprites/Cute_Fantasy_Free/Tiles/Beach_Tile.png");

        if (waterSprite == null || beachSprite == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not load sprites:\nWater_Tile.png or Beach_Tile.png not found", "OK");
            return;
        }

        // Create Tiles folder if needed
        if (!AssetDatabase.IsValidFolder("Assets/Tiles"))
        {
            AssetDatabase.CreateFolder("Assets", "Tiles");
        }

        Tile waterTile = GetOrCreateTile("Assets/Tiles/WaterTile.asset", waterSprite);
        Tile beachTile = GetOrCreateTile("Assets/Tiles/BeachTile.asset", beachSprite);
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Success", "Tile assets updated. The current scene was left unchanged to preserve your map layout.", "OK");
    }

    private static Sprite LoadSpriteFromPath(string path)
    {
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
        return sprites.Length > 0 ? sprites[0] : null;
    }

    private static Tile GetOrCreateTile(string assetPath, Sprite sprite)
    {
        Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(assetPath);

        if (tile == null)
        {
            tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            AssetDatabase.CreateAsset(tile, assetPath);
        }
        else
        {
            tile.sprite = sprite;
        }

        return tile;
    }

}
