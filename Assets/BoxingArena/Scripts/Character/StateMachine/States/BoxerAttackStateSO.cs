using UnityEngine;
using Premium.EditableStateMachine;

namespace BoxingArena.StateMachine.States
{
    [CreateAssetMenu(fileName = "BoxerAttackState", menuName = "BoxingArena/States/BoxerAttackState")]
    public class BoxerAttackStateSO : BaseBoxerStateSO
    {
        [Header("Attack State Settings")]
        [SerializeField] private float attackDuration = 1f;
        [SerializeField] private string attackAnimationTrigger = "Attack";
        [SerializeField] private float attackDamage = 15f;
        
        private float currentAttackTime;
        private Animator animator;
        private BaseBoxer boxer;
        private bool hasDealtDamage = false;

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
            currentAttackTime = 0f;
            hasDealtDamage = false;
            
            if (animator != null)
            {
                animator.SetTrigger(attackAnimationTrigger);
            }
            
            Debug.Log($"{boxer?.name ?? "Boxer"} entered Attack state");
        }

        protected override void StateUpdate()
        {
            currentAttackTime += Time.deltaTime;
            
            // Deal damage at the middle of the attack animation
            if (!hasDealtDamage && currentAttackTime >= attackDuration * 0.5f)
            {
                DealDamage();
                hasDealtDamage = true;
            }
            
            // Return to idle after attack completes
            if (currentAttackTime >= attackDuration)
            {
                // Transition back to idle
                Debug.Log("Attack completed - transitioning to idle state");
            }
        }

        protected override void StateDisable()
        {
            Debug.Log($"{boxer?.name ?? "Boxer"} exited Attack state");
        }

        private void DealDamage()
        {
            // Check for enemies in range and deal damage
            Collider[] hitColliders = Physics.OverlapSphere(boxer.transform.position, 2f);
            
            foreach (var hitCollider in hitColliders)
            {
                var damageable = hitCollider.GetComponent<IDamageable>();
                if (damageable != null && hitCollider.gameObject != boxer.gameObject)
                {
                    damageable.TakeDamage(attackDamage);
                    Debug.Log($"{boxer?.name ?? "Boxer"} dealt {attackDamage} damage to {hitCollider.name}");
                }
            }
        }
    }
} 