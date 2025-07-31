using UnityEngine;
using Premium.EditableStateMachine;

namespace BoxingArena.StateMachine.States
{
    [CreateAssetMenu(fileName = "BoxerKOState", menuName = "BoxingArena/States/BoxerKOState")]
    public class BoxerKOStateSO : BaseBoxerStateSO
    {
        [Header("KO State Settings")]
        [SerializeField] private string koAnimationTrigger = "KO";
        [SerializeField] private float koDuration = 3f;
        
        private float currentKOTime;
        private Animator animator;
        private BaseBoxer boxer;
        private bool isKO = false;

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
            currentKOTime = 0f;
            isKO = true;
            
            if (animator != null)
            {
                animator.SetTrigger(koAnimationTrigger);
            }
            
            Debug.Log($"{boxer?.name ?? "Boxer"} entered KO state - GAME OVER!");
        }

        protected override void StateUpdate()
        {
            currentKOTime += Time.deltaTime;
            
            // Stay in KO state for the duration
            if (currentKOTime >= koDuration)
            {
                // Could transition to a "revive" state or end the game
                Debug.Log("KO duration completed - boxer remains unconscious");
            }
        }

        protected override void StateDisable()
        {
            isKO = false;
            Debug.Log($"{boxer?.name ?? "Boxer"} exited KO state");
        }

        public bool IsKO()
        {
            return isKO;
        }

        public float GetKOTime()
        {
            return currentKOTime;
        }
    }
} 