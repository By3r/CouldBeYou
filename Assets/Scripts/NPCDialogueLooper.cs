using System.Collections;
using UnityEngine;

/// <summary>
/// Plays a two-lane, sprite-based conversation between two NPCs,
/// using DialogueSequenceSO: npcSprites = Speaker A, playerSprites = Speaker B.
/// Toggle chatting via the serialized bool or SetChatEnabled(bool).
/// </summary>
public class NPCDialogueLooper : MonoBehaviour
{
    [Header("Sequence")]
    [Tooltip("Uses npcSprites for Speaker A and playerSprites for Speaker B.")]
    [SerializeField] private DialogueSequenceSO sequence;

    [Header("Renderers")]
    [Tooltip("SpriteRenderer for Speaker A.")]
    [SerializeField] private SpriteRenderer speakerA;
    [Tooltip("SpriteRenderer for Speaker B.")]
    [SerializeField] private SpriteRenderer speakerB;

    [Header("Control")]
    [Tooltip("If true, chatting starts automatically on Start().")]
    [SerializeField] private bool autoStart = false;

    [Tooltip("Master toggle to allow chatting. Can be changed at runtime.")]
    [SerializeField] private bool canChat = false;

    [Tooltip("If true, the conversation loops forever while canChat is true.")]
    [SerializeField] private bool loop = true;

    [Header("Timing")]
    [Tooltip("Extra pause between individual beats (in seconds).")]
    [SerializeField] private float pauseBetweenBeats = 0.0f;

    [Tooltip("Extra pause after the whole sequence finishes, before looping (in seconds).")]
    [SerializeField] private float pauseBetweenLoops = 0.5f;

    [Tooltip("Optional global speed multiplier for all step durations.")]
    [SerializeField][Min(0.01f)] private float speedMultiplier = 1.0f;

    [Header("Cleanup")]
    [Tooltip("Clear both sprites when chat stops or component is disabled.")]
    [SerializeField] private bool clearSpritesOnStop = true;

    private Coroutine chatRoutine;
    private Sprite _aOriginal;
    private Sprite _bOriginal;

    private void Start()
    {
        // Cache originals (in case these renderers are shared)
        if (speakerA != null) _aOriginal = speakerA.sprite;
        if (speakerB != null) _bOriginal = speakerB.sprite;

        if (autoStart)
            SetChatEnabled(true);
    }

    private void OnDisable()
    {
        StopChatInternal();
    }

    /// <summary>
    /// Public setter to match your pattern: enable or disable chatting.
    /// </summary>
    public void SetChatEnabled(bool enable)
    {
        canChat = enable;

        if (canChat)
        {
            if (chatRoutine == null && isActiveAndEnabled)
                chatRoutine = StartCoroutine(ChatLoop());
        }
        else
        {
            StopChatInternal();
        }
    }

    private void StopChatInternal()
    {
        if (chatRoutine != null)
        {
            StopCoroutine(chatRoutine);
            chatRoutine = null;
        }

        if (clearSpritesOnStop)
        {
            if (speakerA != null) { speakerA.sprite = _aOriginal; speakerA.enabled = _aOriginal != null; }
            if (speakerB != null) { speakerB.sprite = _bOriginal; speakerB.enabled = _bOriginal != null; }
        }
    }

    private IEnumerator ChatLoop()
    {
        if (sequence == null || speakerA == null || speakerB == null)
            yield break;

        while (canChat)
        {
            int steps = Mathf.Min(
                sequence.npcSprites != null ? sequence.npcSprites.Count : 0,
                sequence.playerSprites != null ? sequence.playerSprites.Count : 0,
                sequence.durations != null ? sequence.durations.Count : 0
            );

            if (steps <= 0)
            {
                // Nothing to play, break or idle briefly
                yield return null;
                if (!loop) break;
                yield return new WaitForSeconds(0.2f);
                continue;
            }

            for (int i = 0; i < steps && canChat; i++)
            {
                // Speaker A
                if (speakerA != null)
                {
                    var sA = sequence.npcSprites[i];
                    speakerA.sprite = sA;
                    speakerA.enabled = sA != null;
                }

                // Speaker B
                if (speakerB != null)
                {
                    var sB = sequence.playerSprites[i];
                    speakerB.sprite = sB;
                    speakerB.enabled = sB != null;
                }

                float dur = sequence.durations[i] > 0f ? sequence.durations[i] : sequence.fallbackDuration;
                dur = Mathf.Max(0.01f, dur) / Mathf.Max(0.01f, speedMultiplier);

                yield return new WaitForSeconds(dur);

                if (pauseBetweenBeats > 0f)
                    yield return new WaitForSeconds(pauseBetweenBeats);
            }

            if (!loop) break;

            if (pauseBetweenLoops > 0f)
                yield return new WaitForSeconds(pauseBetweenLoops);
        }

        // End-of-loop cleanup
        if (clearSpritesOnStop)
        {
            if (speakerA != null) { speakerA.sprite = _aOriginal; speakerA.enabled = _aOriginal != null; }
            if (speakerB != null) { speakerB.sprite = _bOriginal; speakerB.enabled = _bOriginal != null; }
        }

        chatRoutine = null;
    }
}
