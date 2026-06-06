using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MantaMinigames.RapidEat
{
    // Puente aislado entre el puesto de ceviche y RapidEat.
    //
    // Integracion en el mapa:
    // 1. Agrega este componente al objeto del puesto de ceviche.
    // 2. Asigna una instancia inactiva de RapidEatMinigame a rapidEatMinigameRoot.
    //    Si asignas el prefab directamente, activa instantiateFromPrefab.
    // 3. Desde la interaccion del puesto llama StartRapidEatFromCeviche().
    // 4. Conecta OnCevicheMinigameWon/OnCevicheMinigameLost desde Inspector si hace falta.
    [DisallowMultipleComponent]
    [AddComponentMenu("Manta Minigames/Rapid Eat/Ceviche Integration")]
    public sealed class RapidEatCevicheIntegration : MonoBehaviour
    {
        [Header("RapidEat")]
        [SerializeField] private GameObject rapidEatMinigameRoot;
        [SerializeField] private RapidEatMinigameController minigameController;
        [SerializeField] private bool instantiateFromPrefab;
        [SerializeField] private Transform instanceParent;
        [SerializeField] private bool hideOnAwake = true;
        [SerializeField, Min(0.5f)] private float mapUiScale = 1.35f;

        [Header("Completion")]
        [SerializeField] private bool hideAutomaticallyOnComplete = true;
        [SerializeField, Min(0f)] private float autoCloseDelay = 1f;

        public UnityEvent OnCevicheMinigameWon = new UnityEvent();
        public UnityEvent OnCevicheMinigameLost = new UnityEvent();

        private GameObject runtimeInstance;
        private bool isSubscribed;
        private Coroutine pendingClose;

        private void Awake()
        {
            if (!instantiateFromPrefab)
            {
                ResolveController();

                if (hideOnAwake)
                {
                    CloseRapidEat();
                }
            }
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void StartRapidEatFromCeviche()
        {
            CancelPendingClose();

            if (!PrepareMinigame())
            {
                Debug.LogWarning("RapidEatCevicheIntegration: no se pudo iniciar porque falta el prefab, instancia o RapidEatMinigameController.");
                return;
            }

            rapidEatMinigameRoot.SetActive(true);
            ApplyMapUiScale();
            ResolveController();
            Subscribe();

            if (minigameController == null)
            {
                Debug.LogWarning("RapidEatCevicheIntegration: no se encontro RapidEatMinigameController.");
                return;
            }

            minigameController.StartMinigame();
        }

        public void CloseRapidEat()
        {
            CancelPendingClose();

            if (rapidEatMinigameRoot != null)
            {
                rapidEatMinigameRoot.SetActive(false);
            }
        }

        private bool PrepareMinigame()
        {
            if (instantiateFromPrefab && runtimeInstance == null)
            {
                if (rapidEatMinigameRoot == null)
                {
                    return false;
                }

                runtimeInstance = Instantiate(rapidEatMinigameRoot, instanceParent);
                runtimeInstance.name = rapidEatMinigameRoot.name;
                rapidEatMinigameRoot = runtimeInstance;
                minigameController = null;
            }

            ResolveController();
            return rapidEatMinigameRoot != null && minigameController != null;
        }

        private void ResolveController()
        {
            if (minigameController == null && rapidEatMinigameRoot != null)
            {
                minigameController = rapidEatMinigameRoot.GetComponentInChildren<RapidEatMinigameController>(true);
            }

            if (rapidEatMinigameRoot == null && minigameController != null)
            {
                rapidEatMinigameRoot = minigameController.gameObject;
            }
        }

        private void Subscribe()
        {
            if (instantiateFromPrefab && runtimeInstance == null)
            {
                return;
            }

            ResolveController();

            if (isSubscribed || minigameController == null)
            {
                return;
            }

            minigameController.OnMinigameWon += HandleWon;
            minigameController.OnMinigameLost += HandleLost;
            isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!isSubscribed || minigameController == null)
            {
                isSubscribed = false;
                return;
            }

            minigameController.OnMinigameWon -= HandleWon;
            minigameController.OnMinigameLost -= HandleLost;
            isSubscribed = false;
        }

        private void HandleWon()
        {
            Debug.Log("Ceviche minigame completed");
            OnCevicheMinigameWon?.Invoke();
            HideAfterCompletionIfNeeded();
        }

        private void HandleLost()
        {
            Debug.Log("Ceviche minigame failed");
            OnCevicheMinigameLost?.Invoke();
            HideAfterCompletionIfNeeded();
        }

        private void HideAfterCompletionIfNeeded()
        {
            if (!hideAutomaticallyOnComplete)
            {
                return;
            }

            CancelPendingClose();
            pendingClose = StartCoroutine(CloseAfterDelay());
        }

        private IEnumerator CloseAfterDelay()
        {
            yield return new WaitForSecondsRealtime(autoCloseDelay);
            pendingClose = null;
            CloseRapidEat();
        }

        private void CancelPendingClose()
        {
            if (pendingClose == null)
            {
                return;
            }

            StopCoroutine(pendingClose);
            pendingClose = null;
        }

        private void ApplyMapUiScale()
        {
            if (rapidEatMinigameRoot == null)
            {
                return;
            }

            Transform panel = FindChildByName(rapidEatMinigameRoot.transform, "Panel");
            if (panel != null)
            {
                panel.localScale = Vector3.one * Mathf.Max(0.5f, mapUiScale);
            }
        }

        private static Transform FindChildByName(Transform root, string childName)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == childName)
                {
                    return child;
                }
            }

            return null;
        }

        private void OnValidate()
        {
            mapUiScale = Mathf.Max(0.5f, mapUiScale);
            autoCloseDelay = Mathf.Max(0f, autoCloseDelay);
        }
    }
}
