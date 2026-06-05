using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace JuegoPrueba.Editor.Fishing
{
    /// <summary>
    /// Configura los sprites pixel art del minijuego de pesca y genera clips/prefabs auxiliares.
    /// </summary>
    public static class FishingVisualAssetSetup
    {
        private const int GameplayPixelsPerUnit = 32;
        private const int UiPixelsPerUnit = 100;
        private const int SpriteSheetColumns = 4;
        private const float AnimationFrameRate = 8f;
        private const string SpriteRoot = "Assets/Art/Sprites/Minigames/Fishing";
        private const string AnimationFolder = "Assets/Animations/Minigames/Fishing";
        private const string EnvironmentPrefabFolder = "Assets/Prefabs/Minigames/Fishing/Environment";
        private const string ControllerPath = AnimationFolder + "/Fishing_ResultAnimations.controller";

        private static readonly string[] SpriteSheetPaths =
        {
            SpriteRoot + "/Animations/Fishing_Fail_Escape_Sheet.png",
            SpriteRoot + "/Animations/Fishing_Idle_Bobber_Sheet.png",
            SpriteRoot + "/Animations/Fishing_Success_FishJump_Sheet.png",
            SpriteRoot + "/Animations/Player_Fishing_Fail_Sheet.png",
            SpriteRoot + "/Animations/Player_Fishing_Idle_Sheet.png",
            SpriteRoot + "/Animations/Player_Get_Fish_Sheet.png"
        };

        private static readonly Dictionary<string, int> EnvironmentSortingOrderByPath = new Dictionary<string, int>
        {
            { SpriteRoot + "/Environment/Water_waves.png", 1 },
            { SpriteRoot + "/Environment/Dock_Wood.png", 1 },
            { SpriteRoot + "/Environment/Small_Boat.png", 1 },
            { SpriteRoot + "/Environment/Object_Shadow.png", -1 },
            { SpriteRoot + "/Environment/Fishing_Bucket.png", 2 },
            { SpriteRoot + "/Environment/Fishing_Net.png", 2 },
            { SpriteRoot + "/Environment/Fishing_Rod_Prop.png", 2 },
            { SpriteRoot + "/Environment/Fishing_Sign.png", 3 }
        };

        private static readonly Dictionary<string, string> ClipPathsBySheetPath = new Dictionary<string, string>
        {
            { SpriteRoot + "/Animations/Fishing_Fail_Escape_Sheet.png", AnimationFolder + "/Fishing_Fail_Escape.anim" },
            { SpriteRoot + "/Animations/Fishing_Idle_Bobber_Sheet.png", AnimationFolder + "/Fishing_Idle_Bobber.anim" },
            { SpriteRoot + "/Animations/Fishing_Success_FishJump_Sheet.png", AnimationFolder + "/Fishing_Success_FishJump.anim" },
            { SpriteRoot + "/Animations/Player_Fishing_Fail_Sheet.png", AnimationFolder + "/Player_Fishing_Fail.anim" },
            { SpriteRoot + "/Animations/Player_Fishing_Idle_Sheet.png", AnimationFolder + "/Player_Fishing_Idle.anim" },
            { SpriteRoot + "/Animations/Player_Get_Fish_Sheet.png", AnimationFolder + "/Player_Get_Fish.anim" }
        };

        [MenuItem("Tools/Fishing/Setup Fishing Visual Assets")]
        public static void SetupFishingVisualAssets()
        {
            EnsureFolder(AnimationFolder);
            EnsureFolder(EnvironmentPrefabFolder);

            ConfigureAllFishingTextures();
            List<AnimationClip> clips = CreateAnimationClips();
            CreateAnimatorController(clips);
            CreateEnvironmentPrefabs();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Fishing] Visual assets configured. Sprites imported, clips generated, and environment prefabs prepared.");
        }

        [MenuItem("Tools/Fishing/Decorate Selected Fishing Dock")]
        public static void DecorateSelectedFishingDock()
        {
            GameObject selectedDock = Selection.activeGameObject;
            if (selectedDock == null)
            {
                Debug.LogWarning("[Fishing] Selecciona el objeto del muelle antes de decorar.");
                return;
            }

            ConfigureAllFishingTextures();
            CreateDockDecoration(selectedDock.transform);
            EditorSceneManager.MarkSceneDirty(selectedDock.scene);
            Debug.Log($"[Fishing] Decoracion aplicada bajo {selectedDock.name}. Revisa posiciones antes de guardar la escena.");
        }

        [MenuItem("Tools/Fishing/Decorate Selected Fishing Dock", true)]
        private static bool CanDecorateSelectedFishingDock()
        {
            return Selection.activeGameObject != null;
        }

        private static void ConfigureAllFishingTextures()
        {
            string[] texturePaths = AssetDatabase.FindAssets("t:Texture2D", new[] { SpriteRoot })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (string texturePath in texturePaths)
            {
                bool isSpriteSheet = SpriteSheetPaths.Contains(texturePath);
                ConfigureTexture(texturePath, isSpriteSheet);
            }
        }

        private static void ConfigureTexture(string assetPath, bool isSpriteSheet)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = GetPixelsPerUnit(assetPath);
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.spriteImportMode = isSpriteSheet ? SpriteImportMode.Multiple : SpriteImportMode.Single;

            if (isSpriteSheet)
            {
                ConfigureFourFrameSpriteSheet(importer, assetPath);
            }

            importer.SaveAndReimport();
        }

        private static void ConfigureFourFrameSpriteSheet(TextureImporter importer, string assetPath)
        {
            importer.GetSourceTextureWidthAndHeight(out int width, out int height);
            if (width <= 0 || height <= 0)
            {
                Debug.LogWarning($"[Fishing] No se pudo leer el tamano de {assetPath} para cortar spritesheet.");
                return;
            }

            float frameWidth = width / (float)SpriteSheetColumns;
            string baseName = Path.GetFileNameWithoutExtension(assetPath).Replace("_Sheet", string.Empty);
            SpriteMetaData[] spritesheet = new SpriteMetaData[SpriteSheetColumns];

            for (int i = 0; i < SpriteSheetColumns; i++)
            {
                spritesheet[i] = new SpriteMetaData
                {
                    name = $"{baseName}_{i + 1:00}",
                    rect = new Rect(i * frameWidth, 0f, frameWidth, height),
                    alignment = (int)SpriteAlignment.Center,
                    pivot = new Vector2(0.5f, 0.5f)
                };
            }

#pragma warning disable 0618
            importer.spritesheet = spritesheet;
#pragma warning restore 0618
        }

        private static List<AnimationClip> CreateAnimationClips()
        {
            List<AnimationClip> clips = new List<AnimationClip>();

            foreach (KeyValuePair<string, string> pair in ClipPathsBySheetPath)
            {
                Sprite[] frames = LoadSpriteSheetFrames(pair.Key);
                if (frames.Length < SpriteSheetColumns)
                {
                    Debug.LogWarning($"[Fishing] {pair.Key} debe tener {SpriteSheetColumns} frames cortados y solo tiene {frames.Length}.");
                    continue;
                }

                bool loop = pair.Value.Contains("Idle");
                AnimationClip clip = CreateOrUpdateSpriteRendererClip(pair.Value, frames, loop);
                clips.Add(clip);
            }

            return clips;
        }

        private static Sprite[] LoadSpriteSheetFrames(string spriteSheetPath)
        {
            return AssetDatabase.LoadAllAssetRepresentationsAtPath(spriteSheetPath)
                .OfType<Sprite>()
                .OrderBy(sprite => sprite.name)
                .ToArray();
        }

        private static AnimationClip CreateOrUpdateSpriteRendererClip(string clipPath, Sprite[] frames, bool loop)
        {
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
            if (clip == null)
            {
                clip = new AnimationClip();
                AssetDatabase.CreateAsset(clip, clipPath);
            }

            clip.frameRate = AnimationFrameRate;

            EditorCurveBinding spriteBinding = new EditorCurveBinding
            {
                type = typeof(SpriteRenderer),
                path = string.Empty,
                propertyName = "m_Sprite"
            };

            ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[frames.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                keyframes[i] = new ObjectReferenceKeyframe
                {
                    time = i / AnimationFrameRate,
                    value = frames[i]
                };
            }

            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes);
            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = loop;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            EditorUtility.SetDirty(clip);
            return clip;
        }

        private static void CreateAnimatorController(List<AnimationClip> clips)
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            }

            foreach (AnimationClip clip in clips)
            {
                EnsureState(controller, Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(clip)), clip);
            }

            EditorUtility.SetDirty(controller);
        }

        private static void EnsureState(AnimatorController controller, string stateName, Motion motion)
        {
            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;

            foreach (ChildAnimatorState childState in stateMachine.states)
            {
                if (childState.state.name == stateName)
                {
                    childState.state.motion = motion;
                    return;
                }
            }

            AnimatorState state = stateMachine.AddState(stateName);
            state.motion = motion;

            if (stateMachine.defaultState == null)
            {
                stateMachine.defaultState = state;
            }
        }

        private static int GetPixelsPerUnit(string assetPath)
        {
            return assetPath.Contains("/UI/") ? UiPixelsPerUnit : GameplayPixelsPerUnit;
        }

        private static void CreateEnvironmentPrefabs()
        {
            string environmentFolder = SpriteRoot + "/Environment";
            string[] environmentSpritePaths = AssetDatabase.FindAssets("t:Sprite", new[] { environmentFolder })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (string spritePath in environmentSpritePaths)
            {
                Sprite sprite = LoadSprite(spritePath);
                if (sprite == null)
                {
                    continue;
                }

                string prefabName = Path.GetFileNameWithoutExtension(spritePath);
                GameObject prefabRoot = new GameObject(prefabName);
                SpriteRenderer renderer = prefabRoot.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = EnvironmentSortingOrderByPath.TryGetValue(spritePath, out int sortingOrder) ? sortingOrder : 1;

                PrefabUtility.SaveAsPrefabAsset(prefabRoot, $"{EnvironmentPrefabFolder}/{prefabName}.prefab");
                UnityObject.DestroyImmediate(prefabRoot);
            }
        }

        private static void CreateDockDecoration(Transform dockRoot)
        {
            Transform parent = dockRoot.parent != null ? dockRoot.parent : dockRoot;
            Transform existingRoot = parent.Find("FishingDockDecorations");
            GameObject decorationRoot = existingRoot != null ? existingRoot.gameObject : new GameObject("FishingDockDecorations");
            if (existingRoot == null)
            {
                Undo.RegisterCreatedObjectUndo(decorationRoot, "Create FishingDockDecorations");
            }

            if (existingRoot == null)
            {
                decorationRoot.transform.SetParent(parent, false);
                decorationRoot.transform.position = dockRoot.position;
                decorationRoot.transform.localScale = Vector3.one;
            }

            CreateOrUpdateDecoration(decorationRoot.transform, "Dock_Wood", SpriteRoot + "/Environment/Dock_Wood.png", Vector3.zero, new Vector3(1.35f, 1.35f, 1f), 1);
            CreateOrUpdateDecoration(decorationRoot.transform, "Water_waves", SpriteRoot + "/Environment/Water_waves.png", new Vector3(-0.2f, -0.55f, 0f), new Vector3(1.15f, 1.15f, 1f), 1);
            CreateOrUpdateDecoration(decorationRoot.transform, "Fishing_Bucket", SpriteRoot + "/Environment/Fishing_Bucket.png", new Vector3(-0.48f, -0.24f, 0f), new Vector3(0.46f, 0.46f, 1f), 2);
            CreateOrUpdateDecoration(decorationRoot.transform, "Fishing_Net", SpriteRoot + "/Environment/Fishing_Net.png", new Vector3(0.42f, 0.08f, 0f), new Vector3(0.46f, 0.46f, 1f), 2);
            CreateOrUpdateDecoration(decorationRoot.transform, "Fishing_Rod_Prop", SpriteRoot + "/Environment/Fishing_Rod_Prop.png", new Vector3(0.28f, 0.34f, 0f), new Vector3(0.5f, 0.5f, 1f), 2);
            CreateOrUpdateDecoration(decorationRoot.transform, "Fishing_Sign", SpriteRoot + "/Environment/Fishing_Sign.png", new Vector3(-0.52f, 0.32f, 0f), new Vector3(0.52f, 0.52f, 1f), 3);
            CreateOrUpdateDecoration(decorationRoot.transform, "Small_Boat", SpriteRoot + "/Environment/Small_Boat.png", new Vector3(0.48f, -0.36f, 0f), new Vector3(0.55f, 0.55f, 1f), 1);
            CreateOrUpdateDecoration(decorationRoot.transform, "Object_Shadow", SpriteRoot + "/Environment/Object_Shadow.png", new Vector3(-0.46f, -0.32f, 0f), new Vector3(0.42f, 0.42f, 1f), -1);
        }

        private static void CreateOrUpdateDecoration(Transform parent, string objectName, string spritePath, Vector3 localPosition, Vector3 localScale, int sortingOrder)
        {
            Sprite sprite = LoadSprite(spritePath);
            if (sprite == null)
            {
                Debug.LogWarning($"[Fishing] No se encontro sprite para decoracion: {spritePath}");
                return;
            }

            Transform existing = parent.Find(objectName);
            GameObject target = existing != null ? existing.gameObject : new GameObject(objectName);
            if (existing == null)
            {
                Undo.RegisterCreatedObjectUndo(target, $"Create {objectName}");
            }

            if (existing == null)
            {
                target.transform.SetParent(parent, false);
                target.transform.localPosition = localPosition;
                target.transform.localScale = localScale;
            }

            SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = target.AddComponent<SpriteRenderer>();
            }

            if (renderer.sprite == null)
            {
                renderer.sprite = sprite;
            }

            if (existing == null)
            {
                renderer.sortingOrder = sortingOrder;
            }
        }

        private static Sprite LoadSprite(string assetPath)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite != null)
            {
                return sprite;
            }

            return AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath)
                .OfType<Sprite>()
                .FirstOrDefault();
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
    }
}
