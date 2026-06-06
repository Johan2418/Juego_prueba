using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MantaMinigames.RapidEat
{
    [DisallowMultipleComponent]
    public sealed class RapidEatCevicheZoneInteractable : MonoBehaviour, global::IInteractable
    {
        [SerializeField] private RapidEatCevicheInteractAdapter adapter;

        private static GameObject promptRoot;
        private static TMP_Text promptText;
        private static RapidEatCevicheZoneInteractable activePromptOwner;

        public void Configure(RapidEatCevicheInteractAdapter targetAdapter)
        {
            adapter = targetAdapter;
        }

        public void Interact(global::PlayerInteractor interactor)
        {
            if (adapter == null)
            {
                adapter = GetComponentInParent<RapidEatCevicheInteractAdapter>();
            }

            if (adapter == null)
            {
                Debug.LogWarning("RapidEatCevicheZoneInteractable: falta RapidEatCevicheInteractAdapter en el puesto.");
                return;
            }

            HidePromptOverlay(this);
            adapter.Interact(interactor);
        }

        public string GetInteractionPrompt()
        {
            if (adapter == null)
            {
                adapter = GetComponentInParent<RapidEatCevicheInteractAdapter>();
            }

            return adapter != null ? adapter.GetInteractionPrompt() : "Pulsa E para comer ceviche";
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (ResolveInteractor(other) != null)
            {
                ShowPromptOverlay(GetInteractionPrompt(), this);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (ResolveInteractor(other) != null)
            {
                ShowPromptOverlay(GetInteractionPrompt(), this);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (ResolveInteractor(other) != null)
            {
                HidePromptOverlay(this);
            }
        }

        private void OnDisable()
        {
            HidePromptOverlay(this);
        }

        private static global::PlayerInteractor ResolveInteractor(Collider2D other)
        {
            if (other == null)
            {
                return null;
            }

            global::PlayerInteractor interactor = other.GetComponent<global::PlayerInteractor>();
            if (interactor != null)
            {
                return interactor;
            }

            return other.GetComponentInParent<global::PlayerInteractor>();
        }

        private static void ShowPromptOverlay(string message, RapidEatCevicheZoneInteractable owner)
        {
            EnsurePromptOverlay();
            if (promptText != null)
            {
                promptText.text = string.IsNullOrWhiteSpace(message) ? "Pulsa E para comer ceviche" : message;
            }

            if (promptRoot != null)
            {
                promptRoot.SetActive(true);
            }

            activePromptOwner = owner;
        }

        private static void HidePromptOverlay(RapidEatCevicheZoneInteractable owner)
        {
            if (activePromptOwner != owner)
            {
                return;
            }

            if (promptRoot != null)
            {
                promptRoot.SetActive(false);
            }

            activePromptOwner = null;
        }

        private static void EnsurePromptOverlay()
        {
            if (promptRoot != null && promptText != null)
            {
                return;
            }

            Canvas canvas = FindScreenSpaceCanvas();
            if (canvas == null)
            {
                GameObject canvasObject = new GameObject(
                    "RapidEatPromptCanvas",
                    typeof(Canvas),
                    typeof(CanvasScaler),
                    typeof(GraphicRaycaster));
                canvas = canvasObject.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 1200;

                CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1280f, 720f);
                scaler.matchWidthOrHeight = 0.5f;
            }

            GameObject panel = new GameObject("RapidEatPromptUI", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(canvas.transform, false);
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(520f, 58f);
            panelRect.anchoredPosition = new Vector2(0f, -270f);

            Image panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.78f);
            panelImage.raycastTarget = false;

            GameObject textObject = new GameObject("PromptText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(panel.transform, false);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(16f, 8f);
            textRect.offsetMax = new Vector2(-16f, -8f);

            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            text.text = "Pulsa E para comer ceviche";
            text.fontSize = 26f;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.raycastTarget = false;

            promptRoot = panel;
            promptText = text;
            promptRoot.SetActive(false);
        }

        private static Canvas FindScreenSpaceCanvas()
        {
            Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < canvases.Length; i++)
            {
                Canvas canvas = canvases[i];
                if (canvas != null &&
                    (canvas.renderMode == RenderMode.ScreenSpaceOverlay || canvas.renderMode == RenderMode.ScreenSpaceCamera))
                {
                    return canvas;
                }
            }

            return null;
        }
    }
}
