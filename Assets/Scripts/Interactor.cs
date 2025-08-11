using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Interactor : MonoBehaviour
{
    #region Variables
    [Tooltip("Called when the player presses F or ButtonEast while in range")]
    public UnityEvent OnInteract;

    [SerializeField] private GameObject interactSprite;

    [SerializeField] private string boolToControl;
    [SerializeField] private string sceneName;

    [SerializeField] private bool boolStateOnEntry;
    [SerializeField] private bool boolStateOnExit;
    [SerializeField] private bool wantToChangeScenes;

    [SerializeField] private Animator animator;

    private bool playerInteracted = false;
    private bool playerInRange;
    #endregion

    private void Reset()
    {
        var collider = GetComponent<Collider2D>();
        collider.isTrigger = true;
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
        if (!playerInRange)
        {
            if (interactSprite != null)
            {
                interactSprite.SetActive(false);
            }
            return;
        }

        if (interactSprite != null && interactSprite.activeSelf != true && playerInteracted == false)
        {
            interactSprite.SetActive(true);
            playerInteracted = true;
        }

        if (Input.GetKeyDown(KeyCode.F) && OnInteract != null || Input.GetKeyDown(KeyCode.JoystickButton1) && OnInteract != null)
        {
            OnInteract?.Invoke();
        }
        if (wantToChangeScenes)
        {
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton1))
                SceneManager.LoadScene(sceneName);
        }
    }
}