using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    public class EnemyDeadState : EnemyBaseState
    {
        private readonly NavMeshAgent agent;
        private readonly Transform player;
        public EnemyDeadState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
        {
            this.agent = agent;
            this.player = player;
        }
        public override void OnEnter()
        {
            animator.CrossFade(DieHash, crossFadeDuration);
            enemy.DeadTimerStart();
        }

        public override void Update()
        {
            enemy.Dead();
            
        }
        
    }
}