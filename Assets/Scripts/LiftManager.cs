using Interactibles.WheelChairs;
using UnityEngine;

namespace Interactibles.Lift
{
    public class LiftManager : MonoBehaviour
    {
        #region Variables
        [SerializeField] private WheelChair wheelchair;
        [SerializeField] private Animator elevatorAnimator;

        private bool elevatorIsUp = true;
        #endregion

        private void Awake()
        {
            if (elevatorAnimator != null)
                elevatorIsUp = elevatorAnimator.GetBool("isUp");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (wheelchair == null || elevatorAnimator == null)
            {
                Debug.LogWarning("Missing reference!", this);
                return;
            }

            if (wheelchair.transform.parent != other.transform)
                return;

            elevatorIsUp = !elevatorIsUp;   

            elevatorAnimator.SetBool("isUp", elevatorIsUp); 
        }
    }
}