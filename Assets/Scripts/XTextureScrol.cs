using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Renderer))]
public class XTextureScrol : MonoBehaviour
{
    #region Variables
    [Tooltip("Speed to scroll the texture in UV units per second")]
    [SerializeField] private float scrollSpeed = 0.5f;

    [Tooltip("Name of the scene to load after delay")]
    [SerializeField] private string sceneName;

    [Tooltip("Time in seconds before the new scene loads")]
    [SerializeField] private float delayBeforeLoad = 8f;

    private Renderer rend;
    private Vector2 currentOffset;

    private float timer = 0f;
    private bool hasLoaded = false;
    #endregion

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        currentOffset = rend.material.mainTextureOffset;
        currentOffset.x += scrollSpeed * Time.deltaTime;
        rend.material.mainTextureOffset = currentOffset;

        if (!hasLoaded)
        {
            timer += Time.deltaTime;
            if (timer >= delayBeforeLoad)
            {
                hasLoaded = true;
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}