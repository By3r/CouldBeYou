using UnityEngine;
using UnityEngine.UI;

public class XTextureScroll : MonoBehaviour
{
    #region Variables
    [Tooltip("Speed to scroll the texture in UV units per second")]
    [SerializeField] private float scrollSpeed = 0.5f;

    [SerializeField] private Renderer rendererr;
    [SerializeField] private Image image;
    private Vector2 currentOffset;
    #endregion

    private void Update()
    {
        if (rendererr != null && image == null)
        {
            currentOffset = rendererr.material.mainTextureOffset;
            currentOffset.x += scrollSpeed * Time.deltaTime;
            rendererr.material.mainTextureOffset = currentOffset;
        }
        if (image != null && rendererr == null)
        {
            currentOffset = image.material.mainTextureOffset;
            currentOffset.x += scrollSpeed * Time.deltaTime;
            image.material.mainTextureOffset = currentOffset;
        }
    }
}