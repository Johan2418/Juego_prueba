using System;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject continueIndicator;

    [Header("Input")]
    [SerializeField] private KeyCode advanceKey = KeyCode.E;
    [SerializeField] private float advanceInputDelay = 0.1f;

    public static DialogueManager Instance { get; private set; }
    public bool IsDialogueActive => activeDialogue != null;

    private DialogueData activeDialogue;
    private int activeLineIndex;
    private PlayerInteractor activeInteractor;
    private Action onDialogueFinished;
    private float nextAdvanceTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        HideDialoguePanel();
    }

    private void Update()
    {
        if (!IsDialogueActive)
        {
            return;
        }

        if (Time.unscaledTime < nextAdvanceTime)
        {
            return;
        }

        if (Input.GetKeyDown(advanceKey))
        {
            AdvanceDialogue();
        }
    }

    public void StartDialogue(DialogueData dialogueData, PlayerInteractor interactor = null, Action onFinished = null)
    {
        if (dialogueData == null || dialogueData.LineCount == 0)
        {
            Debug.LogWarning("Cannot start dialogue because DialogueData is empty.");
            return;
        }

        if (IsDialogueActive)
        {
            ForceEndDialogue();
        }

        activeDialogue = dialogueData;
        activeLineIndex = 0;
        activeInteractor = interactor;
        onDialogueFinished = onFinished;
        nextAdvanceTime = Time.unscaledTime + Mathf.Max(0f, advanceInputDelay);

        if (activeInteractor != null)
        {
            activeInteractor.SetInteractionLocked(true);
        }

        ShowDialoguePanel();
        RenderCurrentLine();
    }

    public void AdvanceDialogue()
    {
        if (!IsDialogueActive)
        {
            return;
        }

        activeLineIndex++;
        if (activeLineIndex >= activeDialogue.LineCount)
        {
            EndDialogue();
            return;
        }

        RenderCurrentLine();
    }

    private void RenderCurrentLine()
    {
        DialogueLine line = activeDialogue.Lines[activeLineIndex];

        if (speakerNameText != null)
        {
            speakerNameText.text = string.IsNullOrWhiteSpace(line.SpeakerName) ? "NPC" : line.SpeakerName;
        }

        if (dialogueText != null)
        {
            dialogueText.text = line.Text;
        }
    }

    private void EndDialogue()
    {
        Action callback = onDialogueFinished;
        PlayerInteractor interactorToUnlock = activeInteractor;

        activeDialogue = null;
        activeLineIndex = 0;
        activeInteractor = null;
        onDialogueFinished = null;
        nextAdvanceTime = 0f;
        HideDialoguePanel();

        if (interactorToUnlock != null)
        {
            interactorToUnlock.SetInteractionLocked(false);
        }

        callback?.Invoke();
    }

    private void ForceEndDialogue()
    {
        activeDialogue = null;
        activeLineIndex = 0;
        onDialogueFinished = null;
        nextAdvanceTime = 0f;

        if (activeInteractor != null)
        {
            activeInteractor.SetInteractionLocked(false);
            activeInteractor = null;
        }

        HideDialoguePanel();
    }

    private void ShowDialoguePanel()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        if (continueIndicator != null)
        {
            continueIndicator.SetActive(true);
        }
    }

    private void HideDialoguePanel()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (continueIndicator != null)
        {
            continueIndicator.SetActive(false);
        }
    }

    private void OnValidate()
    {
        advanceInputDelay = Mathf.Max(0f, advanceInputDelay);
    }
}
