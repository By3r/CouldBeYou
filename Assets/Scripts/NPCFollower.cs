using NUnit.Framework.Internal;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class NPCFollower : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Player transform. If left empty, will try to find GameObject tagged 'Player'.")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform player;
    [SerializeField] private Transform selfTarget;

    [Header("Movement")]
    [Tooltip("Units per second while following.")]
    [SerializeField] private float moveSpeed = 3.5f;

    [Tooltip("Optional acceleration. Set to 0 to snap to speed.")]
    [SerializeField] private float maxAcceleration = 12f;

    [Header("Offsets")]
    [Tooltip("Horizontal distance from the player.")]
    [SerializeField] private float xOffset = 1.5f;

    [Tooltip("Vertical distance from the player (0 = none).")]
    [SerializeField] private float yOffset = 0f;

    [Header("Rendering")]
    [Tooltip("SpriteRenderer to flip on X when moving left or right. If empty, searched on children.")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("Tick this if your base art faces LEFT. Untick if art faces RIGHT.")]
    [SerializeField] private bool baseArtFacesLeft = true;

    private bool followPlayer = false;
    private Rigidbody2D rb;
    private Vector2 lastNonZeroDir = Vector2.right;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
    }

    private void Start()
    {
        if (target == null && followPlayer == true)
        {
            target = player;
        }
        if (followPlayer == false)
        {
            selfTarget.position = this.gameObject.transform.position;
        }
    }

    private void FixedUpdate()
    {
        if (target == null && followPlayer == false)
        {
            target = selfTarget;
            rb.velocity = Vector2.zero;
            return;
        }
        else if (followPlayer)
        {
            if (player != null)
            {
                target = player;
            }
        }

        Vector2 desiredPos = (Vector2)target.position;

        float sidePos = (rb.position.x < target.position.x) ? -1f : 1f;
        Vector2 offset = new Vector2(xOffset * sidePos, yOffset);

        desiredPos += offset;

        Vector2 toTarget = desiredPos - rb.position;
        Vector2 desiredVel = toTarget.normalized * moveSpeed;

        if (maxAcceleration > 0f)
            rb.velocity = Vector2.MoveTowards(rb.velocity, desiredVel, maxAcceleration * Time.fixedDeltaTime);
        else
            rb.velocity = desiredVel;

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