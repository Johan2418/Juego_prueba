using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

namespace MantaMinigames.Fishing
{
    // Arma la UI temporal de pesca para el mapa sin depender de misiones o NPCs.
    [ExecuteAlways]
    public sealed class FishingMapMinigameSceneBuilder : MonoBehaviour
    {
        private const string RuntimeCanvasName = "GameplayRuntimeCanvas";

        [SerializeField] private Canvas canvas;
        [SerializeField] private FishingMinigameLauncher fishingLauncher;
        [SerializeField] private FishingPlayerFishingState fishingState;
        [Header("Visual Assets")]
        [SerializeField] private Sprite panelSprite;
        [SerializeField] private Sprite barFrameSprite;
        [SerializeField] private Sprite indicatorSprite;
        [SerializeField] private Sprite successZoneSprite;
        [SerializeField] private Sprite corvinaIcon;
        [SerializeField] private Sprite albacoraIcon;
        [SerializeField] private Sprite doradoIcon;
        [SerializeField] private Sprite picudoIcon;
        [SerializeField] private Sprite[] idleBobberFrames;
        [SerializeField] private Sprite[] successFishJumpFrames;
        [SerializeField] private Sprite[] failEscapeFrames;
        [SerializeField] private Sprite[] playerFishingIdleFrames;
        [SerializeField] private Sprite[] playerGetFishFrames;
        [SerializeField] private Sprite[] playerFishingFailFrames;

        private const string MinigamePanelName = "FishingMinigameUI";
        private const string PanelBackgroundName = "PanelBackground";
        private const string BarName = "BarFrame";
        private const string SuccessName = "SuccessZone";
        private const string IndicatorName = "MovingIndicator";
        private const string FishIconName = "Image_FishIcon";
        private const string ResultAnimationName = "Image_ResultAnimation";
        private const string PlayerAnimationName = "Image_PlayerAnimation";
        private const string TargetFishName = "Text_FishName";
        private const string AttemptsName = "Text_Attempts";
        private const string MessageName = "Text_Result";
        private const string HitsName = "Text_Hits";
        private const string StateName = "Text_HasFish";
        private const string WorldVisualsName = "FishingWorldVisuals";
        private const int SpriteSheetColumns = 4;
        private const float AnimationPixelsPerUnit = 64f;

        private void Awake()
        {
            BuildIfNeeded();
        }

        private void OnEnable()
        {
            BuildIfNeeded();
        }

        private void Start()
        {
            BuildIfNeeded();
        }

        public void ConfigureSceneReferences(Canvas targetCanvas, FishingMinigameLauncher targetLauncher, FishingPlayerFishingState targetState)
        {
            if (targetCanvas != null)
            {
                canvas = targetCanvas;
            }

            if (targetLauncher != null)
            {
                fishingLauncher = targetLauncher;
            }

            if (targetState != null)
            {
                fishingState = targetState;
            }
        }

        public FishingMinigameController BuildIfNeeded()
        {
            ResolveVisualAssetsInEditor();

            if (canvas == null)
            {
                canvas = GetComponent<Canvas>();
            }

            if (canvas == null)
            {
                canvas = FindGameplayCanvas();
            }

            if (fishingLauncher == null)
            {
                fishingLauncher = UnityEngine.Object.FindFirstObjectByType<FishingMinigameLauncher>();
            }

            if (fishingState == null)
            {
                fishingState = UnityEngine.Object.FindFirstObjectByType<FishingPlayerFishingState>();
            }

            if (fishingState == null)
            {
                GameObject stateObject = new GameObject(nameof(FishingPlayerFishingState));
                fishingState = stateObject.AddComponent<FishingPlayerFishingState>();
            }

            if (canvas == null || fishingLauncher == null || fishingState == null)
            {
                return null;
            }

            RectTransform panel = GetOrCreateRect(canvas.transform, MinigamePanelName, new Vector2(820f, 440f), out bool createdPanel);
            if (createdPanel)
            {
                panel.anchorMin = new Vector2(0.5f, 0.5f);
                panel.anchorMax = new Vector2(0.5f, 0.5f);
                panel.pivot = new Vector2(0.5f, 0.5f);
                panel.anchoredPosition = Vector2.zero;
            }

            panel.SetAsLastSibling();
            Image rootImage = panel.GetComponent<Image>();
            if (rootImage != null)
            {
                rootImage.color = Color.clear;
                rootImage.raycastTarget = false;
            }

            RectTransform panelBackground = GetOrCreateRect(panel, PanelBackgroundName, new Vector2(820f, 440f), out bool createdPanelBackground);
            if (createdPanelBackground)
            {
                panelBackground.anchoredPosition = Vector2.zero;
            }

            panelBackground.SetAsFirstSibling();
            Image panelImage = EnsureImage(panelBackground.gameObject, Color.white);
            ApplySprite(panelImage, panelSprite, Image.Type.Simple, false);
            DisableLegacyUiChildren(panel);

            RectTransform bar = GetOrCreateRect(panel, BarName, new Vector2(660f, 90f), out bool createdBar);
            if (createdBar)
            {
                bar.anchoredPosition = new Vector2(0f, -20f);
            }

            Image barImage = EnsureImage(bar.gameObject, Color.white);
            ApplySprite(barImage, barFrameSprite, Image.Type.Simple, false);

            RectTransform successZone = GetOrCreateRect(bar, SuccessName, new Vector2(140f, 42f), out _);
            Image successImage = EnsureImage(successZone.gameObject, Color.white);
            ApplySprite(successImage, successZoneSprite, Image.Type.Simple, false);

            RectTransform indicator = GetOrCreateRect(bar, IndicatorName, new Vector2(48f, 78f), out _);
            Image indicatorImage = EnsureImage(indicator.gameObject, new Color(1f, 0.92f, 0.32f, 1f));
            ApplySprite(indicatorImage, indicatorSprite, Image.Type.Simple, true);

            RectTransform fishIcon = GetOrCreateRect(panel, FishIconName, new Vector2(110f, 110f), out bool createdFishIcon);
            if (createdFishIcon)
            {
                fishIcon.anchoredPosition = new Vector2(-280f, 75f);
            }

            Image fishIconImage = EnsureImage(fishIcon.gameObject, Color.white);
            fishIconImage.enabled = false;
            fishIconImage.preserveAspect = true;

            DisableChild(panel, ResultAnimationName);
            DisableChild(panel, PlayerAnimationName);

            TextMeshProUGUI targetFishText = GetOrCreateText(panel, TargetFishName, "Pez: Pescado", 30f, new Vector2(0f, 150f), new Vector2(500f, 45f));
            TextMeshProUGUI attemptsText = GetOrCreateText(panel, AttemptsName, "Intentos: 3", 26f, new Vector2(-220f, -92f), new Vector2(240f, 44f));
            TextMeshProUGUI hitsText = GetOrCreateText(panel, HitsName, "Aciertos: 0/1", 26f, new Vector2(220f, -92f), new Vector2(240f, 44f));
            TextMeshProUGUI messageText = GetOrCreateText(panel, MessageName, "Resultado: None", 28f, new Vector2(0f, -128f), new Vector2(560f, 48f));
            TextMeshProUGUI stateText = GetOrCreateText(panel, StateName, string.Empty, 1f, new Vector2(0f, -500f), new Vector2(1f, 1f));
            stateText.gameObject.SetActive(false);
            FishingWorldVisuals worldVisuals = GetOrCreateWorldVisuals();

            if (panel.GetComponent<FishingInputHandler>() == null)
            {
                panel.gameObject.AddComponent<FishingInputHandler>();
            }

            FishingMinigameController controller = panel.GetComponent<FishingMinigameController>();
            if (controller == null)
            {
                controller = panel.gameObject.AddComponent<FishingMinigameController>();
            }

            controller.SetUiReferences(barImage, successImage, indicatorImage, messageText, attemptsText, targetFishText, hitsText);
            controller.SetVisualReferences(
                fishIconImage,
                worldVisuals,
                corvinaIcon,
                albacoraIcon,
                doradoIcon,
                picudoIcon,
                idleBobberFrames,
                successFishJumpFrames,
                failEscapeFrames,
                playerFishingIdleFrames,
                playerGetFishFrames,
                playerFishingFailFrames);
            fishingLauncher.Configure(controller, fishingState, stateText, null);
            return controller;
        }

        [ContextMenu("Build Map Fishing Minigame UI")]
        private void BuildFromContextMenu()
        {
            BuildIfNeeded();
        }

        private static RectTransform GetOrCreateRect(Transform parent, string objectName, Vector2 size, out bool created)
        {
            Transform existing = parent.Find(objectName);
            bool adopted = false;
            if (existing == null)
            {
                existing = FindLooseRectTransform(objectName);
                if (existing != null && existing != parent)
                {
                    existing.SetParent(parent, false);
                    adopted = true;
                }
            }

            bool needsNewObject = existing == null;
            created = needsNewObject || adopted;
            GameObject target = needsNewObject ? new GameObject(objectName, typeof(RectTransform)) : existing.gameObject;

            if (needsNewObject)
            {
                target.transform.SetParent(parent, false);
            }

            RectTransform rect = (RectTransform)target.transform;
            if (created || rect.parent == parent)
            {
                rect.localScale = Vector3.one;
                rect.sizeDelta = size;
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
            }

            return rect;
        }

        private static RectTransform FindLooseRectTransform(string objectName)
        {
            RectTransform[] rects = UnityEngine.Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < rects.Length; i++)
            {
                RectTransform rect = rects[i];
                if (rect == null || rect.name != objectName)
                {
                    continue;
                }

                return rect;
            }

            return null;
        }

        private static Canvas FindGameplayCanvas()
        {
            GameObject namedCanvas = GameObject.Find(RuntimeCanvasName);
            Canvas canvas = namedCanvas != null ? namedCanvas.GetComponent<Canvas>() : null;
            if (canvas != null)
            {
                return canvas;
            }

            Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < canvases.Length; i++)
            {
                Canvas candidate = canvases[i];
                if (candidate != null &&
                    (candidate.renderMode == RenderMode.ScreenSpaceOverlay || candidate.renderMode == RenderMode.ScreenSpaceCamera))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static Image EnsureImage(GameObject target, Color color)
        {
            Image image = target.GetComponent<Image>();
            if (image == null)
            {
                image = target.AddComponent<Image>();
            }

            image.color = color;
            return image;
        }

        private static void ApplySprite(Image image, Sprite sprite, Image.Type imageType, bool preserveAspect)
        {
            if (image == null || sprite == null)
            {
                return;
            }

            image.sprite = sprite;
            image.type = imageType;
            image.preserveAspect = preserveAspect;
        }

        private static TextMeshProUGUI GetOrCreateText(RectTransform parent, string objectName, string text, float size, Vector2 position, Vector2 rectSize)
        {
            RectTransform rect = GetOrCreateRect(parent, objectName, rectSize, out bool created);
            if (created)
            {
                rect.anchoredPosition = position;
            }

            TextMeshProUGUI label = rect.GetComponent<TextMeshProUGUI>();
            if (label == null)
            {
                label = rect.gameObject.AddComponent<TextMeshProUGUI>();
            }

            label.text = text;
            label.fontSize = size;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            label.raycastTarget = false;
            return label;
        }

        private FishingWorldVisuals GetOrCreateWorldVisuals()
        {
            GameObject worldVisualsObject = GameObject.Find(WorldVisualsName);
            if (worldVisualsObject == null)
            {
                worldVisualsObject = new GameObject(WorldVisualsName);
            }

            FishingWorldVisuals worldVisuals = worldVisualsObject.GetComponent<FishingWorldVisuals>();
            if (worldVisuals == null)
            {
                worldVisuals = worldVisualsObject.AddComponent<FishingWorldVisuals>();
            }

            Transform playerAnchor = ResolvePlayerAnchor();
            Transform waterAnchor = fishingLauncher != null ? fishingLauncher.transform : transform;
            worldVisuals.ConfigureAnchors(playerAnchor, waterAnchor);
            worldVisuals.SetFrames(
                idleBobberFrames,
                successFishJumpFrames,
                failEscapeFrames,
                playerFishingIdleFrames,
                playerGetFishFrames,
                playerFishingFailFrames);
            worldVisuals.Hide();
            return worldVisuals;
        }

        private static Transform ResolvePlayerAnchor()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                return playerObject.transform;
            }

            playerObject = GameObject.Find("Player");
            return playerObject != null ? playerObject.transform : null;
        }

        private static void DisableChild(RectTransform parent, string objectName)
        {
            Transform child = parent.Find(objectName);
            if (child != null)
            {
                child.gameObject.SetActive(false);
            }
        }

        private static void DisableLegacyUiChildren(RectTransform panel)
        {
            DisableChild(panel, "PrecisionBar");
            DisableChild(panel, "Text_TargetFish");
            DisableChild(panel, "Text_Message");
            DisableChild(panel, "Text_FishingResult");
            DisableChild(panel, ResultAnimationName);
            DisableChild(panel, PlayerAnimationName);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void ResolveVisualAssetsInEditor()
        {
#if UNITY_EDITOR
            panelSprite = panelSprite != null ? panelSprite : LoadSprite("Assets/Art/Sprites/Minigames/Fishing/UI/Fishing_UI_Panel.png");
            barFrameSprite = barFrameSprite != null ? barFrameSprite : LoadSprite("Assets/Art/Sprites/Minigames/Fishing/UI/Fishing_Bar_Frame.png");
            indicatorSprite = indicatorSprite != null ? indicatorSprite : LoadSprite("Assets/Art/Sprites/Minigames/Fishing/UI/Fishing_Indicator.png");
            successZoneSprite = successZoneSprite != null ? successZoneSprite : LoadSprite("Assets/Art/Sprites/Minigames/Fishing/UI/Fishing_Success_Zone.png");

            corvinaIcon = corvinaIcon != null ? corvinaIcon : LoadSprite("Assets/Art/Sprites/Minigames/Fishing/Fish/Fish_Corvina.png");
            albacoraIcon = albacoraIcon != null ? albacoraIcon : LoadSprite("Assets/Art/Sprites/Minigames/Fishing/Fish/Fish_Albacora.png");
            doradoIcon = doradoIcon != null ? doradoIcon : LoadSprite("Assets/Art/Sprites/Minigames/Fishing/Fish/Fish_Dorado.png");
            picudoIcon = picudoIcon != null ? picudoIcon : LoadSprite("Assets/Art/Sprites/Minigames/Fishing/Fish/Fish_Picudo.png");

            idleBobberFrames = LoadFramesOrKeep("Assets/Art/Sprites/Minigames/Fishing/Animations/Fishing_Idle_Bobber_Sheet.png", idleBobberFrames);
            successFishJumpFrames = LoadFramesOrKeep("Assets/Art/Sprites/Minigames/Fishing/Animations/Fishing_Success_FishJump_Sheet.png", successFishJumpFrames);
            failEscapeFrames = LoadFramesOrKeep("Assets/Art/Sprites/Minigames/Fishing/Animations/Fishing_Fail_Escape_Sheet.png", failEscapeFrames);
            playerFishingIdleFrames = LoadFramesOrKeep("Assets/Art/Sprites/Minigames/Fishing/Animations/Player_Fishing_Idle_Sheet.png", playerFishingIdleFrames);
            playerGetFishFrames = LoadFramesOrKeep("Assets/Art/Sprites/Minigames/Fishing/Animations/Player_Get_Fish_Sheet.png", playerGetFishFrames);
            playerFishingFailFrames = LoadFramesOrKeep("Assets/Art/Sprites/Minigames/Fishing/Animations/Player_Fishing_Fail_Sheet.png", playerFishingFailFrames);
#endif
        }

        private static bool HasFrames(Sprite[] frames)
        {
            return frames != null && frames.Length >= SpriteSheetColumns;
        }

#if UNITY_EDITOR
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

        private static Sprite[] LoadSprites(string assetPath)
        {
            Sprite[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath)
                .OfType<Sprite>()
                .OrderBy(sprite => sprite.name)
                .ToArray();

            if (sprites.Length >= SpriteSheetColumns)
            {
                return sprites;
            }

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            return texture != null ? CreateTemporaryFrames(texture) : sprites;
        }

        private static Sprite[] LoadFramesOrKeep(string assetPath, Sprite[] fallbackFrames)
        {
            Sprite[] loadedFrames = LoadSprites(assetPath);
            return HasFrames(loadedFrames) ? loadedFrames : fallbackFrames;
        }

        private static Sprite[] CreateTemporaryFrames(Texture2D texture)
        {
            float frameWidth = texture.width / (float)SpriteSheetColumns;
            Sprite[] frames = new Sprite[SpriteSheetColumns];

            for (int i = 0; i < SpriteSheetColumns; i++)
            {
                Rect rect = new Rect(i * frameWidth, 0f, frameWidth, texture.height);
                frames[i] = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), AnimationPixelsPerUnit);
                frames[i].name = $"{texture.name}_{i + 1:00}";
            }

            return frames;
        }
#endif
    }
}
