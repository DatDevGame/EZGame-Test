using UnityEngine;
using Premium.EditableStateMachine;

namespace BoxingArena.StateMachine.States
{
    [CreateAssetMenu(fileName = "BoxerMoveState", menuName = "BoxingArena/States/BoxerMoveState")]
    public class BoxerMoveStateSO : BaseBoxerStateSO
    {
        [Header("Move State Settings")]
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private string moveAnimationTrigger = "Move";
        [SerializeField] private float rotationSpeed = 5f;
        
        private Animator animator;
        private BaseBoxer boxer;
        private CharacterController characterController;
        private Vector3 moveDirection;

        public override void SetupState(object[] parameters = null)
        {
            if (parameters != null && parameters.Length >= 2)
            {
                animator = parameters[0] as Animator;
                boxer = parameters[1] as BaseBoxer;
                characterController = boxer?.GetComponent<CharacterController>();
            }
        }

        protected override void StateEnable()
        {
            if (animator != null)
            {
                animator.SetTrigger(moveAnimationTrigger);
            }
            
            Debug.Log($"entered Move state Move");
        }

        protected override void StateUpdate()
        {
            Debug.Log($"Concac - 2");
        }

        protected override void StateDisable()
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
            
            Debug.Log($"exited Move state");
        }
    }
} 