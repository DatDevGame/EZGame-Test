using UnityEngine;
using Premium.EditableStateMachine;
using BoxingArena.StateMachine.States;
using Unity.VisualScripting;
using Sirenix.OdinInspector;
using Premium.StateMachine;

namespace BoxingArena.StateMachine
{
    using BoxingArena.StateMachine.Transitions;
    using Premium.StateMachine;
    using static Premium.StateMachine.StateMachine.State;

    public class BoxerStateMachineController : EditableStateMachineController
    {
        [Header("Boxer References")]
        [SerializeField] private BaseBoxer boxer;
        [SerializeField] private Animator animator;
        protected override void Awake()
        {
            base.Awake();
            if (boxer == null)
                boxer = GetComponent<BaseBoxer>();

            if (animator == null)
                animator = GetComponent<Animator>();

            StartBoxing();
        }

        public void StartBoxing()
        {
            SetUpTransitions();
            StartStateMachine();
        }

        public void StopBoxing()
        {
            StopStateMachine();
        }

        [Button]
        protected virtual void SetUpTransitions()
        {
            states.ForEach((v) =>
            {
                object[] stateParams = new object[] { animator, boxer };
                v.SetupState(stateParams);

                BaseBoxerStateSO baseBoxerState = v as BaseBoxerStateSO;
                if (baseBoxerState != null)
                {
                    baseBoxerState.TransitionSO.ForEach(x => x.SetupTransition(new object[] { }));
                }
            });
        }

        protected override void Update()
        {
            if (stateMachineController == null) return;
            if (isRunning == false) return;
            stateMachineController.Update();
        }

        [Button]
        public void ChangeState(StateSO stateSO)
        {
            // Trigger attack transition
            StateSO currentStateSO = states.Find(v => v.State == stateMachineController.CurrentState);
            BaseBoxerStateSO baseBoxerStateSO = currentStateSO as BaseBoxerStateSO;

            if (baseBoxerStateSO != null)
            {
                SetUpTransitions();
                TransitionSO NextTransitionSO = baseBoxerStateSO.TransitionSO.Find(x => x.Transition.TargetState == stateSO.State);
                stateMachineController.StateChanged(states[1].State);
                SetUpTransitions();
            }
        }

    }
}
