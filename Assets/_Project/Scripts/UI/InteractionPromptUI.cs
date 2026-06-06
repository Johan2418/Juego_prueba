using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text promptText;

    public bool IsConfigured => root != null && promptText != null;

    public void Configure(GameObject promptRoot, TMP_Text text)
    {
        root = promptRoot;
        promptText = text;
        HidePrompt();
    }

    private void Awake()
    {
        EnsureReferences();
        HidePrompt();
    }

    public void ShowPrompt(string message)
    {
        EnsureReferences();

        if (promptText != null)
        {
            promptText.text = message;
        }

        if (root != null)
        {
            root.SetActive(true);
        }
    }

    public void HidePrompt()
    {
        EnsureReferences();

        if (root != null)
        {
            root.SetActive(false);
        }
    }

    private void EnsureReferences()
    {
        if (root == null)
        {
            root = gameObject;
        }

        if (promptText == null)
        {
            promptText = GetComponent<TMP_Text>();
        }

        if (promptText == null)
        {
            promptText = GetComponentInChildren<TMP_Text>(true);
        }
    }
}
