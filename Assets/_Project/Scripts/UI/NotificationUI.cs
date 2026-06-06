using System.Collections;
using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float defaultDuration = 2f;

    private Coroutine hideRoutine;

    public bool IsConfigured => root != null && messageText != null;

    public void Configure(GameObject notificationRoot, TMP_Text text)
    {
        root = notificationRoot;
        messageText = text;
        gameObject.SetActive(true);
        HideInstant();
    }

    private void Awake()
    {
        HideInstant();
    }

    public void ShowMessage(string message, float duration = -1f)
    {
        Show(message);

        float finalDuration = duration > 0f ? duration : defaultDuration;
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
        }

        hideRoutine = StartCoroutine(HideAfterDelay(finalDuration));
    }

    public void ShowPersistentMessage(string message)
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }

        Show(message);
    }

    public void HideInstant()
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }

        if (root != null)
        {
            root.SetActive(false);
        }
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (root != null)
        {
            root.SetActive(false);
        }

        hideRoutine = null;
    }

    private void Show(string message)
    {
        gameObject.SetActive(true);

        if (messageText != null)
        {
            messageText.text = message;
        }

        if (root != null)
        {
            root.SetActive(true);
        }
    }

    private void OnValidate()
    {
        defaultDuration = Mathf.Max(0.25f, defaultDuration);
    }
}
