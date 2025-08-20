using UnityEngine;
using UnityEngine.SceneManagement;

public class LightCharger : MonoBehaviour
{
    #region Variables
    [SerializeField] private string playerTag = "Player";

    [SerializeField] private bool forRestaurant = false;

    [Header("Arrow Sprites")]
    [Tooltip("Sprite when the arrow becomes active.")]
    [SerializeField] private Sprite lightArrow;
    [Tooltip("Sprite when the arrow is idle.")]
    [SerializeField] private Sprite darkArrow;

    [Header("Lights Renderer")]
    [SerializeField] private SpriteRenderer lightRenderer;

    [Header("Light Sprites for progress")]
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite orangeSprite;
    [SerializeField] private Sprite yellowSprite;
    [SerializeField] private Sprite paleGreenSprite;
    [SerializeField] private Sprite greenSprite;


    [Header("Timing")]
    [Tooltip("Seconds required to move from one light colour to the next.")]
    [SerializeField] private float stepSeconds = 2f;

    private SpriteRenderer arrowRenderer;
    private Collider2D triggerColider;

    private bool playerInside;
    private float stayTimer;
    private int colourStateIndex;

    private const int MaxState = 4;
    #endregion

    private void Reset()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.isTrigger = true;
    }

    private void Awake()
    {
        arrowRenderer = GetComponent<SpriteRenderer>();
        triggerColider = GetComponent<Collider2D>();

        if (triggerColider != null)
        {
            triggerColider.isTrigger = true;
        }

        if (arrowRenderer != null && darkArrow != null)
        {
            arrowRenderer.sprite = darkArrow;
        }

        SetState(0, force: true);
    }

    private void Update()
    {
        if (!playerInside) return;

        stayTimer += Time.deltaTime;

        int targetState = Mathf.Clamp(Mathf.FloorToInt(stayTimer / stepSeconds), 0, MaxState);
        if (targetState != colourStateIndex)
        {
            SetState(targetState);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInside = true;
        stayTimer = 0f;
        SetState(0, force: true);

        if (arrowRenderer != null && lightArrow != null)
        {
            arrowRenderer.sprite = lightArrow;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInside = false;
        stayTimer = 0f;
        SetState(0, force: true);

        if (arrowRenderer != null && darkArrow != null)
        {
            arrowRenderer.sprite = darkArrow;
        }
    }

    #region Private Functions
    private void SetState(int newIndex, bool force = false)
    {
        if (!force && newIndex == colourStateIndex) return;

        colourStateIndex = Mathf.Clamp(newIndex, 0, MaxState);

        if (lightRenderer == null) return;

        switch (colourStateIndex)
        {
            case 0:
                if (redSprite != null) lightRenderer.sprite = redSprite;
                break;
            case 1:
                if (orangeSprite != null) lightRenderer.sprite = orangeSprite;
                break;
            case 2:
                if (yellowSprite != null) lightRenderer.sprite = yellowSprite;
                break;
            case 3:
                if (paleGreenSprite != null) lightRenderer.sprite = paleGreenSprite;
                break;
            case 4:
                if (greenSprite != null)
                {
                    lightRenderer.sprite = greenSprite;
                    Invoke("LoadNextScene", 0.5f);
                }
                break;
        }
    }

    private void LoadNextScene()
    {
        if (forRestaurant == false)
        {
            SceneManager.LoadScene(3);
        }
        else
        {
            SceneManager.LoadScene(5);
        }
    }
    #endregion
}