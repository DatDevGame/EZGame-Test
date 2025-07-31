using UnityEngine;
using BoxingArena.StateMachine.States;
using BoxingArena.StateMachine.Transitions;

namespace BoxingArena.StateMachine
{
    /// <summary>
    /// Ví dụ cách sử dụng StateMachine cho Boxing Game
    /// </summary>
    public class BoxerStateMachineExample : MonoBehaviour
    {
        [Header("State Machine Components")]
        [SerializeField] private BoxerStateMachineController stateMachineController;
        [SerializeField] private BaseBoxer boxer;
        [SerializeField] private Animator animator;

        [Header("States")]
        [SerializeField] private BoxerIdleStateSO idleState;
        [SerializeField] private BoxerAttackStateSO attackState;
        [SerializeField] private BoxerBlockStateSO blockState;
        [SerializeField] private BoxerMoveStateSO moveState;
        [SerializeField] private BoxerKOStateSO koState;

        [Header("Transitions")]
        [SerializeField] private BoxerTransitionSO idleToAttackTransition;
        [SerializeField] private BoxerTransitionSO idleToBlockTransition;
        [SerializeField] private BoxerTransitionSO idleToMoveTransition;
        [SerializeField] private BoxerTransitionSO attackToIdleTransition;
        [SerializeField] private BoxerTransitionSO blockToIdleTransition;
        [SerializeField] private BoxerTransitionSO moveToIdleTransition;

        private void Start()
        {
            SetupStateMachine();
        }

        private void SetupStateMachine()
        {
            // Setup states with parameters
            object[] stateParams = new object[] { animator, boxer };
            
            idleState.SetupState(stateParams);
            // attackState.SetupState(stateParams);
            // blockState.SetupState(stateParams);
            // moveState.SetupState(stateParams);
            // koState.SetupState(stateParams);

            // Setup transitions
            SetupTransitions();

            // Start the state machine
            stateMachineController.StartBoxing();
        }

        private void SetupTransitions()
        {
            // Setup transition parameters
            object[] transitionParams = new object[] { };

            idleToAttackTransition.SetupTransition(transitionParams);
            idleToBlockTransition.SetupTransition(transitionParams);
            idleToMoveTransition.SetupTransition(transitionParams);
            attackToIdleTransition.SetupTransition(transitionParams);
            blockToIdleTransition.SetupTransition(transitionParams);
            moveToIdleTransition.SetupTransition(transitionParams);
        }

        private void Update()
        {
            // Handle input for manual transitions
            HandleInput();
        }

        private void HandleInput()
        {
            // Example input handling
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Trigger attack transition
                idleToAttackTransition.TriggerTransition();
            }
            
            if (Input.GetKeyDown(KeyCode.B))
            {
                // Trigger block transition
                idleToBlockTransition.TriggerTransition();
            }
            
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                // Trigger move transition
                idleToMoveTransition.TriggerTransition();
            }
        }

        /// <summary>
        /// Ví dụ cách tạo StateMachine programmatically
        /// </summary>
        [ContextMenu("Create State Machine Programmatically")]
        private void CreateStateMachineProgrammatically()
        {
            // Tạo states
            var idleStateInstance = ScriptableObject.CreateInstance<BoxerIdleStateSO>();
            var attackStateInstance = ScriptableObject.CreateInstance<BoxerAttackStateSO>();
            
            // Setup states
            object[] stateParams = new object[] { animator, boxer };
            idleStateInstance.SetupState(stateParams);
            attackStateInstance.SetupState(stateParams);
            
            // Tạo transitions
            var transitionInstance = ScriptableObject.CreateInstance<BoxerTransitionSO>();
            transitionInstance.SetupTransition(new object[] { });
            
            // Tạo StateMachine controller
            var controller = new Premium.StateMachine.StateMachine.Controller();
            
            // Start với idle state
            controller.Start(idleStateInstance.State);
            
            Debug.Log("State Machine created programmatically!");
        }

        /// <summary>
        /// Ví dụ cách sử dụng StateMachine với events
        /// </summary>
        private void SetupStateEvents()
        {
            // Subscribe to state events
            idleState.State.StateStarted += (state) => Debug.Log("Idle state started");
            idleState.State.StateEnded += (state) => Debug.Log("Idle state ended");
            
            attackState.State.StateStarted += (state) => Debug.Log("Attack state started");
            attackState.State.StateEnded += (state) => Debug.Log("Attack state ended");
        }

        /// <summary>
        /// Ví dụ cách tạo custom transition conditions
        /// </summary>
        private void CreateCustomTransitions()
        {
            // Tạo custom predicate event
            var healthCheckEvent = new Premium.StateMachine.StateMachine.PredicateEvent(() => 
            {
                return boxer.GetBoxerStats().Health <= 0;
            });
            
            // Tạo transition từ bất kỳ state nào đến KO state
            var koTransition = new Premium.StateMachine.StateMachine.State.Transition(
                healthCheckEvent,
                koState.State
            );
            
            Debug.Log("Custom KO transition created!");
        }
    }
} 