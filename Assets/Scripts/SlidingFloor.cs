using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SlidiingFloor : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Only bodies with this tag are affected.")]
    public string targetTag = "Player";

    [Header("Belt Settings")]
    [Tooltip("Desired extra leftward speed while on the belt (units/sec).")]
    public float maxBeltSpeed = 3f;

    [Tooltip("How quickly the belt nudges the body toward the target speed (units/sec^2).")]
    public float beltAcceleration = 25f;

    private float deltaTime => Time.fixedDeltaTime;

    [Header("Box Check (Top Sensor)")]
    [Tooltip("Vertical thickness of the sensor box that sits just above the floor's top.")]
    public float sensorHeight = 0.12f;

    [Tooltip("How far above the colliderr top to place the sensor (prevents self-overlap).")]
    public float sensorOffset = 0.02f;

    [Tooltip("Which layers can contain targets. Keep this to just the Player layer if possible.")]
    public LayerMask targetLayers = ~0;

    [Header("Debug")]
    public bool drawGizmos = true;

    // cache
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void Reset()
    {
        Collider2D collider = GetComponent<Collider2D>();
        collider.isTrigger = false;
    }

    private void OnValidate()
    {
        var c = GetComponent<Collider2D>();
        if (c) c.isTrigger = false;
    }

    private void FixedUpdate()
    {
        if (col == null) return;

        // Build a thin box that hugs the top of the floor
        var b = col.bounds;
        Vector2 size = new Vector2(b.size.x, sensorHeight);
        Vector2 center = new Vector2(b.center.x, b.max.y + sensorOffset + sensorHeight * 0.5f);

        // Look for any collider inside the sensor
        var hits = Physics2D.OverlapBoxAll(center, size, 0f, targetLayers);
        if (hits == null || hits.Length == 0) return;

        for (int i = 0; i < hits.Length; i++)
        {
            var other = hits[i];
            if (other == null) continue;
            if (!other.CompareTag(targetTag)) continue;

            var rb = other.attachedRigidbody;
            if (rb == null) continue;

            // Apply leftward belt force to approach -maxBeltSpeed
            float currentVx = rb.velocity.x;

            float desiredDeltaV = Mathf.Clamp((-maxBeltSpeed) - currentVx,
                                              -beltAcceleration * deltaTime,
                                               beltAcceleration * deltaTime);

            float neededAx = desiredDeltaV / deltaTime;
            float forceX = rb.mass * neededAx;

            rb.AddForce(new Vector2(forceX, 0f), ForceMode2D.Force);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        var c = GetComponent<Collider2D>();
        if (c == null) return;

        var b = c.bounds;
        Vector2 size = new Vector2(b.size.x, sensorHeight);
        Vector2 center = new Vector2(b.center.x, b.max.y + sensorOffset + sensorHeight * 0.5f);

        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }
}
