using UnityEngine;

namespace MantaMinigames.Fishing
{
    // Reproduce las animaciones visuales de pesca en el mundo, fuera del Canvas.
    public sealed class FishingWorldVisuals : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer playerRenderer;
        [SerializeField] private SpriteRenderer waterRenderer;
        [SerializeField] private Transform playerAnchor;
        [SerializeField] private Transform waterAnchor;
        [SerializeField] private Vector3 playerOffset = new Vector3(0f, 0.15f, 0f);
        [SerializeField] private Vector3 upperWaterOffset = new Vector3(-0.35f, 0.95f, 0f);
        [SerializeField] private Vector3 lowerWaterOffset = new Vector3(-0.35f, -0.95f, 0f);
        [SerializeField] private Vector3 playerScale = Vector3.one;
        [SerializeField] private Vector3 waterScale = new Vector3(1.1f, 1.1f, 1f);
        [SerializeField, Min(0.1f)] private float playerSizeMultiplier = 1.7f;
        [SerializeField, Min(1f)] private float framesPerSecond = 8f;
        [SerializeField, Min(0.1f)] private float missAnimationSeconds = 0.45f;
        [SerializeField, Min(0.1f)] private float minimumCompletionVisibleSeconds = 0.65f;

        private Sprite[] idleBobberFrames;
        private Sprite[] successFishJumpFrames;
        private Sprite[] failEscapeFrames;
        private Sprite[] playerFishingIdleFrames;
        private Sprite[] playerGetFishFrames;
        private Sprite[] playerFishingFailFrames;
        private Sprite[] activePlayerFrames;
        private Sprite[] activeWaterFrames;
        private float playerAnimationTime;
        private float waterAnimationTime;
        private bool loopPlayerAnimation;
        private bool loopWaterAnimation;
        private float returnToIdleTime;
        private float autoHideTime;
        private float completionStartedTime;
        private bool hideWhenCompletionFramesFinish;
        private SpriteRenderer[] playerAnchorRenderers;
        private bool[] playerAnchorRendererStates;
        private float cachedPlayerHeight;
        private bool hasReplacedPlayerRenderer;

        private void Awake()
        {
            EnsureRenderers();
            Hide();
        }

        private void LateUpdate()
        {
            if (returnToIdleTime > 0f && Time.unscaledTime >= returnToIdleTime)
            {
                returnToIdleTime = 0f;
                PlayIdle();
            }

            if (autoHideTime > 0f && Time.unscaledTime >= autoHideTime)
            {
                Hide();
                return;
            }

            FollowAnchors();
            UpdateAnimation(playerRenderer, activePlayerFrames, loopPlayerAnimation, ref playerAnimationTime);
            UpdateAnimation(waterRenderer, activeWaterFrames, loopWaterAnimation, ref waterAnimationTime);

            if (hideWhenCompletionFramesFinish && CompletionFramesFinished())
            {
                Hide();
            }
        }

        public void ConfigureAnchors(Transform player, Transform water)
        {
            if (player != null)
            {
                playerAnchor = player;
                CachePlayerAnchorRenderers();
            }

            if (water != null)
            {
                waterAnchor = water;
            }

            FollowAnchors();
        }

        public void SetFrames(
            Sprite[] idleBobber,
            Sprite[] successFishJump,
            Sprite[] failEscape,
            Sprite[] playerIdle,
            Sprite[] playerGetFish,
            Sprite[] playerFail)
        {
            idleBobberFrames = idleBobber;
            successFishJumpFrames = successFishJump;
            failEscapeFrames = failEscape;
            playerFishingIdleFrames = playerIdle;
            playerGetFishFrames = playerGetFish;
            playerFishingFailFrames = playerFail;
        }

        public void PlayIdle()
        {
            gameObject.SetActive(true);
            returnToIdleTime = 0f;
            autoHideTime = 0f;
            hideWhenCompletionFramesFinish = false;
            ReplacePlayerRenderer();
            SetPlayerAnimation(playerFishingIdleFrames, true);
            SetWaterAnimation(idleBobberFrames, true);
        }

        public void PlayCompletion(FishingResult result)
        {
            gameObject.SetActive(true);
            returnToIdleTime = 0f;
            autoHideTime = 0f;
            hideWhenCompletionFramesFinish = false;
            ReplacePlayerRenderer();

            if (result == FishingResult.Success)
            {
                SetPlayerAnimation(playerGetFishFrames, false);
                SetWaterAnimation(successFishJumpFrames, false);
                BeginCompletionAutoHide(playerGetFishFrames, successFishJumpFrames);
                return;
            }

            if (result == FishingResult.Failed)
            {
                SetPlayerAnimation(playerFishingFailFrames, false);
                SetWaterAnimation(failEscapeFrames, false);
                BeginCompletionAutoHide(playerFishingFailFrames, failEscapeFrames);
                return;
            }

            Hide();
        }

        public void PlayMiss()
        {
            gameObject.SetActive(true);
            autoHideTime = 0f;
            hideWhenCompletionFramesFinish = false;
            ReplacePlayerRenderer();
            SetPlayerAnimation(playerFishingFailFrames, false);
            SetWaterAnimation(failEscapeFrames, false);
            returnToIdleTime = Time.unscaledTime + missAnimationSeconds;
        }

        public void Hide()
        {
            returnToIdleTime = 0f;
            autoHideTime = 0f;
            hideWhenCompletionFramesFinish = false;

            if (playerRenderer != null)
            {
                playerRenderer.enabled = false;
                playerRenderer.sprite = null;
            }

            if (waterRenderer != null)
            {
                waterRenderer.enabled = false;
                waterRenderer.sprite = null;
            }

            RestorePlayerRenderer();
        }

        private void SetPlayerAnimation(Sprite[] frames, bool loop)
        {
            activePlayerFrames = HasUsableFrames(frames) ? frames : null;
            loopPlayerAnimation = loop;
            playerAnimationTime = 0f;
            ApplyFrame(playerRenderer, activePlayerFrames, 0);
        }

        private void SetWaterAnimation(Sprite[] frames, bool loop)
        {
            activeWaterFrames = HasUsableFrames(frames) ? frames : null;
            loopWaterAnimation = loop;
            waterAnimationTime = 0f;
            ApplyFrame(waterRenderer, activeWaterFrames, 0);
        }

        private void UpdateAnimation(SpriteRenderer targetRenderer, Sprite[] frames, bool loop, ref float elapsedTime)
        {
            if (targetRenderer == null || frames == null || frames.Length == 0)
            {
                return;
            }

            elapsedTime += Time.unscaledDeltaTime;
            int frameIndex = Mathf.FloorToInt(elapsedTime * framesPerSecond);
            frameIndex = loop ? frameIndex % frames.Length : Mathf.Min(frameIndex, frames.Length - 1);
            ApplyFrame(targetRenderer, frames, frameIndex);
        }

        private void FollowAnchors()
        {
            EnsureRenderers();
            CachePlayerAnchorRenderers();

            if (playerRenderer != null && playerAnchor != null)
            {
                Transform playerTransform = playerRenderer.transform;
                playerTransform.position = playerAnchor.position + playerOffset;
                playerTransform.localScale = ResolvePlayerScale();
            }

            if (waterRenderer != null && waterAnchor != null)
            {
                Transform waterTransform = waterRenderer.transform;
                waterTransform.position = waterAnchor.position + ResolveWaterOffset();
                waterTransform.localScale = waterScale;
            }
        }

        private Vector3 ResolveWaterOffset()
        {
            if (playerAnchor == null || waterAnchor == null)
            {
                return lowerWaterOffset;
            }

            return playerAnchor.position.y >= waterAnchor.position.y ? upperWaterOffset : lowerWaterOffset;
        }

        private void EnsureRenderers()
        {
            playerRenderer = playerRenderer != null ? playerRenderer : GetRenderer("Fishing_PlayerAnimationWorld", 4);
            waterRenderer = waterRenderer != null ? waterRenderer : GetRenderer("Fishing_WaterAnimationWorld", 3);
            SyncPlayerSortingOrder();
        }

        private SpriteRenderer GetRenderer(string objectName, int sortingOrder)
        {
            Transform existing = transform.Find(objectName);
            GameObject target = existing != null ? existing.gameObject : new GameObject(objectName);
            target.transform.SetParent(transform, false);

            SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = target.AddComponent<SpriteRenderer>();
            }

            spriteRenderer.sortingOrder = sortingOrder;
            spriteRenderer.color = Color.white;
            return spriteRenderer;
        }

        private Vector3 ResolvePlayerScale()
        {
            if (playerRenderer == null || playerRenderer.sprite == null)
            {
                return playerScale;
            }

            float targetHeight = cachedPlayerHeight;
            float sourceHeight = playerRenderer.sprite.bounds.size.y;
            if (targetHeight <= 0f || sourceHeight <= 0f)
            {
                return playerScale;
            }

            float uniformScale = Mathf.Clamp((targetHeight / sourceHeight) * playerSizeMultiplier, 0.35f, 2.5f);
            return new Vector3(uniformScale, uniformScale, 1f);
        }

        private void SyncPlayerSortingOrder()
        {
            if (playerRenderer == null || playerAnchor == null)
            {
                return;
            }

            SpriteRenderer anchorRenderer = playerAnchor.GetComponentInChildren<SpriteRenderer>();
            if (anchorRenderer != null)
            {
                playerRenderer.sortingLayerID = anchorRenderer.sortingLayerID;
                playerRenderer.sortingOrder = anchorRenderer.sortingOrder + 1;
            }
        }

        private void CachePlayerAnchorRenderers()
        {
            if (playerAnchor == null)
            {
                playerAnchorRenderers = null;
                playerAnchorRendererStates = null;
                cachedPlayerHeight = 0f;
                return;
            }

            SpriteRenderer[] renderers = playerAnchor.GetComponentsInChildren<SpriteRenderer>(true);
            if (hasReplacedPlayerRenderer)
            {
                return;
            }

            playerAnchorRenderers = renderers;
            playerAnchorRendererStates = new bool[playerAnchorRenderers.Length];
            cachedPlayerHeight = 0f;

            for (int i = 0; i < playerAnchorRenderers.Length; i++)
            {
                SpriteRenderer renderer = playerAnchorRenderers[i];
                playerAnchorRendererStates[i] = renderer != null && renderer.enabled;

                if (renderer != null && renderer.sprite != null)
                {
                    cachedPlayerHeight = Mathf.Max(cachedPlayerHeight, renderer.bounds.size.y);
                }
            }
        }

        private void ReplacePlayerRenderer()
        {
            CachePlayerAnchorRenderers();

            if (playerAnchorRenderers == null)
            {
                return;
            }

            for (int i = 0; i < playerAnchorRenderers.Length; i++)
            {
                if (playerAnchorRenderers[i] != null)
                {
                    playerAnchorRenderers[i].enabled = false;
                }
            }

            hasReplacedPlayerRenderer = true;
        }

        private void RestorePlayerRenderer()
        {
            if (playerAnchorRenderers == null || playerAnchorRendererStates == null)
            {
                return;
            }

            for (int i = 0; i < playerAnchorRenderers.Length; i++)
            {
                if (playerAnchorRenderers[i] != null)
                {
                    playerAnchorRenderers[i].enabled = playerAnchorRendererStates[i];
                }
            }

            hasReplacedPlayerRenderer = false;
        }

        private void BeginCompletionAutoHide(Sprite[] playerFrames, Sprite[] waterFrames)
        {
            float playerDuration = GetAnimationDuration(playerFrames);
            float waterDuration = GetAnimationDuration(waterFrames);
            completionStartedTime = Time.unscaledTime;
            hideWhenCompletionFramesFinish = true;
            autoHideTime = Time.unscaledTime + Mathf.Max(minimumCompletionVisibleSeconds, playerDuration, waterDuration) + 0.1f;
        }

        private bool CompletionFramesFinished()
        {
            if (Time.unscaledTime < completionStartedTime + minimumCompletionVisibleSeconds)
            {
                return false;
            }

            return AnimationFinished(activePlayerFrames, playerAnimationTime) &&
                AnimationFinished(activeWaterFrames, waterAnimationTime);
        }

        private bool AnimationFinished(Sprite[] frames, float elapsedTime)
        {
            if (!HasUsableFrames(frames))
            {
                return true;
            }

            int currentFrame = Mathf.FloorToInt(elapsedTime * framesPerSecond);
            return currentFrame >= frames.Length - 1;
        }

        private float GetAnimationDuration(Sprite[] frames)
        {
            if (!HasUsableFrames(frames))
            {
                return 0f;
            }

            return frames.Length / Mathf.Max(1f, framesPerSecond);
        }

        private static void ApplyFrame(SpriteRenderer targetRenderer, Sprite[] frames, int frameIndex)
        {
            if (targetRenderer == null)
            {
                return;
            }

            if (frames == null || frames.Length == 0)
            {
                targetRenderer.enabled = false;
                return;
            }

            targetRenderer.sprite = frames[Mathf.Clamp(frameIndex, 0, frames.Length - 1)];
            targetRenderer.enabled = targetRenderer.sprite != null;
        }

        private static bool HasUsableFrames(Sprite[] frames)
        {
            return frames != null && frames.Length >= 2;
        }
    }
}
