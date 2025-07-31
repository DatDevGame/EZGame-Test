using UnityEngine;
using Premium.EditableStateMachine;
using BoxingArena.StateMachine.Transitions;
using System.Collections.Generic;

namespace BoxingArena.StateMachine.States
{
    [CreateAssetMenu(fileName = "BoxerBlockState", menuName = "BoxingArena/States/BoxerBlockState")]
    public class BoxerBlockStateSO : BaseBoxerStateSO
    {
        [Header("Block State Settings")]
        [SerializeField] private float blockDuration = 0.5f;
        [SerializeField] private string blockAnimationTrigger = "Block";
        [SerializeField] private float blockDamageReduction = 0.8f; // 80% damage reduction

        private float currentBlockTime;
        private Animator animator;
        private BaseBoxer boxer;
        private bool isBlocking = false;

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
            currentBlockTime = 0f;
            isBlocking = true;

            if (animator != null)
            {
                animator.SetTrigger(blockAnimationTrigger);
            }

            Debug.Log($"{boxer?.name ?? "Boxer"} entered Block state");
        }

        protected override void StateUpdate()
        {
            currentBlockTime += Time.deltaTime;

            // Check if block input is still held
            if (Input.GetKey(KeyCode.B) || Input.GetMouseButton(1))
            {
                // Continue blocking
                currentBlockTime = 0f; // Reset timer while blocking
            }
            else
            {
                // Stop blocking if input is released
                if (currentBlockTime >= blockDuration)
                {
                    // Transition back to idle
                    Debug.Log("Block released - transitioning to idle state");
                }
            }
        }

        protected override void StateDisable()
        {
            isBlocking = false;
            Debug.Log($"{boxer?.name ?? "Boxer"} exited Block state");
        }

        public bool IsBlocking()
        {
            return isBlocking;
        }

        public float GetDamageReduction()
        {
            return blockDamageReduction;
        }
    }
}