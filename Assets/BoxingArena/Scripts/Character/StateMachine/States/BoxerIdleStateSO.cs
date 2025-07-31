using UnityEngine;
using Premium.EditableStateMachine;

namespace BoxingArena.StateMachine.States
{
    [CreateAssetMenu(fileName = "BoxerIdleState", menuName = "BoxingArena/States/BoxerIdleState")]
    public class BoxerIdleStateSO : BaseBoxerStateSO
    {
        [Header("Idle State Settings")]
        [SerializeField] private float idleDuration = 2f;
        [SerializeField] private string idleAnimationTrigger = "Idle";
        
        private float currentIdleTime;
        private Animator animator;
        private BaseBoxer boxer;

        public override void SetupState(object[] parameters = null)
        {
            if (parameters != null && parameters.Length >= 2)
            {
                animator = parameters[0] as Animator;
                boxer = parameters[1] as BaseBoxer;
            }
        }

        protected override void StateEnable()
        {
            currentIdleTime = 0f;
            
            if (animator != null)
            {
                animator.SetTrigger(idleAnimationTrigger);
            }
            
            Debug.Log($"entered Idle state");
        }

        protected override void StateUpdate()
        {
            currentIdleTime += Time.deltaTime;
            
            // Check for transitions
            CheckForAttackInput();
            CheckForBlockInput();
            CheckForMoveInput();
            Debug.Log($"Concac - 1");
        }

        protected override void StateDisable()
        {
            Debug.Log($"Disable Idle state");
        }

        private void CheckForAttackInput()
        {
            // Check if attack input is pressed
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                // Trigger attack transition
                Debug.Log("Attack input detected - transitioning to attack state");
            }
        }

        private void CheckForBlockInput()
        {
            // Check if block input is pressed
            if (Input.GetKeyDown(KeyCode.B) || Input.GetMouseButtonDown(1))
            {
                // Trigger block transition
                Debug.Log("Block input detected - transitioning to block state");
            }
        }

        private void CheckForMoveInput()
        {
            // Check if movement input is pressed
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                // Trigger move transition
                Debug.Log("Movement input detected - transitioning to move state");
            }
        }
    }
} 