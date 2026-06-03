using UnityEditor;
using UnityEngine;

namespace JuegoPrueba.Editor.Art
{
    /// <summary>
    /// Applies the project's default pixel art import settings to selected textures.
    /// </summary>
    public static class SpriteImportSettingsApplier
    {
        private const int DefaultPixelsPerUnit = 16;
        private const string MenuPath = "Tools/Art/Apply Pixel Art Import Settings To Selected Textures";

        [MenuItem(MenuPath)]
        private static void ApplyToSelectedTextures()
        {
            int configuredCount = 0;

            foreach (Object selectedObject in Selection.objects)
            {
                if (!(selectedObject is Texture2D))
                {
                    continue;
                }

                string assetPath = AssetDatabase.GetAssetPath(selectedObject);
                if (string.IsNullOrEmpty(assetPath))
                {
                    continue;
                }

                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer == null)
                {
                    continue;
                }

                if (!ApplyPixelArtSettings(importer))
                {
                    continue;
                }

                importer.SaveAndReimport();
                configuredCount++;
            }

            Debug.Log($"[Art] Pixel art import settings applied to {configuredCount} selected texture(s).");
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateApplyToSelectedTextures()
        {
            foreach (Object selectedObject in Selection.objects)
            {
                if (selectedObject is Texture2D)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ApplyPixelArtSettings(TextureImporter importer)
        {
            bool changed = false;

            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }

            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                changed = true;
            }

            if (importer.spritePixelsPerUnit != DefaultPixelsPerUnit)
            {
                importer.spritePixelsPerUnit = DefaultPixelsPerUnit;
                changed = true;
            }

            if (importer.filterMode != FilterMode.Point)
            {
                importer.filterMode = FilterMode.Point;
                changed = true;
            }

            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                changed = true;
            }

            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
                changed = true;
            }

            return changed;
        }
    }
}
