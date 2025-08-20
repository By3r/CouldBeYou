using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "Dialogue/Sequence")]
public class DialogueSequenceSO : ScriptableObject
{
    #region Variables
    public List<Sprite> npcSprites = new List<Sprite>();
    public List<Sprite> playerSprites = new List<Sprite>();
    public List<float> durations = new List<float>();

    public float fallbackDuration = 1f;
    #endregion

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (fallbackDuration <= 0f) fallbackDuration = 0.1f;
    }
#endif
}