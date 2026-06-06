using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDesignUpdater : MonoBehaviour
{
    [MenuItem("Map/Update Map Design with Sprites")]
    public static void UpdateMapDesign()
    {
        var scene = EditorSceneManager.GetActiveScene();
        // Exclude specific scenes from being modified by the updater
        string[] excludedScenes = new[] { "Map_Manta_Prototype" };
        if (excludedScenes.Contains(scene.name))
        {
            EditorUtility.DisplayDialog("Skipped", $"Scene '{scene.name}' is excluded from map updates.", "OK");
            return;
        }

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

        // Find all tilemaps and update them
        Tilemap[] allTilemaps = FindObjectsOfType<Tilemap>();
        int updatedCount = 0;

        foreach (Tilemap tilemap in allTilemaps)
        {
            if (tilemap.name == "Water_Tilemap")
            {
                PaintTilemapWithTile(tilemap, waterTile);
                Debug.Log($"✓ Updated {tilemap.name} with Water_Tile");
                updatedCount++;
            }
            else if (tilemap.name == "Ground_Tilemap" || tilemap.name == "Decoration_Tilemap" || tilemap.name == "Paths_Tilemap")
            {
                PaintTilemapWithTile(tilemap, beachTile);
                Debug.Log($"✓ Updated {tilemap.name} with Beach_Tile");
                updatedCount++;
            }
        }

        if (updatedCount == 0)
        {
            EditorUtility.DisplayDialog("Warning", "No tilemaps were updated. Check tilemap names.", "OK");
            return;
        }

        EditorSceneManager.MarkSceneDirty(scene);
        AssetDatabase.SaveAssets();
        EditorSceneManager.SaveScene(scene);
        EditorUtility.DisplayDialog("Success", $"Map design updated and saved! {updatedCount} tilemap(s) modified.", "OK");
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

    private static void PaintTilemapWithTile(Tilemap tilemap, Tile tile)
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase currentTile = tilemap.GetTile(pos);

            if (currentTile != null)
            {
                tilemap.SetTile(pos, tile);
            }
        }
    }

}
