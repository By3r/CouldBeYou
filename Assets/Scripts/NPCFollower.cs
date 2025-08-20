using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class NPCFollower : MonoBehaviour
{
    #region Variables
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;

    [SerializeField] private float maxAcceleration = 12f;

    [SerializeField] private float arriveThreshold = 0.05f;

    [SerializeField] private float slowRadius = 0.5f;

    [SerializeField] private float xOffset = 1.5f;
    [SerializeField] private float yOffset = 0f;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private bool baseArtFacesLeft = true;

    private bool followPlayer = false;
    private Rigidbody2D rb;
    private Vector2 lastNonZeroDir = Vector2.right;

    private Vector2 homePosition;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;

        homePosition = rb.position;
    }

    private void FixedUpdate()
    {
        Vector2 targetPoint;

        if (followPlayer && player != null)
        {
            Vector2 rawTarget = player.position;
            float sidePos = (rb.position.x < player.position.x) ? -1f : 1f;
            Vector2 offset = new Vector2(xOffset * sidePos, yOffset);
            targetPoint = rawTarget + offset;

            rb.isKinematic = false;
        }
        else
        {
            targetPoint = homePosition;
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }

        Vector2 toTarget = targetPoint - rb.position;
        float distance = toTarget.magnitude;

        if (distance <= arriveThreshold)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            float speed = moveSpeed;
            if (distance < slowRadius)
            {
                float factor = distance / slowRadius;
                speed *= factor;
            }

            Vector2 desiredVel = toTarget.normalized * speed;

            if (maxAcceleration > 0f)
                rb.velocity = Vector2.MoveTowards(rb.velocity, desiredVel, maxAcceleration * Time.fixedDeltaTime);
            else
                rb.velocity = desiredVel;
        }

        if (spriteRenderer != null)
        {
            float xVelocity = rb.velocity.x != 0 ? rb.velocity.x : lastNonZeroDir.x;
            bool movingLeft = xVelocity < 0f;
            spriteRenderer.flipX = baseArtFacesLeft ? !movingLeft : movingLeft;

            if (rb.velocity.sqrMagnitude > 0.0001f)
                lastNonZeroDir = rb.velocity;
        }
    }

    public void MakeNPCFollowPlayer(bool shouldFollow)
    {
        followPlayer = shouldFollow;
    }
}