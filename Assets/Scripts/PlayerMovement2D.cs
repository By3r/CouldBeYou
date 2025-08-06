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
        #endregion

        private void Awake()
        {
            originalSpeed = speed;

            rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
            rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        private void Update()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
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
