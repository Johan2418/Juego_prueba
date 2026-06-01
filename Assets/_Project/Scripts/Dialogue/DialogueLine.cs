using System;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    [SerializeField] private string speakerName;
    [TextArea(2, 6)]
    [SerializeField] private string text;

    public string SpeakerName => speakerName;
    public string Text => text;
}
