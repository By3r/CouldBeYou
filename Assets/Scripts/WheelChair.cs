using UnityEngine;
using Player.Movement;

namespace Interactibles.WheelChairs
{
    public class WheelChair : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float speedMultiplier = 0.3f;
        [SerializeField] private GameObject dialogueImage;

        private Rigidbody2D rb;
        private bool isPickedUp;
        private GameObject playerInRange;
        private GameObject carrier;
        private Transform originalParent;
        private Vector3 dropPosition;
        #endregion

        private void Awake()
        {
            originalParent = transform.parent;
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isPickedUp && other.CompareTag("Player"))
            {
                playerInRange = other.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!isPickedUp && other.gameObject == playerInRange)
            {
                playerInRange = null;
            }
        }

        private void Update()
        {
            if (!isPickedUp && playerInRange != null && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton1)))
            {
                PickUp(playerInRange);
                if (dialogueImage is not null && dialogueImage.activeSelf == true)
                {
                    dialogueImage.SetActive(false);
                }
            }
            else if (isPickedUp && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton1)))
            {
                Drop();
            }
        }

        private void PickUp(GameObject who)
        {
            isPickedUp = true;
            carrier = who;

            transform.SetParent(carrier.transform, worldPositionStays: true);
            transform.localPosition = Vector3.zero;

            if (carrier.TryGetComponent<PlayerMovement2D>(out var playerMovement))
                playerMovement.ModifySpeed(speedMultiplier);
        }

        private void Drop()
        {
            isPickedUp = false;

            dropPosition = transform.position;
            transform.SetParent(originalParent, worldPositionStays: true);
            transform.position = dropPosition;

            if (carrier != null && carrier.TryGetComponent<PlayerMovement2D>(out var playerMovement))
                playerMovement.ResetSpeed();

            carrier = null;
            playerInRange = null;
        }
    }
}