using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "Dialogue/Sequence")]
public class DialogueSequenceSO : ScriptableObject
{
    [Header("Per-step sprites (same length for all lists)")]
    [Tooltip("NPC dialogue sprites per step. Use null entries to show nothing for the NPC at that step.")]
    public List<Sprite> npcSprites = new List<Sprite>();

    [Tooltip("Player dialogue sprites per step. Use null entries to show nothing for the Player at that step.")]
    public List<Sprite> playerSprites = new List<Sprite>();

    [Tooltip("Display duration (seconds) for each step. Must match sprites list length.")]
    public List<float> durations = new List<float>();

    [Tooltip("Fallback duration if a value is <= 0. Only used at runtime.")]
    public float fallbackDuration = 1f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (npcSprites.Count != playerSprites.Count || npcSprites.Count != durations.Count)
        {
            Debug.LogWarning("Lists should have equal lengths. " + $"NPC={npcSprites.Count}, Player={playerSprites.Count}, Durations={durations.Count}. " + $"Sequence: {name}", this);

        }
        if (fallbackDuration <= 0f) fallbackDuration = 0.1f;
    }
#endifS
}