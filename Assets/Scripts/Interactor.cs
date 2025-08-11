using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Interactor : MonoBehaviour
{
    [Tooltip("Called when the player presses F or ButtonEast while in range")]
    public UnityEvent OnInteract;
    [SerializeField] private string boolToControl;
    [SerializeField] private bool boolStateOnEntry;
    [SerializeField] private bool boolStateOnExit;
    [SerializeField] private Animator animator;
    [SerializeField] private bool wantToChangeScenes;

    private bool playerInRange;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (boolToControl != null && animator != null)
            {
                animator.SetBool(boolToControl, boolStateOnEntry);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (boolToControl != null && animator != null)
            {
                animator.SetBool(boolToControl, boolStateOnExit);
            }
        }
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.F) && OnInteract != null || Input.GetKeyDown(KeyCode.JoystickButton1) && OnInteract != null)
        {
            OnInteract?.Invoke();
        }
        if (wantToChangeScenes)
        {
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton1))
                SceneManager.LoadScene("City");
        }
    }
}