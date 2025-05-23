using Unity.VisualScripting;
using UnityEngine;

namespace Platformer
{
    public abstract class EnemyBaseState : IState
    {
        protected readonly Enemy enemy;
        protected readonly Animator animator;

        
        protected static readonly int IdleHash = Animator.StringToHash("IdleNormal");
        protected static readonly int RunHash = Animator.StringToHash("RunFWD");
        protected static readonly int WalkHash = Animator.StringToHash("WalkFWD");
        protected static readonly int AttackHash = Animator.StringToHash("Mushroom_Attack01Smile");
        protected static readonly int DieHash = Animator.StringToHash("Die");
        
        protected const float crossFadeDuration = 0.1f;

        protected EnemyBaseState(Enemy enemy, Animator animator)
        {
            this.enemy = enemy;
            this.animator = animator;
        }
        
        
        public virtual void OnEnter()
        {
            
        }

        public virtual void Update()
        {
            
        }

        public virtual void FixedUpdate()
        {
            
        }

        public virtual void OnExit()
        {
            
        }
    }
}