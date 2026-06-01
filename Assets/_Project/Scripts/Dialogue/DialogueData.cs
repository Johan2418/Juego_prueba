using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CozyGameplay/Dialogue Data", fileName = "DialogueData")]
public class DialogueData : ScriptableObject
{
    [SerializeField] private List<DialogueLine> lines = new List<DialogueLine>();

    public IReadOnlyList<DialogueLine> Lines => lines;
    public int LineCount => lines != null ? lines.Count : 0;
}
