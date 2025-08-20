using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PressurePlate : MonoBehaviour
{
    [Header("Door Reference")]
    [Tooltip("Animator that controls the door.")]
    [SerializeField] private Animator doorAnimator;

    [Tooltip("The trigger parameter name in the Animator to open the door.")]
    [SerializeField] private string doorTriggerName = "Open";

    [Header("Filter")]
    [Tooltip("Only objects with this tag can activate the plate. Leave empty to accept all.")]
    [SerializeField] private string requiredTag = "NPC";

    private void Reset()
    {
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (string.IsNullOrEmpty(requiredTag) || other.CompareTag(requiredTag))
        {
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger(doorTriggerName);
            }
        }
    }
}
