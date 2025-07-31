using UnityEngine;
using Premium.EditableStateMachine;
using static Premium.StateMachine.StateMachine;
using Sirenix.OdinInspector;


namespace BoxingArena.StateMachine.Transitions
{
    using Premium.StateMachine;
    [CreateAssetMenu(fileName = "BoxerTransition", menuName = "BoxingArena/Transitions/BoxerTransition")]
    public class BoxerTransitionSO : TransitionSO
    {
        public override StateMachine.State.Transition Transition
        {
            get
            {
                if (transition == null)
                {
                    transition = new StateMachine.State.Transition(new ManualEvent(), targetState.State);
                }
                return base.Transition;
            }
        }

        [Header("Transition Settings")]
        [SerializeField] private TransitionType transitionType;
        [SerializeField] private float transitionDelay = 0f;
        private float currentDelay;
        private bool isTriggered = false;

        public enum TransitionType
        {
            Manual,
            Timed,
            InputBased,
            HealthBased
        }

        public override void SetupTransition(object[] parameters)
        {
            Debug.Log($"Set Up Transition {name}");
            // Setup transition based on type
            switch (transitionType)
            {
                case TransitionType.Manual:
                    transition = new State.Transition(
                        new ManualEvent(),
                        targetState.State
                    );
                    break;

                case TransitionType.Timed:
                    transition = new State.Transition(
                        new PredicateEvent(() => currentDelay >= transitionDelay),
                        targetState.State
                    );
                    break;

                case TransitionType.InputBased:
                    transition = new State.Transition(
                        new PredicateEvent(() => isTriggered),
                        targetState.State
                    );
                    break;

                case TransitionType.HealthBased:
                    transition = new State.Transition(
                        new PredicateEvent(() => CheckHealthCondition()),
                        targetState.State
                    );
                    break;
            }
        }

        public void TriggerTransition()
        {
            isTriggered = true;
        }

        public void ResetTransition()
        {
            isTriggered = false;
            currentDelay = 0f;
        }

        private bool CheckHealthCondition()
        {
            // Example: Transition to KO state when health is low
            return false; // Implement based on your health system
        }
    }
}