using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// - Disables specified behaviours (e.g., player movement/input)
/// - Plays a timed sequence controlling NPC and Player SpriteRenderers
/// - Restores everything when done
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class NPCInteractor : Interactor
{
    [Header("Dialogue")]
    [SerializeField] private DialogueSequenceSO sequence;

    [Header("Renderers (Targets to drive during dialogue)")]
    [SerializeField] private SpriteRenderer npcRenderer;
    [SerializeField] private SpriteRenderer playerRenderer;

    [Tooltip("Scripts to disable during dialogue.")]
    [SerializeField] private List<Behaviour> disableDuringDialogue = new List<Behaviour>();

    [Header("Events")]
    [Tooltip("Invoked when the dialogue sequence begins.")]
    public UnityEvent OnDialogueStarted;
    [Tooltip("Invoked when the dialogue sequence finishes (successfully or aborted).")]
    public UnityEvent OnDialogueFinished;

    [Header("Extra Options!")]
    [Tooltip("Restore the original sprites on both renderers after dialogue ends.")]
    [SerializeField] private bool restoreOriginalSpritesOnEnd = true;

    [Tooltip("Prevent re-trigger while a sequence is already playing.")]
    [SerializeField] private bool blockReentryWhilePlaying = true;

    private Sprite _npcOriginalSprite;
    private Sprite _playerOriginalSprite;
    private bool _isPlaying;
    private Coroutine _playRoutine;

    private void OnEnable()
    {
        if (OnInteract != null)
            OnInteract.AddListener(HandleInteract);
    }

    private void OnDisable()
    {
        if (OnInteract != null)
            OnInteract.RemoveListener(HandleInteract);
    }

    private void HandleInteract()
    {
        if (sequence == null)
        {
            return;
        }

        if (_isPlaying && blockReentryWhilePlaying)
            return;

        if (_playRoutine == null)
            _playRoutine = StartCoroutine(PlayDialogueRoutine());
    }

    private IEnumerator PlayDialogueRoutine()
    {
        _isPlaying = true;

        if (npcRenderer != null) _npcOriginalSprite = npcRenderer.sprite;
        if (playerRenderer != null) _playerOriginalSprite = playerRenderer.sprite;

        SetScriptsEnabled(false);

        OnDialogueStarted?.Invoke();

        int steps = Mathf.Min(
            sequence.npcSprites != null ? sequence.npcSprites.Count : 0,
            sequence.playerSprites != null ? sequence.playerSprites.Count : 0,
            sequence.durations != null ? sequence.durations.Count : 0
        );

        if (steps == 0)
        {
            yield return null;
            goto END;
        }

        for (int i = 0; i < steps; i++)
        {
            if (npcRenderer != null)
            {
                npcRenderer.sprite = sequence.npcSprites[i];
                npcRenderer.enabled = sequence.npcSprites[i] != null;
            }

            if (playerRenderer != null)
            {
                playerRenderer.sprite = sequence.playerSprites[i];
                playerRenderer.enabled = sequence.playerSprites[i] != null;
            }

            float t = sequence.durations[i] > 0f ? sequence.durations[i] : sequence.fallbackDuration;
            yield return new WaitForSeconds(t);
        }

    END:
        if (restoreOriginalSpritesOnEnd)
        {
            if (npcRenderer != null)
            {
                npcRenderer.sprite = _npcOriginalSprite;
                npcRenderer.enabled = _npcOriginalSprite != null;
            }
            if (playerRenderer != null)
            {
                playerRenderer.sprite = _playerOriginalSprite;
                playerRenderer.enabled = _playerOriginalSprite != null;
            }
        }
        else
        {
            if (npcRenderer != null) npcRenderer.enabled = npcRenderer.sprite != null;
            if (playerRenderer != null) playerRenderer.enabled = playerRenderer.sprite != null;
        }

        SetScriptsEnabled(true);
        OnDialogueFinished?.Invoke();

        _isPlaying = false;
        _playRoutine = null;
    }

    private void SetScriptsEnabled(bool enabled)
    {
        if (disableDuringDialogue == null) return;
        for (int i = 0; i < disableDuringDialogue.Count; i++)
        {
            var scripts = disableDuringDialogue[i];
            if (scripts == null) continue;
            scripts.enabled = enabled;
        }
    }
}
