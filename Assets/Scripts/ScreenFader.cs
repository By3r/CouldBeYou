using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Fades a full-screen UI Image to black, then loads scene index 0.
/// Place this on a GameObject with a full-screen Image, or assign the Image in the Inspector.
/// </summary>
[RequireComponent(typeof(Image))]
public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1.0f;

    [SerializeField] private AnimationCurve ease = AnimationCurve.Linear(0, 0, 1, 1);

    [SerializeField] private bool blockInputDuringFade = true;
    [SerializeField] private int sceneIndexToLoad = 0;

    private void Reset()
    {
        fadeImage = GetComponent<Image>();
    }

    private void Awake()
    {
        if (fadeImage == null)
            fadeImage = GetComponent<Image>();

        if (fadeImage != null)
        {
            fadeImage.enabled = true; 
            fadeImage.raycastTarget = blockInputDuringFade;
        }
    }
    public void StartFade()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        StopAllCoroutines();
        StartCoroutine(FadeOutThenLoad());
    }

    private System.Collections.IEnumerator FadeOutThenLoad()
    {
        if (fadeImage == null)
        {
            yield break;
        }

        bool originalRaycast = fadeImage.raycastTarget;
        fadeImage.raycastTarget = blockInputDuringFade;

        Color colour = fadeImage.color;
        float startAlpha = colour.a;
        float targetAlpha = 1f; 

        float time = 0f;
        float duration = Mathf.Max(0.0001f, fadeDuration);

        while (time < 1f)
        {
            time += Time.unscaledDeltaTime / duration;
            float k = ease != null ? ease.Evaluate(Mathf.Clamp01(time)) : Mathf.Clamp01(time);
            colour.a = Mathf.Lerp(startAlpha, targetAlpha, k);
            fadeImage.color = colour;
            yield return null;
        }

        colour.a = targetAlpha;
        fadeImage.color = colour;

        SceneManager.LoadScene(sceneIndexToLoad);
        fadeImage.raycastTarget = originalRaycast;
    }
}