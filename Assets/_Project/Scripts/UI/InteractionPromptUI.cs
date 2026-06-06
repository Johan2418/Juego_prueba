using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text promptText;

    public void Configure(GameObject promptRoot, TMP_Text text)
    {
        root = promptRoot;
        promptText = text;
        HidePrompt();
    }

    private void Awake()
    {
        HidePrompt();
    }

    public void ShowPrompt(string message)
    {
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
        if (root != null)
        {
            root.SetActive(false);
        }
    }
}
