using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class SecretBossGameManager : MonoBehaviour
{
    [Header("Fight References")]
    [SerializeField] private SecretBossHealth bossHealth;
    [SerializeField] private Health playerHealth;
    [SerializeField] private SecretBossController bossController;
    [SerializeField] private SecretBossPlayerAttack playerAttack;
    [SerializeField] private GameObject rewardObject;

    [Header("Fight Settings")]
    [SerializeField] private bool startFightOnStart = true;
    [SerializeField] private KeyCode restartKey = KeyCode.R;

    [Header("Fight Messages")]
    [SerializeField] private string initialMessage = "Derrota al Gólem de la Silla Manteña";
    [SerializeField] private string victoryMessage = "Has vencido al guardián ancestral";
    [SerializeField] private string defeatMessage = "Has sido derrotado. Presiona R para reiniciar";
    [SerializeField] private Text fightMessageText;

    private bool fightIsActive;

    public bool FightIsActive => fightIsActive;

    private void Awake()
    {
        if (rewardObject != null)
        {
            rewardObject.SetActive(false);
        }

        if (fightMessageText == null)
        {
            fightMessageText = CreateRuntimeFightMessage();
        }
    }

    private void OnEnable()
    {
        if (bossHealth != null)
        {
            bossHealth.Defeated += HandlePlayerVictory;
        }

        if (playerHealth != null)
        {
            playerHealth.Died += HandlePlayerDefeat;
        }
    }

    private void Start()
    {
        if (startFightOnStart)
        {
            StartFight();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(restartKey))
        {
            RestartScene();
        }
    }

    private void OnDisable()
    {
        if (bossHealth != null)
        {
            bossHealth.Defeated -= HandlePlayerVictory;
        }

        if (playerHealth != null)
        {
            playerHealth.Died -= HandlePlayerDefeat;
        }
    }

    public void StartFight()
    {
        if (fightIsActive)
        {
            return;
        }

        fightIsActive = true;
        SetCombatEnabled(true);
        ShowMessage(initialMessage);
        Debug.Log("[SecretBossGameManager] La pelea contra el jefe secreto ha comenzado.");
    }

    public void PlayerLost()
    {
        HandlePlayerDefeat();
    }

    private void HandlePlayerVictory()
    {
        if (!fightIsActive)
        {
            return;
        }

        fightIsActive = false;
        SetCombatEnabled(false);
        ActivateReward();
        ShowMessage(victoryMessage);
        Debug.Log("[SecretBossGameManager] Victoria del jugador.");
    }

    private void HandlePlayerDefeat()
    {
        if (!fightIsActive)
        {
            return;
        }

        fightIsActive = false;
        SetCombatEnabled(false);
        ShowMessage(defeatMessage);
        Debug.Log("[SecretBossGameManager] El jugador ha perdido.");
    }

    private void RestartScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    private void SetCombatEnabled(bool enabled)
    {
        if (bossController != null)
        {
            bossController.enabled = enabled;
        }

        if (playerAttack != null)
        {
            playerAttack.enabled = enabled;
        }
    }

    private void ShowMessage(string message)
    {
        if (fightMessageText != null)
        {
            fightMessageText.text = message;
        }
    }

    private void ActivateReward()
    {
        if (rewardObject == null)
        {
            Debug.LogWarning("[SecretBossGameManager] No se asigno la recompensa del jefe secreto.");
            return;
        }

        rewardObject.SetActive(true);
        Debug.Log("Recompensa activada: Piedra de Umiña");
    }

    private static Text CreateRuntimeFightMessage()
    {
        GameObject canvasObject = new GameObject(
            "SecretBossCombatUI",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);

        GameObject textObject = new GameObject(
            "FightMessage",
            typeof(RectTransform),
            typeof(Text),
            typeof(Outline));
        textObject.transform.SetParent(canvasObject.transform, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -30f);
        rect.sizeDelta = new Vector2(900f, 90f);

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 34;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;

        Outline outline = textObject.GetComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2f, -2f);
        return text;
    }
}
