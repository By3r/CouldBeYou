using UnityEngine;

namespace Player.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement2D : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float speed = 5f;
        private float originalSpeed;
        private Rigidbody2D rigidBody;
        private float horizontalInput;

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Animation")]
        [SerializeField] private Animator animator;

        [Tooltip("Input threshold to consider the player walking.")]
        [SerializeField] private float walkThreshold = 0.01f;
        #endregion

        private void Awake()
        {
            originalSpeed = speed;

            rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
            rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (animator == null) animator = GetComponent<Animator>();
        }

        private void Update()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");

            if (spriteRenderer != null)
            {
                if (horizontalInput > walkThreshold)
                    spriteRenderer.flipX = false; 
                else if (horizontalInput < -walkThreshold)
                    spriteRenderer.flipX = true; 
            }

            if (animator != null)
            {
                bool isWalking = Mathf.Abs(horizontalInput) > walkThreshold;
                animator.SetBool("isPlayerWalking", isWalking);
            }
        }

        private void FixedUpdate()
        {
            Vector2 newVelocity = new Vector2(horizontalInput * speed, rigidBody.velocity.y);
            rigidBody.velocity = newVelocity;
        }

        public void ModifySpeed(float multiplier)
        {
            speed = originalSpeed * multiplier;
        }

        public void ResetSpeed()
        {
            speed = originalSpeed;
        }
    }
}